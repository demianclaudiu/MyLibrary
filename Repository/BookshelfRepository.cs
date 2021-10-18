using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLibrary.Models.DBObjects;
using MyLibrary.Models;

namespace MyLibrary.Repository
{
    public class BookshelfRepository
    {
        private MyLibraryModelsDataContext dbContext;

        public BookshelfRepository()
        {
            this.dbContext = new MyLibraryModelsDataContext();
        }

        public BookshelfRepository(MyLibraryModelsDataContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<BookshelfModel> GetAllBookshelfs()
        {
            List<BookshelfModel> bookshelfList = new List<BookshelfModel>();
            foreach (Bookshelf dbBookshelf in dbContext.Bookshelfs)
            {
                bookshelfList.Add(MapDBObjectToModel(dbBookshelf));
            }
            return bookshelfList;
        }

        public List<BookshelfModel> GetAllBookshelfsByLibrary(Guid libraryId)
        {
            List<BookshelfModel> bookshelfList = new List<BookshelfModel>();
            foreach (Bookshelf dbBookshelf in dbContext.Bookshelfs.Where(x=>x.LibraryId == libraryId))
            {
                bookshelfList.Add(MapDBObjectToModel(dbBookshelf));
            }
            return bookshelfList;
        }

        public BookshelfModel GetBookshelfById(Guid bookshelfId)
        {
            return MapDBObjectToModel(dbContext.Bookshelfs
                .FirstOrDefault(x => x.BookshelfId == bookshelfId));
        }

        public void InsertBookshelf(BookshelfModel bookshelfModel)
        {
            bookshelfModel.BookshelfId = Guid.NewGuid();
            dbContext.Bookshelfs.InsertOnSubmit(MapModelToDBObject(bookshelfModel));
            dbContext.SubmitChanges();
        }

        public void UpdateBookshelf(BookshelfModel bookshelfModel)
        {
            Bookshelf dbBookshelf = dbContext.Bookshelfs
                .FirstOrDefault(x => x.BookshelfId == bookshelfModel.BookshelfId);
            if (dbBookshelf != null)
            {
                dbBookshelf.Description = bookshelfModel.Description;
                dbBookshelf.LibraryId = bookshelfModel.LibraryId;
                dbContext.SubmitChanges();
            }
        }

        public void DeleteBookshelf(Guid bookshelfId)
        {
            Bookshelf dbBookshelf = dbContext.Bookshelfs
                .FirstOrDefault(x => x.BookshelfId == bookshelfId);
            if (dbBookshelf != null)
            {
                dbContext.Bookshelfs.DeleteOnSubmit(dbBookshelf);
                dbContext.SubmitChanges();
            }
        }

        private Bookshelf MapModelToDBObject(BookshelfModel bookshelfModel)
        {
            throw new NotImplementedException();
        }

        private BookshelfModel MapDBObjectToModel(Bookshelf dbBookshelf)
        {
            throw new NotImplementedException();
        }
    }
}