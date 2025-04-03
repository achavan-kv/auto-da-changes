-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

EXEC sp_rename 'interfacecontrol', 'InterfaceControl'
GO

EXEC sp_rename
    @objname = 'interfacecontrol.interface',
    @newname = 'Interface',
    @objtype = 'COLUMN'
GO

EXEC sp_rename
    @objname = 'interfacecontrol.runno',
    @newname = 'RunNo',
    @objtype = 'COLUMN'
GO

EXEC sp_rename
    @objname = 'interfacecontrol.datestart',
    @newname = 'DateStart',
    @objtype = 'COLUMN'
GO

EXEC sp_rename
    @objname = 'interfacecontrol.datefinish',
    @newname = 'DateFinish',
    @objtype = 'COLUMN'
GO 

EXEC sp_rename
    @objname = 'interfacecontrol.result',
    @newname = 'Result',
    @objtype = 'COLUMN'
GO   

EXEC sp_rename
    @objname = 'interfacecontrol.fileName',
    @newname = 'FileName',
    @objtype = 'COLUMN'
GO   