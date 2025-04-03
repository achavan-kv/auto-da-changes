GO
/****** Object:  StoredProcedure [dbo].[DN_SelectBDWTransactions]    Script Date: 07/02/2008 12:14:36 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SelectBDWTransactions]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SelectBDWTransactions]
GO
/****** Object:  StoredProcedure [dbo].[DN_SelectBDWTransactions]    Script Date: 07/02/2008 12:14:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[DN_SelectBDWTransactions]
@acctno VARCHAR(12),
@return INT OUT 
as

SET @return  = 0

SELECT * FROM fintrans 
	WHERE acctno  = @acctno
		AND 
		datetrans <= 
		DATEADD(minute,2, (SELECT top 1 datetrans FROM fintrans 
		WHERE transtypecode = 'BDW' AND  acctno  =@acctno
		ORDER BY datetrans desc ))
		AND 
		datetrans >=
		DATEADD(minute,-2, (SELECT top 1 datetrans FROM fintrans 
		WHERE transtypecode = 'BDW' AND  acctno  =@acctno 
		ORDER BY datetrans desc ))