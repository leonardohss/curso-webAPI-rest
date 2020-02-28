using Microsoft.AspNetCore.Mvc;
using MimicAPI.Database;
using MimicAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly MimicContext _banco;

        public PalavrasController(MimicContext banco)
        {
            _banco = banco;
        }

        //retorna todas as palavras cadastradas -- /api/palavras
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas()
        {
            return Ok(_banco.Palavras);         
        }

        //retorna a palavra com o id informado -- /api/palavras/id
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            return Ok(_banco.Palavras.Find(id));
        }

        // -- /api/palavras(POST: id, nome, ativo, ...)
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();

            return Ok();
        }

        // -- /api/palavras/1(PUT: id, nome, ativo, ...)
        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {
            palavra.Id = id;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return Ok();
        }

        // -- /api/palavras/1(DELETE)
        [Route("{id}")]
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _banco.Palavras.Find(id);
            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return Ok();
        }
    }
}
