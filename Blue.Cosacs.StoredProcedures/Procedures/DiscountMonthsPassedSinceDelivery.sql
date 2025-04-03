SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF OBJECT_ID('DiscountMonthsPassedSinceDelivery') IS NOT NULL
    DROP PROCEDURE DiscountMonthsPassedSinceDelivery
GO

CREATE PROCEDURE DiscountMonthsPassedSinceDelivery
	@custid 		  varchar(20),		
    @itemId           int,
    @currentDate      date,
    @monthsSinceDelivery int OUTPUT

AS

    declare @DiscountDelDate date

    select @DiscountDelDate = (select 
                                  max(d.datetrans)
                              from 
                                  delivery d
                              inner join 
                                  custacct ca on d.acctno = ca.acctno
                              where 
                                  ca.custid = @custid
                                  and d.itemid = @itemId
                                  and d.delorcoll = 'D'
                                  and ca.hldorjnt = 'H')

    --Return -1 to indicate discount has not previously been sold
    set @monthsSinceDelivery = (select isnull(DATEDIFF(month, @DiscountDelDate, @currentDate)
                              - CASE
                                    WHEN @DiscountDelDate = @currentDate THEN 1
                                    WHEN DAY(@currentDate) < DAY(@DiscountDelDate) THEN 1
                                    ELSE 0
                                END
                              + 1,-1))
GO
