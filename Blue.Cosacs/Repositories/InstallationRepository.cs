using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Linq;
using System.Transactions;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Extensions;
using STL.Common;
using Blue.Cosacs.ComLib;
using System.Reflection;
using Blue.Cosacs.Shared.CodeCategories;
using System.Data.Common;
using STL.Common.Constants.Categories;
using System.Data.SqlClient;
using Blue.Cosacs.Messages.Warehouse;
using STL.Common.Constants.ColumnNames;


namespace Blue.Cosacs.Repositories
{
    public class InstallationRepository
    {
        private CommonObject commonObject = new CommonObject();     //IP - 26/09/11 - RI - #8239 - CR8201

        //IP - 26/09/11 - RI - #8239 - CR8201
        public CountryParameterCollection CountryParam
        {
            get { return commonObject.Country; }
        }

        public IEnumerable<InstResult> GetPendingItems(string acctNo, DateTime? deliveryAuthorisedAfter, DateTime? deliveryAuthorisedBefore, 
                                                                    int? stockLocation, bool authorisedOnly, bool deliveredOnly, int? top)
        {
            var context = Context.Create();

            var status = "";
            if (authorisedOnly && !deliveredOnly)
                status="A";
            else if (deliveredOnly && !authorisedOnly)
                status = "D";
         
            var dataSet = new PendingInstallationsGetSP
            {
                acctno = acctNo,
                DAdatefrom = deliveryAuthorisedAfter,
                DAdateto = deliveryAuthorisedBefore,
                stocklocation = stockLocation ?? 0,
                status = status,
                top = top
            }.ExecuteDataSet();


            return dataSet.Tables[0].AsEnumerable()
                    .Select(r => new InstResult
                    {
                        InstNo = r.Field<int?>("InstNo"),
                        InstDate = r.Field<DateTime?>("InstDate"),
                        InstValue = r.Field<decimal>("InstValue"),
                        Status = r.Field<string>("InstallationStatus"),
                        FormattedStatus = r.Field<string>("InstallationStatus"), //Formatting should be done after the query execution, could be in the PL layer
                        AcctNo = r.Field<string>("AcctNo"),
                        AgreementNo = r.Field<int>("AgreementNo"),
                        AuthorisedDate = r.Field<DateTime?>("DateDelAuthorised"),
                        CustomerId = r.Field<string>("CustId"),
                        CustomerName = String.Format("{0} {1}", r.Field<string>("CustFirstName"), r.Field<string>("CustLastName")),
                        DeliveryAddress1 = r.Field<string>("DelAddress1"),
                        DeliveryAddress2 = r.Field<string>("DelAddress2"),
                        DeliveryAddress3 = r.Field<string>("DelAddress3"),
                        DeliveryDate = r.Field<DateTime?>("DelDate"),
                        HasWarranty = r.Field<bool?>("HasWarranty") ?? false,
                        ItemId = r.Field<int>("ItemId"),
                        ItemNo = r.Field<string>("ItemNo"),
                        CourtsCode = r.Field<string>("CourtsCode"),
                        PhoneHome = r.Field<string>("HomeTelNo"),           // jec 18/07/11
                        PhoneWork = r.Field<string>("WorkTelNo"),           // jec 18/07/11
                        ProductDescription1 = r.Field<string>("ProductDescription1"),
                        ProductDescription2 = r.Field<string>("ProductDescription2"),
                        Quantity = r.Field<double>("Quantity"),
                        StockLocation = r.Field<short>("StockLocation"),
                        SupplierName = r.Field<string>("SupplierCode"),
                        DeliveryStatus = r.Field<string>("DelStatus")
                    });
        }

        public Installation BookTechnician(Installation inst, int technicianId, DateTime slotDate, short[] slots, 
                                           int user, string rebookingReasonCode)
        {
            var currentTime = DateTime.Now;
            
            using (var scope = new TransactionScope())
            {
                var context = Context.Create();

                var slotsNotAvailable = context.SR_TechnicianDiary
                                        .Any(d => d.TechnicianId == technicianId && 
                                                  d.SlotDate.Date == slotDate.Date && 
                                                  slots.Contains(d.SlotNo));
                if (slotsNotAvailable)
                    throw new InvalidOperationException("The slots have already been booked");

                //Check installation exists, Installation may not exists at this point
                var queryExistingInst = context.Installation
                                            .WhereIf(inst.InstallationNo != 0,
                                                        i => i.InstallationNo == inst.InstallationNo)
                                            .WhereIf(inst.InstallationNo == 0,
                                                        i => i.AcctNo == inst.AcctNo &&
                                                             i.AgreementNo == inst.AgreementNo &&
                                                             i.ItemId == inst.ItemId &&
                                                             i.StockLocation == inst.StockLocation);

                if (!queryExistingInst.Any())
                {
                    context.Installation.InsertOnSubmit(new Installation
                    {
                        AcctNo           = inst.AcctNo,
                        AgreementNo      = inst.AgreementNo,
                        BranchNo         = Convert.ToInt16(inst.AcctNo.Substring(0,3)),
                        Comment          = inst.Comment,
                        InstallationDate = slotDate.Date,
                        ItemId           = inst.ItemId,
                        Status           = InstStatus.New.ToString(),
                        StockLocation    = inst.StockLocation,
                        CreatedBy        = user,
                        CreatedOn        = currentTime//,
                        //ItemNo           = string.Empty                        //IP - 19/07/11 - CR1254 - RI - #4300
                    });

                    context.SubmitChanges();
                }

                inst = queryExistingInst.AnsiFirstOrDefault(context);  //query will be re-executed at this point

                if (inst.Status.NotIn(InstStatus.New.ToString(), InstStatus.Booked.ToString()))
                    throw new InvalidOperationException(String.Format(@"Bookings on installations with status '{0}' is not allowed",
                                                                        inst.Status.SplitCamelCase()));

                //Check installation already booked
                var queryExistingBooking = context.InstallationBooking
                                            .Where(b => b.InstallationNo == inst.InstallationNo && !b.IsDeleted);

                if (queryExistingBooking.Any(b => b.InstallationDate >= DateTime.Today))
                    throw new InvalidOperationException("This installation has another booking on or after today");

                //Mark previous booking as deleted
                foreach (var booking in queryExistingBooking)
                {
                    booking.IsDeleted = true;
                    booking.DeletedOn = currentTime;
                    booking.DeletedBy = user;

                    var technicianDiaries = context.SR_TechnicianDiary
                                                .Where(d => d.TechnicianId == booking.TechnicianId &&
                                                            d.SlotDate.Date == booking.InstallationDate &&
                                                            d.InstallationNo == booking.InstallationNo);

                    context.SR_TechnicianDiary.DeleteAllOnSubmit(technicianDiaries);
                }


                foreach (var slot in slots.OrderBy(s => s))
                {
                    context.SR_TechnicianDiary.InsertOnSubmit(new SR_TechnicianDiary
                    {
                        TechnicianId = technicianId,
                        SlotNo = slot,
                        SlotDate = slotDate.Date,
                        BookingType = "I",
                        InstallationNo = inst.InstallationNo
                    });
                }

                context.InstallationBooking.InsertOnSubmit(new InstallationBooking
                {
                    InstallationNo      = inst.InstallationNo,
                    InstallationDate    = slotDate.Date,
                    StartSlot           = slots.Min(),
                    NoOfSlots           = (short)slots.Length,
                    TechnicianId        = technicianId,
                    RebookingReasonCode = rebookingReasonCode,
                    BookedOn            = currentTime,
                    BookedBy            = user
                });

                //Update Installation
                inst.Status           = InstStatus.Booked.ToString();
                inst.InstallationDate = slotDate.Date;
                inst.LastUpdatedOn    = currentTime;
                inst.LastUpdatedBy    = user;

                context.SubmitChanges();
                scope.Complete();
            }

            return inst;
        }

