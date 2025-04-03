using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.PickLists
{
    public class CachedPickListProvider : IPickListProvider
    {
        public CachedPickListProvider(IPickListProvider provider)
        {
            this.provider = provider;
        }

        private readonly IPickListProvider provider;
        private IList<IPickListRow> rows;

        public string Id
        {
            get { return provider.Id; }
        }

        public IEnumerable<IPickListRow> Load()
        {
            lock (this)
            {
                if (rows == null)
                {
                    rows = provider.Load().ToList(); // force load becaue it can be lazy
                }
            }
            return rows;
        }

        public string Name
        {
            get { return provider.Name; }
        }
    }
}
