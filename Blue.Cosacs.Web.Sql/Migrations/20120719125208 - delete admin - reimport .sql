







Select 337 AS ID, 'HomeClub Edit Membership End Date' AS Name, 'Customer Functions' AS Category, 'Customer Details - Allows the user to set a new membership end date on creation of membership' AS Description 
INTO #temp UNION ALL
Select 338, 'HomeClub Edit Membership End Date Override', 'Customer Functions', 'Customer Details - Allows the user to set a new membership end date on an existing membership' union
Select 339, 'HomeClub Review Voucher Status', 'Customer Functions', 'Customer Details - Allows the user to override the status of a home club voucher' union
Select 313, 'Oracle Outbound Test', 'End of Day', 'Oracle Test - Allows the user to access a test screen for the inbound and outbound exports to oracle' union
Select 129, 'Account Details - Create Bailif Action', 'Credit Collections', 'Account Details - Allows the user to add a follow up action through the Follow Up tab on the Account Details Screen' union
Select 133, 'Cash and Go - Instant Replacement', 'Sales', 'Cash and Go - Allows the sale of instant replacement warranties on cash and go accounts' union
Select 134, 'Cash and Go - Supashield Warranties', 'Sales', 'Cash and Go - Allows the sale of supashield/extended warranties on cash and go accounts' union
Select 131, 'Cash and Go - Warranties on Credit', 'Sales', 'Cash and Go - Enables the buy on credit check box when selling warranties on cash and go to allow for the warranty to be purchased' union
Select 170, 'Cashier Disbursements - Allow Branch Float', 'Cashier', 'Cashier Disbursements - This permissions allows users to see SAF floats in the Cashier Disbursements screen.  Note that it does not determine visiblity of the filter but if the user does not have this permission they will not see SAF transactions even with the filter selected' union
Select 330, 'Change Order Details - Before DA', 'Sales', 'Change Order Details - Allows the user to make changes to a sale through the Change Order Details screen before the account is delivery authorised' union
Select 276, 'Change Terms Type Band', 'Sales', 'New Sales Order Screen - A small button above the terms type when setting up an account' union
Select 325, 'Collection Account Analysis - Allocation', 'Credit Collections', 'Collection Account Analysis - Allows a user to allocate accounts returned in the Collection Account Analysis screen to a particular Bailiff/Collector' union
Select 75, 'Customer Details - Refer Rejected Account', 'Credit Sanctioning', 'Sanctioning - Allows the user to refer an account if it has been rejected based on the data entered when applying for credit' union
Select 329, 'Customer Search - Reopen/Un-archive account', 'Customer Functions', 'Customer Search - Enables the unsettle and unarchive options on the right click menu of an account in the Customer search screen for settled (not BDW) accounts' union
Select 275, 'Edit Terms Type Matrix Service Charge (not Points)', 'System Administration', 'Customise Scoreband Matrix - Allows a user to access and edit the Scoreband Matrix which determines the points required for a particular scoring band' union
Select 177, 'Employee Appears in Cashier Deposits', 'Cashier', 'Cashier Deposists - Determines if the employee will be shown in the Cashier Deposits Screen' union
Select 143, 'Gift Voucher - Authorise Expired Voucher', 'Payments', 'Payments - Allows a user to authorise a payment made with an expired gift voucher' union
Select 224, 'Goods Return - Authorise Warranty Cancellation', 'Sales', 'Goods Return - Allows the user to authorise the collection of a warranty delivered more than Warranty Cancellation Days (Country Parameter) ago without the collection of the parent' union
Select 58, 'Import Scoring Rules', 'System Administration', 'Customer Scoring Rules - Allows the user to import new scoring rules set' union
Select 368, 'Installation - Can Close Installation', 'Service', 'Installation - Determines if the user can authorise a saving to the resolution of an installation' union
Select 364, 'Installation - Rebook Technician', 'Service', 'Installation - Shows the Technician Booking Panel for already booked Installations as well as a ReBooking reason' union
Select 162, 'New Account - Allow Tax Exempt Sale', 'Sales', 'New Sales Order - Enables the Tax Exempt check box at the top of the New Sales Order Screen' union
Select 242, 'New Sales Order - Allow Deleted Product Sales', 'Sales', 'New Sales Order - Gives the user permission to authorise the sale of deleted products' union
Select 204, 'New Sales Order - Authorise Selling Out Of Stock Products', 'Sales', 'New Sales Order - allows users to authorise sale of out of stock merchandise and stock on order.  Note that this relates to the country parameters Password for out of stock products required and Password for stock on order required' union
Select 94, 'New Sales Order - Create Special Accounts', 'Sales', 'New Sales Order - Determines if Special is available under the account types for manual sales' union
Select 4, 'New Sales Order - Duty Free Sales', 'Sales', 'New Sales Order - Determines if Tick box at the top of the screen for duty free is available.  This is also dependant on the Duty Free country parameter' union
Select 171, 'New Sales Order - Override Discount Limit', 'Sales', 'New Sales Order - When adding a discount more than X % item value value this must be authorised' union
Select 173, 'New Sales Order - Refer Accounts > Available Spend Limit', 'Sales', 'New Sales Order - When creating an account, if you exceed the available spend limit you can refer the account instead of cancelling ' union
Select 132, 'New Sales Order - Warranties on Credit', 'Sales', 'New Sales Order - Allows a warranty to be sold on a cash account without charge so that the warranty can be charged separately on a credit account' union
Select 154, 'Open Cash Till on Log in', 'Cashier', 'Main Screen - Opens the till on login so that the float can be placed in the cash draw' union
Select 376, 'Payment - Manual Store Card Entry', 'Cashier', 'Payment - Allows for manual store card entry rather than swiping through mag stripe reader' union
Select 298, 'Payments - Cheque Authorisation', 'Cashier', 'Payment - Allows authorisation of cheque payment if customer has had more than X returned cheques in the last Y days.  This relates to the country parameters Number of allowed return cheques and Returned Cheque Period' union
Select 118, 'Payments - Credit Fee Waiver', 'Cashier', 'Payment - Gives the user permission to authorise a change in credit fee when taking a payment on account in arrears and assigned to a collector' union
Select 119, 'Payments - Credit Fee Waiver(Combined RF)', 'Cashier', 'Payment - Gives the user permission to authorise a change to credit fees when taking a payment on multiple sub RF accounts  in arrears and assigned to a collector' union
Select 7, 'Remove Customer-Account link', 'Customer Functions', 'Customer details - Allows user to link an existing customers account to different customer' union
Select 318, 'Scoring Band Change', 'Sales', 'New Sales Order - Allows the user to override the scoring band for an account in order to give a different interest rate' union
Select 367, 'Service - Cancel Service Cash Account', 'Account Functions', 'Cancel Account - Allows to cancel a service charge account for a settled service request with no charge' union
Select 357, 'Service - Previous Repair Totals', 'Service', 'Management Review Screen - Gives the user permission to view service requests where the  previous repair total has been exceeded' union
Select 363, 'Service - Reassign Technician', 'Service', 'Technician Diary - The ability to re-assign a service request to another techinician even if already assigned' union
Select 366, 'Service - Write-Off Service Cash Account', 'Service', 'Management Review Screen - Allows the user to write off a customer service charge account that has been awaiting payment more than X Months.  This relies on country parameter Months Since Service Request Resolved' union
Select 283, 'Service Request - Authorise Change to Charge To', 'Service', 'Service Request - Allows the user to change the primary charge to from the default charge to, determined by the warranty status of the item, to another charge account' union
Select 284, 'Service Request - Authorise Change to Resolution', 'Service', 'service request - Allows the user to authorise the resolution of a service request' union
Select 316, 'Service Request - Batch Print View All', 'Service', 'Batch Print - Allows the user to print Service Job sheets for all branches' union
Select 287, 'Service Request - Enable Allocation Tab', 'Service', 'service request - Enables the allocation tab so that a user can assign a Service Request to a technician' union
Select 314, 'Service Request - Estimates Update Allowed', 'Service', 'Service request - Allows the user to update previously entered repair estimates' union
Select 139, 'Status Code Maintenance - Manual Write Off', 'Account Functions', 'Status Code Maintenance - Allows the user to change the status of an account to a write off status (6, 7 or 8)' union
Select 371, 'StoreCard - Higher Level Rights', 'Account Functions', 'Store Card - Allows the user to edit the credit limit and interest rate of an existing store card account' union
Select 334, 'Tallyman 3PL Interfaces', 'End of Day', 'Malaysia Interface - Allows the user to access Malaysia Specific EOD options such as tally man, DHL and Home Club ' union
Select 189, 'Terms Type - Activate', 'System Administration', 'Terms Type Maintenance - Allows users with Edit Access to the Terms Type Maintenance Screen to activate or deactivate terms types' union
Select 210, 'Unpaid Accounts - Current User', 'Account Functions', 'Unpaid Accounts - Enables access to the Unpaid Accounts screen via the Accounts menu with only their own accounts displayed' union
Select 160, 'Update Date Due - Date First > 2 Months After Del Date', 'Account Functions', 'Update Date Due - Allows the user to update the date first to a date more than 2 months after the account reached the delivery threshold' union
Select 99, 'View Credit Score Result', 'Credit Sanctioning', 'Sanction Stage 1 - Allows the user to see the result of credit sanctioning when completing sanction stage 1' union
Select 168, 'Account Details - Show Agreement Changes', 'Account Functions', 'Account Details - Allows User access to the Agreement Changes tab on the account details screen' union
Select 215, 'Account Number Control', 'System Administration', 'Account Number Control - Allows access to the Account number control screen via the system configuration menu' union
Select 327, 'Account Status', 'Account Functions', 'Account Status - Allows user access to the Account Status Screen via the Account Menu' union
Select 2, 'Add Customer/Account Codes', 'Account Functions', 'Add codes to customer - Allows user access to theAdd codes to customer screen from various area in the system' union
Select 190, 'Allow Cashier To Do Renewal Sale', 'Sales', 'Warranty Renewals - Allows user access to the Warranty Renewals screen via the Accoutn menu or the menu option on the payments screen' union
Select 126, 'Authorise Cash And Go Return', 'Sales', 'Search Cash and go - allow the user the option to complete a Cash and go return from the right click menu of an account line on the Search Cash and Go Screen or the New Account Screen' union
Select 369, 'Authorise Delivery - View Underwriter Information', 'Credit Sanctioning', 'Authorise Delivery - When Viewing Summary for accounts, this permission will allow you to see the Underwriter Information tab' union
Select 360, 'Authorise Instant Credit', 'Credit Sanctioning', 'Authorise Instant Credit - Allows the user access to the Authorise Instant Credit Screen from the Customer Menu.' union
Select 359, 'Authorise Instant Credit - Clear Proposal', 'Credit Sanctioning', 'Authorise Instant Credit - Allows the user to have the clear proposal option in the right click menu when selecting an account in Authorise Instant Credit screen' union
Select 361, 'Authorise Instant Credit - Load Other Branches', 'Credit Sanctioning', 'Authorise Instant Credit - Allows the user access to the Branch drop down on the Authorise Instant Credit screen to choose a branch they would like to authorise instant credit for.' union
Select 136, 'Bad Debt Write Off Review', 'Credit Collections', 'Write Off Review - Allow user access to the Write off Review screen from the Credit menu' union
Select 137, 'Bad Debt Write Off Review - Accept For Write Off', 'Credit Collections', 'Write Off Review - Allow User Access to the Approve button on the write off review screen' union
Select 138, 'Bad Debt Write Off Review - Reject For Write Off', 'Credit Collections', 'Write Off Review - Allow User Access to the Decline button on the write off review screen' union
Select 217, 'Bailiff Commission Maintenance', 'Credit Collections', 'Collections Commission Maintenance - Allow user access to the collection Commission Maintenance Screen via the Collections menu' union
Select 192, 'Bailiff Review', 'Credit Collections', 'Bailiff Review - Enables access to the Bailiff Review menu option on the Credit menu' union
Select 332, 'Bailiff Review - Allocate Single Account', 'Credit Collections', 'Bailiff Review - Enables access to the Allocate Single Account tab on the Bailiff review Screen' union
Select 331, 'Bailiff Review - Override Maximum Allocation', 'Credit Collections', 'Bailiff Review - When checked the user will have access to the Override Maximum Allocation check box on the Customer/Account Information Tab of the Bailiff Review Screen' union
Select 176, 'Bailiff Review - Search By Account Number', 'Credit Collections', 'Bailiff Review - Gives the user the ability to search by account number on the Bailiff review screen' union
Select 344, 'Bailiff Review - View References', 'Credit Collections', 'Bailiff Review - gives the user access to the view References button on the Customer/Account Information Tab of the Bailiff review screen' union
Select 304, 'Bank Maintenance', 'System Administration', 'Bank Maintenance - Allows user access to the Bank Maintenance screen from the System Configuration Menu' union
Select 317, 'Behavioural Scoring Results', 'Credit Sanctioning', 'Behavioural Scoring - Allows user Access to the Behavioural Scoring Rescore report functionality' union
Select 81, 'Branch', 'System Administration', 'Branch - Allows user access to the Branch screen from the System Configuration Menu' union
Select 183, 'Branch - Bank Deposits', 'System Administration', 'Branch - Allows user access to the Bank Deposits tab of the Branch screen' union
Select 182, 'Branch - Details', 'System Administration', 'Branch - Allows user access to the Details tab of the Branch screen' union
Select 350, 'Branch - Store Card Qualification Rules', 'System Administration', 'Branch - Allows user access to the Storecard Qualification Rules tab of the Branch screen' union
Select 218, 'Calculate Bailiff Commission', 'Credit Collections', 'Calculate Bailiff Commission - Allows user access to the Calculate Bailiff Commission screen via the Credit Menu' union
Select 114, 'Cancel Account', 'Account Functions', 'Cancel Account - Allows user access to the Cancel Account screen via the Credit menu/ other screens' union
Select 64, 'Cash and Go', 'Sales', 'Cash and Go - Allows user access to the Cash and Go screen via the Account Menu' union
Select 156, 'Cash And Go - Legacy Returns', 'Sales', 'Cash and Go Returns - Allows user access to the Legacy Cash and Go Returns screen via the Account Menu' union
Select 373, 'Cash Loan Application', 'Sales', 'Cash Loan Application - Allows user access to the Cash Loan Application screen via the Customer Menu' union
Select 374, 'Cash Loan Disbursement', 'Cashier', 'Cash Loan Application - Give user access to the Disbursement button on the Cash Loan Application Screen' union
Select 158, 'Cashier Deposits', 'Cashier', 'Cashier Deposits - Allows user access to the Legacy Cash and Go Returns screen via the Account Menu' union
Select 147, 'Cashier Disbursements - Cashier Functions', 'Cashier', 'Cashier Disbursements - Enables only the functions that will be useful to the Cashiers within the Cashier Disburesements screen. All other functions will be disabled' union
Select 146, 'Cashier Disbursements - Full Control', 'Cashier', 'Cashier Disbursements - Enables all functions within the Cashier Disburesements screen.' union
Select 100, 'Cashier Totals - Print Cashier Totals By Branch', 'Cashier', 'Branch tab - Enables print icon and therefore allows user to print cashier totals from the Cashier Totals screen' union
Select 101, 'Cashier Totals - Print Cashier Totals By Employee', 'Cashier', 'Employee tab - Enables print icon and therefore allows user to print cashier totals from the Cashier Totals screen' union
Select 109, 'Cashier Totals - Save Unreconciled Cashier Totals', 'Cashier', 'Cashier Totals - Enables save icon and therefore allows user to save (with authorisation) unreconciled amounts on the Cashier Totals screen' union
Select 161, 'Cashier Totals - Summary Tab', 'Cashier', 'Cashier Totals - Allows user access to the Summary Tab on Cashier Totals screen' union
Select 319, 'Cashier Totals - View Branch Tab', 'Cashier', 'Cashier Totals - Allows user access to the Branch tab on the Cashier Totals screen' union
Select 172, 'Change Customer ID', 'Customer Functions', 'Change Customer ID - Enables the user to change the customers ID by allowing access to the change customer ID screen on the Customer menu' union
Select 179, 'Change Order Details', 'Sales', ' Change Order Details - Enables the user to see the Change Order details option on the Account menu and thus make changes to existing orders' union
Select 108, 'Check Customer Reference', 'Credit Sanctioning', 'Sanctioning - Enables the Reference Checked checkbox and shows the checked by and date checked fields on the References tab at Sanction stage 2' union
Select 106, 'Cheque Return', 'Cashier', 'Cheque Return - Allows user access to the Cheque Return screen via the Transactions menu' union
Select 79, 'Code Maintenance', 'System Administration', 'Code Maintenance - Allows access to the Code Maintenance scree nunder System Configuration menu' union
Select 97, 'Config File Maintenance', 'System Administration', 'File Menu - Allows access to the Config File Maintenance screen under the File menu of CoSACS.' union
Select 82, 'Copy Staff Allocations to Excel', 'Credit Collections', 'Collection Account Analysis - Allows user acess to the copy to excel button on the result Tab withing the collection Account Analysis screen' union
Select 151, 'Country Maintenance', 'System Administration', 'System config menu - Allows user access to the Country Maintenance screen via the System Configuration menu' union
Select 247, 'Create HP Accounts', 'Customer Functions', 'Customer record - Allows access to the Create HP Account button on the customer record screen' union
Select 62, 'Create RF Accounts', 'Customer Functions', 'Customer record - Allows access to the Create RF Account button on the customer record screen' union
Select 76, 'Credit Staff Allocation', 'Credit Collections', 'Collection Account Analysis - Allows user access to the Collection Account Analysis screen via the Credit menu' union
Select 175, 'Credit Staff Allocation - Load Other Branches', 'Credit Collections', 'Collection Account Analysis - Allows user to select different branches vis a drop down on the Collection Account Analysis screen' union
Select 236, 'Customer - Create Manual CASH Account Number', 'Customer Functions', 'Customer Record - Enables the user to select Create Manual Cash Account under the Customer Menu on the Customer Record screen' union
Select 235, 'Customer - Create Manual HP Account Number', 'Customer Functions', 'Customer Record - Enables the user to select Create Manual HP Account under the Customer Menu on the Customer Record screen' union
Select 233, 'Customer - Generate CASH Account', 'Customer Functions', 'customer record - Enables the Create Cash Account button on the customer record screen' union
Select 232, 'Customer - Generate HP Account', 'Customer Functions', 'Customer Record - Enables the Create HP Account button on the customer record screen' union
Select 234, 'Customer - Generate SPECIAL Account', 'Customer Functions', 'Customer record - Enables the user to select Generate Special Account under the Customer Menu on the Customer Record screen' union
Select 347, 'Customer - Generate Store Card Account', 'Customer Functions', 'Customer Details - Enables the create store card button on the Customer Record screen. If customers qualify for a store card user can create it.' union
Select 117, 'Customer Details - Add To', 'Customer Functions', 'Customer Record - Enables the option add to account on the rightclick of an account line on the Customer Record Screen' union
Select 178, 'Customer Details - Add To Reversal', 'Customer Functions', 'Customer Record - Enables the option add to accountReverse Add To on the rightclick of an account line on the Customer Record Screen (if there has already been an Add to performed on the account)' union
Select 222, 'Customer Details - Bank', 'Customer Functions', 'Customer Record | Bank tab - Enables the user access to the Bank Details tab on the Customer Record Screen' union
Select 223, 'Customer Details - Employment', 'Customer Functions', 'Customer Record | Employment Details tab - Enables the user access to the Employment Details tab on the Customer Record Screen' union
Select 294, 'Customer Details - Financial', 'Customer Functions', 'Customer Record |Financial Details tab - Enables the user access to the Financial Details tab on the Customer Record Screen' union
Select 295, 'Customer Details - Previous', 'Customer Functions', 'customer record | Photo/signature - Enables the previous button on the photo/signature tab of the customer record screen' union
Select 296, 'Customer Details - Residential', 'Customer Functions', 'Customer Record |Residential Details tab - Enables the user access to the Residential Details tab on the Customer Record Screen' union
Select 63, 'Customer Details - Sanction Accounts', 'Credit Sanctioning', 'Customer Record | Sanction menu - Allows user access to the Sanction menu for credit Accounts' union
Select 140, 'Customer Details - Unblock Credit', 'Credit Sanctioning', 'Customer Record | Customer menu | unblock credit - Allows user access to the Unblock Credit option in the Customer Menu' union
Select 238, 'Customer Mailing', 'Reports', 'Reports | Customer Mailing - Gives user access to the Retrieve Customers screen via the Customer Mailing option on the Reports menu' union
Select 6, 'Customer Search', 'Customer Functions', 'Main Screen - Gives user access to the customer search screen under the customer menu' union
Select 13, 'Customise Mandatory Fields', 'System Administration', 'Customise Mandatory Fields - Allows user access to the Customise Mandatory Fields screen under the System Configuration Menu' union
Select 8, 'Customise Menus', 'System Administration', 'Customise Menus - Allows user access to the Customise menu screen under System Configuration Menu' union
Select 60, 'Customise RF Scoring Matrix', 'System Administration', 'Scoring - Allows user access to the Scoring menu option (which then offers all scoring options) under System Configuration Menu' union
Select 159, 'Delete Reference Data', 'Credit Sanctioning', 'Sanction Stage 2 - Enables the - button on the References tab of Sanction Stage 2 Screen so allows user to remove references' union
Select 200, 'Delivery Area Maintenance', 'System Administration', 'Delivery Area Maintenance - Allows user access to the Delivery Area Maintenance screen under the System Configuration Menu' union
Select 88, 'Document Confirmation', 'Credit Sanctioning', 'Sanctioning - Gives user access to the Document confirmations screen within the Sanctioning process' union
Select 128, 'Document Confirmation - View Previous Details', 'Credit Sanctioning', 'Sanctioning - Allows user to view previous Document confirmation details from the Document confirmation screen' union
Select 59, 'Edit Scoring Matrix', 'System Administration', 'Customise Credit Matrices - Allows user access to the Customise Credit Matrices screen under the Scoring Menu accessed via the Systems Maintenance menu' union
Select 148, 'End Of Day', 'End of Day', 'Main Screen - Access to the End of Day menu item under the System Configuration menu' union
Select 237, 'End Of Day Configuration', 'System Administration', 'End of Day - Enables access to the End of Day sub menu option under the End of Day menu item within the System Configuration menu' union
Select 241, 'End Of Day Configuration - Delete', 'System Administration', 'End of Day - Enables the delete button on the End of day screen' union
Select 293, 'EPOS', 'Sales', 'EPOS - Allows user access to the EPOS screen under the Account Menu.' union
Select 145, 'Exchange Rates', 'System Administration', 'Exchange Rate Maintenance - Enables access to the Exchange Rate Maintenance option under the System Configuration menu' union
Select 267, 'Factoring Reports', 'Reports', 'Factoring Report - Enables access to the Factoring Reports screen via the Reports menu' union
Select 163, 'Financial Interface Report', 'Reports', 'Financial Interface Report - Enables access to the Financial Interface Report screen via the Reports menu' union
Select 191, 'Financial Transactions Query', 'Finance', 'Financial Transactions Query - Enables access to the Financial Transactions Query screen via the Transactions menu' union
Select 150, 'Flag Customer As Bankrupt', 'Customer Functions', 'Add Account/Customer Codes - Allows User the ability to flag the customer as backrupt on the Add Account/Customer Codes screen which is accessed via the Customer menu' union
Select 112, 'General Financial Transactions', 'Finance', 'General Financial Transactions - Enables access to the General Financial Transactions screen via the Transactions menu' union
Select 149, 'Gift Voucher - Allow Free', 'Payments', 'Gift Voucher - Allows user to see and make use of the free check box within the details section of the Gift Voucher screen (found under the transactions menu)' union
Select 144, 'Gift Voucher - Sell', 'Payments', 'Gift Voucher - Enables access to the Sell Gift Voucher screen via the Transactions menu' union
Select 199, 'Giro Extra Payments', 'Payments', 'Giro Extra Payments - Enables access to the Giro Extra Payments Screen screen via the Credit menu' union
Select 113, 'Giro Mandate', 'Payments', 'Giro Mandate - Enables access to the Giro Mandate Screen screen via the Credit menu' union
Select 198, 'Giro Rejections', 'Payments', 'Giro Rejections - Enables access to the Giro Rejections Screen screen via the Credit menu' union
Select 67, 'Goods Return', 'Customer Functions', 'Goods Return - Enables access to the Goods Return Screen screen via the Customer menu' union
Select 61, 'Incomplete Credit Applications', 'Credit Sanctioning', 'Incomplete Credit Applications - Enables access to the Incomplete Credit Applications screen via the Credit menu' union
Select 130, 'Incomplete Credit Applications - View Referral Details', 'Credit Sanctioning', 'Incomplete Credit Applications - Allows the user to make use of the Referral Reason drop down box on the Incomplete Credit Applications screen' union
Select 355, 'Installation - Pending Installations', 'Service', 'Pending Installations - Enables access to the Pending Installations screen via the Service menu' union
Select 356, 'Installation Booking Print', 'Service', 'Installations booking print - Enables access to the Installation booking print screen via the Service menu' union
Select 358, 'Installation Management', 'Service', 'Installation Management - Enables access to the Installation Management screen via the Service menu' union
Select 322, 'Letter Merge', 'Credit Collections', 'Letter Merge - Enables access to the Letter Merge screen via the Collections menu' union
Select 12, 'Manual Sale', 'Sales', 'Manual Sale - Enables access to the Manual Sale screen via the Account menu' union
Select 185, 'Monitor Bookings', 'Reports', 'Monitor Bookings - Enables access to the Monitor Bookings screen via the Reports menu' union
Select 221, 'Monitor Bookings - Allow Live Database', 'Reports', 'Monitor Bookings - Allows access to the Use LIVE Database check box on the Monitor bookings screen' union
Select 186, 'Monitor Outstanding Deliveries', 'Reports', 'Monitor Deliveries - Enables access to the Monitor Deliveries screen via the Reports menu' union
Select 10, 'New Customer', 'Customer Functions', 'Customer Record - Enables access to the Customer Record screen via the Customer menu in order to set up a new customer' union
Select 297, 'New Sales Order - Add Extra SPIFF', 'Sales', 'New Sales Order - Enables the user to authorise adding extra SPIFF from the right click menu of an individual item once that item is added to a new sales order' union
Select 107, 'New Sales Order - Change Branch For New Sale', 'Customer Functions', 'Customer Record - Enables the user to view and change the Branch drop down in the open new Account ara of the Customer record screen' union
Select 352, 'Non Stock Maintenance', 'System Administration', 'Non Stock Item Maintenance - Enables access to the Non Stock Item Maintenance screen via the System Configuration menu' union
Select 351, 'Non Stock Maintenance - View only', 'System Administration', 'Non Stock Item Maintenance - Allows view only access to the Non Stock Item Maintenance screen via the System Configuration menu' union
Select 180, 'Number Generation', 'Account Functions', 'Number Generation - Enables access to the Number Generation screen via the Account menu' union
Select 153, 'Open Cash Till', 'Cashier', 'payments - Anables access to the option to open the till by selecting the Cash Till | Open cash till option on the payments screen' union
Select 184, 'Overages and Shortages', 'System Administration', 'Overages and Shortages - Allows access to the Overages and Shortages (Cashiers by branch) screen via the System configuration menu' union
Select 239, 'Payment File Definition', 'System Administration', 'Payment File Definition - Enables access to the Payment File Definition screen via the system Configuration menu' union
Select 69, 'Payments', 'Cashier', 'Payments - Enables access to the Payment screen via the Transaction menu' union
Select 345, 'Payments - View Customer details', 'Cashier', 'Payements - Access to the customer details button on the Payments screen' union
Select 299, 'Prize Vouchers', 'Customer Functions', 'Prize Vouchers - Access to the Prize Vouchers screen via the Customer menu' union
Select 300, 'Prize Vouchers - Remove Existing Vouchers', 'Customer Functions', 'Prize Vouchers - Allow access to the Delete Vouchers button on the Prize Voucher screen' union
Select 301, 'Prize Vouchers - Reprint Existing Vouchers', 'Customer Functions', 'Prize Vouchers - Allow access to the Reprint Vouchers button on the Prize Voucher screen' union
Select 341, 'Provisions Screen', 'Credit Collections', 'Provisions - Enables the user to view the Provisons Screen under the System Configuration Menu. This is where the provision percentages will be set up for all customers according to Months in Arrears and Status Codes.' union
Select 197, 'Re-Print Action Sheet', 'Credit Collections', 'Re-print Debt Collectors Action Sheet - Enable access to the Re Print Debt Collectors Action Sheet screen via the Credit menu' union
Select 96, 'Referral - Override Credit Limit', 'Credit Sanctioning', 'Underwriters - If the user has this permission they can manually change the credit limit on the Underwriters screen within the sanctioning process' union
Select 105, 'Refunds and Corrections', 'Cashier', 'Refunds and Corrections - Enable access to the Refunds and Corrections screen via the Transaction menu' union
Select 110, 'Refunds and Corrections - Enter Corrections', 'Cashier', 'Refunds and Corrections - Allows user to enter Corrections on the Refunds and Corrections screen' union
Select 111, 'Refunds and Corrections - Enter Refunds', 'Cashier', 'Refunds and Corrections - Allows user to enter Refunds on the Refunds and Corrections screen' union
Select 90, 'Reopen Document Confirmation', 'Credit Sanctioning', 'Document Confirmation - This permission will allow the user access to Re-open Document Confirmation option in the document confirmation menu' union
Select 71, 'Reopen Stage 1', 'Credit Sanctioning', 'Sanction Stage 1 - this permission will allow the user access to Re-open Stage 1 option in the Sanction menu' union
Select 89, 'Reopen Stage 2', 'Credit Sanctioning', 'Sanction Stage 2 - this permission will allow the user access to Re-open Stage 2 option in the Sanction menu' union
Select 219, 'Reprint Bailiff Commission', 'Credit Collections', 'Reprint Bailiff Commission - Enables access to the Reprint Bailiff Commission screen via the Credit menu' union
Select 181, 'Reverse Cancellation', 'Account Functions', 'Reverse Cancelled Account - Enables access to the Reverse Cancelled Account screen via the Credit menu' union
Select 14, 'Revise Account', 'Account Functions', 'Revise Account - Enables access to the Revise Account screen via the Account menu' union
Select 302, 'Revise Account - Cash Loan', 'Sales', 'Customer Details | Revise Account - If an account is a Cash Loan account users with this permission will be able to revise it ' union
Select 93, 'Revise Account - Change Terms Type', 'Sales', 'Revise Sales Order - Allows the user to edit the terms type of an account when accessing it through the revise account search screen' union
Select 253, 'Revise Account - Revise Cash Accounts', 'Account Functions', 'Revise Account | Account search - Gives the user an option to revise account in the right click menu on a cash account when searched for via the revise Account search screen' union
Select 228, 'Revise Account - Revise Item Awaiting Scheduling', 'Sales', 'Revise Account - Gives the user the option to revise account in the right click menu on an account awaiting scheduling when searched for via the Revise Account search screen' union
Select 169, 'Revise Account - Revise Repossessed Accounts', 'Sales', 'Revise Account - Gives the user the option to revise account in the right click menu on an account that has a repossessed status when searched for via the Revise Account search screen' union
Select 229, 'Revise Account - Revise Scheduled Item', 'Sales', 'Revise Account - Gives the user the option to revise account in the right click menu on an account that has been scheduled when searched for via the Revise Account search screen' union
Select 152, 'Revise Account - Revise Settled Accounts', 'Sales', 'Revise Account - Gives the user the option to revise account in the right click menu on an account that is settled when searched for via the Revise Account search screen' union
Select 290, 'Sales Commission Enquiry', 'Reports', 'Sales Commission Enquiry - Enables access to the Sales Commission Enquiry screen via the Reports menu' union
Select 315, 'Sales Commission Enquiry - view all Branches', 'Reports', 'Sales Commission Enquiry - If a user has this permission they will be able to access the branch drop down box on the Sales Commission Enquiry Screen' union
Select 291, 'Sales Commission Enquiry - view all Sales Staff', 'Reports', 'Sales Commission Enquiry - If a user has this permission they will be able to access the Employee drop down box on the Sales Commission Enquiry Screen' union
Select 288, 'Sales Commission Maintenance', 'System Administration', 'Sales Commission Maintenance - Enables access to the Sales Commission Maintenance screen via the System Administration menu' union
Select 289, 'Sales Commissions - maintain Spiffs', 'System Administration', 'Sales Commission Maintenance - Gives user access to the save button and to the data entry fields on the Spiffs tab of the sales commission Maintenance screen' union
Select 85, 'Sanction Stage 1', 'Credit Sanctioning', 'Credit Santioning - Allows access to the Sanction Stage 1 screens' union
Select 83, 'Sanction Stage 1 - Manual Refer', 'Credit Sanctioning', 'Credit Santioning - Allows Access to the Manual Refer option in the Sanction menu within the Credit Sanctioning screens' union
Select 86, 'Sanction Stage 2', 'Credit Sanctioning', 'Credit Santioning - Allows access to the Sanction Stage 2 screens' union
Select 74, 'Screen Translation', 'System Administration', 'Screen Translation - Enables access to the Screen Translation screen via the System Administration menu' union
Select 78, 'Search Cash and Go', 'Account Functions', 'Search Cash and Go - Enables access to the Search Cash and Go screen via the Account menu' union
Select 378, 'Search Cash and Go - Print All', 'Account Functions', 'Search Cash and Go - Access to the Print all button on the search cash and go screen' union
Select 377, 'Search Cash and Go - Reprint Receipt', 'Account Functions', 'Search Cash and Go - Access to the Reprint Receipt option when right clicking on an account that has come up in the search window' union
Select 365, 'Service - Food Loss', 'Service', 'Service request - Enable access to the food loss button on the Service Request screen so that the user add items to the list.' union
Select 285, 'Service Request - Authorise Change to Extended Warranty', 'Service', 'Service request - Enables access to the Extended Warranty check box on the product tab of the service request screen' union
Select 286, 'Service Request - Authorise Change to Labour Cost', 'Service', 'Service - Enables fields for entry of additional labour costs (transport, labour, hourly rate, additional labour)' union
Select 309, 'Service Request - Batch Print', 'Service', 'Batch Print - Enables access to the Batch Print screen via the Service menu' union
Select 310, 'Service Request - Customer Interaction', 'Service', 'Customer Interaction - Enables access to the Customer Interaction screen via the Customer menu' union
Select 307, 'Service Request - Price Index Matrix', 'Service', 'Service Price Index Matrix - Enables access to the Service Price Index Matrix screen via the Service menu' union
Select 308, 'Service Request - Reports', 'Service', 'Service - Enables access to the Service Claims Report, Service Failure Report, Service Progress Report and Management Review Screens via the Service menu' union
Select 305, 'Service Request - Service Request & Service Request Search', 'Service', 'Service Request and Service Request Search - Enables access to the Service Request and Service Request Search Screens via the Service menu' union
Select 306, 'Service Request - Technician', 'Service', 'Technician Maintenance - Enables access to the Technician Maintenance and Technician Payment Screens via the Service menu' union
Select 348, 'Service Request - Technician Unavailable Dates', 'Service', 'Technician Diary - Enable access to the Technician Diary Screen via the service menu. This will include access to the add unavailable dates button on the technician diary screen' union
Select 280, 'Service Request - View audit trail', 'Service', 'Service Request - Enable access to the View Audit button on the Service request screen' union
Select 282, 'Service Request - View Charge To Authorisation Screen', 'Service', 'Service Request - Enables access to the Charge To Authorisation Screen via the Service menu' union
Select 279, 'Service Request Screen', 'Service', 'Service menu - Enables access to the Service Menu (at the top level)' union
Select 349, 'Setup Storecard interest Rates', 'System Administration', 'Store Card Rates setup - Enables access to the Store Card Rates setup screen under the Systems Administration menu' union
Select 323, 'SMS Setup', 'Credit Collections', 'SMS Setup - Enables access to the SMS Setup screen via the Collections menu' union
Select 80, 'Staff Maintenance', 'System Administration', 'System Configuration | Staff Maintenance - Allows user access to the Staff Maintenance Screen from the System Configuration menu' union
Select 277, 'Staff Maintenance - View Logon History', 'System Administration', 'Staff Maintenance - Allows access to the Show Logon History button on the Staff Maintenance Screen' union
Select 135, 'Status Code Maintenance', 'Account Functions', 'Status Code Maintenance - Enables access to the Status Code Maintenance screen via the Credit menu' union
Select 66, 'Stock Item Translation', 'System Administration', 'Stock Item Translation - Enables access to the Stock Item Translation screen via the System Administration menu' union
Select 321, 'Strategy Configuration', 'Credit Collections', 'Strategy Configuration/collections - When checked the user will have the ability to view strategies' union
Select 240, 'Summary Update Control Report', 'Reports', 'Summary Update Control Report - Enables access to the Summary Update Control Report screen via the Reports menu' union
Select 77, 'Telephone Action', 'Credit Collections', 'Telephone Action - Enables access to the telephone action screen via the Credit menu' union
Select 342, 'Telephone Action - Apply to all accounts', 'Credit Collections', 'Telephone Action - enables the apply to all accounts check box on the telephone actions screen' union
Select 326, 'Telephone Action - View Multiple Accounts', 'Credit Collections', 'Telephone Action - Enables the Accounts tab in the Telephone Action screen.' union
Select 343, 'Telephone Action - View References', 'Credit Collections', 'Telephone Action - Enables the view references button on the telephone actions screen' union
Select 231, 'Temporary Receipt Allocation', 'Credit Collections', 'Temporary Receipts - Enables access to the Temporary Receipt Allocation tab on the Temporary Receipts screen' union
Select 230, 'Temporary Receipt Investigation', 'Credit Collections', 'Temporary Receipts - Enables access to the Temporary Receipt Investigation tab on the Temporary Receipts screen' union
Select 202, 'Temporary Receipts', 'Credit collections', 'Temporary Receipts - Enables access to the Temporary Receipts screen via the Credit menu' union
Select 188, 'Terms Type - Edit', 'System Administration', 'Terms Type Maintenance - Enables access to the Terms Type Maintenance screen via the System Administration menu with ability to edit Terms' union
Select 187, 'Terms Type - View', 'System Administration', 'Terms Type Maintenance - Enables access to the Terms Type Maintenance screen via the System Administration menu with view only access' union
Select 142, 'Transaction Journal Enquiry', 'Finance', 'Transaction Journal Enquiry - Enables access to the Transaction Journal Enquiry screen via the Transactions menu' union
Select 115, 'Transaction Type Maintenance', 'System Administration', 'Transaction Type Maintenance - Enables access to the Transaction Type Maintenance screen via the System Administration menu' union
Select 164, 'Transaction Type Maintenance - Edit Transaction Codes', 'System Administration', 'Transaction Type Maintenance - Enables a user to edit the codes on the Transaction type Maintenance screen' union
Select 141, 'Transfer Transactions', 'Finance', 'Transfer Transactions - Enables access to the Transfer Transaction screen via the Transactions menu' union
Select 87, 'Underwriter Referral', 'Credit Sanctioning', 'Credit Santioning - With this permission the user is able to view the Under writer (UW) tab on within the sanctioning process' union
Select 212, 'Unpaid Accounts - All Branches', 'Sales', 'Unpaid Accounts - Enables access to the Unpaid Accounts screen via the Accounts menu with full access to view all branches and accounts' union
Select 211, 'Unpaid Accounts - Current Branch', 'Account Functions', 'Unpaid Accounts - Enables access to the Unpaid Accounts screen via the Accounts menu with the Branch drop down disabled so that the user may not select another branch.  This allows user to see all sales staff for this branch' union
Select 157, 'Update Date Due', 'Account Functions', 'Update Date Due - Enables access to the Update Date Due screen via the Credit menu' union
Select 3, 'View Account Details', 'Account Functions', 'Account Search - Enables access to the Account Search screen via the Account menu' union
Select 9, 'View Customer Details', 'Customer Functions', 'Customer Search - Enables access to the Customer Search screen via the Customer menu' union
Select 362, 'View Proposal from IC', 'Credit Sanctioning', 'Instant Credit Authorisation - Gives the user the ability to right click on an account and select view proposal from within the Instant Credit Authorisation screen' union
Select 370, 'View StoreCardView screen', 'Account Functions', 'Store Card - View Details - Enables access to the StoreCard - View Details screen via the Customer menu' union
Select 303, 'Warranty Reporting', 'Reports', 'Warranty Reporting - Enables access to the Warranty Reporting screen via the Reports menu' union
Select 311, 'Warranty Reporting -Live Database', 'Reports', 'Warranty Reporting - Enables access to the Live Database option button on the Warranty Reporting screen' union
Select 354, 'Warranty Return Codes Maintenance', 'System Administration', 'Warranty Return Codes Maintenance - Enables users to edit data in the Warranty Return Codes Maintenance screen' union
Select 353, 'Warranty Return Codes Maintenance - View only', 'System Administration', 'Warranty Return Codes Maintenance - - Enables View Only access to the Warranty Return Codes Maintenance screen, no changes can be made.' union
Select 320, 'Work List Setup', 'Credit Collections', 'Work List set up - Enables access to the Worklist Setup screen via the collections menu' union
Select 333, 'Zone Allocation Automation', 'Credit Collections', 'Zone Automated Allocation - Enables access to the Zone Automated Allocation screen via the Collections menu.'


