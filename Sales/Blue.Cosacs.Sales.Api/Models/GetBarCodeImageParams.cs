using BarcodeLib;

namespace Blue.Cosacs.Sales.Api.Models
{
    public class GetBarCodeImageParams
    {
        public string Value { get; set; }

        public string Type { get; set; }

        public int W { get; set; }

        public int H { get; set; }

        public bool? Label { get; set; }

        public AlignmentPositions Align { get; set; }

        public bool Rotate { get; set; }

        public GetBarCodeImageParams()
        {
            Align = AlignmentPositions.LEFT;
            Rotate = false;
            Label = null;
        }
    }
}
