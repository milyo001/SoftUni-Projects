namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {


            var authors = context.Authors

                .Select(a => new ExportAuthorDto
                {
                    AuthorName = a.FirstName + ' ' + a.LastName,
                    Books = a.AuthorsBooks
                        .Select(b => new ExportBookDto
                        {
                            BookName = b.Book.Name,
                            BookPrice = b.Book.Price.ToString("F2")
                        })

                        .OrderByDescending(b => Decimal.Parse(b.BookPrice))
                        .ToArray()
                })
                .ToArray()
                .OrderByDescending(a => a.Books.Count)
                .ThenBy(a => a.AuthorName);
                

            var json = JsonConvert.SerializeObject(authors, Formatting.Indented);

            return json;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var books = context.Books
                .Where(b => b.PublishedOn < date && b.Genre.ToString() == "Science")
                
                .Select(b => new ExportBook
                {
                    Pages = b.Pages.ToString(),
                    Name = b.Name.ToString(),
                    Date = b.PublishedOn.ToString()
                })
                .OrderByDescending(b => int.Parse(b.Pages))
                .ToArray()
               
                
                .OrderByDescending(b => DateTime.Parse(b.Date))
                 .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportBook[]), new XmlRootAttribute("Books"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), books, namespaces);

            return sb.ToString().Trim();
        }
    }
}