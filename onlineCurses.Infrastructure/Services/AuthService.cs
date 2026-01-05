using Microsoft.EntityFrameworkCore;
using onlineCurses.Infrastructure.Data; 
using onlineCurses.Domain.Entities;

namespace onlineCurses.Application.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
            throw new Exception("El email ya está registrado");

        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return user;
    }
}