        public Installation ReleaseTechnician(int technicianId, DateTime slotDate, short slotNoAny, int user)
        {
            var currentTime = DateTime.Now;

            using (var scope = new TransactionScope())
            {
                var context = Context.Create();

                var diaryEntry = context.SR_TechnicianDiary
                                    .FirstOrDefault(d => d.TechnicianId == technicianId &&
                                                         d.SlotDate.Date == slotDate.Date &&
                                                         d.SlotNo == slotNoAny &&
                                                         d.BookingType == "I");

                if (diaryEntry == null)
                    throw new InvalidOperationException("Cannot find the booking for this slot");

                var inst = context.Installation
                                    .FirstOrDefault(i => i.InstallationNo == diaryEntry.InstallationNo.Value);

                if (inst.Status != InstStatus.Booked.ToString())
                    throw new InvalidOperationException(String.Format(@"Cannot release booking on installations with status - '{0}'",
                                                                 inst.Status.SplitCamelCase()));

                var diaryEntries = context.SR_TechnicianDiary
                                    .Where(d => d.TechnicianId == technicianId &&
                                                 d.SlotDate.Date == slotDate.Date &&
                                                 d.InstallationNo == inst.InstallationNo);

                context.SR_TechnicianDiary.DeleteAllOnSubmit(diaryEntries);

                var bookings = context.InstallationBooking
                                .Where(b => b.InstallationNo == inst.InstallationNo && !b.IsDeleted);

                foreach (var booking in bookings)
                {
                    booking.IsDeleted = true;
                    booking.DeletedBy = user;
                    booking.DeletedOn = currentTime;
                }

                inst.InstallationDate = null;
                inst.Status           = InstStatus.New.ToString();
                inst.LastUpdatedBy    = user;
                inst.LastUpdatedOn    = currentTime;

                context.SubmitChanges();
                scope.Complete();

                return inst;
            }
        }

        public Installation ReleaseTechnician(int instNo, int user)
        {
            var currentTime = DateTime.Now;

            using (var scope = new TransactionScope())
            {
                var context = Context.Create();

                var inst = context.Installation
                            .FirstOrDefault(i => i.InstallationNo == instNo);

                if (inst.Status != InstStatus.Booked.ToString())
                    throw new InvalidOperationException(String.Format(@"Cannot release booking on installations with status - '{0}'",
                                                                     inst.Status.SplitCamelCase()));

                var diaryEntries = context.SR_TechnicianDiary
                                    .Where(d => d.InstallationNo == instNo);

                context.SR_TechnicianDiary.DeleteAllOnSubmit(diaryEntries);

                var bookings = context.InstallationBooking
                                .Where(b => b.InstallationNo == instNo && !b.IsDeleted);

                foreach (var booking in bookings)
                {
                    booking.IsDeleted = true;
                    booking.DeletedBy = user;
                    booking.DeletedOn = currentTime;
                }

                inst.InstallationDate = null;
                inst.Status = InstStatus.New.ToString();
                inst.LastUpdatedBy = user;
                inst.LastUpdatedOn = currentTime;

                context.SubmitChanges();
                scope.Complete();    
                
                return inst;            
            }
        }       

