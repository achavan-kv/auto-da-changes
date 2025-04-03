
IF EXISTS (SELECT * FROM sysobjects  WHERE NAME ='CM_ApplyZonesSP')
DROP PROCEDURE CM_ApplyZonesSP
GO

CREATE PROCEDURE [dbo].[CM_ApplyZonesSP]
 -- ************************************************************************************
 --Version: 002

-- Title:
-- Developer: Alex Ayscough
-- Date: 2007
-- Purpose: This updates zones on customer addresses

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/08/10  jec UAT45 @statement variable too small
-- 08/08/19  Zensar(SH) Strategy Job Optimization : Optimised the stored procedure for performance by putting Nolock and replacing * with 1 in all exist
-- **************************************************************************************
	@Zone VARCHAR(4), @return INT OUT
AS 

	DECLARE @statement NVARCHAR(max) ,@column_name VARCHAR(32), @or_clause VARCHAR(2),@query VARCHAR(128)
	,@Prevcolumn_name VARCHAR(4), @Prevor_clause VARCHAR(2),@Prevquery VARCHAR(128), @counter INT , @prevzone VARCHAR(4)
	,@totalnumber INT ,@notlike BIT 
	SET @Prevcolumn_name = ''
	SET @Prevor_clause =''
	SET @Prevquery = '' 
	SET @return = 0
	SET @counter = 0
	SET @Prevor_clause = ''








		SELECT @totalnumber = COUNT(*) FROM cmzoneallocation WITH(NOLOCK)


		-- if a zone has been deleted need to remove this from Cosacs.....
		UPDATE custaddress SET zone = '' WHERE  
		NOT EXISTS (SELECT 1 FROM CMZone Z WITH(NOLOCK) WHERE z.Zone = custaddress.zone)--Zensar(SH)
		AND ISNULL(zone,'')  != ''


		DECLARE @not VARCHAR(32)

		DECLARE zone_cursor CURSOR FAST_FORWARD READ_ONLY FOR
		
		SELECT Zone,column_name, REPLACE(query,'''','%'),ISNULL(or_clause,''),notlike 
		FROM cmzoneallocation WITH(NOLOCK)--Zensar(SH)
		WHERE zone = @zone OR @zone = 'All'
		order by zone ,or_clause
		
		OPEN zone_cursor
		
		FETCH NEXT FROM zone_cursor INTO @zone,@column_name,@query,@or_clause,@notlike 

		WHILE @@FETCH_STATUS = 0
		BEGIN
			--SET @s
	
			SET @query = REPLACE(@query,' ','%')  -- remove unnecessary spaces
			SET @query = REPLACE(@query,'.','%')  -- St. is always causing a problem
	

			IF @zone != @prevzone 
			BEGIN
				IF @prevor_clause !=''
					SET @statement = @statement + ' )'
		
					--IP - 11/06/09 - Credit Collection Walkthrough Changes - check for isnull	
					SET @statement = @statement + ' and isnull(zone, '''') !=' + '''' + @prevZone + '''' 
					PRINT @statement 
					EXEC sp_executesql @statement
					SET @return = @@error
					SET @counter = 0
					SET @statement =''
					SET @prevor_clause =''
					SET @or_clause = ''
			END
	
			IF @or_clause != @prevor_clause AND @prevor_clause !=''
				SET @statement = @statement + ' )'
			IF @or_clause !='' AND @or_clause != @Prevor_clause
				SET @statement = @statement + ' AND ('
	
			IF @counter = 0  -- new clause so 
			BEGIN
				SET @statement = ' UPDATE custaddress SET zone = ' + '''' +  @zone + '''' + ' WHERE 1=1 AND ' 
				

				IF @or_clause !='' 
					SET @statement = @statement + ' ('
	
			END

			ELSE 
			BEGIN
				IF @or_clause = @Prevor_clause AND @or_clause !='' 
				BEGIN
					IF @notlike !=1
						SET @statement = @statement + ' OR  ' 	
					ELSE 
						SET @statement = @statement + ' and  '
				END

		  
				ELSE 
					IF RIGHT(@statement,1) !='('
						SET @statement  = @statement + ' AND '	
		
			END
			IF @notlike = 1
				SET @not = ' not '
			ELSE
				SET @not = ''
			SET @statement = @statement + @column_name + @not + ' LIKE ' + '''' + '%' +  @query + '%' + ''''

			--IF @or_clause !=@Prevor_clause AND @counter >0 AND or_clause !='' -- close the brackets
			--	SET @statement = @statement + ' ) '
	
			SET @counter = @counter + 1 

			SET @Prevor_clause = @or_clause
			SET @prevzone = @zone
			SET @Prevquery =@query	
	
			FETCH NEXT FROM zone_cursor INTO @zone,@column_name,@query,@or_clause,@notlike

		END
		

		IF @or_clause !=''
			SET @statement = @statement + ' ) '
		CLOSE zone_cursor
		DEALLOCATE zone_cursor



		IF @statement !=''
		BEGIN
			SET @statement = @statement + ' and isnull(zone,'''') !=' + '''' + @Zone + ''''
			PRINT @statement  
			EXEC sp_executesql @statement
		END 
GO 



