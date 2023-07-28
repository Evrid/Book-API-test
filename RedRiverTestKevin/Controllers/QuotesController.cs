using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedRiverTestKevin.Models;
using System.IdentityModel.Tokens.Jwt;


namespace RedRiverTestKevin.Controllers
{
    [Route("api/[controller]")]
    [Authorize]        //secure API endpoints that require authentication
    [ApiController]
    public class QuotesController : ControllerBase
    {
       

        private static List<UserAccount> _Users = new List<UserAccount>
        {
            new UserAccount { Id = 1, Username = "user1", Password = "1111" },
            new UserAccount { Id = 2, Username = "user2", Password = "2222" },
            // Add more user accounts as needed
        };

        private static List<Quote> _Quotes = new List<Quote>
        {
            new Quote { Id = 1, Content = "The river is of thee", Author = "user1" },
            new Quote { Id = 2, Content = "Yankee is the people on the west", Author = "user1" },
            new Quote { Id = 3, Content = "I love dancing queen", Author = "user2" },
            new Quote { Id = 3, Content = "Go swim in the ocean", Author = "user2" },
            
            // Add more initial quotes if needed
        };

        // Helper method to get the username from the JWT token
        private string GetUsernameFromToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // The username is stored as a claim with the name "unique_name" in the JWT token
            var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;

            return username;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Quote>> GetQuotes()
        {
            // Retrieve the username from the JWT token
            var username = GetUsernameFromToken();

            // Retrieve quotes associated with the specified author (case-insensitive comparison)
            //  var userQuotes = _Quotes.Where(q => q.Author != null && q.Author.Username.Equals(username, StringComparison.OrdinalIgnoreCase)).ToList();
            var userQuotes = _Quotes.Where(q => q.Author != null && q.Author.Equals(username, StringComparison.OrdinalIgnoreCase)).ToList();


            return Ok(userQuotes);
        }


        [HttpPost]
        public ActionResult<Quote> AddQuote(string content)
        {
            // Assign a unique ID to the new quote

            Quote quote = new Quote(); 
            Random random = new Random();
            int randomId = random.Next(100000, 1000000); // The upper bound is exclusive, so we use 1000000 to include 999999.
            quote.Id = randomId;

            

            // Retrieve the username from the JWT token
            var username = GetUsernameFromToken();

           
            // Associate the UserAccount with the quote
            quote.Author = username;
            quote.Content = content;

            // Add the new quote to the list
            _Quotes.Add(quote);

            return Ok(quote);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateQuote(int id, Quote quote)
        {
            // Update an existing quote in the list
            var existingQuote = _Quotes.FirstOrDefault(q => q.Id == id);
            if (existingQuote == null)
            {
                return NotFound();
            }

            existingQuote.Content = quote.Content;
            existingQuote.Author = quote.Author;

            return Ok(existingQuote);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteQuote(int id)
        {
            // Delete an existing quote from the list
            var quote = _Quotes.FirstOrDefault(q => q.Id == id);
            if (quote == null)
            {
                return NotFound();
            }

            _Quotes.Remove(quote);
            return NoContent();
        }
    }
}
