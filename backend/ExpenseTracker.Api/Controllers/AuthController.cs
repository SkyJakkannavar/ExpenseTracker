using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.DTOs.Auth;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthController(
        IUserRepository userRepository,
        IPasswordHasherService passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest("User already exists.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var newUser = await _userRepository.CreateUserAsync(request.Email, passwordHash);

        var token = _tokenService.GenerateToken(newUser);

        return Ok(new AuthResponse { Token = token, Email = newUser.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = _tokenService.GenerateToken(user);

        return Ok(new AuthResponse { Token = token, Email = user.Email });
    }
}
