REM Regenerates the 
REM "%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\TF.exe" checkout $/CoSACS_6.4/CoSACS_CURRENT/COURTS.NET/Blue.Cosacs.Messages/Warehouse.cs

"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Warehouse.xsd /c /namespace:Blue.Cosacs.Messages.Warehouse
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\WarehouseDeliver.xsd /c /namespace:Blue.Cosacs.Messages.Warehouse
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\BookingSubmit.xsd /c /namespace:Blue.Cosacs.Messages.Warehouse

REM "%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\TF.exe" checkout $/CoSACS_6.4/CoSACS_CURRENT/COURTS.NET/Blue.Cosacs.Messages/Service.cs
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Service.xsd /c /namespace:Blue.Cosacs.Messages.Service
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Warranty.xsd /c /namespace:Blue.Cosacs.Messages.Warranty

"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\CustomerPhoneNumbers.xsd /c /namespace:Blue.Cosacs.Messages.CustomerPhoneNumbers


mkdir Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\Vendors.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.Vendors /o:Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\VendorPurchaseOrder.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.PurchaseOrder /o:Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\GoodsReceiptEmail.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.GoodsReceiptEmail /o:Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\Cints.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.Cints /o:Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\Cint.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.Cint /o:Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\Products.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.Products /o:Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\PurchaseOrder.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.PurchaseOrder /o:Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\StockAdjustmentMessage.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.StockAdjustment /o:Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\VendorReturnMessage.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.VendorReturn /o:Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\GoodsReceiptMessage.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.GoodsReceipt /o:Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\CintOrderReceiptMessage.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.CintOrderReceipt /o:Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\TransferMessage.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.Transfer /o:Merchandising
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" ..\Schemas\Merchandising\BookingMessage.xsd /c /namespace:Blue.Cosacs.Messages.Merchandising.BookingMessage /o:Merchandising