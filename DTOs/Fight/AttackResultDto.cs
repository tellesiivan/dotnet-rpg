namespace dotnet_rpg.DTOs.Fight;

public class AttackResultDto
{
    public string AttackerName { get; set; } = string.Empty;
    public string OpponentName { get; set; } = string.Empty;
    public int AttackerHp { get; set; }
    public int OpponentHp { get; set; }
    public int Damage { get; set; }
}