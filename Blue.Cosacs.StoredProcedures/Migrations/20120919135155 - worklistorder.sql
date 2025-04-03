SELECT DISTINCT SortColumnName,AscDesc,SortOrder, id = IDENTITY(INT,1,1)
INTO #temp
FROM dbo.CMWorkListSortOrder
ORDER BY SortOrder


DELETE FROM dbo.CMWorkListSortOrder

INSERT INTO dbo.CMWorkListSortOrder
        ( EmpeeType ,
          SortColumnName ,
          SortOrder ,
          AscDesc
        )
select '' , -- EmpeeType - varchar(3)
          SortColumnName , -- SortColumnName - varchar(32)
          id, -- SortOrder - smallint
          AscDesc  -- AscDesc - varchar(4)
        FROM #temp
        
DROP TABLE #temp