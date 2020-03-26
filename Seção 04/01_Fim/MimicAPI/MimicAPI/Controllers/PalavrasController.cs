using Microsoft.AspNetCore.Mvc;
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
using MimicAPI.Repositories.Contracts;
using AutoMapper;
using MimicAPI.Models.DTO;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        private readonly IMapper _mapper;

        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        //retorna todas as palavras cadastradas -- /api/palavras?data=2019-01-01
        [HttpGet("", Name = "ObterTodas")]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {
            var item = _repository.ObterPalavras(query);

            if (item.Results.Count == 0)
                return NotFound();

            PaginationList<PalavraDto> lista = CriarLinksListPalavraDto(query, item);

            return Ok(lista);
        }

        private PaginationList<PalavraDto> CriarLinksListPalavraDto(PalavraUrlQuery query, PaginationList<Palavra> item)
        {
            var lista = _mapper.Map<PaginationList<Palavra>, PaginationList<PalavraDto>>(item);

            foreach (var palavra in lista.Results)
            {
                palavra.Links = new List<LinkDto>();
                palavra.Links.Add(new LinkDto("self", Url.Link("ObterPalavra", new { id = palavra.Id }), "GET"));
            }

            lista.Links.Add(new LinkDto("self", Url.Link("ObterTodas", query), "GET"));

            if (item.Paginacao != null)
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));

                if (query.PagNumero + 1 <= item.Paginacao.TotalPaginas)
                {
                    //proxima pagina
                    var queryString = new PalavraUrlQuery()
                    {
                        PagNumero = query.PagNumero + 1,
                        QtdeRegistrosPag = query.QtdeRegistrosPag,
                        Data = query.Data
                    };
                    lista.Links.Add(new LinkDto("next", Url.Link("ObterTodas", queryString), "GET"));

                    //ultima pagina
                    var ultimaPag = new PalavraUrlQuery()
                    {
                        PagNumero = item.Paginacao.TotalPaginas,
                        QtdeRegistrosPag = query.QtdeRegistrosPag,
                        Data = query.Data
                    };
                    lista.Links.Add(new LinkDto("last", Url.Link("ObterTodas", ultimaPag), "GET"));
                }
                if (query.PagNumero - 1 > 0)
                {
                    //pagina anterior
                    var queryString = new PalavraUrlQuery()
                    {
                        PagNumero = query.PagNumero - 1,
                        QtdeRegistrosPag = query.QtdeRegistrosPag,
                        Data = query.Data
                    };
                    lista.Links.Add(new LinkDto("prev", Url.Link("ObterTodas", queryString), "GET"));

                    //primeira pagina
                    var primeiraPag = new PalavraUrlQuery()
                    {
                        PagNumero = 0,
                        QtdeRegistrosPag = query.QtdeRegistrosPag,
                        Data = query.Data
                    };
                    lista.Links.Add(new LinkDto("first", Url.Link("ObterTodas", primeiraPag), "GET"));
                }  
            }

            return lista;
        }

        //retorna a palavra com o id informado -- /api/palavras/id
        [HttpGet("{id}", Name = "ObterPalavra")]
        public ActionResult Obter(int id)
        {
            var obj = _repository.Obter(id);

            if (obj == null)
            {
                //return StatusCode(404);
                return NotFound();
            }

            PalavraDto palavraDto = _mapper.Map<Palavra, PalavraDto>(obj);
            palavraDto.Links = new List<LinkDto>();
            palavraDto.Links.Add(
                new LinkDto("self", Url.Link("ObterPalavra", new { id = palavraDto.Id}), "GET")
                );
            palavraDto.Links.Add(
                new LinkDto("update", Url.Link("AtualizarPalavras", new { id = palavraDto.Id }), "PUT")
                );
            palavraDto.Links.Add(
                new LinkDto("delete", Url.Link("ExcluirPalavra", new { id = palavraDto.Id }), "DELETE")
                );

            return Ok(palavraDto);
        }

        // -- /api/palavras(POST: id, nome, ativo, ...)
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _repository.Cadastrar(palavra);

            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        // -- /api/palavras/1(PUT: id, nome, ativo, ...)
        [HttpPut("{id}", Name = "AtualizarPalavra")]
        public ActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {
            var obj = _repository.Obter(id);

            if (obj == null)
            {
                //return StatusCode(404);
                return NotFound();
            }

            palavra.Id = id;
            _repository.Atualizar(palavra);

            return Ok();
        }

        // -- /api/palavras/1(DELETE)
        [HttpDelete("{id}", Name = "ExcluirPalavra")]
        public ActionResult Deletar(int id)
        {
            var palavra = _repository.Obter(id);

            if (palavra == null)
            {
                //return StatusCode(404);
                return NotFound();
            }

            _repository.Deletar(id);
            
            return NoContent();
        }
    }
}
