
/****** Object:  Trigger [Warehouse].[CloseServiceRequest]    Script Date: 8/21/2019 7:55:52 PM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE [name] = N'trig_RevertLineitemOnBookingClose' AND [type] = 'TR')
BEGIN
      DROP TRIGGER [Warehouse].[trig_RevertLineitemOnBookingClose];
END;
go

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create TRIGGER [Warehouse].[trig_RevertLineitemOnBookingClose] ON [Warehouse].[Cancellation] 
AFTER INSERT  
AS 

DECLARE @BookingId INT,
		@date datetime,
		@AcctNo NVARCHAR(20),
		@ItemNo NVARCHAR(18),
		@StockBranch int,
		@quantitybefore int,
		@quantityafter int,
		@valuebefore money,
		@valueafter money,
		@taxamtbefore money,
		@taxamtafter money,
		@datechange datetime

 SELECT @BookingId =  id FROM INSERTED;

 SELECT @Acctno = AcctNo, @ItemNo = ItemNo, @StockBranch = StockBranch , @date = DeliveryOrCollectionDate from [Warehouse].[Booking] WHERE id = @BookingId

 SELECT top 1 @datechange from lineitemaudit where acctno=@Acctno AND ITEMNO=@itemno and cast(datechange as date) = cast(@date as date)
 and stocklocn = @StockBranch order by lineitemauditid desc
  
 create table #itemno(id int PRIMARY KEY IDENTITY, itemno varchar(18),contractno VARCHAR(10))
 insert into #itemno
 select distinct itemno,contractno from lineitem where acctno=@acctno and (itemno = @itemno or parentitemno = @itemno)

 declare @counter int = 1, @id int;
 select @id = max(id) from #itemno
 while(@counter <= @id)
	 BEGIN
		 SELECT top 1  @quantitybefore = quantitybefore,@quantityafter=quantityafter,@valuebefore = valuebefore,@valueafter = valueafter,
		 @taxamtbefore = taxamtbefore,@taxamtafter = taxamtafter from lineitemaudit 
		 where acctno=@Acctno AND ITEMNO=(select itemno from #itemno where id = @counter)
		 and cast(datechange as date) = cast(@date as date)
		 and stocklocn = @StockBranch
		 order by lineitemauditid desc

		 update lineitem
		 set quantity = @quantitybefore, ordval = @valuebefore, taxamt = @taxamtbefore
		 where acctno=@Acctno AND ITEMNO=(select itemno from #itemno where id = @counter)
		 and ordval = @valueafter and quantity = @quantityafter and taxamt = @taxamtafter
		 set @counter = @counter + 1

		 declare @buffno int , @transrefno int
		 select @buffno = buffno,@transrefno = transrefno from delivery where itemno = (select itemno from #itemno where id = @counter) and delorcoll ='C'
		 and contractno = (select contractno from #itemno where id = @counter) and parentitemno = @itemno

		 delete from delivery where acctno = @acctno and buffno = @buffno and transrefno = @transrefno
		 delete from fintrans where acctno = @acctno and transrefno = @transrefno and transtypecode = 'GRT'
	 END
 

 --*************************************************************************************************************	
---------------------------------------
--Update agreement details
---------------------------------------	
	UPDATE agreement
	SET agrmttotal = (SELECT SUM(ordval) FROM lineitem
	                  WHERE lineitem.acctno = agreement.acctno
						and lineitem.isKit = 0
						and lineitem.itemno not in (select code from code where category = 'HCI' and reference = '1')),
        datechange = DATEADD(MINUTE, 1, datechange)
	WHERE acctno IN (@acctno)

 --*************************************************************************************************************	
---------------------------------------
--Update acct details
---------------------------------------
	UPDATE acct
	SET agrmttotal = (SELECT SUM(ordval) FROM lineitem
	                  WHERE lineitem.acctno = acct.acctno
						and lineitem.isKit = 0
						and lineitem.itemno not in (select code from code where category = 'HCI' and reference = '1')),
	     outstbal =  (SELECT SUM(transvalue) FROM fintrans
	                  WHERE fintrans.acctno = acct.acctno)
	WHERE acctno IN (@acctno)

--*************************************************************************************************************	

