
GO

IF EXISTS (	SELECT	* 
			FROM	dbo.sysobjects 
			WHERE	id = object_id('[dbo].[DN_CashierOutstandingIncomeGetSP]') 
					AND OBJECTPROPERTY(id, 'IsProcedure') = 1
			)
BEGIN
	DROP PROCEDURE [dbo].[DN_CashierOutstandingIncomeGetSP]
END

GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[DN_CashierOutstandingIncomeGetSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CashierOutstandingIncomeGetSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Cashier Outstanding Income
-- Date         : 12/12/2019
-- Version		: 003
-- This procedure will get the Cashier Outstanding Income.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/02/12  jec #9700 Amount for the Paymethods which are not displayed in Cashier deposits screen should be excluded from the amount available to deposit
-- 22/01/13  ip  #11209 Return all employees that have an outstanding amount to deposit irrespective of the user right to display the user in the Cashier
--				 Deposits screen.
-- 02/10/19   cp  The optimization will limit the screen to return only persons who have outstanding moneys to be deposited to the safe/bank (all users will now not be listed)	
-- ================================================
	-- Add the parameters for the stored procedure here
			@branchno smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	/* First select all of the employees from the courtsperson 
	   table who we are likely to be interested in. We are interested in
	   cashiers who have outstanding income i.e. entries in the 
	   vw_cashieroutstandingincome view. Either the courtsperson must 
	   belong to the branch passed in or the outstanding income record
	   must be for that branch. This is so that cashiers can change 
	   branch without messing things up. 

	   Cashiers are all put in a temp table which will be updated subsequently */

	   --02/10/19 Filter users to return only those with only payment permissions----Start
	   SELECT	
			u.id AS EmpeeNo,
			u.FullName as EmployeeName,
			Convert(money,0) as fordeposit,
			Convert(money,0) as transferredtosafe,
			Convert(money,0) as allocatedfloat,
			N'N' as hastotalled
		INTO #cashiers
		From [Admin].[RolePermission] R
			Inner Join [Admin].[Permission] P
			On R.PermissionId = P.Id
			Inner Join [Admin].[Role] RO
			On R.Roleid = RO.Id
			Inner Join [Admin].UserRole ur 
			On ur.RoleId = Ro.Id
			Inner Join [Admin].[User] u 
			on ur.UserId=u.id
			LEFT OUTER JOIN vw_cashieroutstandingincome [COS] 
			ON u.id = [COS].empeeno
		WHERE 
			(u.branchno = @branchno OR [COS].branchno = @branchno) 
		And Admin.CheckPermission(u.id, 177) = 1  ------12/12/19 added to allow users who are permitted to be visible on the screen 
		--AND  P.Name = 'Payments' And Locked =0  ------12/112/19 commented out to allow users who are permitted to be visible on the screen 
		GROUP BY u.id, u.FullName



	--SELECT	u.id AS EmpeeNo,
	--		u.FullName as EmployeeName,
	--		Convert(money,0) as fordeposit,
	--		Convert(money,0) as transferredtosafe,
	--		Convert(money,0) as allocatedfloat,
	--		N'N' as hastotalled
	--INTO		#cashiers
	--FROM Admin.[User] u 
	--LEFT OUTER JOIN vw_cashieroutstandingincome [COS] ON u.id = [COS].empeeno 
	--WHERE 
	--	--IP - 22/01/13 - #11209
	--	--Admin.CheckPermission(u.id, 177) = 1		
	--	--AND 
	--(u.branchno = @branchno OR [COS].branchno = @branchno)
	--GROUP BY u.id, u.FullName
	--02/10/19 Filter users to return only those with only payment permissions----End

	DECLARE	@empeeno int, 
			@transferredtosafe money, 
			@allocatedfloat money, 
			@totalnewincome money,
			@insafe money,
			@totaltosafe money,
			@hastotalled char(1),
			@fordeposit money
	
	SET		@totaltosafe = 0

	/* Create a cursor to loop through the cashiers contained in the 
	   temporary table */
	DECLARE	cashiers_cursor CURSOR
	FOR	
	SELECT	empeeno
	FROM 		#cashiers
	
	OPEN		cashiers_cursor
	FETCH NEXT FROM cashiers_cursor
	INTO		@empeeno
	
	WHILE	@@FETCH_STATUS = 0
	BEGIN
		/* Find out how much the current cashier has deposited to 
		   Safe */
		EXEC	DN_CashierSafeDepositsSP	@empeeno = @empeeno,
							@paymethod  = '', 
							@branchno = @branchno,
							@safedeposits  = @transferredtosafe OUT,
							@return = @return OUT

		/* Find out how much the current cashier has been allocated  
		   from the Safe */
		EXEC	DN_CashierSafeFloatSP		@empeeno = @empeeno,
							@paymethod = '',
							@branchno = @branchno,
							@safefloats  = @allocatedfloat OUT,
							@return = @return OUT

		/* Find out whether the cashier has totalled */
		EXEC DN_FintransHasCashierTotalledSP 	@empeeno = @empeeno,
												@hastotalled = @hastotalled OUTPUT,
												@return = @return OUTPUT

		/* maintain a sum of the total amount of money that has moved to 
		   and from the safe (for all cashiers) */
		SELECT	@totaltosafe = @totaltosafe + @transferredtosafe + @allocatedfloat

		/* fintrans_new_income contains a copy of all payment records 
		   received by a cashier during their session. When they run 
		   cashier totals records are deleted from this table and a 
		   corresponding record will be written to the cashieroutstanding 
		   table */
		SELECT	@totalnewincome = isnull(sum(transvalue), 0)
		FROM		fintrans_new_income
		WHERE	empeeno = @empeeno


		/*	need to make sure that everything in the @fordeposit figure
			is given in local currency. This will not be the case straight
			out of the vw_cashieroutstandingincome view which is why we must
			look at the two tables which the view looks at separately. 
			Foreign currency records in the cashieroutstanding table
			must be converted back to local currency. Note that the best
			we can do here is use the current exchange rate. This may lead
			to small errors but this is not critical because this is used
			to populate the top datagrid in the cashierdeposits screen
			which is for information purposes only */				
		SELECT		@fordeposit = isnull(sum(depositoutstanding),0) 
		FROM			cashieroutstanding o
					INNER JOIN code c on o.paymethod=c.code and c.category='FPM'		-- #9700
		WHERE		empeeno = @empeeno
		AND			branchno = @branchno
		AND			Convert(int, paymethod) < 100 AND CONVERT(INT,paymethod) !=13					/* not storecard local currency */
		and (LEN(c.additional2)>1 and SUBSTRING(c.additional2,2,1)!='0')	-- #9700	
		
		SELECT		@fordeposit = @fordeposit + isnull(round(sum(CO.depositoutstanding * isnull(ER.rate, 1)),2),0) 
		FROM			cashieroutstanding CO INNER JOIN
					exchangerate ER ON CO.paymethod = ER.currency
					AND	ER.datefrom <= getdate()
					AND	ER.Status = 'C'
					INNER JOIN code c on co.paymethod=c.code and c.category='FPM'		-- #9700
		WHERE		CO.empeeno = @empeeno
		AND			CO.branchno = @branchno
		AND			Convert(int, CO.paymethod) >= 100				/* foreign currency */
		and (LEN(c.additional2)>1 and SUBSTRING(c.additional2,2,1)!='0')	-- #9700		
		
		SELECT		@fordeposit = @fordeposit + isnull(sum (-transvalue),0)
		FROM 			fintrans_new_income
		WHERE		empeeno = @empeeno
		AND			branchno = @branchno		

		/* update the temporary table with the figures calculated to far */
		UPDATE	#cashiers
		SET		transferredtosafe = isnull(@transferredtosafe,0),
				allocatedfloat = -isnull(@allocatedfloat,0),
				hastotalled = 	@hastotalled,
				fordeposit = @fordeposit
		WHERE	empeeno = @empeeno	
	
		FETCH NEXT FROM cashiers_cursor
		INTO		@empeeno
	END
	
	CLOSE 		cashiers_cursor
	DEALLOCATE	cashiers_cursor

    EXEC DN_SafeBalanceGetSP @BranchNo = @BranchNo,
                             @InSafe   = @InSafe OUTPUT, 
                             @Return   = @Return OUTPUT

	SELECT	empeeno,
			EmployeeName,	
			fordeposit,
			transferredtosafe,
			allocatedfloat,
			hastotalled
	FROM 		#cashiers
	--IP - 22/01/13 - #11209
		WHERE 

			[Admin].CheckPermission(empeeno, 177) = 1 OR  ------12/12/19 added to allow users who are permitted to be visible on the screen 
			 (fordeposit != 0 OR transferredtosafe != 0 OR allocatedfloat != 0)
	UNION
	SELECT	-1, 'Safe',  @insafe, 0, 0, ''	
	ORDER BY	empeeno	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO