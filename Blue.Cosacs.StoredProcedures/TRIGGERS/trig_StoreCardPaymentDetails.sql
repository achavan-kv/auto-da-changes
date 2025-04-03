IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='trig_storecardPaymentDetails' )
DROP TRIGGER trig_storecardPaymentDetails 
GO 
CREATE TRIGGER trig_storecardPaymentDetails ON StoreCardPaymentDetails FOR UPDATE 
AS 
SET NOCOUNT ON 
DECLARE @oldacctno CHAR(12), @oldPaymentMethod CHAR(4), @oldPaymentOption CHAR(4), @oldRateId INT , @oldInterestRate FLOAT , @oldRateFixed BIT ,@empeeno INT  ,
@oldStatementFrequency VARCHAR(24), @oldDateLastStatementPrinted DATETIME,
@oldDatePaymentDue datetime, @oldStatus VARCHAR(5),  @oldContactMethod VARCHAR(6), @oldDateNotePrinted SMALLDATETIME, @oldNextContactDate DATETIME
DECLARE @newacctno CHAR(12), @newPaymentMethod CHAR(4), @newPaymentOption CHAR(4), @newRateId INT , @newInterestRate FLOAT , @newRateFixed BIT , 
@newStatementFrequency VARCHAR(24), @newDateLastStatementPrinted DATETIME,
@newDatePaymentDue datetime, @newStatus VARCHAR(5),  @newContactMethod VARCHAR(6), @newDateNotePrinted SMALLDATETIME, @newNextContactDate DATETIME

SELECT @oldacctno =acctno, @oldPaymentMethod =PaymentMethod, @oldPaymentOption=PaymentOption,
 @oldRateId=RateId, @oldInterestRate=InterestRate, @oldRateFixed=RateFixed, @oldStatementFrequency=StatementFrequency,
 @oldDateLastStatementPrinted=DateLastStatementPrinted,@oldDatePaymentDue=DatePaymentDue, @oldStatus=Status,
 @oldContactMethod=ContactMethod, @oldDateNotePrinted=DateNotePrinted,
 @oldNextContactDate=NextContactDate
 FROM deleted 

SELECT @newacctno =acctno, @newPaymentMethod =PaymentMethod, @newPaymentOption=PaymentOption,
 @newRateId=RateId, @newInterestRate=InterestRate, @newRateFixed=RateFixed, @newStatementFrequency=StatementFrequency,
 @newDateLastStatementPrinted=DateLastStatementPrinted,@newDatePaymentDue=DatePaymentDue, @newStatus=Status,
  @newContactMethod=ContactMethod, @newDateNotePrinted=DateNotePrinted,
 @newNextContactDate=NextContactDate, @empeeno = lastupdatedby 
 FROM inserted 

 if @empeeno is null
   set @empeeno=99999
--SELECT @newStatementFrequency,@oldStatementFrequency 

if @oldPaymentMethod	!= @newPaymentMethod					
	insert into StoreCardAudit	(empeeno,datechanged,field,oldvalue,newvalue,acctno)	values ( @empeeno,getdate(),'PaymentMethod',@oldPaymentMethod,@newPaymentMethod		,@newacctno)
if @oldPaymentOption	!= @newPaymentOption					
	insert into StoreCardAudit	(empeeno,datechanged,field,oldvalue,newvalue,acctno)	values ( @empeeno,getdate(),'PaymentOption',@oldPaymentOption,@newPaymentOption		,@newacctno)
if @oldRateId	!= @newRateId					
	insert into StoreCardAudit	(empeeno,datechanged,field,oldvalue,newvalue,acctno)	values ( @empeeno,getdate(),'RateId',CONVERT(VARCHAR,@oldRateId),CONVERT(VARCHAR,@newRateId)		,@newacctno)
if @oldInterestRate	!= @newInterestRate					
	insert into StoreCardAudit	(empeeno,datechanged,field,oldvalue,newvalue,acctno)	values ( @empeeno,getdate(),'InterestRate',CONVERT(VARCHAR,@oldInterestRate),CONVERT(VARCHAR,@newInterestRate)		,@newacctno)
if @oldRateFixed	!= @newRateFixed					
	insert into StoreCardAudit	(empeeno,datechanged,field,oldvalue,newvalue,acctno)	values ( @empeeno,getdate(),'RateFixed',CONVERT(VARCHAR,@oldRateFixed),CONVERT(VARCHAR,@newRateFixed)		,@newacctno)
								
								
if @oldStatementFrequency	!= @newStatementFrequency					
	insert into StoreCardAudit	(empeeno,datechanged,field,oldvalue,newvalue,acctno)	values ( @empeeno,getdate(),'StatementFrequency',@oldStatementFrequency,@newStatementFrequency		,@newacctno)
if isnull(@oldDateLastStatementPrinted,getdate())	!= isnull(@newDateLastStatementPrinted,getdate())					
	insert into StoreCardAudit	(empeeno,datechanged,field,oldvalue,newvalue,acctno)	values ( @empeeno,getdate(),'DateLastStatementPrinted',CONVERT(VARCHAR,@oldDateLastStatementPrinted),CONVERT(VARCHAR,@newDateLastStatementPrinted)		,@newacctno)
if isnull(@oldDatePaymentDue,getdate())	!= isnull(@newDatePaymentDue,getdate())					
	insert into StoreCardAudit	(empeeno,datechanged,field,oldvalue,newvalue,acctno)	values ( @empeeno,getdate(),'DatePaymentDue',CONVERT(VARCHAR,@oldDatePaymentDue),CONVERT(VARCHAR,@newDatePaymentDue)		,@newacctno)
if isnull(@oldStatus,'')	!= isnull(@newStatus,'')					
	insert into StoreCardAudit	(empeeno,datechanged,field,oldvalue,newvalue,acctno)	values ( @empeeno,getdate(),'Status',@oldStatus,@newStatus		,@newacctno)
if isnull(@oldContactMethod,'')	!= isnull(@newContactMethod,'')					
	insert into StoreCardAudit	(empeeno,datechanged,field,oldvalue,newvalue,acctno)	values ( @empeeno,getdate(),'ContactMethod',@oldContactMethod,@newContactMethod		,@newacctno)
if isnull(@oldDateNotePrinted,getdate())	!= isnull(@newDateNotePrinted,getdate())					
	insert into StoreCardAudit	(empeeno,datechanged,field,oldvalue,newvalue,acctno)	values ( @empeeno,getdate(),'DateNotePrinted',CONVERT(VARCHAR,@oldDateNotePrinted),CONVERT(VARCHAR,@newDateNotePrinted)		,@newacctno)
if isnull(@oldNextContactDate,getdate())	!= isnull(@newNextContactDate,getdate())					
	insert into StoreCardAudit	(empeeno,datechanged,field,oldvalue,newvalue,acctno)	values ( @empeeno,getdate(),'NextContactDate',CONVERT(VARCHAR,@oldNextContactDate),CONVERT(VARCHAR,@newNextContactDate)		,@newacctno)

GO
