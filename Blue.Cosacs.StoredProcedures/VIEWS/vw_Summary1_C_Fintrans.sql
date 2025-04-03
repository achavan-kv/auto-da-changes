/****** Object:  View [dbo].[vw_Summary1_C_Fintrans]    Script Date: 01/30/2012 17:14:37 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_Summary1_C_Fintrans]'))
DROP VIEW [dbo].[vw_Summary1_C_Fintrans]
GO
/****** Object:  View [dbo].[vw_Summary1_C_Fintrans]    Script Date: 01/30/2012 17:14:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create view [dbo].[vw_Summary1_C_Fintrans]
as
	Select f.acctno,f.datetrans,f.transtypecode,f.transvalue
		From fintrans f, temp_Summary1_MR s1
			Where f.acctno = s1.acctno
				AND f.transtypecode in ('SCT', 'RAG', 'GRT', 'INT', 'ADM', 'FEE', 'DEL', 'REP','ADD','RDL', 'DDF','BDW','RPO','CLD')					-- #10138 

GO
