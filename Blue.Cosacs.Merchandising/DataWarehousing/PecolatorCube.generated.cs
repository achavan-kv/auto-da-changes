
/**********************************************************************************************************************
*
*								Thank you for using Percolator Analysis Services!!!!!!!
*
*	WARNING: Changing this auto generated file may lead to wonky actions, and could possibly injure many puppies. 
*			 Any manual changes will be lost upon new generation. 
*
*	CONNECTION STRING: Provider=MSOLAP;Data Source=.;Initial Catalog=Merchandising;
*	LAST GENERATION: 06/16/2015 09:52:24
*
*	SHOWING:
*		Visible dimensions only
*		Visible hierarchies only
*		Visible attributes only
*		Visible measures only
*
**********************************************************************************************************************/

using System.Configuration;

namespace Blue.Cosacs.Merchandising.DataWarehousing
{
	using Percolator.AnalysisServices;
	using Percolator.AnalysisServices.Attributes;
	using Percolator.AnalysisServices.Linq;

	public partial class CosacsDW : CubeBase
	{
		static string _CON { get { return  @"Provider=MSOLAP;Data Source=.;Initial Catalog=Merchandising;"; } }

		public CosacsDW()
			: base(_CON) 
		{
			this.Inventory = new Cube<Inventory>(this._provider);
			this.Sales = new Cube<Sales>(this._provider);
		}

		public CosacsDW(string connectionString)
			: base(connectionString) 
		{
			this.Inventory = new Cube<Inventory>(this._provider);
			this.Sales = new Cube<Sales>(this._provider);
		}
		
		#region Cubes
		/// <summary>
        /// The Inventory cube from the Merchandising database.
        /// </summary>
		public Cube<Inventory> Inventory { get; private set; }
		/// <summary>
        /// The Sales cube from the Merchandising database.
        /// </summary>
		public Cube<Sales> Sales { get; private set; }
		#endregion
	}

	[Cube(Tag = "Inventory")]
	public partial class Inventory : ICubeObject
	{
		static Inventory _instance;
		public static Inventory Instance { get { return Inventory._instance ?? (Inventory._instance = new Inventory()); } }

		public Date_Dimension Date  { get { return Date_Dimension.Instance; } }
		public Hierarchy_Dimension Hierarchy  { get { return Hierarchy_Dimension.Instance; } }
		public Location_Dimension Location  { get { return Location_Dimension.Instance; } }
		public ProductStock_Dimension ProductStock  { get { return ProductStock_Dimension.Instance; } }
		
		public Measure StockValue { get { return new Measure("Stock Value"); } }
		
		public Measure ProductId { get { return new Measure("Product Id"); } }
		
		public Measure LocationId { get { return new Measure("Location Id"); } }
		
		public Measure Id { get { return new Measure("Id"); } }
		
		public Measure ProductStockCount { get { return new Measure("Product Stock Count"); } }
		
		public Measure StockOnHandQuantity { get { return new Measure("Stock On Hand Quantity"); } }
		
		public Measure StockOnHandValue { get { return new Measure("Stock On Hand Value"); } }
		
		public Measure StockOnHandSalesValue { get { return new Measure("Stock On Hand Sales Value"); } }
	}

	[Cube(Tag = "Sales")]
	public partial class Sales : ICubeObject
	{
		static Sales _instance;
		public static Sales Instance { get { return Sales._instance ?? (Sales._instance = new Sales()); } }

		public AccountSales_Dimension AccountSales  { get { return AccountSales_Dimension.Instance; } }
		public AffinitySales_Dimension AffinitySales  { get { return AffinitySales_Dimension.Instance; } }
		public Date_Dimension Date  { get { return Date_Dimension.Instance; } }
		public Discounts_Dimension Discounts  { get { return Discounts_Dimension.Instance; } }
		public DivisionSales_Dimension DivisionSales  { get { return DivisionSales_Dimension.Instance; } }
		public InterestCharges_Dimension InterestCharges  { get { return InterestCharges_Dimension.Instance; } }
		public MerchandiseSales_Dimension MerchandiseSales  { get { return MerchandiseSales_Dimension.Instance; } }
		public OtherCharges_Dimension OtherCharges  { get { return OtherCharges_Dimension.Instance; } }
		public ServiceCharges_Dimension ServiceCharges  { get { return ServiceCharges_Dimension.Instance; } }
		public WarrantySales_Dimension WarrantySales  { get { return WarrantySales_Dimension.Instance; } }
		
