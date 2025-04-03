-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
CREATE TABLE Service.Resolution
(
	id				TINYINT IDENTITY(1, 1)	NOT NULL,
	[Description]	VARCHAR(128)			NOT NULL,
	Fail			BIT						NOT NULL
)  ON [PRIMARY]

ALTER TABLE Service.Resolution ADD CONSTRAINT PK_ServiceResolution PRIMARY KEY CLUSTERED 
(
	id
) WITH
( 
	STATISTICS_NORECOMPUTE = OFF, 
	IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY]
