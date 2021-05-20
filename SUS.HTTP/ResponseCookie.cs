using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUS.HTTP
{
    public class ResponseCookie : Cookie
    {
        public ResponseCookie(string name, string value)
            : base(name, value)
        {
            Path = "/";
        }

        public int MaxAge { get; set; }

        public bool HttpOnly { get; set; }

        public string Path { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"{Name}={Value}; Path={Path};");

            if (MaxAge != 0)
            {
                sb.Append($" Max-Age={MaxAge};");
            }

            if (HttpOnly)
            {
                sb.Append(" HttpOnly;");
            }

            return sb.ToString();
        }
    }
}