		public Measure MerchandisePrice { get { return new Measure("Merchandise Price"); } }
		
		public Measure MerchandiseGrossProfit { get { return new Measure("Merchandise Gross Profit"); } }
		
		public Measure AffinityPrice { get { return new Measure("Affinity Price"); } }
		
		public Measure AffinityGrossProfit { get { return new Measure("Affinity Gross Profit"); } }
		
		public Measure ServiceChargePrice { get { return new Measure("Service Charge Price"); } }
		
		public Measure ServiceChargeGrossProfit { get { return new Measure("Service Charge Gross Profit"); } }
		
		public Measure RebatesPrice { get { return new Measure("Rebates Price"); } }
		
		public Measure RebatesGrossProfit { get { return new Measure("Rebates Gross Profit"); } }
		
		public Measure OtherChargesPrice { get { return new Measure("Other Charges Price"); } }
		
		public Measure OtherChargesGrossProfit { get { return new Measure("Other Charges Gross Profit"); } }
		
		public Measure InterestChargesPrice { get { return new Measure("Interest Charges Price"); } }
		
		public Measure InterestChargesGrossProfit { get { return new Measure("Interest Charges Gross Profit"); } }
		
		public Measure LoanDisbursementPrice { get { return new Measure("Loan Disbursement Price"); } }
		
		public Measure LoanDisbursementGrossProfit { get { return new Measure("Loan Disbursement Gross Profit"); } }
		
		public Measure DiscountPrice { get { return new Measure("Discount Price"); } }
		
		public Measure DiscountGrossProfit { get { return new Measure("Discount Gross Profit"); } }
		
		public Measure WarrantyPrice { get { return new Measure("Warranty Price"); } }
		
		public Measure WarrantyGrossProfit { get { return new Measure("Warranty Gross Profit"); } }
		
		public Measure PriceDifferential { get { return new Measure("Price Differential"); } }
		
		public Measure PriceDifferentialGrossProfit { get { return new Measure("Price Differential Gross Profit"); } }
	}


	#region Dimensions
	[Dimension]
	public partial class Date_Dimension : ICubeObject
	{
		static Date_Dimension _instance;
		public static Date_Dimension Instance { get { return Date_Dimension._instance ?? (Date_Dimension._instance = new Date_Dimension()); } }
		
		Date_Dimension() { }
		
		public Attribute CalendarYear { get { return "[Date].[Calendar Year]"; } }
		
		public Attribute DateKey { get { return "[Date].[Date Key]"; } }
		
		public Attribute FiscalPeriod { get { return "[Date].[Fiscal Period]"; } }
		
		public Attribute FiscalWeek { get { return "[Date].[Fiscal Week]"; } }
		
		public Attribute FiscalYear { get { return "[Date].[Fiscal Year]"; } }
		
		[Hierarchy]
		public FiscalDate_Hierarchy FiscalDate { get { return new FiscalDate_Hierarchy(); } }
		
		public partial class FiscalDate_Hierarchy : Hierarchy, ICubeObject
		{
			public FiscalDate_Hierarchy()
				: base("[Date].[Fiscal Date]") { }

			
			public Level FiscalYear { get { return new Level("[Date].[Fiscal Date].[Fiscal Year]", 1); } }
			
			public Level FiscalPeriod { get { return new Level("[Date].[Fiscal Date].[Fiscal Period]", 2); } }
			
			public Level FiscalWeek { get { return new Level("[Date].[Fiscal Date].[Fiscal Week]", 3); } }
			
			public Level DateKey { get { return new Level("[Date].[Fiscal Date].[Date Key]", 4); } }
		}
	}

	[Dimension]
	public partial class Hierarchy_Dimension : ICubeObject
	{
		static Hierarchy_Dimension _instance;
		public static Hierarchy_Dimension Instance { get { return Hierarchy_Dimension._instance ?? (Hierarchy_Dimension._instance = new Hierarchy_Dimension()); } }
		
		Hierarchy_Dimension() { }
		
		public Attribute Class { get { return "[Hierarchy].[Class]"; } }
		
		public Attribute Department { get { return "[Hierarchy].[Department]"; } }
		
		public Attribute Division { get { return "[Hierarchy].[Division]"; } }
		
		public Attribute Id { get { return "[Hierarchy].[Id]"; } }
		
		[Hierarchy]
		public Hierarchy_Hierarchy Hierarchy { get { return new Hierarchy_Hierarchy(); } }
		
