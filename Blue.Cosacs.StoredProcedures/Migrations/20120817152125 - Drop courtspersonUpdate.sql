/****** Object:  Trigger [trig_courtsperson_update]    Script Date: 08/17/2012 16:21:49 ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_courtsperson_update]'))
DROP TRIGGER [dbo].[trig_courtsperson_update]
GO


