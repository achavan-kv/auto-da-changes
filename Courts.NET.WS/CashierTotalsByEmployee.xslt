<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<head>
				<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"></meta>
				<style type="text/css" media="all">
					@import url(styles.css);
				</style>
			</head>
			<BODY>	
				<xsl:apply-templates select="CASHIERTOTALS" />
			</BODY>
		</HTML>		
	</xsl:template>
	
	<xsl:template match="CASHIERTOTALS">
		<div style="POSITION: relative">
			<div style="BORDER-RIGHT: black 1px solid; PADDING-RIGHT: 3px; BORDER-TOP: black 1px solid; PADDING-LEFT: 3px; FONT-WEIGHT: bold; FONT-SIZE: 16pt; LEFT: 1cm; PADDING-BOTTOM: 3px; BORDER-LEFT: black 1px solid; WIDTH: 12cm; PADDING-TOP: 3px; BORDER-BOTTOM: black 1px solid; FONT-FAMILY: Arial, Tahoma; POSITION: absolute; TOP: 2cm; HEIGHT: 0.9cm; BACKGROUND-COLOR: #cccccc">CASHIER'S 
				DAILY RECONCILIATION
			</div>
			<div style="BORDER-RIGHT: black 1px solid; PADDING-RIGHT: 3px; BORDER-TOP: black 1px solid; PADDING-LEFT: 3px; FONT-WEIGHT: bold; FONT-SIZE: 16pt; LEFT: 13cm; PADDING-BOTTOM: 3px; BORDER-LEFT: black 1px solid; WIDTH: 3cm; PADDING-TOP: 3px; BORDER-BOTTOM: black 1px solid; FONT-FAMILY: Arial, Tahoma; POSITION: absolute; TOP: 2cm; HEIGHT: 0.9cm; BACKGROUND-COLOR: #cccccc">
				<xsl:if test="SUBTOTAL = 'False'">
						Total
					</xsl:if>
				<xsl:if test="SUBTOTAL = 'True'">
						Subtotal
					</xsl:if>
			</div>
			<div class="RFHead1" style="BORDER-TOP-WIDTH: 1px; PADDING-RIGHT: 3px; PADDING-LEFT: 3px; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: black; LEFT: 1cm; BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: black; PADDING-BOTTOM: 3px; WIDTH: 6cm; BORDER-TOP-COLOR: black; PADDING-TOP: 3px; POSITION: absolute; TOP: 3.3cm; HEIGHT: 1cm; BORDER-RIGHT-WIDTH: 1px; BORDER-RIGHT-COLOR: black">
				EMPLOYEE No.
				<xsl:value-of select="EMPLOYEE" />
				<br/>
				BRANCH No.
				<xsl:value-of select="BRANCH" />
			</div>
			<div class="RFHead1" style="BORDER-TOP-WIDTH: 1px; PADDING-RIGHT: 3px; PADDING-LEFT: 3px; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: black; LEFT: 9.5cm; BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: black; PADDING-BOTTOM: 3px; WIDTH: 6cm; BORDER-TOP-COLOR: black; PADDING-TOP: 3px; POSITION: absolute; TOP: 3.3cm; HEIGHT: 1cm; BORDER-RIGHT-WIDTH: 1px; BORDER-RIGHT-COLOR: black">
				From:
				<xsl:value-of select="DATEFROM" />
				<br/>
				To:
				<xsl:value-of select="DATETO" />
			</div>
			<div class="RFHead1" style="BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; Z-INDEX: 1; LEFT: 1cm; BORDER-LEFT: black 1px solid; WIDTH: 5.5cm; BORDER-BOTTOM: black 1px solid; POSITION: absolute; TOP: 4.5cm; HEIGHT: 0.5cm; BACKGROUND-COLOR: #cccccc; TEXT-ALIGN: center">RECEIPTS</div>
			<div class="RFHead1" style="BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: black; LEFT: 6.5cm; WIDTH: 2.5cm; BORDER-BOTTOM: black 1px solid; POSITION: absolute; TOP: 4.5cm; HEIGHT: 0.5cm; BACKGROUND-COLOR: #cccccc; TEXT-ALIGN: center"></div>
			<div class="RFHead1" style="BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; Z-INDEX: 1; LEFT: 9.5cm; BORDER-LEFT: black 1px solid; WIDTH: 5.5cm; BORDER-BOTTOM: black 1px solid; POSITION: absolute; TOP: 4.5cm; HEIGHT: 0.5cm; BACKGROUND-COLOR: #cccccc; TEXT-ALIGN: center">BANKING</div>
			<div class="RFHead1" style="BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: black; LEFT: 15cm; WIDTH: 2.5cm; BORDER-BOTTOM: black 1px solid; POSITION: absolute; TOP: 4.5cm; HEIGHT: 0.5cm; BACKGROUND-COLOR: #cccccc; TEXT-ALIGN: center"></div>
			<div class="RFHead1" style="BORDER-TOP-WIDTH: 1px; BORDER-RIGHT: black 1px solid; LEFT: 6.5cm; BORDER-LEFT: black 1px solid; WIDTH: 2.5cm; BORDER-TOP-COLOR: black; BORDER-BOTTOM: black 1px solid; POSITION: absolute; TOP: 5cm; HEIGHT: 6cm; TEXT-ALIGN: center"></div>
			<div class="RFHead1" style="BORDER-TOP-WIDTH: 1px; BORDER-RIGHT: black 1px solid; LEFT: 15cm; BORDER-LEFT: black 1px solid; WIDTH: 2.5cm; BORDER-TOP-COLOR: black; BORDER-BOTTOM: black 1px solid; POSITION: absolute; TOP: 5cm; HEIGHT: 3.5cm; TEXT-ALIGN: center"></div>
			<div class="RFHead1" style="BORDER-TOP-WIDTH: 1px; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: black; LEFT: 1.3cm; BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: black; WIDTH: 5cm; BORDER-TOP-COLOR: black; POSITION: absolute; TOP: 5.3cm; HEIGHT: 10cm; TEXT-ALIGN: left; BORDER-RIGHT-WIDTH: 1px; BORDER-RIGHT-COLOR: black">
				CASH<br/>
				<br/>
				CHEQUE<br/>
				<br/>
				CREDIT CARD<br/>
				<br/>
				DEBIT CARD<br/>
				<br/>
				STANDING ORDER<br/>
				<br/>
				WIRELESS TRANSFER<br/>
				<br/>
				ATM<br/>
				<br/>
				TOTAL PAYMENTS<br/>
				<br/>
				CORRECTIONS<br/>
				<br/>
				REFUNDS<br/>
				<br/>
				OTHER<br/>
				<br/>
				NET RECEIPTS<br/>
				<br/>
				<br/>
				<br/>
				<br/>
				<br/>
				REASON FOR DIFFERENCE
			</div>
			<div class="RFHead1" style="BORDER-TOP-WIDTH: 1px; BORDER-RIGHT: black 1px solid; LEFT: 6.5cm; BORDER-LEFT: black 1px solid; WIDTH: 2.5cm; BORDER-TOP-COLOR: black; BORDER-BOTTOM: black 1px solid; POSITION: absolute; TOP: 11cm; HEIGHT: 3.5cm"></div>
			<div class="RFHead1" style="BORDER-TOP-WIDTH: 1px; BORDER-RIGHT: black 1px solid; LEFT: 15cm; BORDER-LEFT: black 1px solid; WIDTH: 2.5cm; BORDER-TOP-COLOR: black; BORDER-BOTTOM: black 1px solid; POSITION: absolute; TOP: 8.5cm; HEIGHT: 3.5cm"></div>
			<div class="RFHead1" style="BORDER-TOP-WIDTH: 1px; BORDER-RIGHT: black 1px solid; LEFT: 6.5cm; BORDER-LEFT: black 1px solid; WIDTH: 2.5cm; BORDER-TOP-COLOR: black; BORDER-BOTTOM: black 1px solid; POSITION: absolute; TOP: 14.5cm; HEIGHT: 0.6cm"></div>
			<div class="RFHead1" style="BORDER-TOP-WIDTH: 1px; BORDER-RIGHT: black 1px solid; LEFT: 15.001cm; BORDER-LEFT: black 1px solid; WIDTH: 2.5cm; BORDER-TOP-COLOR: black; BORDER-BOTTOM: black 1px solid; POSITION: absolute; TOP: 11.986cm; HEIGHT: 0.6cm"></div>
			<div class="RFHead1" style="BORDER-TOP: black 1px solid; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: black; LEFT: 15.001cm; WIDTH: 2.5cm; BORDER-BOTTOM: black 1px solid; POSITION: absolute; TOP: 12.86cm; HEIGHT: 0.5cm; BORDER-RIGHT-WIDTH: 1px; BORDER-RIGHT-COLOR: black"></div>
			<div class="RFHead1" style="BORDER-TOP-WIDTH: 1px; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: black; LEFT: 6.8cm; BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: black; WIDTH: 0.002cm; BORDER-TOP-COLOR: black; POSITION: absolute; TOP: 5.3cm; HEIGHT: 0.004cm; TEXT-ALIGN: right; BORDER-RIGHT-WIDTH: 1px; BORDER-RIGHT-COLOR: black" align="right">
				<xsl:value-of select="format-number(sum(//TRANSACTION[PAYMETHOD='1']/TRANSVALUE), '#,##0.00')" /><br/>
				<br/>
				<xsl:value-of select="format-number(sum(//TRANSACTION[PAYMETHOD='2']/TRANSVALUE), '#,##0.00')" /><br/>
				<br/>
				<xsl:value-of select="format-number(sum(//TRANSACTION[PAYMETHOD='3']/TRANSVALUE), '#,##0.00')" /><br/>
				<br/>
				<xsl:value-of select="format-number(sum(//TRANSACTION[PAYMETHOD='4']/TRANSVALUE), '#,##0.00')" /><br/>
				<br/>
				<xsl:value-of select="format-number(sum(//TRANSACTION[PAYMETHOD='5']/TRANSVALUE), '#,##0.00')" /><br/>
				<br/>
				<xsl:value-of select="format-number(sum(//TRANSACTION[PAYMETHOD='6']/TRANSVALUE), '#,##0.00')" /><br/>
				<br/>
				<xsl:value-of select="format-number(sum(//TRANSACTION[PAYMETHOD='7']/TRANSVALUE), '#,##0.00')" /><br/>
				<br/>
				<xsl:value-of select="format-number(sum(//TRANSACTION[TRANSTYPE='PAY']/TRANSVALUE), '#,##0.00')" /><br/>
				<br/>
				<xsl:value-of select="format-number(sum(//TRANSACTION[TRANSTYPE='COR']/TRANSVALUE), '#,##0.00')" /><br/>
				<br/>
				<xsl:value-of select="format-number(sum(//TRANSACTION[TRANSTYPE='REF']/TRANSVALUE), '#,##0.00')" /><br/>
				<br/>
				<xsl:value-of select="format-number(sum(//TRANSACTION[TRANSTYPE!='PAY' and TRANSTYPE!='COR' and TRANSTYPE!='REF']/TRANSVALUE), '#,##0.00')" /><br/>
				<br/>
				<xsl:value-of select="format-number(sum(//TRANSACTION/TRANSVALUE), '#,##0.00')" />
			</div>
			<div class="RFHead1" style="BORDER-TOP-WIDTH: 1px; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: black; LEFT: 9.8cm; BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: black; WIDTH: 5cm; BORDER-TOP-COLOR: black; POSITION: absolute; TOP: 5.3cm; HEIGHT: 8cm; TEXT-ALIGN: left; BORDER-RIGHT-WIDTH: 1px; BORDER-RIGHT-COLOR: black">CASH<br/>
				NOTES<br/>
				<br/>
				<br/>
				<br/>
				COINS<br/>
				FOREIGN CURRENCY
				<br/>
				<br/>
				TOTAL CASH<br/>
				<br/>
				CHEQUES<br/>
				<br/>
				CREDIT CARD<br/>
				<br/>
				DEBIT CARD<br/>
				<br/>
				NET TAKINGS<br/>
				<br/>
				DIFFERENCE
			</div>
		</div>
	</xsl:template>	
</xsl:stylesheet>

  