namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ExportDtos;
    using Medicines.DataProcessor.ImportDtos;
    using Medicines.Utilities;
    using Newtonsoft.Json;
  
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml.Linq;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            XmlHelper xmlParser= new XmlHelper();
            var patients = context.Patients
                .Where(p=>p.PatientsMedicines.Any(pm=>pm.Medicine.ProductionDate> DateTime.Parse(date)))
                .Select(p=>new ExsportPatientsDto
                {
                    Name=p.FullName,
                    Gender=p.Gender.ToString(),
                    AgeGroup=p.AgeGroup.ToString(),
                    Medicines=p.PatientsMedicines
                    .Where(p => p.Medicine.ProductionDate > DateTime.Parse(date))
                    .OrderByDescending(p => p.Medicine.ExpiryDate)
                    .ThenBy(p => p.Medicine.Price)
                    .Select(p => new ExportMedicineDto() 
                    {
                        Category = p.Medicine.Category.ToString(),
                        Name = p.Medicine.Name,
                        Price = decimal.Parse( p.Medicine.Price.ToString("F2")),
                        Producer = p.Medicine.Producer,
                        ExpiryDate = p.Medicine.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)


                    }).ToList()
                })
                .OrderByDescending(p => p.Medicines.Count)
                .ThenBy(p => p.Name)
                .ToArray();

            return xmlParser.Serialize(patients, "Patients");

        }

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            var medicines = context.Medicines
                .Where(m => (int)m.Category == medicineCategory
                    && m.Pharmacy.IsNonStop == true)
                 .Select(m => new
                 {
                     Name = m.Name,
                     Price = m.Price.ToString("0,00"),
                     Pharmacy = new
                     {
                         Name = m.Pharmacy.Name,
                         PhoneNumber = m.Pharmacy.PhoneNumber
                     }
                 })
                 .OrderBy(m => m.Price)
                 .ThenBy(m => m.Name)
                .ToList();

            return JsonConvert.SerializeObject(medicines);

        }
    }
}