		public partial class Hierarchy_Hierarchy : Hierarchy, ICubeObject
		{
			public Hierarchy_Hierarchy()
				: base("[Hierarchy].[Hierarchy]") { }

			
			public Level DivisionId { get { return new Level("[Hierarchy].[Hierarchy].[Division Id]", 1); } }
			
			public Level DepartmentId { get { return new Level("[Hierarchy].[Hierarchy].[Department Id]", 2); } }
			
			public Level ClassId { get { return new Level("[Hierarchy].[Hierarchy].[Class Id]", 3); } }
		}
	}

	[Dimension]
	public partial class Location_Dimension : ICubeObject
	{
		static Location_Dimension _instance;
		public static Location_Dimension Instance { get { return Location_Dimension._instance ?? (Location_Dimension._instance = new Location_Dimension()); } }
		
		Location_Dimension() { }
		
		public Attribute Fascia { get { return "[Location].[Fascia]"; } }
		
		public Attribute LocationId { get { return "[Location].[Location Id]"; } }
		
		public Attribute LocationName { get { return "[Location].[Location Name]"; } }
		
		public Attribute SalesId { get { return "[Location].[Sales Id]"; } }
		
		public Attribute StoreType { get { return "[Location].[Store Type]"; } }
		
		public Attribute VirtualWarehouse { get { return "[Location].[Virtual Warehouse]"; } }
		
		public Attribute Warehouse { get { return "[Location].[Warehouse]"; } }
		
		[Hierarchy]
		public Hierarchy_Hierarchy Hierarchy { get { return new Hierarchy_Hierarchy(); } }
		
		public partial class Hierarchy_Hierarchy : Hierarchy, ICubeObject
		{
			public Hierarchy_Hierarchy()
				: base("[Location].[Hierarchy]") { }

			
			public Level LocationId { get { return new Level("[Location].[Hierarchy].[Location Id]", 1); } }
			
			public Level LocationName { get { return new Level("[Location].[Hierarchy].[Location Name]", 2); } }
			
			public Level SalesId { get { return new Level("[Location].[Hierarchy].[Sales Id]", 3); } }
			
			public Level Fascia { get { return new Level("[Location].[Hierarchy].[Fascia]", 4); } }
			
			public Level Warehouse { get { return new Level("[Location].[Hierarchy].[Warehouse]", 5); } }
		}
	}

	[Dimension]
	public partial class ProductStock_Dimension : ICubeObject
	{
		static ProductStock_Dimension _instance;
		public static ProductStock_Dimension Instance { get { return ProductStock_Dimension._instance ?? (ProductStock_Dimension._instance = new ProductStock_Dimension()); } }
		
		ProductStock_Dimension() { }
		
		public Attribute Department { get { return "[Product Stock].[Department]"; } }
		
		public Attribute Division { get { return "[Product Stock].[Division]"; } }
		
		public Attribute Id { get { return "[Product Stock].[Id]"; } }
		
		[Hierarchy]
		public ProductHierarchy_Hierarchy ProductHierarchy { get { return new ProductHierarchy_Hierarchy(); } }
		
		public partial class ProductHierarchy_Hierarchy : Hierarchy, ICubeObject
		{
			public ProductHierarchy_Hierarchy()
				: base("[Product Stock].[Product Hierarchy]") { }

			
			public Level Division { get { return new Level("[Product Stock].[Product Hierarchy].[Division]", 1); } }
			
			public Level Department { get { return new Level("[Product Stock].[Product Hierarchy].[Department]", 2); } }
		}
	}

	[Dimension]
	public partial class AccountSales_Dimension : ICubeObject
	{
		static AccountSales_Dimension _instance;
		public static AccountSales_Dimension Instance { get { return AccountSales_Dimension._instance ?? (AccountSales_Dimension._instance = new AccountSales_Dimension()); } }
		
		AccountSales_Dimension() { }
		
		public Attribute Division { get { return "[Account Sales].[Division]"; } }
		
		public Attribute Id { get { return "[Account Sales].[Id]"; } }
		
		public Attribute SaleType { get { return "[Account Sales].[Sale Type]"; } }
		
		[Hierarchy]
		public AccountSales_Hierarchy AccountSales { get { return new AccountSales_Hierarchy(); } }
		
		public partial class AccountSales_Hierarchy : Hierarchy, ICubeObject
		{
			public AccountSales_Hierarchy()
				: base("[Account Sales].[Account Sales]") { }

			
			public Level SaleType { get { return new Level("[Account Sales].[Account Sales].[Sale Type]", 1); } }
			
