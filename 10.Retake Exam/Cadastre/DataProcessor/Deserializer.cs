namespace Cadastre.DataProcessor
{
    using Boardgames.Helpers;
    using Cadastre.Data;
    using Cadastre.Data.Enumerations;
    using Cadastre.Data.Models;
    using Cadastre.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid Data!";
        private const string SuccessfullyImportedDistrict =
            "Successfully imported district - {0} with {1} properties.";
        private const string SuccessfullyImportedCitizen =
            "Succefully imported citizen - {0} {1} with {2} properties.";

        public static string ImportDistricts(CadastreContext dbContext, string xmlDocument)
        {
            StringBuilder sb = new StringBuilder();
            ImportDistrictDto[] districtsDto = XmlSerializationHelper.Deserialize<ImportDistrictDto[]>(xmlDocument, "Districts");
           
            var districtsContextName = dbContext.Districts.Select(s=>s.Name).ToArray();

            
            foreach (var districtDto in districtsDto)
            {
                if (!IsValid(districtDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (Enum.TryParse<Region>(districtDto.Region, out Region parsedValue))
                {
                    
                    int numericValue = (int)parsedValue;

                    if (numericValue < 0 && numericValue > 3)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                if (districtsContextName.Contains(districtDto.Name))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                District district = new District()
                {
                    Name= districtDto.Name,
                    PostalCode= districtDto.PostalCode,
                    Region= parsedValue

                };

                foreach (var propertyDto in districtDto.Properties)
                {
                    if (!IsValid(propertyDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    bool DateOfAcquisitionBool = DateTime.TryParseExact(propertyDto.DateOfAcquisition,
                       "dd/MM/yyyy", CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out DateTime dateOfAcquisition);
                    if (!DateOfAcquisitionBool)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (dbContext.Properties.Any(p=>p.PropertyIdentifier== propertyDto.PropertyIdentifier))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    if (district.Properties.Any(p => p.PropertyIdentifier == propertyDto.PropertyIdentifier))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (dbContext.Properties.Any(p=>p.Address==propertyDto.Address))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    if (district.Properties.Any(p => p.Address == propertyDto.Address))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Property property = new Property()
                    {
                        PropertyIdentifier = propertyDto.PropertyIdentifier,
                        Area = propertyDto.Area,
                        Address = propertyDto.Address,
                        DateOfAcquisition = dateOfAcquisition
                    };
                    district.Properties.Add(property);

                }
                sb.AppendLine(String.Format(SuccessfullyImportedDistrict, district.Name, district.Properties.Count()));

                dbContext.Districts.Add(district);
                dbContext.SaveChanges();
            }

            return sb.ToString().TrimEnd();
        }

        public static string ImportCitizens(CadastreContext dbContext, string jsonDocument)
        {
            StringBuilder sb = new StringBuilder();

            var citizensDto = JsonConvert.DeserializeObject<ImportCitizenDto[]>(jsonDocument);
            List<Citizen> citizensValid = new List<Citizen>();

            foreach (var citizenDto in citizensDto)
            {
                if (!IsValid(citizenDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (Enum.TryParse<MaritalStatus>(citizenDto.MaritalStatus, out MaritalStatus maritalStatus))
                {

                    int numericValue = (int)maritalStatus;

                    if (numericValue < 0 && numericValue > 3)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool BirthDateBool = DateTime.TryParseExact(citizenDto.BirthDate,
                      "dd-MM-yyyy", CultureInfo.InvariantCulture,
                      DateTimeStyles.None, out DateTime birthDate);
                if (!BirthDateBool)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Citizen citizen = new Citizen()
                {
                    FirstName= citizenDto.FirstName,
                    LastName= citizenDto.LastName,
                    BirthDate = birthDate,
                    MaritalStatus= maritalStatus,
                   
                };

                foreach (var propertiesId in citizenDto.Properties.Distinct())
                {
                    PropertyCitizen propertyCitizen = new PropertyCitizen()
                    {
                        Citizen= citizen,
                        PropertyId = propertiesId,
                    };
                    citizen.PropertiesCitizens.Add(propertyCitizen);
                }
                citizensValid.Add(citizen);
                sb.AppendLine(String.Format( SuccessfullyImportedCitizen,citizen.FirstName,citizen.LastName,citizen.PropertiesCitizens.Count()));
            }
            dbContext.AddRange(citizensValid);
            dbContext.SaveChanges();
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
