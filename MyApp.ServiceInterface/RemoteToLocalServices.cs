using ServiceStack;
using MyApp.ServiceModel;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using System;
using Microsoft.EntityFrameworkCore;

namespace MyApp.ServiceInterface;

public class RemoteToLocalServices(UserDbContext context) : Service
{
    private readonly UserDbContext _context = context;
    private readonly HttpClient _client = new();

    private async Task UpdateUsersData()
    {
        List<User> remoteUsers;

        var response = await _client.GetAsync(ApiConfig.ApiUrl);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            remoteUsers = JsonSerializer.Deserialize<List<User>>(content);

            foreach (var remoteUser in remoteUsers)
            {
                var existingUser = await _context.Users
                    .Include(user => user.Address)
                    .ThenInclude(address => address.Geo)
                    .Include(user => user.Company).FirstOrDefaultAsync(user => user.Id == remoteUser.Id);

                User.SetInitialValues(remoteUser);

                if (existingUser != null)
                {
                    if (!existingUser.Equals(remoteUser))
                    {
                        // Update existing user
                        _context.Entry(existingUser).CurrentValues.SetValues(remoteUser);
                    }
                }
                else
                {
                    // Add new user
                    await _context.Users.AddAsync(remoteUser);
                }
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task<object> AnyAsync(RemoteToLocalRequest request)
    {
        try
        {
            _context.Database.EnsureCreated();
            await UpdateUsersData();
            return new MessageResponse { Result = $"Success: Remote API synced to Local DB!" };
        }
        catch (Exception ex)
        {
            return new MessageResponse { Result = $"Error: {ex.Message}" };
        }

    }
}