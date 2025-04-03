-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if exists (select * from sys.objects where name = 'SingerBranch')
DROP TABLE SingerBranch


CREATE TABLE SingerBranch
(
	CountryCode char(1),
	CourtsBranch smallint,
	SingerBranch smallint
)

insert into SingerBranch (CountryCode, CourtsBranch, SingerBranch)
select 'L', 960, 116 union
select 'L', 960, 01010 union
select 'L', 960, 01040 union
select 'L', 750, 01060 union
select 'L', 750, 01080 union
select 'G', 700, 200 union
select 'G', 701, 272 union
select 'T', 590, 51 union
select 'T', 591, 52 union
select 'T', 605, 86 union
select 'T', 615, 87 union
select 'T', 607, 88 union
select 'T', 588, 89 union
select 'T', 589, 90 union
select 'T', 609, 280 union
select 'T', 602, 340 union
select 'T', 611, 510 union
select 'T', 612, 550 union
select 'T', 600, 116 union
select 'T', 600, 117 union
select 'T', 600, 540 union
select 'T', 611, 500 union
select 'T', 610, 560 