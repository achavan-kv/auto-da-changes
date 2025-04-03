
/****** Object:  StoredProcedure [dbo].[AccountHasBDWSP]    Script Date: 11/20/2007 16:28:24 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[AccountHasBDWSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[AccountHasBDWSP]

GO


/****** Object:  StoredProcedure [dbo].[AccountHasBDWSP]    Script Date: 11/20/2007 16:28:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[AccountHasBDWSP]
	@acctno char(12),
	@HasBDW bit out,
	@return int out
as
	set nocount on
	set @return = 0
	
	if exists (select * from fintrans 
				where acctno = @acctno
					and transtypecode = 'bdw')
		begin
			set @HasBDW = 1
		end
	else
		begin
			set @HasBDW = 0
		end

	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

