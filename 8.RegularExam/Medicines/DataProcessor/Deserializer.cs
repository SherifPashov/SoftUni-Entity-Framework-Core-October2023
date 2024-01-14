namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ImportDtos;
    using Medicines.Utilities;
    using Microsoft.Extensions.Primitives;
    using Newtonsoft.Json;

    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var patientsDto = JsonConvert.DeserializeObject<ImportPatientDto[]>(jsonString);

            var patientsValid = new List<Patient>();

            foreach (var pDto in patientsDto)
            {
                if (!IsValid(pDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (!(Enum.TryParse(pDto.AgeGroup, out AgeGroup ageGroup)))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (!(Enum.TryParse(pDto.Gender, out Gender gender)))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var patient = new Patient()
                {
                    FullName = pDto.FullName,
                    AgeGroup = (AgeGroup)Enum.Parse(typeof(AgeGroup), pDto.AgeGroup),
                    Gender = (Gender)Enum.Parse(typeof(Gender), pDto.Gender),

                };
                foreach (var medicinesId in pDto.Madicines)
                {
                    if (patient.PatientsMedicines.Any(pm => pm.MedicineId == medicinesId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    patient.PatientsMedicines
                        .Add(new PatientMedicine()
                        {
                            MedicineId = medicinesId,
                        });
                }

                sb.AppendLine(String.Format(SuccessfullyImportedPatient, patient.FullName, patient.PatientsMedicines.Count()));
                patientsValid.Add(patient);
            }


            context.AddRange(patientsValid);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var XmlParser = new XmlHelper();

            ImportPharmacyDto[] pharmaciesDtos = XmlParser.Deserialize<ImportPharmacyDto[]>(xmlString, "Pharmacies");

            var pharmaciesValid = new List<Pharmacy>();


            foreach (var pharDto in pharmaciesDtos)
            {
                if (!IsValid(pharDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;

                }
                if (!(pharDto.IsNonStop == "true" || pharDto.IsNonStop == "false"))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Pharmacy pharmacy = new Pharmacy()
                {
                    Name = pharDto.Name,
                    PhoneNumber = pharDto.PhoneNumber,
                    IsNonStop = bool.Parse(pharDto.IsNonStop)
                };

                foreach (var medDto in pharDto.Medicines)
                {
                    if (!IsValid(medDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool productionDateBool = DateTime
                        .TryParseExact(medDto.ProductionDate, "yyyy-MM-dd", CultureInfo
                        .InvariantCulture, DateTimeStyles.None, out DateTime medicineProductionDate);

                    if (!productionDateBool)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isExpityDateValid = DateTime
                        .TryParseExact(medDto.ExpiryDate, "yyyy-MM-dd", CultureInfo
                        .InvariantCulture, DateTimeStyles.None, out DateTime medicineExpityDate);

                    if (!isExpityDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (medicineProductionDate >= medicineExpityDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (pharmacy.Medicines.Any(m => m.Name == medDto.Name
                    && m.Producer == medDto.Producer))
                    {
                        sb.Append(ErrorMessage);
                        continue;
                    }

                    Medicine medicine = new Medicine()
                    {
                        Name = medDto.Name,
                        Price = (decimal)medDto.Price,
                        Category = (Category)medDto.Category,
                        ProductionDate = medicineProductionDate,
                        ExpiryDate = medicineExpityDate,
                        Producer = medDto.Producer,

                    };

                    pharmacy.Medicines.Add(medicine);
                }

                pharmaciesValid.Add(pharmacy);
                sb.AppendLine(String.Format(SuccessfullyImportedPharmacy, pharmacy.Name, pharmacy.Medicines.Count()));
            }

            context.AddRange(pharmaciesValid);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
