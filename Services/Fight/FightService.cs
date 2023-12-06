using dotnet_rpg.Data;
using dotnet_rpg.DTOs.Fight;
using dotnet_rpg.DTOs.Skill;
using dotnet_rpg.Models;
using Microsoft.IdentityModel.Tokens;
using Exception = System.Exception;
using CharacterModel = dotnet_rpg.Models.Character;

namespace dotnet_rpg.Services.Fight;

public class FightService: IFightService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public FightService(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
    {
        var response = new ServiceResponse<AttackResultDto>();

        try
        {
            var attacker = await _dataContext.Characters
                .Include(character => character.Weapon)
                .FirstOrDefaultAsync(character => character.Id == request.AttackerId);
            
            var opponent = await _dataContext.Characters
                .FirstOrDefaultAsync(character => character.Id == request.OpponentId);


            if (attacker is null || opponent is null || attacker.Weapon is null)
            {
                throw new Exception("Oops ran into an error regarding the required information");
            }

            var damage = DoWeaponAttack(attacker, opponent);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            } else if (opponent.HitPoints <= 0)
            {
                response.Message = $"{opponent.Name} has been defeated";
            }

            await _dataContext.SaveChangesAsync();

            response.Data = new AttackResultDto()
            {
                AttackerName = attacker.Name,
                AttackerHp = attacker.HitPoints,
                Damage = damage,
                OpponentHp = opponent.HitPoints,
                OpponentName = opponent.Name
            };
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = e.Message;
        }

        return response;
    }

    private static int DoWeaponAttack(CharacterModel attacker, CharacterModel opponent)
    {
        var damage = attacker.Weapon.Damage + new Random().Next(attacker.Strength);
        damage -= new Random().Next(opponent.Defense);
        return damage;
    }


    public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto attackDto)
    {
        
        var response = new ServiceResponse<AttackResultDto>();

        try
        {
            var attacker = await _dataContext.Characters
                .Include(character => character.Skills)
                .FirstOrDefaultAsync(character => character.Id == attackDto.AttackerId);
            
            var opponent = await _dataContext.Characters
                .FirstOrDefaultAsync(character => character.Id == attackDto.OpponentId);


            if (attacker is null || opponent is null || attacker.Skills is null)
            {
                throw new Exception("Oops ran into an error regarding the required information");
            }

            var matchedAttackerSkill =
                attacker.Skills.FirstOrDefault(skill => skill.Id == attackDto.AttackerId);

            if (matchedAttackerSkill is null)
            {
                throw new Exception($"{attacker.Name} does not know that skill");
            }
            

            var damage = DoSkillAttack(matchedAttackerSkill, attacker, opponent);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }
            else if (opponent.HitPoints <= 0)
            {
                response.Message = $"{opponent.Name} has been defeated";
            }

            await _dataContext.SaveChangesAsync();

            response.Data = new AttackResultDto()
            {
                AttackerName = attacker.Name,
                AttackerHp = attacker.HitPoints,
                Damage = damage,
                OpponentHp = opponent.HitPoints,
                OpponentName = opponent.Name
            };
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = e.Message;
        }

        return response;
        
    }

    private static int DoSkillAttack(
        Skill matchedAttackerSkill,
        CharacterModel attacker,
        CharacterModel opponent
    )
    {
        var damage = matchedAttackerSkill.Damage + new Random().Next(attacker.Intellegence);
        damage -= new Random().Next(opponent.Defeats);

        if (damage > 0)
        {
            opponent.HitPoints -= damage;
        }
        
        return damage;
    }

    public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequestDto)
    {
        var response = new ServiceResponse<FightResultDto>()
        {
            Data = new FightResultDto()
        };

        try
        {
            var characters = await _dataContext.Characters
                .Include(character => character.Weapon)
                .Include(character => character.Skills)
                .Where(character => fightRequestDto.CharacterIds.Contains(character.Id))
                .ToListAsync();


            if (characters.IsNullOrEmpty())
            {
                throw new Exception("There was no characters found");
            }

            bool defeated = false;

            while (!defeated)
            {
                foreach (var attackerCharacter in characters)
                {
                    var ops = characters.Where(character => character.Id != attackerCharacter.Id).ToList();
                    var opponent = ops[new Random().Next(ops.Count)];

                    int damage = 0;
                    string attackUsed = string.Empty;

                    bool useWeapon = new Random().Next(2) == 0;

                    if (useWeapon && attackerCharacter.Weapon is not null)
                    {
                        attackUsed = attackerCharacter.Weapon.Name;
                        damage = DoWeaponAttack(attackerCharacter, opponent);
                        
                    }
                    else if (!useWeapon && attackerCharacter.Skills is not null)
                    {
                        var skill =
                            attackerCharacter.Skills[
                                new Random().Next(attackerCharacter.Skills.Count)];
                        attackUsed = skill.Name;

                        damage = DoSkillAttack(skill, attackerCharacter, opponent );
                        

                    }
                    else
                    {
                        response.Data.Log
                            .Add($"{attackerCharacter.Name} wasn't able to attack!!");
                    }
                    
                    response.Data.Log
                        .Add($"{attackerCharacter.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage");

                    if (opponent.HitPoints <= 0)
                    {
                        defeated = true;
                        attackerCharacter.Victories++;
                        opponent.Defeats++;
                        response.Data.Log
                            .Add($"{opponent.Name} has been defeated");
                        response.Data.Log
                            .Add($"{attackerCharacter.Name} wins with {attackerCharacter.HitPoints} HP remaining!");
                        break;
                    }
                    
                }
            }
            characters.ForEach(c =>
            {
                c.Fights++;
                c.HitPoints = 100;
            });
            
            await _dataContext.SaveChangesAsync();

        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = e.Message;
        }

        return response;  
    }

    public async Task<ServiceResponse<List<HighScoreDto>>> GetHighscores()
    {
        var response = new ServiceResponse<List<HighScoreDto>>();
        
        try
        {
            var characters = await _dataContext.Characters
                .Where(character => character.Fights > 0)
                .OrderByDescending(character => character.Victories)
                .ThenBy(character => character.Defeats)
                .ToListAsync();
            
            if (characters.IsNullOrEmpty())
            {
                throw new Exception("There was no characters found");
            }
            response.IsSuccess = true;
            response.Data = characters.Select(character => _mapper.Map<HighScoreDto>(character))
                .ToList();

        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = e.Message;
        }

        return response;
    }
}