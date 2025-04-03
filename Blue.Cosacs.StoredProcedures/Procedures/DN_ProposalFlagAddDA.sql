SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalFlagAddDA]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalFlagAddDA]
GO


/****** Object:  StoredProcedure [dbo].[DN_ProposalFlagAddDA]    Script Date: 11/05/2007 11:43:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE  procedure [dbo].[DN_ProposalFlagAddDA] 
			@return integer output,
			@DateProp smallDateTime,
			@custid varchar (20),
			@empeenopflg integer,
			@acctno char(12)
as
	set @return = 0

	insert 
	into 	proposalflag
		(origbr, custid, dateprop, checktype, datecleared, empeenopflg, acctno)
	select 	0,@custid, @DateProp, code,  null,@empeenopflg, @acctno
	from 	code 
	where 	category =N'PH2'
	and 	statusflag = 'L'

