using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLibrary.Models.DBObjects;
using MyLibrary.Models;

namespace MyLibrary.Repository
{
    public class ShelfRepository
    {
        private MyLibraryModelsDataContext dbContext;

        public ShelfRepository()
        {
            this.dbContext = new MyLibraryModelsDataContext();
        }

        public ShelfRepository(MyLibraryModelsDataContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<ShelfModel> GetAllShelfs()
        {
            List<ShelfModel> shelfList = new List<ShelfModel>();
            foreach (Shelf dbShelf in dbContext.Shelfs)
            {
                shelfList.Add(MapDBObjectToModel(dbShelf));
            }
            return shelfList;
        }

        public List<ShelfModel> GetAllShelfsInBookshelf(Guid bookshelfId)
        {
            List<ShelfModel> shelfList = new List<ShelfModel>();
            foreach (Shelf dbShelf in dbContext.Shelfs.Where(x=>x.BookshelfId == bookshelfId))
            {
                shelfList.Add(MapDBObjectToModel(dbShelf));
            }
            return shelfList;
        }

        public ShelfModel GetShelfById( Guid shelfId)
        {
            return MapDBObjectToModel(dbContext.Shelfs
                .FirstOrDefault(x => x.ShelfId == shelfId));
        }

        public void InserShelf(ShelfModel shelfModel)
        {
            shelfModel.ShelfId = Guid.NewGuid();
            dbContext.Shelfs.InsertOnSubmit(MapModelToDBObject(shelfModel));
            dbContext.SubmitChanges();
        }

        public void UpdateShelf(ShelfModel shelfModel)
        {
            Shelf dbShelf = dbContext.Shelfs.FirstOrDefault(x => x.ShelfId == shelfModel.ShelfId);
            if (dbShelf != null)
            {
                dbShelf.Description = shelfModel.Description;
                dbShelf.BookshelfId = shelfModel.BookshelfId;
                dbContext.SubmitChanges();
            }
        }

        public void DeleteShelf(Guid shelfId)
        {
            Shelf dbShelf = dbContext.Shelfs.FirstOrDefault(x => x.ShelfId == shelfId);
            if (dbShelf != null)
            {
                dbContext.Shelfs.DeleteOnSubmit(dbShelf);
                dbContext.SubmitChanges();
            }
        }

        private Shelf MapModelToDBObject(ShelfModel shelfModel)
        {
            if (shelfModel != null)
            {
                return new Shelf
                {
                    ShelfId = shelfModel.ShelfId,
                    Description = shelfModel.Description,
                    BookshelfId = shelfModel.BookshelfId
                };
            }
            return null;
        }

        private ShelfModel MapDBObjectToModel(Shelf dbShelf)
        {
            if (dbShelf != null)
            {
                return new ShelfModel
                {
                    ShelfId = dbShelf.ShelfId,
                    Description = dbShelf.Description,
                    BookshelfId = dbShelf.BookshelfId
                };
            }
            return null;
        }
    }
}