        public List<InstBookingResult> GetBookings(int instNo, bool withHistory = false)
        {
            var context = Context.Create();

            var queryBookings = context.InstallationBooking
                                    .Where(b => b.InstallationNo == instNo)
                                    .WhereIfNot(withHistory, b => b.IsDeleted == false);

            var bookings = (from b in queryBookings
                           join t in context.SR_Technician on b.TechnicianId equals t.TechnicianId
                           join c in context.Code.Where(c => c.category == INST.REBOOKREASON.Category) 
                               on b.RebookingReasonCode equals c.code into JoinedRebookingReason
                           from r in JoinedRebookingReason.DefaultIfEmpty() //left join
                           orderby b.IsDeleted, b.BookedOn descending                        
                           select new InstBookingResult
                           {
                               InstNo              = b.InstallationNo,
                               InstDate            = b.InstallationDate,  
                               TechnicianId        = t.TechnicianId,
                               TechnicianName      = String.Format("{0} : {1} {2}", t.TechnicianId, t.FirstName.Trim(), t.LastName.Trim()),                      
                               StartSlot           = b.StartSlot,
                               NoOfSlots           = b.NoOfSlots,                            
                               BookedBy            = b.BookedBy,
                               BookedOn            = b.BookedOn,
                               ReleasedBy          = b.DeletedBy,
                               ReleasedOn          = b.DeletedOn,
                               RebookingReasonText = r.codedescript,
                               RebookingReasonCode = b.RebookingReasonCode,
                               IsActive            = !b.IsDeleted
                           })
                             .ToList();                 //IP - 25/08/11 - #4559 - Replaces the below
                           //.AnsiToList(context);

            bookings.ForEach(b =>
            {
                b.FormattedSlots = Enumerable.Range(b.StartSlot, b.NoOfSlots)
                                    .Stringify(separator: ",");
            });

            return bookings;
        }

        public IQueryable<InstTechPrintResult> GetBookingForPrint(int? instNoFrom, int? instNoTo,
                                                                    DateTime? instDateFrom, DateTime? instDateTo,
                                                                    DateTime? allocationDateFrom, DateTime? allocationDateTo,
                                                                    int? technicianId)
        {
            var context = Context.Create();

            var queryBooking = context.InstallationBooking
                                .Where(b => !b.IsDeleted)
                                .WhereIf(instNoFrom.HasValue,
                                            b => b.InstallationNo >= instNoFrom)
                                .WhereIf(instNoTo.HasValue,
                                            b => b.InstallationNo <= instNoTo)
                                .WhereIf(instDateFrom.HasValue,
                                            b => b.InstallationDate.Date >= instDateFrom.Value.Date)
                                .WhereIf(instDateTo.HasValue,
                                            b => b.InstallationDate.Date <= instDateTo.Value.Date)
                                .WhereIf(technicianId.HasValue,
                                            b => b.TechnicianId == technicianId.Value)
                                .WhereIf(allocationDateFrom.HasValue,
                                            b => b.BookedOn.Date >= allocationDateFrom.Value.Date)
                                .WhereIf(allocationDateTo.HasValue,
                                            b => b.BookedOn.Date <= allocationDateTo.Value.Date);

            return from i in context.InstallationVw.Where(i => i.InstallationStatus == InstStatus.Booked.ToString())
                   join b in queryBooking on i.InstNo equals b.InstallationNo
                   join t in context.SR_Technician on b.TechnicianId equals t.TechnicianId
                   select new InstTechPrintResult
                   {
                       AcctNo              = i.AcctNo,
                       BookedOn            = b.BookedOn,
                       CustomerName        = String.Format("{0} {1}", i.CustFirstName, i.CustLastName),
                       CustomerId          = i.CustId,
                       InstDate            = b.InstallationDate,
                       InstNo              = b.InstallationNo,
                       ItemNo              = i.ItemNo,
                       CourtsCode          = i.CourtsCode,
                       NumberOfSlots       = b.NoOfSlots,
                       StartSlot           = b.StartSlot,
                       TechnicianId        = t.TechnicianId,                                      
                       TechnicianName      = String.Format("{0} {1}", t.FirstName, t.LastName),
                       InstItemNo          = i.InstItemNo,
                       InstCourtsCode      = i.InstCourtsCode
                   };
        }

        public bool IsBooked(int instNo)
        {
            return Context.Create()
                    .InstallationBooking
                    .Where(b => b.InstallationNo == instNo && !b.IsDeleted)
                    .Any();
        }

