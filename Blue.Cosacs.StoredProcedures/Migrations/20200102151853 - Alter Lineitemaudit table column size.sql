-- =============================================
-- Author:		Rahul D, Zensar
-- Create date: 27/11/2019
-- Description:	Change column size for itemno of LineItemAudit table
-- This is a part of solution provided for queue 200 Cint(poisons) for repossesed items poisons
-- =============================================


ALTER TABLE [dbo].[LineitemAudit] ALTER COLUMN [itemno] VARCHAR (18)
