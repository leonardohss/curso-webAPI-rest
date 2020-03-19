using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Models.DTO
{
    public class LinkDto
    {
        public string Rel { get; set; }
        public string Href { get; set; }
        public string Method { get; set; }

        public LinkDto(string rel, string href, string method)
        {
            Rel = rel;
            Href = href;
            Method = method;
        }
    }
}
