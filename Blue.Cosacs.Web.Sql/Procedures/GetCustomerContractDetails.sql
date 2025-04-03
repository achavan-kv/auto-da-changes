
IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'GetCustomerContractDetails'
   )
BEGIN
DROP PROCEDURE [dbo].[GetCustomerContractDetails]
END
GO




CREATE PROCEDURE [dbo].[GetCustomerContractDetails]
	@CustId VARCHAR(20),
	@AccountNumber NVARCHAR(50),
	@DayOfMonth INT = 0,
	@Frequency char(1) = '',
	@Message NVARCHAR(MAX) OUTPUT
AS
BEGIN

	SET @Message = ''
	IF(NOT EXISTS(SELECT 1 FROM dbo.customer WHERE custid = @CustId))
	BEGIN
		SET @Message = 'No user found'
	END
	ELSE IF(NOT EXISTS(SELECT 1 FROM dbo.custacct WHERE custid=@CustId AND acctno=@AccountNumber))
	BEGIN
		SET @Message = 'No accounts for user'
	END
	ELSE
	BEGIN
		IF ISNULL(@AccountNumber,'') <> ''
		/*--BEGIN
		--	DECLARE @Date DATETIME
		--	DECLARE @DateLast DATETIME
		--	DECLARE @Month INT  = MONTH(GETDATE())
		--	DECLARE @Year INT = YEAR(GETDATE())

		--	SET @Date = DATEFROMPARTS(@year, @month, @DayOfMonth)

		--	IF(DATEDIFF(d, @Date, GetDate()) > 0)
		--	BEGIN
		--		SET @DateLast = @Date
		--		SET @Date = DATEADD(month, 1, @Date)
		--	END
		--	ELSE
		--	BEGIN
		--		SET @DateLast = DATEADD(month, -1, @Date)
		--	END

		--	UPDATE dbo.instalplan
		--	SET DATEFIRST = @Date, dueday = @DayOfMonth, datelast = @DateLast
		--	WHERE acctno = @AccountNumber

		--END*/

		--SELECT TOP 1 
		--	c.FirstName+' '+c.name as Name,
		--	ca.acctno as AccountNumber,
		--	cadd.cusaddr1+' '+cadd.cuspocode+' '+isnull(cadd.DeliveryArea,'') as Address
		--FROM customer c 
		--	JOIN custacct ca on c.custid =ca.custid 
		--	JOIN custaddress cadd on c.custid =cadd.custid    
		--WHERE c.custid=@CustId and ca.acctno=@AccountNumber

	SELECT TOP 1 
			c.FirstName+' '+c.name as Name,
			ca.acctno as AccountNumber,
			cadd.cusaddr1+' '+cadd.cuspocode+' '+isnull(cadd.DeliveryArea,'') as Address,
			datepart(day,dateprop) As propdate,
			Datename(month,dateprop) As propmonth,
			datepart(year,dateprop) As propyear,
			--Convert(date,dateprop) as Propyear,
			--quantity as PurchaseAmount,
			Convert(decimal(10,2),ip.instaltot) as PurchaseAmount,
			Convert(decimal(10,2),instalamount) as installment,
			dueday as DueDay
		FROM customer c 
			JOIN custacct ca on c.custid =ca.custid 
			JOIN custaddress cadd on c.custid =cadd.custid and cadd.addtype = 'H' 
			inner join Proposal Prop on Prop.custid= c.custid and Prop.acctno = ca.acctno
			inner join lineitem li on li.acctno= ca.acctno
			inner join instalPlan ip on ip.acctno= ca.acctno
		WHERE c.custid=@CustId and ca.acctno=@AccountNumber and
		      li.itemno = (SELECT	Top 1 l.itemno
				FROM lineitem l INNER JOIN StockInfo s ON l.ItemID = s.ID
				Inner join  merchandising.product MP on MP.SKU=s.itemno
				inner join merchandising.supplier MS on Ms.Id=MP.PrimaryVendorid
				WHERE	acctno=@AccountNumber)

		SET @Message = 'User found with details'
	END
END
