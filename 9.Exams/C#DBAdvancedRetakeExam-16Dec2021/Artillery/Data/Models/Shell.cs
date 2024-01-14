

using System.ComponentModel.DataAnnotations;

namespace Artillery.Data.Models
{
    public class Shell
    {
        public Shell()
        {
            Guns = new List<Gun>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(1680)]
        public double ShellWeight  { get; set; }

        [Required]
        [MaxLength(30)]
        public string Caliber  { get; set; }

        public List<Gun> Guns { get; set; }
    }
}
