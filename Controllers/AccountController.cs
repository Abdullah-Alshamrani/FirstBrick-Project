/*
Developer: Abdullah Alshamrani
FirstBrick Project
VillaCapital
*/



using FirstBrickAPI.Data;
using FirstBrickAPI.Models;
using FirstBrickAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
// importing services folder 
using FirstBrickAPI.Services;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{

    
    // Database context for querying and updating user data.
    private readonly FirstBrickContext _context;
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly RabbitMqProducer _rabbitMqProducer;


    public AccountController(IConfiguration configuration, FirstBrickContext context, RabbitMqProducer rabbitMqProducer)
    {
        _context = context;
        _jwtKey = configuration["JwtSettings:Key"] ?? throw new ArgumentNullException("JwtSettings:Key");
        _jwtIssuer = configuration["JwtSettings:Issuer"] ?? throw new ArgumentNullException("JwtSettings:Issuer");
        _jwtAudience = configuration["JwtSettings:Audience"] ?? throw new ArgumentNullException("JwtSettings:Audience");
        _rabbitMqProducer = rabbitMqProducer;
    }
    // POST: api/account/login - Allows users to authenticate and receive a JWT token.

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            return BadRequest(new { Message = "Username and password are required." });

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null || user.PasswordHash != request.Password)
            return Unauthorized(new { Message = "Invalid username or password." });

        var token = JwtTokenGenerator.GenerateToken(
            user.UserId,
            user.Username,
            user.FullName,
            user.Role,
            _jwtKey,
            _jwtIssuer,
            _jwtAudience
        );

        return Ok(new { Token = token });
    }
    // Allows users to create an account. POST 

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
        if (existingUser != null)
            return Conflict(new { Message = "Username already exists." });

        user.Role = string.IsNullOrEmpty(user.Role) ? "User" : user.Role;
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // assync event to RabbitMQq
        var message = $"NEW user registered: {user.Username} - Role: {user.Role}";
        _rabbitMqProducer.SendMessage(message); // message sent asycn

        return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
    }


    // This retrive all users (ADMINs) only.
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _context.Users
                .Include(u => u.Projects)
                .ToListAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error retrieving users", Details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _context.Users
            .Include(u => u.Projects)
            .FirstOrDefaultAsync(u => u.UserId == id);

        if (user == null)
            return NotFound(new { Message = $"User with ID {id} not found." });

        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserRequest request)
    {
        if (id <= 0)
        {
            return BadRequest(new { Message = "Invalid user ID." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        if (user == null)
        {
            return NotFound(new { Message = $"User with ID {id} not found." });
        }

        var userIdFromClaims = GetUserIdFromClaims();
        if (userIdFromClaims != id)
        {
            return Forbid("You are not authorized to update this user's profile.");
        }

        // Updating properties.
        user.FullName = request.FullName ?? user.FullName;
        user.Email = request.Email ?? user.Email;

        if (!string.IsNullOrEmpty(request.Password))
        {
            user.PasswordHash = request.Password; 
        }

        user.UpdatedAt = DateTime.UtcNow;

        // Save changes to the database.
        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var message = $"User Profile Updates:  {user.Username} (ID: {user.UserId})";
            _rabbitMqProducer.SendMessage(message); //sent asych


            return Ok(new { Message = "User profile updated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error updating user profile.", Details = ex.Message });
        }
    }

    // Helper function to get User ID from claims.
    private int GetUserIdFromClaims()
    {
        if (HttpContext.User.Identity is not ClaimsIdentity identity)
            throw new UnauthorizedAccessException("No identity found in the current user context.");

        var userIdClaim = identity.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid or missing user ID claim.");

        return userId;
    }
}

// DTO for UpdateUserRequest. payload structure
public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; } // I had to add ? because of a refrence problem in new versions of .NET core
}

// Helper LoginRequest class.
public class LoginRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}
