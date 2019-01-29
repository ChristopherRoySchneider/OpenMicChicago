using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OpenMicChicago.Models {
    public class Genre {

        [Key]
        public int GenreID { get; set; }

        [Required]
        [MinLength (2)]
        public String Name { get; set; }

        public List<OpenMicGenres> OpenMics { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        

       

    }


}