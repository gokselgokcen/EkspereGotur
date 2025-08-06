using EkspereGotur.Data;
using EkspereGotur.Models;
using EkspereGotur.Dtos;
using EkspereGotur.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;



namespace EkspereGotur.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }




    // GET: api/users
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.Name,
                u.Surname,
                u.GSM,
                Roles= u.UserRoles.Select(ur => ur.Role.RoleName)
            })
            .ToListAsync();

        return Ok(users);
    }



    // POST: api/auth/register  
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already registered.");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = request.Password, // d�z metin � test i�in  
            Name = request.Name,
            Surname = request.Surname,
            GSM = request.GSM,
            UserRoles = new List<UserRole>
           {
               new UserRole { RoleId = 1 } // varsay�lan olarak "user"  
           }
        };

        _context.Users.Add(user); // Fixed the issue by replacing 'context' with '_context'  
        await _context.SaveChangesAsync();

        return Ok(new { message = "Registration successful", user.Id, user.Email });
    }

    [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
{
    var user = await _context.Users
        .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u =>
            u.Email == request.Email && 
            u.PasswordHash == request.Password);

    if (user == null)
        return Unauthorized("Invalid credentials.");

    // Kullanıcının rollerini çek
    var roles = user.UserRoles.Select(ur => ur.Role.RoleName);

    // Yeni imzalı token—rolleri de geçiyoruz
    var token = _jwtService.GenerateToken(user.Id, user.Email, roles);

    return Ok(new { token });
}



    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.Name,
            user.Surname,
            Roles = user.UserRoles.Select(r => r.Role.RoleName)
        });
    }

   
    



}
