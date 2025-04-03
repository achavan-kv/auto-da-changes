IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_ScheduleRemoval_AgrmtNo_StockLocn_BuffNo_ItemID' 
    AND object_id = OBJECT_ID('[dbo].[ScheduleRemoval]'))
  BEGIN
	CREATE INDEX [IX_ScheduleRemoval_AgrmtNo_StockLocn_BuffNo_ItemID] ON [dbo].[ScheduleRemoval] ([AgrmtNo], [StockLocn], [BuffNo], [ItemID])
END

IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_ScheduleRemoval_AgrmtNo_StockLocn_ItemID' 
    AND object_id = OBJECT_ID('[dbo].[ScheduleRemoval]'))
  BEGIN
	CREATE INDEX [IX_ScheduleRemoval_AgrmtNo_StockLocn_ItemID] ON [dbo].[ScheduleRemoval] ([AgrmtNo], [StockLocn], [ItemID])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_intratehistory_termstype_datefrom_Band' 
    AND object_id = OBJECT_ID('[dbo].[intratehistory]'))
  BEGIN
	CREATE INDEX [IX_intratehistory_termstype_datefrom_Band] ON [dbo].[intratehistory] ([termstype], [datefrom],[Band]) INCLUDE ([intrate], [inspcent], [InsIncluded], [intrate2])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_intratehistory_termstype_datefrom_dateto_Band' 
    AND object_id = OBJECT_ID('[dbo].[intratehistory]'))
  BEGIN
	CREATE INDEX [IX_intratehistory_termstype_datefrom_dateto_Band] ON [dbo].[intratehistory] ([termstype],[datefrom], [dateto], [Band]) INCLUDE ([inspcent], [InsIncluded])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_CashLoan_AcctNo_LoanStatus' 
    AND object_id = OBJECT_ID('[dbo].[CashLoan]'))
  BEGIN
	CREATE INDEX [IX_CashLoan_AcctNo_LoanStatus] ON [dbo].[CashLoan] ([AcctNo], [LoanStatus]) INCLUDE (ReferralReasons)
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_fintrans_new_income_branchno_empeeno' 
    AND object_id = OBJECT_ID('[dbo].[fintrans_new_income]'))
  BEGIN
	CREATE INDEX [IX_fintrans_new_income_branchno_empeeno] ON [dbo].[fintrans_new_income] ([branchno], [empeeno]) INCLUDE ([transvalue])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_ServiceChargeAcct_AcctNo' 
    AND object_id = OBJECT_ID('[dbo].[ServiceChargeAcct]'))
  BEGIN
	CREATE INDEX [IX_ServiceChargeAcct_AcctNo] ON [dbo].[ServiceChargeAcct] ([AcctNo]) INCLUDE ([ServiceRequestNo])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_intratehistory_Band' 
    AND object_id = OBJECT_ID('[dbo].[intratehistory]'))
  BEGIN
	CREATE INDEX [IX_intratehistory_Band] ON [dbo].[intratehistory] ([Band]) INCLUDE ([termstype], [datefrom], [dateto], [inspcent], [InsIncluded])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_LineItemBookingSchedule_LineItemID' 
    AND object_id = OBJECT_ID('[dbo].[LineItemBookingSchedule]'))
  BEGIN
	CREATE INDEX [IX_LineItemBookingSchedule_LineItemID] ON [dbo].[LineItemBookingSchedule] ([LineItemID]) INCLUDE ([Quantity])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_User_BranchNo' 
    AND object_id = OBJECT_ID('[Admin].[User]'))
  BEGIN
	CREATE INDEX [IX_User_BranchNo] ON [Admin].[User] ([BranchNo]) INCLUDE ([Id])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_code_code_category' 
    AND object_id = OBJECT_ID('[dbo].[code]'))
  BEGIN
	CREATE INDEX [IX_code_code_category] ON [dbo].[code] ([code],[category]) INCLUDE ([codedescript])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_instalplan_dateaudit_datechanged' 
    AND object_id = OBJECT_ID('[dbo].[instalplan_dateaudit]'))
  BEGIN
	CREATE INDEX [IX_instalplan_dateaudit_datechanged] ON [dbo].[instalplan_dateaudit] ([datechanged])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_WarrantyPotentialSale_CustomerAccount' 
    AND object_id = OBJECT_ID('[Warranty].[WarrantyPotentialSale]'))
  BEGIN
	CREATE INDEX [IX_WarrantyPotentialSale_CustomerAccount] ON [Warranty].[WarrantyPotentialSale] ([CustomerAccount])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_CustomerLocking_CustID_LockCount' 
    AND object_id = OBJECT_ID('[dbo].[CustomerLocking]'))
  BEGIN
	CREATE INDEX [IX_CustomerLocking_CustID_LockCount] ON [dbo].[CustomerLocking] ([CustID],[LockCount])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_proposalflag_checktype_Acctno_datecleared' 
    AND object_id = OBJECT_ID('[dbo].[proposalflag]'))
  BEGIN
	CREATE INDEX [IX_proposalflag_checktype_Acctno_datecleared] ON [dbo].[proposalflag] ([checktype], [Acctno],[datecleared])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_LineItemBookingSchedule_LineItemID_BookingId' 
    AND object_id = OBJECT_ID('[dbo].[LineItemBookingSchedule]'))
  BEGIN
	CREATE INDEX [IX_LineItemBookingSchedule_LineItemID_BookingId] ON [dbo].[LineItemBookingSchedule] ([LineItemID],[BookingId])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_proposalflag_datecleared_Acctno' 
    AND object_id = OBJECT_ID('[dbo].[proposalflag]'))
  BEGIN
	CREATE INDEX [IX_proposalflag_datecleared_Acctno] ON [dbo].[proposalflag] ([datecleared], [Acctno]) INCLUDE ([checktype])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_User_Id' 
    AND object_id = OBJECT_ID('[Admin].[User]'))
  BEGIN
	CREATE INDEX [IX_User_Id] ON [Admin].[User] ([Id]) INCLUDE ([BranchNo], [FullName])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_WarrantyPotentialSale_ItemId_WarrantyId_WarrantyType_CustomerAccount' 
    AND object_id = OBJECT_ID('[Warranty].[WarrantyPotentialSale]'))
  BEGIN
	CREATE INDEX [IX_WarrantyPotentialSale_ItemId_WarrantyId_WarrantyType_CustomerAccount] ON [Warranty].[WarrantyPotentialSale] ([ItemId], [WarrantyId], [WarrantyType], [CustomerAccount]) INCLUDE ([Id], [InvoiceNumber], [SaleBranch], [SoldOn], [SoldById], [CustomerId], [ItemNumber], [ItemPrice], [WarrantyNumber], [WarrantyLength], [WarrantyTaxRate], [WarrantyCostPrice], [WarrantyRetailPrice], [WarrantySalePrice], [ItemSerialNumber], [ItemCostPrice], [ItemDeliveredOn], [IsItemReturned], [SecondEffort], [AgreementNumber], [Quantity])
END
IF NOT EXISTS (SELECT Index_id  FROM sys.indexes  WHERE name='IX_storderprocess_runno_FileName_error' 
    AND object_id = OBJECT_ID('[dbo].[storderprocess]'))
  BEGIN
	CREATE INDEX [IX_storderprocess_runno_FileName_error] ON [dbo].[storderprocess] ([runno], [FileName],[error])
END

