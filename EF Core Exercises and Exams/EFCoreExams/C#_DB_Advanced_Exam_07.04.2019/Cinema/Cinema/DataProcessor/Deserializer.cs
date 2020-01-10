namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data;
    using Cinema.Data.Models;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";



        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            //trying solution without DTO without Validator, kids never try this at home 
            StringBuilder sb = new StringBuilder();
            var json = JsonConvert.DeserializeObject<Movie[]>(jsonString);
            foreach (var movie in json)
            {
                try
                {
                    if (context.Movies.Any(m => m.Title == movie.Title))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    else if (movie.Title == null || movie.Title.Length < 3 || movie.Title.Length > 20)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    else if ((int)movie.Genre > 9 || (int)movie.Genre < 0)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    else if (movie.Duration == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    else if (movie.Rating > 10 || movie.Rating < 1)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    else if (movie.Director == null || movie.Director.Length <= 3 || movie.Director.Length >= 20)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    context.Add(movie);
                    context.SaveChanges();
                    sb.AppendLine($"Successfully imported {movie.Title} with genre {movie.Genre} and rating {movie.Rating.ToString("F2")}!");

                }
                catch (Exception)
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var json = JsonConvert.DeserializeObject<ImportHallDto[]>(jsonString);
            var validatedHalls = new List<Hall>();

            foreach (var hallDto in json)
            {
                if (IsValid(hallDto))
                {
                    var validatedHall = new Hall
                    {
                        Name = hallDto.Name,
                        Is3D = hallDto.Is3D,
                        Is4Dx = hallDto.Is4Dx,
                    };
                    validatedHalls.Add(validatedHall);
                    ImportSeats(validatedHall, hallDto.Seats);
                    string projectionType = GetProjectionType(validatedHall);
                    sb.AppendLine(String.Format(SuccessfulImportHallSeat, validatedHall.Name, projectionType, validatedHall.Seats.Count));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
            }

            context.Halls.AddRange(validatedHalls);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }



        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ImportProjectionDto[]), new XmlRootAttribute("Projections"));
            var projectionDtos = (ImportProjectionDto[])serializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();

            foreach (var dto in projectionDtos)
            {
                if (IsValid(dto) && IsValidMovieId(context, dto.MovieId) && IsValidHallId(context, dto.HallId))
                {
                    var projection = new Projection
                    {
                        MovieId = dto.MovieId,
                        HallId = dto.HallId,
                        DateTime = DateTime.ParseExact(dto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    };
                    context.Projections.Add(projection);
                    context.SaveChanges();

                    sb.AppendLine(String.Format(SuccessfulImportProjection,
                        projection.Movie.Title,
                        projection.DateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }
            context.SaveChanges();
            return sb.ToString().Trim();
        }



        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));
            var customersDto = (ImportCustomerDto[])xmlSerializer.Deserialize(new StringReader(xmlString));
            var sb = new StringBuilder();

            foreach (var customerDto in customersDto)
            {
                if (IsValid(customerDto))
                {
                    var validatedCustomer = new Customer
                    {
                        FirstName = customerDto.FirstName,
                        LastName = customerDto.LastName,
                        Age = customerDto.Age,
                        Balance = customerDto.Balance
                        
                    };
                    context.Customers.Add(validatedCustomer);
                    AddTicketsToCustomer(context, validatedCustomer.Id, customerDto.Tickets);
                    sb.AppendLine(String.Format(SuccessfulImportCustomerTicket, validatedCustomer.FirstName,
                        validatedCustomer.LastName, validatedCustomer.Tickets.Count));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        private static string GetProjectionType(Hall validatedHall)
        {
            if (validatedHall.Is3D && validatedHall.Is4Dx)
            {
                return "4Dx/3D";
            }
            if (validatedHall.Is3D)
            {
                return "3D";
            }
            if (validatedHall.Is4Dx)
            {
                return "4Dx";
            }
            else
            {
                return "Normal";
            }
        }

        private static void ImportSeats(Hall validatedHall, int seats)
        {
            for (int i = 0; i < seats; i++)
            {
                validatedHall.Seats.Add(new Seat
                {
                    HallId = validatedHall.Id
                });
            }
        }

        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationResult = new List<ValidationResult>();
            var result = Validator.TryValidateObject(obj, validator, validationResult, true);
            return result;
        }

        private static bool IsValidMovieId(CinemaContext context, int movieId)
        {
            return context.Movies.Any(m => m.Id == movieId);
        }

        private static bool IsValidHallId(CinemaContext context, int hallId)
        {
            return context.Halls.Any(h => h.Id == hallId);
        }

        private static void AddTicketsToCustomer
         (CinemaContext context, int customerId, ImportTicketDto[] dtoTickets)


        {
            var tickets = new List<Ticket>();

            foreach (var dto in dtoTickets)
            {
                if (IsValid(dto))
                {
                    var ticket = new Ticket
                    {
                        ProjectionId = dto.ProjectionId,
                        CustomerId = customerId,
                        Price = dto.Price
                    };
                    tickets.Add(ticket);
                    
                }
            }
            context.Tickets.AddRange(tickets);
            context.SaveChanges();
        }
    }
}