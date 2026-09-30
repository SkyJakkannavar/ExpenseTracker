import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExpenseService, Expense } from '../../../core/expense.service';
import { AuthService } from '../../../core/auth.service';
import { Router } from '@angular/router';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { ExpenseDialogComponent } from '../expense-dialog/expense-dialog.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatToolbarModule, MatDialogModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  expenses: any[] = [];
  displayedColumns: string[] = ['title', 'amount', 'date', 'actions'];
  
  private expenseService = inject(ExpenseService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private dialog = inject(MatDialog);

  ngOnInit() {
    this.loadExpenses();
  }

  loadExpenses() {
    this.expenseService.getExpenses().subscribe({
      next: (data) => {
        this.expenses = data.map(e => ({
          ...JSON.parse(e.encryptedData),
          id: e.id,
          createdAt: e.createdAt
        }));
      },
      error: (err) => console.error(err)
    });
  }

  openAddDialog() {
    const dialogRef = this.dialog.open(ExpenseDialogComponent, {
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const expensePayload: Expense = {
          encryptedData: JSON.stringify(result)
        };
        this.expenseService.addExpense(expensePayload).subscribe(() => this.loadExpenses());
      }
    });
  }

  openEditDialog(expense: any) {
    const dialogRef = this.dialog.open(ExpenseDialogComponent, {
      width: '400px',
      data: { title: expense.title, amount: expense.amount, date: expense.date }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const expensePayload: Expense = {
          encryptedData: JSON.stringify(result)
        };
        this.expenseService.updateExpense(expense.id, expensePayload).subscribe(() => this.loadExpenses());
      }
    });
  }

  deleteExpense(id: number) {
    if (confirm('Are you sure you want to delete this expense?')) {
      this.expenseService.deleteExpense(id).subscribe(() => this.loadExpenses());
    }
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
