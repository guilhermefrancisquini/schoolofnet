using apirest.Data;
using apirest.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using apirest.HATEOAS;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace apirest.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public ProdutosController(ApplicationDbContext database)
        {
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/api/v1/Produtos");
            HATEOAS.AddAction("GET_INFO", "GET");
            HATEOAS.AddAction("DELETE_PRODUCT", "DELETE");
            HATEOAS.AddAction("EDIT_PRODUCT", "PACTH");
        }

        [HttpGet("teste")]
        public IActionResult TesteClaims()
        {            
            return Ok(HttpContext.User.Claims.First(claim => claim.Type.ToString().Equals("id", StringComparison.CurrentCultureIgnoreCase)).Value);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var produtos = database.Produtos.ToList();

            List<ProdutoContainer> produtosHATEOAS = new List<ProdutoContainer>();
            foreach (var produto in produtos)
            {
                ProdutoContainer produtoHATEOAS = new ProdutoContainer();
                produtoHATEOAS.produto = produto;
                produtoHATEOAS.links = HATEOAS.GetActions(produto.Id.ToString());
                produtosHATEOAS.Add(produtoHATEOAS);
            }
            // return BadRequest()
            // return NotFound() // 404
            return Ok(new { produtosHATEOAS }); //Status code = 200 && dados
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try {
                // var produto = database.Produtos.Where( p => p.Id == id).FirstOrDefault();
                Produto produto = database.Produtos.First( p => p.Id == id);
                ProdutoContainer produtoHATEOAS = new ProdutoContainer();
                produtoHATEOAS.produto = produto;
                produtoHATEOAS.links = HATEOAS.GetActions(produto.Id.ToString());

                return Ok(new { produtoHATEOAS }); //Status code = 200 && dados
            }
            catch
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
            
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoTemp pTemp)
        {
            /* Validação */
            if(pTemp.Preco <= 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult( new { msg="O preço do produto não pode ser menor ser menor ou igual a 0." });
            }

            if(pTemp.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult( new { msg="O nome do produto do produto precisa ter mais de 1 caracter." });
            }

            Produto p = new Produto();
            p.Nome = pTemp.Nome;
            p.Preco = pTemp.Preco;

            database.Produtos.Add(p);
            database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult("");
            // return Ok( new {
            //     msg = "Produto criado com sucesso!"
            // }); //Status code = 200 && dados
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try {
                Produto produto = database.Produtos.First( p => p.Id == id);
                
                database.Produtos.Remove(produto);
                database.SaveChanges();

                return Ok("");
            }
            catch
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
        }
        
        [HttpPatch]
        public IActionResult Patch([FromBody]  Produto produto)
        {
            if(produto.Id > 0)
            {
                try 
                {
                    var p = database.Produtos.FirstOrDefault(ptemp => ptemp.Id == produto.Id);

                    p.Nome = produto.Nome != null ? produto.Nome : p.Nome;
                    p.Preco = produto.Preco != 0 ? produto.Preco : p.Preco;

                    database.SaveChanges();

                    return Ok();
                }
                catch (Exception e)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult( new { msg = e.Message });
                }
            }
            else 
            {
                Response.StatusCode = 400;
                return new ObjectResult( new { msg = "O Id do produto é inválido." });
            }
        }
    }

    public class ProdutoTemp 
    {
        public string Nome { get; set; }
        public float Preco { get; set; }
    }

    public class ProdutoContainer 
    {
        public Produto produto;
        public Link[] links;
    }
}