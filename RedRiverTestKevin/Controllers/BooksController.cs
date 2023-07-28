using Microsoft.AspNetCore.Mvc;


using System.Collections.Generic;
using RedRiverTestKevin.Models;
using Microsoft.AspNetCore.Authorization;

namespace RedRiverTestKevin.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class BooksController : ControllerBase
    {

        private static List<Book> _books = new List<Book>
{
    new Book { Id = 1, Title = "Book 1", Author = "Author 1", PublicationDate = DateTime.Now },
    new Book { Id = 2, Title = "Book 2", Author = "Author 2", PublicationDate = DateTime.Now.AddDays(-100) },
    // Add more sample books if needed
   // Now is just a sample, we can eventually replace this with a database
   //If you stop the application and run it again, the list will be reset to its initial state with the two books.
};

        //Read: we get all books. GET: api/books
        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetBooks()
        {
            return Ok(_books);
        }

        //Read: we get books by id, GET: api/books/{id}   
        [HttpGet("{id}")]
        public ActionResult<Book> GetBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        //Create: we add a new book to the list using POST: api/books
       
        [HttpPost]
        public ActionResult<Book> AddBook(Book book)
        {
            // Generate a random 6-digit number
            Random random = new Random();
            int randomId = random.Next(100000, 1000000); // The upper bound is exclusive, so we use 1000000 to include 999999.

            book.Id = randomId;
            _books.Add(book);
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        //Update: we update a book's information. PUT: api/books/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, Book updatedBook)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            // Update the book properties with the updated values
            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.PublicationDate = updatedBook.PublicationDate;

            return NoContent();
        }

        //Delete:we delete a book. DELETE: api/books/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            _books.Remove(book);
            return NoContent();
        }


    }
}
