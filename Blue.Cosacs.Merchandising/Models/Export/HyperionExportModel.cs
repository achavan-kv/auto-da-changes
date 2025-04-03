namespace Blue.Cosacs.Merchandising.Models
{
    using System.ComponentModel.DataAnnotations;
    using Blue.Cosacs.Merchandising.Helpers;
    using FileHelpers;

    [DelimitedRecord("|")]
    public class HyperionExportModel
    {
        [MaxLength(150), FieldTitle("ent_Entidad")]
        public string EntityCode { get; set; }

        //Fascia for the time being
        [MaxLength(150), FieldTitle("CAD_CADENA")]
        public string ChainCode { get; set; }

        //Fascia for the time being
        [MaxLength(150), FieldTitle("CDA_CDA")]
        public string SalesLocation { get; set; }

        [MaxLength(150), FieldTitle("clp_ClaseProducto")]
        public string ProductClassCode { get; set; }

        [MaxLength(150), FieldTitle("desc_clase")]
        public string ProductClassDescription { get; set; }

        [MaxLength(150), FieldTitle("mar_MarcaProducto")]
        public string ProductBrandCode { get; set; }

        [MaxLength(150), FieldTitle("desc_marca")]
        public string ProductBrandDescription { get; set; }

        [StringLength(4), FieldTitle("inv_CalAnio")]
        public string CalendarYear { get; set; }

        [StringLength(3), FieldTitle("inv_Periodo")]
        public string Period { get; set; }

        [StringLength(4), FieldTitle("inv_FinAnio")]
        public string FinancialYear { get; set; }

        [FieldTitle("inv_ComprasUnidades")]
        public int PurchaseUnits { get; set; }

        [FieldTitle("inv_ComprasCosto")]
        public decimal PurchaseCost { get; set; }

        [FieldTitle("inv_ComprasVentas")]
        public decimal PurchaseSalePrice { get; set; }

        [FieldTitle("inv_InventarioUnidades")]
        public int InventoryUnits { get; set; }

        [FieldTitle("inv_InventarioCosto")]
        public decimal InventoryCosts { get; set; }

        [FieldTitle("inv_InventarioVenta")]
        public decimal InventorySalePrice { get; set; }

        [FieldTitle("inv_VentaUnidades")]
        public int SaleUnits { get; set; }

        [FieldTitle("inv_VentaCosto")]
        public decimal SalesCostWithoutTax { get; set; }

        [FieldTitle("inv_VentaVenta")]
        public decimal SalesTotal { get; set; }

        [FieldTitle("inv_PorcMargenVenta")]
        public decimal InventoryProductMargin { get; set; }

        [FieldTitle("inv_FechaActualizacion")]
        public string LastUpdated { get; set; }
    }
}
