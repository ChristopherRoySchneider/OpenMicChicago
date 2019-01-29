using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenMicChicago.Models {
    public class User {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MinLength (2)]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", 
         ErrorMessage = "Characters are not allowed.")]
        public string FirstName { get; set; }

        [Required]
        [MinLength (2)]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", 
         ErrorMessage = "Characters are not allowed.")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType (DataType.Password)]
        [MinLength (8)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", 
         ErrorMessage = "Min length: 8 characters, min 1 letter, 1 number and 1 special character")]
        public string Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        [Required]
        [DataType (DataType.Password)]
        
        [Compare ("Password")]

        public string ConfirmPassword { get; set; }

        public List<OpenMic> OpenMics { get; set; }
        public List<Venue> Venues { get; set; }
        public List<Like> Likes { get; set; }

        public string PhotoURL { get; set; }

    }
}