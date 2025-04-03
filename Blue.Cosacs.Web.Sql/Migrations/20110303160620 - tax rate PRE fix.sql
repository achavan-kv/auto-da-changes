SELECT *, DATEADD(SECOND,ROW_NUMBER() OVER (ORDER BY (CASE WHEN dateto < '1901-01-01' THEN '2050-01-01' ELSE dateto END)) -1,datefrom) AS new
INTO #temp
FROM taxratehistory H
WHERE datefrom IN (SELECT datefrom 
				   FROM taxratehistory H2
				   GROUP BY datefrom
				   HAVING COUNT(*) > 1)

UPDATE taxratehistory
SET datefrom = new
FROM #temp t
WHERE taxratehistory.datefrom = t.datefrom
AND taxratehistory.dateto = t.dateto
AND taxratehistory.taxrate = t.taxrate 
				