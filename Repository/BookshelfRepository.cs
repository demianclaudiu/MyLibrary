using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLibrary.Models.DBObjects;
using MyLibrary.Models;
using MyLibrary.ViewModel;
using System.Web.Mvc;
using System.Data.Entity;

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

        public IEnumerable<SelectListItem> GetBookshelvesList()
        {
            List<SelectListItem> bookshelves = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = null,
                    Text = ""
                }
            };
            return bookshelves;
        }

        public IEnumerable<SelectListItem> GetBookshelvesListByLibraryId(Guid libraryId)
        {
            if (libraryId != null)
            {
                List<SelectListItem> bookshelves = dbContext.Bookshelfs.AsNoTracking()
                    .Where(n => n.LibraryId == libraryId)
                    .Select(n =>
                    new SelectListItem
                    {
                        Value = n.BookshelfId.ToString(),
                        Text = n.Description
                    }).ToList();
                bookshelves.Insert(0, new SelectListItem
                {
                    Value = null,
                    Text = "--- Select Library ---"
                });
                return bookshelves;
            }
            return null;
        }

        public BookshelfModel GetBookshelfById(Guid bookshelfId)
        {
            return MapDBObjectToModel(dbContext.Bookshelfs
                .FirstOrDefault(x => x.BookshelfId == bookshelfId));
        }

        public int GetNumberOfBooksInBookshelf(Guid bookshelfId)
        {
            List<Shelf> shelves = dbContext.Shelfs.Where(x => x.BookshelfId == bookshelfId).ToList();
            int numberOfBooks = 0;
            if (shelves.Count > 0)
                foreach (Shelf shelf in shelves)
                    numberOfBooks += dbContext.Ownerships.Count(x => x.ShelfId == shelf.ShelfId);

            return numberOfBooks;
        }

        public List<BookshelfStatsViewModel> GetBookshelvesStats(Guid libraryId)
        {
            List<BookshelfStatsViewModel> bookshelfStatsViewModels = new List<BookshelfStatsViewModel>();
            foreach (Bookshelf dbBookshelf in dbContext.Bookshelfs.Where(x => x.LibraryId == libraryId))
            {

                int numberOfShelves = dbContext.Shelfs.Count(x => x.BookshelfId == dbBookshelf.BookshelfId);
                
                bookshelfStatsViewModels.Add(new BookshelfStatsViewModel
                {
                    BookshelfId = dbBookshelf.BookshelfId,
                    LibraryId = dbBookshelf.LibraryId,
                    Description = dbBookshelf.Description,
                    NumberOfShelves = numberOfShelves,
                    NumberOfBooks = GetNumberOfBooksInBookshelf(dbBookshelf.BookshelfId)
                });
            }
            return bookshelfStatsViewModels;
        }

        public void InsertBookshelf(BookshelfModel bookshelfModel)
        {
            //bookshelfModel.BookshelfId = Guid.NewGuid();
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

        public void BulkDeleteBookshelf(Guid libraryId)
        {
            List<Bookshelf> bookshelves = dbContext.Bookshelfs.Where(x => x.LibraryId == libraryId).ToList();
            if (bookshelves!=null)
            {
                dbContext.Bookshelfs.DeleteAllOnSubmit(bookshelves);
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
            if (bookshelfModel != null)
                return new Bookshelf
                {
                    BookshelfId = bookshelfModel.BookshelfId,
                    Description = bookshelfModel.Description,
                    LibraryId = bookshelfModel.LibraryId
                };
            return null;
        }

        private BookshelfModel MapDBObjectToModel(Bookshelf dbBookshelf)
        {
            if (dbBookshelf != null)
                return new BookshelfModel
                {
                    BookshelfId = dbBookshelf.BookshelfId,
                    Description = dbBookshelf.Description,
                    LibraryId = dbBookshelf.LibraryId
                };
            return null;
        }
    }
}