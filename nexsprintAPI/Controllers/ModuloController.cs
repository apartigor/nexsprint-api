using nexsprintAPI.Data;
using nexsprintAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace nexsprintAPI.Controllers
{
    [ApiController]
    [Route("api/modulos")]
    public class ModuloController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public ModuloController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet] // - /api/modulos listar os modulos (catalogo)
        public async Task<ActionResult<IEnumerable<Modulo>>> ListarModulos()
        {
            var modulos = await _appDbContext.Modulos.ToListAsync();

            return Ok(modulos);
        }

        [HttpGet("{id}")] // - /api/modulos/{id} listar modulo especifico (listagem)
        public async Task<ActionResult<Modulo>> GetModuloById(int id)
        {
            var modulo = await _appDbContext.Modulos.FindAsync(id);

            // valida a existencia do modulo
            if (modulo == null)
                return NotFound("Modulo não encontrado.");

            return Ok(modulo);
        }

        [HttpGet("{id}/baixar/pdf")] // - /api/modulos/{id}/baixar/pdf baixa o pdf do modulo especifico (download)
        public async Task<ActionResult<Modulo>> GetPdfByModuloId(int id)
        {
            var modulo = await _appDbContext.Modulos.FindAsync(id);

            // valida a existencia do modulo
            if (modulo == null)
                return NotFound("Modulo não encontrado.");

            // constrói o caminho até o arquivo
            var urlPdf = Path.Combine("wwwroot", modulo.PDF_Url);

            // valida a existencia do arquivo
            if (!System.IO.File.Exists(urlPdf))
                return NotFound("Arquivo PDF não encontrado.");

            // le o arquivo
            var ler = await System.IO.File.ReadAllBytesAsync(urlPdf);

            // retorna baixando o arquivo
            return File(ler, "application/pdf", Path.GetFileName(urlPdf));
        }

        [HttpGet("{id}/ler/pdf")] // - /api/modulos/{id}/ler/pdf abre pdf do modulo especifico (leitura)
        public async Task<ActionResult<Modulo>> LerPdfDoModuloPorId(int id)
        {
            var modulo = await _appDbContext.Modulos.FindAsync(id);

            // valida a existencia do modulo
            if (modulo == null)
                return NotFound("Modulo não encontrado.");

            // constrói o caminho até o arquivo
            var urlPdf = Path.Combine("wwwroot", modulo.PDF_Url);

            // valida a existencia do arquivo
            if (!System.IO.File.Exists(urlPdf))
                return NotFound("Arquivo PDF não encontrado.");

            // le o arquivo
            var ler = await System.IO.File.ReadAllBytesAsync(urlPdf);

            // retorna baiixando o arquivo
            return File(ler, "application/pdf");
        }

    }
}