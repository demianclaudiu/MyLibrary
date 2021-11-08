using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLibrary.Models.DBObjects;
using MyLibrary.Models;

namespace MyLibrary.Repository
{
    public class OwnershipRepository
    {
        private MyLibraryModelsDataContext dbContext;

        public OwnershipRepository()
        {
            this.dbContext = new MyLibraryModelsDataContext();
        }

        public OwnershipRepository(MyLibraryModelsDataContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<OwnershipModel> GetAllOwnerships()
        {
            List<OwnershipModel> ownershipList = new List<OwnershipModel>();

            foreach (Ownership dbOwnership in dbContext.Ownerships)
            {
                ownershipList.Add(MapDBObjectToModel(dbOwnership));
            }

            return ownershipList;
        }

        public List<OwnershipModel> GetAllOwnershipsByShelfId(Guid shelfId)
        {
            List<OwnershipModel> ownershipList = new List<OwnershipModel>();

            foreach (Ownership dbOwnership in dbContext.Ownerships
                .Where(x=>x.ShelfId == shelfId))
            {
                ownershipList.Add(MapDBObjectToModel(dbOwnership));
            }

            return ownershipList;
        }
        public List<OwnershipModel> GetAllOwnershipsByBookId(Guid bookId)
        {
            List<OwnershipModel> ownershipList = new List<OwnershipModel>();

            foreach (Ownership dbOwnership in dbContext.Ownerships
                .Where(x => x.BookId == bookId))
            {
                ownershipList.Add(MapDBObjectToModel(dbOwnership));
            }

            return ownershipList;
        }

        public List<OwnershipModel> GetAllReadOwnershipsByShelfId(Guid shelfId)
        {
            List<OwnershipModel> ownershipList = new List<OwnershipModel>();

            foreach (Ownership dbOwnership in dbContext.Ownerships
                .Where(x => x.ShelfId == shelfId && x.IsRead == true))
            {
                ownershipList.Add(MapDBObjectToModel(dbOwnership));
            }

            return ownershipList;
        }

        public List<OwnershipModel> GetAllBookmarkedOwnershipsByShelfId(Guid shelfId)
        {
            List<OwnershipModel> ownershipList = new List<OwnershipModel>();

            foreach (Ownership dbOwnership in dbContext.Ownerships
                .Where(x => x.ShelfId == shelfId && x.BookmarkedPage.HasValue))
            {
                ownershipList.Add(MapDBObjectToModel(dbOwnership));
            }

            return ownershipList;
        }

        public OwnershipModel GetOwnershipById(Guid ownershipId)
        {
            return MapDBObjectToModel(dbContext.Ownerships
                .FirstOrDefault(x => x.OwnershipId == ownershipId));
        }

        public void InsertOwnership(OwnershipModel ownershipModel)
        {
            ownershipModel.OwnershipId = Guid.NewGuid();
            dbContext.Ownerships.InsertOnSubmit(MapModelToDBObject(ownershipModel));
            dbContext.SubmitChanges();
        }

        public void UpdateOwnership(OwnershipModel ownershipModel)
        {
            Ownership dbOwnership = dbContext.Ownerships
                .FirstOrDefault(x => x.OwnershipId == ownershipModel.OwnershipId);
            if (dbOwnership != null)
            {
                dbOwnership.BookId = ownershipModel.BookId;
                dbOwnership.ShelfId = ownershipModel.ShelfId;
                dbOwnership.IsRead = ownershipModel.IsRead;
                dbOwnership.BookmarkedPage = ownershipModel.BookmarkedPage;

                dbContext.SubmitChanges();
            }
        }

        public void DeleteOwnershipsByShelfId(Guid shelfId)
        {
            List<Ownership> ownerships = dbContext.Ownerships.Where(x => x.ShelfId == shelfId).ToList(); ;
            if (ownerships != null)
            {
                dbContext.Ownerships.DeleteAllOnSubmit(ownerships);
                dbContext.SubmitChanges();
            }
        }

        public void DeleteOwnership(Guid ownershipId)
        {
            Ownership dbOwnership = dbContext.Ownerships
                .FirstOrDefault(x => x.OwnershipId == ownershipId);
            if (dbOwnership != null)
            {
                dbContext.Ownerships.DeleteOnSubmit(dbOwnership);
                dbContext.SubmitChanges();
            }
        }

        private Ownership MapModelToDBObject(OwnershipModel ownershipModel)
        {
            if (ownershipModel != null)
            {
                return new Ownership
                {
                    OwnershipId = ownershipModel.OwnershipId,
                    BookId = ownershipModel.BookId,
                    ShelfId = ownershipModel.ShelfId,
                    IsRead = ownershipModel.IsRead,
                    BookmarkedPage = ownershipModel.BookmarkedPage
                };
            }
            return null;
        }

        private OwnershipModel MapDBObjectToModel(Ownership dbOwnership)
        {
            if (dbOwnership!=null)
            {
                return new OwnershipModel
                {
                    OwnershipId = dbOwnership.OwnershipId,
                    BookId = dbOwnership.BookId,
                    ShelfId = dbOwnership.ShelfId,
                    IsRead = dbOwnership.IsRead,
                    BookmarkedPage = dbOwnership.BookmarkedPage
                };
            }
            return null;
        }
    }
}