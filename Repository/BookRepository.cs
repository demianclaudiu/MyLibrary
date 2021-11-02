using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLibrary.Models.DBObjects;
using MyLibrary.Models;

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