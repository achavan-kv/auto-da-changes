
	if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DN_OracleInteg_GetCustomerDetail]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dbo].[DN_OracleInteg_GetCustomerDetail]
	GO

	---------------------------------------------------------------------------------------------------
	-- Author : NMoe
	-- Oracle Integration Phase II
	-- 31/03/2009
	-- Contents of this procedure is a scrap copied from and almost identical to 'OracleExport_InvoiceCustomerReceiptSP' 
	-- ------------------------------------------------------------------------------------------------


	CREATE PROCEDURE [dbo].[DN_OracleInteg_GetCustomerDetail] 
					@runNo int,
					@return int output
	                                
	AS
					SET @return = 0                                -- initialise return code
	-----------------------------------------------------------------------------------------------------
					SELECT OE.AcctNo, OE.agrmtno, REPLACE(LI.DeliveryAddress,' ','H') as DeliveryAddress, SUM(LI.OrdVal) as OrdVal, ' ' as ThisAdr
					INTO #CustDelAdr
					FROM LineItemoracleExport OE 
					INNER JOIN LineItem LI on OE.ItemNo = LI.ItemNo and OE.AcctNo = LI.AcctNo and OE.AgrmtNo = LI.AgrmtNo 
													and OE.StockLocn = LI.StockLocn and OE.ContractNo = LI.ContractNo 
					--INNER JOIN StockItem SI on LI.ItemNo = SI.ItemNo and LI.StockLocn = SI.StockLocn
					WHERE OE.RunNo = @runNo -- and SI.ItemType = 'S'
					GROUP BY OE.AcctNo, LI.DeliveryAddress,OE.agrmtno
					ORDER BY OE.AcctNo, LI.DeliveryAddress,OE.agrmtno

					DECLARE @datepreviousrun DATETIME
	                
					SELECT @datepreviousrun =datestart FROM interfacecontrol WHERE interface = 'orinteg2'
					AND runno = @runNo -1
	                
					-- insert where no order details being sent - only if account cancelled. 
					INSERT INTO #CustDelAdr (acctno,                          DeliveryAddress,                             OrdVal,                 ThisAdr ,agrmtno) 
					SELECT f.AcctNo, 'H',0,' ',1
					FROM FinTransOracleExport f ,acct a 
					WHERE f.RunNo = @runNo
					AND NOT EXISTS (SELECT * FROM #CustDelAdr d WHERE d.acctno= f.AcctNo)
					AND f.AcctNo = a.acctno 
	                
	                
					INSERT INTO #CustDelAdr 
					(acctno,DeliveryAddress,OrdVal,ThisAdr,agrmtno)
					SELECT a.acctno, 'H',0,'',1
					FROM acct a
					INNER JOIN SR_ChargeAcct SRCT ON SRCT.AcctNo = a.acctno
					inner join delivery d on --SRCT.acctno = d.acctno and
					 d.agrmtno like '%'+convert(varchar, srct.servicerequestno)
					WHERE ( (d.datetrans BETWEEN (SELECT MAX(datestart) 
														 FROM interfacecontrol
														 WHERE interface = 'OrInteg2'
														 AND runno = @runNo-1) AND (SELECT MAX(datestart) 
																									FROM interfacecontrol
																									WHERE interface = 'OrInteg2'
																									AND runno = @runNo ))
						or (d.datetrans BETWEEN (SELECT MAX(datestart) 
														 FROM interfacecontrol
														 WHERE interface = 'OrInteg2'
														 ) AND (SELECT GETDATE()
																									FROM interfacecontrol
																									WHERE interface = 'OrInteg2'
																									AND 0 = @runNo ))
																									
																									
																									
																									
																									) 

					-- select delivery address with highest ord_value as Delivered address
					UPDATE #CustDelAdr
									SET ThisAdr = 'Y'
					FROM #CustDelAdr DA
					WHERE OrdVal =(Select MAX(OrdVal) From #CustDelAdr DA1 Where DA1.AcctNo = DA.AcctNo Group By DA1.AcctNo)
	  -----------------------------------------------------------------------------------------------------
	                

	  -----------------------------------------------------------------------------------------------------
					SELECT DISTINCT 
									CAST(REPLACE(CU.FirstName +' '+ CU.Name,',',' ') as varchar(90)) as CustomerName,
									RTRIM(LTRIM(UPPER(CA.CustId))) as CustId, DA.AcctNo, 'Person' as CustType1,
									CAST(REPLACE(CU.Title,',',' ') as varchar(25))as Title, 
									CAST(REPLACE(CU.Firstname,',',' ') as varchar(30)) as FirstName, 
									CAST(REPLACE(CU.Name,',',' ') as varchar(60)) as Name, 'N/A' as CustType2,'N/A' as CustClass, 
									CAST(REPLACE(REPLACE(CAST(ISNULL(CCC.Code,'N/A') as varchar(10)),'N/A','Individual'),'STAF','Employee') as varchar(10)) as CustCat,
									CAST(REPLACE(HTEL.TelNo,',',' ') as varchar(30)) as HomeTelNo, CAST(REPLACE(BA.Email,' ',' ') as varchar(60)) as Email,
									RTRIM(LTRIM(UPPER(ca.custid))) as Passport, AG.EmpeeNoSale as EmpeeNo,
									CAST(REPLACE(BA.CusAddr1,',',' ') as varchar(50)) as BillAddr1, CAST(REPLACE(BA.CusAddr2,',',' ') as varchar(50)) as BillAddr2,
									CAST(REPLACE(BA.CusAddr3,',',' ') as varchar(50)) as BillAddr3, CAST(' ' as varchar(50)) as BillCity, 
									CAST(REPLACE(BA.CusPoCode,',',' ') as varchar(10)) as BillPostCode, CM.Value as BillCountry, ISNULL(BA.AddressID,0) as BillAdrRef,
									CAST(REPLACE(SA.CusAddr1,',',' ') as varchar(50)) as ShipAddr1, CAST(REPLACE(SA.CusAddr2,',',' ') as varchar(50)) as ShipAddr2,
									CAST(REPLACE(SA.CusAddr3,',',' ') as varchar(50)) as ShipAddr3, CAST(' ' as varchar(50)) as ShipCity, 
									CAST(REPLACE(SA.CusPoCode,',',' ') as varchar(10)) as ShipPostCode, CM.Value as ShipCountry, ISNULL(SA.AddressID,0) as ShipAdrRef,
									CAST(REPLACE(MTEL.TelNo,',',' ') as varchar(30)) as MobileTelNo, CAST(REPLACE(WTEL.TelNo,',',' ') as varchar(30)) as WorkTelNo, @runno AS Runno 
					INTO #OracleCustomersTemp   
					From  #CustDelAdr DA 
					LEFT JOIN Agreement AG on DA.AgrmtNo = AG.AgrmtNo and DA.AcctNo = AG.AcctNo
					LEFT JOIN CustAcct CA on DA.AcctNo = CA.AcctNo and CA.HldorJnt = 'H'
					LEFT JOIN Customer CU on CA.CustId = CU.CustId
					LEFT JOIN CustAddress BA on CA.CustId = BA.CustId and BA.AddType = 'H'  -- (Billing)Home Address -- change to loj As might not be Home address
																	and ( BA.DateMoved is NULL or CA.CustId = 'Paid & Taken' )        -- P&T may not have datemoved set correctly                                                                                                                                                              
					LEFT JOIN CustAddress SA on CA.CustId = SA.CustId and SA.AddType = DA.DeliveryAddress 
																	and SA.DateMoved is NULL -- (Shipping)Delivery Address
					LEFT JOIN CustTel HTEL on CA.CustId = HTEL.CustId and HTEL.TelLocn = 'H' and HTEL.DateDiscon is NULL -- Home tel
					LEFT JOIN CustTel MTEL on CA.CustId = MTEL.CustId and MTEL.TelLocn = 'M' and MTEL.DateDiscon is NULL          -- Mobile tel
					LEFT JOIN CustTel WTEL on CA.CustId = WTEL.CustId and WTEL.TelLocn = 'W' and WTEL.DateDiscon is NULL          -- Work tel
					LEFT JOIN CustCatCode CCC on CA.CustId = CCC.CustId and CCC.Code = 'STAF',
					CountryMaintenance CM
					WHERE CM.CodeName = 'countryname'  --and OE.RunNo = @runNo 
					AND DA.ThisAdr = 'Y'
	  -----------------------------------------------------------------------------------------------------
	                
	                
	  -----------------------------------------------------------------------------------------------------
					IF (@@error = 0)
					BEGIN   
									-- Set City (from 2nd or 3rd line of address) -------------------------------
									UPDATE #OracleCustomersTemp
									SET BillCity = Case
																									   When BillAddr3 != '' Then BillAddr3
																									   When BillAddr2 != '' Then BillAddr2
																									   Else BillCity
																					   End,
													ShipCity = Case
																									   When ShipAddr3 != '' Then ShipAddr3
																									   When ShipAddr2 != '' Then ShipAddr2
																									   Else ShipCity
																					   End
									-----------------------------------------------------------------------------
	                                
	                                
									-- Clear Address line containing City ---------------------------------------
									UPDATE #OracleCustomersTemp
									SET BillAddr3 = Case
																													When BillAddr3 = Billcity Then '.'
																													When BillAddr3 = '' Then '.'
																													Else BillAddr3                                                     
																									End,
													BillAddr2 = Case
																													When BillAddr2 = Billcity Then '.'
																													Else BillAddr2                                     
																									End,
													ShipAddr3 = Case
																													When ShipAddr3 = Shipcity Then '.'          
																													Else ShipAddr3                                  
																									End,
													ShipAddr2 = Case
																													When ShipAddr2 = Shipcity Then '.'          
																													Else ShipAddr2                  
																									End,
													Email = Case
																									When Email = '' Then '.'
																									Else Email
																					End,                                       
													WorkTelNo = case
																													When ISNULL(WorkTelNo,'') = '' Then '.'
																													Else WorkTelNo
																									End,
													MobileTelNo = case
																													  When ISNULL(MobileTelNo,'') = '' Then '.'
																													  Else MobileTelNo
																									  End
									-----------------------------------------------------------------------------
	                                                                
					END
	  -----------------------------------------------------------------------------------------------------
	                

	  -----------------------------------------------------------------------------------------------------
					SELECT * FROM #OracleCustomersTemp
	  -----------------------------------------------------------------------------------------------------

		SET @return = @@error
	                
	                
	GO
	SET QUOTED_IDENTIFIER OFF 
	GO
	SET ANSI_NULLS ON 
	GO


