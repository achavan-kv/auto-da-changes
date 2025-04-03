using System;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Enums;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.Elements;

namespace STL.Common
{
	public class XmlUtilities : CommonObject
	{

		public XmlAttribute Attribute(XmlDocument doc, string attName, string attValue)
		{
			XmlAttribute att = doc.CreateAttribute(attName);
			att.Value = attValue;
			return att; 
		}
		/// <summary>
		/// Recursive function to loop through all items in the order
		/// and add up the value of all which are discounts
		/// </summary>
		/// <param name="lineItems"></param>
		/// <returns></returns>
		/// 
		public decimal CalculateDiscount(XmlNode lineItems)
		{
			decimal discount = 0;

			//iterate through item tags
			foreach(XmlNode item in lineItems.ChildNodes)
			{
				if(item.NodeType==XmlNodeType.Element)	// ignore whitespace
				{
					if(item.Attributes[Tags.Type].Value == IT.Discount ||
						item.Attributes[Tags.Type].Value == IT.KitDiscount)
					{
						discount += Convert.ToDecimal(StripCurrency(item.Attributes[Tags.Value].Value));
					}


					foreach(XmlNode child in item.ChildNodes)
						if(child.NodeType==XmlNodeType.Element&&child.Name==Elements.RelatedItem)
						{
							if(child.HasChildNodes)
								discount += CalculateDiscount(child);
						}
				}
			}
			return discount;	
		}

		/// <summary>
		/// This is a recursive function which will search the xml
		/// structure for a node with a particular key. It will always
		/// either return the node requested or a null node.
		/// </summary>
		/// <param name="relatedItems"></param>
		/// <param name="key"></param>
		/// <returns></returns>

        // rdb 363 added parentproductCode 
        // lots of references to original version, keeping but not 100% 
        public XmlNode findItem(XmlNode relatedItems, string key)
        {
            XmlNode found = null;
            string xPath = "//Item[@Key = '" + key + "']";

            found = relatedItems.SelectSingleNode(xPath);

            return found;
        }

		public XmlNode findItem(XmlNode relatedItems, string key, string parentProductCode)
		{
            // rdb 363 added parentproductCode 
			XmlNode found = null;
			string xPath = "//Item[@Key = '"+ key +"'";
            if (parentProductCode != string.Empty)
                xPath += "and ../../@Code = '" + parentProductCode + "'";
            xPath += "]";

			found = relatedItems.SelectSingleNode(xPath);

			return found;	//will either contain our node or be null
		}


		/// <summary>
		/// This function sets the quantity of this item zero. It will
		/// also set the quantity of all related items to zero by
		/// calling itself
		/// </summary>
		/// <param name="toDelete"></param>
		public void deleteItem(XmlNode toDelete)
		{
			Function = "deleteItem";

            toDelete.Attributes[Tags.Quantity].Value = "0";
			toDelete.Attributes[Tags.Value].Value = "0";

			XmlNode related = toDelete.SelectSingleNode(Elements.RelatedItem);
			if(related!=null)
			{
				foreach(XmlNode child in related.ChildNodes)
				{
					if(child.Name==Elements.Item && child.NodeType==XmlNodeType.Element)
					{
						deleteItem(child);
					}
				}
			}
		}

