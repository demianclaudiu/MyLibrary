using MyLibrary.Models.DBObjects;
using MyLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLibrary.ViewModel;
using System.Web.Mvc;
using System.Data.Entity;

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

        public int GetNumberOfBooksInLibrary(Guid libraryId)
        {
            BookshelfRepository bookshelfRepository = new BookshelfRepository();
            
            List<BookshelfModel> bookshelfModels = bookshelfRepository.GetAllBookshelfsByLibrary(libraryId);
            int numberOfBooks = 0;
            foreach (BookshelfModel bookshelfModel in bookshelfModels)
                numberOfBooks += bookshelfRepository.GetNumberOfBooksInBookshelf(bookshelfModel.BookshelfId);

            return numberOfBooks;
        }

        public List<LibraryStatsViewModel> GetLibrariesByUser(Guid userId)
        {
            List<LibraryStatsViewModel> libraryList = new List<LibraryStatsViewModel>();
            foreach (Library dbLibrary in dbContext.Libraries.Where(x=>x.UserId == userId))
            {
                libraryList.Add(new LibraryStatsViewModel
                {
                    LibraryId = dbLibrary.LibraryId,
                    Description = dbLibrary.Description,
                    NumberOfBooks = GetNumberOfBooksInLibrary(dbLibrary.LibraryId),
                    NumberOfBookshelves = dbContext.Bookshelfs.Count(x=>x.LibraryId == dbLibrary.LibraryId)
                });
            }
            return libraryList;
        }

        public IEnumerable<SelectListItem> GetLibraryNamesByUser(Guid userId)
        {
            List<SelectListItem> libraries = dbContext.Libraries.AsNoTracking()
                .Where(n => n.UserId == userId)
                .Select(n =>
                new SelectListItem
                {
                    Value = n.LibraryId.ToString(),
                    Text = n.Description
                }).ToList();
            libraries.Insert(0, new SelectListItem
            {
                Value = null,
                Text = "--- Select Library ---"
            });
            return libraries;
        }


        public LibraryModel GetLibraryById(Guid libraryId)
        {
            return MapDBObjectToModel(dbContext.Libraries
                .FirstOrDefault(x => x.LibraryId == libraryId));
        }
 

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