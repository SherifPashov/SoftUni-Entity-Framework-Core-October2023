using Medicines.Data.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.DataProcessor.ImportDtos
{
    public class ImportPatientDto
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        [JsonProperty("FullName")]
        
        public string FullName { get; set; }

        [Required]
        [JsonProperty("AgeGroup")]
        public string AgeGroup { get; set; }

        [Required]
        [JsonProperty("Gender")]
        public string Gender { get; set; }
        [JsonProperty("Medicines")]
        public int[] Madicines { get; set; }

    }
}
