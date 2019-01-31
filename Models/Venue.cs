using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OpenMicChicago.Models {
    public class Venue {

        [Key]
        public int VenueID { get; set; }
        public int UserID { get; set; }
        public User Creator { get; set; }

        [Required]
        [MinLength (2)]
        public String Name { get; set; }

        [Required]
        [MinLength (2)]
        public String StreetAndNumber { get; set; }

        
        [MinLength (2)]
        public String Unit { get; set; }

        [Required]
        [MinLength (2)]
        public String City { get; set; }

        [Required]
        [MinLength (2)]
        [MaxLength (2)]
        public String State { get; set; }

        [Required]
        
        public String ZipCode { get; set; }

        [NotMapped]
        public String Address{
            get{ return $"{this.StreetAndNumber} {this.Unit}, {this.City}, {this.State} {this.ZipCode}";}
            protected set { }
        }

        [Required]
        
        public String PhoneNumber { get; set; }
        [Required]
        
        public String Website { get; set; }

        

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<OpenMic> OpenMics { get; set; }

        public string PhotoURL { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }


}