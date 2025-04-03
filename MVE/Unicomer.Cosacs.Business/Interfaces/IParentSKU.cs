using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model;
namespace Unicomer.Cosacs.Business.Interfaces
{
    public interface IParentSKU
    {
        dynamic GetParentSKUMaster();

        dynamic GetParentSKUMasterEOD();
        dynamic UpdateParentSKUMaster(ParentSKUUpdate objJSON);
        //dynamic getParentSKUEOD(int spanInMinutes);
        dynamic getParentSKUEOD(int spanInMinutes, string ProductID);
        dynamic GetParentSKUUpdate();
    }


}
