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
    public class ShelfController : Controller
    {
        ShelfRepository shelfRepository = new ShelfRepository();

        // GET: Shelf
        [Authorize(Roles = "User, Admin")]
        public ActionResult Index(Guid bookshelfId)
        {
            List<ShelfStatsViewModel> shelfStatsViewModels = shelfRepository.GetAllShelfStatsInBookshelf(bookshelfId);

            return View(shelfStatsViewModels);
        }

        // GET: Shelf/Details/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Details(Guid shelfId)
        {
            ShelfModel shelfModel = shelfRepository.GetShelfById(shelfId);
            return View(shelfModel);
        }

        // GET: Shelf/Create
        [Authorize(Roles = "User, Admin")]
        public ActionResult Add(Guid bookshelfId)
        {
            ShelfModel shelfModel = new ShelfModel
            {
                ShelfId = Guid.NewGuid(),
                BookshelfId = bookshelfId,
                Description = "New Shelf"
            };
            
            return View(shelfModel);
        }

        // POST: Shelf/Create
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Add(FormCollection collection)
        {
            try
            {
                ShelfModel shelfModel = new ShelfModel();

                UpdateModel(shelfModel);

                shelfRepository.InserShelf(shelfModel);

                return RedirectToAction("Index", new { bookshelfId = shelfModel.BookshelfId });
            }
            catch
            {
                return View();
            }
        }

        // GET: Shelf/Edit/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Edit(Guid shelfId)
        {
            ShelfModel shelfModel = shelfRepository.GetShelfById(shelfId);
            return View(shelfModel);
        }

        // POST: Shelf/Edit/5
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Edit(Guid shelfId, FormCollection collection)
        {
            try
            {
                ShelfModel shelfModel = new ShelfModel();

                UpdateModel(shelfModel);

                shelfRepository.UpdateShelf(shelfModel);

                return RedirectToAction("Index", new { bookshelfId = shelfModel.BookshelfId });
            }
            catch
            {
                return View();
            }
        }

        // GET: Shelf/Delete/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Delete(Guid shelfId)
        {
            ShelfModel shelfModel = shelfRepository.GetShelfById(shelfId);
            return View(shelfModel);
        }

        // POST: Shelf/Delete/5
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Delete(Guid shelfId, FormCollection collection)
        {
            try
            {
                Guid bookshelfId = shelfRepository.GetShelfById(shelfId).BookshelfId;

                OwnershipRepository ownershipRepository = new OwnershipRepository();

                ownershipRepository.DeleteOwnershipsByShelfId(shelfId);

                shelfRepository.DeleteShelf(shelfId);

                return RedirectToAction("Index", new { bookshelfId = bookshelfId });
            }
            catch
            {
                return View();
            }
        }
    }
}
