
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

        public IActionResult Index(int? pagina, string textopesquisa)
        {
        
            
            var nome = HttpContext.Session.GetString("TextoPesquisa") ?? string.Empty; 
            int numeroPagina = (pagina ?? 1);

            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@nome", nome)
            };
            List<Paciente> pacientes = _context.RetornarLista<Paciente>("sp_consultarPaciente", parametros);

            ViewBagClinicas();
            return View(pacientes.ToPagedList(numeroPagina, itensPorPagina));
        }

        [HttpGet]
        public IActionResult Detalhe(int id)
        {
            Models.Paciente paciente = new Models.Paciente();

            if (id > 0)
            {
                SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@identificacao", id)
            };
                paciente = _context.ListarObjeto<Models.Paciente>("sp_buscarPacientePorId", parametros);
            }

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
                    new SqlParameter("@DataNascimento", paciente.DataNascimento)
                };

                if (paciente.Id > 0)
                {
                    parametros.Add(new SqlParameter("@Identificacao", paciente.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(paciente.Id > 0 ? "sp_atualizarPaciente" : "sp_inserirPaciente", parametros.ToArray());

                if (retorno.Mensagem == "Ok")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", retorno.Mensagem);

                }
            }

            return View(paciente);
        }

        public JsonResult Excluir(int id)
        {
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@Identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirPaciente", parametros);
            return new JsonResult(new { Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }


        public PartialViewResult ListaPartialView(string nome)
        {
            
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@nome",  nome)
            };
            List<Models.Paciente> pacientes = _context.RetornarLista<Models.Paciente>("sp_consultarPaciente", parametros);

            if (string.IsNullOrEmpty(nome))
            {
                HttpContext.Session.Remove("TextoPesquisa");
            }
            else
            {
                HttpContext.Session.SetString("TextoPesquisa", nome);
            }

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

    }
}