using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyLibrary.Models.DBObjects;
using MyLibrary.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Text;
using MyLibrary.ViewModel;
using System.Data.Entity;

namespace MyLibrary.Repository
{
    public class BookRepository
    {
        private MyLibraryModelsDataContext dbContext;

        public BookRepository()
        {
            this.dbContext = new MyLibraryModelsDataContext();
        }

        public BookRepository(MyLibraryModelsDataContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<BookModel> GetAllBooks()
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books.AsNoTracking())
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }

        internal List<BookRadioButtonViewModel> GetAllRBBooks()
        {
            List<BookRadioButtonViewModel> bookRadioButtonViewModels = new List<BookRadioButtonViewModel>();

            foreach (Book dbBook in dbContext.Books.AsNoTracking())
            {
                bookRadioButtonViewModels.Add(new BookRadioButtonViewModel
                {
                    BookId = dbBook.BookId,
                    ISBN = dbBook.ISBN,
                    Title = dbBook.Title,
                    CoverImageLocation = dbBook.CoverImageLocation,
                    Author = dbBook.Author,
                    YearPublished = dbBook.YearPublished
                });
            }

            return bookRadioButtonViewModels;
        }

        internal List<BookRadioButtonViewModel> GetRBBooksBySearch(string searchString)
        {
            List<BookRadioButtonViewModel> bookRadioButtonViewModels = new List<BookRadioButtonViewModel>();

            foreach (Book dbBook in dbContext.Books.AsNoTracking()
                .Where(x=>x.Title.ToLower().Contains(searchString.Trim().ToLower()) || 
                x.Author.ToLower().Contains(searchString.Trim().ToLower()) || 
                x.ISBN == searchString.Trim() ))
            {
                bookRadioButtonViewModels.Add(new BookRadioButtonViewModel
                {
                    BookId = dbBook.BookId,
                    ISBN = dbBook.ISBN,
                    Title = dbBook.Title,
                    CoverImageLocation = dbBook.CoverImageLocation,
                    Author = dbBook.Author,
                    YearPublished = dbBook.YearPublished
                });
            }

            return bookRadioButtonViewModels;
        }

