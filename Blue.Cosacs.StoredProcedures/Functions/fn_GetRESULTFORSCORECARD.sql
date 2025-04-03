
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetRESULTFORSCORECARD]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [dbo].[GetRESULTFORSCORECARD]
go


CREATE FUNCTION [dbo].[GetRESULTFORSCORECARD] 
(
@Operand NVARCHAR(100)='',
@value INT = 0
)
RETURNS DECIMAL
AS
BEGIN
-- SELECT DBO.GetRESULTFORSCORECARD('TIMECURRENTADDRESS_LN',457,'>=',50)
DECLARE @RESULT DECIMAL

DECLARE @opr1 nvarchar(5) = ''
DECLARE @opr2 nvarchar(5) = ''

IF(@value = 0)
BEGIN
  SET  @RESULT = 0
END

ELSE
BEGIN

 if EXISTS( (select  O2Operand from  tempTT where operand = @Operand and Operator in('<','>')) )  -- 18
   begin
  SET @opr1 = ( select TOP(1)  O2Operand from  tempTT where operand = @Operand and Operator = '<')  
  SET @opr2 = ( select  TOP(1) O2Operand from  tempTT where operand = @Operand and Operator = '>')  

  -- @opr1= 18  @opr2 =75  VALUE = 90

    if(@opr1 > @value ) --(18<90)
		begin
		 SELECT  TOP(1)  @RESULT = ISNULL(Result,0.00) FROM tempTT WHERE Operand = @Operand AND O2Operand = @opr1   --return @opr
		  
		end
		else IF (@opr2 < @value )
		begin
		  SELECT  TOP(1) @RESULT = ISNULL(Result,0.00) FROM tempTT WHERE Operand = @Operand AND O2Operand = @opr2   --return @opr
		end
		ELSE
		BEGIN
		 SET  @RESULT = @value
		END

   end

-- *****************************************************************
 if EXISTS( select  O2Operand from  tempTT where operand = @Operand and Operator in ('<=','>=') )  -- 18
   begin
  SET @opr1 = ( select  TOP(1) O2Operand from  tempTT where operand = @Operand and Operator = '<=')  
  SET @opr2 = ( select  TOP(1)  O2Operand from  tempTT where operand = @Operand and Operator = '>=')  

  -- @opr1= 18  @opr2 =75  VALUE = 90

    if(@opr1 >= @value ) --(18<90)
		begin
		 SELECT  TOP(1) @RESULT = ISNULL(Result,0.00) FROM tempTT WHERE Operand = @Operand AND O2Operand = @opr1   --return @opr
		  
		end
		else IF (@opr2 <= @value )
		begin
		  SELECT  TOP(1) @RESULT = ISNULL(Result,0.00) FROM tempTT WHERE Operand = @Operand AND O2Operand = @opr2   --return @opr
		end
		ELSE
		BEGIN
		 SET  @RESULT = @value
		END

   end
END
-- *****************************************************************

RETURN isnull(@RESULT,0)

END



--   SELECT [dbo].[GetRESULTFORSCORECARD] ('AGE',50)

--select DBO.GetRESULTFORSCORECARD('OLDEST_CREDIT_LN',55)  
--Ans should be 51

--select * from tempTT where Operand ='OLDEST_CREDIT_LN'
GO


