using ServiceStack;
using MyApp.ServiceModel;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace MyApp.ServiceInterface;

public class UpdateFieldServices(UserDbContext context) : Service
{
    private readonly UserDbContext _context = context;
    public async Task<object> AnyAsync(UpdateFieldRequest request)
    {
        try
        {
            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                user.Email = request.Email;
            }
            _context.SaveChanges();
            return new MessageResponse { Result = $"Success: Email field updated to {users.Count} users {request.Email}" };
        }
        catch (Exception ex)
        {
            return new MessageResponse { Result = $"Error: {ex.Message}" };
        }

    }
}