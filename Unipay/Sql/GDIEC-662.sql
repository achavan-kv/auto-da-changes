----------------------------25-Apr-2023----------------------------------------------
ALTER TABLE BSAPILOG ADD RequestTraceId NVARCHAR(500) Null;
GO
ALTER TABLE BSAPILOG ADD ResponseTraceId NVARCHAR(500) Null;
GO
ALTER TABLE BSAPILOG ADD RequestHeaders NVARCHAR(MAX) Null;
GO
ALTER TABLE BSAPILOG ADD ResponseHeaders NVARCHAR(MAX) Null;
GO

	
-----------------------------------------
-- Created By - Suresh
-- Date - 25-Apr-2023
-----------------------------------------

ALTER PROCEDURE [dbo].[SP_BS_InsertBSAPILog]
-- Add the parameters for the stored procedure here
@UniqueId varchar(50),
@CoSaCSAPIName varchar(200),
@MambuAPIName varchar(200),
@APIURL varchar(500),
@RequestJSON varchar(MAX),
@ResponseJSON varchar(MAX),
@IsSuccess bit,
@IsDBError bit,
@ErrorDetail varchar(MAX),
@RequestTraceId NVARCHAR(500),
@ResponseTraceId NVARCHAR(500),
@RequestHeaders NVARCHAR(MAX),
@ResponseHeaders NVARCHAR(MAX)
AS
BEGIN

	INSERT INTO [dbo].[BSAPILog]
	([UniqueId]
	,[CoSaCSAPIName]
	,[MambuAPIName]
	,[APIURL]
	,[RequestJSON]
	,[ResponseJSON]
	,[IsSuccess]
	,[IsDBError]
	,[ErrorDetail]
	,[RequestTraceId]
	,[ResponseTraceId]
	,[RequestHeaders]
	,[ResponseHeaders]
	)
	VALUES
	(@UniqueId,
	@CoSaCSAPIName,
	@MambuAPIName,
	@APIURL,
	@RequestJSON,
	@ResponseJSON,
	@IsSuccess,
	@IsDBError,
	@ErrorDetail,
	@RequestTraceId,
	@ResponseTraceId,
	@RequestHeaders,
	@ResponseHeaders
	)

END

--------------------------25-Apr-2023-----------------------------------------------------