-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO dbo.code
        ( origbr ,
          category ,
          code ,
          codedescript ,
          statusflag ,
          sortorder ,
          reference ,
          additional ,
          Additional2
        )
SELECT NULL , -- origbr - smallint
          'SOA' , -- category - varchar(12)
          'SING' , -- code - varchar(18)
          'SINGER MIGRATION' , -- codedescript - nvarchar(64)
          'L' , -- statusflag - char(1)
          0 , -- sortorder - smallint
          '0' , -- reference - varchar(12)
          NULL , -- additional - varchar(15)
          NULL  -- Additional2 - varchar(15)
        
        where not exists ( select 'x' from code where code = 'sing' and category = 'soa')