DELETE FROM admin.Role

DELETE FROM admin.[User]

DELETE FROM admin.Permission

DELETE FROM admin.PermissionCategory

GO








SET IDENTITY_INSERT admin.[User] ON

GO








DBCC CHECKIDENT ('[admin].[PermissionCategory]', RESEED, 0)

GO








DBCC CHECKIDENT ('[admin].[role]', RESEED, 0)

GO








INSERT INTO admin.PermissionCategory
SELECT DISTINCT(category) FROM #temp

INSERT INTO  Admin.[User]
		( 
	      Id,
		  BranchNo ,
		  Login ,
		  PasswordExpireDate ,
		  FirstName ,
		  LastName ,
		  ExternalLogin ,
		  LegacyPassword
		)
SELECT 	empeeno,
		branchno ,
		empeeno,
		GETDATE(),
		firstname ,
		lastname, 
		FactEmployeeNo ,
		CASE
			WHEN ISNUMERIC(password) = 1 THEN [password]
			ELSE NULL
		END AS OldPassword
FROM dbo.courtsperson

SET IDENTITY_INSERT admin.[User] OFF

GO








UPDATE admin.[User]
SET BranchNo = hobranchno
FROM dbo.country
WHERE NOT EXISTS (SELECT 1 
				  FROM dbo.branch
				  WHERE branchno = admin.[User].BranchNo)

