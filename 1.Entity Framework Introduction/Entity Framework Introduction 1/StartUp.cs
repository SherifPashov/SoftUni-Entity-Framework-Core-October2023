
using _1.EntityFrameworkIntroduction.Data;
using SoftUni.Models;
using System.Globalization;
using System.Text;

namespace SoftUni;


public class StartUp
{
    public static void Main(string[] args)
    {
        SoftUniContext context = new SoftUniContext();
        //string output = string.Empty;
        //03.Employees Full Information

        //output = GetEmployeesFullInformation(context);

        //4.Employees with Salary Over 50 000
        //output = GetEmployeesWithSalaryOver50000(context);

        //5.Employees from Research and Development
        //output = GetEmployeesFromResearchAndDevelopment(context);

        //6.Adding a New Address and Updating Employee
        //output = AddNewAddressToEmployee(context);

        //07.Employees and Projects
        //output = GetEmployeesInPeriod(context);

        //8.Addresses by Town
        //output = GetAddressesByTown(context);

        //9.Employee 147
        //output = GetEmployee147(context);

        //10.Departments with More Than 5 Employees
        //output = GetDepartmentsWithMoreThan5Employees(context);

        //11.Find Latest 10 Projects
        //output = GetLatestProjects(context);

        //12.Increase Salaries
        //output = IncreaseSalaries(context);

        //13.Find Employees by First Name Starting With Sa
        //output = GetEmployeesByFirstNameStartingWithSa(context);

        //14.Delete Project by Id
        //output = DeleteProjectById(context);

        //15.
        //output = RemoveTown(context);



        //Console.WriteLine(output);
    }


    //03.Employees Full Information
    public static string GetEmployeesFullInformation(SoftUniContext context)
    {
        var employees = context.Employees
            .Select(e => new
            {
                e.EmployeeId,
                e.FirstName,
                e.MiddleName,
                e.LastName,
                e.JobTitle,
                e.Salary
            })
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var employee in employees)
        {
            sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
        }

