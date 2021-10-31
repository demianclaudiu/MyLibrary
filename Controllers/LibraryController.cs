using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyLibrary.Models;
using MyLibrary.Repository;
using MyLibrary.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MyLibrary.Controllers
{
    public class LibraryController : Controller
    {
        private LibraryRepository libraryRepository = new LibraryRepository();

        // GET: Library
        [Authorize(Roles = "User, Admin")]
        public ActionResult Index()
        {
            List<LibraryStatsViewModel> libraryStatsViewModels = libraryRepository.GetLibrariesByUser(GetCurrentUserId());

            return View(libraryStatsViewModels);
        }

        // GET: Library/Details/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Details(Guid libraryId)
        {
            LibraryModel libraryModel = libraryRepository.GetLibraryById(libraryId);
            
            return View(libraryModel);
        }

        // GET: Library/Add
        [Authorize(Roles ="User, Admin")]
        public ActionResult Add()
        {
            return View("AddLibrary");
        }

        // POST: Library/Add
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Add(FormCollection collection)
        {
            try
            {
                LibraryAddViewModel libraryAddViewModel = new LibraryAddViewModel();
                UpdateModel(libraryAddViewModel);
                    
                LibraryModel libraryModel = new LibraryModel();
                //libraryModel.LibraryId = Guid.NewGuid();
                libraryModel.Description = libraryAddViewModel.LibraryDescription;
                libraryModel.UserId = GetCurrentUserId();

                Guid LibraryId = libraryRepository.InsertLibrary(libraryModel);

                return RedirectToAction("BatchCreate", "Bookshelf",
                    new { 
                        libraryId = LibraryId, 
                        numberOfBookshelves = libraryAddViewModel.NumberOfBookshelfs 
                    });
            }
            catch
            {
                return View();
            }
        }

        // GET: Library/Edit/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Edit(Guid libraryId)
        {
            LibraryModel libraryModel = libraryRepository.GetLibraryById(libraryId);
            
            return View(libraryModel);
        }

        // POST: Library/Edit/5
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Edit(Guid libraryId, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                LibraryModel libraryModel = new LibraryModel();

                UpdateModel(libraryModel);
                
                libraryModel.UserId = GetCurrentUserId();

                libraryRepository.UpdateLibrary(libraryModel);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Library/Delete/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Delete(Guid libraryId)
        {
            LibraryModel libraryModel = libraryRepository.GetLibraryById(libraryId);

            return View(libraryModel);
        }

        // POST: Library/Delete/5
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Delete(Guid libraryId, FormCollection collection)
        {
            try
            {
                LibraryModel libraryModel = new LibraryModel();

                UpdateModel(libraryModel);

                BookshelfRepository bookshelfRepository = new BookshelfRepository();
                
                List<BookshelfModel> bookshelfModels = bookshelfRepository.GetAllBookshelfsByLibrary(libraryModel.LibraryId);
                if (bookshelfModels != null)
                {
                    ShelfRepository shelfRepository = new ShelfRepository();
                    OwnershipRepository ownershipRepository = new OwnershipRepository();

                    foreach (BookshelfModel bookshelfModel in bookshelfModels)
                    {
                        List<ShelfModel> shelfModels = shelfRepository.GetAllShelfsInBookshelf(bookshelfModel.BookshelfId);
                        if (shelfModels!=null)
                        {
                            foreach (ShelfModel shelfModel in shelfModels)
                                ownershipRepository.DeleteOwnershipsByShelfId(shelfModel.ShelfId);
                            shelfRepository.BulkDeleteShelf(bookshelfModel.BookshelfId);
                        }
                    }
                    bookshelfRepository.BulkDeleteBookshelf(libraryModel.LibraryId);
                }

                libraryRepository.DeleteLibrary(libraryModel.LibraryId);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private Guid GetCurrentUserId()
        {
            Guid userId;
            HttpCookie cookie = Request.Cookies.Get("LibraryInfo");
            if (cookie == null)
            {
                UserRepository userRepository = new UserRepository();
                userId = userRepository.GetUserByEmail(User.Identity.Name).UserId;
            }
            else
            {
                userId = Guid.Parse(cookie.Value);
            }
            return userId;
        }
    }
}
