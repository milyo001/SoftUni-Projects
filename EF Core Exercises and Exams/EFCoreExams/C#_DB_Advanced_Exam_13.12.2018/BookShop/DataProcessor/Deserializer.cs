namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(
                typeof(ImportBookDto[]), new XmlRootAttribute("Books"));
            var booksDtos = (ImportBookDto[])xmlSerializer.Deserialize(new StringReader(xmlString));
            var sb = new StringBuilder();
            

            foreach (var bookDto in booksDtos)
            {
                if (IsValid(bookDto) && Enum.IsDefined(typeof(Genre), bookDto.Genre))
                {
                    var book = new Book
                    {
                        Name = bookDto.Name,
                        Genre = (Genre)Enum.ToObject
                        (typeof(Genre), bookDto.Genre),
                        Price = bookDto.Price,
                        Pages = bookDto.Pages,
                        PublishedOn = DateTime.ParseExact(bookDto.PublishedOn, @"MM/dd/yyyy", CultureInfo.InvariantCulture)
                    };
                    context.Books.Add(book);
                    sb.AppendLine(String.Format(SuccessfullyImportedBook, book.Name, book.Price));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var authorsDtos = JsonConvert.DeserializeObject<ImportAuthorDto[]>(jsonString);

            var sb = new StringBuilder();

            foreach (var authorDto in authorsDtos)
            {
                if (IsValid(authorDto))
                {
                    if (context.Authors.Any(a => a.Email == authorDto.Email))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var author = new Author
                    { 
                        FirstName = authorDto.FirstName,
                        LastName = authorDto.LastName,
                        Phone = authorDto.Phone,
                        Email = authorDto.Email
                    };

                    

                    foreach (var book in authorDto.Books)
                    {
                        if (book.Id == null)
                        {
                            continue;
                        }

                        if (context.Books.Any(b => b.Id == int.Parse(book.Id)))
                        {
                            var bookToAdd = new AuthorBook
                            {
                                BookId = int.Parse(book.Id)
                            };
                            author.AuthorsBooks.Add(bookToAdd);
                        }
                        else
                        {
                            continue;
                        }

                       
                    }


                    if (author.AuthorsBooks.Count == 0)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    context.Authors.Add(author);
                    sb.AppendLine(String.Format(SuccessfullyImportedAuthor, (author.FirstName + " " + author.LastName), author.AuthorsBooks.Count));
                    context.SaveChanges();

                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }

            }
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