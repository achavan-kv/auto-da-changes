using System.Collections.Generic;

namespace Blue.Cosacs.Credit.Repositories
{
    public interface ICustomizeFieldsRepository
    {
        List<Model.CustomizeFields> GetCustomizedFields();
        void SaveCustomizedField(Model.Field field);
    }
}
