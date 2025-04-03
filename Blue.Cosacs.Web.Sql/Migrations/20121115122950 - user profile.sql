IF NOT EXISTS (SELECT * FROM admin.AdditionalProfile
		   WHERE Name = 'Technician')
BEGIN
	INSERT INTO Admin.AdditionalProfile
			( Name, Module, Permission )
	VALUES  ( 'Technician', -- Name - varchar(20)
			  'service/technicianProfile', -- Module - varchar(50)
			   1601  -- Permission - int
			  )
END