using System.Data.SqlClient;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace Clinica.Controllers
{
    public class MedicoController : Controller
    {
        private readonly ILogger<MedicoController> _logger;
        private readonly DadosContext _context;
        const int itensPorPagina = 5;

        public MedicoController(ILogger<MedicoController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? pagina)
        {
            var nome = "";
            int numeroPagina = (pagina ?? 1);

            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@nome", nome)
            };
            List<Models.Medico> medicos = _context.RetornarLista<Models.Medico>("sp_consultarMedico", parametros);

            ViewBagClinicas();
            return View(medicos.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Medico medico = new Models.Medico();
            if (id > 0)
            {
                SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@identificacao", id)
            };
                medico = _context.ListarObjeto<Models.Medico>("sp_buscarMedicoPorId", parametros);
            }

            return View(medico);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Medico medico)
        {

            if (string.IsNullOrEmpty(medico.Nome))
            {
                ModelState.AddModelError("", "O nome não pode ser vazio");
            }
            if (string.IsNullOrEmpty(medico.Endereco))
            {
                ModelState.AddModelError("", "O endereco deve ser informado");
            }
            if (string.IsNullOrEmpty(medico.Bairro))
            {
                ModelState.AddModelError("", "O bairro deve ser informado");
            }
            if (string.IsNullOrEmpty(medico.Cidade))
            {
                ModelState.AddModelError("", "A cidade deve ser informado");
            }
            if (string.IsNullOrEmpty(medico.Estado))
            {
                ModelState.AddModelError("", "O estado deve ser informado");
            }
            if (string.IsNullOrEmpty(medico.Pais))
            {
                ModelState.AddModelError("", "O país deve ser informado");
            }
            if (string.IsNullOrEmpty(medico.CEP))
            {
                ModelState.AddModelError("", "O CEP deve ser informado");
            }
            if (string.IsNullOrEmpty(medico.Telefone))
            {
                ModelState.AddModelError("", "O telefone deve ser informado");
            }
            if (string.IsNullOrEmpty(medico.Especializacao))
            {
                ModelState.AddModelError("", "A especialização deve ser informada");
            }
            if (string.IsNullOrEmpty(medico.CRM))
            {
                ModelState.AddModelError("", "O CRM deve ser informado");
            }

            if (ModelState.IsValid)
            {

                List<SqlParameter> parametros = new List<SqlParameter>(){
                    new SqlParameter("@Nome", medico.Nome),
                    new SqlParameter("@CPF", medico.CPF),
                    new SqlParameter("@RG", medico.RG),
                    new SqlParameter("@Telefone", medico.Telefone),
                    new SqlParameter("@Email", medico.Email),
                    new SqlParameter("@Endereco", medico.Endereco),
                    new SqlParameter("@Numero", medico.Numero),
                    new SqlParameter("@Complemento", medico.Complemento),
                    new SqlParameter("@Bairro", medico.Bairro),
                    new SqlParameter("@Cidade", medico.Cidade),
                    new SqlParameter("@Estado", medico.Estado),
                    new SqlParameter("@Pais", medico.Pais),
                    new SqlParameter("@CEP", medico.CEP),
                    new SqlParameter("@DataNascimento", medico.DataNascimento),
                    new SqlParameter("@Especializacao", medico.Especializacao),
                    new SqlParameter("@CRM", medico.CRM)

                };
                if (medico.Id > 0)
                {
                    parametros.Add(new SqlParameter("@Id", medico.Id));
                    parametros.Add(new SqlParameter("@Acao", 2));
                    parametros.Add(new SqlParameter("@Opcao", "medico"));
                }
                else
                {
                    parametros.Add(new SqlParameter("@Acao", 1));
                    parametros.Add(new SqlParameter("@Opcao", "medico"));
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

            return View(medico);
        }

        public JsonResult Excluir(int id)
        {
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@Id", id),
                new SqlParameter("@Acao", 0),
                new SqlParameter("@Opcao", "medico")
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_salvarPessoa", parametros);
            return new JsonResult(new { Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string nome)
        {
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@nome", nome)
            };
            List<Models.Medico> medicos = _context.RetornarLista<Models.Medico>("sp_consultarMedico", parametros);

            if (string.IsNullOrEmpty(nome)){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
                HttpContext.Session.SetString("TextoPesquisa", nome);
            }

            return PartialView(medicos.ToPagedList(1, itensPorPagina));
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