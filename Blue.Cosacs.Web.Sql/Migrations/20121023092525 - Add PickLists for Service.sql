-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #11486

IF NOT EXISTS(SELECT * FROM Config.picklist WHERE id = 'ContactType')
BEGIN
	
	INSERT INTO Config.PickList ( Id, Name )
	VALUES  ('ContactType', 'Contact Type' )
	
	INSERT INTO Config.PickRow( ListId, [Order], String )
	VALUES ('ContactType', 1, 'Email'),
		   ('ContactType', 2, 'Fax'),
		   ('ContactType', 3, 'HomePhone'),
		   ('ContactType', 4, 'MobilePhone'),
		   ('ContactType', 5, 'WorkPhone'),
		   ('ContactType', 6, 'Facebook'),
		   ('ContactType', 7, 'Twitter')	

END

IF NOT EXISTS(SELECT * FROM Config.picklist WHERE id = 'ServiceLocation')
BEGIN

	CREATE TABLE #ServiceLocation
	(
		 NAME VARCHAR(200),
		 Id int IDENTITY (1, 1) NOT NULL
	)

	INSERT INTO #ServiceLocation
	SELECT c.codedescript
	FROM code c WHERE category = 'SRSERVLCN'
	AND c.statusflag = 'L'


	INSERT INTO Config.PickList ( Id, Name )
	VALUES  ('ServiceLocation', 'Service Location' )
	
	INSERT INTO Config.PickRow( ListId, [Order], String )
	SELECT 'ServiceLocation', #ServiceLocation.Id, #ServiceLocation.NAME
	FROM #ServiceLocation 

END



IF NOT EXISTS(SELECT * FROM Config.picklist WHERE id = 'ServiceAction')
BEGIN

	CREATE TABLE #ServiceAction
	(
		 NAME VARCHAR(200),
		 Id int IDENTITY (1, 1) NOT NULL
	)
	
	INSERT INTO #ServiceAction
	SELECT c.codedescript
	FROM code c WHERE category = 'SRSERVACT'
	AND c.statusflag = 'L'

	INSERT INTO Config.PickList ( Id, Name )
	VALUES  ('ServiceAction', 'Service Action' )
	
	INSERT INTO Config.PickRow( ListId, [Order], String )
	SELECT 'ServiceAction', #ServiceAction.Id, #ServiceAction.NAME
	FROM #ServiceAction

END

IF NOT EXISTS(SELECT * FROM Config.picklist WHERE id = 'ServiceZone')
BEGIN
	
	CREATE TABLE #ServiceZone
	(
		 NAME VARCHAR(200),
		 Id int IDENTITY (1, 1) NOT NULL
	)
	
	INSERT INTO #ServiceZone
	SELECT c.codedescript
	FROM code c WHERE category = 'SRZONE'
	AND c.statusflag = 'L'

	INSERT INTO Config.PickList ( Id, Name )
	VALUES  ('ServiceZone', 'Service Technician Zone' )
	
	INSERT INTO Config.PickRow( ListId, [Order], String )
	SELECT 'ServiceZone', #ServiceZone.Id, #ServiceZone.NAME
	FROM #ServiceZone
    	
END

IF NOT EXISTS(SELECT * FROM Config.picklist WHERE id = 'ServiceResolution')
BEGIN

	CREATE TABLE #ServiceResolution
	(
		 NAME VARCHAR(200),
		 Id int IDENTITY (1, 1) NOT NULL
	)
	
	INSERT INTO #ServiceResolution
	SELECT c.codedescript
	FROM code c WHERE category = 'SRRESOLVE'
	AND c.statusflag = 'L'
	
	INSERT INTO Config.PickList ( Id, Name )
	VALUES  ('ServiceResolution', 'Service Resolution' )
	
	INSERT INTO Config.PickRow( ListId, [Order], String )
	SELECT 'ServiceResolution', #ServiceResolution.Id, #ServiceResolution.NAME
	FROM #ServiceResolution
				
END


IF NOT EXISTS(SELECT * FROM Config.picklist WHERE id = 'ServiceChargeTo')
BEGIN


	CREATE TABLE #ServiceChargeTo
	(
		 NAME VARCHAR(200),
		 Id int IDENTITY (1, 1) NOT NULL
	)
	
	INSERT INTO #ServiceChargeTo
	SELECT c.codedescript
	FROM code c WHERE category = 'SRCHARGE'
	AND c.statusflag = 'L'
	
	INSERT INTO Config.PickList ( Id, Name )
	VALUES  ('ServiceChargeTo', 'Service Charge To' )
	
	INSERT INTO Config.PickRow( ListId, [Order], String )
	SELECT 'ServiceChargeTo', #ServiceChargeTo.Id, #ServiceChargeTo.NAME
	FROM #ServiceChargeTo
		
END

IF NOT EXISTS(SELECT * FROM Config.picklist WHERE id = 'ServiceSupplier')
BEGIN

	CREATE TABLE #ServiceSupplier
	(
		 NAME VARCHAR(200),
		 Id int IDENTITY (1, 1) NOT NULL
	)
	
	INSERT INTO #ServiceSupplier
	SELECT c.codedescript
	FROM code c WHERE category = 'SRSUPPLIER'
	AND c.statusflag = 'L'
	
	INSERT INTO Config.PickList ( Id, Name )
	VALUES  ('ServiceSupplier', 'Service Supplier' )
	
	INSERT INTO Config.PickRow( ListId, [Order], String )
	SELECT 'ServiceSupplier', #ServiceSupplier.Id, #ServiceSupplier.NAME
	FROM #ServiceSupplier
    	
END

IF NOT EXISTS(SELECT * FROM Config.picklist WHERE id = 'ServiceTechReasons')
BEGIN

	
	CREATE TABLE #ServiceTechReasons
	(
		 NAME VARCHAR(200),
		 Id int IDENTITY (1, 1) NOT NULL
	)
	
	INSERT INTO #ServiceTechReasons
	SELECT c.codedescript
	FROM code c WHERE category = 'SRREASON'
	AND c.statusflag = 'L'
	
	INSERT INTO Config.PickList ( Id, Name )
	VALUES  ('ServiceTechReasons', 'Service Reassign Technician Reasons' )
	
	INSERT INTO Config.PickRow( ListId, [Order], String )
	SELECT 'ServiceTechReasons', #ServiceTechReasons.Id, #ServiceTechReasons.NAME
	FROM #ServiceTechReasons
    	
END


CREATE TABLE #ServiceDeliverers
	(
		 NAME VARCHAR(200),
		 Id int IDENTITY (1, 1) NOT NULL
	)

INSERT INTO #ServiceDeliverers
	SELECT c.codedescript
	FROM code c WHERE category = 'SRDELIVERER'
	AND c.statusflag = 'L'
	

IF NOT EXISTS(SELECT * FROM Config.picklist WHERE id = 'ServiceDeliverers')
BEGIN
	
	INSERT INTO Config.PickList ( Id, Name )
	VALUES  ('ServiceDeliverers', 'Service Deliverers' )
	
	INSERT INTO Config.PickRow( ListId, [Order], String )
	SELECT 'ServiceDeliverers', #ServiceDeliverers.Id, #ServiceDeliverers.NAME
	FROM #ServiceDeliverers
    	
END

--declare @text varchar(max)=''

--;with srdel as (select id from #ServiceDeliverers)

--select @text = @text + name + char(13)+char(10) from #ServiceDeliverers s inner join srdel d on d.id=s.id

--update config.Setting set ValueText=@text where id='ServiceDeliverers'

