using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenMicChicago.Models {
    public class OpenMicGenres {
        [Key]
        public int OpenMicGenreID { get; set; }

        public int GenreID { get; set; }
        public int OpenMicID  { get; set; }
        public Genre Genre  { get; set; }
        public OpenMic OpenMic  { get; set; }
        
    }
}
