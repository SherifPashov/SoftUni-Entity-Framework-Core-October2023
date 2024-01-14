

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Artillery.Data.Models
{
    public class Manufacturer
    {
        public Manufacturer()
        {
            Guns = new List<Gun>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [Unicode]
        [MaxLength(40)]
        public string ManufacturerName  { get; set; }

        [Required]
        [MaxLength(100)]
        public string Founded  { get; set; }

        public List<Gun> Guns { get; set; }
    }
}
