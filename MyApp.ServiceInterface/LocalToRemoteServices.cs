using MyApp.ServiceModel;
using ServiceStack;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System;

namespace MyApp.ServiceInterface;

public class LocalToRemoteServices(UserDbContext context) : Service
{

    private readonly UserDbContext _context = context;
    private readonly HttpClient _client = new();

    private async Task UpdateUsersData()
    {
        List<User> usersRemote;
        List<User> usersLocal;

        usersLocal = await _context.Users
            .Include(user => user.Address)
            .ThenInclude(address => address.Geo)
            .Include(user => user.Company).ToListAsync();

        var response = await _client.GetAsync(ApiConfig.ApiUrl);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            usersRemote = JsonSerializer.Deserialize<List<User>>(content);

            foreach (var userLocal in usersLocal)
            {
                var existingUser = usersRemote.Find(user => user.Id == userLocal.Id);
                existingUser.Formedemail = User.GetFormedEmail(existingUser.Name);

                if (existingUser != null )
                {
                    if (!userLocal.Equals(existingUser))
                    {
                        // Update existing local user to remote (PUT)                    
                        string url = ApiConfig.ApiUrl;
                        string json = JsonSerializer.Serialize(userLocal);

                        Console.WriteLine($"PUT {url} HTTP/1.1");
                        Console.WriteLine($"Content-Type: application/json");
                        Console.WriteLine();
                        Console.WriteLine(json);

                        //HttpResponseMessage response = await _client.PutAsJsonAsync(url, userLocal);
                    }

                }
                else
                {
                    // Add existing loca user to remote (POST)
                    string url = ApiConfig.ApiUrl;
                    string json = JsonSerializer.Serialize(userLocal);

                    Console.WriteLine($"POST {url} HTTP/1.1");
                    Console.WriteLine($"Content-Type: application/json");
                    Console.WriteLine();
                    Console.WriteLine(json);

                    //await _client.PostAsJsonAsync(url, userLocal);
                }
            }
        }
    }

    public async Task<object> AnyAsync(LocalToRemoteRequest request)
    {
        try
        {
            await UpdateUsersData();
            return new MessageResponse { Result = $"Success: Local DB synced to Remote API!" };
        }
        catch (Exception ex)
        {
            return new MessageResponse { Result = $"Error: {ex.Message}" };
        }

    }
}