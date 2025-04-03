BEGIN TRAN

	
	INSERT INTO cron.endpoint (id,name,URl,module) VALUES (401,'Export stock CSV', '/cosacs/Merchandising/AshleySheduleJobs/ExportStockCSV','Ashley')
	INSERT INTO [Cron].[Job] VALUES (401,null,null,'',0)

	INSERT INTO cron.endpoint (id,name,URl,module) VALUES (402,'Create Sale', '/cosacs/Merchandising/AshleySheduleJobs/CreateCashSale','Ashley')
	INSERT INTO [Cron].[Job] VALUES (402,null,null,'',0)

	INSERT INTO cron.endpoint (id,name,URl,module) VALUES (403,'Create Auto PO', '/cosacs/Merchandising/AshleySheduleJobs/CreateAutoPO','Ashley')
	INSERT INTO [Cron].[Job] VALUES (403,null,null,'',0)

	INSERT INTO cron.endpoint (id,name,URl,module) VALUES (404,'Export PO XML to FTP', '/cosacs/Merchandising/AshleySheduleJobs/ExportPOXML','Ashley')
	INSERT INTO [Cron].[Job] VALUES (404,null,null,'',0)


	INSERT INTO cron.endpoint (id,name,URl,module) VALUES (405,'Read ASN', '/cosacs/Merchandising/AshleySheduleJobs/ReadAsnXML','Ashley')
	INSERT INTO [Cron].[Job] VALUES (405,null,null,'',0)


	INSERT INTO cron.endpoint (id,name,URl,module) VALUES (406,'Read Acknowledgement', '/cosacs/Merchandising/AshleySheduleJobs/ReadAckXML','Ashley')
	INSERT INTO [Cron].[Job] VALUES (406,null,null,'',0)

COMMIT
