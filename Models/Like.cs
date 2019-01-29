using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenMicChicago.Models {
    public class Like {
        [Key]
        public int LikeID { get; set; }

        public int UserID { get; set; }
        public int OpenMicID  { get; set; }
        public User User  { get; set; }
        public OpenMic OpenMic  { get; set; }
        
    }
}
