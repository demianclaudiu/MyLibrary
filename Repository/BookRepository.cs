using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLibrary.Models.DBObjects;
using MyLibrary.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace MyLibrary.Repository
{
    public class BookRepository
    {
        private MyLibraryModelsDataContext dbContext;

        public BookRepository()
        {
            this.dbContext = new MyLibraryModelsDataContext();
        }

        public BookRepository(MyLibraryModelsDataContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<BookModel> GetAllBooks()
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books)
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }

        public List<BookModel> GetAllBooksByTile(string title)
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books.Where(x=>x.Title.ToLower().Contains(title.ToLower())))
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }

        public List<BookModel> GetAllBooksByAuthor(string author)
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books.Where(x => x.Author.ToLower().Contains(author.ToLower())))
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }

        public List<BookModel> GetAllBooksByYearPublished(int yearPublished)
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books
                .Where(x => x.YearPublished.HasValue && x.YearPublished.Value==yearPublished))
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }

        public List<BookModel> GetAllBooksByPublisher(string publisher)
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books.Where(x => x.Publisher.ToLower().Contains(publisher.ToLower())))
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }

        public List<BookModel> GetAllBooksByGenre(string genre)
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books.Where(x => x.Genre.ToLower().Contains(genre.ToLower())))
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }
        public BookModel GetBookByISBN(string isbn)
        {
            return MapDBObjectToModel(dbContext.Books
                .FirstOrDefault(x => x.ISBN == isbn));
        }

        public BookModel GetBookById(Guid bookId)
        {
            return MapDBObjectToModel(dbContext.Books
                .FirstOrDefault(x => x.BookId == bookId));
        }

        public BookModel GetBookFromApiByISBN(string ISBN)
        {
            BookModel bookModel = new BookModel();

            HttpClient client = new HttpClient();

            string uri = "https://openlibrary.org/api/books?bibkeys=ISBN:" + ISBN + "&jscmd=data&format=json";

            Task<string> stringTask = client.GetStringAsync(uri);
            string bookData = stringTask.Result;
            

            JObject document = JObject.Parse(bookData);
            if(document.Count!=0)
            {
                bookModel.BookId = Guid.NewGuid();
                bookModel.ISBN = ISBN;
                bookModel.Title = (string)document["ISBN:" + ISBN]["title"];
                bookModel.IsCoverImageLocal = false;
                bookModel.CoverImageLocation = (string)document["ISBN:" + ISBN]["cover"]["medium"];
                bookModel.Publisher = (string)document["ISBN:" + ISBN]["publishers"][0]["name"];
                if ((string)document["ISBN:" + ISBN]["publish_date"]!=null)
                    bookModel.YearPublished = int.Parse(Regex.Match((string)document["ISBN:" + ISBN]["publish_date"], @"\d{4}").Value);

                JArray authors = (JArray)document["ISBN:" + ISBN]["authors"];
                StringBuilder authorList = new StringBuilder();
                foreach (JObject author in authors)
                {
                    authorList.Append((string)author["name"] + ", ");
                }
                if (authorList.Length != 0)
                    authorList.Length = authorList.Length - 2;//remove the last 2 characters
                bookModel.Author = authorList.ToString();

                JArray subjects = (JArray)document["ISBN:" + ISBN]["subjects"];
                StringBuilder subjectList = new StringBuilder();
                foreach (JObject subject in subjects)
                {
                    subjectList.Append((string)subject["name"] + ", ");
                }
                if (subjectList.Length != 0)
                    subjectList.Length = subjectList.Length - 2;
                bookModel.Genre = subjectList.ToString();

                stringTask = client.GetStringAsync("https://openlibrary.org/api/books?bibkeys=ISBN:" + ISBN + "&jscmd=details&format=json");
                string bookDetails = stringTask.Result;
                JObject details = JObject.Parse(bookDetails);

                bookModel.Description = (string)details["ISBN:" + ISBN]["details"]["description"]["value"];

                client.Dispose();

                return bookModel;

            }

            client.Dispose();

            return null;
        }

        public void InsertBook(BookModel bookModel)
        {
            bookModel.BookId = Guid.NewGuid();
            dbContext.Books.InsertOnSubmit(MapModelToDBContext(bookModel));
            dbContext.SubmitChanges();
        }

        public void UpdateBook(BookModel bookModel)
        {
            Book dbBook = dbContext.Books.FirstOrDefault(x=>x.BookId==bookModel.BookId);
            if (dbBook != null)
            {
                dbBook.ISBN = bookModel.ISBN;
                dbBook.Title = bookModel.Title;
                dbBook.Author = bookModel.Author;
                dbBook.Description = bookModel.Description;
                dbBook.YearPublished = bookModel.YearPublished;
                dbBook.Publisher = bookModel.Publisher;
                dbBook.Genre = bookModel.Genre;
                dbBook.CoverImageLocation = bookModel.CoverImageLocation;
                dbBook.IsCoverImageLocal = bookModel.IsCoverImageLocal;
                dbContext.SubmitChanges();
            }
        }

        public void DeleteBook(Guid bookId)
        {
            Book dbBook = dbContext.Books.FirstOrDefault(x => x.BookId == bookId);
            if (dbBook != null)
            {
                dbContext.Books.DeleteOnSubmit(dbBook);
                dbContext.SubmitChanges();
            }
        }

        private Book MapModelToDBContext(BookModel bookModel)
        {
            if (bookModel != null)
            {
                return new Book
                {
                    BookId = bookModel.BookId,
                    ISBN = bookModel.ISBN,
                    Title = bookModel.Title,
                    Author = bookModel.Author,
                    Description = bookModel.Description,
                    YearPublished = bookModel.YearPublished,
                    Publisher = bookModel.Publisher,
                    Genre = bookModel.Genre,
                    CoverImageLocation = bookModel.CoverImageLocation,
                    IsCoverImageLocal = bookModel.IsCoverImageLocal
                };
            }
            return null;
        }

        private BookModel MapDBObjectToModel(Book dbBook)
        {
            if (dbBook != null)
            {
                return new BookModel
                {
                    BookId = dbBook.BookId,
                    ISBN = dbBook.ISBN,
                    Title = dbBook.Title,
                    Author = dbBook.Author,
                    Description = dbBook.Description,
                    YearPublished = dbBook.YearPublished,
                    Publisher = dbBook.Publisher,
                    Genre = dbBook.Genre,
                    CoverImageLocation = dbBook.CoverImageLocation,
                    IsCoverImageLocal = dbBook.IsCoverImageLocal
                };
            }
            return null;
        }
    }
}