

using Cadastre.Data.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace Cadastre.Data.Models
{
    public class District
    {
        public District()
        {
            Properties = new List<Property>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string Name { get; set; }

        [Required]
        [MaxLength(8)]
        public string PostalCode  { get; set; }

        [Required]
        public Region Region { get; set; }

        public ICollection<Property> Properties  { get; set; }
    }
}
