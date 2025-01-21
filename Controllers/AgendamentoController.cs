using Microsoft.Data.SqlClient;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList.Extensions;

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
            // Verifica se há um valor salvo na sessão para "IdClinica". 
            // Se não houver (ou seja, for nulo), define o valor padrão como 1.
            if (HttpContext.Session.GetInt32("IdClinica") == null)
            {
                HttpContext.Session.SetInt32("IdClinica", 1);
            }

            // Obtém o valor de "IdClinica" da sessão. 
            // Se o valor for nulo, utiliza o valor padrão 0 (através do operador de coalescência nula '??').
            int idClinica = HttpContext.Session.GetInt32("IdClinica") ?? 0;
            int numeroPagina = (pagina ?? 1);

            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@idClinica", idClinica)
            };
            List<Models.Agendamento> agendamentos = _context.RetornarLista<Models.Agendamento>("sp_consultarAgendamento", parametros);

            // Se o textopesquisa não for nulo ou vazio, filtra os resultados
            if (!string.IsNullOrEmpty(textopesquisa))
            {
                agendamentos = agendamentos
                    .Where(c => c.NomeEspecialidade != null && c.NomeEspecialidade.Contains(textopesquisa, StringComparison.OrdinalIgnoreCase))
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
            ViewBagAtendimentos(agendamento.IdClinica);
            ViewBagPacientes();
            return View(agendamento);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Agendamento agendamento)
        {
            if (ModelState.IsValid)
            {
                List<SqlParameter> parametros = new List<SqlParameter>(){
                    new SqlParameter("@IdTipoAtendimento", agendamento.IdTipoAtendimento),
                    new SqlParameter("@IdPaciente", agendamento.IdPaciente),
                    new SqlParameter("@DataHora", agendamento.DataHora),
                    new SqlParameter("@DataHoraConfirmacao", (object)agendamento.DataHoraConfirmacao ?? DBNull.Value),
                    new SqlParameter("@EstevePresente", agendamento.EstevePresente)
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

            ViewBagClinicas();
            ViewBagAtendimentos(agendamento.IdClinica);
            ViewBagPacientes();
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
                new SqlParameter("@IdClinica", idClinica)
            };
            List<Models.Agendamento> agendamentos = _context.RetornarLista<Models.Agendamento>("sp_consultarAgendamento", parametros);

            if (!string.IsNullOrEmpty(textopesquisa))
            {
                agendamentos = agendamentos
                    .Where(c => c.NomeEspecialidade != null && c.NomeEspecialidade.Contains(textopesquisa, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            HttpContext.Session.SetInt32("IdClinica", idClinica);

            return PartialView(agendamentos.ToPagedList(1, itensPorPagina));
        }

        public JsonResult CarregarMedicos(int idEspecialidade)
        {
            SqlParameter[] param = new SqlParameter[] {
            new SqlParameter("@IdEspecialidade", idEspecialidade)
            };

            List<Models.Medico> medicos = _context.RetornarLista<Models.Medico>("sp_consultarMedicoPorEspecialidade", param);

            var medicosSelectList = medicos.Select(c => new
            {
                Value = c.Id,
                Text = c.Nome
            }).ToList();

            return Json(medicosSelectList);
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

        private void ViewBagAtendimentos(int idClinica)
        {
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@idClinica", idClinica)
            };
            List<Models.TipoAtendimento> atendimentos = new List<Models.TipoAtendimento>();

            atendimentos = _context.RetornarLista<Models.TipoAtendimento>("sp_consultarTipoAtendimento", param);

            ViewBag.Atendimentos = atendimentos.Select(c => new SelectListItem()
            {
                Text = c.Id + " - " + c.NomeEspecialidade + " - " + c.NomeMedico,
                Value = c.Id.ToString()
            }).ToList();
        }

        private void ViewBagPacientes()
        {
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@nome", "")
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