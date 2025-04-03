SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerGetOtherCustomers]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerGetOtherCustomers]
GO

CREATE PROCEDURE dbo.DN_CustomerGetOtherCustomers
		 	@acctno varchar(12),
		 	@return int OUTPUT

AS

	SET @return = 0			--initialise return code

	SELECT	ca.custid as 'custid',
			ca.hldorjnt as 'hldorjnt',
			c.firstname as 'firstname',
			c.name as 'name',
			a.cusaddr1 as 'address1',
			a.cusaddr2 as 'address2',
			a.cusaddr3 as 'address3',
			a.cuspocode as 'postcode',
			ISNULL(co.codedescript, '') as 'description',
			ISNULL(ct.dialcode, '') + ISNULL(ct.telno, '') as 'hometel',
			'' as 'worktel',
			'' as 'mobileno',
			'' as comment,
			Notes as Directions
	FROM		custacct ca INNER JOIN customer c ON ca.custid = c.Custid
			INNER JOIN custaddress a ON a.custid = ca.Custid
			LEFT JOIN code co on ca.hldorjnt = co.code AND category = 'LCT'
			LEFT JOIN custtel ct on ca.Custid = ct.Custid AND tellocn = 'H'
	WHERE 	ca.acctno = @acctno
	AND 		ca.hldorjnt != 'H'
	AND		a.addtype = 'H'
	AND		a.datemoved IS NULL
	UNION
	SELECT	'' as 'custid',
			'R' as 'hldorjnt',
			p.name as 'firstname',
			p.surname as 'name',
			p.address1 'address1',
			p.address2 'address2',
			p.city 'address3',
			p.postcode as 'postcode',
			ISNULL(c.codedescript, 'Reference') as 'description',
			p.telcode + p.tel as 'hometel',
			p.wtelcode + p.wtel as 'worktel',
			p.mtelcode + p.mtel as 'mobileno',
			p.comment,
			p.Directions
	FROM	proposalref p
			LEFT JOIN code c on p.relation = c.code AND category = 'RL1'
	WHERE	acctno = @acctno

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


