using dotnet_rpg.DTOs;
using dotnet_rpg.DTOs.Character;
using dotnet_rpg.DTOs.Skill;
using dotnet_rpg.DTOs.Weapon;
using dotnet_rpg.Models;

namespace dotnet_rpg
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>().ReverseMap();
            CreateMap<AddCharacterDto, Character>().ReverseMap();
            CreateMap<UpdateCharterDto, Character>().ReverseMap();
            CreateMap<AddWeaponDto, Weapon>().ReverseMap();
            CreateMap<Weapon, GetWeaponDto>().ReverseMap();
            CreateMap<Skill, GetSkillDto>().ReverseMap();
        }
    }
}
