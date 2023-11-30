using dotnet_rpg.Data;
using dotnet_rpg.DTOs;
using dotnet_rpg.DTOs.Character;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;

        public CharacterService(IMapper mapper, DataContext dataContext)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetCharacters()
        {

            ServiceResponse<List<GetCharacterDto>> serviceResponse =
                new() { Data = await GetMappedCharactersList(), IsSuccess = true, };

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            Character? character =
                await _dataContext.Characters.FirstOrDefaultAsync((character => character.Id == id));

            ServiceResponse<GetCharacterDto> serviceResponse =
                new()
                {
                    Data = character is not null ? _mapper.Map<GetCharacterDto>(character) : null,
                    IsSuccess = true,
                };

            return serviceResponse;
        }

        public async Task<BaseResponse> DeleteCharacterById(int id)
        {
            BaseResponse response = new();
            Character? matchedCharacter = _dataContext.Characters.FirstOrDefault(
                character => character.Id == id
            );

            try
            {
                if (matchedCharacter is null)
                {
                    throw new Exception($"The id:{id} does not match any character id");
                }
                _dataContext.Characters.Remove(matchedCharacter);
                await _dataContext.SaveChangesAsync();
                response.IsSuccess = true;
                response.Message =
                    $"Character with the following id was successfully deleted: {id}";
            }
            catch (Exception exception)
            {
                response.IsSuccess = false;
            }

            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(
            AddCharacterDto newCharacter
        )
        {
            // map our new character to the correct model
            Character characterToAdd = _mapper.Map<Character>(newCharacter);
            _dataContext.Characters.Add(characterToAdd);
            await _dataContext.SaveChangesAsync();
            
            ServiceResponse<List<GetCharacterDto>> serviceResponse =
                new() { Data = await GetMappedCharactersList(), IsSuccess = true, };

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(
            UpdateCharterDto updateCharacter
        )
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new();
            var matchedCharacter = _dataContext.Characters.FirstOrDefault(
                character => character.Id == updateCharacter.Id
            );

            // we can also wrap this into a try-catch block and set the data, message and isSuccess = false
            if (matchedCharacter is not null)
            {
                // we are mapping the values from the new updated character to the matched character
                _mapper.Map(updateCharacter, matchedCharacter);
                await _dataContext.SaveChangesAsync();

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
        
        private async Task<List<GetCharacterDto>> GetMappedCharactersList()
        {
            var charactersAsyncList = await _dataContext.Characters.ToListAsync();
            var mappedCharacterList = charactersAsyncList.Select(character => _mapper.Map<GetCharacterDto>(character)).ToList();
            return mappedCharacterList;
        }
    }
}
