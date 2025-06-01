using System.Text.Json;
using nexsprintAPI.Data;
using nexsprintAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace nexsprintAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserModuloController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public UserModuloController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // DTO Simples para salvar o progresso
        public class ProgressoUpdateRequest
        {
            public double Pagina { get; set; }
        }

        [HttpPost("progresso/{moduloId}/{nomeUsuario}")]
        public async Task<IActionResult> SalvarProgresso(int moduloId, string nomeUsuario, [FromBody] ProgressoUpdateRequest request)
        {
            if (string.IsNullOrEmpty(nomeUsuario))
                return BadRequest("Nome do usuário é obrigatório.");

            // valida se o modulo existe
            var moduloExistente = await _appDbContext.Modulos.FindAsync(moduloId);
            if (moduloExistente == null)
                return BadRequest("Modulo não encontrado.");

            // valida se o usuario existe
            var usuarioEncontrado = await _appDbContext.Users.FindAsync(nomeUsuario);
            if (usuarioEncontrado == null)
                return BadRequest("Usuário não encontrado.");

            double paginaAtual = request.Pagina;

            if (paginaAtual > moduloExistente.TotalPaginas || paginaAtual < 0)
                return BadRequest("Dados colocados incorretamente!");

            // valida se ja existe um registro de progresso para o usuario
            var userModulo = await _appDbContext.UsersModulos
            .FirstOrDefaultAsync(um => um.NomeUsuario == nomeUsuario && um.ModuloId == moduloId);
            // se não existir, cria o progresso, e se ja existe apenas atualiza o progresso
            if (userModulo == null)
            {
                userModulo = new UserModulo
                {
                    NomeUsuario = nomeUsuario,
                    ModuloId = moduloId,
                    Progresso = paginaAtual
                };
                _appDbContext.UsersModulos.Add(userModulo);
            }
            else
            {
                userModulo.Progresso = paginaAtual;
            }

            await _appDbContext.SaveChangesAsync();
            return Ok("Progresso salvo com sucesso.");
        }

        [HttpGet("progresso/{moduloId}/{nomeUsuario}")]
        public async Task<IActionResult> GetProgresso(int moduloId, string nomeUsuario)
        {
            if (string.IsNullOrEmpty(nomeUsuario))
                return BadRequest("Nome do usuário é obrigatório.");

            // valida se o id do modulo existe
            var moduloExistente = await _appDbContext.Modulos.FindAsync(moduloId);
            if (moduloExistente == null)
                return BadRequest("Modulo não encontrado.");

            // valida se o user existe
            var usuarioEncontrado = await _appDbContext.Users.FindAsync(nomeUsuario);
            if (usuarioEncontrado == null)
                return BadRequest("Usuário não encontrado.");

            var userModulo = await _appDbContext.UsersModulos
                .FirstOrDefaultAsync(um => um.NomeUsuario == nomeUsuario && um.ModuloId == moduloId);

            if (userModulo == null)
                return NotFound("Progresso não encontrado");

            return Ok(userModulo.Progresso);
        }

        [HttpGet("meus-modulos/{nomeUsuario}")]
        public async Task<IActionResult> GetModulosByUser(string nomeUsuario)
        {
            var modulosUsuario = await _appDbContext.UsersModulos
            .Where(um => um.NomeUsuario == nomeUsuario)
            .Include(um => um.Modulo)
            .Select(um => new
            {
                um.ModuloId,
                um.Modulo.Nome,
                um.Modulo.PDF_Url,
                um.Modulo.TotalPaginas,
                um.Modulo.Capa_Url,
                Progresso = um.Progresso
            })
            .ToListAsync();
            return Ok(modulosUsuario);
        }
    }
}