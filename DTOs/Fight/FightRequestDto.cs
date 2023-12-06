namespace dotnet_rpg.DTOs.Fight;

public class FightRequestDto
{
    public List<int> CharacterIds { get; set; } = new();
}