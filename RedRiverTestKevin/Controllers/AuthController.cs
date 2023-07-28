using Microsoft.AspNetCore.Mvc;


using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RedRiverTestKevin.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RedRiverTestKevin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
      

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            //    _users = users;

       
        }
        // Add the list of users as a singleton to the DI container
    

        private static List<UserAccount> _users = new List<UserAccount>
{
    new UserAccount { Username = "user1", Password = "1111" },
    new UserAccount { Username = "user2", Password = "2222" },
    // Add more user accounts as needed
};


[HttpPost("login")]
        public IActionResult Login(LoginRequest model)
        {
            // Replace this logic with your actual user authentication and authorization
            var isAuthenticated = VerifyCredentials(model.Username, model.Password);

            if (!isAuthenticated)
            {
                return Unauthorized("Invalid username or password");
            }

            var token = GenerateJwtToken(model.Username);
            return Ok(new { token });
        }

        private bool VerifyCredentials(string username, string password)
        {
        
            foreach (UserAccount user in _users)
            {
                if (username == user.Username && password == user.Password)
                {
                    return true;
                }
            }

            return false;
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Generate a random 128-byte key
            //  var key =   GenerateRandomKey(128);

            // Replace GenerateRandomKey(128) with the hardcoded key
            var key = HexStringToByteArray("E7F86A0A49A1C86EAC378F1D93A57C9E71B3C6CFA770A1D33DF9873F0F4C45C9");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, username)
        }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /*
         example payload:
        eyJ1bmlxdWVfbmFtZSI6InVzZXIxIiwibmJmIjoxNjkwMzc0Njg1LCJleHAiOjE2OTA5Nzk0ODUsImlhdCI6MTY5MDM3NDY4NX0

        after decoded:
        {
  "unique_name": "user1",
  "nbf": 1690374685,
  "exp": 1690979485,
  "iat": 1690374685
}



         */





        private byte[] HexStringToByteArray(string hexString)
        {
            int numChars = hexString.Length;
            byte[] bytes = new byte[numChars / 2];
            for (int i = 0; i < numChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return bytes;
        }
        private byte[] GenerateRandomKey(int keyLength)
        {
            // Create a byte array to hold the random key
            byte[] key = new byte[keyLength];

            // Use a secure random number generator to fill the array with random bytes
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }

            return key;
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
