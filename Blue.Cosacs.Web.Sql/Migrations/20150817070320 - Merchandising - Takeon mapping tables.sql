-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


create table Merchandising.VendorMapping
(
	Oracle Varchar(30) not null,
	CoSACS Varchar(40) not null
)


create table Merchandising.SkuMapping
(
	NewSku Varchar(10) not null,
	OldSku Varchar(10) not null
)
