INSERT INTO courtsperson
        ( origbr ,
          branchno ,
          empeeno ,
          empeetype ,
          commndue ,
          alloccount ,
          password ,
          serialno ,
          datepasschge ,
          datelstaudit ,
          maxrow ,
          lstcommn ,
          dutyfree ,
          loggedIn ,
          loggedInAt ,
          FactEmployeeNo ,
          MachineLoggedOn ,
          UpliftCommissionRate ,
          empeechange ,
          MinAccounts ,
          MaxAccounts ,
          AllocationRank ,
          firstname ,
          lastname ,
          RICashierCode
        )
VALUES  ( 0 , -- origbr - smallint
          0 , -- branchno - smallint
          -116 , -- empeeno - int
          'N' , -- empeetype - varchar(3)
          0.0 , -- commndue - float
          0 , -- alloccount - int
          '9999999999' , -- password - varchar(10)
          0 , -- serialno - int
          '2011-08-22 16:00:07' , -- datepasschge - datetime
          '2011-08-22 16:00:07' , -- datelstaudit - datetime
          0 , -- maxrow - int
          0 , -- lstcommn - money
          '' , -- dutyfree - char(1)
          0 , -- loggedIn - smallint
          '2011-08-22 16:00:07' , -- loggedInAt - datetime
          '' , -- FactEmployeeNo - varchar(4)
          '' , -- MachineLoggedOn - varchar(32)
          0.0 , -- UpliftCommissionRate - float
          0 , -- empeechange - int
          0 , -- MinAccounts - int
          0 , -- MaxAccounts - int
          0 , -- AllocationRank - smallint
          'StoreCardEOD' , -- firstname - varchar(50)
          'StoreCardEOD',  -- lastname - varchar(50)
          0  -- RICashierCode - int
        )