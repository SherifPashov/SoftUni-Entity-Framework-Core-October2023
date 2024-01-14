namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.DataProcessor.ImportDto;
    using Boardgames.Helpers;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var countrysDto = XmlSerializationHelper.Deserialize<ImportCountryDto[]>(xmlString, "Countries");
            List<Country> countryList = new List<Country>();

            foreach ( var countryDto in countrysDto )
            {
                if (!IsValid(countryDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Country country = new Country()
                {
                    CountryName= countryDto.CountryName,
                    ArmySize= countryDto.ArmySize,
                };
                countryList.Add(country);
                sb.AppendLine(string.Format(SuccessfulImportCountry,country.CountryName,country.ArmySize));
            }

            context.AddRange(countryList);
            context.SaveChanges();
            

            return sb.ToString().TrimEnd();

        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var manufacturersDto = XmlSerializationHelper.Deserialize<ImportManufacturersDto[]>(xmlString, "Manufacturers");
            List<Manufacturer> manufacturersValid=new List<Manufacturer>();

            foreach (var manufacturerDto in manufacturersDto)
            {
                if (!IsValid(manufacturerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
               
                var FoundedInfo = manufacturerDto.Founded.Split(", ").ToArray();
                if (FoundedInfo.Length == 5)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Manufacturer manufacturer = new Manufacturer()
                {
                    ManufacturerName = manufacturerDto.ManufacturerName,
                    Founded = manufacturerDto.Founded,
                };
                string countryName = FoundedInfo.Last(); 
                string townName = FoundedInfo[FoundedInfo.Length-2]; 
                
                manufacturersValid.Add(manufacturer);

                sb.AppendLine(String.Format(SuccessfulImportManufacturer,manufacturer.ManufacturerName,townName,countryName));

            }

            context.AddRange(manufacturersValid);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            throw new NotImplementedException();
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            throw new NotImplementedException();
        }
        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}