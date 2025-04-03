using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.PickLists
{
    public class CodePickListProvider : LinqPickListProvider
    {
        public CodePickListProvider(string id, string name) : base(
            id,
            name,
            () => Load(id))
        {
        }

        private static IEnumerable<IPickListRow> Load(string id)
        {
            using (var ctx = Context.Create())
            {
                var rows = (from c in ctx.Code
                            where c.category == id && c.statusflag == "L"
                            select new { k = c.code, v = c.codedescript, s = c.sortorder }).Distinct().ToArray();
                return (from r in rows orderby r.s select new PickListRow(r.k, r.v)).ToArray();
            }
        }

        public static IEnumerable<IPickListProvider> All()
        {
            using (var ctx = Context.Create())
            {
                var cats = (from cat in ctx.CodeCat
                            select new { id = cat.category, name = cat.catdescript }).ToList();

                return from cat in cats
                       select new CodePickListProvider(cat.id, cat.name);
            }
        }
    }
}
