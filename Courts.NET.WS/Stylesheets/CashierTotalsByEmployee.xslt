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
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD class="RFHead2" width="50%">CASHIER'S DAILY RECONCILIATION</TD>
				<TD class="RFHead2" width="50%">Total</TD>
			</TR>
		</TABLE>
		<P></P>
		<TABLE class="normal" width="800" border="0">
			<TR>
				<TD width="50%">EMPLOYEE:<xsl:value-of select="EMPLOYEE" /></TD>
				<TD width="50%">From:<xsl:value-of select="DATEFROM" /></TD>
			</TR>
			<TR>
				<TD width="50%">BRANCH No:<xsl:value-of select="BRANCH" /></TD>
				<TD width="50%">To:<xsl:value-of select="DATETO" /></TD>
			</TR>
		</TABLE>
		<P></P>
		<TABLE class="normal" height="7" width="700" border="0">
			<TR>
				<TD class="RFHead2" width="25%"></TD>
				<TD class="RFHead2" width="13%">System</TD>
				<TD class="RFHead2" width="13%">User</TD>
				<TD class="RFHead2" width="13%">Deposit</TD>
				<TD class="RFHead2" width="13%">Difference</TD>
				<TD class="RFHead2">Reason</TD>
			</TR>
			<xsl:apply-templates select="TOTALVALS" />
			<TR>
				<TD class="RFHead2">TOTAL PAYMENTS</TD>
				<TD class="RFBody"><xsl:value-of select="format-number(sum(//TRANSACTION[TRANSTYPE='PAY']/TRANSVALUE), '#,##0.00')" /></TD>
				<TD></TD>
				<TD></TD>
			</TR>
			<TR>
				<TD class="RFHead2">CORRECTIONS</TD>
				<TD class="RFBody"><xsl:value-of select="format-number(sum(//TRANSACTION[TRANSTYPE='COR']/TRANSVALUE), '#,##0.00')" /></TD>
				<TD></TD>
				<TD></TD>
			</TR>
			<TR>
				<TD class="RFHead2">REFUNDS</TD>
				<TD class="RFBody"><xsl:value-of select="format-number(sum(//TRANSACTION[TRANSTYPE='REF']/TRANSVALUE), '#,##0.00')" /></TD>
				<TD></TD>
				<TD></TD>
			</TR>
			<TR>
				<TD class="RFHead2">OTHER</TD>
				<TD class="RFBody"><xsl:value-of select="format-number(sum(//TRANSACTION[TRANSTYPE!='PAY' and TRANSTYPE!='COR' and TRANSTYPE!='REF']/TRANSVALUE), '#,##0.00')" /></TD>
				<TD></TD>
				<TD></TD>
			</TR>
			<TR>
				<TD class="RFHead2">NET RECEIPTS</TD>
				<TD class="RFBody"><xsl:value-of select="format-number(sum(//TRANSACTION/TRANSVALUE), '#,##0.00')" /></TD>
				<TD></TD>
				<TD></TD>
			</TR>
		</TABLE>
	</xsl:template>	
	
	<xsl:template match="TOTALVALS">
		<xsl:apply-templates select="TOTAL" />
	</xsl:template>
	
	<xsl:template match="TOTAL">
		<TR>
			<TD class="RFBody"><xsl:value-of select="DESCRIPTION" /></TD>
			<TD class="RFBody"><xsl:value-of select="SYSTEMTOTAL" /></TD>
			<TD class="RFBody"><xsl:value-of select="USERTOTAL" /></TD>
			<TD class="RFBody"><xsl:value-of select="DEPOSIT" /></TD>
			<TD class="RFBody"><xsl:value-of select="DIFFERENCE" /></TD>
			<TD class="RFBody"><xsl:value-of select="REASON" /></TD>
		</TR>
	</xsl:template>

</xsl:stylesheet>
