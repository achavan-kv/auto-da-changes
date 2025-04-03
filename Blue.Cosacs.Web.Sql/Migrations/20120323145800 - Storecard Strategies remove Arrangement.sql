-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Remove any references to Special Arrangement for Store Card

DELETE FROM cmstrategycondition where strategy in ('SCARB', 'SCARR')

DELETE FROM cmstrategycondition where actioncode in ('SCARB', 'SCARR')

DELETE FROM cmstrategyactions where strategy in ('SCARB', 'SCARR')

DELETE FROM cmstrategyacct where strategy in  ('SCARB', 'SCARR')

DELETE FROM cmworklistsacct where strategy in ('SCARB', 'SCARR')

DELETE FROM cmstrategy where strategy in ('SCARB', 'SCARR')

DELETE FROM cmworklistactions where worklist in ('SCWAB1', 'SCWAB2', 'SCWAU1')

DELETE FROM cmworklist where worklist in ('SCWAB1', 'SCWAB2', 'SCWAU1')

DELETE FROM cmworklistrights where worklist in ('SCWAB1', 'SCWAB2', 'SCWAU1')

DELETE FROM cmactionrights where strategy in ('SCARB', 'SCARR')


