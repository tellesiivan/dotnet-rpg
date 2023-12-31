using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Models;

namespace dotnet_rpg.DTOs
{
    public class AddCharacterDto
    {
        public string Name { get; set; } = "Wes";
        public int HitPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Intellegence { get; set; } = 10;
        public RpgClass Class { get; set; } = RpgClass.Cleric;
    }
}
