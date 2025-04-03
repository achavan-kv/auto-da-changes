
IF OBJECT_ID('[Warranty].[WarrantyPriceRepositoryBulkInsertTestScript]') IS NOT NULL
	DROP PROCEDURE [Warranty].[WarrantyPriceRepositoryBulkInsertTestScript]
GO

CREATE PROCEDURE [Warranty].[WarrantyPriceRepositoryBulkInsertTestScript]
    @Scenario INT = 2,
    @SampleYear varchar(4) = '2014'
AS

  DECLARE @BranchNo SMALLINT = (SELECT TOP 1 origbr FROM country)

  SET IDENTITY_INSERT Warranty.Warranty ON

-- Clean everything -- Clean everything -- Clean everything --
  DELETE FROM Warranty.WarrantyPrice WHERE Id >= 2100000000
  DELETE FROM Warranty.Warranty WHERE Id >= 2100000000
-- Clean everything -- Clean everything -- Clean everything --

  INSERT INTO Warranty.Warranty
    (        [Id],     [Number], [Description], [Length], [TaxRate], [TypeCode], [Deleted]) VALUES
      (2100010001, '2100010001',        'War1',       12,        10,      'E',         0),
      (2100010002, '2100010002',        'War2',       12,        10,      'E',         0)

  SET IDENTITY_INSERT Warranty.Warranty OFF

  SET IDENTITY_INSERT Warranty.WarrantyPrice ON

  IF (@Scenario=1)
  BEGIN
  INSERT INTO Warranty.WarrantyPrice 
    (          Id, WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice,                   EffectiveDate, CostPriceChange, CostPricePercentChange, RetailPriceChange, RetailPricePercentChange, TaxInclusivePriceChange, TaxInclusivePricePercentChange, BulkEditId) VALUES
      (2100020101, 2100010001,       null,         null,       200,         300, @SampleYear + '-01-22 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020102, 2100010001,       null,         null,       210,         310, @SampleYear + '-01-23 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020103, 2100010001,       null,         null,       220,         320, @SampleYear + '-01-30 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020104, 2100010002,       null,         null,       500,         600, @SampleYear + '-02-15 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null)
  END

  IF (@Scenario=2)
  BEGIN
  INSERT INTO Warranty.WarrantyPrice
    (          Id, WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice,                   EffectiveDate, CostPriceChange, CostPricePercentChange, RetailPriceChange, RetailPricePercentChange, TaxInclusivePriceChange, TaxInclusivePricePercentChange, BulkEditId) VALUES
      (2100020201, 2100010001,       null,         null,       220,         320, @SampleYear + '-01-19 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020202, 2100010001,       null,         null,       200,         300, @SampleYear + '-01-22 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020203, 2100010001,       null,         null,       210,         310, @SampleYear + '-01-23 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020204, 2100010002,       null,         null,       500,         300, @SampleYear + '-01-15 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null)
  END

  IF (@Scenario=3)
  BEGIN
  INSERT INTO Warranty.WarrantyPrice
    (          Id, WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice,                   EffectiveDate, CostPriceChange, CostPricePercentChange, RetailPriceChange, RetailPricePercentChange, TaxInclusivePriceChange, TaxInclusivePricePercentChange, BulkEditId) VALUES
      (2100020301, 2100010001,        'C',    @BranchNo,       200,         300, @SampleYear + '-05-01 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020302, 2100010001,        'C',         null,       210,         310, @SampleYear + '-05-15 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020303, 2100010001,        'C',         null,       220,         320, @SampleYear + '-07-30 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020304, 2100010002,        'C',         null,       500,         600, @SampleYear + '-05-15 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null)
  END

  IF (@Scenario=4)
  BEGIN
  INSERT INTO Warranty.WarrantyPrice
    (          Id, WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice,                   EffectiveDate, CostPriceChange, CostPricePercentChange, RetailPriceChange, RetailPricePercentChange, TaxInclusivePriceChange, TaxInclusivePricePercentChange, BulkEditId) VALUES
      (2100020401, 2100010001,        'C',    @BranchNo,       200,         300, @SampleYear + '-02-01 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020402, 2100010001,        'C',         null,       210,         310, @SampleYear + '-02-15 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020404, 2100010002,        'C',         null,       500,         600, @SampleYear + '-03-15 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null)
  END

  IF (@Scenario=5)
  BEGIN
  INSERT INTO Warranty.WarrantyPrice
    (          Id, WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice,                   EffectiveDate, CostPriceChange, CostPricePercentChange, RetailPriceChange, RetailPricePercentChange, TaxInclusivePriceChange, TaxInclusivePricePercentChange, BulkEditId) VALUES
      (2100020501, 2100010001,        'C',    @BranchNo,       200,         300, @SampleYear + '-01-01 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020502, 2100010001,        'C',         null,       210,         310, @SampleYear + '-01-15 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020504, 2100010002,        'C',         null,       500,         600, @SampleYear + '-02-15 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null)
  END

  -- Scenario 6
  -- The system should not allow the users to select Effective date as current date. It should always be a future date.

  IF (@Scenario=7)
  BEGIN
  INSERT INTO Warranty.WarrantyPrice
    (          Id, WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice,                   EffectiveDate, CostPriceChange, CostPricePercentChange, RetailPriceChange, RetailPricePercentChange, TaxInclusivePriceChange, TaxInclusivePricePercentChange, BulkEditId) VALUES
      (2100020701, 2100010001,        'C',         null,       200,         300, @SampleYear + '-02-01 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
--    (2100020702, 2100010001,        'C',         null,       210,         310, @SampleYear + '-02-01 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020703, 2100010001,        'C',         null,       220,         320, @SampleYear + '-02-30 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020704, 2100010002,        'C',         null,       500,         600, @SampleYear + '-04-15 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null)
  END

-- extra scenarios...

  IF (@Scenario=8)
  BEGIN
  INSERT INTO Warranty.WarrantyPrice
    (          Id, WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice,         EffectiveDate, CostPriceChange, CostPricePercentChange, RetailPriceChange, RetailPricePercentChange, TaxInclusivePriceChange, TaxInclusivePricePercentChange, BulkEditId) VALUES
      (2100020801, 2100010001,       null,         null,       105,         113, '2014-01-01 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020802, 2100010001,       null,    @BranchNo,       125,         135, '2014-01-01 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020803, 2100010001,       null,         null,      null,        null, '2014-01-02 00:00:00',            null,                   null,                10,                     null,                    null,                           null,          1),
      (2100020807, 2100010001,       null,    @BranchNo,      null,        null, '2014-01-02 00:00:00',            null,                   null,                10,                     null,                    null,                           null,          1),
      (2100020808, 2100010001,       null,         null,      null,        null, '2014-01-03 00:00:00',            null,                   null,              null,                        2,                    null,                           null,          4),
      (2100020809, 2100010001,       null,         null,      null,        null, '2014-01-05 00:00:00',            null,                   null,              null,                     null,                    null,                              3,          5),
      (2100020810, 2100010001,       null,    @BranchNo,      null,        null, '2014-01-05 00:00:00',            null,                   null,              null,                     null,                    null,                              3,          5),
      (2100020811, 2100010001,       null,         null,      null,        null, '2014-01-09 00:00:00',            null,                      3,              null,                     null,                    null,                           null,          6),
      (2100020812, 2100010001,       null,    @BranchNo,      null,        null, '2014-01-09 00:00:00',            null,                      3,              null,                     null,                    null,                           null,          6),
      (2100020813, 2100010001,       null,         null,      null,        null, '2014-01-11 00:00:00',            null,                   null,              null,                        1,                    null,                           null,          7),
      (2100020814, 2100010001,       null,    @BranchNo,      null,        null, '2014-01-11 00:00:00',            null,                   null,              null,                        1,                    null,                           null,          7)
  END
  
  IF (@Scenario=9)
  BEGIN
  INSERT INTO Warranty.WarrantyPrice
    (          Id, WarrantyId, BranchType, BranchNumber, CostPrice, RetailPrice,                                                         EffectiveDate, CostPriceChange, CostPricePercentChange, RetailPriceChange, RetailPricePercentChange, TaxInclusivePriceChange, TaxInclusivePricePercentChange, BulkEditId) VALUES
      (2100020901, 2100010001,        'C',    @BranchNo,       220,         320,                                       @SampleYear + '-02-13 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020902, 2100010001,        'C',    @BranchNo,       200,         300,                                       @SampleYear + '-02-16 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020903, 2100010001,        'C',    @BranchNo,       210,         310,                                       @SampleYear + '-02-19 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020904, 2100010001,       null,         null,       200,         300,                                       @SampleYear + '-02-13 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020905, 2100010001,       null,         null,      null,        null,                                       @SampleYear + '-02-15 00:00:00',            null,                     10,              null,                       10,                    null,                           null,       1015),
      (2100020906, 2100010001,       null,         null,      null,        null,                                       @SampleYear + '-02-18 00:00:00',             250,                   null,              null,                       15,                    null,                           null,       1015),
      (2100020907, 2100010002,       null,         null,       200,         300,                                       @SampleYear + '-02-11 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020908, 2100010002,       null,         null,      null,        null,                                       @SampleYear + '-02-18 00:00:00',            null,                     10,              null,                     null,                    null,                           null,       1013),
      (2100020909, 2100010002,       null,         null,      null,        null, CONVERT(VARCHAR, (CONVERT(INT, @SampleYear) + 1)) + '-01-01 00:00:00',             510,                   null,              null,                     null,                    null,                           null,       1013),
      (2100020910, 2100010002,       null,         null,      null,        null, CONVERT(VARCHAR, (CONVERT(INT, @SampleYear) + 2)) + '-01-01 00:00:00',            null,                      4,              null,                     null,                    null,                           null,       1013),
      (2100020911, 2100010002,       null,         null,      null,        null, CONVERT(VARCHAR, (CONVERT(INT, @SampleYear) + 3)) + '-01-01 00:00:00',            null,                      2,              null,                        5,                    null,                           null,       1013),
      (2100020912, 2100010002,       null,         null,       190,         300,                                                 '2014-02-27 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020913, 2100010002,       null,         null,       195,         305,                                                 '2014-02-28 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null),
      (2100020914, 2100010002,       null,         null,       200,         310,                                                 '2014-03-01 00:00:00',            null,                   null,              null,                     null,                    null,                           null,       null)
  END

  SET IDENTITY_INSERT Warranty.WarrantyPrice OFF

  --select * from Warranty.Warranty where id > 2100000000
  --select * from Warranty.WarrantyPrice where id > 2100000000

GO
