using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using nexsprintAPI.Data;
using nexsprintAPI.Models;

namespace nexsprintAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public AutenticacaoController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost("cadastro")]
        public async Task<IActionResult> Cadastro([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.NomeUsuario) || string.IsNullOrEmpty(user.SenhaHash) || string.IsNullOrEmpty(user.Email))
                return BadRequest("Nome de usuário, senha e email são obrigatórios!");

            var usuarioExistente = await _appDbContext.Users
                .FirstOrDefaultAsync(u => u.NomeUsuario == user.NomeUsuario || u.Email == user.Email);

            if (usuarioExistente != null)
                return BadRequest("Usuário ou email já existente!");

            user.SenhaHash = BCrypt.Net.BCrypt.HashPassword(user.SenhaHash);

            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            return Ok("Cadastro realizado com sucesso!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.SenhaHash))
                return BadRequest("Senha incorreta!");

            var usuarioEncontrado = await _appDbContext.Users
                .FirstOrDefaultAsync(u => u.NomeUsuario == user.NomeUsuario);

            if (usuarioEncontrado == null)
                return Unauthorized("Usuário não encontrado!");


            if (!BCrypt.Net.BCrypt.Verify(user.SenhaHash, usuarioEncontrado.SenhaHash))
                return Unauthorized("Senha incorreta!");

            return Ok("Login realizado com sucesso!");
        }

    }
}