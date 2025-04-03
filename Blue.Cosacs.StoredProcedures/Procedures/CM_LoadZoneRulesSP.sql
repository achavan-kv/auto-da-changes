IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CM_LoadZoneRulesSP')
DROP PROCEDURE CM_LoadZoneRulesSP
GO 

CREATE PROCEDURE CM_LoadZoneRulesSP @return INTEGER OUT, @Zone VARCHAR (4)  
AS 
	SET @return = 0 
	if @zone = '' 
		set @zone = '%'
	SELECT Zone,
		   column_name,
		   Query,
		   or_clause,
		   notlike
    FROM CmZoneAllocation
	WHERE zone like @zone 	
    order by or_clause ASC
go