        public InstallationBookingPrintXML GetBookingPrintXML(List<int> instNos, string countryCode)
        {
            var context = Context.Create();

            var records = (from i in context.InstallationVw
                           join b in context.InstallationBooking on i.InstNo equals b.InstallationNo
                           join t in context.SR_Technician on b.TechnicianId equals t.TechnicianId
                           join s in context.StockInfo on i.ItemId equals s.Id                              //IP - 26/09/11 - RI - #8239 - CR8201
                           where instNos.Contains(i.InstNo) && !b.IsDeleted                         // re IP - 19/07/11 - CR1254 - RI - #4300
                           select new
                           {
                               AcctNo = i.AcctNo,
                               AgreementNo = i.AgreementNo,
                               DelNoteBranch = i.DelNoteBranch,
                               DateAgreement = i.DateAgreement,
                               InstallationNo = i.InstNo,
                               InstallationDate = i.InstDate,
                               DeliveryDate = i.DelDate,
                               ItemNo = i.ItemNo,
                               ItemDesc = i.ProductDescription2,
                               Manufacturer = i.ProductDescription1,
                               HasWarranty = i.HasWarranty,
                               CustomerName = String.Format("{0} {1}", i.CustFirstName.Trim(), i.CustLastName.Trim()),
                               HomeTel          = i.HomeTelNo,          //IP - 19/07/11 - #4312 - Re-instated
                               WorkTel          = i.WorkTelNo,          //IP - 19/07/11 - #4312 - Re-instated
                               Address1 = i.DelAddress1,
                               Address2 = i.DelAddress2,
                               Address3 = i.DelAddress3, 
                               PostCode = i.DelPostCode,                //NM/IP - 07/03/11 - #3278
                               Direction = i.DelAddressNote,
                               StartSlot = b.StartSlot,
                               NoOfSlots = b.NoOfSlots,
                               TechnicianId = b.TechnicianId,
                               Technician = String.Format("{0} {1} ({2})", t.FirstName.Trim(), t.LastName.Trim(), t.TechnicianId),
                               Brand = s.Brand,                                                                 //IP - 26/09/11 - RI - #8239 - CR8201
                               Style = s.VendorLongStyle                                                        //IP - 26/09/11 - RI - #8239 - CR8201
                           })
                          .ToList();                        //IP - 19/07/11 - #4310 - re-instated. Previously was below. 
                         //.AnsiToList(context);

            int index = records.Count;

            var mainXml = new InstallationBookingPrintXML(countryCode);
            mainXml.Load("<BOOKINGS/>");


            foreach (var r in records)
            {
                var subXml = mainXml.Create(countryCode);

                subXml.SetNode("BOOKING/HEADER/ACCTNO",         r.AcctNo);
                subXml.SetNode("BOOKING/HEADER/AGREEMENTNO",    r.AgreementNo.ToString());
                subXml.SetNode("BOOKING/HEADER/BRANCHNO",       r.DelNoteBranch.NullSafe(x => x.ToString()));
                subXml.SetNode("BOOKING/HEADER/PURCHASEDATE",   r.DateAgreement.NullSafe(x => x.Value.ToShortDateString()));
                //subXml.SetNode("BOOKING/HEADER/INSTNO",         r.InstallationNo.NullSafe(x => x.ToString()));
                subXml.SetNode("BOOKING/HEADER/INSTNO", r.InstallationNo.ToString());           // re IP - 19/07/11 - CR1254 - RI - #4300 
                subXml.SetNode("BOOKING/HEADER/INSTDATE",       r.InstallationDate.NullSafe(x => x.Value.ToShortDateString()));
                subXml.SetNode("BOOKING/HEADER/DELDATE",        r.DeliveryDate.NullSafe(x => x.Value.ToShortDateString()));
                subXml.SetNode("BOOKING/HEADER/ITEMNO",         r.ItemNo);

             
                //IP - 26/09/11 - RI -  #8239 - CR8201
                if (Convert.ToString(CountryParam[CountryParameterNames.RIInterfaceOptions]) != "FACT")
                {
                    subXml.SetNode("BOOKING/HEADER/ITEMDESC", r.ItemDesc + " " + r.Brand + " " + r.Style);
                }
                else
                {
                    subXml.SetNode("BOOKING/HEADER/ITEMDESC", r.ItemDesc);
                }

                subXml.SetNode("BOOKING/HEADER/HASWARRANTY",    r.HasWarranty.NullSafe(x => x.ToString()));
                subXml.SetNode("BOOKING/HEADER/MANUFACTURER",   r.Manufacturer);
                subXml.SetNode("BOOKING/HEADER/MODELNO",        "");
                subXml.SetNode("BOOKING/HEADER/SERIALNO",       "");
                subXml.SetNode("BOOKING/HEADER/CUSTNAME",       r.CustomerName);
                subXml.SetNode("BOOKING/HEADER/HOMETEL", r.HomeTel);                            //IP - 19/07/11 - #4312 - Re-instated
                subXml.SetNode("BOOKING/HEADER/WORKTEL", r.WorkTel);                            //IP - 19/07/11 - #4312 - Re-instated
                subXml.SetNode("BOOKING/HEADER/ADDRESS1", r.Address1);
                subXml.SetNode("BOOKING/HEADER/ADDRESS2", r.Address2);
                subXml.SetNode("BOOKING/HEADER/ADDRESS3", r.Address3);
                subXml.SetNode("BOOKING/HEADER/POSTCODE",       r.PostCode); //NM/IP - 07/03/11 - #3278
                subXml.SetNode("BOOKING/HEADER/DIRECTION",      r.Direction);
                subXml.SetNode("BOOKING/HEADER/TECHNICIAN",     r.Technician);
                subXml.SetNode("BOOKING/HEADER/SLOTS",          Enumerable.Range(r.StartSlot, r.NoOfSlots).Stringify(separator: ","));
                index--;
                subXml.SetNode("BOOKING/LAST", index <= 0 ? "TRUE" : "FALSE");

                mainXml.ImportNode(subXml.DocumentElement, true);
            }

            return mainXml;
        }

