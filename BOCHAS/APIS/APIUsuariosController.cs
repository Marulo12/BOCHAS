using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;
using System.Security.Claims;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BOCHAS.Controllers
{
    [Produces("application/json")]
    [Route("api/APIUsuarios")]
    public class APIUsuariosController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BOCHASContext _context;

        public APIUsuariosController(BOCHASContext context , IConfiguration configuration )
        {
            _configuration = configuration;
            
            _context = context;
        }

        
        // GET: api/APIUsuarios
        [HttpGet]
        [Authorize(AuthenticationSchemes= JwtBearerDefaults.AuthenticationScheme)]
        public IEnumerable<Usuario> GetUsuario()
        {
            return _context.Usuario.ToList();
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuario.SingleOrDefaultAsync(m => m.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }
                                           
            var claims = new[]
            {
            new Claim("UserData", JsonConvert.SerializeObject(usuario))
        };

            // Generamos el Token
            var token = new JwtSecurityToken
            (
                issuer: _configuration["ApiAuth:Issuer"],
                audience: _configuration["ApiAuth:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(60),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApiAuth:SecretKey"])),
                        SecurityAlgorithms.HmacSha256)
            );
            

            // Retornamos el token
            return Ok(
               new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token)
                }
            );
         
        }
      



    }
}