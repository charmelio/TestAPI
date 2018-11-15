using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using WebApplicationCoreTest1.Models;

namespace WebApplicationCoreTest1.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public const string CharNotFound = "Character not found.";

        /// <summary>
        /// Returns a character name from the database.
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        [HttpGet("lookup/{characterName}")]
        [Produces("application/json")]
        public IActionResult CharacterLookup(string characterName)
        {
            using (var context = new testingContext())
            {
                if (string.IsNullOrEmpty(characterName.Trim()))
                    return BadRequest(CharNotFound);

                return Ok(JsonConvert.SerializeObject(context.Character
                    .Where(x => x.Name.Contains(characterName, StringComparison.OrdinalIgnoreCase))
                    .Select(x => new { x.Name, x.Class.ClassName, x.Level })
                    .FirstOrDefault()));
            }
        }

        /// <summary>
        /// Creates a new character while letting the user set the level and class.
        /// </summary>
        /// <param name="characterName">Character name.</param>
        /// <param name="level">Character level.</param>
        /// <param name="classId">1 - Novice
        /// 2 - Rune Knight
        /// 3 - Royal Guard
        /// 4 - Warlock
        /// 5 - Sorcerer
        /// 6 - Ranger
        /// 7 - Performer
        /// 8 - Mechanic
        /// 9 - Geneticist
        /// 10 - Guillotine Cross
        /// 11 - Shadow Chaser
        /// 12 - Arch Bishop
        /// 13 - Sura
        /// 14 - Star Emperor
        /// 15 - Soul Reaper
        /// 16 - Kagerou
        /// 17 - Rebel
        /// </param>
        /// <returns></returns>
        [HttpPost("expanded/{characterName}/{level:int=1}/{classId:int=1}")]
        public IActionResult CreateExpandedCharacter(string characterName, int level = 1, int classId = 1)
        {
            using (var context = new testingContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (context.Character.Any(c => c.Name.Equals(characterName, StringComparison.OrdinalIgnoreCase)) || characterName.Trim().Equals(string.Empty))
                        {
                            transaction.Rollback();
                            return BadRequest("This character name is already taken.");
                        }

                        // Use ToString here to ignore coercion failures between Class.ClassId and this.classId
                        if (!context.Class.Any(c => c.ClassId.ToString().Equals(classId.ToString())))
                        {
                            transaction.Rollback();
                            return BadRequest("This character class does not exist.");
                        }

                        context.Character.Add(new Character()
                        {
                            Name = characterName,
                            ClassId = classId,
                            Level = level,
                            Strength = 1,
                            Agility = 1,
                            Vitality = 1,
                            Intellegence = 1,
                            Dexterity = 1,
                            Experience = 1
                        });

                        context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest($"Dont do this: {Environment.NewLine}{ex.StackTrace}");
                    }
                }

                return Content($"[{characterName}] has been created as a [{context.Class.Find(classId).ClassName}] at level [{level}]");
            }
        }

        /// <summary>
        /// Creates a new character in the database.
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        [HttpPost("create/{characterName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreateNewCharacter(string characterName)
        {
            using (var context = new testingContext())
            {
                using (var contextTransaction = context.Database.BeginTransaction())
                {
                    var command = context.Database.GetDbConnection().CreateCommand();

                    command.CommandText = "Create_New_Character";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new MySqlParameter("@CharacterName", MySqlDbType.VarChar) { Value = characterName });

                    try
                    {
                        command.ExecuteNonQuery();
                        contextTransaction.Commit();
                        return Ok($"{characterName} has been created!");
                    }
                    catch (Exception)
                    {
                        contextTransaction.Rollback();
                        return BadRequest($"There was an error creating {characterName}.");
                    }
                }
            }
        }
    }
}