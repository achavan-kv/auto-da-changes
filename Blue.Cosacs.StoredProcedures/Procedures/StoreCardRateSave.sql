
IF EXISTS (SELECT * FROM sysobjects WHERE NAME LIKE 'StoreCardRateSave')
DROP PROCEDURE StoreCardRateSave
GO 
CREATE PROCEDURE StoreCardRateSave 
@isdeleted BIT,
@name VARCHAR(50),
@id INT,
@idout INT OUT ,
@ratefixed BIT,
@isDefaultRate BIT
AS 

if @id < 0
BEGIN
	SET @idout = (SELECT MAX(id)+1 FROM dbo.StoreCardRate)
	SET @idout = ISNULL(@idout,1)
END
ELSE
BEGIN
	SET @idout = @id
END

	
	INSERT INTO dbo.StoreCardRate (
		Id,
		[$Version],
		[$IsDeleted],
		[NAME],
		ratefixed,
		isDefaultRate
	) VALUES
	( @idout,0,0,@name,@ratefixed,@isDefaultRate)
GO 