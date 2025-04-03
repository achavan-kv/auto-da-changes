namespace Blue.Cosacs.Merchandising.Models.RP3Export
{
    using System.Diagnostics.CodeAnalysis;

    using Blue.Cosacs.Merchandising.Helpers;

    using FileHelpers;

    [DelimitedRecord(",")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required for export")]
    public class LocationExportModel
    {
        public LocationExportModel()
        {
            // we dont send these fields so set them to a default value
            this.FasciaCode = 0;
            this.SquareMeters = 0;
        }

        [FieldTitle]
        [FieldQuoted]
        public string Company;

        [FieldQuoted]
        [FieldTitle]
        public int FasciaCode;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string FasciaName;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string LocationCode;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string LocationName;

        [FieldQuoted]
        [FieldTitle]
        public string WarehouseFlag;

        [FieldQuoted]
        [FieldTitle]
        public int SquareMeters;

        [FieldTitle]
        [FieldQuoted]
        public string ActiveFlag;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string StoreType;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string AddressLine1;

        [FieldQuoted]
        [FieldConverter(typeof(UnicodeEscapeConverter))]
        [FieldTitle]
        public string City;
    }
}