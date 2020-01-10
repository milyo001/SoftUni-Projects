namespace BookShop
{
    using BookShop.Models;
    using Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //DbInitializer.ResetDatabase(db);
                
                string result = CountCopiesByAuthor(db);
                Console.WriteLine(result);
            }

        }
        //Problem 1.
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 02.
        public static string GetGoldenBooks(BookShopContext context)
        {
            //Return in a single string titles of the golden edition books that have less than 5000 copies, each on a new line. Order them by book id ascending.
            //Call the GetGoldenBooks(BookShopContext context) method in your Main() and print the returned string to the console.

            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.Copies < 5000)
                .Where(b => (int)b.EditionType == 2)
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title
                })
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 03.
        public static string GetBooksByPrice(BookShopContext context)
        {
            //Return in a single string all titles and prices of books with price higher than 40, each
            //on a new row in the format given below. Order them by price descending.

            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }
            return sb.ToString().TrimEnd();
        }
        //Problem 04.
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            //Return in a single string all titles of books that are NOT released on a given year. Order them by book id ascending

            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year > year || b.ReleaseDate.Value.Year < year) 
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title
                })
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 05.
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            //Return in a single string the titles of books by a given list of categories. The list of categories will be
            //given in a single line separated with one or more spaces. Ignore casing. Order by title alphabetically.

            List<Book> books = new List<Book>();

            var categories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            foreach (var c in categories)
            {
                var booksToCategory = context
                    .Books
                    .Where(b => b.BookCategories
                        .Select(bc => new { bc.Category.Name })
                        .Any(ca => ca.Name.ToLower() == c))
                    .ToList();
                books.AddRange(booksToCategory);
            }

            var orderedBooks = books
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();
            return string.Join(Environment.NewLine, orderedBooks);

        }
        //Problem 06.
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            //Return the title, edition type and price of all books that are released before a given date. The date will be a string in format dd-MM-yyyy.
            StringBuilder sb = new StringBuilder();

            var dateTokens = date
                .Split('-', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
            int inputDay = int.Parse(dateTokens[0]);
            int inputMonth = int.Parse(dateTokens[1]);
            int inputYear = int.Parse(dateTokens[2]);
            DateTime inputDate = new DateTime(inputYear, inputMonth, inputDay);



            var books = context
                .Books
                .Where(b => b.ReleaseDate < inputDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType.ToString()} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 07.
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context
                .Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    Fullname = a.FirstName + " " + a.LastName
                })
                .OrderBy(b => b.Fullname)
                .ToList();

            foreach (var author in authors)
            {
                sb.AppendLine(author.Fullname);
            }
            return sb.ToString().TrimEnd();
        }
        //Problem 08.
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            //Return the titles of book, which contain a given string. Ignore casing.
            //Return all titles in a single string, each on a new row, ordered alphabetically.
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => new { b.Title })
                .OrderBy(b => b.Title)
                .ToList();
            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }
            return sb.ToString().TrimEnd();
        }
        //Problem 09.
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            //Return all titles of books and their authors’ names for books, which are written by authors whose last names start with the given string.
            //Return a single string with each title on a new row. Ignore casing. Order by book id ascending.

            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    AuthorFullname = b.Author.FirstName + " " + b.Author.LastName
                })
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.AuthorFullname})");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 10.
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            //Return the number of books, which have a title longer than the number given as an input.

            var books = context
                .Books
                .Where(b => b.Title.Length > lengthCheck)
                .Select(b => new { b.Title })
                .ToList();

            return books.Count;
        }
        //Problem 11.
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            //Return the total number of book copies for each author. Order the results descending by total book copies.
            //Return all results in a single string, each on a new line.

            StringBuilder sb = new StringBuilder();

            var authors =
                context
                .Authors
                .Select(a => new
                {
                    Fullname = a.FirstName + " " + a.LastName,
                    Count = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.Count)
                .ToList();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.Fullname} - {author.Count}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
