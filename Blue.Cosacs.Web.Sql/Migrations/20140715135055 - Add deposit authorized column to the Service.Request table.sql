
ALTER TABLE Service.Request
ADD DepositAuthorised BIT NOT NULL
CONSTRAINT Service_Request_DepositAuthorisedDefaultValue DEFAULT 0
