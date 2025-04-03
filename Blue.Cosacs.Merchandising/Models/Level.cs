using System;
namespace Blue.Cosacs.Merchandising.Models
{
    public sealed class Level : IComparable<Level>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        int IComparable<Level>.CompareTo(Level other)
        {
            if (this.Id > other.Id)
                return 1;
            else if (this.Id == other.Id)
                return 0;
            else
                return -1;
        }
    }
}