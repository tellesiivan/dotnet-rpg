using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.DTOs.Fight;
using dotnet_rpg.DTOs.Skill;
using dotnet_rpg.Models;
using dotnet_rpg.Services.Fight;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;

        public FightController(IFightService fightService)
        {
            _fightService = fightService;
        }

        [Route("WeaponAttack")]
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttack([FromBody] WeaponAttackDto request)
        {
            var response = await _fightService.WeaponAttack(request);
            return response.IsSuccess ? Ok(response) : StatusCode(433, response);
        }
        
        [Route("SkillAttack")]
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>>SkillAttack([FromBody] SkillAttackDto request)
        {
            var response = await _fightService.SkillAttack(request);
            return response.IsSuccess ? Ok(response) : StatusCode(433, response);
        }
        
        [Route("Fight")]
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>>Fight([FromBody] FightRequestDto request)
        {
            var response = await _fightService.Fight(request);
            return response.IsSuccess ? Ok(response) : StatusCode(433, response);
        }
        
        [Route("Highscores")]
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<HighScoreDto>>>> GetHighscores()
        {
            var response = await _fightService.GetHighscores();
            return response.IsSuccess ? Ok(response) : StatusCode(433, response);
        }
    }
}
