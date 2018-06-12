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
        public IEnumerable<Usuario> GetUsuario()
        {
            return _context.Usuario;
        }

        // GET: api/APIUsuarios/5
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
            
   
            // Asumamos que tenemos un usuario válido
            var user = new Usuario
            {
                Nombre = "Eduardo",
                Contraseña= "Admin",
                //UserId = Guid.NewGuid().ToString()
            };

            /* Creamos la información que queremos transportar en el token,
             * en nuestro los datos del usuario
             */
            var claims = new[]
            {
            new Claim("UserData", JsonConvert.SerializeObject(user))
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
          //  return Ok(usuario);
        }
        [Authorize]
        // PUT: api/APIUsuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario([FromRoute] int id, [FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != usuario.Id)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/APIUsuarios
        [HttpPost]
        public async Task<IActionResult> PostUsuario([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // DELETE: api/APIUsuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario([FromRoute] int id)
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

            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(usuario);
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }
    }
}