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
		<TABLE id="Table1" width="700">
			<TR>
				<TD>
					<table width="100%" cellpadding="3">
						<tr class="RFHead1">
							<td>
								Cashier Cheque Listing Sheet
							</td>
						</tr>
						<tr class="RFHead2">
							<td>
								<br/>
								<br/>
								Date: <xsl:value-of select="DATEFROM" />
							</td>
						</tr>
					</table>
					<table width="100%" cellpadding="3">
						<tr class="RFHead1">
							<td>
								<br/>
								Employee Number: <xsl:value-of select="EMPLOYEE" />
							</td>
							<td>
								<br/>
								Employee Name: <xsl:value-of select="EMPLOYEENAME" />
							</td>
						</tr>
					</table>
					<table width="100%" cellpadding="3">
						<tr class="RFHead2">
							<td width="16.66%">
								<br/>
								<u>Account Number</u>
							</td>
							<td width="16.66%">
								<br/>
								<u>Customer Name</u>
							</td>
							<td width="16.66%">
								<br/>
								<u>Amount</u>
							</td>
							<td width="16.66%">
								<br/>
								<u>Cheque Number</u>
							</td>
							<td width="16.66%">
								<br/>
								<u>Bank Code</u>
							</td>
							<td width="16.66%">
								<br/>
								<u>Bank A/C Number</u>
							</td>
						</tr>
					</table>
					
					<xsl:apply-templates select="TRANSACTIONS" />
					
					<table width="100%" cellpadding="3" ID="Table3">
						<tr class="RFHead1">
							<td width="50%">
								<br/>
								<br/>
								<br/>
								Cheque Total: <xsl:value-of select="TOTALVALUE" />
							</td>
							<td width="50%">
								<br/>
								<br/>
								<br/>
								Cashier Signature:
							</td>
						</tr>
					</table>
				</TD>
			</TR>
		</TABLE>
	</xsl:template>
	
	<xsl:template match="TRANSACTIONS">
		<table width="100%" cellpadding="3" ID="Table2">
			<xsl:apply-templates select="TRANSACTION" />
		</table>	
	</xsl:template>
	
	<xsl:template match="TRANSACTION">
		<tr class="RFBody">
			<td width="16.66%">
				<xsl:value-of select="ACCTNO" />
			</td>
			<td width="16.66%">
				<xsl:value-of select="CUSTNAME" />
			</td>
			<td width="16.66%">
				<xsl:value-of select="format-number(TRANSVALUE, '#,##0.00')" />
			</td>
			<td width="16.66%">
				<xsl:value-of select="CHEQUENO" />
			</td>
			<td width="16.66%">
				<xsl:value-of select="BANKCODE" />
			</td>
			<td width="16.66%">
				<xsl:value-of select="BANKACCTNO" />
			</td>
		</tr>
	</xsl:template>	
</xsl:stylesheet>

  