using System.ComponentModel.DataAnnotations;

namespace WebApplicationCoreTest1.Models
{
    public partial class Character
    {
        public int Agility { get; set; }

        public int CharacterId { get; set; }

        public Class Class { get; set; }

        public int ClassId { get; set; }

        public int Dexterity { get; set; }

        public long? Experience { get; set; }

        public int Intellegence { get; set; }

        public int Level { get; set; }

        [Required]
        public string Name { get; set; }

        public int Strength { get; set; }

        public int Vitality { get; set; }
    }
}