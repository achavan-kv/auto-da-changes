-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE #ServiceDeliverers
	(
		 NAME VARCHAR(200),
		 Id int IDENTITY (1, 1) NOT NULL
	)

INSERT INTO #ServiceDeliverers
	SELECT c.codedescript
	FROM code c WHERE category = 'SRDELIVERER'
	AND c.statusflag = 'L'
	
declare @text varchar(max)=''

;with srdel as (select id from #ServiceDeliverers)

select @text = @text + name + char(13)+char(10) from #ServiceDeliverers s inner join srdel d on d.id=s.id

if exists(select * from config.Setting where id='ServiceDeliverers')
	update config.Setting set ValueText=@text where id='ServiceDeliverers'
else
	insert into config.Setting (Namespace, Id, ValueBit, ValueInt, ValueDateTime, ValueDecimal, ValueString, ValueText, ValueFile)
	values ('Blue.Cosacs.Service','ServiceDeliverers',null,null,null,null,null,@text,null)


