using dotnet_rpg.DTOs;
using dotnet_rpg.Models;
using dotnet_rpg.Services.CharacterService;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    // [Route("api/[controller]")]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<ServiceResponse<List<Character>>>> GetCharcters()
        {
            return Ok(await _characterService.GetCharacters());
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetCharcterDetails(int id)
        {
            Task<ServiceResponse<GetCharacterDto>> response = _characterService.GetCharacterById(
                id
            );
            return Ok(await response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<BaseResponse>> DeleteCharacterById(int id)
        {
            BaseResponse baseResponse = await _characterService.DeleteCharacterById(id);
            return baseResponse.IsSuccess ? Ok(baseResponse) : NotFound(baseResponse);
        }

        [HttpPost("Add")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(
            AddCharacterDto character
        )
        {
            return Ok(await _characterService.AddCharacter(character));
        }

        [HttpPut("Update")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharcter(
            UpdateCharterDto updateCharacter
        )
        {
            var UpdatedCharacterRes = await _characterService.UpdateCharacter(updateCharacter);

            if (UpdatedCharacterRes.Data is null)
            {
                return NotFound(UpdatedCharacterRes);
            }

            return Ok(UpdatedCharacterRes);
        }
    }
}