        public List<BookModel> GetAllBooksBySearch(string searchString)
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books.AsNoTracking()
                .Where(x => x.Title.ToLower().Contains(searchString.Trim().ToLower()) ||
                x.Author.ToLower().Contains(searchString.Trim().ToLower()) ||
                x.ISBN == searchString.Trim() )) 
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }

        public List<BookModel> GetAllBooksByTile(string title)
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books.AsNoTracking().Where(x=>x.Title.ToLower().Contains(title.ToLower())))
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }

        public List<BookModel> GetAllBooksByAuthor(string author)
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books.AsNoTracking().Where(x => x.Author.ToLower().Contains(author.ToLower())))
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }

        public List<BookModel> GetAllBooksByYearPublished(int yearPublished)
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books.AsNoTracking()
                .Where(x => x.YearPublished.HasValue && x.YearPublished.Value==yearPublished))
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }

        public List<BookModel> GetAllBooksByPublisher(string publisher)
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books.AsNoTracking().Where(x => x.Publisher.ToLower().Contains(publisher.ToLower())))
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }

        public List<BookModel> GetAllBooksByGenre(string genre)
        {
            List<BookModel> booksList = new List<BookModel>();
            foreach (Book dbBook in dbContext.Books.AsNoTracking().Where(x => x.Genre.ToLower().Contains(genre.ToLower())))
            {
                booksList.Add(MapDBObjectToModel(dbBook));
            }
            return booksList;
        }
        public BookModel GetBookByISBN(string isbn)
        {
            return MapDBObjectToModel(dbContext.Books.AsNoTracking()
                .FirstOrDefault(x => x.ISBN == isbn));
        }

        public BookModel GetBookById(Guid bookId)
        {
            return MapDBObjectToModel(dbContext.Books.AsNoTracking()
                .FirstOrDefault(x => x.BookId == bookId));
        }

        public BookModel GetBookFromApiByISBN(string ISBN)
        {
            BookModel bookModel = new BookModel();

            HttpClient client = new HttpClient();

            string uri = "https://openlibrary.org/api/books?bibkeys=ISBN:" + ISBN + "&jscmd=data&format=json";

            Task<string> stringTask = client.GetStringAsync(uri);
            string bookData = stringTask.Result;
            

            JObject document = JObject.Parse(bookData);
            if(document.Count!=0)
            {
                bookModel.ISBN = ISBN;
                bookModel.Title = (string)document["ISBN:" + ISBN]["title"];
                if (document["ISBN:" + ISBN]["cover"] == null)
                {
                    bookModel.IsCoverImageLocal = true;
                    bookModel.CoverImageLocation = "~/Covers/default_cover.png";
                }
                else
                {
                    bookModel.IsCoverImageLocal = false;
                    bookModel.CoverImageLocation = (string)document["ISBN:" + ISBN]["cover"]["medium"];
                }
                
                bookModel.Publisher = (string)document["ISBN:" + ISBN]["publishers"][0]["name"];
                
                if ((string)document["ISBN:" + ISBN]["publish_date"]!=null)
                    bookModel.YearPublished = int.Parse(Regex.Match((string)document["ISBN:" + ISBN]["publish_date"], @"\d{4}").Value);
                
                if (document["ISBN:" + ISBN]["authors"] == null)
                    bookModel.Author = "Unknown Author";
                else
                {
                    JArray authors = (JArray)document["ISBN:" + ISBN]["authors"];
                    StringBuilder authorList = new StringBuilder();
                    foreach (JObject author in authors)
                    {
                        authorList.Append((string)author["name"] + ", ");
                    }
                    if (authorList.Length != 0)
                        authorList.Length = authorList.Length - 2;//remove the last 2 characters
                    bookModel.Author = authorList.ToString();
                }

                if (document["ISBN:" + ISBN]["subjects"] == null)
                    bookModel.Genre = "Unknown Genre";
                else
                {
                    JArray subjects = (JArray)document["ISBN:" + ISBN]["subjects"];
                    StringBuilder subjectList = new StringBuilder();
                    foreach (JObject subject in subjects)
                    {
                        subjectList.Append((string)subject["name"] + ", ");
                    }
                    if (subjectList.Length != 0)
                        subjectList.Length = subjectList.Length - 2;
                    bookModel.Genre = subjectList.ToString();
                }

                stringTask = client.GetStringAsync("https://openlibrary.org/api/books?bibkeys=ISBN:" + ISBN + "&jscmd=details&format=json");
                string bookDetails = stringTask.Result;
                JObject details = JObject.Parse(bookDetails);
                if (details["ISBN:" + ISBN]["details"]["description"]!=null)
                    bookModel.Description = (string)details["ISBN:" + ISBN]["details"]["description"]["value"];

                client.Dispose();

                return bookModel;

            }

            client.Dispose();

            return null;
        }

        public List<AddedBookViewModel> GetAllAddeBooksByShelfId(Guid shelfId)
        {
            List<AddedBookViewModel> addedBooks = new List<AddedBookViewModel>();
            Shelf shelf = dbContext.Shelfs.FirstOrDefault(x => x.ShelfId == shelfId);
            Bookshelf bookshelf = dbContext.Bookshelfs.FirstOrDefault(x => x.BookshelfId == shelf.BookshelfId);
            Library library = dbContext.Libraries.FirstOrDefault(x => x.LibraryId == bookshelf.LibraryId);

            List<Ownership> ownerships = dbContext.Ownerships.AsNoTracking().Where(x => x.ShelfId == shelf.ShelfId).ToList();
            if (ownerships.Count != 0)
            {
                foreach (Ownership ownership in ownerships)
                {
                    Book book = dbContext.Books.AsNoTracking().FirstOrDefault(x => x.BookId == ownership.BookId);

                    if (book != null)
                    {
                        addedBooks.Add(new AddedBookViewModel
                        {
                            OwnershipId = ownership.OwnershipId,
                            BookId = book.BookId,
                            CoverImageLocation = book.CoverImageLocation,
                            Title = book.Title,
                            Author = book.Author,
                            IsRead = ownership.IsRead ? "Read" : "Not Read",
                            BookMark = ownership.BookmarkedPage.HasValue ? ownership.BookmarkedPage.Value : 0,
                            LibraryDescription = library.Description,
                            BookshelfDescription = bookshelf.Description,
                            ShelfDescription = shelf.Description,
                            ShelfId = shelf.ShelfId
                        });
                    }
                }
            }

            return addedBooks;
        }

        public List<AddedBookViewModel> GetAllAddeBooksByBookshelfId(Guid bookshelfId)
        {
            List<AddedBookViewModel> addedBooks = new List<AddedBookViewModel>();
            Bookshelf bookshelf = dbContext.Bookshelfs.FirstOrDefault(x => x.BookshelfId == bookshelfId);
            Library library = dbContext.Libraries.FirstOrDefault(x => x.LibraryId == bookshelf.LibraryId);

            List<Shelf> shelves = dbContext.Shelfs.AsNoTracking().Where(x => x.BookshelfId == bookshelf.BookshelfId).ToList();
            if (shelves.Count != 0)
            {
                foreach (Shelf shelf in shelves)
                {
                    List<Ownership> ownerships = dbContext.Ownerships.AsNoTracking().Where(x => x.ShelfId == shelf.ShelfId).ToList();
                    if (ownerships.Count != 0)
                    {
                        foreach (Ownership ownership in ownerships)
                        {
                            Book book = dbContext.Books.AsNoTracking().FirstOrDefault(x => x.BookId == ownership.BookId);

                            if (book != null)
                            {
                                addedBooks.Add(new AddedBookViewModel
                                {
                                    OwnershipId = ownership.OwnershipId,
                                    BookId = book.BookId,
                                    CoverImageLocation = book.CoverImageLocation,
                                    Title = book.Title,
                                    Author = book.Author,
                                    IsRead = ownership.IsRead ? "Read" : "Not Read",
                                    BookMark = ownership.BookmarkedPage.HasValue ? ownership.BookmarkedPage.Value : 0,
                                    LibraryDescription = library.Description,
                                    BookshelfDescription = bookshelf.Description,
                                    ShelfDescription = shelf.Description,
                                    ShelfId = shelf.ShelfId
                                });
                            }
                        }
                    }
                }
            }

            return addedBooks;
        }

        public List<AddedBookViewModel> GetAllAddeBooksByLibraryId(Guid libraryId)
        {
            List<AddedBookViewModel> addedBooks = new List<AddedBookViewModel>();
            Library library = dbContext.Libraries.FirstOrDefault(x => x.LibraryId == libraryId);

            List<Bookshelf> bookshelves = dbContext.Bookshelfs.AsNoTracking().Where(x => x.LibraryId == library.LibraryId).ToList();
            if (bookshelves.Count != 0)
            {
                foreach (Bookshelf bookshelf in bookshelves)
                {
                    List<Shelf> shelves = dbContext.Shelfs.AsNoTracking().Where(x => x.BookshelfId == bookshelf.BookshelfId).ToList();
                    if (shelves.Count != 0)
                    {
                        foreach (Shelf shelf in shelves)
                        {
                            List<Ownership> ownerships = dbContext.Ownerships.AsNoTracking().Where(x => x.ShelfId == shelf.ShelfId).ToList();
                            if (ownerships.Count != 0)
                            {
                                foreach (Ownership ownership in ownerships)
                                {
                                    Book book = dbContext.Books.AsNoTracking().FirstOrDefault(x => x.BookId == ownership.BookId);

                                    if (book != null)
                                    {
                                        addedBooks.Add(new AddedBookViewModel
                                        {
                                            OwnershipId = ownership.OwnershipId,
                                            BookId = book.BookId,
                                            CoverImageLocation = book.CoverImageLocation,
                                            Title = book.Title,
                                            Author = book.Author,
                                            IsRead = ownership.IsRead ? "Read" : "Not Read",
                                            BookMark = ownership.BookmarkedPage.HasValue ? ownership.BookmarkedPage.Value : 0,
                                            LibraryDescription = library.Description,
                                            BookshelfDescription = bookshelf.Description,
                                            ShelfDescription = shelf.Description,
                                            ShelfId = shelf.ShelfId
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return addedBooks;
        }


        public List<AddedBookViewModel> GetAllAddeBooks(Guid userId, string searchString)
        {
            List<AddedBookViewModel> addedBooks = new List<AddedBookViewModel>();

            List<Library> libraries = dbContext.Libraries.AsNoTracking().Where(x => x.UserId == userId).ToList();
            if (libraries.Count != 0)
            {
                foreach (Library library in libraries)
                {
                    List<Bookshelf> bookshelves = dbContext.Bookshelfs.AsNoTracking().Where(x => x.LibraryId == library.LibraryId).ToList();
                    if (bookshelves.Count != 0)
                    {
                        foreach (Bookshelf bookshelf in bookshelves)
                        {
                            List<Shelf> shelves = dbContext.Shelfs.AsNoTracking().Where(x => x.BookshelfId == bookshelf.BookshelfId).ToList();
                            if (shelves.Count != 0)
                            {
                                foreach (Shelf shelf in shelves)
                                {
                                    List<Ownership> ownerships = dbContext.Ownerships.AsNoTracking().Where(x => x.ShelfId == shelf.ShelfId).ToList();
                                    if (ownerships.Count != 0)
                                    {
                                        foreach (Ownership ownership in ownerships)
                                        {
                                            Book book;
                                            if (searchString != null && !String.IsNullOrEmpty(searchString))
                                            {
                                                book = dbContext.Books.AsNoTracking().FirstOrDefault(x => x.BookId == ownership.BookId && 
                                                (x.Title.ToLower().Contains(searchString.Trim().ToLower()) || 
                                                x.Author.ToLower().Contains(searchString.Trim().ToLower()) ||
                                                x.Genre.ToLower().Contains(searchString.Trim().ToLower()) ));
                                            }
                                            else
                                            {
                                                book = dbContext.Books.AsNoTracking().FirstOrDefault(x => x.BookId == ownership.BookId);
                                            }
                                            if (book != null)
                                            {
                                                addedBooks.Add(new AddedBookViewModel
                                                {
                                                    OwnershipId = ownership.OwnershipId,
                                                    BookId = book.BookId,
                                                    CoverImageLocation = book.CoverImageLocation,
                                                    Title = book.Title,
                                                    Author = book.Author,
                                                    IsRead = ownership.IsRead ? "Read" : "Not Read",
                                                    BookMark = ownership.BookmarkedPage.HasValue ? ownership.BookmarkedPage.Value : 0,
                                                    LibraryDescription = library.Description,
                                                    BookshelfDescription = bookshelf.Description,
                                                    ShelfDescription = shelf.Description,
                                                    ShelfId = shelf.ShelfId
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return addedBooks;

        }

        public UpdateReadViewModel GetUpdateReadViewModel(Guid ownershipId)
        {
            UpdateReadViewModel updateReadViewModel = new UpdateReadViewModel();

            Ownership ownership = dbContext.Ownerships.AsNoTracking().FirstOrDefault(x => x.OwnershipId == ownershipId);
            Book book = dbContext.Books.AsNoTracking().FirstOrDefault(x => x.BookId == ownership.BookId);

            updateReadViewModel.OwnershipId = ownership.OwnershipId;
            updateReadViewModel.BookId = ownership.BookId;
            updateReadViewModel.Author = book.Author;
            updateReadViewModel.Title = book.Title;
            updateReadViewModel.IsRead = ownership.IsRead;
            updateReadViewModel.BookmarkedPage = ownership.BookmarkedPage;

            return updateReadViewModel;
        }


        public void InsertBook(BookModel bookModel)
        {
            bookModel.BookId = Guid.NewGuid();
            bookModel.DateAdded = DateTime.Now;
            dbContext.Books.InsertOnSubmit(MapModelToDBContext(bookModel));
            dbContext.SubmitChanges();
        }

        public void UpdateBook(BookModel bookModel)
        {
            Book dbBook = dbContext.Books.FirstOrDefault(x=>x.BookId==bookModel.BookId);
            if (dbBook != null)
            {
                dbBook.ISBN = bookModel.ISBN;
                dbBook.Title = bookModel.Title;
                dbBook.Author = bookModel.Author;
                dbBook.Description = bookModel.Description;
                dbBook.YearPublished = bookModel.YearPublished;
                dbBook.Publisher = bookModel.Publisher;
                dbBook.Genre = bookModel.Genre;
                dbBook.CoverImageLocation = bookModel.CoverImageLocation;
                dbBook.IsCoverImageLocal = bookModel.IsCoverImageLocal;
                dbBook.DateAdded = bookModel.DateAdded;
                dbContext.SubmitChanges();
            }
        }

        public void DeleteBook(Guid bookId)
        {
            Book dbBook = dbContext.Books.FirstOrDefault(x => x.BookId == bookId);
            if (dbBook != null)
            {
                dbContext.Books.DeleteOnSubmit(dbBook);
                dbContext.SubmitChanges();
            }
        }

        private Book MapModelToDBContext(BookModel bookModel)
        {
            if (bookModel != null)
            {
                return new Book
                {
                    BookId = bookModel.BookId,
                    ISBN = bookModel.ISBN,
                    Title = bookModel.Title,
                    Author = bookModel.Author,
                    Description = bookModel.Description,
                    YearPublished = bookModel.YearPublished,
                    Publisher = bookModel.Publisher,
                    Genre = bookModel.Genre,
                    CoverImageLocation = bookModel.CoverImageLocation,
                    IsCoverImageLocal = bookModel.IsCoverImageLocal,
                    DateAdded = bookModel.DateAdded
                };
            }
            return null;
        }

        private BookModel MapDBObjectToModel(Book dbBook)
        {
            if (dbBook != null)
            {
                return new BookModel
                {
                    BookId = dbBook.BookId,
                    ISBN = dbBook.ISBN,
                    Title = dbBook.Title,
                    Author = dbBook.Author,
                    Description = dbBook.Description,
                    YearPublished = dbBook.YearPublished,
                    Publisher = dbBook.Publisher,
                    Genre = dbBook.Genre,
                    CoverImageLocation = dbBook.CoverImageLocation,
                    IsCoverImageLocal = dbBook.IsCoverImageLocal,
                    DateAdded = dbBook.DateAdded
                };
            }
            return null;
        }
    }
}