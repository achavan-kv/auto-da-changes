INSERT INTO dbo.courtsperson
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
VALUES  ( 700 , -- origbr - smallint
          700 , -- branchno - smallint
          -200 , -- empeeno - int
          'N' , -- empeetype - varchar(3)
          0.0 , -- commndue - float
          0 , -- alloccount - int
          '8956735' , -- password - varchar(10)
          0 , -- serialno - int
          '2012-06-18 15:15:55' , -- datepasschge - datetime
          '2012-06-18 15:15:55' , -- datelstaudit - datetime
          0 , -- maxrow - int
          0 , -- lstcommn - money
          '' , -- dutyfree - char(1)
          0 , -- loggedIn - smallint
          '2012-06-18 15:15:55' , -- loggedInAt - datetime
          '' , -- FactEmployeeNo - varchar(4)
          '' , -- MachineLoggedOn - varchar(32)
          0.0 , -- UpliftCommissionRate - float
          0 , -- empeechange - int
          0 , -- MinAccounts - int
          0 , -- MaxAccounts - int
          0 , -- AllocationRank - smallint
          'AutoWarehouse' , -- firstname - varchar(50)
          'PickingConfirm' , -- lastname - varchar(50)
          0  -- RICashierCode - int
        )