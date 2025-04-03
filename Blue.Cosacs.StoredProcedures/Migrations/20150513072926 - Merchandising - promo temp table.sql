-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'temp_rawpromoload')
	BEGIN
		DROP TABLE temp_rawpromoload
	END	
	
	CREATE TABLE temp_rawpromoload
	(
	    itemno          varchar(10)    default ''  not null,
	    warehouseno     varchar(4)     default ''  not null,
	    pricehp1        varchar(17)    default ''  not null,
	    datefromhp1     varchar(22)    default ''  not null,
	    datetohp1       varchar(22)    default ''  not null,
	    pricehp2        varchar(17)    default ''  not null,
	    datefromhp2     varchar(22)    default ''  not null,
	    datetohp2       varchar(22)    default ''  not null,
	    pricehp3        varchar(17)    default ''  not null,
	    datefromhp3     varchar(22)    default ''  not null,
	    datetohp3       varchar(22)    default ''  not null,
	    pricecash1      varchar(17)    default ''  not null,
	    datefromcash1   varchar(22)    default ''  not null,
	    datetocash1     varchar(22)    default ''  not null,
	    pricecash2      varchar(17)    default ''  not null,
	    datefromcash2   varchar(22)    default ''  not null,
	    datetocash2     varchar(22)    default ''  not null,
	    pricecash3      varchar(17)    default ''  not null,
	    datefromcash3   varchar(22)    default ''  not null,
	    datetocash3     varchar(22)    default ''  not null,
		PromotionId     varchar(20)     default ''  not null,
		branchno        smallint       default 0   not null
	
	)
