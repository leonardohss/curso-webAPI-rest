using AutoMapper;
using MimicAPI.Models;
using MimicAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Helpers
{
    public class DtoMapperProfile : Profile
    {
        public DtoMapperProfile()
        {
            /*
             AutoMapper

            Palavra > PalavraDTO;
             */

            CreateMap<Palavra, PalavraDto>();
            CreateMap<PaginationList<Palavra>, PaginationList<PalavraDto>>();
        }
    }
}
