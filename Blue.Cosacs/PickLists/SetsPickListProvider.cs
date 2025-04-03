using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.PickLists
{
    public class SetsPickListProvider : LinqPickListProvider
    {
        public SetsPickListProvider(string id) : base(
            id,
            id,
            () => Load(id))
        {    
        }

        private static IEnumerable<IPickListRow> Load(string id)
        {
            using (var ctx = Context.Create())
            {
                return (from s in ctx.ViewSets
                        where s.tname == id
                        orderby s.setdescript
                        select new PickListRow { k = s.setname, v = s.setdescript }).ToArray();
            }
        }

        public static IEnumerable<IPickListProvider> All()
        {
            using (var ctx = Context.Create())
            {
                var sets = (from s in ctx.ViewSets select s.tname).Distinct().ToList();
                return from s in sets select new SetsPickListProvider(s);
            }
        }
    }
}
