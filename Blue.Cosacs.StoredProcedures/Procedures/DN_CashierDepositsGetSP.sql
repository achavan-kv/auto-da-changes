SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierDepositsGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierDepositsGetSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierDepositsGetSP
-- ================================================  
-- Project      : CoSACS .NET  
-- File Name    : DN_CashierDepositsGetSP.prc  
-- File Type    : MSSQL Server Stored Procedure Script  
-- Title        : Retrieve Cashier Deposits
-- Date         : ?
--  
-- This procedure will retrieve Cashier Deposits 
--  
-- Change Control  
-- --------------  
-- Date      By  Description  
-- ----      --  -----------  
-- 15/12/11  IP  #8810 - CR1234 - Filter by Payment Method. (-1) represents 'ALL'
-- ================================================  
			@empeeno int,
			@postedtofact smallint,
            @datefrom datetime,
			@dateto datetime,
            @branchno smallint,	
			@deposittype varchar(4),
			@branchfloats bit,
			@paymentMethod int,						--IP - 15/12/11 - #8810
			@return int OUTPUT

--JJ 20/7/04 we need to be able to retrieve deposits which 
--have been posted to FACT or deposits which haven't OR BOTH
--we'll use @postedtofact = 2 to represent both
AS

	SET 	@return = 0			--initialise return code

 SELECT CD.depositid,  
   CD.datedeposit,  
   CD.code AS TransTypeCode,  
   ISNULL(T.description, CD.Code + ':Not Defined!') AS Description,  
   CD.paymethod AS Code,  
   ISNULL(C.codedescript, CD.paymethod + ':Not Defined!') AS CodeDescript,  
   CD.runno,  
   CD.empeeno,  
   CD.depositvalue,  
   CD.branchno,  
   CD.cashiertotalid,  
   CD.reference,  
   ISNULL(T.isdeposit,0) AS isdeposit, --IP - 05/05/09 - Livewire(71112)  
   CD.includeincashiertotals,  
   CD.IsReversed  
 FROM CashierDeposits CD  
    LEFT OUTER JOIN transtype T ON CD.code = T.transtypecode  
    LEFT OUTER JOIN code C ON CD.paymethod = C.code AND C.category = 'FPM'  
 
 WHERE (CD.empeeno = @empeeno or @empeeno =0)  
 AND CD.datedeposit BETWEEN @datefrom AND @dateto  
 AND ((@postedtofact = 0 and CD.runno <= 0) OR (@postedtofact = 1 and CD.runno > 0) OR @postedtofact = 2)  
 AND CD.datevoided is null  
 AND (CD.code = @deposittype OR @deposittype = '-1')  
 AND (CD.branchno = @branchno OR @branchno = 0)  
 AND ((@branchfloats = 0 and CD.runno >= 0) or @branchfloats = 1)  
 AND ((CD.paymethod = @paymentMethod) or @paymentMethod = -1)    --IP - 15/12/11 - #8810  
 AND ((@branchfloats = 0 AND CD.code != 'SAF') OR @branchfloats = 1) 
			

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

