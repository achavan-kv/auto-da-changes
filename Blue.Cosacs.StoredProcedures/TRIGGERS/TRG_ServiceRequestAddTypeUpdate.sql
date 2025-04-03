SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[Service].[TRG_ServiceRequestAddTypeUpdate]'))
DROP TRIGGER [Service].[TRG_ServiceRequestAddTypeUpdate]
GO 
CREATE TRIGGER [Service].[TRG_ServiceRequestAddTypeUpdate] 
ON [Service].[Request]
FOR INSERT AS
BEGIN
DECLARE @Serviceid CHAR(12) 
DECLARE @addtype CHAR(2)
DECLARE @CustomerId VARCHAR(50)

SELECT @Serviceid = id, @CustomerId = CustomerId, @addtype = ISNULL(addtype,'') 
   FROM inserted 
      UPDATE [Service].[Request] SET ItemDeliveredOn = getdate() WHERE id = @Serviceid and  ItemDeliveredOn is null
IF @addtype = '' 
  BEGIN
     SELECT TOP 1  @addtype = isnull(CA.addtype,'') 
	 FROM [dbo].[custaddress] CA, inserted SR  
     WHERE CA.custid = SR.CustomerId 
	 AND CA.cusaddr1 = SR.CustomerAddressLine1 
	 AND CA.cusaddr2 = SR.CustomerAddressLine2 
	 AND CA.cusaddr3 = SR.CustomerAddressLine3 
	 AND CA.cuspocode = SR.CustomerPostcode 
	 AND SR.addtype IS NULL 
	 AND CA.custid = @CustomerId 
	 ORDER BY SR.addtype 
	 
	 IF @addtype != '' 
		 BEGIN
		    UPDATE [Service].[Request] 
			SET addtype = @addtype, CreatedBy = (SELECT FULLNAME FROM [ADMIN].[USER] WHERE ID  = 99999), CreatedById = 99999,
			    LastUpdatedUser = 99999,LastUpdatedUserName = (SELECT FULLNAME FROM [ADMIN].[USER] WHERE ID  = 99999)
		    WHERE id = @Serviceid
		 END
  END 

END

GO
