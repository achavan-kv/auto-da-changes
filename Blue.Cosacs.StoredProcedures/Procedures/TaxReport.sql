IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaxReport]') AND type IN (N'P', N'PC'))
  DROP PROCEDURE [dbo].[TaxReport]
GO

CREATE PROCEDURE [dbo].[TaxReport]
AS
BEGIN
declare @totalsales_lux money,
		@totalsalesorder_lux int,
		@totalsales_ob money,
		@totalsalesorder_ob int,
		@totalsales_del money,
		@totalsalesorder_del int,
		@normtaxamt_LUX money,
		@normtaxamt_OB money


select  d.*, p.id as ProductId 
into #temp1 from delivery as d join merchandising.product as p on d.itemno = p.sku

select t1.* , t.Name 
into #temp2 from #temp1 t1 inner join [Merchandising].[TaxRate] t on t.ProductId = t1.ProductId 

select  @totalsales_lux=sum(transvalue) from #temp2 where Name='LUX'
select  @totalsalesorder_lux=count(distinct acctno) from #temp2 where Name='LUX'

select @totalsales_del= (select sum(transvalue) from delivery) 
select @totalsalesorder_del= (select count(distinct acctno) from delivery ) 

select @totalsales_ob = @totalsales_del - @totalsales_lux
select @totalsalesorder_ob = @totalsalesorder_del - @totalsalesorder_lux

select @normtaxamt_LUX=(select distinct(Rate) from [Merchandising].[TaxRate] where Name = 'LUX') * (select @totalsales_lux)
select @normtaxamt_OB=(select distinct(Rate) from [Merchandising].[TaxRate] where Name = 'OB') * (select @totalsales_Ob)

create table #TaxHistoryReport(Id int IDENTITY,Total_Sales_Order money,Total_Sales money, NonTaxable money,Total_Taxable_Amt money,Norm_Tax_Amt money,Total_Tax_Amt money)
insert into #TaxHistoryReport values(@totalsalesorder_del,@totalsales_del,@totalsales_lux,@totalsales_ob,@normtaxamt_LUX,@normtaxamt_LUX)
insert into #TaxHistoryReport values(@totalsalesorder_del,@totalsales_del,@totalsales_ob,@totalsales_lux,@normtaxamt_OB,@normtaxamt_OB)

select * from #TaxHistoryReport

drop table #temp1
drop table #temp2
drop table #TaxHistoryReport

END
go

 
