-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

delete from cmstrategycondition where strategy = 'RDYAST'

--Entry Conditions

	INSERT INTO CMStrategyCondition
	SELECT 'RDYAST', 'READYASSIST', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'N'

	INSERT INTO CMStrategyCondition
	SELECT 'RDYAST', 'DAYARRS', '>=', '1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'N'

	INSERT INTO CMStrategyCondition
	SELECT 'RDYAST', 'ArrearsPC', '>', '50', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'N'


--Steps

	INSERT INTO CMStrategyCondition
	SELECT 'RDYAST', 'DayX', '=', '2', NULL, NULL, '2', NULL, NULL, '1', NULL, 'S'

	INSERT INTO CMStrategyCondition
	SELECT 'RDYAST', '', NULL, NULL, NULL, NULL, '3', 'WLR1 Low Risk List 1', 'W', '2', NULL, 'S'

	INSERT INTO CMStrategyCondition
	SELECT 'RDYAST', 'DayX', '=', '14', NULL, NULL, '4', NULL, NULL, '3', NULL, 'S'

	INSERT INTO CMStrategyCondition
	SELECT 'RDYAST', '', NULL, NULL, NULL, NULL, NULL, 'WLR2 Low Risk List 2', 'W', '4', NULL, 'S'

--Exit Conditions
	
	INSERT INTO CMStrategyCondition
	SELECT 'RDYAST', 'PTPEnt', NULL, NULL, NULL, NULL, NULL, 'PTP', NULL, NULL, NULL, 'X'

	INSERT INTO CMStrategyCondition
	SELECT 'RDYAST', 'ArrearsPC', '<', '50', NULL, NULL, NULL, 'NON', NULL, NULL, NULL, 'X'

	INSERT INTO CMStrategyCondition
	SELECT 'RDYAST', 'AOK', NULL, NULL, NULL, NULL, NULL, 'NON', NULL, NULL, NULL, 'X'


--Add Actions

delete from cmstrategyactions where strategy = 'RDYAST'

insert into cmstrategyactions
select 'RDYAST', 'COM', 'Add a Comment', NULL
union
select 'RDYAST', 'PREM', 'Set a personal call reminder', NULL
union 
select 'RDYAST', 'PTP', 'Promise to Pay', NULL
union
select 'RDYAST', 'REM', 'Set a call reminder', NULL
union
select 'RDYAST', 'SPA', 'Special Arrangement', NULL
union
select 'RDYAST', 'STS', 'Send to Strategy:', NULL
union
select 'RDYAST', 'SUP', 'Supervisor', NULL
union
select 'RDYAST', 'TEL', 'Telephoned customer', NULL