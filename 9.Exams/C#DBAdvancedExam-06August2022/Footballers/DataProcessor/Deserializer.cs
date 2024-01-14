namespace Footballers.DataProcessor
{
    using Boardgames.Helpers;
    using Footballers.Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {

            var coachesDto = XmlSerializationHelper.Deserialize<ImportCoachDto[]>(xmlString, "Coaches");
            StringBuilder sb = new StringBuilder();

            List<Coach> coachValid = new List<Coach>();

            foreach (var coachDto in coachesDto)
            {
                if (!IsValid(coachDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Coach coach = new Coach()
                {
                    Name = coachDto.Name,
                    Nationality = coachDto.Nationality
                };

                foreach (var footballerDto in coachDto.Footballers)
                {
                    if (!IsValid(footballerDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool StartContractDateBool = DateTime.TryParseExact(footballerDto.ContractStartDate,
                        "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime contractStartDate);
                    if (!StartContractDateBool)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool EndContractDateBool = DateTime.TryParseExact(footballerDto.ContractEndDate,
                        "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime contractEndDate);
                    if (!EndContractDateBool)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (contractStartDate > contractEndDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;

                    }

                    Footballer footballer = new Footballer()
                    {
                        Name= footballerDto.Name,
                        ContractStartDate= contractStartDate,
                        ContractEndDate= contractEndDate,
                        BestSkillType = (BestSkillType)footballerDto.BestSkillType,
                        PositionType = (PositionType)footballerDto.PositionType
                    };

                    coach.Footballers.Add(footballer);


                }
                coachValid.Add(coach);
                sb.AppendLine(String.Format(SuccessfullyImportedCoach, coach.Name, coach.Footballers.Count()));
            }

            context.AddRange(coachValid);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var teamsDto = JsonConvert.DeserializeObject<ImportTeamDto[]>(jsonString);

            List<Team> teamsValid = new List<Team>();
            var footbolarIds=context.Footballers.Select(x => x.Id).ToList();

            foreach (var teamDto in teamsDto)
            {
                if (!IsValid(teamDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (teamDto.Trophies == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Team team = new Team()
                {
                    Name = teamDto.Name,
                    Nationality= teamDto.Nationality,
                    Trophies=teamDto.Trophies
                };
                

                foreach (var footbolarIdDto in teamDto.Footballers.Distinct())
                {
                    if (!IsValid(footbolarIdDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }


                    if (!footbolarIds.Contains(footbolarIdDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    TeamFootballer teamFootballer = new TeamFootballer()
                    {
                        FootballerId = footbolarIdDto,
                        Team = team
                    };

                    team.TeamsFootballers.Add(teamFootballer);

                }

                teamsValid.Add(team);
                sb.AppendLine(String.Format(SuccessfullyImportedTeam, team.Name, team.TeamsFootballers.Count()));
            }

            context.AddRange(teamsValid);
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
