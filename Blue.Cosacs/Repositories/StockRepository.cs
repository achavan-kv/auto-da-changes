using System;
using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.Shared;
using STL.Common.Constants.Categories;
using STL.Common;
using System.Data.SqlClient;
using System.Data;
using Blue.Cosacs.Model;

namespace Blue.Cosacs.Repositories
{
    public class StockRepository
    {
        private CommonObject commonObject = new CommonObject();

        public CountryParameterCollection Country
        {
            get { return commonObject.Country; }
        }

        public IQueryable<StockInfo> LoadAllWarranty(Context context = null)
        {
            context = context ?? Context.Create();

            var warCode = context.Code
                            .Where(c => c.category == "WAR")
                            .Select(c => c.code);

            return context.StockInfo
                            .Where(s => s.category != null && warCode.Contains(s.category.ToString()));

        }

        public IQueryable<StockInfo> GetStockInfo(string itemCode, bool repoItem, int? itemId = null, bool includeWarranties = false)   // RI
        {
            var context = Context.Create();

            var query = context.StockInfo
                            .Where(s => s.IUPC == itemCode ||
                                        s.SKU == itemCode ||
                                        s.itemno == itemCode ||
                                        s.VendorEAN == itemCode)                                    //IP - 28/09/12 - #10393 - LW74980
                            .Where(s => s.RepossessedItem == repoItem)    
                            .WhereIf(itemId.HasValue, 
                                        s => s.Id == itemId.Value);

            if (includeWarranties == false)
            {
                var warrantyCats = context.Code
                                    .Where(c => c.category == CAT.Warranty)
                                    .Select(c => c.code);

                query = query.Where(s => !warrantyCats.Contains(Convert.ToString(s.category)));
            }

            //TODO need to exclude Installation Items

            return query;
        }

        public List<short> GetStockLocation(int itemID)
        {
            var context = Context.Create();

            var query = from s in context.StockInfo
                        join q in context.StockQuantity on s.Id equals q.ID
                        where s.Id == itemID
                        select q.stocklocn;

            return query.Distinct().ToList();
        }

        public StockInfo GetKitDiscountItem(string itemCategory)
        {
            var context = Context.Create();

            var items = from c in context.Code
                        join s in context.StockInfo on c.reference equals s.IUPC
                        where c.category == CAT.KitItemCatDiscount &&
                              c.statusflag == "L" &&
                              c.code == itemCategory
                        select s;

            return items.FirstOrDefault();
        }

        public Dictionary<string, int> GetStockItemCache()
        {
            var IUPCs = StockItemKeys.AsEnumerable()
                            .Select(k =>
                            {
                                if (k.IsCountryParam)
                                    return Country[k.Key].NullSafe(x => x.ToString());
                                else
                                    return k.Key;
                            })
                            .Where(k => !String.IsNullOrEmpty(k))
                            .ToList();

            var context = Context.Create();

            var query = context.StockInfo
                        .Where(s => IUPCs.Contains(s.IUPC) &&
                                    s.RepossessedItem == false)
                        .OrderBy(s => s.IUPC)
                        .Select(s => new { s.Id, IUPC = s.IUPC.ToUpper() });

            return query.ToDictionary(k => k.IUPC, e => e.Id);
        }

        //IP - 08/09/11 - RI - #8112 
        public int GetReturnItemIDForItemCode(string itemNo, short stockLocn)
        {
             var context = Context.Create();

             var query = from s in context.StockInfo
                         join sp in context.StockPrice on s.Id equals sp.ID
                         join b in context.Branch on sp.branchno equals b.branchno
                         where s.IUPC == itemNo
                         && sp.branchno == stockLocn
                         && s.RepossessedItem == false
                         select s.Id;

            return query.AnsiFirstOrDefault(context);
        }

        public class OnlineProducts
        {
            public string product {get; set;}
            public bool online {get; set;}
            public bool dcOnly { get; set; }
        }
        public void UpdateOnlineProducts(SqlConnection conn, SqlTransaction trans, DataTable products)
        {   
            List<OnlineProducts> changes = new List<OnlineProducts>();
            
            using (var ctx = Context.Create(conn, trans))
            {
                foreach (DataRow dr in products.Rows)
                {
                    var onlineProduct = ctx.StockInfo.Where(s => s.Id == Convert.ToInt32(dr["ItemId"]));

                    foreach (var product in onlineProduct)
                    {
                        if ((product.OnlineAvailable==null && Convert.ToBoolean(dr["Online"])==true) || (product.OnlineAvailable!=null && product.OnlineAvailable!=Convert.ToBoolean(dr["Online"]))
                            || (product.OnlineDConly == null && Convert.ToBoolean(dr["DC Only"]) == true) || (product.OnlineDConly!=null && product.OnlineDConly!=Convert.ToBoolean(dr["DC Only"])) )
                        {
                           changes.Add( new OnlineProducts{
                               product = Convert.ToString(dr["Product Code"]),
                               online = Convert.ToBoolean(dr["Online"]),
                               dcOnly = Convert.ToBoolean(dr["DC Only"])
                            });

                            product.OnlineAvailable=Convert.ToBoolean(dr["Online"]);
                            product.OnlineDConly = Convert.ToBoolean(dr["DC Only"]);

                            if (product.OnlineAvailable==true)
                            {
                                product.OnlineDateAdded=DateTime.Now;
                            }
                            else
                            {
                                if (product.OnlineDateAdded != null)        // product has been online
                                {
                                    product.OnlineDateRemoved = DateTime.Now;
                                }
                            }
                        }
                    }
                }
                //Audit changes
                EventStore.Instance.Log(new {Changes =changes}, "OnlineProductSearch", EventCategory.Stock);
                            
                ctx.SubmitChanges();
                
            }
        }

