using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace appaspcore.Controllers
{
    [Route("painel/admin")]
    public class AdminController : Controller
    {
        [HttpGet("principal/{num:int?}/{nome}")]
        public IActionResult Index(int num, string nome)
        {
            return Content("O número é " + num + " E o nome é " + nome);
        }

        [HttpGet("son")]
        public IActionResult SchoolOfNet()
        {
            string nome = Request.Query["nome"];
            return Content("Aprendendo ASP.NET Core na School of Net" + nome);
        }

        [HttpGet("view")]
        public IActionResult Visualizar()
        {
            ViewData["helloword"] = true;
            ViewData["nome"] = "Guilherme";
            return View("nada");
        }

        [HttpGet("form")]
        public IActionResult Form()
        {
            return View();
        }

        [HttpPost("dados")]
        public IActionResult dados()
        {
            StringValues nome;
            StringValues email;

            Request.Form.TryGetValue("nome", out nome);
            Request.Form.TryGetValue("email", out email);

            return Content("Formulário enviado " + nome + " " + email);
        }
    }
}