using ServiceStack;
using MyApp.ServiceModel;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace MyApp.ServiceInterface;

public class DeleteServices(UserDbContext context) : Service
{
    private readonly UserDbContext _context = context;
    public async Task<object> AnyAsync(DeleteRequest request)
    {
        try
        {
            var userToDelete = await _context.Users
                    .Include(user => user.Address)
                    .ThenInclude(address => address.Geo)
                    .Include(user => user.Company).FirstOrDefaultAsync(user => user.Id == request.Id);

            if (userToDelete != null)
            {
                // Remove the user from the list
                _context.Geos.Remove(userToDelete.Address.Geo);
                _context.Addresses.Remove(userToDelete.Address);
                _context.Companies.Remove(userToDelete.Company);
                _context.Users.Remove(userToDelete);

                await _context.SaveChangesAsync();
                return new MessageResponse { Result = $"Success: user {request.Id} deleted" };
            }
            return new MessageResponse { Result = $"Error: user {request.Id} not found" };
        }
        catch (Exception ex)
        {
            return new MessageResponse { Result = $"Error: {ex.Message}" };
        }

    }
}