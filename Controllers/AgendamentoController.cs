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

        public IActionResult Index(int? pagina, string textopesquisa)
        {
            if (HttpContext.Session.GetInt32("IdClinica") == null)
            {
                HttpContext.Session.SetInt32("IdClinica", 1);
            }

            int idClinica = HttpContext.Session.GetInt32("IdClinica") ?? 0;
            int numeroPagina = (pagina ?? 1);

            _logger.LogInformation("ID da Clínica: {IdClinica}", idClinica);

            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@idClinica", idClinica)
            };
            List<Models.Agendamento> agendamentos = _context.RetornarLista<Models.Agendamento>("sp_consultarAgendamento", parametros);

            if (!string.IsNullOrEmpty(textopesquisa))
            {
                agendamentos = agendamentos
                    .Where(c => c.NomePaciente != null && c.NomePaciente.Contains(textopesquisa, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

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

            ViewBagClinicas();
            ViewBagConsultas(agendamento.IdClinica);
            ViewBagMedicos(agendamento.Especializacao ?? "");
            ViewBagPacientes(agendamento.Nome ?? "");  // Passa string vazia caso agendamento.Nome seja nulo
            return View(agendamento);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Agendamento agendamento)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("NomeConsulta");
            ModelState.Remove("NomeMedico");
            ModelState.Remove("NomePaciente");
            ModelState.Remove("NomeClinica");
            ModelState.Remove("Especializacao");

            if (ModelState.IsValid)
            {
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
            ViewBagMedicos(agendamento.Especializacao ?? "");
            ViewBagPacientes(agendamento.Nome ?? "");
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
            List<Agendamento> agendamentos = _context.RetornarLista<Agendamento>("sp_consultarAgendamento", parametros);

            if (!string.IsNullOrEmpty(textopesquisa))
            {
                agendamentos = agendamentos
                    .Where(c => c.NomePaciente != null && c.NomePaciente.Contains(textopesquisa, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            HttpContext.Session.SetInt32("IdClinica", idClinica);

            return PartialView(agendamentos.ToPagedList(1, itensPorPagina));
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
                Text = c.Id + " - " + c.Nome,
                Value = c.Id.ToString()
            }).ToList();
        }

        private void ViewBagMedicos(string especializacao)
        {
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@especializacao", especializacao)
            };
            List<Models.Medico> medicos = new List<Models.Medico>();

            medicos = _context.RetornarLista<Models.Medico>("sp_consultarMedico", param);

            ViewBag.Medicos = medicos.Select(c => new SelectListItem()
            {
                Text = c.Id + " - " + c.Nome,
                Value = c.Id.ToString()
            }).ToList();
        }

        private void ViewBagPacientes(string nome)
        {
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@nome", nome)
            };
            List<Models.Paciente> pacientes = new List<Models.Paciente>();

            pacientes = _context.RetornarLista<Models.Paciente>("sp_consultarPaciente", param);

            ViewBag.Pacientes = pacientes.Select(c => new SelectListItem()
            {
                Text = c.Id + " - " + c.Nome,
                Value = c.Id.ToString()
            }).ToList();
        }

    }
}