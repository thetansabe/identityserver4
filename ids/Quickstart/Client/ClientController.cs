using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.Entities;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace IdentityServerHost.Quickstart.UI
{
    public class ClientController : Controller
    {
        private readonly ConfigurationDbContext _configDbContext;
        private readonly PersistedGrantDbContext _persistedGrantDbContext;

        public ClientController(ConfigurationDbContext configDbContext, PersistedGrantDbContext persistedGrantDbContext)
        {
            _configDbContext = configDbContext;
            _persistedGrantDbContext = persistedGrantDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _configDbContext.Clients
                                .Include(c => c.RedirectUris)
                                .Include(c => c.PostLogoutRedirectUris)
                                .Include(c => c.ClientSecrets)
                                .ToListAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                IgnoreNullValues = true,
                Converters =
                    {
                        new JsonStringEnumConverter()
                    }
            };

            var json = JsonSerializer.Serialize(clients, jsonOptions);

            var viewModels = JsonSerializer.Deserialize<List<ClientViewModel>>(json, jsonOptions);
            //return viewModels;
            return View("Client", viewModels);
            //return Ok(json);
        }

        public static string GenerateRandomString(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder sb = new StringBuilder();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                for (int i = 0; i < length; i++)
                {
                    int index = randomBytes[i] % validChars.Length;
                    sb.Append(validChars[index]);
                }
            }

            return sb.ToString();
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegenSecret(string Id)
        {
            var client = await _configDbContext.Clients.FirstOrDefaultAsync(c => c.Id == int.Parse(Id));

            if (client != null)
            {
                //// Generate a new client secret
                //string newSecret = GenerateRandomString(44);

                //// Update the client secret in the database
                //client.ClientSecrets[0].Value = newSecret;
                //await _configDbContext.SaveChangesAsync();

                // Optionally, you can perform any additional logic or return a success message
                return RedirectToAction("Client", "GetAll");
            }

            // Handle the case where the client is not found
            return NotFound();
        }


    }
}
