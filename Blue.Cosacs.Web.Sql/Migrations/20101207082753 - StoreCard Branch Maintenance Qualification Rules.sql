-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


--Create the table to hold Store Card Qualification Rules for each Branch

CREATE TABLE StoreCardBranchQualRules
(
	BranchNo smallint not null,
	MinApplicationScore int null,
	MinBehaviouralScore int null,
	MinMthsAcctHist int null,
	MaxCurrMthsInArrs int null,
	MaxPrevMthsInArrs int null,
	MinAvailRFLimit money null	
)
GO

--Add primary key
ALTER TABLE StoreCardBranchQualRules ADD CONSTRAINT [pk_StoreCardBranchQualRules] PRIMARY KEY CLUSTERED
(
	BranchNo ASC
)
GO

--Add foreign key 

ALTER TABLE StoreCardBranchQualRules WITH CHECK ADD CONSTRAINT [fk_StoreCardBranchQualRules] FOREIGN KEY([BranchNo])
REFERENCES [dbo].[branch] ([branchno])
GO

--Create an audit table for the Store Card Qualification Rules
CREATE TABLE StoreCardBranchQualRulesAudit
(
	BranchNo smallint not null,
	MinApplicationScore int null,
	MinBehaviouralScore int null,
	MinMthsAcctHist int null,
	MaxCurrMthsInArrs int null,
	MaxPrevMthsInArrs int null,
	MinAvailRFLimit money null,
	EmpeenoChange int null,
	DateChanged datetime not null
)
GO

--Add primary key
ALTER TABLE StoreCardBranchQualRulesAudit ADD CONSTRAINT [pk_StoreCardBranchQualRulesAudit] PRIMARY KEY CLUSTERED
(
	BranchNo ASC,
	DateChanged ASC
)
GO

--Add foreign key

ALTER TABLE StoreCardBranchQualRulesAudit WITH CHECK ADD CONSTRAINT [fk_StoreCardBranchQualRulesAudit] FOREIGN KEY([BranchNo])
REFERENCES [dbo].[StoreCardBranchQualRules] ([BranchNo])
GO

--Add new user right for enabling/disabling the Store Card Qualification Rules tab on the Branch screen

DECLARE @taskid INT 
SELECT @taskid =MAX(TaskID)+1 FROM task

IF NOT EXISTS (SELECT * FROM [Control] WHERE [Control]='lStoreCardQualRules' and screen = 'Branch')
BEGIN
    insert into control 
    (TaskID ,Screen, [Control], Visible ,[Enabled] , ParentMenu)  
    values
    (@taskid,'Branch','lStoreCardQualRules',0,1,'')

    insert into task
    (taskid,taskname)
    values
    (@taskid,'Branch - Store Card Qualification Rules')	
END
GO