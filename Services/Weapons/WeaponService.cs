using System.Security.Claims;
using dotnet_rpg.Data;
using dotnet_rpg.DTOs.Character;
using dotnet_rpg.DTOs.Weapon;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.Weapons;

public class WeaponService: IWeaponService
{
    private readonly DataContext _dataContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public WeaponService(DataContext dataContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _dataContext = dataContext;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }
    
    public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
    {
        var response = new ServiceResponse<GetCharacterDto>();

        try
        {
            var character = await _dataContext.Characters.FirstOrDefaultAsync(character =>
                character.Id
                == newWeapon.CharacterId && character.User!.Id == GetAuthUserId()
            );

            if (character is null)
            {
                throw new Exception("Character not found!");
            }

            var weapon = _mapper.Map<Weapon>(newWeapon);
            weapon.Character = character;

            _dataContext.Weapons.Add(weapon);
            await _dataContext.SaveChangesAsync();
            
            response.IsSuccess = true;
            response.Message = "Weapon was successfully added";
            response.Data = _mapper.Map<GetCharacterDto>(character);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = e.Message;
        }

        return response;
    }
    
    private int GetAuthUserId()
    {
        var claims = _httpContextAccessor.HttpContext!.User;
        // we set the user if to NameIdentifier claimType
        // claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        var nameIdentifier = claims.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return int.Parse(nameIdentifier);
    }
}