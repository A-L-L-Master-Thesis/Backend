using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Models
{
    public class Boat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ID { get; set; }
        [ForeignKey("CurrentPositionID")]
        public Position CurrentPosition { get; set; }
    }
}
