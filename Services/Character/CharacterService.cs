using System.Security.Claims;
using dotnet_rpg.Data;
using dotnet_rpg.DTOs;
using dotnet_rpg.DTOs.Character;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.Character
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetCharacters()
        {

            ServiceResponse<List<GetCharacterDto>> serviceResponse =
                new() { Data = await GetMappedCharactersList(), IsSuccess = true, };

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var character =
                await _dataContext.Characters
                    .Include(character => character.Skills)
                    .Include(character => character.Weapon)
                    .FirstOrDefaultAsync((character => character.Id == id && 
                    character.User!.Id == GetAuthUserId()));

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
            Models.Character? matchedCharacter = _dataContext.Characters.FirstOrDefault(
                character => character.Id == id  && 
                             character.User!.Id == GetAuthUserId()
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
            Models.Character characterToAdd = _mapper.Map<Models.Character>(newCharacter);
            // add auth user to the character 
            characterToAdd.User = await GetDbUser();
            
            _dataContext.Characters.Add(characterToAdd);
            await _dataContext.SaveChangesAsync();
            
            ServiceResponse<List<GetCharacterDto>> serviceResponse =
                new() { Data = await GetMappedCharactersList(), IsSuccess = true, Message = $"Characters corresponding to userId: {characterToAdd.User!.Id}"};

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(
            UpdateCharterDto updateCharacter
        )
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new();
            var matchedCharacter = _dataContext.Characters
                    // Might need to include related objects(Character User) first.
                .Include(character => character.User)
                .FirstOrDefault(
                character => character.Id == updateCharacter.Id
            );

            // we can also wrap this into a try-catch block and set the data, message and isSuccess = false
            if (matchedCharacter is not null || matchedCharacter?.User!.Id == GetAuthUserId())
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

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto characterSkillDto)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            try
            {

                var character = await _dataContext.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(
                    c => c.Id == characterSkillDto.CharacterId && c.User!.Id == GetAuthUserId()
                );

                if (character is null)
                {
                    throw new Exception("Character not found");
                }

                var skill =
                    await _dataContext.Skills.FirstOrDefaultAsync(s =>
                        s.Id == characterSkillDto.SkillId);
                
                if (skill is null)
                {
                    throw new Exception("Skill not found");
                }
                
                // add the skill found by id to the matched character
                character.Skills!.Add(skill);
                await _dataContext.SaveChangesAsync();

                response.Message = "Successfully added a skill to the provided character";
                response.Data = _mapper.Map<GetCharacterDto>(character);

            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }

            return response;
        }

        private async Task<User?> GetDbUser()
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(user => user.Id == GetAuthUserId());
            return user;
        }
        
        private int GetAuthUserId()
        {
            var claims = _httpContextAccessor.HttpContext!.User;
            // we set the user if to NameIdentifier claimType
            // claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            var nameIdentifier = claims.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return int.Parse(nameIdentifier);
        }
        
        private async Task<List<GetCharacterDto>> GetMappedCharactersList()
        {
            var userId = GetAuthUserId();

            var charactersAsyncList = await _dataContext.Characters.
                // only get the characters related to the auth user
                Where(c => c.User!.Id == userId).ToListAsync();
            var mappedCharacterList = charactersAsyncList.Select(character => _mapper.Map<GetCharacterDto>(character)).ToList();
            return mappedCharacterList;
        }
    }
}
