using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Services;
using Blue.Cosacs.Shared;

namespace STL.PL
{
    public partial class NewAccount
    {
        private void AddFreeWarranty(string code, string location, string key, int itemQty, int FreeReplacementQty = 0)           // #17677 - FreeReplacementQty from GRT
        {
            //var warranties = Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).GetFreeWarranties(code, location);
            warranties = Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).GetWarranties(code, location, WarrantyType.Free);

            if (dvExchange != null) //#16828
            {
                XmlNode exchangeParent = xml.findItem(itemDoc.DocumentElement, key);

                ProcessExchange(exchangeParent);        //#16828
            }

            if (warranties.Items == null || warranties.Items.Count == 0)// || warranties.Items.Count != 0)
                return;

            // #16169 - check for existing free warranties
            var existingFree = 0;
            //string xPath = "//Item[@Type = 'Warranty' and @IsFree = 'True' and @Code = '" + code + "' and @Location = '" + location.ToString() + "']";  //#16277
            string xPath = "//Item[@Type = 'Warranty' and @WarrantyType ='" + WarrantyType.Free + "']"; //#17287 //#16277

            string xPathItem = "//Item[@Code = '" + code + "' and @Location = '" + location.ToString() + "']";

            //#16303
            XmlNode item = itemDoc.SelectSingleNode(xPathItem);
            string xPathRelated = "RelatedItems/Item[@Type = 'Warranty' and @WarrantyType ='" + WarrantyType.Free + "' and @Quantity != '0']"; //#17287
            XmlNodeList freeWarranties = item.SelectNodes(xPathRelated);

            if (freeWarranties != null && freeWarranties.Count > 0)
            {
                existingFree = freeWarranties.Count;
            }            

            // #16169 - If item quantity reduced - set quantity of surplus free warranties to zero
            if (existingFree > itemQty)
            {
                foreach (XmlNode exists in freeWarranties)
                {
                    if (existingFree > itemQty)
                    {
                        if (exists.Attributes[Tags.Quantity].Value == "1")
                        {
                            exists.Attributes[Tags.Quantity].Value = "0";
                            existingFree--;
                        }
                    }
                }
            }

            //var qty = FreeReplacementQty == 0 ? itemQty - existingFree : FreeReplacementQty - existingFree;   // #17677 - use Replacement qty if value passed in from GRT
            var qty = FreeReplacementQty == 0 ? itemQty - existingFree : existingFree == 0 ? 1 : FreeReplacementQty - existingFree; //#18437  // #17677 - use Replacement qty if value passed in from GRT

            for (var i = 0; i < qty; i++)
            {
                string Error;
                string contract = AccountManager.AutoWarranty(location, out Error);
                if (!string.IsNullOrEmpty(Error))
                    throw new Exception("Can not load warranty contract number. " + Error);
            
                var warranty = warranties.Items[0].ToItem();
                warranty.ContractNumber = contract;
                warranty.Value = 0;
                warranty.Quantity = 1;
                warranty.DeliveryDate = Convert.ToString(dtDeliveryRequired.Value);
                warranty.DeliveryTime = Convert.ToString(cbTime.SelectedItem);
                warranty.BranchForDeliveryNote = drpBranch.Text;
                warranty.Location = Convert.ToInt32(drpBranch.Text);

                XmlNode parent = xml.findItem(itemDoc.DocumentElement, key);
                ////////string xParentPathItem = "//Item[@Code = '" + code + "' and @Location = '" + location.ToString() + "' and @Quantity != '0']";
                ////////XmlNode parent = itemDoc.SelectSingleNode(xParentPathItem);

                xPath = "//Item[@Type = 'Warranty' and @Code = '" + warranty.Code + "' and @Location = '" + warranty.Location.ToString() + "' and @ContractNumber = '" + warranty.ContractNumber + "']";
                foreach (XmlNode toDel in itemDoc.SelectNodes(xPath))
                {
                    if (toDel.ParentNode.ParentNode.Attributes[Tags.Key].Value == key ||
                        toDel.Attributes[Tags.Quantity].Value == "0")
                        toDel.ParentNode.RemoveChild(toDel);
                }
                parent.SelectSingleNode("RelatedItems").AppendChild(parent.OwnerDocument.ImportNode(warranty.ToXml(), true));
            }
        }
    }
}
