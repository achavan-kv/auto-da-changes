UPDATE pf
SET pf.datecleared = pf2.datecleared,
    pf.empeenopflg = pf2.empeenopflg
FROM proposalflag pf, proposalflag pf2
WHERE pf.datecleared IS NULL
AND pf.checktype IN ('S1','S2','DC')
AND pf2.custid = pf.custid
AND pf2.checktype = pf.checktype
AND pf2.datecleared = (SELECT MAX(datecleared) 
					   FROM proposalflag pf3
					   WHERE pf3.custid = pf.custid
					   AND pf3.checktype = pf.checktype)