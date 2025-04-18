using TaskManager.Infrastructure;
using TaskManager.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace TaskManager.Util.Utils
{
    public class PasswordHasher
    {
        private readonly Repository _userRepository;

        public PasswordHasher(Repository userRepository)
        {
            _userRepository = userRepository;
        }

        public static string HashPassword(string password, string email)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + email));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashedBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public async Task<IActionResult> Login(string username, string password)
        {
            User user = await _userRepository.GetUserByUsername(username);

            if (user == null)
                return new UnauthorizedResult();

            string hashedPassword = HashPassword(password, username);

            if (hashedPassword == user.PasswordHash)
                return new OkObjectResult("Login bem-sucedido");
            else
                return new UnauthorizedResult();
        }
    }
}