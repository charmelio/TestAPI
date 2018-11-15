using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplicationCoreTest1.Configs;
using WebApplicationCoreTest1.Models;

namespace WebApplicationCoreTest1.Interfaces
{
    public interface ITokenService
    {
        Task<Account> AuthenticateAsync(string username, string password);

        Task<IEnumerable<Account>> GetAllAsync();
    }

    public class TokenService : ITokenService
    {
        private readonly List<Account> _accounts = new List<Account>()
        {
            //TODO: Update this to get Account info from a database.
            new Account() { Id = 1, Level = 1, Username = "test", Password = "test", Email = "a@a.com"}
        };

        private readonly TokenSettings _tokenSettings;

        public TokenService(IOptions<TokenSettings> tokenSettings)
        {
            _tokenSettings = tokenSettings.Value;
        }

        public async Task<Account> AuthenticateAsync(string username, string password)
        {
            var account = await Task.Run(() => _accounts.SingleOrDefault(a => a.Username == username && a.Password == password)).ConfigureAwait(false);

            if (account == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, account.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            account.Token = tokenHandler.WriteToken(token);
            account.Password = null;

            return account;
        }

        public Task<IEnumerable<Account>> GetAllAsync()
        {
            return Task.Run(() => _accounts.Select(a =>
            {
                a.Password = null;
                return a;
            }));
        }
    }
}