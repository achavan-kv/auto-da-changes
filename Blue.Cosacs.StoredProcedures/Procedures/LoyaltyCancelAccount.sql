
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyCancelAccount'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyCancelAccount
END
GO


CREATE PROCEDURE [dbo].[LoyaltyCancelAccount]
@acctno CHAR(12),
@user INT,
@return INT output
AS
BEGIN


	

	
	INSERT INTO cancellation (
		origbr,
		acctno,
		agrmtno,
		datecancel,
		empeenocanc,
		code,
		agrmttotal,
		notes
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* acctno - char(12) */ @acctno,
		/* agrmtno - int */ 1,
		/* datecancel - datetime */ GETDATE(),
		/* empeenocanc - int */ @user,
		/* code - varchar(4) */ 'L',
		/* agrmttotal - money */ 0,
		/* notes - varchar(300) */ '' ) 
		
		
		UPDATE lineitem
		SET quantity = 0,
			delqty =0,
			ordval = 0,
			price = 0,
			qtydiff = 'Y'
		WHERE acctno = @acctno
		
		UPDATE acct
		SET agrmttotal = 0,
			currstatus = 'S',
			outstbal = 0
		WHERE acctno = @acctno
		
		UPDATE agreement 
		SET agrmttotal = 0 
		WHERE acctno = @acctno
		
	

END
GO
