using Microsoft.Data.SqlClient;
using Clinica.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace Clinica.Controllers
{
    public class ClinicaController : Controller
    {
        private readonly ILogger<ClinicaController> _logger;
        private readonly DadosContext _context ;
        const int itensPorPagina = 5;
  
        public ClinicaController(ILogger<ClinicaController> logger, DadosContext context)
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
            List<Models.Clinica> clinicas = _context.RetornarLista<Models.Clinica>("sp_consultarClinica", parametros);
            
            return View(clinicas.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Clinica clinica = new Models.Clinica();
            if (id > 0)  {
                SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@identificacao", id)
            };
                clinica = _context.ListarObjeto<Models.Clinica>("sp_buscarClinicaPorId", parametros); 
            }
                   
            return View(clinica);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Clinica clinica){
            if(string.IsNullOrEmpty(clinica.Nome)){
                ModelState.AddModelError("", "O nome não pode ser vazio");
            } 
            if(string.IsNullOrEmpty(clinica.Telefone)){
                ModelState.AddModelError("", "O Telefone deve ser informado");
            }
            if(string.IsNullOrEmpty(clinica.Endereco)){
                ModelState.AddModelError("", "O Endereço deve ser informado");
            }
            if(string.IsNullOrEmpty(clinica.Bairro)){
                ModelState.AddModelError("", "O Bairro deve ser informado");
            }
            if(string.IsNullOrEmpty(clinica.Cidade)){
                ModelState.AddModelError("", "O Cidade deve ser informado");
            }
            if(string.IsNullOrEmpty(clinica.Estado)){
                ModelState.AddModelError("", "O Estado deve ser informado");
            }
            if(string.IsNullOrEmpty(clinica.Pais)){
                ModelState.AddModelError("", "O País deve ser informado");
            }
            if(string.IsNullOrEmpty(clinica.CEP)){
                ModelState.AddModelError("", "O CEP deve ser informado");
            }


            if(ModelState.IsValid){
           
                List<SqlParameter> parametros = new List<SqlParameter>(){
                    new SqlParameter("@Nome", clinica.Nome),
                    new SqlParameter("@Telefone", clinica.Telefone),
                    new SqlParameter("@Endereco", clinica.Endereco),
                    new SqlParameter("@Numero", clinica.Numero),
                    new SqlParameter("@Complemento", clinica.Complemento),
                    new SqlParameter("@Bairro", clinica.Bairro),
                    new SqlParameter("@Cidade", clinica.Cidade),
                    new SqlParameter("@Estado", clinica.Estado),
                    new SqlParameter("@Pais", clinica.Pais),
                    new SqlParameter("@CEP", clinica.CEP)
                };
                if (clinica.Id > 0){
                    parametros.Add(new SqlParameter("@Identificacao", clinica.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(clinica.Id > 0? "sp_atualizarClinica" : "sp_inserirClinica", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                }
            }
            return View(clinica);
        }

        public JsonResult Excluir(int id){
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@Identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirClinica", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string nome){
            SqlParameter[] parametros = new SqlParameter[]{
                new SqlParameter("@nome", nome)
            };
            List<Models.Clinica> clinicas = _context.RetornarLista<Models.Clinica>("sp_consultarClinica", parametros);
            if (string.IsNullOrEmpty(nome)){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
            HttpContext.Session.SetString("TextoPesquisa", nome);
            }

            return PartialView(clinicas.ToPagedList(1, itensPorPagina));
        }
    }
}