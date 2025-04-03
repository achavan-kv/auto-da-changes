using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Business.Interfaces
{
    public interface ICustomer
    {
        JResponse ValidateUser(ValidatetUser objValidateUser);
        JResponse CreateUser(User objUser);
        JResponse UpdateUser(UpdateUser objUpdateUser, string CustId);
        JResponse getAuthQAndA(string CustId);
    }
}
