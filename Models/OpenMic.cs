using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OpenMicChicago.Models {
    public class OpenMic {

        [Key]
        public int OpenMicID { get; set; }

        [Required]

        public string Type { get; set; }

        [Required]
        [MinLength (2)]
        public String Title { get; set; }

        [Column (TypeName = "text")]
        [Required]
        [MinLength (10)]
        public string Description { get; set; }

        [Required]
        [DateInTheFuture]
        public DateTime DateTime { get; set; }

        [Required]
        public DateTime Signup { get; set; }

        public User Creator { get; set; }

        [Required]

        public int Duration { get; set; }

        [Required]

        public string DurationUnit { get; set; }

        public DateTime EndDateTime {
            get {
                if (this.DurationUnit == "Minutes") {
                    return this.DateTime.AddMinutes (this.Duration);
                } else if (this.DurationUnit == "Hours") {
                    return this.DateTime.AddHours (this.Duration);
                } else if (this.DurationUnit == "Days") {
                    return this.DateTime.AddDays (this.Duration);
                } else {
                    return this.DateTime;
                }
            }
            protected set { }

        }
        [Required]
        
        [Column(TypeName="VARCHAR(50)")]
       
        public String Frequency { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<Like> Likes { get; set; }

        [Required]
        [Range (1, 999999999999999, ErrorMessage = "Please Create a Venue First")]
        public int VenueID { get; set; }

        public Venue Venue { get; set; }

        public string PhotoURL { get; set; }

        public List<OpenMicGenre> Genres { get; set; }

    }

    [AttributeUsage (AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DateInTheFutureAttribute : ValidationAttribute {
        protected override ValidationResult IsValid (object value, ValidationContext context) {
            var futureDate = value as DateTime?;
            var memberNames = new List<string> () { context.MemberName };

            if (futureDate != null) {
                if (futureDate.Value < DateTime.Now) {
                    return new ValidationResult ("This must be a date in the future", memberNames);
                }
            }

            return ValidationResult.Success;
        }
    }

}