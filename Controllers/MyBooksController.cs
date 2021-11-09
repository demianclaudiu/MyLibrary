﻿using MyLibrary.Models;
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

        [Authorize(Roles = "User, Admin")]
        // GET: MyBooks
        public ActionResult Index(string searchString)
        {
            List<AddedBookViewModel> addedBooks;

            addedBooks = bookRepository.GetAllAddeBooks(GetCurrentUserId(),searchString);
            
            return View(addedBooks);
        }

        [Authorize(Roles = "User, Admin")]
        public ActionResult AddBookToShelf(Guid shelfId, string searchString)
        {

            SelectBookViewModel selectBookViewModel = new SelectBookViewModel();
            if (searchString == null || searchString == "")
                selectBookViewModel.Books = bookRepository.GetAllRBBooks();
            else
                selectBookViewModel.Books = bookRepository.GetRBBooksBySearch(searchString);

            return View(selectBookViewModel);
        }

        [Authorize(Roles = "User, Admin")]
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

        [Authorize(Roles = "User, Admin")]
        public ActionResult AddBookToLibrary()
        {
            List<BookModel> bookModels = bookRepository.GetAllBooks();

            return View(bookModels);
        }

        [Authorize(Roles = "User, Admin")]
        public ActionResult SelectLibrary(Guid bookId)
        {
            LibraryRepository libraryRepository = new LibraryRepository();
            BookshelfRepository bookshelfRepository = new BookshelfRepository();
            ShelfRepository shelfRepository = new ShelfRepository();

            MyBookMoveViewModel myBookMoveViewModel = new MyBookMoveViewModel();

            BookModel bookModel = bookRepository.GetBookById(bookId);

            myBookMoveViewModel.BookId = bookModel.BookId;
            myBookMoveViewModel.Title = bookModel.Title;

            myBookMoveViewModel.Libraries = libraryRepository.GetLibraryNamesByUser(GetCurrentUserId());
            myBookMoveViewModel.Bookshelves = bookshelfRepository.GetBookshelvesList();
            myBookMoveViewModel.Shelves = shelfRepository.GetShelvesList();

            return View(myBookMoveViewModel);
        }

        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult SelectLibrary(Guid bookId, FormCollection collection)
        {
            try
            {
                MyBookMoveViewModel myBookMoveViewModel = new MyBookMoveViewModel();

                UpdateModel(myBookMoveViewModel);

                OwnershipModel ownershipModel = new OwnershipModel
                {
                    BookId = bookId,
                    ShelfId = myBookMoveViewModel.SelectedShelf
                };

                ownershipRepository.InsertOwnership(ownershipModel);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "User, Admin")]
        public ActionResult UpdateRead(Guid ownershipId)
        {
            UpdateReadViewModel updateReadViewModel = bookRepository.GetUpdateReadViewModel(ownershipId);

            return View(updateReadViewModel);
        }

        [Authorize(Roles = "User, Admin")]
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
                return View("UpdateRead", new { ownershipId = ownershipId });
            }
        }


        // GET: MyBooks/Move
        [Authorize(Roles = "User, Admin")]
        public ActionResult Move(Guid ownershipId)
        {
            LibraryRepository libraryRepository = new LibraryRepository();
            BookshelfRepository bookshelfRepository = new BookshelfRepository();
            ShelfRepository shelfRepository = new ShelfRepository();

            MyBookMoveViewModel myBookMoveViewModel = new MyBookMoveViewModel();

            OwnershipModel ownershipModel = ownershipRepository.GetOwnershipById(ownershipId);
            BookModel bookModel = bookRepository.GetBookById(ownershipModel.BookId);

            myBookMoveViewModel.BookId = ownershipModel.BookId;
            myBookMoveViewModel.Title = bookModel.Title;

            myBookMoveViewModel.Libraries = libraryRepository.GetLibraryNamesByUser(GetCurrentUserId());
            myBookMoveViewModel.Bookshelves = bookshelfRepository.GetBookshelvesList();
            myBookMoveViewModel.Shelves = shelfRepository.GetShelvesList();

            return View(myBookMoveViewModel);
        }

        // POST: MyBooks/Move
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Move(Guid ownershipId, FormCollection collection)
        {
            try
            {
                MyBookMoveViewModel myBookMoveViewModel = new MyBookMoveViewModel();

                UpdateModel(myBookMoveViewModel);

                OwnershipModel ownershipModel = ownershipRepository.GetOwnershipById(ownershipId);

                ownershipModel.ShelfId = myBookMoveViewModel.SelectedShelf;

                ownershipRepository.UpdateOwnership(ownershipModel);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "User, Admin")]
        // GET: MyBooks/Delete/5
        public ActionResult Remove(Guid ownershipId)
        {
            UpdateReadViewModel updateReadViewModel = bookRepository.GetUpdateReadViewModel(ownershipId);

            return View(updateReadViewModel);
        }

        [Authorize(Roles = "User, Admin")]
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
