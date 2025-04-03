SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DN_CodeUpdateSP]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DN_CodeUpdateSP]
GO
 
CREATE PROCEDURE [dbo].[DN_CodeUpdateSP]
-- ================================================  
-- Project      : CoSACS .NET  
-- File Name    : DN_CodeUpdateSP.prc  
-- File Type    : MSSQL Server Stored Procedure Script  
-- Title        : Code Table Update  
-- Date         : ??  
--  
-- This procedure will update the code table.  
--  
-- Change Control  
-- --------------  
-- Date      By  Description  
-- ----      --  -----------  
-- 27/06/11 jec  Increase Code to varchar(18) to cater for IUPC in TXR category
-- 07/12/11 ip   CR1234 - Save Additional2 column
-- ================================================  
 -- Add the parameters for the stored procedure here  
					 @category VARCHAR(12), 
				     @code VARCHAR(18),
				     @oldcategory CHAR(3), 
				     @oldcode VARCHAR(18),
				     @codedescript VARCHAR(64),
				     @sortorder SMALLINT,
				     @reference VARCHAR(12),
					 @additional VARCHAR(15),
					 @additional2 VARCHAR(15),					--IP - 07/12/11 - CR1234
					 @IsMmiApplicable BIT,
				     @return int OUTPUT			     
AS
DECLARE @count INTEGER
DECLARE @CodeConfCount INTEGER
SELECT @return = 0

IF @category IN ('WDP','WDL','WDC')
BEGIN
UPDATE [dbo].[code]
		SET [origbr] = 0, 
		    [category] = @category, 
		    [code]= @code, 
		    [codedescript] = @codedescript,
		    [statusflag] = N'L', -- obsolete column so default to live
		    [sortorder]= @sortorder,
		    [reference]= @reference,
		    additional = @additional,
		    additional2= @additional2							--IP - 07/12/11 - CR1234
		WHERE [category] = @category
		AND   [code]= @code
		AND codedescript = @codedescript
END
ELSE
BEGIN
		UPDATE [dbo].[code]
		SET [origbr] = 0, 
		    [category] = @category, 
		    [code]= @code, 
		    [codedescript] = @codedescript,
		    [statusflag] = N'L', -- obsolete column so default to live
		    [sortorder]= @sortorder,
		    [reference]= @reference,
		    additional= @additional,
		    additional2=@additional2
		WHERE [category] = @category							--IP - 07/12/11 - CR1234
		AND   [code]= @code
END
		SELECT @return = @@error,@count =@@rowcount

IF @return = 0 and @count = 0
BEGIN
	INSERT INTO [dbo].[code]([origbr], [category], [code], [codedescript], [statusflag], [sortorder], [reference],additional, additional2)	--IP - 07/12/11 - CR1234
	VALUES(0, @category, @code, @codedescript, N'L', @sortorder, @reference,@additional, @additional2)

	SELECT @return = @@error
END


	IF @category IN ('CC1','CC2') AND @return = 0 
	BEGIN

		UPDATE	[dbo].[CodeConfiguration]
		SET		[IsMmiApplicable] = @IsMmiApplicable
		WHERE	[category] = @category							
		AND		[code]= @code

		SELECT @CodeConfCount = @@rowcount

		IF @return = 0 and @CodeConfCount = 0
		BEGIN
			INSERT INTO [dbo].[CodeConfiguration]([Category], [Code], [IsMmiApplicable])	--IP - 07/12/11 - CR1234
			VALUES(@category, @code, @IsMmiApplicable)

			SELECT @return = @@error

		END
END



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End