		/// <summary>
		/// This function sets the quantity of this item zero. It will
		/// also set the quantity of all related items to zero by
		/// calling itself. 
		/// Alternative version which takes a parameter to determine 
		/// whether to delete warranties or not.
        /// rdb 12/10/07 altering to return a delivered warranty on a non-delivered item
        /// back to the parent item level - see note below
        /// 
		/// </summary>
		/// <param name="toDelete"></param>
		public void deleteItem(XmlNode toDelete, bool deleteWarranties, XmlElement itemsElement)
		{
			Function = "deleteItem";

			string type = toDelete.Attributes[Tags.Type].Value;

			if( (type != IT.Warranty &&
				type != IT.KitWarranty) ||
				deleteWarranties )
			{
				toDelete.Attributes[Tags.Quantity].Value = "0";
				toDelete.Attributes[Tags.Value].Value = "0";
			}

			XmlNode related = toDelete.SelectSingleNode(Elements.RelatedItem);
			if(related!=null)
			{
				foreach(XmlNode child in related.ChildNodes)
				{
					if(child.Name==Elements.Item && child.NodeType==XmlNodeType.Element)
					{
                        // 69282 rdb check here if the warranty has been delivered and the parent item has not been delivered
                        // if so the scenario is that an item with a waranty has been collected for exchange.  We
                        // have then returned to NewAccount screen and added a new item, the orginal warranty attached to this item
                        // however the new item was then removed, instead of deleting warranty return it to the xml as a lineitem

                        // 69282 rdb problem 2 - we need check if there are any other items that we can add warranty to


                        // if (currentItem.Attributes[Tags.CanAddWarranty].Value == Boolean.TrueString)

                        if (Convert.ToInt32(child.Attributes[Tags.DeliveredQuantity].Value) > 0 
                            && Convert.ToInt32(toDelete.Attributes[Tags.DeliveredQuantity].Value )== 0)
                        {
                            // get other items before we remove warranty or the item we are deleting from will be included in the selection
                            XmlNodeList availableItems = itemsElement.SelectNodes("//item[@CanAddWaranty='True' and count( RelatedItems/Items)=0]");
                            // move to parent
                            related.RemoveChild(child);

                            // 15/11/07 rdb amended to actually delete the warranty when deleteWarranty true
                            // need to check if a warranty should be deleteded on a replacement item
                            if (deleteWarranties)
                            {
                                deleteItem(child, deleteWarranties, itemsElement);                            
                            }
                            else
                            {
                                if (availableItems.Count == 0)
                                {
                                    itemsElement.AppendChild(child);
                                }
                                else
                                {
                                    availableItems[0].AppendChild(child);
                                }
                            }
                        }
                        else
                        {
                            deleteItem(child, deleteWarranties , itemsElement);
                        }
					}
				}
			}
		}

		/// <summary>
		/// This function will take a new item node and an old item 
		/// node and replace the new item node's related items node with the 
		/// related items node from the old item. This is used when an
		/// item is being overwritten
		/// The related items quantity fields must also be updated
		/// to match the quantity of the new parent.
		/// </summary>
		/// <param name="oldItem"></param>
		/// <param name="newItem"></param>
		public void replaceRelatedItems(XmlNode oldItem, XmlNode newItem)
		{
			Function = "replaceRelatedItems";

			XmlNode oldRelated = null;
			XmlNode newRelated = null;
			double newQty = 0;
			double oldQty = 0;
			double curQty = 0;
			double unitPrice = 0;


			newRelated = newItem.SelectSingleNode(Elements.RelatedItem);
			newQty = Convert.ToDouble(newItem.Attributes[Tags.Quantity].Value);

			oldRelated = oldItem.SelectSingleNode(Elements.RelatedItem);
			oldQty = Convert.ToDouble(oldItem.Attributes[Tags.Quantity].Value);
			
			//quantities must be set to (currentvalue / oldqty) * new quantity			
			if(oldRelated!=null&&newRelated!=null)
			{
				foreach(XmlNode item in oldRelated.ChildNodes)
				{
					if(item.NodeType==XmlNodeType.Element)
					{
						if(item.Name==Elements.Item)
						{
                            if (((string)item.Attributes[Tags.Type].Value != IT.Warranty || newQty < oldQty) &&
                                (string)item.Attributes[Tags.Type].Value != IT.Discount
                                && (string)item.Attributes[Tags.Type].Value != IT.Stock) //RI - #4241 - prevent associated stock items quantity from being updated to new quantity of main item
                            {
								unitPrice = Convert.ToDouble(StripCurrency(item.Attributes[Tags.UnitPrice].Value));
								curQty = Convert.ToDouble(item.Attributes[Tags.Quantity].Value);
								if((string)item.Attributes[Tags.Type].Value == IT.Warranty)
								{
									if(newQty < curQty)
									{
										item.Attributes[Tags.Quantity].Value = newQty.ToString();
										item.Attributes[Tags.Value].Value = (newQty * unitPrice).ToString();

                                        XmlNode tempNode = item.SelectSingleNode(Elements.ContractNos); //uat(5.2)-907 , 4.3 merge //uat(4.3)-162
                                        if (tempNode != null)
                                        {
                                            XmlNode contract = tempNode.FirstChild;
                                            for (int i = 0; i < (curQty - newQty); i++)
                                            {
                                                contract.Attributes[Tags.DelToFact].Value = "-1";
                                                contract = contract.NextSibling;
                                            }
                                        }
									}
								}
								else
								{
                                    // new code add for Invoice CR by Tosif ali
                                    if (curQty < 1 || oldQty < 1)
                                    {
                                        item.Attributes[Tags.Quantity].Value = newQty.ToString();
                                        item.Attributes[Tags.Value].Value = (newQty * unitPrice).ToString();
                                    }
                                    // ENd here .....
                                    else
                                    {
                                        item.Attributes[Tags.Quantity].Value = ((curQty / oldQty) * newQty).ToString();
                                        item.Attributes[Tags.Value].Value = (((curQty / oldQty) * newQty) * unitPrice).ToString();
                                    }

                                        
								}
							}
						}
					}
				}
				newRelated.ParentNode.ReplaceChild(oldRelated, newRelated);
			}
		}

