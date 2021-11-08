using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MyLibrary.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public Guid LibraryUserId { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("MyLibraryConnectionString", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<MyLibrary.ViewModel.LibraryAddViewModel> LibraryAddViewModels { get; set; }

        public System.Data.Entity.DbSet<MyLibrary.ViewModel.BookshelfAddViewModel> BookshelfAddViewModels { get; set; }

        public System.Data.Entity.DbSet<MyLibrary.Models.BookshelfModel> BookshelfModels { get; set; }

        public System.Data.Entity.DbSet<MyLibrary.ViewModel.LibraryStatsViewModel> LibraryStatsViewModels { get; set; }

        public System.Data.Entity.DbSet<MyLibrary.Models.LibraryModel> LibraryModels { get; set; }

        public System.Data.Entity.DbSet<MyLibrary.Models.ShelfModel> ShelfModels { get; set; }

        public System.Data.Entity.DbSet<MyLibrary.ViewModel.ShelfStatsViewModel> ShelfStatsViewModels { get; set; }

        public System.Data.Entity.DbSet<MyLibrary.Models.BookModel> BookModels { get; set; }

        public System.Data.Entity.DbSet<MyLibrary.ViewModel.AddedBookViewModel> AddedBookViewModels { get; set; }

        public System.Data.Entity.DbSet<MyLibrary.ViewModel.UpdateReadViewModel> UpdateReadViewModels { get; set; }
    }
}