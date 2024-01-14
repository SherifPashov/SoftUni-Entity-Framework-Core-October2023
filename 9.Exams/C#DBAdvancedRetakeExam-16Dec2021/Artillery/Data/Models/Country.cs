
using System.ComponentModel.DataAnnotations;

namespace Artillery.Data.Models
{
    public class Country
    {
        public Country()
        {
            CountriesGuns=new List<CountryGun>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(60)]
        public string CountryName  { get; set; }

        [Required]
        public int ArmySize  { get; set; }

        public List<CountryGun> CountriesGuns { get; set; }
    }
}