			public Level Division { get { return new Level("[Account Sales].[Account Sales].[Division]", 2); } }
		}
	}

	[Dimension]
	public partial class AffinitySales_Dimension : ICubeObject
	{
		static AffinitySales_Dimension _instance;
		public static AffinitySales_Dimension Instance { get { return AffinitySales_Dimension._instance ?? (AffinitySales_Dimension._instance = new AffinitySales_Dimension()); } }
		
		AffinitySales_Dimension() { }
		
		public Attribute Id { get { return "[Affinity Sales].[Id]"; } }
		
		public Attribute Type { get { return "[Affinity Sales].[Type]"; } }
		
		[Hierarchy]
		public AffinitySales_Hierarchy AffinitySales { get { return new AffinitySales_Hierarchy(); } }
		
		public partial class AffinitySales_Hierarchy : Hierarchy, ICubeObject
		{
			public AffinitySales_Hierarchy()
				: base("[Affinity Sales].[Affinity Sales]") { }

			
			public Level Type { get { return new Level("[Affinity Sales].[Affinity Sales].[Type]", 1); } }
		}
	}

	[Dimension]
	public partial class Discounts_Dimension : ICubeObject
	{
		static Discounts_Dimension _instance;
		public static Discounts_Dimension Instance { get { return Discounts_Dimension._instance ?? (Discounts_Dimension._instance = new Discounts_Dimension()); } }
		
		Discounts_Dimension() { }
		
		public Attribute Division { get { return "[Discounts].[Division]"; } }
		
		public Attribute Id { get { return "[Discounts].[Id]"; } }
		
		public Attribute SaleType { get { return "[Discounts].[Sale Type]"; } }
		
		[Hierarchy]
		public Discounts_Hierarchy Discounts { get { return new Discounts_Hierarchy(); } }
		
		public partial class Discounts_Hierarchy : Hierarchy, ICubeObject
		{
			public Discounts_Hierarchy()
				: base("[Discounts].[Discounts]") { }

			
			public Level SaleType { get { return new Level("[Discounts].[Discounts].[Sale Type]", 1); } }
			
			public Level Division { get { return new Level("[Discounts].[Discounts].[Division]", 2); } }
		}
	}

	[Dimension]
	public partial class DivisionSales_Dimension : ICubeObject
	{
		static DivisionSales_Dimension _instance;
		public static DivisionSales_Dimension Instance { get { return DivisionSales_Dimension._instance ?? (DivisionSales_Dimension._instance = new DivisionSales_Dimension()); } }
		
		DivisionSales_Dimension() { }
		
		public Attribute Division { get { return "[Division Sales].[Division]"; } }
		
		public Attribute Id { get { return "[Division Sales].[Id]"; } }
		
		public Attribute SaleType { get { return "[Division Sales].[Sale Type]"; } }
		
		[Hierarchy]
		public DivisionSales_Hierarchy DivisionSales { get { return new DivisionSales_Hierarchy(); } }
		
		public partial class DivisionSales_Hierarchy : Hierarchy, ICubeObject
		{
			public DivisionSales_Hierarchy()
				: base("[Division Sales].[Division Sales]") { }

			
			public Level Division { get { return new Level("[Division Sales].[Division Sales].[Division]", 1); } }
			
			public Level SaleType { get { return new Level("[Division Sales].[Division Sales].[Sale Type]", 2); } }
		}
	}

	[Dimension]
	public partial class InterestCharges_Dimension : ICubeObject
	{
		static InterestCharges_Dimension _instance;
		public static InterestCharges_Dimension Instance { get { return InterestCharges_Dimension._instance ?? (InterestCharges_Dimension._instance = new InterestCharges_Dimension()); } }
		
		InterestCharges_Dimension() { }
		
		public Attribute Id { get { return "[Interest Charges].[Id]"; } }
		
		public Attribute Type { get { return "[Interest Charges].[Type]"; } }
		
		[Hierarchy]
		public InterestCharges_Hierarchy InterestCharges { get { return new InterestCharges_Hierarchy(); } }
		
		public partial class InterestCharges_Hierarchy : Hierarchy, ICubeObject
		{
			public InterestCharges_Hierarchy()
				: base("[Interest Charges].[Interest Charges]") { }

			
			public Level Type { get { return new Level("[Interest Charges].[Interest Charges].[Type]", 1); } }
		}
	}

