
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[GenerateCashAndGoSRCustidSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GenerateCashAndGoSRCustidSP]
GO
-- ============================================================================================
-- Author:		Ilyas Parker
-- Create date: 18/11/2008
-- Description:	This procedure will generate a new Customer ID for a Cash & Go
--				Service Request and populate the Customer ID field
--				on the Service Request screen.		
-- ============================================================================================
CREATE PROCEDURE [dbo].[GenerateCashAndGoSRCustidSP] 
				@branch int,
				@NextSRCashAndGoCustid varchar(17)output,
				@return	int	output
as
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;

    set @return = 0    --initialise return code

	set @NextSRCashAndGoCustid = (select 'CASHANDGO' + cast(b.branchno as char(3)) + case
		when len(cast(b.SRCashAndGoCustid + 1 as varchar(8))) = 5 then cast (b.SRCashAndGoCustid + 1 as varchar(8))
		when len(cast(b.SRCashAndGoCustid + 1 as varchar(8))) = 4 then '0' + cast (b.SRCashAndGoCustid + 1 as varchar(8))
		when len(cast(b.SRCashAndGoCustid + 1 as varchar(8))) = 3 then '00' + cast (b.SRCashAndGoCustid + 1 as varchar(8))
		when len(cast(b.SRCashAndGoCustid + 1 as varchar(8))) = 2 then '000' + cast (b.SRCashAndGoCustid + 1 as varchar(8))
		when len(cast(b.SRCashAndGoCustid  + 1 as varchar(8))) = 1 then '0000' + cast (b.SRCashAndGoCustid + 1 as varchar(8))	
		else cast (b.SRCashAndGoCustid + 1 as varchar(8))
		end as 'SRCashAndGoCustid'
	from branch b
	where b.branchno = @branch)

	update branch 
	set SRCashAndGoCustid = SRCashAndGoCustid + 1
	where branchno = @branch
		
	
    if (@@error != 0)
    begin
        set @return = @@error
    end
end

GO


