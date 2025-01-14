using Microsoft.Data.SqlClient;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace Clinica.Controllers
{
    public class EspecialidadeController : Controller
    {
        private readonly ILogger<EspecialidadeController> _logger;
        private readonly DadosContext _context;
        const int itensPorPagina = 5;
        public EspecialidadeController(ILogger<EspecialidadeController> logger, DadosContext context)
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
            List<Models.Especialidade> especialidades = _context.RetornarLista<Models.Especialidade>("sp_consultarEspecialidade", parametros);
            return View(especialidades.ToPagedList(numeroPagina, itensPorPagina));
        }
        public IActionResult Detalhe(int id)
        {
            Models.Especialidade especialidade = new Models.Especialidade();
            if (id > 0)
            {
                SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@identificacao", id)
            };
                especialidade = _context.ListarObjeto<Models.Especialidade>("sp_buscarEspecialidadePorId", parametros);
            }
            return View(especialidade);
        }
        [HttpPost]
        public IActionResult Detalhe(Models.Especialidade especialidade)
        {
            if (string.IsNullOrEmpty(especialidade.Nome))
            {
                ModelState.AddModelError("", "O nome não pode ser vazio");
            }
            if (string.IsNullOrEmpty(especialidade.Tipo))
            {
                ModelState.AddModelError("", "O Telefone deve ser informado");
            }
            if (ModelState.IsValid)
            {
                List<SqlParameter> parametros = new List<SqlParameter>(){
                    new SqlParameter("@Nome", especialidade.Nome),
                    new SqlParameter("@Tipo", especialidade.Tipo)
                };
                if (especialidade.Id > 0)
                {
                    parametros.Add(new SqlParameter("@Identificacao", especialidade.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(especialidade.Id > 0 ? "sp_atualizarEspecialidade" : "sp_inserirEspecialidade", parametros.ToArray());
                if (retorno.Mensagem == "Ok")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", retorno.Mensagem);
                }
            }
            return View(especialidade);
        }
        public JsonResult Excluir(int id)
        {
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@Identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirEspecialidade", parametros);
            return new JsonResult(new { Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }
        public PartialViewResult ListaPartialView(string nome)
        {
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@nome", nome)
            };
            List<Models.Especialidade> especialidades = _context.RetornarLista<Models.Especialidade>("sp_consultarEspecialidade", parametros);
            if (string.IsNullOrEmpty(nome))
            {
                HttpContext.Session.Remove("TextoPesquisa");
            }
            else
            {
                HttpContext.Session.SetString("TextoPesquisa", nome);
            }
            return PartialView(especialidades.ToPagedList(1, itensPorPagina));
        }
    }
}