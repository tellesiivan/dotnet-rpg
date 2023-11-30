using dotnet_rpg.Data;
using dotnet_rpg.Models;
using Cryptography = System.Security.Cryptography;
using Encoding = System.Text.Encoding;

namespace dotnet_rpg.Services.Auth;

public class AuthRepository: IAuthRepository
{
    private readonly DataContext _dataContext;

    public AuthRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        var response = new ServiceResponse<int>();

        if (await UserExists(user.Username))
        {
            response.IsSuccess = false;
            response.Message = "User is already registered";
            return response;
        }
        
        CreatePasswordHash(password, out var passwordHash,
            out var passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        
        _dataContext.Users.Add(user);
        await _dataContext.SaveChangesAsync();

        response.Data = user.Id;
        response.IsSuccess = true;
        response.Message = "User was successfully registered";
        
        return response;
    }

    public async Task<ServiceResponse<string>> Login(string username, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UserExists(string username)
    {
        return await _dataContext.Users.AnyAsync((user => user.Username.ToLower() == username.ToLower()));
    }

    private bool IsEqual(string string1, string string2) {
        return string.Equals(string1, string2, StringComparison.OrdinalIgnoreCase);
    }
    
    private void CreatePasswordHash(
        string password,
        // out parameter in C# is used to pass arguments to methods by reference.
        out byte[] passwordHash,
        out byte[] passwordSalt
    )
    {
        var encodedPassword = Encoding.UTF8.GetBytes(password);
        
        var hmac = new Cryptography.HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(encodedPassword);
    }
}