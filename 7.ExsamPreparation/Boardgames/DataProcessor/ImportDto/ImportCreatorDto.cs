using Boardgames.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType("Creator")]
    public class ImportCreatorDto
    {
        [MaxLength(7)]
        [MinLength(2)]
        [Required]
        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        [MaxLength(7)]
        [MinLength(2)]
        [Required]
        [XmlElement("LastName")]
        public string LastName { get; set; }
        [XmlArray("Boardgames")]
        public ImportBoardGameDto[] Boardgames { get; set; }

    }
}
