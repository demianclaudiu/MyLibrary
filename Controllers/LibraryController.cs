using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyLibrary.Models;
using MyLibrary.Repository;
using MyLibrary.ViewModel;

namespace MyLibrary.Controllers
{
    public class LibraryController : Controller
    {
        private LibraryRepository libraryRepository = new LibraryRepository();
        
        // GET: Library
        public ActionResult Index()
        {
            return View();
        }

        // GET: Library/Details/5
        public ActionResult Details(int id)
        {
            return View();
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

                UserRepository userRepository = new UserRepository();

                LibraryModel libraryModel = new LibraryModel();
                //libraryModel.LibraryId = Guid.NewGuid();
                libraryModel.Description = libraryAddViewModel.LibraryDescription;
                           
                
                libraryModel.UserId = userRepository.GetUserByEmail(User.Identity.Name).UserId;

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
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Library/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Library/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Library/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
