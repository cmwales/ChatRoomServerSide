using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocialMix.Models.Models
{
    public class Person
    {
        [Required]
        public int PersonId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Country { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string State { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public int Age { get; set; }
    }
}
