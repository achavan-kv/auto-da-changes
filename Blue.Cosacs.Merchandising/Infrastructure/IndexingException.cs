namespace Blue.Cosacs.Merchandising.Infrastructure
{
    using System;

    [Serializable]
    public class IndexingException : Exception
    {
        public IndexingException(string typeOfObjectBeingIndexed, Exception innerException)
            : base("Error indexing " + typeOfObjectBeingIndexed, innerException)
        {
        }
    }
}
