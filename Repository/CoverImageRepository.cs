using MyLibrary.Models;
using MyLibrary.Models.DBObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLibrary.Repository
{
    public class CoverImageRepository
    {
        private Models.DBObjects.MyLibraryModelsDataContext dbContext;

        public CoverImageRepository()
        {
            this.dbContext = new Models.DBObjects.MyLibraryModelsDataContext();
        }

        public CoverImageRepository(Models.DBObjects.MyLibraryModelsDataContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<CoverImageModel> GetAllCoverImages()
        {
            List<CoverImageModel> coverImageList = new List<CoverImageModel>();
            foreach (CoverImage dbCoverImage in dbContext.CoverImages)
            {
                coverImageList.Add(MapDBObjectToModel(dbCoverImage));
            }

            return coverImageList;
        }

        public CoverImageModel GetCoverImageById(Guid coverImageId)
        {
            return MapDBObjectToModel(dbContext.CoverImages
                .FirstOrDefault(x => x.CoverImageId == coverImageId));
        }

        public void InserCoverImage(CoverImageModel coverImageModel)
        {
            coverImageModel.CoverImageId = Guid.NewGuid();
            dbContext.CoverImages.InsertOnSubmit(MapModelToDBObject(coverImageModel));
            dbContext.SubmitChanges();
        }

        public void UpdateCoverImage(CoverImageModel coverImageModel)
        {
            CoverImage dbCoverImage = dbContext.CoverImages
                .FirstOrDefault(x => x.CoverImageId == coverImageModel.CoverImageId);
            if (dbCoverImage != null)
            {
                dbCoverImage.ImageLocation = coverImageModel.ImageLocation;
                dbCoverImage.IsLocal = coverImageModel.IsLocal;
                dbContext.SubmitChanges();
            }
        }

        public void DeleteCoverImage(Guid coverImageId)
        {
            CoverImage dbCoverImage = dbContext.CoverImages
                .FirstOrDefault(x => x.CoverImageId == coverImageId);
            if (dbCoverImage!=null)
            {
                dbContext.CoverImages.DeleteOnSubmit(dbCoverImage);
                dbContext.SubmitChanges();
            }
        }

        private CoverImage MapModelToDBObject(CoverImageModel coverImageModel)
        {
            if (coverImageModel != null)
            {
                return new CoverImage
                {
                    CoverImageId = coverImageModel.CoverImageId,
                    ImageLocation = coverImageModel.ImageLocation,
                    IsLocal = coverImageModel.IsLocal
                };
            }
            return null;
        }

        private CoverImageModel MapDBObjectToModel(CoverImage dbCoverImage)
        {
            if (dbCoverImage!=null)
            {
                return new CoverImageModel
                {
                    CoverImageId = dbCoverImage.CoverImageId,
                    ImageLocation = dbCoverImage.ImageLocation,
                    IsLocal = dbCoverImage.IsLocal
                };   
            }
            return null;
        }
    }
}