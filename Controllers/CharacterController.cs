using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    // [Route("api/[controller]")]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        public static readonly List<Character> characters =
            new()
            {
                new Character(),
                new Character() { Name = "Wayne", Class = RpgClass.Mage }
            };

        [HttpGet("all")]
        public ActionResult<List<Character>> GetCharcters()
        {
            return Ok(characters);
        }

        [HttpGet("single/{rpgClass}")]
        public ActionResult<Character> GetCharcterDetails(RpgClass rpgClass)
        {
            Character? character = characters.FirstOrDefault((a) => a.Class == rpgClass);

            if (character is null)
            {
                return NotFound();
            }
            return Ok(character);
        }
    }
}
