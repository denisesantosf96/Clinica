using System.Data.SqlClient;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList.Extensions;

namespace Clinica.Controllers
{
    public class TipoAtendimentoController : Controller
    {
        private readonly ILogger<TipoAtendimentoController> _logger;
        private readonly DadosContext _context;
        const int itensPorPagina = 5;

        public TipoAtendimentoController(ILogger<TipoAtendimentoController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? pagina, string textopesquisa)
        {

            int idClinica = 1;
            int numeroPagina = (pagina ?? 1);

            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@idClinica", idClinica)
            };
            List<Models.TipoAtendimento> atendimentos = _context.RetornarLista<Models.TipoAtendimento>("sp_consultarTipoAtendimento", parametros);

            // Se o textopesquisa não for nulo ou vazio, filtra os resultados
            if (!string.IsNullOrEmpty(textopesquisa))
            {
                atendimentos = atendimentos
                    .Where(c => c.NomeEspecialidade != null && c.NomeEspecialidade.Contains(textopesquisa, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBagClinicas();
            return View(atendimentos.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.TipoAtendimento atendimento = new Models.TipoAtendimento();

            if (id > 0)
            {
                SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@identificacao", id)
            };
                atendimento = _context.ListarObjeto<Models.TipoAtendimento>("sp_buscarTipoAtendimentoPorId", parametros);
            }

            ViewBagClinicas();
            return View(atendimento);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.TipoAtendimento atendimento)
        {
            if (string.IsNullOrEmpty(atendimento.Descricao))
            {
                ModelState.AddModelError("", "O nome não pode ser vazio");
            }

            if (ModelState.IsValid)
            {

                List<SqlParameter> parametros = new List<SqlParameter>(){
                    new SqlParameter("@IdClinica", atendimento.IdClinica),
                    new SqlParameter("@IdEspecialidade", atendimento.IdEspecialidade),
                    new SqlParameter("@IdMedico", atendimento.IdMedico),
                    new SqlParameter("@Descricao", atendimento.Descricao),
                    new SqlParameter("@Valor", atendimento.Valor)
                };
                if (atendimento.Id > 0)
                {
                    parametros.Add(new SqlParameter("@Identificacao", atendimento.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(atendimento.Id > 0 ? "sp_atualizarTipoAtendimento" : "sp_inserirTipoAtendimento", parametros.ToArray());

                if (retorno.Mensagem == "Ok")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", retorno.Mensagem);
                }
            }

            ViewBagClinicas();
            return View(atendimento);
        }

        public JsonResult Excluir(int id)
        {
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@Identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirTipoAtendimento", parametros);
            return new JsonResult(new { Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(int idClinica, string textopesquisa)
        {

            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@IdClinica", idClinica)
            };
            List<Models.TipoAtendimento> atendimentos = _context.RetornarLista<Models.TipoAtendimento>("sp_consultarTipoAtendimento", parametros);

            if (!string.IsNullOrEmpty(textopesquisa))
            {
                atendimentos = atendimentos
                    .Where(c => c.NomeEspecialidade != null && c.NomeEspecialidade.Contains(textopesquisa, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            HttpContext.Session.SetInt32("@IdClinica", idClinica);

            return PartialView(atendimentos.ToPagedList(1, itensPorPagina));
        }

        private void ViewBagClinicas()
        {
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@nome", "")
            };
            List<Models.Clinica> clinicas = new List<Models.Clinica>();
            clinicas = _context.RetornarLista<Models.Clinica>("sp_consultarClinica", param);

            ViewBag.Clinicas = clinicas.Select(c => new SelectListItem()
            {
                Text = c.Nome,
                Value = c.Id.ToString()
            }).ToList();
        }
        
    }
}