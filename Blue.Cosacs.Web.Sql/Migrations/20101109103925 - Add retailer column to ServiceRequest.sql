-- Add new column for Retailer (required for non Courts SR)

alter TABLE dbo.SR_ServiceRequest add Retailer VARCHAR(30) 
