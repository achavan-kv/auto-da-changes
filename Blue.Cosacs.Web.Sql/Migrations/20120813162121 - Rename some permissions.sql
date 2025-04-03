-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE Admin.Permission SET Name = 'Sys Config - Customise Menus' WHERE id = 8
UPDATE Admin.Permission SET Name = 'Sys Config - Customise Mandatory Fields' WHERE id = 13
UPDATE Admin.Permission SET Name = 'Scoring - Import Scoring Rules' WHERE id = 58
UPDATE Admin.Permission SET Name = 'Scoring - Edit Scoring Matrix ' WHERE id = 59
UPDATE Admin.Permission SET Name = 'Scoring - Customise RF Scoring Matrix' WHERE id = 60
UPDATE Admin.Permission SET Name = 'Sys Config - Stock Item Translation' WHERE id = 66
UPDATE Admin.Permission SET Name = 'Sys Config - Screen Translation' WHERE id = 74
UPDATE Admin.Permission SET Name = 'Sys Config - Code Maintenance' WHERE id = 79
UPDATE Admin.Permission SET Name = 'Sys Config - Staff Maintenance' WHERE id = 80
UPDATE Admin.Permission SET Name = 'Branch - View' WHERE id = 81
UPDATE Admin.Permission SET Name = 'SysConfig - Config File Maintenance' WHERE id = 97
UPDATE Admin.Permission SET Name = 'Transactions - Transaction Type Maintenance' WHERE id =115
UPDATE Admin.Permission SET Name = 'Transactions - Exchange Rates' WHERE id = 145
UPDATE Admin.Permission SET Name = 'Sys Config - Country Maintainance' WHERE id = 151
UPDATE Admin.Permission SET Name = 'Transactions - Edit Transaction codes' WHERE id = 164
UPDATE Admin.Permission SET Name = 'Branch - Details' WHERE id = 182
UPDATE Admin.Permission SET Name = 'Branch - Bank Deposits' WHERE id = 183
UPDATE Admin.Permission SET Name = 'Transactions - Overages and Shortages' WHERE id = 184
UPDATE Admin.Permission SET Name = 'Transactions - View Terms type' WHERE id = 187
UPDATE Admin.Permission SET Name = 'Transactions - Edit Terms type' WHERE id = 188
UPDATE Admin.Permission SET Name = 'Transactions - Activate Terms type' WHERE id = 189
UPDATE Admin.Permission SET Name = 'Sys Config - Delivery Area Maintenance' WHERE id = 200
UPDATE Admin.Permission SET Name = 'Sys Config - Account Number Control' WHERE id = 215
UPDATE Admin.Permission SET Name = 'End Of Day - Configuration' WHERE id = 237
UPDATE Admin.Permission SET Name = 'Sys Config - Payment File Definition' WHERE id = 239
UPDATE Admin.Permission SET Name = 'End of Day - Tasks EOD Delete' WHERE id = 241
UPDATE Admin.Permission SET Name = 'Scoring - Edit scoreboard Matrix' WHERE id = 275
UPDATE Admin.Permission SET Name = 'Staff Maintainance - Logon History' WHERE id = 277
UPDATE Admin.Permission SET Name = 'SysConfig - Sales Commission Maintenance' WHERE id = 288
UPDATE Admin.Permission SET Name = 'SysConfig - Sales Commission -Spiffs' WHERE id = 289
UPDATE Admin.Permission SET Name = 'SysConfig - Bank Maintainance' WHERE id = 304
UPDATE Admin.Permission SET Name = 'Branch - Storecard Interest Rates' WHERE id = 349
UPDATE Admin.Permission SET Name = 'Branch - Store Card Qualification Rule' WHERE id = 350
UPDATE Admin.Permission SET Name = 'SysConfig - View Non Stock Maintainance' WHERE id = 351
UPDATE Admin.Permission SET Name = 'SysConfig - Edit Non Stock Maintainance' WHERE id = 352
UPDATE Admin.Permission SET Name = 'Warranty Return Code Maintenance - View' WHERE id = 353
UPDATE Admin.Permission SET Name = 'Warranty Return Code Maintenance - Edit' WHERE id = 354
UPDATE Admin.Permission SET Name = 'Staff Maintainance - Lock User' WHERE id = 379
UPDATE Admin.Permission SET Name = 'Staff Maintainance - Change password' WHERE id = 382
UPDATE Admin.Permission SET Name = 'Staff Maintainance - Password Reset For other users' WHERE id = 383
UPDATE Admin.Permission SET Name = 'Staff Maintainance - Edit' WHERE id = 384
UPDATE Admin.Permission SET Name = 'Branch - Edit' WHERE id = 385
UPDATE Admin.Permission SET Name = 'Sessions - User Sessions' WHERE id = 386
UPDATE Admin.Permission SET Name = 'Sessions - Kill User Session ' WHERE id = 387
UPDATE Admin.Permission SET Name = 'Staff Maintainance - Create User' WHERE id = 388
UPDATE Admin.Permission SET Name = 'Staff Maintainance - Audit User' WHERE id = 389
UPDATE Admin.Permission SET Name = 'SysConfig - Audit' WHERE id = 390
