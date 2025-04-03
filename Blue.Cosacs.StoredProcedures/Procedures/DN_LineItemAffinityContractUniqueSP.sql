SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemAffinityContractUniqueSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemAffinityContractUniqueSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemAffinityContractUniqueSP
			@contract varchar(10),
			@acctno varchar(12),
			@agreementno int,
			@unique smallint OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@unique = count(*)
	FROM	lineitem L INNER JOIN stockitem S
	ON	L.itemno = S.itemno
	AND	L.stocklocn = S.stocklocn
	WHERE	L.contractno = @contract
	AND	L.acctno != @acctno
	AND	(s.category = 11 or (s.category >=51 and s.category <=59)) -- AA 18/08/04		--make sure it's an affinity item

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

