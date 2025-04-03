namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class LevelModel
    {
        public LevelModel()
        {
            Tags = new List<TagModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public List<TagModel> Tags { get; set; }
    }
}
