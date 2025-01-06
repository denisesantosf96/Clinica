using System.Data.SqlClient;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace Clinica.Controllers
{
    public class AgendamentoController : Controller
    {
        private readonly ILogger<AgendamentoController> _logger;
        private readonly DadosContext _context;
        const int itensPorPagina = 5;

        public AgendamentoController(ILogger<AgendamentoController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? pagina)
        {
            if (HttpContext.Session.GetInt32("IdClinica") == null)
            {
                HttpContext.Session.SetInt32("IdClinica", 1);
            }

            int idClinica = HttpContext.Session.GetInt32("IdClinica") ?? 0;
            int numeroPagina = (pagina ?? 1);

            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@idClinica", idClinica)
            };
            List<Models.Agendamento> agendamentos = _context.RetornarLista<Models.Agendamento>("sp_consultarAgendamento", parametros);

            ViewBagClinicas();
            return View(agendamentos.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Agendamento agendamento = new Models.Agendamento();
            int idClinica = HttpContext.Session.GetInt32("IdClinica") ?? 0;

            if (id > 0)
            {
                SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@identificacao", id)
            };
                agendamento = _context.ListarObjeto<Models.Agendamento>("sp_buscarAgendamentoPorId", parametros);
            }
            else
            {
                agendamento.IdClinica = idClinica;
            }

            ViewBagConsultas(agendamento.IdClinica);
            return View(agendamento);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Agendamento agendamento)
        {

            if (ModelState.IsValid)
            {
                if (agendamento.IdClinica == 0)
                {
                    agendamento.IdClinica = HttpContext.Session.GetInt32("IdClinica") ?? 0;
                }

                List<SqlParameter> parametros = new List<SqlParameter>(){
                    new SqlParameter("@IdConsulta", agendamento.IdConsulta),
                    new SqlParameter("@IdMedico", agendamento.IdMedico),
                    new SqlParameter("@IdPaciente", agendamento.IdPaciente),
                    new SqlParameter("@Data", agendamento.Data),
                    new SqlParameter("@Horario", agendamento.Horario),
                    new SqlParameter("@FormaPagamento", agendamento.FormaPagamento)
                };

                if (agendamento.Id > 0)
                {
                    parametros.Add(new SqlParameter("@Identificacao", agendamento.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(agendamento.Id > 0 ? "sp_atualizarAgendamento" : "sp_inserirAgendamento", parametros.ToArray());

                if (retorno.Mensagem == "Ok")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", retorno.Mensagem);

                }
            }

            ViewBagConsultas(agendamento.IdClinica);
            return View(agendamento);
        }

        public JsonResult Excluir(int id)
        {
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@Identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirAgendamento", parametros);
            return new JsonResult(new { Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(int idClinica, string textopesquisa)
        {

            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@idClinica", idClinica)
            };
            List<Consulta> consultas = _context.RetornarLista<Consulta>("sp_consultarAgendamento", parametros);

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

        private void ViewBagConsultas(int idClinica)
        {
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@idClinica", idClinica)
            };
            List<Models.Consulta> consultas = new List<Models.Consulta>();

            consultas = _context.RetornarLista<Models.Consulta>("sp_consultarConsulta", param);

            ViewBag.Consultas = consultas.Select(c => new SelectListItem()
            {
                Text = c.Id.ToString(),
                Value = c.Id.ToString()
            }).ToList();
        }

    }
}