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
          MinAccounts ,
          MaxAccounts ,
          AllocationRank ,
          empeechange ,
          firstname ,
          lastname 
        )
SELECT DISTINCT 0 , -- origbr - smallint
          hobranchno , -- branchno - smallint
          -113 , -- empeeno - int
          code , -- empeetype - varchar(3)
          0.0 , -- commndue - float
          0 , -- alloccount - int
          '9999999999' , -- password - varchar(10)
          0 , -- serialno - int
          '2011-03-01 14:06:49' , -- datepasschge - datetime
          '2011-03-01 14:06:49' , -- datelstaudit - datetime
          0 , -- maxrow - int
          0 , -- lstcommn - money
          '' , -- dutyfree - char(1)
          0 , -- loggedIn - smallint
          '2011-03-01 14:06:49' , -- loggedInAt - datetime
          '' , -- FactEmployeeNo - varchar(4)
          '' , -- MachineLoggedOn - varchar(32)
          0.0 , -- UpliftCommissionRate - float
          0 , -- MinAccounts - int
          0 , -- MaxAccounts - int
          0 , -- AllocationRank - smallint
          0 , -- empeechange - int
          'Auto DA' , -- firstname - varchar(50)
          'System Account'  -- lastname - varchar(50)
         FROM code,country
        WHERE codedescript = 'system administrator'
        

    