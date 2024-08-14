
using System.Data.SqlClient;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace Clinica.Controllers
{
    public class PacienteController : Controller
    {
        private readonly ILogger<PacienteController> _logger;
        private readonly DadosContext _context;
        const int itensPorPagina = 5;

        public PacienteController(ILogger<PacienteController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? pagina)
        {
            var idClinica = 1;
            int numeroPagina = (pagina ?? 1);

            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@idClinica", idClinica)
            };
            List<Paciente> pacientes = _context.RetornarLista<Paciente>("sp_consultarPaciente", parametros);

            ViewBagClinicas();
            return View(pacientes.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id, int idClinica)
        {
            Models.Paciente paciente = new Models.Paciente();
            if (id > 0)
            {
                SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@identificacao", id)
            };
                paciente = _context.ListarObjeto<Models.Paciente>("sp_buscarPacientePorId", parametros);
            }
            else
            {
                paciente.IdClinica = idClinica;
            }

            ViewBagConsultas(id > 0 ? paciente.IdClinica : idClinica);
            return View(paciente);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Paciente paciente)
        {

            if (string.IsNullOrEmpty(paciente.Nome))
            {
                ModelState.AddModelError("", "O nome não pode ser vazio");
            }
            if (string.IsNullOrEmpty(paciente.Endereco))
            {
                ModelState.AddModelError("", "O endereco deve ser informado");
            }
            if (string.IsNullOrEmpty(paciente.Bairro))
            {
                ModelState.AddModelError("", "O bairro deve ser informado");
            }
            if (string.IsNullOrEmpty(paciente.Cidade))
            {
                ModelState.AddModelError("", "A cidade deve ser informado");
            }
            if (string.IsNullOrEmpty(paciente.Estado))
            {
                ModelState.AddModelError("", "O estado deve ser informado");
            }
            if (string.IsNullOrEmpty(paciente.Pais))
            {
                ModelState.AddModelError("", "O país deve ser informado");
            }
            if (string.IsNullOrEmpty(paciente.CEP))
            {
                ModelState.AddModelError("", "O CEP deve ser informado");
            }
            if (string.IsNullOrEmpty(paciente.Telefone))
            {
                ModelState.AddModelError("", "O telefone deve ser informado");
            }

            if (ModelState.IsValid)
            {

                List<SqlParameter> parametros = new List<SqlParameter>(){
                    new SqlParameter("@Nome", paciente.Nome),
                    new SqlParameter("@CPF", paciente.CPF),
                    new SqlParameter("@RG", paciente.RG),
                    new SqlParameter("@Telefone", paciente.Telefone),
                    new SqlParameter("@Email", paciente.Email),
                    new SqlParameter("@Endereco", paciente.Endereco),
                    new SqlParameter("@Numero", paciente.Numero),
                    new SqlParameter("@Complemento", paciente.Complemento),
                    new SqlParameter("@Bairro", paciente.Bairro),
                    new SqlParameter("@Cidade", paciente.Cidade),
                    new SqlParameter("@Estado", paciente.Estado),
                    new SqlParameter("@Pais", paciente.Pais),
                    new SqlParameter("@CEP", paciente.CEP),
                    new SqlParameter("@DataNascimento", paciente.DataNascimento),
                    new SqlParameter("@IdConsulta", paciente.IdConsulta)

                };
                if (paciente.Id > 0)
                {
                    parametros.Add(new SqlParameter("@Id", paciente.Id));
                    parametros.Add(new SqlParameter("@Acao", 2));
                    parametros.Add(new SqlParameter("@Opcao", "paciente"));
                }
                else
                {
                    parametros.Add(new SqlParameter("@Acao", 1));
                    parametros.Add(new SqlParameter("@Opcao", "paciente"));
                }

                var retorno = _context.ListarObjeto<RetornoProcedure>("sp_salvarPessoa", parametros.ToArray());

                if (retorno.Mensagem == "Ok")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", retorno.Mensagem);
                }
            }

            ViewBagConsultas(paciente.IdClinica);
            return View(paciente);
        }

        public JsonResult Excluir(int id)
        {
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@Id", id),
                new SqlParameter("@Acao", 0),
                new SqlParameter("@Opcao", "paciente")
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_salvarPessoa", parametros);
            return new JsonResult(new { Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(int idClinica)
        {
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@idClinica", idClinica)
            };
            List<Models.Paciente> pacientes = _context.RetornarLista<Models.Paciente>("sp_consultarPaciente", parametros);

            HttpContext.Session.SetInt32("IdClinica", idClinica);

            return PartialView(pacientes.ToPagedList(1, itensPorPagina));
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
                Text = c.Id + " - " + c.Nome + " - " + c.Valor,
                Value = c.Id.ToString()
            }).ToList();
        }


    }
}