        //public IQueryable<InstResult> Search(InstSearchParameter searchParam)
        public List<InstResult> Search(InstSearchParameter searchParam)     //6.5
        {
            var itemIds = new List<int>();

            //--- Get ItemIds --------------------------------------------------------------
            if(!String.IsNullOrWhiteSpace(searchParam.ItemNo))
            {
                var repoItem  = searchParam.ItemNo.Length > 6 && searchParam.ItemNo.StartsWith("R");
                
                itemIds = new StockRepository()
                            .GetStockInfo(repoItem ? searchParam.ItemNo.Remove(0,1) : searchParam.ItemNo, repoItem)
                            .Select(s => s.Id)
                            .ToList();
            }

            if (searchParam.ItemId.HasValue)
                itemIds.Add(searchParam.ItemId.Value);                                                
            //-------------------------------------------------------------------------------
            
            var context = Context.Create();

            var query = context.InstallationVw.AsQueryable();
                
            //-- Build Query ----------------------------------------------------------------
            if (!String.IsNullOrWhiteSpace(searchParam.AcctNo))
                query = query.Where(r => r.AcctNo == searchParam.AcctNo);

            if (!String.IsNullOrWhiteSpace(searchParam.CustID))
                query = query.Where(r => r.CustId == searchParam.CustID);

            if(searchParam.InstNo.HasValue)
                query = query.Where(r => r.InstNo == searchParam.InstNo);

            if (itemIds.Count > 0)                                                                   
                query = query.Where(r => itemIds.Contains(r.ItemId));

            if(!String.IsNullOrWhiteSpace(searchParam.ItemNo) && itemIds.Count == 0)                    //IP - 20/07/11 - #4315 - do not return any result if the item entered does not exist.
                query = query.Where(r => itemIds.Contains(r.ItemId));

            if (searchParam.Status.HasValue)
            {
                var statuses = searchParam.Status.ToStringList();
                query = query.Where(r => statuses.Contains(r.InstallationStatus));
            }

            if (searchParam.InstDateFrom.HasValue)
                query = query.Where(r => r.InstDate.Value.Date >= searchParam.InstDateFrom.Value.Date);

            if (searchParam.InstDateTo.HasValue)
                query = query.Where(r => r.InstDate.Value.Date <= searchParam.InstDateTo.Value.Date);
            //-------------------------------------------------------------------------------

            //return (from i in query
            var records = (from i in query              // 6.5
                    join r in context.InstallationResolution on i.InstNo equals r.InstallationNo
                             into JoinedResolution
                    from r in JoinedResolution.DefaultIfEmpty()
                    orderby i.InstDate descending
                    select new InstResult
                    {
                        InstNo = i.InstNo,
                        InstDate = i.InstDate,
                        InstValue = i.InstValue,
                        Status = i.InstallationStatus,
                        FormattedStatus = i.InstallationStatus, //Formatting should be done after the query execution, could be in the PL layer
                        AcctNo = i.AcctNo,
                        AgreementNo = i.AgreementNo,
                        AuthorisedDate = i.DateDelAuthorised,
                        CustomerId = i.CustId,
                        CustomerName = i.CustFirstName + " " + i.CustLastName,     // #12085 jec 23/01/13  //String.Format("{0} {1}", i.CustFirstName, i.CustLastName),
                        DeliveryAddress1 = i.DelAddress1,
                        DeliveryAddress2 = i.DelAddress2,
                        DeliveryAddress3 = i.DelAddress3,
                        DeliveryDate = i.DelDate,
                        //HasWarranty = i.HasWarranty ?? false,
                        HasWarranty = i.HasWarranty,        //6.5
                        ItemId = i.ItemId,
                        ItemNo = i.ItemNo,
                        CourtsCode = i.CourtsCode,
                        //ModelNumber          = "",
                        //PhoneHome              = i.HomeTelNo,
                        //PhoneWork              = i.WorkTelNo,
                        ProductDescription1 = i.ProductDescription1,
                        ProductDescription2 = i.ProductDescription2,
                        Quantity = i.Quantity,
                        //SerialNumber         = "",
                        StockLocation = i.StockLocation,
                        SupplierName = i.SupplierCode,
                        DeliveryStatus = i.DelStatus,
                        //PrimaryChargeCode = i.StockCategory.In(CAT.ProductCatElectrical, CAT.ProductCatWarehouse) ? INST.PRIMARYCHARGE.Electrical :
                        //                         i.StockCategory == CAT.ProductCatFurniture ? INST.PRIMARYCHARGE.Furniture : "",
                        PrimaryChargeCode = i.StockCategory,
                        ResolutionRowVersion = r.RowVersion,
                        TechnicianId = i.TechnicianId,                               //IP - 19/07/11 - #4303
                        TechnicianName = i.TechnicianName                              //IP - 19/07/11 - #4303
                    })
                     .AnsiTake(context, 100).ToList();                   // merge from 6.5 //IP - 25/08/11 - #4559 - Replaces below
            //.AnsiTake(context,100);

            records.ForEach(r =>
            {
                r.PrimaryChargeCode = UpdatePrimaryCharge(r.PrimaryChargeCode);
            });
            return records;

        }

        private string UpdatePrimaryCharge(string stockCategory)
        {
            if (stockCategory == CAT.ProductCatElectrical || stockCategory == CAT.ProductCatWarehouse)
                return INST.PRIMARYCHARGE.Electrical;
            if (stockCategory == CAT.ProductCatFurniture)
                return CAT.ProductCatFurniture;
            else
                return string.Empty;
        }

        public StockItemDetails GetSparePartDetail(string partNo)
        {
            var context = Context.Create();

            var stockInfo = context.StockInfo
                                .Where(s => s.IUPC == partNo || 
                                            s.itemno == partNo || 
                                            s.SKU == partNo)
                                .Where(s => s.SparePart == true)
                                .AnsiFirstOrDefault(context);

            if (stockInfo == null)
                return null;

            //Joined with branch coz there are stocklocations they are not in branch table
            var queryStockPrice = from s in context.StockPrice
                                  join b in context.Branch on s.branchno equals b.branchno
                                  where s.ID == stockInfo.Id
                                  orderby s.branchno
                                  select s;

            var queryStockQty = from s in context.StockQuantity
                                join b in context.Branch on s.stocklocn equals b.branchno
                                where s.ID == stockInfo.Id
                                orderby s.stocklocn
                                select s;
            
            return new StockItemDetails
            {
                StockInfo = stockInfo,
                StockPrices = queryStockPrice.AnsiToList(context),
                StockQuantities = queryStockQty.AnsiToList(context)
            };
        }
        
