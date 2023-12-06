using dotnet_rpg.DTOs.Fight;
using dotnet_rpg.DTOs.Skill;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.Fight;

public interface IFightService
{
    Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request);
    Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto attackDto);
    Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequestDto);
    Task<ServiceResponse<List<HighScoreDto>>> GetHighscores();
}