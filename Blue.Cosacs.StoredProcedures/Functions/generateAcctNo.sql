
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[generateAcctNo]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[generateAcctNo]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


		CREATE function generateAcctNo
			(@branchno VARCHAR(3), @accttype varchar(1), @id int)
		RETURNS varchar(12)
		AS
		BEGIN
			 DECLARE @acctno varchar(12) ='', @hialloc VARCHAR(9), @mod11 INT = 11, @modCalc INT = 0
			DECLARE @Calc TABLE
			( 
				POSITION smallint,
				weighting SMALLINT,
				Acctno SMALLINT
			)
			INSERT INTO @Calc
			        ( POSITION, weighting, Acctno )
			select 1, 9, 0 UNION 
			select 2, 8, 0 UNION 
			select 3, 4, 0 UNION 
			select 4, 10, 0 UNION 
			select 5, 1, 0 UNION 
			select 6, 6, 0 UNION 
			select 7, 3, 0 UNION 
			select 8, 5, 0 UNION 
			select 9, 7, 0 UNION 
			select 10, 2, 0 UNION 
			select 11, 1, 0 UNION 
			select 12, 0, 0 
			
			UPDATE @calc
			SET acctno = SUBSTRING(@branchno, 1, 1)
			WHERE POSITION = 1 
                
             UPDATE @calc
			SET acctno = SUBSTRING(@branchno, 2, 1)
			WHERE POSITION = 2    
			
			UPDATE @calc
			SET acctno = SUBSTRING(@branchno, 3, 1)
			WHERE POSITION = 3 
			
			UPDATE @calc
			SET acctno = 
				CASE WHEN @accttype = 'C'
				THEN 4
				WHEN @accttype = 'S'
				THEN 5
				ELSE 0
				END
			WHERE POSITION = 4
			
			SELECT @hialloc = CONVERT(VARCHAR, hiallocated) + @id FROM acctnoctrl WHERE branchno = @branchno AND acctcat = @accttype
			
			UPDATE @calc
			SET acctno = SUBSTRING(@hialloc, LEN(@hialloc), 1)
			WHERE POSITION = 10
                
             UPDATE @calc
			SET acctno =  SUBSTRING(@hialloc, LEN(@hialloc)-1, 1)
			WHERE POSITION = 9   
			
			UPDATE @calc
			SET acctno =  SUBSTRING(@hialloc, LEN(@hialloc)-2, 1)
			WHERE POSITION = 8
			UPDATE @calc
			SET acctno =  SUBSTRING(@hialloc, LEN(@hialloc)-3, 1)
			WHERE POSITION = 7
                
             UPDATE @calc
			SET acctno =  SUBSTRING(@hialloc, LEN(@hialloc)-4, 1)
			WHERE POSITION = 6    
			
			UPDATE @calc
			SET acctno =  SUBSTRING(@hialloc, LEN(@hialloc)-5, 1)
			WHERE POSITION = 5
			
			DECLARE @modCount SMALLINT = 1
			WHILE @modCount < 13
			BEGIN
			
				SELECT @modCalc = @modcalc + weighting * acctno 
				FROM @calc
				WHERE POSITION = @modCount
				
				set @modCount = @modCount + 1
				
			END
			
			SET @modcalc = @mod11 - (@modcalc % @mod11)
			
			
			UPDATE @calc
			SET acctno = 
				CASE WHEN @modcalc > 9
				THEN 0
				ELSE @modcalc
				END
			WHERE POSITION = 11
			
			UPDATE @calc
			SET acctno = 
				CASE WHEN @Accttype = 'C'
				THEN 0
				ELSE 1
				END
			WHERE POSITION = 12
			
			DECLARE @counter SMALLINT = 1
			WHILE @counter < 13
			BEGIN
			
				select @acctno = @acctno + CONVERT(VARCHAR, acctno)
				FROM @calc
				WHERE POSITION = @counter
			
				SET @counter = @counter+1
			END
			
			IF EXISTS (SELECT * FROM acct WHERE acctno = @acctno)
					SET @acctno = dbo.generateAcctNo(@branchno, @accttype, @id +1)
						
			RETURN @acctno
			
			
			
			  
		END
		GO