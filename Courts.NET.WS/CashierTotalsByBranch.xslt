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
			<div class="CashierHeader" style="BORDER-RIGHT: black thin solid; PADDING-RIGHT: 5px; BORDER-TOP: black thin solid; PADDING-LEFT: 5px; LEFT: 1.9cm; PADDING-BOTTOM: 5px; BORDER-LEFT: black thin solid; WIDTH: 11.8cm; PADDING-TOP: 5px; BORDER-BOTTOM: black thin solid; POSITION: absolute; TOP: 1.9cm; HEIGHT: 1cm; BACKGROUND-COLOR: silver">
				<xsl:value-of select="BRANCH" /> Transactions By Employee
			</div>
			<div class="CashierHeader" style="BORDER-RIGHT: black thin solid; PADDING-RIGHT: 5px; BORDER-TOP: black thin solid; PADDING-LEFT: 5px; LEFT: 13.7cm; PADDING-BOTTOM: 5px; BORDER-LEFT: black thin; WIDTH: 3cm; PADDING-TOP: 5px; BORDER-BOTTOM: black thin solid; POSITION: absolute; TOP: 1.9cm; HEIGHT: 1cm; BACKGROUND-COLOR: silver">Sub 
				Total
			</div>
			<div class="RFHead2" style="BORDER-RIGHT: black thin; PADDING-RIGHT: 0px; BORDER-TOP: black thin; PADDING-LEFT: 0px; LEFT: 2cm; PADDING-BOTTOM: 0px; BORDER-LEFT: black thin; WIDTH: 5cm; PADDING-TOP: 0px; BORDER-BOTTOM: black thin; POSITION: absolute; TOP: 3.6cm; HEIGHT: 0.5cm; BACKGROUND-COLOR: white">BRANCH 
				NO. <xsl:value-of select="BRANCH" />
			</div>
			<div class="RFHead2" style="BORDER-RIGHT: black thin; PADDING-RIGHT: 0px; BORDER-TOP: black thin; PADDING-LEFT: 0px; LEFT: 8cm; PADDING-BOTTOM: 0px; BORDER-LEFT: black thin; WIDTH: 6cm; PADDING-TOP: 0px; BORDER-BOTTOM: black thin; POSITION: absolute; TOP: 3.2cm; HEIGHT: 0.5cm; BACKGROUND-COLOR: white">From: 
				<xsl:value-of select="DATEFROM" />
			</div>
			<div class="RFHead2" style="BORDER-RIGHT: black thin; PADDING-RIGHT: 0px; BORDER-TOP: black thin; PADDING-LEFT: 0px; LEFT: 8cm; PADDING-BOTTOM: 0px; BORDER-LEFT: black thin; WIDTH: 6cm; PADDING-TOP: 0px; BORDER-BOTTOM: black thin; POSITION: absolute; TOP: 3.7cm; HEIGHT: 0.5cm; BACKGROUND-COLOR: white">To: 
				<xsl:value-of select="DATETO" />
			</div>
			<div style="POSITION: absolute; LEFT: 3cm; TOP: 5.6cm; HEIGHT: 0.5cm; WIDTH: 9cm">
				<table bgcolor="#cccccc" cellspacing="0" cellpadding="2" border="1" class="RFHead2" width="100%">
					<tr> 
						<td width="50%">
							Employee No
						</td>
						<td width="50%">
							Receipt
						</td>
					</tr>
				</table>
			</div>

			<div class="smallSS" style="LEFT: 3cm; WIDTH: 9cm; POSITION: absolute; TOP: 6.1cm">
				<xsl:apply-templates select="TRANSACTIONS" />
			</div>

		</div>
	</xsl:template>
	
	<xsl:template match="TRANSACTIONS">
		<table border="1px" cellspacing="0" cellpadding="2" class="smallSS" width="100%">			
			<xsl:apply-templates select="TRANSACTION" />
		</table>
		<table bgcolor="#cccccc" cellspacing="0" cellpadding="2" border="1" class="RFHead2" width="100%">
			<tr> 
				<td width="50%">
					NET RECEIPTS
				</td>
				<td width="50%">
					<xsl:value-of select="../TOTALVALUE" />
				</td>
			</tr>
		</table>		
	</xsl:template>
	
	<xsl:template match="TRANSACTION">
		<tr>
			<td width="50%">
				<xsl:value-of select="EMPLOYEE" />
			</td>
			<td width="50%">
				<xsl:value-of select="format-number(TRANSVALUE, '#,##0.00')" />
			</td>
		</tr>		
	</xsl:template>
	
</xsl:stylesheet>

  