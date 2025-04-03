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
VALUES  ( 0 , -- origbr - smallint
          'ZONE' , -- category - varchar(12)
          'Warehouse Zones' , -- catdescript - nvarchar(64)
          20 , -- codelgth - int
          'N' , -- forcenum - char(1)
          'N' , -- forcenumdesc - char(1)
          'Y' , -- usermaint - char(1)
          'ItemAttribute' , -- CodeHeaderText - varchar(30)
          'Value' , -- DescriptionHeaderText - varchar(30)
          null , -- SortOrderHeaderText - varchar(30)
          null , -- ReferenceHeaderText - varchar(30)
          null , -- AdditionalHeaderText - varchar(30)
          null , -- ToolTipText - varchar(300)
          null  -- Additional2HeaderText - varchar(30)
        )

DELETE FROM codecat
WHERE category = 'zone'