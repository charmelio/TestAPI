using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationCoreTest1.Models
{
    public class JsonCharacter
    {
        public string ClassName { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
    }

    public class TestModel
    {
        public long Id { get; set; }

        [DefaultValue(false)]
        public bool IsComplete { get; set; }

        [Required]
        public string Name { get; set; }
    }
}