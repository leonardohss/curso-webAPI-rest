using Microsoft.AspNetCore.Mvc;
using MimicAPI.Database;
using MimicAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimicAPI.Helpers;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft;

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

        //retorna todas as palavras cadastradas -- /api/palavras?data=2019-01-01
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {
            var item = _banco.Palavras.AsQueryable();

            if (query.Data.HasValue)
            { 
                item = item.Where(a => a.Criado > query.Data.Value || a.Atualizado > query.Data.Value);
            }

            //paginação
            if (query.PagNumero.HasValue)
            {
                var totalRegistrosBd = item.Count();

                item = item.Skip((query.PagNumero.Value - 1) * query.QtdeRegistrosPag.Value).Take(query.QtdeRegistrosPag.Value);

                var paginacao = new Paginacao();

                paginacao.NumeroPagina = query.PagNumero.Value;
                paginacao.QtdeRegistrosPorPagina = query.QtdeRegistrosPag.Value;
                paginacao.TotalRegistros = totalRegistrosBd;
                paginacao.TotalPaginas = (int)Math.Ceiling((double)totalRegistrosBd / query.QtdeRegistrosPag.Value);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginacao));

                if(query.PagNumero > paginacao.TotalPaginas)
                {
                    return NotFound();
                }
            }

            return Ok(item);         
        }

        //retorna a palavra com o id informado -- /api/palavras/id
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var obj = _banco.Palavras.Find(id);

            if (obj == null)
            {
                //return StatusCode(404);
                return NotFound();
            }
            return Ok(obj);
        }

        // -- /api/palavras(POST: id, nome, ativo, ...)
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();

            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        // -- /api/palavras/1(PUT: id, nome, ativo, ...)
        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);

            if (obj == null)
            {
                //return StatusCode(404);
                return NotFound();
            }

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

            if (palavra == null)
            {
                //return StatusCode(404);
                return NotFound();
            }

            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
            
            return NoContent();
        }
    }
}
