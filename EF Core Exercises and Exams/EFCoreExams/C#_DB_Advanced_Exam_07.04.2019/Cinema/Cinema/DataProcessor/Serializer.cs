namespace Cinema.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                .Where(m => m.Rating >= rating && m.Projections.Any(p => p.Tickets.Count > 0))
                .OrderByDescending(m => m.Rating)
                .ThenByDescending(m => m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                .Select(m => new MovieExportDto
                {
                    MovieName = m.Title,
                    Rating = m.Rating.ToString("F2"),
                    TotalIncomes = m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("F2"),
                    Customers = m.Projections
                    .SelectMany(p => p.Tickets)
                        .Select(t => new CustomerMovieExportDto
                        {
                            FirstName = t.Customer.FirstName,
                            LastName = t.Customer.LastName,
                            Balance = t.Customer.Balance.ToString("F2")
                        })
                        .OrderByDescending(t => t.Balance)
                        .ThenBy(t => t.FirstName)
                        .ThenBy(t => t.LastName)
                    .ToList()
                })
                .Take(10)
                .ToList();

            var result = JsonConvert.SerializeObject(movies, Newtonsoft.Json.Formatting.Indented);
            return result.ToString().Trim();
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {

            var customers = context.Customers
                            .Where(c => c.Age >= age)
                            .OrderByDescending(c => c.Tickets.Sum(t => t.Price))
                            .Select(c => new ExportCustomerDto
                            {
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                SpentMoney = c.Tickets.Sum(t => t.Price).ToString("F2"),
                                SpentTime = TimeSpan.FromMilliseconds(c.Tickets
                                .Sum(t => t.Projection.Movie.Duration.TotalMilliseconds))
                                .ToString(@"hh\:mm\:ss")
                            })
                            .Take(10)
                            .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCustomerDto[]), new XmlRootAttribute("Customers"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();

        }
    }
}