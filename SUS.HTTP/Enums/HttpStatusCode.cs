using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUS.HTTP.Enums
{
    public enum HttpStatusCode
    {
        Ok = 200,
        NotFound = 404,
        MovedPermanently = 301,
        TemporaryRedirect = 307,
        ServerError = 500
    }
}
