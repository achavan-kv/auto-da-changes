DELETE FROM ProposalBS WHERE oldlimit = 0
GO 

IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='ProposalBSLoadCustomers')
	DROP PROCEDURE ProposalBSLoadCustomers
GO 	
CREATE PROCEDURE dbo.ProposalBSLoadCustomers
@category      VARCHAR(32), --Rejected, Lower, Higher,Same ,Band, Parallel,Scoreband, Blocked
@runno INT ,
@return INT OUTPUT
AS
/*Called from Behavioural score Results screen
*/
SET NOCOUNT ON 
SET @return = 0 



DECLARE @statement SQLText, @nl VARCHAR(20), @ApplyEodImmediate VARCHAR(8),@scoretype VARCHAR(10)
SELECT @ApplyEodImmediate=value FROM CountryMaintenance WHERE CodeNAME LIKE 'BehaveApplyEodImmediate'
SELECT @scoretype=value FROM CountryMaintenance WHERE CodeNAME LIKE 'BehaviouralScorecard'


--SELECT TOP 30 * FROM ProposalBS order by RUNNO DESC 
SET @nl = '
'

SET @statement =' SELECT p.custid AS CustomerId,c.NAME AS Name,b.dateprop AS [Date Proposal], ' + 
' b.points AS [New Points], ' + @nl 

 SET @statement = @statement + ' B.oldpoints AS [Old Points],  b.newlimit AS [Behavioural Limit], b.oldlimit AS [Old Limit], ' + @nl 	

IF @ApplyEodImmediate !='True'
	SET @statement = @statement + ' b.Applied, ' + @nl 

--set @statement = @statement + ' b.ScoringBand AS [New Band], b.oldBand AS [Old Band], '		
set @statement = @statement + ' C.ScoringBand AS [Customer Band], b.oldBand AS [Old Band], '	 --IP - 09/03/11 - #3290 - Select band from customer

set @statement=@statement +  ' b.scoringBand as [Behave Band], p.acctno AS AccountNo , a.AcctType AS AcctType, b.Reason,b.Reason2' + @nl + 
' FROM proposalbs b  join customer c on c.custid = b.custid ' + @nl +  
' left join proposal p on p.custid = b.custid and p.acctno= b.acctno ' + @nl + 
' JOIN acct a ON a.acctno= p.acctno ' +
' WHERE ' + @nl  


SET @statement = @statement + ' p.dateprop = (SELECT MAX(pl.dateprop) FROM proposal pl WHERE pl.acctno= p.acctno AND pl.custid = p.custid) ' + @nl + 	-- get latest
' '
	SET @statement = @statement + ' AND b.runno= ' + convert(varchar,@runno) + @nl 
/*ELSE 
BEGIN --parallel run 
	SET @statement = @statement + ' AND b.runno= 0' 
END
*/

IF @category = 'LOWER Limit' --AND @ApplyEodImmediate = 'False'
BEGIN
	SET @statement = @statement +
	' AND b.newlimit < b.oldlimit'  + @nl 
END
 
if @category = 'Higher Limit' 
BEGIN 
	--IF @ApplyEodImmediate = 'True'
		SET @statement = @statement +
		' AND b.newlimit > b.oldlimit '  + @nl 
	/*ELSE 
		SET @statement = @statement +
		' AND b.newlimit > c.rfcreditlimit ' + @nl */
END

if @category = 'Same Limit' 
BEGIN 
	SET @statement = @statement + ' AND b.newlimit = b.oldlimit '  + @nl 
END

if @category = 'Rejected'
BEGIN 
	SET @statement = @statement +
	' AND b.propresult = ''X''  ' + @nl 
END

if @category = 'ScoreBand Changed'
BEGIN 
	SET @statement = @statement +
	' AND b.ScoringBand != B.OLDBAND ' + @nl 
END

if @category = 'Blocked Credit' -- Blocked Credit
BEGIN
	SET @statement = @statement + ' and c.creditblocked = 1 '
END
ELSE 
BEGIN
	SET @statement = @statement + ' and c.creditblocked = 0 '
END

EXEC sp_executesql @statement
PRINT @statement
SET @return = @@ERROR
IF @return !=0 
BEGIN
	PRINT @statement 
END
GO 