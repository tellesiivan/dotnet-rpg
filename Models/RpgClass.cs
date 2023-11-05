using System.Text.Json.Serialization;

namespace dotnet_rpg.Models;

// showcase the names values of our enum in our schemas
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RpgClass
{
    Knight = 1,
    Mage = 2,
    Cleric = 3
}
