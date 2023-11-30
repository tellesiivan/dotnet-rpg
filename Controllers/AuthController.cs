using dotnet_rpg.DTOs.User;
using dotnet_rpg.Models;
using dotnet_rpg.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController: ControllerBase
{
    private readonly IAuthRepository _authService;

    public AuthController(IAuthRepository authRepository)
    {
        _authService = authRepository;
    }


    [Route("Registration")]
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<int>>> Register([FromBody]UserRegisterDto user)
    {
        var response = await _authService.Register(new User()
        {
            Username = user.Username
        }, user.Password);

        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
    
}