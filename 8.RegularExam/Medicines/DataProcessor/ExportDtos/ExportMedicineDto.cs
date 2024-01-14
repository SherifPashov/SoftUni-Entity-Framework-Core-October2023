using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    public class ExportMedicineDto
    {
        [XmlAttribute("category")]
        public string Category { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        
        [XmlElement("Price")]
        public decimal Price { get; set; }

        [XmlElement("Producer")]
        public string Producer { get; set; }

        [Required]
        [XmlElement("BestBefore")]
        public string ExpiryDate { get; set; }

        
        
    }
}
