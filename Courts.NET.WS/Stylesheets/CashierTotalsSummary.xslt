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
				<TD class="RFHead2" width="50%">CASHIER DAILY TOTALS SUMMARY REPORT</TD>
			</TR>
		</TABLE>
		<P></P>
		<TABLE class="RFBody" width="700" border="0">
			<TR>
				<TD width="20%">BRANCH No: <xsl:value-of select="BRANCH" /></TD>
				<TD width="40%">From: <xsl:value-of select="DATEFROM" /></TD>
				<TD width="40%">To: <xsl:value-of select="DATETO" /></TD>				
			</TR>
		</TABLE>
		<P></P>
		<TABLE class="normal" width="700" border="0">
			<TR>
				<TD class="RFHead2" width="20%">Pay Method</TD>
				<TD class="RFHead2" width="16%" align="right">System</TD>
				<TD class="RFHead2" width="16%" align="right">User</TD>
				<TD class="RFHead2" width="16%" align="right">Deposit</TD>
				<TD class="RFHead2" width="16%" align="right">Difference</TD>
				<TD class="RFHead2" width="16%" align="right">A.S.</TD>
			</TR>
			<p></p>
			<xsl:apply-templates select="PAYMETHODS" />
			<p></p>
			<TR>
				<TD class="RFHead2" width="20%">Totals:</TD>
				<TD class="RFHead2" width="16%" align="right"><xsl:value-of select="SYSTEMTOTAL" /></TD>
				<TD class="RFHead2" width="16%" align="right"><xsl:value-of select="USERTOTAL" /></TD>
				<TD class="RFHead2" width="16%" align="right"><xsl:value-of select="DEPOSITTOTAL" /></TD>
				<TD class="RFHead2" width="16%" align="right"><xsl:value-of select="DIFFERENCETOTAL" /></TD>
				<TD class="RFHead2" width="16%" align="right"><xsl:value-of select="SECURITISEDTOTAL" /></TD>
			</TR>
		</TABLE>
	</xsl:template>	
	
	<xsl:template match="PAYMETHODS">
		<xsl:apply-templates select="PAYMETHOD" />
	</xsl:template>
	
	<xsl:template match="PAYMETHOD">
		<TR>
			<TD class="RFBody"><xsl:value-of select="NAME" /></TD>
			<TD class="RFBody" align="right"><xsl:value-of select="SYSTEMVALUE" /></TD>
			<TD class="RFBody" align="right"><xsl:value-of select="USERVALUE" /></TD>
			<TD class="RFBody" align="right"><xsl:value-of select="DEPOSITVALUE" /></TD>
			<TD class="RFBody" align="right"><xsl:value-of select="DIFFERENCEVALUE" /></TD>
			<TD class="RFBody" align="right"><xsl:value-of select="SECURITISEDVALUE" /></TD>
		</TR>
	</xsl:template>

</xsl:stylesheet>
