using MyLibrary.Models;
using MyLibrary.Repository;
using System;
using System.Collections.Generic;
using System.IO;
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

        public ActionResult AddByISBN()
        {

            return View();
        }

        // POST: Book/Create
        [HttpPost]
        public ActionResult AddByISBN(string ISBN)
        {
            try
            {
                BookModel bookModel = bookRepository.GetBookFromApiByISBN(ISBN);
                if (bookModel != null)
                {
                    bookRepository.InsertBook(bookModel);
                    return RedirectToAction("Edit", new { bookId = bookModel.BookId });
                }
                else
                    return RedirectToAction("Add");
            }
            catch
            {
                return View();
            }
        }


        // GET: Book/Edit/5
        public ActionResult Edit(Guid bookId)
        {
            BookModel bookModel = bookRepository.GetBookById(bookId);
            
            return View(bookModel);
        }

        // POST: Book/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid bookId, FormCollection collection)
        {
            try
            {
                BookModel bookModel = new BookModel();
                UpdateModel(bookModel);

                bookRepository.UpdateBook(bookModel);

                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch
            {
                return View();
            }
        }

        public ActionResult AddCover(Guid bookId)
        {
            ViewBag.BookId = bookId;

            return View();
        }

        [HttpPost]
        public ActionResult AddCover(Guid bookId, HttpPostedFileBase postedFile)
        {
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Covers/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                postedFile.SaveAs(path + Path.GetFileName(bookId.ToString()+Path.GetExtension(postedFile.FileName)));
                ViewBag.Message = "File uploaded successfully.";

                BookModel bookModel = bookRepository.GetBookById(bookId);
                bookModel.CoverImageLocation = "~/Covers/" + Path.GetFileName(bookId.ToString() + Path.GetExtension(postedFile.FileName));

                bookRepository.UpdateBook(bookModel);

            }

            return View();
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
