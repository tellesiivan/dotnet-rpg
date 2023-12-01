using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using dotnet_rpg.Data;
using dotnet_rpg.Models;
using Microsoft.IdentityModel.Tokens;
using Cryptography = System.Security.Cryptography;
using Encoding = System.Text.Encoding;

namespace dotnet_rpg.Services.Auth;

public class AuthRepository: IAuthRepository
{
    private readonly DataContext _dataContext;
    private readonly IConfiguration _configuration;

    public AuthRepository(DataContext dataContext, IConfiguration configuration)
    {
        _dataContext = dataContext;
        _configuration = configuration;
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
        var response = new ServiceResponse<string>();
        var matchedUser = await _dataContext.Users.FirstOrDefaultAsync(user =>
            user.Username.ToLower().Equals(username.ToLower()));

        if (matchedUser is null)
        {
            response.IsSuccess = false;
            response.Message = "User not found";
            return response;
        }

        if (!VerifyPasswordHashMatch(password, matchedUser.PasswordHash, matchedUser.PasswordSalt))
        {
            response.IsSuccess = false;
            response.Message = "Please double check your password or email";
            return response;
        }

        response.Data = CreateJwtToken(matchedUser);
        response.IsSuccess = true;
        return response;
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

    private bool VerifyPasswordHashMatch(string password, byte[] passwordHash, byte[] passwordSalt)
    { 
        var encodedPassword = Encoding.UTF8.GetBytes(password);
        var hmac = new Cryptography.HMACSHA512(passwordSalt); 
        var computeHash = hmac.ComputeHash(encodedPassword);
        
        return computeHash.SequenceEqual(passwordHash);
    }

    private string CreateJwtToken(User user)
    {
        // list of claims
        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        claims.Add(new Claim(ClaimTypes.Name, user.Username));
        
        // Get the secret key from our app settings
        var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
        if (appSettingsToken is null)
        {
            throw new Exception("AppSettings token is not present");
        }

        // encode token + initiate symmetric security key
        var encodedToken = Encoding.UTF8.GetBytes(appSettingsToken);
        SymmetricSecurityKey key = new SymmetricSecurityKey(encodedToken);

        // create credentials with key and specified algorithm
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        // token descriptor(subject = claims | expires = 1 day from creation | add signing credentials)
        var tokenDescriptor = new SecurityTokenDescriptor();
        tokenDescriptor.Subject = new ClaimsIdentity(claims);
        tokenDescriptor.Expires = DateTime.Now.AddDays(1);
        tokenDescriptor.SigningCredentials = credentials;

        // create and write token
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        
        return tokenHandler.WriteToken(token);
    }
}