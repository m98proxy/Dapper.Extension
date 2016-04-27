using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dapper
{
    [Table("STAR_SHIP")]
    public class StarShip
    {
        [Key]
        [Column("ID")]
        public Guid Code { get; set; }

        public string Serial { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        public string Pilot { get; set; }        
    }
}
