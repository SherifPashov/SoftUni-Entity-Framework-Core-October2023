namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);


            //Input Values
            string result = string.Empty;



            //O2.Age Restriction
            //string command = Console.ReadLine().ToLower();
            //result = GetBooksByAgeRestriction(db, command);

            //O3.Golden Books
            //result = GetGoldenBooks(db);

            //O4.Books by Price
            //result = GetBooksByPrice(db);

            //05.Not Released In
            //int year = int.Parse(Console.ReadLine());
            //result = GetBooksNotReleasedIn(db, year);

            //06.Book Titles By Category
            //string categories = Console.ReadLine();
            //result = GetBooksByCategory(db, categories);

            //07.Released Before Date
            //string date = Console.ReadLine();
            //result = GetBooksReleasedBefore(db, date);

            //08.Author Search
            //string endingCharacters = Console.ReadLine();
            //result = GetAuthorNamesEndingIn(db, endingCharacters);

            //09.Book Search
            //string stringSearched = Console.ReadLine();
            //result = GetBookTitlesContaining(db, stringSearched);

            ////10.Book Search by Author
            //string stringSearched = Console.ReadLine();
            //result = GetBooksByAuthor(db, stringSearched);

            //11.Count Books
            //int length = int.Parse(Console.ReadLine());
            //int countBooks = CountBooks(db, length);
            //Console.WriteLine(countBooks);

            //12.Total Book Copies
            //result = CountCopiesByAuthor(db);

            //13.Profit by Category
            //result = GetTotalProfitByCategory(db);

            //14.Most Recent Books
            //result = GetMostRecentBooks(db);

            //15.Increase Prices
            //IncreasePrices(db);

            //16.Remove Books
            //int removedBooksCount = RemoveBooks(db);
            //Console.WriteLine(removedBooksCount);

            Console.WriteLine(result); // <- Comment this when sumbitting exercises number 11, 15 and 16
        }
        //O2.Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var enumValue = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .Where(b => b.AgeRestriction == enumValue)
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            return String.Join(Environment.NewLine, books);
        }

        //O3.Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            var editionType = Enum.Parse<EditionType>("Gold", false);
            var books = context.Books
                .Where(c => c.Copies < 5000 && c.EditionType == editionType)
                .Select(c => new { c.Title, c.BookId })
                .OrderBy(c => c.BookId)
                .ToArray();

            return String.Join(Environment.NewLine, books.Select(b => b.Title));

        }

        //O4.Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new { b.Title, b.Price })
                .OrderByDescending(b => b.Price)
                .ToArray();
            return String.Join(Environment.NewLine, books.Select(b => $"{b.Title} - ${b.Price:f2}"));
        }

        //5.Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .Select(b => new { b.Title, b.BookId })
                .OrderBy(b => b.BookId)
                .ToArray();
            return String.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        //6.Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var inputInfoSplit = input.ToLower().Split(' ').ToArray();

            var books = context.BooksCategories
                .Where(b => inputInfoSplit.Contains(b.Category.Name.ToLower()))
                .Select(b => b.Book.Title)
                .OrderBy(c => c)
                .ToArray();
            return string.Join(Environment.NewLine, books);

        }

        //07.Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime dateParse = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            var books = context.Books
                .Where(b => b.ReleaseDate < dateParse)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price,
                    b.ReleaseDate,
                })
                .OrderByDescending(b => b.ReleaseDate);

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //08.Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new { FullNameAuthor = $"{a.FirstName} {a.LastName}" })
                .OrderBy(a => a.FullNameAuthor)
                .ToArray();

            return string.Join(Environment.NewLine, authors.Select(a => a.FullNameAuthor));
        }

        //09.Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();
            return String.Join(Environment.NewLine, books);
        }

        //10. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(a => new
                {

                    AuthorName = a.FirstName + " " + a.LastName,
                    Books = a.Books.Select(b => new { b.Title, b.BookId }).OrderBy(b => b.BookId).ToArray()

                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var author in authors)
            {

                foreach (var book in author.Books)
                {
                    sb.AppendLine($"{book.Title} ({author.AuthorName})");
                }

            }
            return sb.ToString().TrimEnd();
        }

        //11.Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var bbokCount = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();
            return bbokCount;
        }

        //12.Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    BookCopis = a.Books.Select(b => b.Copies).Sum()
                }).OrderByDescending(b => b.BookCopis)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var author in authors)
            {
                sb.AppendLine($"{author.AuthorName} - {author.BookCopis}");
            }

            return sb.ToString().TrimEnd();

        }

        //13.Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categoriesInfo = context.Categories
                .AsNoTracking()
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Profit = c.CategoryBooks.Sum(b => b.Book.Copies * b.Book.Price)
                })
                .OrderByDescending(c => c.Profit)
                .ThenBy(c => c.CategoryName)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var category in categoriesInfo)
            {
                sb.AppendLine($"{category.CategoryName} ${category.Profit}");
            }
            return sb.ToString().TrimEnd();
        }

        //14.Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categoriesInfo = context.Categories
                .AsNoTracking()
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Last3Books = c.CategoryBooks
                            .Select(b => new
                            {
                                BookName = b.Book.Title,
                                RelaseDate = b.Book.ReleaseDate
                            })
                            .OrderByDescending(b => b.RelaseDate)
                            .Take(3)
                            .ToArray()
                })
                .OrderBy(b => b.CategoryName)
                .ToArray();
            StringBuilder sb = new StringBuilder();
            foreach (var category in categoriesInfo)
            {
                sb.AppendLine($"--{category.CategoryName}");

                foreach (var book in category.Last3Books)
                {
                    sb.AppendLine($"{book.BookName} ({book.RelaseDate.Value.Year})");
                }
            }
            return sb.ToString().TrimEnd();
        }

        //15.Increase Prices
        public static void IncreasePrices(BookShopContext context)
        { 
            var books =context.Books
                .Where(b=>b.ReleaseDate.Value.Year < 2010)
                .ToList();
            foreach (var book in books)
            {
                book.Price += 5;
            }
            context.SaveChanges();
        }

        //16.Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var booksCategoriesToRemove = context.BooksCategories
           .Where(bc => bc.Book.Copies < 4200)
           .ToArray();

            var booksRemove = context.Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            context.BooksCategories.RemoveRange(booksCategoriesToRemove);
            context.Books.RemoveRange(booksRemove);
            context.SaveChanges();

            return booksRemove.Count();
        }



    }
}


