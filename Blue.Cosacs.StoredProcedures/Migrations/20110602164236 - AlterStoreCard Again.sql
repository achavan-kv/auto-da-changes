DROP TABLE StoreCardActivation
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'FK_StoreCardStatus') AND parent_object_id = OBJECT_ID(N'StoreCardPaymentDetails'))
ALTER TABLE StoreCardPaymentDetails DROP CONSTRAINT FK_StoreCardStatus
GO


SELECT * 
INTO StoreCardStatus_Lookup
FROM StoreCardStatus

DROP TABLE StoreCardstatus
GO

ALTER TABLE StoreCard
 ADD ProofAddress varchar(200) NOT NULL DEFAULT ''
 
 
 ALTER TABLE StoreCard
 ADD ProofAddNotes varchar(5000) NULL
 
 ALTER TABLE StoreCard
 ADD ProofID varchar(200) NOT NULL DEFAULT ''
 
 ALTER TABLE StoreCard
 ADD ProofIDNotes varchar(5000) NULL
 
 ALTER TABLE StoreCard
 ADD SecurityQ varchar(200) NOT NULL DEFAULT ''
 
 ALTER TABLE StoreCard
 ADD SecurityA varchar(5000) NOT NULL DEFAULT ''
 
GO

CREATE TABLE StoreCardStatus
(
 CardNumber bigint NOT NULL,
 DateChanged datetime NOT NULL,
 StatusCode VARCHAR(20) NOT NULL
 CONSTRAINT PK_StoreCardStatus PRIMARY KEY NONCLUSTERED 
(
 CardNumber, DateChanged ASC
)
)

GO

