using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.File
{
    public interface IStorageProvider
    {
        Guid Save(FileDescription file);
        FileDescription Get(Guid file);
        void Delete(Guid guid);
    }
}
