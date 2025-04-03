SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BranchGetAddressSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BranchGetAddressSP]
GO





CREATE PROCEDURE 	dbo.DN_BranchGetAddressSP
			@branch smallint,
			@updhissn smallint,
			@branchname varchar(20) OUT,
			@branchaddr1 varchar(26) OUT,
			@branchaddr2 varchar(26) OUT,
			@branchaddr3 varchar(26) OUT,
			@hissn int OUT,
			@buffno int OUT,
			-- 25/04/08 rdb added telno				--IP - 11/05/10 - UAT(135) UAT5.2.1.0 log - Merged from 4.3
			@telno VARCHAR(13) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@branchname	=	branchname,
			@branchaddr1	=	branchaddr1,
			@branchaddr2	=	branchaddr2,
			@branchaddr3	=	branchaddr3,
			@hissn		=	hissn,
			-- 25/04/08 rdb added telno			--IP - 11/05/10 - UAT(135) UAT5.2.1.0 log - Merged from 4.3
			@telno = telno

	FROM		branch
	WHERE	branchno = @branch

	IF(@updhissn = 1)
	BEGIN
		DECLARE @serialno int
	
		update 	branch 
		set 	hissn = hissn + 1
		where 	branchno = @branch

		IF (@hissn > 999998)
		BEGIN
			update 	branch 
			set 	hissn = 1
			where 	branchno = @branch
		END	
	END

	--increment the buffno for the tax invoice
	EXEC	DN_BranchGetBuffNoSP 	@branch = @branch, 
						@buffno = @buffno OUT,
						@return = @return OUT

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