        public void UpdateCodeMaintenanceNonStocks(List<NonStockModel> nonStocks)
        {
            var context = Context.Create();

            foreach (var nonStock in nonStocks)
            {
                if (nonStock.Type == NonStockTypes.Installation)
                {
                    var codeMaintenance = (from c in context.Code
                                          where c.category == "INST"
                                          && c.code == nonStock.SKU select c).FirstOrDefault();

                    if (codeMaintenance == null)
                    {
                        Code newCode = new Code
                        {
                            origbr = 0,
                            category = "INST",
                            code = nonStock.SKU,
                            codedescript = nonStock.LongDescription,
                            statusflag = "L",
                            sortorder = 0,
                            reference = nonStock.Level2.SelectedKey,
                            additional = string.Empty,
                            Additional2 = string.Empty
                        };

                        context.Code.InsertOnSubmit(newCode);
                    }

                }
                else if (nonStock.Type == NonStockTypes.Assembly)
                {
                    var codeMaintenance = (from c in context.Code
                                           where c.category == "ASSY"
                                           && c.code == nonStock.SKU
                                           select c).FirstOrDefault();

                    if (codeMaintenance == null)
                    {
                        Code newCode = new Code
                        {
                            origbr = 0,
                            category = "ASSY",
                            code = nonStock.SKU,
                            codedescript = nonStock.LongDescription,
                            statusflag = "L",
                            sortorder = 0,
                            reference = string.Empty,
                            additional = string.Empty,
                            Additional2 = string.Empty
                        };

                        context.Code.InsertOnSubmit(newCode);
                    }

                }
                else if (nonStock.Type == NonStockTypes.AnnualServiceContract)
                {
                    var codeMaintenance = (from c in context.Code
                                           where c.category == "ANNSERVCONT"
                                           && c.code == nonStock.SKU
                                           select c).FirstOrDefault();

                    if (codeMaintenance == null)
                    {
                        Code newCode = new Code
                        {
                            origbr = 0,
                            category = "ANNSERVCONT",
                            code = nonStock.SKU,
                            codedescript = nonStock.LongDescription,
                            statusflag = "L",
                            sortorder = 0,
                            reference = nonStock.Length.HasValue ? nonStock.Length.ToString() : string.Empty,
                            additional = nonStock.IsFullRefund.Value == true ? "1" : "0",
                            Additional2 = string.Empty
                        };

                        context.Code.InsertOnSubmit(newCode);
                    }
                    else
                    {
                        codeMaintenance.reference = nonStock.Length.HasValue ? nonStock.Length.ToString() : string.Empty;
                        codeMaintenance.additional = nonStock.IsFullRefund.Value == true ? "1" : "0";
                    }
                }
                else if (nonStock.Type == NonStockTypes.ReadyAssist)
                {
                    var codeMaintenance = (from c in context.Code
                                           where c.category == "RDYAST"
                                           && c.code == nonStock.SKU
                                           select c).FirstOrDefault();

                    if (codeMaintenance == null)
                    {
                        Code newCode = new Code
                        {
                            origbr = 0,
                            category = "RDYAST",
                            code = nonStock.SKU,
                            codedescript = nonStock.LongDescription,
                            statusflag = "L",
                            sortorder = 0,
                            reference = nonStock.Length.HasValue ? nonStock.Length.ToString() : string.Empty,
                            additional = nonStock.CoverageValue.HasValue ? Math.Round(nonStock.CoverageValue.Value, 2).ToString() : string.Empty,
                            Additional2 = string.Empty
                        };

                        context.Code.InsertOnSubmit(newCode);
                       
                    }
                    else
                    {
                        codeMaintenance.reference = nonStock.Length.HasValue ? nonStock.Length.ToString() : string.Empty;
                        codeMaintenance.additional = nonStock.CoverageValue.HasValue ? Math.Round(nonStock.CoverageValue.Value,2).ToString() : string.Empty;
                    }
                }
                else if (nonStock.Type == NonStockTypes.GenericService)
                {
                    var codeMaintenance = (from c in context.Code
                                           where c.category == "GENSERVICE"
                                           && c.code == nonStock.SKU
                                           select c).FirstOrDefault();

                    if (codeMaintenance == null)
                    {
                        Code newCode = new Code
                        {
                            origbr = 0,
                            category = "GENSERVICE",
                            code = nonStock.SKU,
                            codedescript = nonStock.LongDescription,
                            statusflag = "L",
                            sortorder = 0,
                            reference = string.Empty,
                            additional = string.Empty,
                            Additional2 = string.Empty
                        };

                        context.Code.InsertOnSubmit(newCode);
                    }

                }
            }

            context.SubmitChanges();

        }
    }
}