	[Dimension]
	public partial class MerchandiseSales_Dimension : ICubeObject
	{
		static MerchandiseSales_Dimension _instance;
		public static MerchandiseSales_Dimension Instance { get { return MerchandiseSales_Dimension._instance ?? (MerchandiseSales_Dimension._instance = new MerchandiseSales_Dimension()); } }
		
		MerchandiseSales_Dimension() { }
		
		public Attribute Department { get { return "[Merchandise Sales].[Department]"; } }
		
		public Attribute Division { get { return "[Merchandise Sales].[Division]"; } }
		
		public Attribute Id { get { return "[Merchandise Sales].[Id]"; } }
		
		[Hierarchy]
		public ProductHierarchy_Hierarchy ProductHierarchy { get { return new ProductHierarchy_Hierarchy(); } }
		
		public partial class ProductHierarchy_Hierarchy : Hierarchy, ICubeObject
		{
			public ProductHierarchy_Hierarchy()
				: base("[Merchandise Sales].[Product Hierarchy]") { }

			
			public Level Division { get { return new Level("[Merchandise Sales].[Product Hierarchy].[Division]", 1); } }
			
			public Level Department { get { return new Level("[Merchandise Sales].[Product Hierarchy].[Department]", 2); } }
		}
	}

	[Dimension]
	public partial class OtherCharges_Dimension : ICubeObject
	{
		static OtherCharges_Dimension _instance;
		public static OtherCharges_Dimension Instance { get { return OtherCharges_Dimension._instance ?? (OtherCharges_Dimension._instance = new OtherCharges_Dimension()); } }
		
		OtherCharges_Dimension() { }
		
		public Attribute Id { get { return "[Other Charges].[Id]"; } }
		
		public Attribute Type { get { return "[Other Charges].[Type]"; } }
		
		[Hierarchy]
		public OtherCharges_Hierarchy OtherCharges { get { return new OtherCharges_Hierarchy(); } }
		
		public partial class OtherCharges_Hierarchy : Hierarchy, ICubeObject
		{
			public OtherCharges_Hierarchy()
				: base("[Other Charges].[Other Charges]") { }

			
			public Level Type { get { return new Level("[Other Charges].[Other Charges].[Type]", 1); } }
		}
	}

	[Dimension]
	public partial class ServiceCharges_Dimension : ICubeObject
	{
		static ServiceCharges_Dimension _instance;
		public static ServiceCharges_Dimension Instance { get { return ServiceCharges_Dimension._instance ?? (ServiceCharges_Dimension._instance = new ServiceCharges_Dimension()); } }
		
		ServiceCharges_Dimension() { }
		
		public Attribute Id { get { return "[Service Charges].[Id]"; } }
		
		public Attribute Type { get { return "[Service Charges].[Type]"; } }
		
		[Hierarchy]
		public ServiceCharges_Hierarchy ServiceCharges { get { return new ServiceCharges_Hierarchy(); } }
		
		public partial class ServiceCharges_Hierarchy : Hierarchy, ICubeObject
		{
			public ServiceCharges_Hierarchy()
				: base("[Service Charges].[Service Charges]") { }

			
			public Level Type { get { return new Level("[Service Charges].[Service Charges].[Type]", 1); } }
		}
	}

	[Dimension]
	public partial class WarrantySales_Dimension : ICubeObject
	{
		static WarrantySales_Dimension _instance;
		public static WarrantySales_Dimension Instance { get { return WarrantySales_Dimension._instance ?? (WarrantySales_Dimension._instance = new WarrantySales_Dimension()); } }
		
		WarrantySales_Dimension() { }
		
		public Attribute Division { get { return "[Warranty Sales].[Division]"; } }
		
		public Attribute Id { get { return "[Warranty Sales].[Id]"; } }
		
		public Attribute SaleType { get { return "[Warranty Sales].[Sale Type]"; } }
		
		[Hierarchy]
		public WarrantySales_Hierarchy WarrantySales { get { return new WarrantySales_Hierarchy(); } }
		
		public partial class WarrantySales_Hierarchy : Hierarchy, ICubeObject
		{
			public WarrantySales_Hierarchy()
				: base("[Warranty Sales].[Warranty Sales]") { }

			
			public Level SaleType { get { return new Level("[Warranty Sales].[Warranty Sales].[Sale Type]", 1); } }
			
			public Level Division { get { return new Level("[Warranty Sales].[Warranty Sales].[Division]", 2); } }
		}
	}

	#endregion
}
 