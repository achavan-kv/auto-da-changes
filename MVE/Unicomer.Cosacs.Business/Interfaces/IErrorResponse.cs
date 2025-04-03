using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Business.Interfaces
{
    public interface IErrorResponse
    {
        JResponse CreateErrorResponse(dynamic ErrorList);
        JResponse CreateExceptionResponse(dynamic ErrorList);
        dynamic CreateWebExceptionResponse(dynamic ErrorList, int statusCode);
    }
}
