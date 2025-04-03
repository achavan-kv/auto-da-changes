

SELECT '2' AS ID,'Add Customer/Account Codes' AS NAME,'1' AS category,'Add codes to customer - Allows user access to theAdd codes to customer screen from various area in the system' AS description 
INTO #tempPerm UNION ALL
Select '3','View Account Details','1','Account Search - Enables access to the Account Search screen via the Account menu' UNION ALL
Select '4','New Sales Order - Duty Free Sales','10','New Sales Order - Determines if Tick box at the top of the screen for duty free is available.  This is also dependant on the Duty Free country parameter' UNION ALL
Select '6','Customer Search','5','Main Screen - Gives user access to the customer search screen under the customer menu' UNION ALL
Select '7','Remove Customer-Account link','5','Customer details - Allows user to link an existing customers account to different customer' UNION ALL
Select '8','Customise Menus','12','Customise Menus - Allows user access to the Customise menu screen under System Configuration Menu' UNION ALL
Select '9','View Customer Details','5','Customer Search - Enables access to the Customer Search screen via the Customer menu' UNION ALL
Select '10','New Customer','5','Customer Record - Enables access to the Customer Record screen via the Customer menu in order to set up a new customer' UNION ALL
Select '12','Manual Sale','10','Manual Sale - Enables access to the Manual Sale screen via the Account menu' UNION ALL
Select '13','Customise Mandatory Fields','12','Customise Mandatory Fields - Allows user access to the Customise Mandatory Fields screen under the System Configuration Menu' UNION ALL
Select '14','Revise Account','1','Revise Account - Enables access to the Revise Account screen via the Account menu' UNION ALL
Select '58','Import Scoring Rules','12','Customer Scoring Rules - Allows the user to import new scoring rules set' UNION ALL
Select '59','Edit Scoring Matrix','12','Customise Credit Matrices - Allows user access to the Customise Credit Matrices screen under the Scoring Menu accessed via the Systems Maintenance menu' UNION ALL
Select '60','Customise RF Scoring Matrix','12','Scoring - Allows user access to the Scoring menu option (which then offers all scoring options) under System Configuration Menu' UNION ALL
Select '61','Incomplete Credit Applications','4','Incomplete Credit Applications - Enables access to the Incomplete Credit Applications screen via the Credit menu' UNION ALL
Select '62','Create RF Accounts','5','Customer record - Allows access to the Create RF Account button on the customer record screen' UNION ALL
Select '63','Customer Details - Sanction Accounts','4','Customer Record | Sanction menu - Allows user access to the Sanction menu for credit Accounts' UNION ALL
Select '64','Cash and Go','10','Cash and Go - Allows user access to the Cash and Go screen via the Account Menu' UNION ALL
Select '66','Stock Item Translation','12','Stock Item Translation - Enables access to the Stock Item Translation screen via the System Administration menu' UNION ALL
Select '67','Goods Return','5','Goods Return - Enables access to the Goods Return Screen screen via the Customer menu' UNION ALL
Select '68','Authorise Delivery','4','Authorise Delivery - Allows the user access to the Authorise Delivery Screen from the Customer Menu.' UNION ALL
Select '69','Payments','2','Payments - Enables access to the Payment screen via the Transaction menu' UNION ALL
Select '71','Reopen Stage 1','4','Sanction Stage 1 - this permission will allow the user access to Re-open Stage 1 option in the Sanction menu' UNION ALL
Select '74','Screen Translation','12','Screen Translation - Enables access to the Screen Translation screen via the System Administration menu' UNION ALL
Select '75','Customer Details - Refer Rejected Account','4','Sanctioning - Allows the user to refer an account if it has been rejected based on the data entered when applying for credit' UNION ALL
Select '76','Credit Staff Allocation','3','Collection Account Analysis - Allows user access to the Collection Account Analysis screen via the Credit menu' UNION ALL
Select '77','Telephone Action','3','Telephone Action - Enables access to the telephone action screen via the Credit menu' UNION ALL
Select '78','Search Cash and Go','1','Search Cash and Go - Enables access to the Search Cash and Go screen via the Account menu' UNION ALL
Select '79','Code Maintenance','12','Code Maintenance - Allows access to the Code Maintenance scree nunder System Configuration menu' UNION ALL
Select '80','Staff Maintenance','12','System Configuration | Staff Maintenance - Allows user access to the Staff Maintenance Screen from the System Configuration menu' UNION ALL
Select '81','Branch','12','Branch - Allows user access to the Branch screen from the System Configuration Menu' UNION ALL
Select '82','Copy Staff Allocations to Excel','3','Collection Account Analysis - Allows user acess to the copy to excel button on the result Tab withing the collection Account Analysis screen' UNION ALL
Select '83','Sanction Stage 1 - Manual Refer','4','Credit Santioning - Allows Access to the Manual Refer option in the Sanction menu within the Credit Sanctioning screens' UNION ALL
Select '85','Sanction Stage 1','4','Credit Santioning - Allows access to the Sanction Stage 1 screens' UNION ALL
Select '86','Sanction Stage 2','4','Credit Santioning - Allows access to the Sanction Stage 2 screens' UNION ALL
Select '87','Underwriter Referral','4','Credit Santioning - With this permission the user is able to view the Under writer (UW) tab on within the sanctioning process' UNION ALL
Select '88','Document Confirmation','4','Sanctioning - Gives user access to the Document confirmations screen within the Sanctioning process' UNION ALL
Select '89','Reopen Stage 2','4','Sanction Stage 2 - this permission will allow the user access to Re-open Stage 2 option in the Sanction menu' UNION ALL
Select '90','Reopen Document Confirmation','4','Document Confirmation - This permission will allow the user access to Re-open Document Confirmation option in the document confirmation menu' UNION ALL
Select '92','View Proposal From DA','4','Delivery Authorisation - Gives the user the ability to right click on an account and select view proposal from within the Delivery Authorisation screen' UNION ALL
Select '93','Revise Account - Change Terms Type','10','Revise Sales Order - Allows the user to edit the terms type of an account when accessing it through the revise account search screen' UNION ALL
Select '94','New Sales Order - Create Special Accounts','10','New Sales Order - Determines if Special is available under the account types for manual sales' UNION ALL
Select '96','Referral - Override Credit Limit','4','Underwriters - If the user has this permission they can manually change the credit limit on the Underwriters screen within the sanctioning process' UNION ALL
Select '97','Config File Maintenance','12','File Menu - Allows access to the Config File Maintenance screen under the File menu of CoSACS.' UNION ALL
Select '99','View Credit Score Result','4','Sanction Stage 1 - Allows the user to see the result of credit sanctioning when completing sanction stage 1' UNION ALL
Select '100','Cashier Totals - Print Cashier Totals By Branch','2','Branch tab - Enables print icon and therefore allows user to print cashier totals from the Cashier Totals screen' UNION ALL
Select '101','Cashier Totals - Print Cashier Totals By Employee','2','Employee tab - Enables print icon and therefore allows user to print cashier totals from the Cashier Totals screen' UNION ALL
Select '105','Refunds and Corrections','2','Refunds and Corrections - Enable access to the Refunds and Corrections screen via the Transaction menu' UNION ALL
Select '106','Cheque Return','2','Cheque Return - Allows user access to the Cheque Return screen via the Transactions menu' UNION ALL
Select '107','New Sales Order - Change Branch For New Sale','5','Customer Record - Enables the user to view and change the Branch drop down in the open new Account ara of the Customer record screen' UNION ALL
Select '108','Check Customer Reference','4','Sanctioning - Enables the Reference Checked checkbox and shows the checked by and date checked fields on the References tab at Sanction stage 2' UNION ALL
Select '109','Cashier Totals - Save Unreconciled Cashier Totals','2','Cashier Totals - Enables save icon and therefore allows user to save (with authorisation) unreconciled amounts on the Cashier Totals screen' UNION ALL
Select '110','Refunds and Corrections - Enter Corrections','2','Refunds and Corrections - Allows user to enter Corrections on the Refunds and Corrections screen' UNION ALL
Select '111','Refunds and Corrections - Enter Refunds','2','Refunds and Corrections - Allows user to enter Refunds on the Refunds and Corrections screen' UNION ALL
Select '112','General Financial Transactions','7','General Financial Transactions - Enables access to the General Financial Transactions screen via the Transactions menu' UNION ALL
Select '113','Giro Mandate','8','Giro Mandate - Enables access to the Giro Mandate Screen screen via the Credit menu' UNION ALL
Select '114','Cancel Account','1','Cancel Account - Allows user access to the Cancel Account screen via the Credit menu/ other screens' UNION ALL
Select '115','Transaction Type Maintenance','12','Transaction Type Maintenance - Enables access to the Transaction Type Maintenance screen via the System Administration menu' UNION ALL
Select '116','Authorise Delivery - Clear Proposal','4','Authorise Delivery - Allows the user to have the clear proposal option in the right click menu when selecting an account in Authorise Delivery screen' UNION ALL
Select '117','Customer Details - Add To','5','Customer Record - Enables the option add to account on the rightclick of an account line on the Customer Record Screen' UNION ALL
Select '118','Payments - Credit Fee Waiver','2','Payment - Gives the user permission to authorise a change in credit fee when taking a payment on account in arrears and assigned to a collector' UNION ALL
Select '119','Payments - Credit Fee Waiver(Combined RF)','2','Payment - Gives the user permission to authorise a change to credit fees when taking a payment on multiple sub RF accounts  in arrears and assigned to a collector' UNION ALL
Select '120','Stock Enquiry By Location','14','Stock Enquiry by Location - Enables access to the Stock Enquiry by Location screen via the Warehouse menu' UNION ALL
Select '121','Stock Enquiry By Product','14','Stock Enquiry by Product - Enables access to the Stock Enquiry by Product screen via the Warehouse menu' UNION ALL
Select '126','Authorise Cash And Go Return','10','Search Cash and go - allow the user the option to complete a Cash and go return from the right click menu of an account line on the Search Cash and Go Screen or the New Account Screen' UNION ALL
Select '128','Document Confirmation - View Previous Details','4','Sanctioning - Allows user to view previous Document confirmation details from the Document confirmation screen' UNION ALL
Select '129','Account Details - Create Bailif Action','3','Account Details - Allows the user to add a follow up action through the Follow Up tab on the Account Details Screen' UNION ALL
Select '130','Incomplete Credit Applications - View Referral Details','4','Incomplete Credit Applications - Allows the user to make use of the Referral Reason drop down box on the Incomplete Credit Applications screen' UNION ALL
Select '131','Cash and Go - Warranties on Credit','10','Cash and Go - Enables the buy on credit check box when selling warranties on cash and go to allow for the warranty to be purchased' UNION ALL
Select '132','New Sales Order - Warranties on Credit','10','New Sales Order - Allows a warranty to be sold on a cash account without charge so that the warranty can be charged separately on a credit account' UNION ALL
Select '133','Cash and Go - Instant Replacement','10','Cash and Go - Allows the sale of instant replacement warranties on cash and go accounts' UNION ALL
Select '134','Cash and Go - Supashield Warranties','10','Cash and Go - Allows the sale of supashield/extended warranties on cash and go accounts' UNION ALL
Select '135','Status Code Maintenance','1','Status Code Maintenance - Enables access to the Status Code Maintenance screen via the Credit menu' UNION ALL
Select '136','Bad Debt Write Off Review','3','Write Off Review - Allow user access to the Write off Review screen from the Credit menu' UNION ALL
Select '137','Bad Debt Write Off Review - Accept For Write Off','3','Write Off Review - Allow User Access to the Approve button on the write off review screen' UNION ALL
Select '138','Bad Debt Write Off Review - Reject For Write Off','3','Write Off Review - Allow User Access to the Decline button on the write off review screen' UNION ALL
Select '139','Status Code Maintenance - Manual Write Off','1','Status Code Maintenance - Allows the user to change the status of an account to a write off status (6, 7 or 8)' UNION ALL
Select '140','Customer Details - Unblock Credit','4','Customer Record | Customer menu | unblock credit - Allows user access to the Unblock Credit option in the Customer Menu' UNION ALL
Select '141','Transfer Transactions','7','Transfer Transactions - Enables access to the Transfer Transaction screen via the Transactions menu' UNION ALL
Select '142','Transaction Journal Enquiry','7','Transaction Journal Enquiry - Enables access to the Transaction Journal Enquiry screen via the Transactions menu' UNION ALL
Select '143','Gift Voucher - Authorise Expired Voucher','8','Payments - Allows a user to authorise a payment made with an expired gift voucher' UNION ALL
Select '144','Gift Voucher - Sell','8','Gift Voucher - Enables access to the Sell Gift Voucher screen via the Transactions menu' UNION ALL
Select '145','Exchange Rates','12','Exchange Rate Maintenance - Enables access to the Exchange Rate Maintenance option under the System Configuration menu' UNION ALL
Select '146','Cashier Disbursements - Full Control','2','Cashier Disbursements - Enables all functions within the Cashier Disbursements screen.' UNION ALL
Select '147','Cashier Disbursements - Cashier Functions','2','Cashier Disbursements - Enables only the functions that will be useful to the Cashiers within the Cashier Disbursements screen. All other functions will be disabled' UNION ALL
Select '148','End Of Day','6','Main Screen - Access to the End of Day menu item under the System Configuration menu' UNION ALL
Select '149','Gift Voucher - Allow Free','8','Gift Voucher - Allows user to see and make use of the free check box within the details section of the Gift Voucher screen (found under the transactions menu)' UNION ALL
Select '150','Flag Customer As Bankrupt','5','Add Account/Customer Codes - Allows User the ability to flag the customer as backrupt on the Add Account/Customer Codes screen which is accessed via the Customer menu' UNION ALL
Select '151','Country Maintenance','12','System config menu - Allows user access to the Country Maintenance screen via the System Configuration menu' UNION ALL
Select '152','Revise Account - Revise Settled Accounts','10','Revise Account - Gives the user the option to revise account in the right click menu on an account that is settled when searched for via the Revise Account search screen' UNION ALL
Select '153','Open Cash Till','2','payments - Anables access to the option to open the till by selecting the Cash Till | Open cash till option on the payments screen' UNION ALL
Select '154','Open Cash Till on Log in','2','Main Screen - Opens the till on login so that the float can be placed in the cash draw' UNION ALL
Select '156','Cash And Go - Legacy Returns','10','Cash and Go Returns - Allows user access to the Legacy Cash and Go Returns screen via the Account Menu' UNION ALL
Select '157','Update Date Due','1','Update Date Due - Enables access to the Update Date Due screen via the Credit menu' UNION ALL
Select '158','Cashier Deposits','2','Cashier Deposits - Allows user access to the Legacy Cash and Go Returns screen via the Account Menu' UNION ALL
Select '159','Delete Reference Data','4','Sanction Stage 2 - Enables the - button on the References tab of Sanction Stage 2 Screen so allows user to remove references' UNION ALL
Select '160','Update Date Due - Date First > 2 Months After Del Date','1','Update Date Due - Allows the user to update the date first to a date more than 2 months after the account reached the delivery threshold' UNION ALL
Select '161','Cashier Totals - Summary Tab','2','Cashier Totals - Allows user access to the Summary Tab on Cashier Totals screen' UNION ALL
Select '162','New Account - Allow Tax Exempt Sale','10','New Sales Order - Enables the Tax Exempt check box at the top of the New Sales Order Screen' UNION ALL
Select '163','Financial Interface Report','9','Financial Interface Report - Enables access to the Financial Interface Report screen via the Reports menu' UNION ALL
Select '164','Transaction Type Maintenance - Edit Transaction Codes','12','Transaction Type Maintenance - Enables a user to edit the codes on the Transaction type Maintenance screen' UNION ALL
Select '168','Account Details - Show Agreement Changes','1','Account Details - Allows User access to the Agreement Changes tab on the account details screen' UNION ALL
Select '169','Revise Account - Revise Repossessed Accounts','10','Revise Account - Gives the user the option to revise account in the right click menu on an account that has a repossessed status when searched for via the Revise Account search screen' UNION ALL
Select '170','Cashier Disbursements - Allow Branch Float','2','Cashier Disbursements - This permissions allows users to see SAF floats in the Cashier Disbursements screen.  Note that it does not determine visiblity of the filter but if the user does not have this permission they will not see SAF transactions even with the filter selected' UNION ALL
Select '171','New Sales Order - Override Discount Limit','10','New Sales Order - When adding a discount more than X % item value value this must be authorised' UNION ALL
Select '172','Change Customer ID','5','Change Customer ID - Enables the user to change the customers ID by allowing access to the change customer ID screen on the Customer menu' UNION ALL
Select '173','New Sales Order - Refer Accounts > Available Spend Limit','10','New Sales Order - When creating an account, if you exceed the available spend limit you can refer the account instead of cancelling ' UNION ALL
Select '174','Authorise Delivery - Load Other Branches','4','Authorise Delivery - Allows the user access to the Branch drop down on the Authorise delivery screen to choose a branch they would like to see deliveries for' UNION ALL
Select '175','Credit Staff Allocation - Load Other Branches','3','Collection Account Analysis - Allows user to select different branches vis a drop down on the Collection Account Analysis screen' UNION ALL
Select '176','Bailiff Review - Search By Account Number','3','Bailiff Review - Gives the user the ability to search by account number on the Bailiff review screen' UNION ALL
Select '177','Employee Appears in Cashier Deposits','2','Cashier Deposists - Determines if the employee will be shown in the Cashier Deposits Screen' UNION ALL
Select '178','Customer Details - Add To Reversal','5','Customer Record - Enables the option add to accountReverse Add To on the rightclick of an account line on the Customer Record Screen (if there has already been an Add to performed on the account)' UNION ALL
Select '179','Change Order Details','10',' Change Order Details - Enables the user to see the Change Order details option on the Account menu and thus make changes to existing orders' UNION ALL
Select '180','Number Generation','1','Number Generation - Enables access to the Number Generation screen via the Account menu' UNION ALL
Select '181','Reverse Cancellation','1','Reverse Cancelled Account - Enables access to the Reverse Cancelled Account screen via the Credit menu' UNION ALL
Select '182','Branch - Details','12','Branch - Allows user access to the Details tab of the Branch screen' UNION ALL
Select '183','Branch - Bank Deposits','12','Branch - Allows user access to the Bank Deposits tab of the Branch screen' UNION ALL
Select '184','Overages and Shortages','12','Overages and Shortages - Allows access to the Overages and Shortages (Cashiers by branch) screen via the System configuration menu' UNION ALL
Select '185','Monitor Bookings','9','Monitor Bookings - Enables access to the Monitor Bookings screen via the Reports menu' UNION ALL
Select '186','Monitor Outstanding Deliveries','9','Monitor Deliveries - Enables access to the Monitor Deliveries screen via the Reports menu' UNION ALL
Select '187','Terms Type - View','12','Terms Type Maintenance - Enables access to the Terms Type Maintenance screen via the System Administration menu with view only access' UNION ALL
Select '188','Terms Type - Edit','12','Terms Type Maintenance - Enables access to the Terms Type Maintenance screen via the System Administration menu with ability to edit Terms' UNION ALL
Select '189','Terms Type - Activate','12','Terms Type Maintenance - Allows users with Edit Access to the Terms Type Maintenance Screen to activate or deactivate terms types' UNION ALL
Select '190','Allow Cashier To Do Renewal Sale','10','Warranty Renewals - Allows user access to the Warranty Renewals screen via the Accoutn menu or the menu option on the payments screen' UNION ALL
Select '191','Financial Transactions Query','7','Financial Transactions Query - Enables access to the Financial Transactions Query screen via the Transactions menu' UNION ALL
Select '192','Bailiff Review','3','Bailiff Review - Enables access to the Bailiff Review menu option on the Credit menu' UNION ALL
Select '197','Re-Print Action Sheet','3','Re-print Debt Collectors Action Sheet - Enable access to the Re Print Debt Collectors Action Sheet screen via the Credit menu' UNION ALL
Select '198','Giro Rejections','8','Giro Rejections - Enables access to the Giro Rejections Screen screen via the Credit menu' UNION ALL
Select '199','Giro Extra Payments','8','Giro Extra Payments - Enables access to the Giro Extra Payments Screen screen via the Credit menu' UNION ALL
Select '200','Delivery Area Maintenance','12','Delivery Area Maintenance - Allows user access to the Delivery Area Maintenance screen under the System Configuration Menu' UNION ALL
Select '201','Redelivery After Repossession','14','Authorise Redelivery After Repossession - Enable access to the Authorise Redelivery After Repossession screen via the Warehouse menu' UNION ALL
Select '202','Temporary Receipts','3','Temporary Receipts - Enables access to the Temporary Receipts screen via the Credit menu' UNION ALL
Select '204','New Sales Order - Authorise Selling Out Of Stock Products','10','New Sales Order - allows users to authorise sale of out of stock merchandise and stock on order.  Note that this relates to the country parameters Password for out of stock products required and Password for stock on order required' UNION ALL
Select '210','Unpaid Accounts - Current User','1','Unpaid Accounts - Enables access to the Unpaid Accounts screen via the Accounts menu with only their own accounts displayed' UNION ALL
Select '211','Unpaid Accounts - Current Branch','1','Unpaid Accounts - Enables access to the Unpaid Accounts screen via the Accounts menu with the Branch drop down disabled so that the user may not select another branch.  This allows user to see all sales staff for this branch' UNION ALL
Select '212','Unpaid Accounts - All Branches','10','Unpaid Accounts - Enables access to the Unpaid Accounts screen via the Accounts menu with full access to view all branches and accounts' UNION ALL
Select '215','Account Number Control','12','Account Number Control - Allows access to the Account number control screen via the system configuration menu' UNION ALL
Select '216','Cancel Collection Notes','14','Cancel Collection Note - Allows user access to the Cancel Collection Note screen via the Warehouse Menu' UNION ALL
Select '217','Bailiff Commission Maintenance','3','Collections Commission Maintenance - Allow user access to the collection Commission Maintenance Screen via the Collections menu' UNION ALL
Select '218','Calculate Bailiff Commission','3','Calculate Bailiff Commission - Allows user access to the Calculate Bailiff Commission screen via the Credit Menu' UNION ALL
Select '219','Reprint Bailiff Commission','3','Reprint Bailiff Commission - Enables access to the Reprint Bailiff Commission screen via the Credit menu' UNION ALL
Select '221','Monitor Bookings - Allow Live Database','9','Monitor Bookings - Allows access to the Use LIVE Database check box on the Monitor bookings screen' UNION ALL
Select '222','Customer Details - Bank','5','Customer Record | Bank tab - Enables the user access to the Bank Details tab on the Customer Record Screen' UNION ALL
Select '223','Customer Details - Employment','5','Customer Record | Employment Details tab - Enables the user access to the Employment Details tab on the Customer Record Screen' UNION ALL
Select '224','Goods Return - Authorise Warranty Cancellation','10','Goods Return - Allows the user to authorise the collection of a warranty delivered more than Warranty Cancellation Days (Country Parameter) ago without the collection of the parent' UNION ALL
Select '228','Revise Account - Revise Item Awaiting Scheduling','10','Revise Account - Gives the user the option to revise account in the right click menu on an account awaiting scheduling when searched for via the Revise Account search screen' UNION ALL
Select '229','Revise Account - Revise Scheduled Item','10','Revise Account - Gives the user the option to revise account in the right click menu on an account that has been scheduled when searched for via the Revise Account search screen' UNION ALL
Select '230','Temporary Receipt Investigation','3','Temporary Receipts - Enables access to the Temporary Receipt Investigation tab on the Temporary Receipts screen' UNION ALL
Select '231','Temporary Receipt Allocation','3','Temporary Receipts - Enables access to the Temporary Receipt Allocation tab on the Temporary Receipts screen' UNION ALL
Select '232','Customer - Generate HP Account','5','Customer Record - Enables the Create HP Account button on the customer record screen' UNION ALL
Select '233','Customer - Generate CASH Account','5','customer record - Enables the Create Cash Account button on the customer record screen' UNION ALL
Select '234','Customer - Generate SPECIAL Account','5','Customer record - Enables the user to select Generate Special Account under the Customer Menu on the Customer Record screen' UNION ALL
Select '235','Customer - Create Manual HP Account Number','5','Customer Record - Enables the user to select Create Manual HP Account under the Customer Menu on the Customer Record screen' UNION ALL
Select '236','Customer - Create Manual CASH Account Number','5','Customer Record - Enables the user to select Create Manual Cash Account under the Customer Menu on the Customer Record screen' UNION ALL
Select '237','End Of Day Configuration','12','End of Day - Enables access to the End of Day sub menu option under the End of Day menu item within the System Configuration menu' UNION ALL
Select '238','Customer Mailing','9','Reports | Customer Mailing - Gives user access to the Retrieve Customers screen via the Customer Mailing option on the Reports menu' UNION ALL
Select '239','Payment File Definition','12','Payment File Definition - Enables access to the Payment File Definition screen via the system Configuration menu' UNION ALL
Select '240','Summary Update Control Report','9','Summary Update Control Report - Enables access to the Summary Update Control Report screen via the Reports menu' UNION ALL
Select '241','End Of Day Configuration - Delete','12','End of Day - Enables the delete button on the End of day screen' UNION ALL
Select '242','New Sales Order - Allow Deleted Product Sales','10','New Sales Order - Gives the user permission to authorise the sale of deleted products' UNION ALL
Select '247','Create HP Accounts','5','Customer record - Allows access to the Create HP Account button on the customer record screen' UNION ALL
Select '253','Revise Account - Revise Cash Accounts','1','Revise Account | Account search - Gives the user an option to revise account in the right click menu on a cash account when searched for via the revise Account search screen' UNION ALL
Select '267','Factoring Reports','9','Factoring Report - Enables access to the Factoring Reports screen via the Reports menu' UNION ALL
Select '275','Edit Terms Type Matrix Service Charge (not Points)','12','Customise Scoreband Matrix - Allows a user to access and edit the Scoreband Matrix which determines the points required for a particular scoring band' UNION ALL
Select '276','Change Terms Type Band','10','New Sales Order Screen - A small button above the terms type when setting up an account' UNION ALL
Select '277','Staff Maintenance - View Logon History','12','Staff Maintenance - Allows access to the Show Logon History button on the Staff Maintenance Screen' UNION ALL
Select '279','Service Request Screen','11','Service menu - Enables access to the Service Menu (at the top level)' UNION ALL
Select '280','Service Request - View audit trail','11','Service Request - Enable access to the View Audit button on the Service request screen' UNION ALL
Select '282','Service Request - View Charge To Authorisation Screen','11','Service Request - Enables access to the Charge To Authorisation Screen via the Service menu' UNION ALL
Select '283','Service Request - Authorise Change to Charge To','11','Service Request - Allows the user to change the primary charge to from the default charge to, determined by the warranty status of the item, to another charge account' UNION ALL
Select '284','Service Request - Authorise Change to Resolution','11','service request - Allows the user to authorise the resolution of a service request' UNION ALL
Select '285','Service Request - Authorise Change to Extended Warranty','11','Service request - Enables access to the Extended Warranty check box on the product tab of the service request screen' UNION ALL
Select '286','Service Request - Authorise Change to Labour Cost','11','Service - Enables fields for entry of additional labour costs (transport, labour, hourly rate, additional labour)' UNION ALL
Select '287','Service Request - Enable Allocation Tab','11','service request - Enables the allocation tab so that a user can assign a Service Request to a technician' UNION ALL
Select '288','Sales Commission Maintenance','12','Sales Commission Maintenance - Enables access to the Sales Commission Maintenance screen via the System Administration menu' UNION ALL
Select '289','Sales Commissions - maintain Spiffs','12','Sales Commission Maintenance - Gives user access to the save button and to the data entry fields on the Spiffs tab of the sales commission Maintenance screen' UNION ALL
Select '290','Sales Commission Enquiry','9','Sales Commission Enquiry - Enables access to the Sales Commission Enquiry screen via the Reports menu' UNION ALL
Select '291','Sales Commission Enquiry - view all Sales Staff','9','Sales Commission Enquiry - If a user has this permission they will be able to access the Employee drop down box on the Sales Commission Enquiry Screen' UNION ALL
Select '293','EPOS','10','EPOS - Allows user access to the EPOS screen under the Account Menu.' UNION ALL
Select '294','Customer Details - Financial','5','Customer Record |Financial Details tab - Enables the user access to the Financial Details tab on the Customer Record Screen' UNION ALL
Select '295','Customer Details - Previous','5','customer record | Photo/signature - Enables the previous button on the photo/signature tab of the customer record screen' UNION ALL
Select '296','Customer Details - Residential','5','Customer Record |Residential Details tab - Enables the user access to the Residential Details tab on the Customer Record Screen' UNION ALL
Select '297','New Sales Order - Add Extra SPIFF','10','New Sales Order - Enables the user to authorise adding extra SPIFF from the right click menu of an individual item once that item is added to a new sales order' UNION ALL
Select '298','Payments - Cheque Authorisation','2','Payment - Allows authorisation of cheque payment if customer has had more than X returned cheques in the last Y days.  This relates to the country parameters Number of allowed return cheques and Returned Cheque Period' UNION ALL
Select '299','Prize Vouchers','5','Prize Vouchers - Access to the Prize Vouchers screen via the Customer menu' UNION ALL
Select '300','Prize Vouchers - Remove Existing Vouchers','5','Prize Vouchers - Allow access to the Delete Vouchers button on the Prize Voucher screen' UNION ALL
Select '301','Prize Vouchers - Reprint Existing Vouchers','5','Prize Vouchers - Allow access to the Reprint Vouchers button on the Prize Voucher screen' UNION ALL
Select '302','Revise Account - Cash Loan','10','Customer Details | Revise Account - If an account is a Cash Loan account users with this permission will be able to revise it ' UNION ALL
Select '303','Warranty Reporting','9','Warranty Reporting - Enables access to the Warranty Reporting screen via the Reports menu' UNION ALL
Select '304','Bank Maintenance','12','Bank Maintenance - Allows user access to the Bank Maintenance screen from the System Configuration Menu' UNION ALL
Select '305','Service Request - Service Request & Service Request Search','11','Service Request and Service Request Search - Enables access to the Service Request and Service Request Search Screens via the Service menu' UNION ALL
Select '306','Service Request - Technician','11','Technician Maintenance - Enables access to the Technician Maintenance and Technician Payment Screens via the Service menu' UNION ALL
Select '307','Service Request - Price Index Matrix','11','Service Price Index Matrix - Enables access to the Service Price Index Matrix screen via the Service menu' UNION ALL
Select '308','Service Request - Reports','11','Service - Enables access to the Service Claims Report, Service Failure Report, Service Progress Report and Management Review Screens via the Service menu' UNION ALL
Select '309','Service Request - Batch Print','11','Batch Print - Enables access to the Batch Print screen via the Service menu' UNION ALL
Select '310','Service Request - Customer Interaction','11','Customer Interaction - Enables access to the Customer Interaction screen via the Customer menu' UNION ALL
Select '311','Warranty Reporting -Live Database','9','Warranty Reporting - Enables access to the Live Database option button on the Warranty Reporting screen' UNION ALL
Select '313','Oracle Outbound Test','6','Oracle Test - Allows the user to access a test screen for the inbound and outbound exports to oracle' UNION ALL
Select '314','Service Request - Estimates Update Allowed','11','Service request - Allows the user to update previously entered repair estimates' UNION ALL
Select '315','Sales Commission Enquiry - view all Branches','9','Sales Commission Enquiry - If a user has this permission they will be able to access the branch drop down box on the Sales Commission Enquiry Screen' UNION ALL
Select '316','Service Request - Batch Print View All','11','Batch Print - Allows the user to print Service Job sheets for all branches' UNION ALL
Select '317','Behavioural Scoring Results','4','Behavioural Scoring - Allows user Access to the Behavioural Scoring Rescore report functionality' UNION ALL
Select '318','Scoring Band Change','10','New Sales Order - Allows the user to override the scoring band for an account in order to give a different interest rate' UNION ALL
Select '319','Cashier Totals - View Branch Tab','2','Cashier Totals - Allows user access to the Branch tab on the Cashier Totals screen' UNION ALL
Select '320','Work List Setup','3','Work List set up - Enables access to the Worklist Setup screen via the collections menu' UNION ALL
Select '321','Strategy Configuration','3','Strategy Configuration/collections - When checked the user will have the ability to view strategies' UNION ALL
Select '322','Letter Merge','3','Letter Merge - Enables access to the Letter Merge screen via the Collections menu' UNION ALL
Select '323','SMS Setup','3','SMS Setup - Enables access to the SMS Setup screen via the Collections menu' UNION ALL
Select '325','Collection Account Analysis - Allocation','3','Collection Account Analysis - Allows a user to allocate accounts returned in the Collection Account Analysis screen to a particular Bailiff/Collector' UNION ALL
Select '326','Telephone Action - View Multiple Accounts','3','Telephone Action - Enables the Accounts tab in the Telephone Action screen.' UNION ALL
Select '327','Account Status','1','Account Status - Allows user access to the Account Status Screen via the Account Menu' UNION ALL
Select '329','Customer Search - Reopen/Un-archive account','5','Customer Search - Enables the unsettle and unarchive options on the right click menu of an account in the Customer search screen for settled (not BDW) accounts' UNION ALL
Select '330','Change Order Details - Before DA','10','Change Order Details - Allows the user to make changes to a sale through the Change Order Details screen before the account is delivery authorised' UNION ALL
Select '331','Bailiff Review - Override Maximum Allocation','3','Bailiff Review - When checked the user will have access to the Override Maximum Allocation check box on the Customer/Account Information Tab of the Bailiff Review Screen' UNION ALL
Select '332','Bailiff Review - Allocate Single Account','3','Bailiff Review - Enables access to the Allocate Single Account tab on the Bailiff review Screen' UNION ALL
Select '333','Zone Allocation Automation','3','Zone Automated Allocation - Enables access to the Zone Automated Allocation screen via the Collections menu.' UNION ALL
Select '334','Tallyman 3PL Interfaces','6','Malaysia Interface - Allows the user to access Malaysia Specific EOD options such as tally man, DHL and Home Club ' UNION ALL
Select '335','Authorise Delivery - Cash accounts','4','Authorise Delivery - Makes the  Include Cash check box available on the Authorise Delivery Screen' UNION ALL
Select '336','Authorise Delivery - Credit accounts','4','Authorise Delivery - Makes the Include RF check box available on the Authorise Delivery Screen' UNION ALL
Select '337','HomeClub Edit Membership End Date','5','Customer Details - Allows the user to set a new membership end date on creation of membership' UNION ALL
Select '338','HomeClub Edit Membership End Date Override','5','Customer Details - Allows the user to set a new membership end date on an existing membership' UNION ALL
Select '339','HomeClub Review Voucher Status','5','Customer Details - Allows the user to override the status of a home club voucher' UNION ALL
Select '341','Provisions Screen','3','Provisions - Enables the user to view the Provisons Screen under the System Configuration Menu. This is where the provision percentages will be set up for all customers according to Months in Arrears and Status Codes.' UNION ALL
Select '342','Telephone Action - Apply to all accounts','3','Telephone Action - enables the apply to all accounts check box on the telephone actions screen' UNION ALL
Select '343','Telephone Action - View References','3','Telephone Action - Enables the view references button on the telephone actions screen' UNION ALL
Select '344','Bailiff Review - View References','3','Bailiff Review - gives the user access to the view References button on the Customer/Account Information Tab of the Bailiff review screen' UNION ALL
Select '345','Payments - View Customer details','2','Payements - Access to the customer details button on the Payments screen' UNION ALL
Select '347','Customer - Generate Store Card Account','5','Customer Details - Enables the create store card button on the Customer Record screen. If customers qualify for a store card user can create it.' UNION ALL
Select '348','Service Request - Technician Unavailable Dates','11','Technician Diary - Enable access to the Technician Diary Screen via the service menu. This will include access to the add unavailable dates button on the technician diary screen' UNION ALL
Select '349','Setup Storecard interest Rates','12','Store Card Rates setup - Enables access to the Store Card Rates setup screen under the Systems Administration menu' UNION ALL
Select '350','Branch - Store Card Qualification Rules','12','Branch - Allows user access to the Storecard Qualification Rules tab of the Branch screen' UNION ALL
Select '351','Non Stock Maintenance - View only','12','Non Stock Item Maintenance - Allows view only access to the Non Stock Item Maintenance screen via the System Configuration menu' UNION ALL
Select '352','Non Stock Maintenance','12','Non Stock Item Maintenance - Enables access to the Non Stock Item Maintenance screen via the System Configuration menu' UNION ALL
Select '353','Warranty Return Codes Maintenance - View only','12','Warranty Return Codes Maintenance - - Enables View Only access to the Warranty Return Codes Maintenance screen, no changes can be made.' UNION ALL
Select '354','Warranty Return Codes Maintenance','12','Warranty Return Codes Maintenance - Enables users to edit data in the Warranty Return Codes Maintenance screen' UNION ALL
Select '355','Installation - Pending Installations','11','Pending Installations - Enables access to the Pending Installations screen via the Service menu' UNION ALL
Select '356','Installation Booking Print','11','Installations booking print - Enables access to the Installation booking print screen via the Service menu' UNION ALL
Select '357','Service - Previous Repair Totals','11','Management Review Screen - Gives the user permission to view service requests where the  previous repair total has been exceeded' UNION ALL
Select '358','Installation Management','11','Installation Management - Enables access to the Installation Management screen via the Service menu' UNION ALL
Select '359','Authorise Instant Credit - Clear Proposal','4','Authorise Instant Credit - Allows the user to have the clear proposal option in the right click menu when selecting an account in Authorise Instant Credit screen' UNION ALL
Select '360','Authorise Instant Credit','4','Authorise Instant Credit - Allows the user access to the Authorise Instant Credit Screen from the Customer Menu.' UNION ALL
Select '361','Authorise Instant Credit - Load Other Branches','4','Authorise Instant Credit - Allows the user access to the Branch drop down on the Authorise Instant Credit screen to choose a branch they would like to authorise instant credit for.' UNION ALL
Select '362','View Proposal from IC','4','Instant Credit Authorisation - Gives the user the ability to right click on an account and select view proposal from within the Instant Credit Authorisation screen' UNION ALL
Select '363','Service - Reassign Technician','11','Technician Diary - The ability to re-assign a service request to another techinician even if already assigned' UNION ALL
Select '364','Installation - Rebook Technician','11','Installation - Shows the Technician Booking Panel for already booked Installations as well as a ReBooking reason' UNION ALL
Select '365','Service - Food Loss','11','Service request - Enable access to the food loss button on the Service Request screen so that the user add items to the list.' UNION ALL
Select '366','Service - Write-Off Service Cash Account','11','Management Review Screen - Allows the user to write off a customer service charge account that has been awaiting payment more than X Months.  This relies on country parameter Months Since Service Request Resolved' UNION ALL
Select '367','Service - Cancel Service Cash Account','1','Cancel Account - Allows to cancel a service charge account for a settled service request with no charge' UNION ALL
Select '368','Installation - Can Close Installation','11','Installation - Determines if the user can authorise a saving to the resolution of an installation' UNION ALL
Select '369','Authorise Delivery - View Underwriter Information','4','Authorise Delivery - When Viewing Summary for accounts, this permission will allow you to see the Underwriter Information tab' UNION ALL
Select '370','View StoreCardView screen','1','Store Card - View Details - Enables access to the StoreCard - View Details screen via the Customer menu' UNION ALL
Select '371','StoreCard - Higher Level Rights','1','Store Card - Allows the user to edit the credit limit and interest rate of an existing store card account' UNION ALL
Select '373','Cash Loan Application','10','Cash Loan Application - Allows user access to the Cash Loan Application screen via the Customer Menu' UNION ALL
Select '374','Cash Loan Disbursement','2','Cash Loan Application - Give user access to the Disbursement button on the Cash Loan Application Screen' UNION ALL
Select '376','Payment - Manual Store Card Entry','2','Payment - Allows for manual store card entry rather than swiping through mag stripe reader' UNION ALL
Select '377','Search Cash and Go - Reprint Receipt','1','Search Cash and Go - Access to the Reprint Receipt option when right clicking on an account that has come up in the search window' UNION ALL
Select '378','Search Cash and Go - Print All','1','Search Cash and Go - Access to the Print all button on the search cash and go screen' UNION ALL
Select '379','Lock User','12','User can lock another users' UNION ALL
Select '380','Allow Access for Telephone Callers','3','Allow allocation of accounts for collections in Worklist & Collection Commissions Screens' UNION ALL
Select '381','Allow Access for Collectors','3','Allow allocation of accounts for collections in Zone Automated Allocation Screen' UNION ALL
Select '382','Change Users Password', '12', 'User can change other user password' UNION ALL
Select '383','Send Password Reset', '12', 'User can send password reset for other users' UNION ALL
Select '384','Edit Users','12' , 'Edit Users' UNION ALL
Select '385','Change User Branch','12','Can change a user''s default Branch' UNION ALL
Select '386','List User Sessions','12','Allows access to the page that lists currently active user sessions' UNION ALL
Select '387','Kill User Session','12','Allows killing/terminating an active user session' UNION ALL
Select '1401','Booking Search Screen','14','Allows access to the Search Booking screen' UNION ALL
Select '1402','Picking','14','Allows access to the Picking Menu' UNION ALL
Select '1403','Reprint Picklist','14','Allows user to reprint a Picklist' UNION ALL
Select '1404','Confirm Picklist','14','Allows user to confirm a Picklist' UNION ALL
Select '1405','Print Load','14','Allows user to print a Load' UNION ALL
Select '1406','Create Schedules','14','Allows user to create a Schedule' UNION ALL
Select '1407','Reprint Schedule','14','Allows user to reprint a Schedule' UNION ALL
Select '1408','Delivery Confirmation','14','Allows user to confirm an item as delivered successfully or rejected' UNION ALL
Select '1409','Warehouse - All Branches','14','Allows user to have permission to actions on any branch - not only their default branch' UNION ALL
Select '1410','Driver Maintenance - View','14','Admin - Allows user to only view Drivers' UNION ALL
Select '1411','Driver Maintenance - Edit','14','Admin - Allows user to create/maintain Drivers' UNION ALL
Select '1412','Truck Maintenance - View','14','Admin - Allows user to only view Trucks' UNION ALL
Select '1413','Truck Maintenance - Edit','14','Admin - Allows user to create/maintain Trucks' UNION ALL
Select '1414','Driver Comissions','14','Admin - Allows user access to the Driver Comission screen' UNION ALL
Select '1415','Print Delivery Note','14','Allows user to print a Delivery Note' UNION ALL
Select '1416','Reprint Delivery Note','14','Allows user to reprint a Delivery Note' UNION ALL
Select '1417','Customer Pickup - Notify','14','Allows user to confirm an item has been picked up successfully by the customer' UNION ALL
Select '1418','Customer Pickup - All Branches','14','Allows user to confirm an item has been picked up successfully by the customer for any branch - not only their default branch' UNION ALL
Select '1419','Customer Pickup - Change Delivery Date','14','Allows user to change the pick up date of an item' UNION ALL
Select '1420','Customer Pickup - Failed','14','Allows user to confirm an item has not been picked up successfully by the customer' UNION ALL
Select '1422','Search Pick Lists','14','Allows access to the Search Pick Lists screen' UNION ALL
Select '1423','Search Delivery Schedules','14','Allows access to the Search Delivery Schedules screen' 


