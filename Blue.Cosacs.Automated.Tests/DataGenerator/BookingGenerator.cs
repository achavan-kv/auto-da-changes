using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs;
using System.Data.SqlClient;


namespace DataGenerator
{
    public class BookingGenerator
    {
        public void InsertBookings()
        {
            var bookingXml = new System.Xml.XmlDocument();
            #region BookingXml
            bookingXml.LoadXml(@"<BookingSubmit xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://www.bluebridgeltd.com/cosacs/2012/schema.xsd"">
                                  <Id></Id>
                                  <CustomerName>MR Selenium User</CustomerName>
                                  <AddressLine1>Test Address Line 1</AddressLine1>
                                  <AddressLine2>Test Address Line 2</AddressLine2>
                                  <AddressLine3>Test Address Line 3</AddressLine3>
                                  <PostCode>3435</PostCode>
                                  <StockBranch>747</StockBranch>
                                  <DeliveryBranch>747</DeliveryBranch>
                                  <DeliveryOrCollection>D</DeliveryOrCollection>
                                  <DeliveryOrCollectionDate>2014-12-31T16:09:59.213</DeliveryOrCollectionDate>
                                  <ItemNo>101128</ItemNo>
                                  <ItemId>42</ItemId>
                                  <ItemUPC>101128</ItemUPC>
                                  <ProductDescription>Selenium Test Product</ProductDescription>
                                  <ProductBrand>RCA</ProductBrand>
                                  <ProductModel>S21H92</ProductModel>
                                  <ProductArea />
                                  <ProductCategory>21</ProductCategory>
                                  <Quantity>2</Quantity>
                                  <RepoItemId>42</RepoItemId>
                                  <Comment>fgfgh</Comment>
                                  <DeliveryZone />
                                  <ContactInfo>Home 45 3453 Work Mobile  53453 </ContactInfo>
                                  <OrderedOn>2013-08-30T17:03:08.637</OrderedOn>
                                  <Damaged>false</Damaged>
                                  <AssemblyReq>false</AssemblyReq>
                                  <Express>false</Express>
                                  <Acctno>747403469747</Acctno>
                                  <UnitPrice>381.3300</UnitPrice>
                                  <CreatedBy>99999</CreatedBy>
                                  <AddressNotes />
                                  <Fascia>C</Fascia>
                                  <PickUp>false</PickUp>
                                </BookingSubmit>");
            #endregion
            //St. Lucia Branches = 740, 741, 742, 743, 744, 745, 746, 747, 748, 749, 750, 751, 752, 960, 969, 999
            //Brabados Branches = 900, 901, 902, 903, 904, 905, 906, 907, 908, 909, 910, 911, 912, 913, 916, 918
            //Belize Branches = 98, 99, 121, 122, 123, 124, 130, 135, 136, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789
            var seq = HiLo.Cache("Warehouse.Booking");
            using (var con = new SqlConnection())
            {
                con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    const string routing = "Warehouse.Booking.Submit";
                    try
                    {
                        for (int i = 0; i < 800; i++)
                        {
                            var strSql = new StringBuilder();
                            bookingXml.ChildNodes[0].ChildNodes[0].InnerText = seq.NextId().ToString();
                            strSql.Append("INSERT INTO Hub.Message (CreatedOn, Body, Routing)VALUES(");
                            strSql.Append(" GetDate(), '");
                            strSql.Append(bookingXml.OuterXml);
                            strSql.Append("', '");
                            strSql.Append(routing);
                            strSql.Append("')");
                            var command = new SqlCommand(strSql.ToString(), con, tran);
                            command.CommandType = System.Data.CommandType.Text;
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        Console.WriteLine(e.Message);
                        return;
                    }
                    tran.Commit();
                }
            }
            bookingXml = null;
            Console.WriteLine("Bookings Inserted Successfully");
        }
    }
}
