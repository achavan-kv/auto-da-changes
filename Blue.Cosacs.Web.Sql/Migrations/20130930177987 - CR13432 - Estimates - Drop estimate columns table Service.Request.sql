-- transaction: true

ALTER TABLE [Service].[Request]
DROP COLUMN [EstimateReceived]

ALTER TABLE [Service].[Request]
DROP COLUMN [EstimateTransportCost]

ALTER TABLE [Service].[Request]
DROP COLUMN [EstimateLabourCost]

ALTER TABLE [Service].[Request]
DROP COLUMN [EstimateAdditionalLabourCost]
