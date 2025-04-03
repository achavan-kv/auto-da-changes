SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetDeliveryScheduleprintDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetDeliveryScheduleprintDetailsSP]
GO
CREATE PROCEDURE 	dbo.DN_GetDeliveryScheduleprintDetailsSP
			@branchNo smallint,
			@dateDel datetime,
                        @loadNo smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT distinct 
		SC.acctno,
		SI.IUPC as itemno,
        -- UAT 219 --ISNULL(SC.retstocklocn, SC.stocklocn) as stocklocn,
   		CASE WHEN ISNULL(SC.retstocklocn,0) = 0 THEN SC.stocklocn ELSE SC.retstocklocn END as stocklocn,
		SC.datedelplan,
		SC.loadno,
		SC.buffbranchno,
		SC.buffno,
        SC.origbr,
		SC.agrmtno,
        SC.delorcoll,
        SC.quantity,
		SC.retstocklocn,
		SC.retitemno,
		SC.retval,
	    SC.vanno,
		SC.dateprinted,
		SC.printedby,
		SC.picklistnumber,
		SC.undeliveredflag,
        TS.printed,
		isnull(CT.telno, '') as Homephone, --IP - 22/07/08 - CR951
		isnull(CM.telno, '') as Cellphone, --IP - 22/07/08 - CR951
		SC.ItemID
	  FROM  schedule SC
			INNER JOIN dbo.StockInfo SI ON SC.ItemID = SI.ID
			INNER JOIN deliveryload DL 
				ON SC.loadno = DL.loadno
			INNER JOIN Transptsched TS
				ON TS.loadno = DL.loadno
					AND TS.datedel = DL.datedel
					AND   TS.branchNo = DL.branchNo
			INNER JOIN custacct CA --IP - 22/07/08 - CR951
				ON SC.acctno = CA.acctno  --IP - 22/07/08 - CR951
				AND CA.hldorjnt = 'H' --IP - 10/09/09 - UAT5.1(705) merged into 5.2 - was returning duplicate rows if there was a joint holder with telephone numbers.
			LEFT JOIN custtel CT  --IP - 22/07/08 - CR951
				ON CA.custid = CT.custid  --IP - 22/07/08 - CR951
					AND CT.tellocn = 'H' and CT.datediscon is NULL --IP - 22/07/08 - CR951
			LEFT JOIN custtel CM --IP - 22/07/08 - CR951
				ON CA.custid = CM.custid --IP - 22/07/08 - CR951
					AND CM.tellocn = 'M' and CM.datediscon is NULL --IP - 22/07/08 - CR951
	  WHERE DL.loadno = @loadno
	  AND   DL.datedel =  @dateDel
      AND   DL.branchNo = @branchNo
      
      --AND   SC.datedelplan = DL.datedel
	  AND   SC.datedelplan >= DL.datedel --IP - 07/05/08 - UAT(353) V5.1
	   --AND	SC.stocklocn = DL.branchno	-- RD 15/11/05 Added to ensure that only data for select branch are loaded
       -- DSR BranchNo is the originating branch and not the Stock Locn, so ...
       -- This is the stock locn
       AND  DL.BuffBranchNo = (CASE WHEN ISNULL(SC.retstocklocn,0) = 0 THEN SC.stocklocn ELSE SC.retstocklocn END)
       AND  DL.BuffNo = SC.BuffNo        -- The Buff No is unique for the stock locn
       
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

