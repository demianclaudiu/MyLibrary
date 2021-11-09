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

        public IEnumerable<SelectListItem> GetShelvesList()
        {
            List<SelectListItem> shelves = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = null,
                    Text = ""
                }
            };
            return shelves;
        }

        public IEnumerable<SelectListItem> GetShelvesListByBookshelfId(Guid bookshelfId)
        {
            List<SelectListItem> shelves = dbContext.Shelfs.AsNoTracking()
                .Where(n => n.BookshelfId == bookshelfId)
                .Select(n =>
                new SelectListItem
                {
                    Value = n.ShelfId.ToString(),
                    Text = n.Description
                }).ToList();
            shelves.Insert(0, new SelectListItem
            {
                Value = null,
                Text = "--- Select Library ---"
            });
            return shelves;
        }

        public List<ShelfStatsViewModel> GetAllShelfStatsInBookshelf(Guid bookshelfId)
        {
            List<ShelfStatsViewModel> shelfList = new List<ShelfStatsViewModel>();
            foreach (Shelf dbShelf in dbContext.Shelfs.Where(x => x.BookshelfId == bookshelfId))
            {
                shelfList.Add(new ShelfStatsViewModel
                {
                    ShelfId = dbShelf.ShelfId,
                    Description = dbShelf.Description,
                    NumberOfBooks = dbContext.Ownerships.Count(x => x.ShelfId == dbShelf.ShelfId)
                });
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

        public void BulkDeleteShelf(Guid bookshelfId)
        {
            List<Shelf> shelves = dbContext.Shelfs.Where(x => x.BookshelfId == bookshelfId).ToList();
            if (shelves != null)
            {
                dbContext.Shelfs.DeleteAllOnSubmit(shelves);
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