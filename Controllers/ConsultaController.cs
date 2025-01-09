
using System.Data.SqlClient;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace Clinica.Controllers
{
    public class ConsultaController : Controller
    {
        private readonly ILogger<ConsultaController> _logger;
        private readonly DadosContext _context;
        const int itensPorPagina = 5;

        public ConsultaController(ILogger<ConsultaController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int? pagina, string textopesquisa)
        {

            var idClinica = 1;
            int numeroPagina = (pagina ?? 1);

            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@idClinica", idClinica)
            };
            List<Consulta> consultas = _context.RetornarLista<Consulta>("sp_consultarConsulta", parametros);

            // Se o textopesquisa não for nulo ou vazio, filtra os resultados
            if (!string.IsNullOrEmpty(textopesquisa))
            {
                consultas = consultas
                    .Where(c => c.Nome != null && c.Nome.Contains(textopesquisa, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBagClinicas();
            return View(consultas.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Consulta consulta = new Models.Consulta();
            if (id > 0)
            {
                SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@identificacao", id)
            };
                consulta = _context.ListarObjeto<Consulta>("sp_buscarConsultaPorId", parametros);
            }

            ViewBagClinicas();
            return View(consulta);
        }



        [HttpPost]
        public IActionResult Detalhe(Models.Consulta consulta)
        {

            if (string.IsNullOrEmpty(consulta.Nome))
            {
                ModelState.AddModelError("", "O nome deve ser preenchido");
            }
            if (string.IsNullOrEmpty(consulta.Descricao))
            {
                ModelState.AddModelError("", "A descrição deve ser preenchida");
            }

            if (ModelState.IsValid)
            {

                List<SqlParameter> parametros = new List<SqlParameter>(){

                    new SqlParameter("@IdClinica", consulta.IdClinica),
                    new SqlParameter("@Nome", consulta.Nome),
                    new SqlParameter("@Descricao", consulta.Descricao),
                    new SqlParameter("@Valor", consulta.Valor)
                };
                if (consulta.Id > 0)
                {
                    parametros.Add(new SqlParameter("@Identificacao", consulta.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(consulta.Id > 0 ? "sp_atualizarConsulta" : "sp_inserirConsulta", parametros.ToArray());

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
            return View(consulta);
        }

        public JsonResult Excluir(int id)
        {
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@Identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirConsulta", parametros);
            return new JsonResult(new { Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(int idClinica, string textopesquisa)
        {

            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@idClinica", idClinica)
            };
            List<Consulta> consultas = _context.RetornarLista<Consulta>("sp_consultarConsulta", parametros);

            if (!string.IsNullOrEmpty(textopesquisa))
            {
                consultas = consultas
                    .Where(c => c.Nome != null && c.Nome.Contains(textopesquisa, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            HttpContext.Session.SetInt32("@IdClinica", idClinica);

            return PartialView(consultas.ToPagedList(1, itensPorPagina));
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