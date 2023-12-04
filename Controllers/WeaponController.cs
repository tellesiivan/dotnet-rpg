using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.DTOs.Character;
using dotnet_rpg.DTOs.Weapon;
using dotnet_rpg.Models;
using dotnet_rpg.Services.Weapons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService _weaponService;

        public WeaponController(IWeaponService weaponService)
        {
            _weaponService = weaponService;
        }

        [HttpPost]
        [Route("Addweapon")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddWeapon([FromBody]
            AddWeaponDto weaponDto
        )
        {
            var response = await _weaponService.AddWeapon(weaponDto);
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }
        
    }
}