		/// <summary>
		/// This function replace an item with a new item in the xml
		/// document. It will take care of ensuring that related items
		/// are copied across to the new node.
		/// </summary>
		/// <param name="oldItem"></param>
		/// <param name="newItem"></param>
		public void replaceItem(XmlNode oldItem, XmlNode newItem)
		{
			Function = "replaceItem";

			replaceRelatedItems(oldItem, newItem);
			oldItem.ParentNode.ReplaceChild(newItem, oldItem);
		}

		public bool findAffinities(XmlNode relatedItems)
		{
			string xPath = "//Item[@Quantity!='0' and @Type='Affinity' and @Code!='DT' and @Code!='STAX' and @ReadyAssist = 'False']";  //#18604 - CR15594
			return relatedItems.SelectNodes(xPath).Count > 0;

			/*
			bool found = false;
			foreach(XmlNode item in relatedItems.ChildNodes)
			{
				if(item.NodeType==XmlNodeType.Element&&item.Name==Elements.Item)
				{
					if(item.Attributes[Tags.Type].Value==IT.Affinity)
					{
						found = true;
						break;
					}					
				}
			}
			return found;
			*/
		}

		public bool findNonAffinities(XmlNode relatedItems)
		{
			string xPath = "//Item[@Quantity!='0' and @Type!='Affinity' and @Code!='DT' and @Code!='STAX']";
			return relatedItems.SelectNodes(xPath).Count > 0;

			/*
			bool found = false;
			foreach(XmlNode item in relatedItems.ChildNodes)
			{
				if(item.NodeType==XmlNodeType.Element&&item.Name==Elements.Item)
				{
					if(item.Attributes[Tags.Type].Value!=IT.Affinity)
					{
						found = true;
						break;
					}					
				}
			}
			return found;
			*/
		}

		public XmlNode CreateDropDownNode(XmlDocument doc, string name, string[] parms)
		{
			XmlNode node = doc.CreateElement(Elements.DropDown);
			node.Attributes.Append(doc.CreateAttribute(Tags.Name));
			node.Attributes[Tags.Name].Value = name;
			if(parms != null)
			{
				XmlNode parmList = doc.CreateElement(Elements.DropDownParmList);
				node.AppendChild(parmList);
				for(int i=0; i<parms.Length; i++)
				{
					XmlNode parmNode = doc.CreateElement(Elements.DropDownParm);
					parmNode.Attributes.Append(doc.CreateAttribute(Tags.Value));
					parmNode.Attributes[Tags.Value].Value = parms[i];
					parmList.AppendChild(parmNode);
				}
			}
			return node;
		}

		/// <summary>
		/// This function will calculate the price of a kit based on the 
		/// components linked to it and then update the kit node with 
		/// that value. This is to ensure that the kit line item 
		/// always reflects the price of the components even if the 
		/// user removes one or more items.
		/// </summary>
		/// <param name="kitNode"></param>
		public void RecalculateKitPrice(XmlNode kitNode, bool excludeWarranties)
		{
			if(kitNode.Attributes[Tags.Quantity].Value != "0")
			{
				decimal orderValue = 0;

				/* just in case there are multiple levels of related 
				 * items we'll do and xPath select to get them all into 
				 * a single node list to save us having to recurse */
				string xPath = "RelatedItems//Item[@Quantity != '0']";
				XmlNodeList components = kitNode.SelectNodes(xPath);
				foreach (XmlNode c in components)
				{
					if(c.Attributes[Tags.Type].Value == IT.Component ||
                        (c.Attributes[Tags.Type].Value == IT.KitWarranty && !excludeWarranties))
					{
						decimal p = Convert.ToDecimal(c.Attributes[Tags.UnitPrice].Value);
						decimal q = Convert.ToDecimal(c.Attributes[Tags.Quantity].Value);

						orderValue += p * q;
					}
				}
				kitNode.Attributes[Tags.Value].Value = orderValue.ToString();
				kitNode.Attributes[Tags.UnitPrice].Value = (orderValue / Convert.ToDecimal(kitNode.Attributes[Tags.Quantity].Value)).ToString();
			}
		}

		public XmlUtilities()
		{
		}
	}
}
