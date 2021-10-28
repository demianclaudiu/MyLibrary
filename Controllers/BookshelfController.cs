using MyLibrary.Models;
using MyLibrary.Repository;
using MyLibrary.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyLibrary.Controllers
{
    public class BookshelfController : Controller
    {
        BookshelfRepository bookshelfRepository = new BookshelfRepository();

        // GET: Bookshelf
        [Authorize(Roles = "User, Admin")]
        public ActionResult Index(Guid libraryId)
        {
            List<BookshelfStatsViewModel> bookshelves = bookshelfRepository.GetBookshelvesStats(libraryId);
            
            return View(bookshelves);
        }

        // GET: Bookshelf/Details/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Details(Guid bookshelfId)
        {
            BookshelfModel bookshelfModel = bookshelfRepository.GetBookshelfById(bookshelfId);
            
            return View(bookshelfModel);
        }

        [Authorize(Roles = "User, Admin")]
        public ActionResult Add(Guid libraryId)
        {
            BookshelfAddViewModel bookshelfAddViewModel = new BookshelfAddViewModel
            {
                BookshelfId = Guid.NewGuid(),
                Description = "New Bookshelf",
                NumberOfShelves = 1,
                LibraryId = libraryId
            };

            return View("Add", bookshelfAddViewModel);
        }

        // POST: Bookshelf/BatchCreate
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Add(BookshelfAddViewModel bookshelfAddViewModel)
        {
            try
            {
                ShelfRepository shelfRepository = new ShelfRepository();

                BookshelfModel bookshelfModel = new BookshelfModel
                {
                    BookshelfId = bookshelfAddViewModel.BookshelfId,
                    Description = bookshelfAddViewModel.Description,
                    LibraryId = bookshelfAddViewModel.LibraryId
                };

                bookshelfRepository.InsertBookshelf(bookshelfModel);

                for (int i = 0; i < bookshelfAddViewModel.NumberOfShelves; i++)
                {
                    ShelfModel shelfModel = new ShelfModel
                    {
                        Description = $"Shelf {i + 1}",
                        BookshelfId = bookshelfModel.BookshelfId
                    };
                    shelfRepository.InserShelf(shelfModel);
                }
                return RedirectToAction("Index", new { libraryId = bookshelfAddViewModel.LibraryId } );
            }

            catch
            {
                return View();
            }
        }

        // GET: Bookshelf/BatchCreate
        [Authorize(Roles = "User, Admin")]
        public ActionResult BatchCreate(Guid libraryId, int numberOfBookshelves)
        {
            List<BookshelfAddViewModel> bookshelfList = new List<BookshelfAddViewModel>();

            for ( int i = 0; i < numberOfBookshelves; i++ )
            {
                bookshelfList.Add(new BookshelfAddViewModel
                {
                    BookshelfId = Guid.NewGuid(),
                    Description = $"Bookshelf {i + 1}",
                    NumberOfShelves = 1,
                    LibraryId = libraryId
                }); ; 
            }

            return View("BatchCreate",bookshelfList);
        }

        // POST: Bookshelf/BatchCreate
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult BatchCreate(IList<BookshelfAddViewModel> bookshelves)
        {
            try
            {
                ShelfRepository shelfRepository = new ShelfRepository();

                foreach (BookshelfAddViewModel bookshelfAddViewModel in bookshelves)
                {
                    BookshelfModel bookshelfModel = new BookshelfModel
                    {
                        BookshelfId = bookshelfAddViewModel.BookshelfId,
                        Description = bookshelfAddViewModel.Description,
                        LibraryId = bookshelfAddViewModel.LibraryId
                    };

                    bookshelfRepository.InsertBookshelf(bookshelfModel);

                    for (int i=0;i<bookshelfAddViewModel.NumberOfShelves;i++)
                    {
                        ShelfModel shelfModel = new ShelfModel
                        {
                            Description = $"Shelf {i + 1}",
                            BookshelfId = bookshelfModel.BookshelfId
                        };
                        shelfRepository.InserShelf(shelfModel);
                    }
                }

                return RedirectToAction("Index", new { libraryId = bookshelves.First().LibraryId});
            }
            catch
            {
                return View();
            }
        }

        // GET: Bookshelf/Edit/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Edit(Guid bookshelfId)
        {
            BookshelfModel bookshelfModel = bookshelfRepository.GetBookshelfById(bookshelfId);

            return View(bookshelfModel);
        }

        // POST: Bookshelf/Edit/5
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Edit(Guid bookshelfId, FormCollection collection)
        {
            try
            {
                BookshelfModel bookshelfModel = new BookshelfModel();

                UpdateModel(bookshelfModel);

                bookshelfRepository.UpdateBookshelf(bookshelfModel);

                return RedirectToAction("Index", new { libraryId = bookshelfModel.LibraryId });
            }
            catch
            {
                return View();
            }
        }

        // GET: Bookshelf/Delete/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Delete(Guid bookshelfId)
        {
            BookshelfModel bookshelfModel = bookshelfRepository.GetBookshelfById(bookshelfId);

            return View(bookshelfModel);
        }

        // POST: Bookshelf/Delete/5
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Delete(Guid bookshelfId, FormCollection collection)
        {
            try
            {
                ShelfRepository shelfRepository = new ShelfRepository();
                
                //BookshelfModel bookshelfModel = new BookshelfModel();

                //UpdateModel(bookshelfModel);

                Guid libraryId = bookshelfRepository.GetBookshelfById(bookshelfId).LibraryId;

                foreach (ShelfModel shelfModel in shelfRepository.GetAllShelfsInBookshelf(bookshelfId))
                    shelfRepository.DeleteShelf(shelfModel.ShelfId);

                bookshelfRepository.DeleteBookshelf(bookshelfId);

                return RedirectToAction("Index", new { libraryId = libraryId });
            }
            catch
            {
                return View();
            }
        }
    }
}
