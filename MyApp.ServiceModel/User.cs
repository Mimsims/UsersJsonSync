using ServiceStack;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System;

namespace MyApp.ServiceModel;

[Route("/user")]
public class UserRequest : IReturn<MessageResponse>
{
   public string Name { get; set; }
}

[Route("/remotetolocal")]
public class RemoteToLocalRequest : IReturn<MessageResponse>
{
}

[Route("/localtoremote")]
public class LocalToRemoteRequest : IReturn<MessageResponse>
{
}

[Route("/updatefield")]
[Route("/updatefield/{Email}")]
public class UpdateFieldRequest : IReturn<MessageResponse>
{
    public string Email { get; set; }
}

[Route("/delete")]
[Route("/delete/{Id}")]
public class DeleteRequest : IReturn<MessageResponse>
{
    public int Id { get; set; }
}

public class MessageResponse
{
    public string Result { get; set; }
}


public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("address")]
    public Address Address { get; set; }
    [JsonPropertyName("phone")]
    public string Phone { get; set; }
    [JsonPropertyName("website")]
    public string Website { get; set; }
    [JsonPropertyName("company")]
    public Company Company { get; set; }
    public string Formedemail { get; set; }

    public static string GetFormedEmail(string name)
    {
        char firstLetterFirstName;
        string lastname;
        string[] names = name.Split(' ');
        switch (names.Length)
        {
            case 3:
                if (names[0].Contains('.'))
                {
                    firstLetterFirstName = names[1][0];
                    lastname = names[2];
                }
                else
                {
                    firstLetterFirstName = names[0][0];
                    lastname = names[1];
                }
                break;
            default:
                firstLetterFirstName = names[0][0];
                lastname = names[1];
                break;
        }


        return $"{firstLetterFirstName}{lastname}@ibsat.com";
    }

    public static void SetInitialValues(User user)
    {
        user.Address.Id = user.Id;
        user.Address.Geo.Id = user.Id;
        user.Company.Id = user.Id;
        user.Formedemail = GetFormedEmail(user.Name);
    }

    public override bool Equals(object obj)
    {
        try
        {
            User other = (User)obj;

            return Id == other.Id &&
                   Name == other.Name &&
                   Username == other.Username &&
                   Email == other.Email &&
                   Address.Equals(other.Address) &&
                   Phone == other.Phone &&
                   Website == other.Website &&
                   Company.Equals(other.Company) &&
                   Formedemail == other.Formedemail;
        } catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }

    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Username, Email, Address, Phone, Website, Company, Formedemail);
    }

}
public class Address
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("street")]
    public string Street { get; set; }
    [JsonPropertyName("suite")]
    public string Suite { get; set; }
    [JsonPropertyName("city")]
    public string City { get; set; }
    [JsonPropertyName("zipcode")]
    public string Zipcode { get; set; }
    [JsonPropertyName("geo")]
    public Geo Geo { get; set; }
    public override bool Equals(object obj)
    {
        try
        {
            Address other = (Address)obj;

            return  City == other.City &&
                    Street == other.Street &&
                    Suite == other.Suite &&
                    Geo.Equals(other.Geo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Street, Suite, City, Zipcode, Geo);
    }
}

public class Geo
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("lat")]
    public string Lat { get; set; }
    [JsonPropertyName("lng")]
    public string Lng { get; set; }
    public override bool Equals(object obj)
    {
        try
        {
            Geo other = (Geo)obj;

            return Lat == other.Lat &&
                   Lng == other.Lng;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Lat, Lng);
    }
}

public class Company
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("catchPhrase")]
    public string CatchPhrase { get; set; }
    [JsonPropertyName("bs")]
    public string Bs { get; set; }
    public override bool Equals(object obj)
    {
        try
        {
            Company other = (Company)obj;

            return Name == other.Name &&
                CatchPhrase == other.CatchPhrase &&
                Bs == other.Bs;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    public override int GetHashCode() { 
        return HashCode.Combine(Id, Name, CatchPhrase, Bs); 
    }

}