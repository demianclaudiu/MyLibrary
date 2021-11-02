using MyLibrary.Models;
using MyLibrary.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyLibrary.Controllers
{
    public class BookController : Controller
    {
        BookRepository bookRepository = new BookRepository();
        
        // GET: Book
        public ActionResult Index(string filter, string author, string publisher,string genre, int? year, string title)
        {
            List<BookModel> bookModels;

            switch (filter)
            {
                case "author":
                    bookModels = bookRepository.GetAllBooksByAuthor(Server.UrlDecode(author));
                    break;
                case "genre":
                    bookModels = bookRepository.GetAllBooksByGenre(Server.UrlDecode(genre));
                    break;
                case "publisher":
                    bookModels = bookRepository.GetAllBooksByPublisher(Server.UrlDecode(publisher));
                    break;
                case "year":
                    if (year.HasValue)
                        bookModels = bookRepository.GetAllBooksByYearPublished(year.Value);
                    else
                        bookModels = bookRepository.GetAllBooks();
                    break;
                case "title":
                    bookModels = bookRepository.GetAllBooksByTile(Server.UrlDecode(title));
                    break;
                default:
                    bookModels = bookRepository.GetAllBooks();
                    break;
            }
            
            return View(bookModels);
        }

        // GET: Book/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Book/Create
        public ActionResult Add()
        {
            BookModel bookModel = new BookModel();

            bookModel.IsCoverImageLocal = true;
            bookModel.CoverImageLocation = "~/Covers/default_cover.png";
            
            return View(bookModel);
        }

        // POST: Book/Create
        [HttpPost]
        public ActionResult Add(FormCollection collection)
        {
            try
            {
                BookModel bookModel = new BookModel();

                UpdateModel(bookModel);

                bookRepository.InsertBook(bookModel);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }



        // GET: Book/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Book/Edit/5
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

        // GET: Book/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Book/Delete/5
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