select 'Account Functions' name
INTO #cat UNION ALL
select 'Cashier' UNION ALL
select 'Credit Collections' UNION ALL
select 'Credit Sanctioning' UNION ALL
select 'Customer Functions' UNION ALL
select 'End of Day' UNION ALL
select 'Finance' UNION ALL
select 'Payments' UNION ALL
select 'Reports' UNION ALL
select 'Sales' UNION ALL
select 'Service' UNION ALL
select 'System Administration' UNION ALL
select 'Cosacs' UNION ALL
select 'Warehouse' 

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Admin].[FK_RolePermission_PermissionId]') AND parent_object_id = OBJECT_ID(N'[Admin].[RolePermission]'))
ALTER TABLE [Admin].[RolePermission] DROP CONSTRAINT [FK_RolePermission_PermissionId]
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Admin].[FK_Permission_PermissionsCategory]') AND parent_object_id = OBJECT_ID(N'[Admin].[Permission]'))
ALTER TABLE [Admin].[Permission] DROP CONSTRAINT [FK_Permission_PermissionsCategory]
GO

DELETE FROM Admin.Permission

DELETE FROM Admin.PermissionCategory

DBCC CHECKIDENT ( 'Admin.PermissionCategory',RESEED, 0)

INSERT INTO Admin.PermissionCategory
        ( Name )
SELECT  name FROM #cat

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
SELECT Id,name,category,Description	 FROM #tempPerm


ALTER TABLE [Admin].[Permission]  WITH CHECK ADD  CONSTRAINT [FK_Permission_PermissionsCategory] FOREIGN KEY([CategoryId])
REFERENCES [Admin].[PermissionCategory] ([Id])
GO

ALTER TABLE [Admin].[Permission] CHECK CONSTRAINT [FK_Permission_PermissionsCategory]
GO


ALTER TABLE [Admin].[RolePermission]  WITH CHECK ADD  CONSTRAINT [FK_RolePermission_PermissionId] FOREIGN KEY([PermissionId])
REFERENCES [Admin].[Permission] ([Id])
GO

ALTER TABLE [Admin].[RolePermission] CHECK CONSTRAINT [FK_RolePermission_PermissionId]
GO


DROP TABLE #cat
DROP TABLE #tempperm

