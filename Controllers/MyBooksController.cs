using MyLibrary.Models;
using MyLibrary.Repository;
using MyLibrary.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyLibrary.Controllers
{

    public class MyBooksController : Controller
    {
        private BookRepository bookRepository = new BookRepository();
        private OwnershipRepository ownershipRepository = new OwnershipRepository();

        // GET: MyBooks
        public ActionResult Index()
        {
            List<AddedBookViewModel> addedBooks;

            addedBooks = bookRepository.GetAllAddeBooks(GetCurrentUserId());
            
            return View(addedBooks);
        }

        // GET: MyBooks/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult AddBookToShelf(Guid shelfId, string searchString)
        {

            SelectBookViewModel selectBookViewModel = new SelectBookViewModel();
            if (searchString == null || searchString == "")
                selectBookViewModel.Books = bookRepository.GetAllRBBooks();
            else
                selectBookViewModel.Books = bookRepository.GetRBBooksBySearch(searchString);

            return View(selectBookViewModel);
        }

        [HttpPost]
        public ActionResult AddBookToShelf(Guid shelfId, string searchString, SelectBookViewModel selectBookViewModel)
        {
            try
            {
                OwnershipModel ownershipModel = new OwnershipModel
                {
                    ShelfId = shelfId,
                    BookId = selectBookViewModel.SelectedBook
                };
                ownershipRepository.InsertOwnership(ownershipModel);
                ViewBag.SuccessfullyAdded = "Book Successfuly added!";
                return RedirectToAction("AddBookToShelf", new { shelfId = shelfId, searchString = searchString });
            }
            catch
            {
                return View();
            }
        }

        public ActionResult UpdateRead(Guid ownershipId)
        {
            UpdateReadViewModel updateReadViewModel = bookRepository.GetUpdateReadViewModel(ownershipId);

            return View(updateReadViewModel);
        }

        [HttpPost]
        public ActionResult UpdateRead(Guid ownershipId, FormCollection collection)
        {
            try
            {
                UpdateReadViewModel updateReadViewModel = new UpdateReadViewModel();

                UpdateModel(updateReadViewModel);

                OwnershipModel ownershipModel = ownershipRepository.GetOwnershipById(updateReadViewModel.OwnershipId);
                ownershipModel.IsRead = updateReadViewModel.IsRead;
                ownershipModel.BookmarkedPage = updateReadViewModel.BookmarkedPage;

                ownershipRepository.UpdateOwnership(ownershipModel);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MyBooks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MyBooks/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MyBooks/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MyBooks/Edit/5
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

        // GET: MyBooks/Delete/5
        public ActionResult Remove(Guid ownershipId)
        {
            UpdateReadViewModel updateReadViewModel = bookRepository.GetUpdateReadViewModel(ownershipId);

            return View(updateReadViewModel);
        }

        // POST: MyBooks/Delete/5
        [HttpPost]
        public ActionResult Remove(Guid ownershipId, FormCollection collection)
        {
            try
            {
                UpdateReadViewModel updateReadViewModel = new UpdateReadViewModel();

                UpdateModel(updateReadViewModel);

                OwnershipModel ownershipModel = ownershipRepository.GetOwnershipById(updateReadViewModel.OwnershipId);

                ownershipRepository.DeleteOwnership(ownershipModel.OwnershipId);

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