        return sb.ToString().Trim();
    }
    //4.Employees with Salary Over 50 000
    public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
    {
        var employees = context.Employees
            .Where(e => e.Salary > 50000)
            .Select(e => new { e.FirstName, e.Salary })
            .OrderBy(e => e.FirstName)
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var employee in employees)
        {
            sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
        }

        return sb.ToString().Trim();
    }

    //5.Employees from Research and Development
    public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
    {
        var employees = context.Employees
            .Where(e => e.Department.Name == "Research and Development")
            .Select(e => new { e.FirstName, e.LastName, e.Department, e.Salary })
            .OrderBy(e => e.Salary)
            .ThenByDescending(e => e.FirstName)
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var employee in employees)
        {
            sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.Department.Name} - ${employee.Salary:F2}");
        }

        return sb.ToString().TrimEnd();
    }

    //6.Adding a New Address and Updating Employee
    public static string AddNewAddressToEmployee(SoftUniContext context)
    {
        Address addres = new Address();
        addres.AddressText = "Vitoshka 15";
        addres.TownId = 4;


        context.Addresses.Add(addres);
        context.SaveChanges();

        var nakov = context.Employees
            .FirstOrDefault(e => e.LastName == "Nakov");

        nakov.Address = addres;
        context.SaveChanges();

        var empoyees = context.Employees
            .Select(e => new { e.AddressId, e.Address })
            .OrderByDescending(e => e.AddressId)
            .Take(10);

        StringBuilder sb = new StringBuilder();

        foreach (var employee in empoyees)
        {
            sb.AppendLine($"{employee.Address.AddressText}");
        }

        return sb.ToString().TrimEnd();
    }


    //07.Employees and Projects
    public static string GetEmployeesInPeriod(SoftUniContext context)
    {
        var emplpoyees = context.Employees
            .Take(10)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                ManagerFirstName = e.Manager.FirstName,
                ManagerLastName = e.Manager.LastName,
                Projects = e.EmployeesProjects.Where(e => e.Project.StartDate.Year >= 2001 && e.Project.StartDate.Year <= 2003)
                .Select(e => new
                {
                    ProjectName = e.Project.Name,
                    ProjectStartDate = e.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                    ProjectEndDate = e.Project.EndDate != null
                    ? e.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt")
                    : "not finished"
                })
            })
            .ToArray();
        StringBuilder sb = new StringBuilder();

        foreach (var emp in emplpoyees)
        {
            sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.ManagerFirstName} {emp.ManagerLastName}");

            if (emp.Projects.Any())
            {
                foreach (var project in emp.Projects)
                {
                    sb.AppendLine($"--{project.ProjectName} - {project.ProjectStartDate} - {project.ProjectEndDate}");
                }
            }
        }

        return sb.ToString().TrimEnd();
    }

    //8.Addresses by Town
    public static string GetAddressesByTown(SoftUniContext context)
    {
        var address = context.Addresses
            .OrderByDescending(a => a.Employees.Count)
            .ThenBy(a => a.Town.Name)
            .ThenBy(a => a.AddressText)
            .Take(10)
            .Select(a => new
            {
                a.AddressText,
                TownName = a.Town.Name,
                EmployeeCount = a.Employees.Count
            })
            .ToArray();
        StringBuilder sb = new StringBuilder();

        foreach (var ad in address)
        {
            sb.AppendLine($"{ad.AddressText}, {ad.TownName} - {ad.EmployeeCount} employees");
        }

        return sb.ToString().Trim();
    }


    //9.Employee 147
    public static string GetEmployee147(SoftUniContext context)
    {
        var employee = context.Employees
            .Where(e => e.EmployeeId == 147)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                ProjectsNames = e.EmployeesProjects
                .OrderBy(p => p.Project.Name)
                .Select(p => p.Project.Name)

            })
            .FirstOrDefault();

        StringBuilder sb = new StringBuilder();


        sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

        foreach (var projectName in employee.ProjectsNames)
        {
            sb.AppendLine($"{projectName}");
        }

        return sb.ToString().Trim();
    }


    //10.Departments with More Than 5 Employees
    public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
    {
        var departmentsInfo = context.Departments
            .Where(d => d.Employees.Count > 5)
            .OrderBy(d => d.Employees.Count)
            .ThenBy(d => d.Name)
            .Select(d => new
            {
                DepeartmentName = d.Name,
                ManagerName = d.Manager.FirstName + " " + d.Manager.LastName,
                EmployeesNames = d.Employees
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle}")
                .ToArray()
            })
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var depInfo in departmentsInfo)
        {
            sb.AppendLine($"{depInfo.DepeartmentName} - {depInfo.ManagerName}");
            sb.AppendLine(string.Join(Environment.NewLine, depInfo.EmployeesNames));
        }

        return sb.ToString().Trim();
    }

    //11.Find Latest 10 Projects
    public static string GetLatestProjects(SoftUniContext context)
    {
        var projectInfo = context.Projects
            .OrderByDescending(d => d.StartDate)
            .Take(10)
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                p.Name,
                p.Description,
                StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
            })
            .ToArray();
        StringBuilder sb = new StringBuilder();

        foreach (var p in projectInfo)
        {
            sb.AppendLine(p.Name);
            sb.AppendLine(p.Description);
            sb.AppendLine(p.StartDate);
        }

        return sb.ToString().TrimEnd();
    }

    //12.Increase Salaries
    public static string IncreaseSalaries(SoftUniContext context)
    {
        decimal salaryModifier = 1.12m;
        string[] departmentNames = new string[] { "Engineering", "Tool Design", "Marketing", "Information Services" };

        var employees = context.Employees
            .Where(e => departmentNames.Contains(e.Department.Name))
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToArray();

        foreach (var e in employees)
        {
            e.Salary *= salaryModifier;
        }
        context.SaveChanges();

        StringBuilder sb = new StringBuilder();
        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} ({e.Salary:f2})");
        }

        return sb.ToString().TrimEnd();
    }
    //13.Find Employees by First Name Starting With Sa
    public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
    {

        var employees = context.Employees
            .Where(e => e.FirstName.Substring(0, 2).ToLower() == "sa")
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle} - {e.Salary:f2}")
            .ToArray();

        return string.Join(Environment.NewLine, employees);

    }

    //14.Delete Project by Id
    public static string DeleteProjectById(SoftUniContext context)
    {
        var empoyeesProjectDelete = context.EmployeesProjects
            .Where(e => e.ProjectId == 2);

        context.EmployeesProjects.RemoveRange(empoyeesProjectDelete);

        var projectToDelete = context.Projects
            .Where(p => p.ProjectId == 2);

        context.Projects.RemoveRange(projectToDelete);


        context.SaveChanges();


        var projectNames = context.Projects
            .Take(10)
            .Select(e => e.Name)
            .ToArray();

        return String.Join(Environment.NewLine, projectNames);
    }


    //15.Remove Town
    public static string RemoveTown(SoftUniContext context)
    {

        var townDelete = context.Towns
            .Where(e => e.Name == "Seattle")
            .FirstOrDefault();

        var addresDelete = context.Addresses
            .Where(a => a.TownId == townDelete.TownId)
            .ToArray();

        var employeesRemoveAddress = context.Employees
            .Where(e => addresDelete.Contains(e.Address))
            .ToArray();

        foreach (var e in employeesRemoveAddress)
        {
            e.AddressId = null;
        }

        context.Addresses.RemoveRange(addresDelete);
        context.Towns.Remove(townDelete);

        context.SaveChanges();

        return $"{addresDelete.Count()} addresses in Seattle were deleted";
    }

}
