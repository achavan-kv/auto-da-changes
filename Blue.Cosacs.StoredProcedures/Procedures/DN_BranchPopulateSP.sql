SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BranchPopulateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BranchPopulateSP]
GO

CREATE PROCEDURE 	dbo.DN_BranchPopulateSP
			@branch smallint,
			@branchname varchar(20) OUT,
			@address1 varchar(26) OUT,
			@address2 varchar(26) OUT,
			@address3 varchar(26) OUT,
			@postcode varchar(10) OUT,
			@telno varchar(13) OUT,
			@servicepcent float OUT,
			@countrycode char(2) OUT,
			@croffno int out,
			@daterun datetime out,
			@weekno int out,
			@oldpctype char(1) out,
			@newpctype char(1) out,
			@datepcchange datetime out,
			@batchcontrolno int out,
			@hissn int out,
			@hibuffno int out,
			@warehouseno varchar(3) out,
			@as400exp char(1) out,
			@hirefno int out, 
			@as400branchno smallint out,	
			@codreceipt int out,
			@region varchar(3) out,
			@servicelocation smallint out,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@branchname = branchname ,
			@address1 = branchaddr1,
			@address2  = branchaddr2,
			@address2 = branchaddr3,
			@postcode = branchpocode,
			@telno = telno,
			@servicepcent = servpcent,
			@countrycode = countrycode,
			@croffno = croffno,
			@daterun = daterun,
			@weekno = weekno,
			@oldpctype = oldpctype,
			@newpctype = newpctype,
			@datepcchange = datepcchange,
			@batchcontrolno = batchctrlno,
			@hissn = hissn,
			@hibuffno = hibuffno,
			@warehouseno = warehouseno,
			@as400exp = as400exp,
			@hirefno = hirefno, 
			@as400branchno = as400branchno,	
			@codreceipt = codreceipt,
			@region = region,
			@servicelocation = servicelocation
	FROM		branch
	WHERE	branchno = @branch 

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

