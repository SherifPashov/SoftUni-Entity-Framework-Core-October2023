


using Artillery.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Artillery.Data.Models
{
    public class Gun
    {
        public Gun()
        {
            CountriesGuns = new List<CountryGun>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        public int ManufacturerId  { get; set; }
        [ForeignKey(nameof(ManufacturerId))]
        public Manufacturer Manufacturer { get; set; }

        [Required]
        [MaxLength(1350000)]
        public int GunWeight { get; set; }

        [Required]
        [MaxLength(35)]
        public double BarrelLength { get; set; }

        public int? NumberBuild  { get; set; }

        [Required]
        [MaxLength(100000)]
        public int Range  { get; set; }

        [Required]
        public GunType GunType { get; set; }
        public int ShellId  { get; set; }
        [ForeignKey(nameof(ShellId))]
        public Shell Shell { get; set; }
        public List<CountryGun> CountriesGuns  { get; set; }
    }
}
