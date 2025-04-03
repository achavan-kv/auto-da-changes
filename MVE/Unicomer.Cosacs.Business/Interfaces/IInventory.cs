using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model;
namespace Unicomer.Cosacs.Business.Interfaces
{
    public interface IInventory
    {
        dynamic PriceUpdate(PriceUpdate objJSON);
        dynamic CreateStockTransfer(StockTransferModel objStockTransfer);
        dynamic StockTransfer(string StkTrfNo);

    }
}
