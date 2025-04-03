using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warehouse.Utils
{
    public sealed class AutoUser
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Comment { get; set; }

        private AutoUser(string name, int id,string comment)
        {
            this.Name = name;
            this.Id = id;
            this.Comment = comment;
        }

        public static readonly AutoUser AutoPickConfirm = new AutoUser("AutoPickConfirm", -200, "Auto confirmed by system.");

        public override string ToString()
        {
            return string.Format("{0} ({1})",Name,Id);
        }
    }
}
