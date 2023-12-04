using dotnet_rpg.DTOs;
using dotnet_rpg.DTOs.Character;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.Character
{
    public interface ICharacterService
    {
        Task<ServiceResponse<List<GetCharacterDto>>> GetCharacters();
        Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id);
        Task<BaseResponse> DeleteCharacterById(int id);
        Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto character );
        Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharterDto updateCharacter);

        Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(
            AddCharacterSkillDto characterSkillDto
        );
    }
}
