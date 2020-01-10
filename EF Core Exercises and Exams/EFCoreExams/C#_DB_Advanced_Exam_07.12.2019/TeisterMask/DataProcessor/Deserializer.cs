namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data;
    using System.Xml.Serialization;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using System.Linq;
    using TeisterMask.DataProcessor.ImportDto;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(
                typeof(ImportProjectDto[]), new XmlRootAttribute("Projects"));
            var projectsDto = (ImportProjectDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            List<Project> projects = new List<Project>();

            StringBuilder sb = new StringBuilder();

            foreach (var projectDto in projectsDto)
            {
                bool isValidProject = IsValid(projectDto);

                if (isValidProject == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Project project = new Project
                {
                    Name = projectDto.Name,
                    OpenDate = DateTime.ParseExact(
                        projectDto.OpenDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture),
                    DueDate = string.IsNullOrEmpty(projectDto.DueDate) ?
                        (DateTime?)null :
                        DateTime.ParseExact(
                            projectDto.DueDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture)
                };

                foreach (var taskDto in projectDto.Tasks)
                {
                    bool isValidTask = IsValid(taskDto);
                    bool isValidExecutionType = Enum.IsDefined(typeof(ExecutionType), taskDto.ExecutionType);
                    bool isValidLabelType = Enum.IsDefined(typeof(LabelType), taskDto.LabelType);

                    if (isValidTask == false || isValidExecutionType == false || isValidLabelType == false)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime taskOpenDate = DateTime.ParseExact(
                            taskDto.OpenDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime projectOpenDate = project.OpenDate;

                    DateTime taskDueDate = DateTime.ParseExact(
                            taskDto.DueDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime? projectDueDate = project.DueDate;

                    if (taskOpenDate < projectOpenDate || taskDueDate > projectDueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Task task = new Task
                    {
                        Name = taskDto.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)Enum.ToObject(
                            typeof(ExecutionType), taskDto.ExecutionType),
                        LabelType = (LabelType)Enum.ToObject(
                            typeof(LabelType), taskDto.LabelType)
                    };

                    project.Tasks.Add(task);
                }

                projects.Add(project);

                sb.AppendLine(string.Format(SuccessfullyImportedProject,
                    project.Name,
                    project.Tasks.Count));
            }

            context.Projects.AddRange(projects);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var employeesDto = JsonConvert.DeserializeObject<ImportEmployeeWithTasksDto[]>(jsonString);

            var employees = new List<Employee>();
            var sb = new StringBuilder();

            foreach (var employeeDto in employeesDto)
            {
                if (IsValid(employeeDto))
                {
                    var employee = new Employee
                    {
                        Username = employeeDto.Username,
                        Email = employeeDto.Email,
                        Phone = employeeDto.Phone
                    };

                    foreach (var taskId in employeeDto.Tasks.Distinct())
                    {
                        if (context.Tasks.Any(t => t.Id == taskId))
                        {
                            var employeeTask = new EmployeeTask
                            {
                                TaskId = taskId
                            };

                            employee.EmployeesTasks.Add(employeeTask);
                        }
                        else
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                    }

                    context.Employees.Add(employee);
                    sb.AppendLine(String.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }

                
                
            }

            context.SaveChanges();
            return sb.ToString().Trim();
        }



        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}