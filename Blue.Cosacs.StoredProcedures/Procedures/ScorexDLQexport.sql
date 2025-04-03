-- Author; John Croft  
-- Date; October 2003

-- Version: 	1.0
/*
   This script will create the procedure to export the Delinquency table
   using BCP to ScorexDLQ.csv

*/

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[ScorexDLQexport]') 
            and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure ScorexDLQexport
Go

Create Procedure ScorexDLQexport

as 

/*
 set command string and execute BCP utility 
 export delinquency table to ScorexDLQ.csv

Note: the path for the bcp command may differ from "C:\MSSQL7\Binn\bcp" in some countries
Countries                              File location
=======                                =========
(1) Thailand                            c:\program files\microsoft sql server\80\tools\binn
(2) S'pore                              j:\mssql7\binn\bcp
(3) Fiji (courts & Homecentre)   	c:\program files\microsoft sql server\80\tools\binn
(4) Indonesia                           c:\program files\microsoft sql server\80\tools\binn
(5) Madagscar                        	c:\mssql7\binn\bcp
(6) PNG                                 d:\mssql7\binn\bcp  
(7) Mauritius                           c:\program files\microsoft sql server\80\tools\binn
(8) Caribbean? 				c:\program files\microsoft sql server\80\tools\binn
 
*/
DECLARE @BCPpath VARCHAR(500)

SELECT @BCPpath = value + '\Bcp' FROM CountryMaintenance
WHERE Codename = 'BCPpath' 

declare @path varchar(200)
--set @path = '"c:\program files\microsoft sql server\80\tools\binn\BCP" ' + db_name()+'..delinquency' + ' out ' +
-- 'd:\users\default\ScorexDLQ.csv ' + '-c -t, -q -T'		 UAT 254 7/12/07

set @path = '"' + @BCPpath + '" ' + db_name()+'..delinquency' + ' out ' +
 'd:\users\default\ScorexDLQ.csv ' + '-c -t, -q -T'

exec master.dbo.xp_cmdshell @path

go

-- End End End End End End End End End End End End End End End 