        public List<InstChargeAnalysisResult> UpdateResolution(int instNo, InstallationResolution resolution, List<InstallationSparePart> spareParts, 
                                                                        int user, DbConnection conn = null, DbTransaction trans = null)
        {
            var currentTime = DateTime.Now;
            
            using (var scope = new TransactionScope())
            {
                var courtsPartCost = spareParts.Where(p => !p.IsNonCourts).Sum(p => p.UnitPrice * Convert.ToDecimal(p.Quantity));
                var nonCourtsPartCost = spareParts.Where(p => p.IsNonCourts).Sum(p => p.UnitPrice * Convert.ToDecimal(p.Quantity));

                var totalCost = resolution.AdditionalCost +
                                resolution.LabourCost +
                                courtsPartCost +
                                nonCourtsPartCost;

                var analyses = new List<InstChargeAnalysisResult>();

                var context = Context.Create(conn, trans);

                var existingRecord = context.InstallationResolution
                                        .FirstOrDefault(r => r.InstallationNo == instNo);

                if (existingRecord != null)
                {
                    existingRecord.PrimaryChargeTo   = resolution.PrimaryChargeTo;
                    existingRecord.AdditionalCost    = resolution.AdditionalCost;
                    existingRecord.LabourCost        = resolution.LabourCost;
                    existingRecord.CourtsPartCost    = courtsPartCost;
                    existingRecord.NonCourtsPartCost = nonCourtsPartCost;
                    existingRecord.IsCompleted       = resolution.IsCompleted;
                    existingRecord.TotalCost         = totalCost;
                    existingRecord.LastUpdatedBy     = user;
                    existingRecord.LastUpdatedOn     = currentTime;
                    existingRecord.AuthorisedBy      = resolution.IsCompleted ? resolution.AuthorisedBy : null;
                    existingRecord.AuthorisedOn      = resolution.IsCompleted ? (DateTime?)currentTime : null;
                    //existingRecord.RowVersion        = resolution.RowVersion; //todo resolve this issue

                    if(ASCIIEncoding.ASCII.GetString(existingRecord.RowVersion) != ASCIIEncoding.ASCII.GetString(resolution.RowVersion))
                    {
                        var ex = new STLException(); //todo - delete
                        ex.Data["IsConflict"] = true;
                        throw ex;
                    }
                    analyses = BuildChargeAnalysis(context, existingRecord);
                }
                else
                {
                    var newRecord = new InstallationResolution
                    {
                        InstallationNo    = instNo,
                        PrimaryChargeTo   = resolution.PrimaryChargeTo,
                        AdditionalCost    = resolution.AdditionalCost,
                        LabourCost        = resolution.LabourCost,
                        CourtsPartCost    = courtsPartCost,
                        NonCourtsPartCost = nonCourtsPartCost,
                        IsCompleted       = resolution.IsCompleted,
                        TotalCost         = totalCost,
                        AuthorisedBy      = resolution.IsCompleted ? resolution.AuthorisedBy : null,
                        AuthorisedOn      = resolution.IsCompleted ? (DateTime?)currentTime : null,
                        CreatedBy         = user,
                        CreatedOn         = currentTime
                    };

                    context.InstallationResolution.InsertOnSubmit(newRecord);

                    analyses = BuildChargeAnalysis(context, newRecord);
                }


                var installation = context.Installation
                                    .FirstOrDefault(i => i.InstallationNo == instNo);

                installation.Status = resolution.IsCompleted ? InstStatus.Closed.ToString() : InstStatus.ToBeClosed.ToString();                

                //Delete existing spare parts and insert again
                var existingSpareParts = context.InstallationSparePart.Where(p => p.InstallationNo == instNo);
                context.InstallationSparePart.DeleteAllOnSubmit(existingSpareParts);

                var newSpareParts = spareParts.Select(p => new InstallationSparePart
                {
                    InstallationNo = instNo,
                    IsNonCourts    = p.IsNonCourts,
                    PartNo         = p.PartNo,
                    PartID         = p.PartID,
                    Description    = p.Description,
                    StockLocation  = p.StockLocation,
                    UnitPrice      = p.UnitPrice,
                    Quantity       = p.Quantity,
                    Total          = p.UnitPrice * Convert.ToDecimal(p.Quantity)
                });

                context.InstallationSparePart.InsertAllOnSubmit(newSpareParts);

                //Charge Analysis
                var chargeAnalysisExists = context.InstallationChargeAnalysis.Any(ch => ch.InstallationNo == instNo);
                
                if (!chargeAnalysisExists && resolution.IsCompleted)
                {
                    context.InstallationChargeAnalysis
                        .InsertAllOnSubmit(analyses.Select(a => new InstallationChargeAnalysis
                        {
                            InstallationNo = instNo,
                            BreakDownCode  = a.BreakDownCode,
                            Electrical     = a.Electrical,
                            Furniture      = a.Furniture
                        }));
                }

                context.SubmitChanges();
                scope.Complete();

                return !chargeAnalysisExists && resolution.IsCompleted ? analyses : null;
            }
        }

        public InstResolutionResult GetResolution(int instNo)
        {
            var context = Context.Create();

            var resolution = context.InstallationResolution
                                .Where(r => r.InstallationNo == instNo)
                                .AnsiFirstOrDefault(context);

            if (resolution == null)
                return null;

            var spareParts = (from p in context.InstallationSparePart
                             join stock in context.StockInfo on p.PartID equals stock.Id into JoinedStockInfo
                             from s in JoinedStockInfo.DefaultIfEmpty() //left join
                              where p.InstallationNo == instNo
                             select new
                             {
                                 SparePart = p,
                                 Stock = s
                             })
                             .ToList();

            spareParts.ForEach(p=> p.SparePart.CourtsPartNo = p.Stock!=null?p.Stock.itemno:null);

            return new InstResolutionResult
            {
                Resolution  = resolution,
                SpareParts  = spareParts.Select(p=> p.SparePart).ToList()
            };
        }

        public List<InstChargeAnalysisResult> BuildInitialChargeAnalysis()
        {
            return BuildChargeAnalysis(null, null);
        }

        private List<InstChargeAnalysisResult> BuildChargeAnalysis(Context context = null, InstallationResolution resolution = null)
        {
            context = context ?? Context.Create();

            var analyses = context.Code
                            .Where(c => c.category == INST.CHARGEBREAKDOWN.Category)
                            .OrderBy(c => c.sortorder)
                            .Select(c => new InstChargeAnalysisResult
                            {
                                BreakDownCode = c.code,
                                BreakDownText = c.codedescript
                            })
                            .AnsiToList(context);

            if (resolution != null) 
            {
                Func<InstChargeAnalysisResult, Decimal> getBreakdownValue = (ch) =>
                {
                    return
                    ch.BreakDownCode == INST.CHARGEBREAKDOWN.PartsCourts ?  resolution.CourtsPartCost :
                    ch.BreakDownCode == INST.CHARGEBREAKDOWN.PartsOther  ?  resolution.NonCourtsPartCost :
                    ch.BreakDownCode == INST.CHARGEBREAKDOWN.PartsTotal  ?  resolution.CourtsPartCost + resolution.NonCourtsPartCost :
                    ch.BreakDownCode == INST.CHARGEBREAKDOWN.LabourTotal ?  resolution.AdditionalCost + resolution.LabourCost :
                    ch.BreakDownCode == INST.CHARGEBREAKDOWN.Total       ?  resolution.TotalCost : default(Decimal);
                };
                
                foreach(var analysis in analyses)
                { 
                    analysis.InstNo = resolution.InstallationNo;

                    if(resolution.PrimaryChargeTo == INST.PRIMARYCHARGE.Electrical)
                        analysis.Electrical = getBreakdownValue(analysis);
                    else if(resolution.PrimaryChargeTo == INST.PRIMARYCHARGE.Furniture)
                        analysis.Furniture = getBreakdownValue(analysis);
                };
            }

            return analyses;
        }

