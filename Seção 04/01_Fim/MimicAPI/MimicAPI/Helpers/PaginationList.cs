using MimicAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Helpers
{
    public class PaginationList<T>
    {
        public List<T> Results { get; set; } = new List<T>();
        public Paginacao Paginacao { get; set; }
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}
