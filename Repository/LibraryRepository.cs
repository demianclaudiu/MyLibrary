using MyLibrary.Models.DBObjects;
using MyLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public void InsertLibrary(LibraryModel libraryModel)
        {
            libraryModel.LibraryId = Guid.NewGuid();
            dbContext.Libraries.InsertOnSubmit(MapModelToDBOject(libraryModel));
            dbContext.SubmitChanges();
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
            throw new NotImplementedException();
        }

        private LibraryModel MapDBObjectToModel(Library dbLibrary)
        {
            throw new NotImplementedException();
        }
    }
}