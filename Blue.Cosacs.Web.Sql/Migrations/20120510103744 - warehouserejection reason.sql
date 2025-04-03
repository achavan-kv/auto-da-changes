
INSERT INTO codecat
        ( origbr ,
          category ,
          catdescript ,
          codelgth ,
          forcenum ,
          forcenumdesc ,
          usermaint ,
          CodeHeaderText ,
          DescriptionHeaderText ,
          SortOrderHeaderText ,
          ReferenceHeaderText ,
          AdditionalHeaderText ,
          ToolTipText ,
          Additional2HeaderText
        )
SELECT 0 , -- origbr - smallint
          'WPR' , -- category - varchar(12)
          N'Warehouse Picking Rejection Reasons' , -- catdescript - nvarchar(64)
          20 , -- codelgth - int
          'N' , -- forcenum - char(1)
          'N' , -- forcenumdesc - char(1)
          'Y' , -- usermaint - char(1)
          'Rejection Code' , -- CodeHeaderText - varchar(30)
          'Rejection Reason' , -- DescriptionHeaderText - varchar(30)
          '' , -- SortOrderHeaderText - varchar(30)
          '' , -- ReferenceHeaderText - varchar(30)
          '' , -- AdditionalHeaderText - varchar(30)
          '' , -- ToolTipText - varchar(300)
          ''  -- Additional2HeaderText - varchar(30) 
          UNION ALL 
SELECT 0 , -- origbr - smallint
          'WLR' , -- category - varchar(12)
          N'Warehouse Load Rejection Reasons' , -- catdescript - nvarchar(64)
          20 , -- codelgth - int
          'N' , -- forcenum - char(1)
          'N' , -- forcenumdesc - char(1)
          'Y' , -- usermaint - char(1)
          'Rejection Code' , -- CodeHeaderText - varchar(30)
          'Rejection Reason' , -- DescriptionHeaderText - varchar(30)
          '' , -- SortOrderHeaderText - varchar(30)
          '' , -- ReferenceHeaderText - varchar(30)
          '' , -- AdditionalHeaderText - varchar(30)
          '' , -- ToolTipText - varchar(300)
          ''  -- Additional2HeaderText - varchar(30) 
          UNION ALL 
SELECT 0 , -- origbr - smallint
          'WDR' , -- category - varchar(12)
          N'Warehouse Delivery Rejection Reasons' , -- catdescript - nvarchar(64)
          20 , -- codelgth - int
          'N' , -- forcenum - char(1)
          'N' , -- forcenumdesc - char(1)
          'Y' , -- usermaint - char(1)
          'Rejection Code' , -- CodeHeaderText - varchar(30)
          'Rejection Reason' , -- DescriptionHeaderText - varchar(30)
          '' , -- SortOrderHeaderText - varchar(30)
          '' , -- ReferenceHeaderText - varchar(30)
          '' , -- AdditionalHeaderText - varchar(30)
          '' , -- ToolTipText - varchar(300)
          ''  -- Additional2HeaderText - varchar(30) 

