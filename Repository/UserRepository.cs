using MyLibrary.Models;
using MyLibrary.Models.DBObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLibrary.Repository
{
    public class UserRepository
    {
        private Models.DBObjects.MyLibraryModelsDataContext dbContext;

        public UserRepository()
        {
            this.dbContext = new Models.DBObjects.MyLibraryModelsDataContext();
        }

        public UserRepository(Models.DBObjects.MyLibraryModelsDataContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<UserModel> GetAllUsers()
        {
            List<UserModel> userList = new List<UserModel>();
            foreach (Models.DBObjects.User dbUser in dbContext.Users)
            {
                userList.Add(MapDbObjectToModel(dbUser));
            }
            return userList;
        }

        public UserModel GetUserByEmail(string email)
        {
            return MapDbObjectToModel(dbContext.Users.FirstOrDefault(x => x.Email == email));
        }

        public void InsertUser(UserModel userModel)
        {
            userModel.UserId = Guid.NewGuid();
            dbContext.Users.InsertOnSubmit(MapModelToDbObject(userModel));
            dbContext.SubmitChanges();
        }

        public void UpdateUser(UserModel userModel)
        {
            Models.DBObjects.User existingUser = dbContext.Users
                .FirstOrDefault(x => x.UserId == userModel.UserId);
            if (existingUser!=null)
            {
                existingUser.Email = userModel.Email;
                existingUser.Name = userModel.Name;
                dbContext.SubmitChanges();
            }
        }

        public void DeleteUser(Guid UserId)
        {
            Models.DBObjects.User existingUser = dbContext.Users
                .FirstOrDefault(x => x.UserId == UserId);
            if (existingUser!=null)
            {
                dbContext.Users.DeleteOnSubmit(existingUser);
                dbContext.SubmitChanges();
            }
        }

        private User MapModelToDbObject(UserModel userModel)
        {
            Models.DBObjects.User dbUser = new Models.DBObjects.User();
            if (userModel!=null)
            {
                dbUser.UserId = userModel.UserId;
                dbUser.Email = userModel.Email;
                dbUser.Name = userModel.Name;

                return dbUser;
            }
            return null;
        }

        private UserModel MapDbObjectToModel(User dbUser)
        {
            UserModel userModel = new UserModel();

            if (dbUser!=null)
            {
                userModel.UserId = dbUser.UserId;
                userModel.Email = dbUser.Email;
                userModel.Name = dbUser.Name;

                return userModel;
            }
            return null;
        }
    }
}