SELECT id = IDENTITY(INT,1,1),code,codedescript AS NAME, code AS oldCode
INTO #roles
FROM dbo.code
WHERE category = 'ET1'


UPDATE #roles
SET Name = code 
WHERE Name = ''

GO









UPDATE #roles
SET Name = Name + CONVERT(VARCHAR(100),(SELECT CASE WHEN 1=1 THEN (SELECT COUNT(*) FROM #roles a 
										    WHERE a.Name = #roles.Name 
										    AND a.ID < #roles.ID) END ))

UPDATE #roles
SET Name = CASE WHEN SUBSTRING(#roles.Name,LEN(#roles.Name) , 1) != 0 
           THEN #roles.Name
           ELSE SUBSTRING(#roles.Name,0 , LEN(#roles.Name) ) END



INSERT INTO Admin.Role
		( Name )
SELECT name 
FROM #roles


INSERT INTO Admin.UserRole
		( RoleId, UserId )
SELECT r.Id,u.Id 
FROM dbo.courtsperson c
INNER JOIN #roles ON #roles.oldCode = c.empeetype
INNER JOIN Admin.Role R ON R.Name = #roles.NAME
INNER JOIN Admin.[User] U ON c.empeeno = U.Id


SET IDENTITY_INSERT admin.Permission ON
GO



INSERT INTO admin.Permission
		( 
		  Id,
		  Name ,
		  CategoryId ,
		  Description 
		)
SELECT  t.id,t.name,pc.Id,Description 
FROM #temp t
INNER JOIN admin.PermissionCategory pc ON t.category = pc.Name

SET IDENTITY_INSERT admin.Permission OFF
GO


DECLARE @permax INT
SELECT @permax = MAX(id) FROM admin.Permission
DBCC CHECKIDENT ('admin.Permission', RESEED, @permax)
GO


DECLARE @usermax INT
SELECT @usermax = MAX(id) FROM admin.[User]
DBCC CHECKIDENT ('admin.[User]', RESEED,@usermax)
GO


INSERT INTO Admin.RolePermission
        ( RoleId, PermissionId,[Deny] )
SELECT distinct ar.Id, p.Id,0 
FROM dbo.RolePermissions 
INNER JOIN #roles TR ON tr.oldCode = dbo.RolePermissions.Role
INNER JOIN Admin.Role ar ON tr.NAME = ar.Name
INNER JOIN task ON dbo.RolePermissions.TaskID = dbo.Task.TaskID
INNER JOIN Admin.Permission p ON task.TaskName = p.Name	


update t
SET empeetype = r.id
FROM dbo.bailcommnbas t
INNER JOIN #roles ON t.empeetype = #roles.oldcode
INNER JOIN admin.ROLE r ON r.NAME = #roles.name

update t
SET empeetype = r.id
FROM dbo.CMWorkListSortOrder t
INNER JOIN #roles ON t.empeetype = #roles.oldcode
INNER JOIN admin.ROLE r ON r.NAME = #roles.NAME

update t
SET empeetype = r.id
FROM dbo.CMBailiffAllocationRules t
INNER JOIN #roles ON t.empeetype = #roles.oldcode
INNER JOIN admin.ROLE r ON r.NAME = #roles.NAME

update t
SET empeetype = r.id
FROM dbo.commnbasis t
INNER JOIN #roles ON t.empeetype = #roles.oldcode
INNER JOIN admin.ROLE r ON r.NAME = #roles.NAME

update t
SET empeetype = r.id
FROM dbo.CollectionCommnRules t
INNER JOIN #roles ON t.empeetype = #roles.oldcode
INNER JOIN admin.ROLE r ON r.NAME = #roles.NAME

update t
SET empeetype = r.id
FROM dbo.CollectionCommnRuleAudit t
INNER JOIN #roles ON t.empeetype = #roles.oldcode
INNER JOIN admin.ROLE r ON r.NAME = #roles.NAME

update t
SET empeetype = r.id
FROM dbo.CollectionCommnAccts t
INNER JOIN #roles ON t.empeetype = #roles.oldcode
INNER JOIN admin.ROLE r ON r.NAME = #roles.NAME

update t
SET empeetype = r.id
FROM dbo.CollectionCommn t
INNER JOIN #roles ON t.empeetype = #roles.oldcode
INNER JOIN admin.ROLE r ON r.NAME = #roles.NAME


DROP TABLE #roles
GO

DROP TABLE #temp

GO