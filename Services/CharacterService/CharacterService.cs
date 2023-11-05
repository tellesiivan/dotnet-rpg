using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.DTOs;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        public static readonly List<Character> characters =
            new()
            {
                new Character(),
                new Character()
                {
                    Name = "Wayne",
                    Class = RpgClass.Mage,
                    Id = 5
                }
            };

        private readonly IMapper _mapper;

        public CharacterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetCharacters()
        {
            List<GetCharacterDto> mappedData = MapUserData();

            ServiceResponse<List<GetCharacterDto>> serviceResponse =
                new() { Data = mappedData, IsSuccess = true, };

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            Character? character = characters.FirstOrDefault((a) => a.Id == id);

            ServiceResponse<GetCharacterDto> serviceResponse =
                new()
                {
                    Data = character is not null ? _mapper.Map<GetCharacterDto>(character) : null,
                    IsSuccess = true,
                };

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(
            AddCharacterDto newCharacter
        )
        {
            // map our new character to the corrrect model
            Character characterToAdd = _mapper.Map<Character>(newCharacter);
            // find the max id value
            int currentIdValue = characters.Max((character) => character.Id);
            // set the new character id value to the max id value + 1
            characterToAdd.Id = currentIdValue + 1;
            // add it into the characters list
            characters.Add(characterToAdd);

            List<GetCharacterDto> mappedData = MapUserData();

            ServiceResponse<List<GetCharacterDto>> serviceResponse =
                new() { Data = mappedData, IsSuccess = true, };

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdataChacter(
            UpdateCharterDto updateCharacter
        )
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new();
            var matchedCharacter = characters.FirstOrDefault(
                character => character.Id == updateCharacter.Id
            );

            // we can also wrap this into a trycatch block and set the data, message and isSuccess = false
            if (matchedCharacter is not null)
            {
                matchedCharacter.Name = updateCharacter.Name;
                matchedCharacter.Strength = updateCharacter.Strength;
                matchedCharacter.Defense = updateCharacter.Defense;
                matchedCharacter.Class = updateCharacter.Class;
                matchedCharacter.Intellegence = updateCharacter.Intellegence;

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(matchedCharacter);
            }
            else
            {
                serviceResponse.Data = null;
                serviceResponse.Message =
                    $"There was no user found with the following id: {updateCharacter.Id}";
                serviceResponse.IsSuccess = false;
            }

            return serviceResponse;
        }

        private List<GetCharacterDto> MapUserData()
        {
            // return it back into a list in order to satisfy our response type
            return characters.Select(character => _mapper.Map<GetCharacterDto>(character)).ToList();
        }
    }
}
