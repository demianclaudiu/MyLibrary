using MyLibrary.Models.DBObjects;
using MyLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLibrary.ViewModel;

namespace MyLibrary.Repository
{
    public class LibraryRepository
    {
        private MyLibraryModelsDataContext dbContext;

        public LibraryRepository()
        {
            this.dbContext = new MyLibraryModelsDataContext();
        }

        public LibraryRepository(MyLibraryModelsDataContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<LibraryModel> GetAllLibraries()
        {
            List<LibraryModel> libraryList = new List<LibraryModel>();
            foreach (Library dbLibrary in dbContext.Libraries)
            {
                libraryList.Add(MapDBObjectToModel(dbLibrary));
            }
            return libraryList;
        }

        public List<LibraryModel> GetLibrariesByUser(Guid userId)
        {
            List<LibraryModel> libraryList = new List<LibraryModel>();
            foreach (Library dbLibrary in dbContext.Libraries.Where(x=>x.UserId == userId))
            {
                libraryList.Add(MapDBObjectToModel(dbLibrary));
            }
            return libraryList;
        }

        public LibraryModel GetLibraryById(Guid libraryId)
        {
            return MapDBObjectToModel(dbContext.Libraries
                .FirstOrDefault(x => x.LibraryId == libraryId));
        }


        //}public List<LibraryViewModel> GetAllLibraryViewModelByUserId(Guid userId)
        //{
        //    List<LibraryViewModel> libraryViewModels = new List<LibraryViewModel>();
        //    foreach (Library dbLibrary in dbContext.Libraries.Where(x=>x.UserId == userId))
        //    {
        //        LibraryViewModel libraryViewModel = new LibraryViewModel();
        //        libraryViewModel.LibraryId = dbLibrary.LibraryId;
        //        libraryViewModel.LibraryDescription = dbLibrary.Description;

        //        List<BookshelfModel> bookshelves = new List<BookshelfModel>();
        //        Dictionary<Guid, List<ShelfModel>> bookshelfShelves = new Dictionary<Guid, List<ShelfModel>>();
        //        foreach (Bookshelf dbBookshelf in dbLibrary.Bookshelfs)
        //        {
        //            BookshelfModel bookshelfModel = new BookshelfModel();
        //            bookshelfModel.BookshelfId = dbBookshelf.BookshelfId;
        //            bookshelfModel.Description = dbBookshelf.Description;
        //            bookshelves.Add(bookshelfModel);

        //            List<ShelfModel> shelves = new List<ShelfModel>();
        //            foreach (Shelf dbShelf in dbBookshelf.Shelfs)
        //            {
        //                ShelfModel shelfModel = new ShelfModel();
        //                shelfModel.ShelfId = dbShelf.ShelfId;
        //                shelfModel.Description = dbShelf.Description;
        //                shelves.Add(shelfModel);
        //            }
        //            bookshelfShelves.Add(bookshelfModel.BookshelfId, shelves);

        //        }
        //    }
        //    return libraryViewModels;
        //}

        public Guid InsertLibrary(LibraryModel libraryModel)
        {
            libraryModel.LibraryId = Guid.NewGuid();
            dbContext.Libraries.InsertOnSubmit(MapModelToDBOject(libraryModel));
            dbContext.SubmitChanges();

            return libraryModel.LibraryId;
        }

        public void UpdateLibrary(LibraryModel libraryModel)
        {
            Library dbLibrary = dbContext.Libraries
                .FirstOrDefault(x => x.LibraryId == libraryModel.LibraryId);
            if (dbLibrary != null)
            {
                dbLibrary.Description = libraryModel.Description;
                dbLibrary.UserId = libraryModel.UserId;
                dbContext.SubmitChanges();
            }
        }

        public void DeleteLibrary(Guid libraryId)
        {
            Library dbLibrary = dbContext.Libraries
                .FirstOrDefault(x => x.LibraryId == libraryId);
            if (dbLibrary != null)
            {
                dbContext.Libraries.DeleteOnSubmit(dbLibrary);
                dbContext.SubmitChanges();
            }
        }

        private Library MapModelToDBOject(LibraryModel libraryModel)
        {
            if (libraryModel != null)
            {
                return new Library
                {
                    LibraryId = libraryModel.LibraryId,
                    Description = libraryModel.Description,
                    UserId = libraryModel.UserId
                };
            }
            return null;
        }

        private LibraryModel MapDBObjectToModel(Library dbLibrary)
        {
            if (dbLibrary != null)
            {
                return new LibraryModel
                {
                    LibraryId = dbLibrary.LibraryId,
                    Description = dbLibrary.Description,
                    UserId = dbLibrary.UserId
                };
            }
            return null;
        }
    }
}