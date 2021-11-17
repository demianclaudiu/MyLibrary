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
        public ActionResult Index(string searchString, string filter, string author, string publisher,string genre, int? year, string title)
        {
            List<BookModel> bookModels;

            if (searchString!=null && searchString.Trim()!="")
            {
                bookModels = bookRepository.GetAllBooksBySearch(searchString);
                ViewBag.BookCount = bookModels.Count;
                return View(bookModels);
            }

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

            ViewBag.BookCount = bookModels.Count;

            return View(bookModels.OrderByDescending(x=>x.DateAdded).Take(10));
        }

        // GET: Book/Details/5
        public ActionResult Details(Guid bookId)
        {
            BookModel bookModel = bookRepository.GetBookById(bookId);

            return View(bookModel);
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

                return RedirectToAction("Index");
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
        public ActionResult Delete(Guid bookId)
        {
            BookModel bookModel = bookRepository.GetBookById(bookId);

            return View(bookModel);
        }

        // POST: Book/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid bookId, FormCollection collection)
        {
            try
            {
                BookModel bookModel = new BookModel();

                UpdateModel(bookModel);

                OwnershipRepository ownershipRepository = new OwnershipRepository();

                if (ownershipRepository.GetAllOwnershipsByBookId(bookModel.BookId) != null)
                    bookRepository.DeleteBook(bookModel.BookId);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
