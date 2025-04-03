IF OBJECT_ID('CM_BailiffAllocationRulesLoad') IS NOT NULL
	DROP PROCEDURE CM_BailiffAllocationRulesLoad
GO

CREATE PROCEDURE dbo.CM_BailiffAllocationRulesLoad
	@return INTEGER OUT
AS 
	SET @return = 0   
  
	SELECT   
		BranchorZone,  
		count(empeeno) as numEmps   
	INTO #e1  
	FROM   
		CMBailiffAllocationRules  
	GROUP BY   
		BranchorZone  
	ORDER BY   
		BranchorZone  
   
	SELECT   
		empeeno,  
		empeetype,  
		r.BranchorZone,  
		0 as Bailiffs,  
		0 as NumAccs,       
		IsZone,  
		AllocationOrder,  
		empeenochange,  
		datechange,  
		reallocate   
	FROM   
		CMBailiffAllocationRules r  
  
	SELECT 
		cmw.acctno, 
		c.zone
	INTO #cmwTemp
	FROM 
		cmworklistsacct cmw
		INNER JOIN custacct ca    
			ON cmw.acctno = ca.acctno   
			AND ca.hldorjnt = 'H' 
		INNER JOIN custaddress c     
			ON ca.custid = c.custid     
		AND c.addtype = 'H'  
		AND c.datemoved IS NULL   
	WHERE 
		cmw.dateto IS NULL  
		AND NOT EXISTS 
		(
			SELECT 1   
			FROM follupalloc f   
			WHERE f.acctno = cmw.acctno AND f.datedealloc IS NULL
		)     

	SELECT DISTINCT  
		r.BranchorZone,  
		numemps as Bailiffs,  
		numacs as NumAccs  
	FROM   
		CMBailiffAllocationRules r  
		LEFT OUTER JOIN #e1   
			on #e1.BranchorZone=r.BranchorZone  
		LEFT OUTER JOIN   
		(  
			SELECT r.BranchorZone, COUNT(DISTINCT cmw.acctno) as numacs  
			FROM   
				#cmwTemp cmw   
				INNER JOIN   
				(  
					SELECT DISTINCT BranchorZone  
					FROM CMBailiffAllocationRules   
				) r  
				ON 
				(
					cmw.zone=r.branchorzone    
					OR substring(cmw.acctno, 1, 3) = r.branchorzone
				)   
			GROUP BY   
				r.BranchorZone  
		) AS t1   
			on t1.BranchorZone=r.BranchorZone    