        public List<Blue.Cosacs.Messages.Service.RequestSubmit> GetInstallationData(SqlConnection conn, SqlTransaction trans, DataTable lineItemBooking)        
        {
            using (var ctx = Context.Create(conn, trans))
            {
                List<Blue.Cosacs.Messages.Service.RequestSubmit> installation = new List<Blue.Cosacs.Messages.Service.RequestSubmit>();

                //var repoDelUnitPrice = Convert.ToBoolean(Country[CountryParameterNames.RepoDelUnitPrice]);     

                HiLo requestID = HiLo.Cache("Service.Request");

                foreach (DataRow dr in lineItemBooking.Rows)
                {

                    var details = (from lb in ctx.LineItemBooking
                                   join l in ctx.LineItem on lb.LineItemID equals l.ID
                                   join li in ctx.LineItem on l.ItemID equals li.ParentItemID 
                                   join b in ctx.Branch on l.SalesBrnNo != null ? l.SalesBrnNo : Convert.ToInt16(l.acctno.Substring(0, 3)) equals b.branchno     // #11042 l.SalesBrNo may be null
                                   join ca in ctx.CustAcct on l.acctno equals ca.acctno
                                   join a in ctx.Acct on ca.acctno equals a.acctno
                                   join ag in ctx.Agreement on a.acctno equals ag.acctno        
                                   join cu in ctx.Customer on ca.custid equals cu.custid
                                   join cadd in ctx.CustAddress on ca.custid equals cadd.custid
                                   join si in ctx.StockInfo on l.ItemID equals si.Id                //main item
                                   join sii in ctx.StockInfo on li.ItemID equals sii.Id             // installation item - lineitem
                                   join co in ctx.Code on Convert.ToString(sii.category) equals co.code
                                   join us in ctx.UserView on ag.empeenosale equals us.Id
                                   join ctelH in ctx.CustTel on cu.custid equals ctelH.custid into h
                                   from hTel in
                                       (from ctelH in h where ctelH.tellocn == "H" && ctelH.datediscon == null select ctelH).DefaultIfEmpty()          // #10299
                                   join ctelW in ctx.CustTel on cu.custid equals ctelW.custid into w
                                   from wTel in
                                       (from ctelW in w where ctelW.tellocn == "W" && ctelW.datediscon == null select ctelW).DefaultIfEmpty()          // #10299
                                   join ctelM in ctx.CustTel on cu.custid equals ctelM.custid into m
                                   from mTel in
                                       (from ctelM in m where ctelM.tellocn == "M" && ctelM.datediscon == null select ctelM).DefaultIfEmpty()          // #10299
                                   where ca.hldorjnt == "H" &&
                                   l.acctno==li.acctno &&           // #11808
                                   cadd.addtype == l.deliveryaddress &&                     // #10311 
                                   cadd.datemoved == null &&
                                   ((co.category == "PCE" || co.category == "PCF" || co.category == "PCW") && co.codedescript == "Installation") &&      // installation item
                                   lb.ID == Convert.ToInt32(dr["BookingID"])

                                   select new Blue.Cosacs.Messages.Service.RequestSubmit
                                   {
                                       Id = requestID.NextId(),
                                       Account = l.acctno,
                                       Branch=l.stocklocn,
                                       CreatedBy = us.FullName,
                                       CreatedById = ag.empeenosale,
                                       CustomerId = cu.custid,
                                       Title = cu.title,
                                       FirstName = cu.firstname,
                                       LastName= cu.name,
                                       AddressLine1 = cadd.cusaddr1,
                                       AddressLine2 = cadd.cusaddr2,
                                       AddressLine3 = cadd.cusaddr3,
                                       PostCode = cadd.cuspocode,
                                       HomePhone = (hTel.DialCode == null ? "" : hTel.DialCode.Trim()) + " " + (hTel.telno == null ? "" : hTel.telno.Trim()) + " " + (hTel.extnno == null ? "" : hTel.extnno.Trim()),
                                       WorkPhone = (wTel.DialCode == null ? "" : wTel.DialCode.Trim()) + " " + (wTel.telno == null ? "" : wTel.telno.Trim()) + " " + (wTel.extnno == null ? "" : wTel.extnno.Trim()),
                                       MobilePhone = (mTel.DialCode == null ? "" : mTel.DialCode.Trim()) + " " + (mTel.telno == null ? "" : mTel.telno.Trim()),
                                       Email = cadd.Email,
                                       Notes = cadd.Notes,
                                       ItemId = l.ItemID,
                                       ItemNumber=si.IUPC,      // Main Item
                                       ItemValue=li.ordval,     // Installation Item value
                                       ItemSoldBy = us.FullName,
                                       Product = si.itemdescr1 + " " + "\n" + si.itemdescr2,
                                       StockLocation= l.stocklocn,      // #16666
                                       Supplier = si.Supplier             // #16666
                                   }


                                   ).ToList<Blue.Cosacs.Messages.Service.RequestSubmit>();

                    if (details.Count != 0)         //#11793
                    {
                        installation.Add(details.FirstOrDefault());
                    }

                }

                return installation;
            }
        }

        // #14432
        public DataTable GetInstallationItems(SqlConnection conn =null, SqlTransaction trans=null)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var InstallationItemCat = (from s in ctx.StockInfo
                               join co in ctx.Code on Convert.ToString(s.category) equals co.code
                               where (co.category == "PCE" || co.category == "PCF" || co.category == "PCW") && co.codedescript == "Installation"

                               select new 
                               {
                                   Code = s.IUPC
                               }

                               ).ToDataTable();

