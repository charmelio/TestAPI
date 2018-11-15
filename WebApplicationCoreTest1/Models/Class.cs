using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationCoreTest1.Models
{
    public partial class Class
    {
        public ICollection<Character> Character { get; set; }

        [Key]
        public int ClassId { get; set; }

        public string ClassName { get; set; }

        public Class()
        {
            Character = new HashSet<Character>();
        }
    }
}