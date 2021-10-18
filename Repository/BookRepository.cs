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
            foreach (Book dbBook in dbContext.Books.Where(x => x.Author.ToLower().Contains(genre.ToLower())))
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
                dbBook.CoverImageId = bookModel.CoverImageId;
                dbBook.YearPublished = bookModel.YarPublished;
                dbBook.Publisher = bookModel.Publisher;
                dbBook.Genre = bookModel.Genre;
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
            throw new NotImplementedException();
        }

        private BookModel MapDBObjectToModel(Book dbBook)
        {
            throw new NotImplementedException();
        }
    }
}