                return InstallationItemCat;
            }
        }
        // #14432 Paid&Taken Installations
        public List<Blue.Cosacs.Messages.Service.RequestSubmit> GetInstallationDataPaidAndTaken(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                List<Blue.Cosacs.Messages.Service.RequestSubmit> installation = new List<Blue.Cosacs.Messages.Service.RequestSubmit>();

                DataTable Acct = new DataTable();
                Acct.Columns.Add(CN.AcctNo);
                DataRow ar = Acct.NewRow();
                ar[CN.AcctNo] = accountNo;

                

                var LineItemPaidandTaken = (from l in ctx.LineItem
                                            where l.acctno==accountNo && l.quantity>0
                                            select new
                                            {
                                                LineItemID = l.ID,
                                                ItemID = l.ItemID,
                                                stocklocn=l.stocklocn
                                            }
                                            ).ToDataTable();

                HiLo requestID = HiLo.Cache("Service.Request");

                foreach (DataRow dr in LineItemPaidandTaken.Rows)
                {
                    var ServiceReques = new ServiceRequestGetSP(conn, trans);
                    // new code add for Invoice CR by Tosif ali
                    DataSet ServiceRequest = ServiceReques.ExecuteDataSet(accountNo, Convert.ToString(dr["ItemID"]), Convert.ToString(dr["stocklocn"]));

                    if (ServiceRequest.Tables[0].Rows.Count <= 0)
                    {
                        /// ENd here .....
                        var details = (from l in ctx.LineItem
                                       join li in ctx.LineItem on l.ItemID equals li.ParentItemID
                                       join b in ctx.Branch on l.SalesBrnNo != null ? l.SalesBrnNo : Convert.ToInt16(l.acctno.Substring(0, 3)) equals b.branchno     // #11042 l.SalesBrNo may be null
                                       join ca in ctx.CustAcct on l.acctno equals ca.acctno
                                       join a in ctx.Acct on ca.acctno equals a.acctno
                                       join ag in ctx.Agreement on a.acctno equals ag.acctno
                                       join cu in ctx.Customer on ca.custid equals cu.custid
                                       join cadd in ctx.CustAddress on ca.custid equals cadd.custid
                                       join si in ctx.StockInfo on l.ItemID equals si.Id                //main item
                                       join sii in ctx.StockInfo on li.ItemID equals sii.Id             // installation item - lineitem
                                       join co in ctx.Code on Convert.ToString(sii.category) equals co.code
                                       join us in ctx.UserView on ag.empeenosale equals us.Id
                                       join ctelH in ctx.CustTel on cu.custid equals ctelH.custid into h
                                       from hTel in
                                           (from ctelH in h where ctelH.tellocn == "H" && ctelH.datediscon == null select ctelH).DefaultIfEmpty()          // #10299
                                       join ctelW in ctx.CustTel on cu.custid equals ctelW.custid into w
                                       from wTel in
                                           (from ctelW in w where ctelW.tellocn == "W" && ctelW.datediscon == null select ctelW).DefaultIfEmpty()          // #10299
                                       join ctelM in ctx.CustTel on cu.custid equals ctelM.custid into m
                                       from mTel in
                                           (from ctelM in m where ctelM.tellocn == "M" && ctelM.datediscon == null select ctelM).DefaultIfEmpty()          // #10299

                                       where ca.hldorjnt == "H" &&
                                       l.acctno == li.acctno &&           // #11808
                                       cadd.addtype == l.deliveryaddress &&                     // #10311 
                                       cadd.datemoved == null &&
                                       ((co.category == "PCE" || co.category == "PCF" || co.category == "PCW") && co.codedescript == "Installation") &&      // installation item
                                       l.ID == Convert.ToInt32(dr["LineItemID"])

                                       select new Blue.Cosacs.Messages.Service.RequestSubmit
                                       {
                                           Id = requestID.NextId(),
                                           Account = l.acctno,
                                           Branch = l.stocklocn,
                                           CreatedBy = us.FullName,
                                           CreatedById = ag.empeenosale,
                                           CustomerId = cu.custid,
                                           Title = cu.title,
                                           FirstName = cu.firstname,
                                           LastName = cu.name,
                                           AddressLine1 = cadd.cusaddr1,
                                           AddressLine2 = cadd.cusaddr2,
                                           AddressLine3 = cadd.cusaddr3,
                                           PostCode = cadd.cuspocode,
                                           HomePhone = (hTel.DialCode == null ? "" : hTel.DialCode.Trim()) + " " + (hTel.telno == null ? "" : hTel.telno.Trim()) + " " + (hTel.extnno == null ? "" : hTel.extnno.Trim()),
                                           WorkPhone = (wTel.DialCode == null ? "" : wTel.DialCode.Trim()) + " " + (wTel.telno == null ? "" : wTel.telno.Trim()) + " " + (wTel.extnno == null ? "" : wTel.extnno.Trim()),
                                           MobilePhone = (mTel.DialCode == null ? "" : mTel.DialCode.Trim()) + " " + (mTel.telno == null ? "" : mTel.telno.Trim()),
                                           Email = cadd.Email,
                                           Notes = cadd.Notes,
                                           ItemId = l.ItemID,
                                           ItemNumber = si.IUPC,      // Main Item
                                           ItemValue = li.ordval,     // Installation Item value
                                           ItemSoldBy = us.FullName,
                                           Product = si.itemdescr1 + " " + "\n" + si.itemdescr2,
                                           StockLocation = l.stocklocn,      // #17191
                                           Supplier = si.Supplier             // #17191
                                       }


                                       ).ToList<Blue.Cosacs.Messages.Service.RequestSubmit>();


                        if (details.Count != 0)         //#11793
                        {
                            installation.Add(details.FirstOrDefault());
                        }
                    }
                }

                return installation;
            }
        } 
    } 
}
