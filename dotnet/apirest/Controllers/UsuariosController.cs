using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using apirest.Data;
using apirest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace apirest.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public UsuariosController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpPost("registro")]
        public IActionResult Registro([FromBody] Usuario usuario)
        {
            database.Usuarios.Add(usuario);
            database.SaveChanges();

            return Ok(new {Migrations="Usuário cadastrado com sucesso."});
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario credenciais)
        {
            //Buscar um usuário por email
            //verificar se a senha está correta
            //gerar um token JWT e retornar esse token para o usuário
            try
            {
                Usuario usuario = database.Usuarios.First(user => user.Email.Equals(credenciais.Email));
                 if(usuario != null)
                {
                    //Achou um usuário com cadastro válido
                    if(usuario.Senha.Equals(credenciais.Senha))
                    {
                        //usuario acertou a senha => Logou !!!
                        
                        //Chave de segurança
                        string chaveDeSeguranca = "school_of_net_manda_muito_bem";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

                        var claims = new List<Claim>();
                        claims.Add(new Claim("id", usuario.Id.ToString()));
                        claims.Add(new Claim("email", usuario.Email));
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                        var JWT = new JwtSecurityToken(
                            issuer: "apirest", //quem está forncendo o JWT para o usuário
                            expires: DateTime.Now.AddHours(1),
                            audience: "usuario_comum",
                            signingCredentials: credenciaisDeAcesso,
                            claims: claims
                        );

                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));
                    }
                }

                //Não existe nenhum usuário com este e-mail
                Response.StatusCode = 401;
                return new ObjectResult("");

            }
            catch
            {
                //Não existe nenhum usuário com este e-mail
                Response.StatusCode = 401;
                return new ObjectResult("");
            }
           
        }
    }
}