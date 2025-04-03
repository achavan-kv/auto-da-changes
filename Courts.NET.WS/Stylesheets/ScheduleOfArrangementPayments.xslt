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
				<xsl:apply-templates select="SCHEDULEOFPAYMENTS" />
			</BODY>
		</HTML>
	</xsl:template>

	<xsl:template match="SCHEDULEOFPAYMENTS">
		<xsl:apply-templates select="HEADER" />
		<p></p>
		<table width="700">
			<tr class="RFHead1" >
				<td width="14%" align="center">Month</td>
				<td width="14%" align="center">Due Date</td>
				<td width="14%" align="center">Arrangement Instalment</td>
				<!--<td width="14%" align="center">Charges</td>
				<td width="14%" align="center">Capital</td>-->
				<td width="14%" align="center">Total Payment</td>
				<!--<td width="14%" align="center">Closing Balance</td>-->
			</tr>
			<xsl:apply-templates select="INSTALMENTS" />
		</table>
		<p></p>
		<xsl:apply-templates select="FOOTER" />
	</xsl:template>

	<xsl:template match="HEADER">
		<table width="700" class="RFHead1">
			<tr>
				<td>
					<xsl:value-of select="DEALERNAME" />
				</td>
				<td>SCHEDULE OF ARRANGEMENT PAYMENTS</td>
				<td>
					Print Date: <xsl:value-of select="DATE" />
				</td>
			</tr>
		</table>
		<p></p>
		<table width="700">
			<tr>
				<td class="RFHead2" width="20%">Name of Dealer:</td>
				<td class="RFBody" width="80%">
					<xsl:value-of select="DEALERNAME" />
				</td>
			</tr>
			<tr>
				<td class="RFHead2" >Name of Hirer:</td>
				<td class="RFBody">
					<xsl:value-of select="CUSTOMERNAME" />
				</td>
			</tr>
			<tr>
				<td class="RFHead2" >Account Number:</td>
				<td class="RFBody">
					<xsl:value-of select="ACCTNO" />
				</td>
			</tr>
		</table>
		<p></p>
		<table width="700">
			<tr>
				<td class="RFHead2" width="20%">Goods:</td>
				<td width="10%"></td>
				<td></td>
			</tr>
			<xsl:apply-templates select="LINEITEMS" />
		</table>
		<p></p>
		<table width="700">
      <!--IP - 04/10/10 UAT(983)UAT5.2 Do not need to display APR-->
			<!--<tr>
				<td class="RFHead2" width="20%">Annual Percentage Rate:</td>
				<td class="RFBody">
					<xsl:value-of select="APR" />%
				</td>
			</tr>-->
			<tr>
				<td class="RFHead2" width ="20%" >Number of Instalments:</td>
				<td class="RFBody">
					<xsl:value-of select="NOINSTALMENTS" />
				</td>
			</tr>
			<tr>
				<td class="RFHead2" width ="20%">Repayment Period:</td>
				<td class="RFBody">
					<xsl:value-of select="FIRSTDATE" /> to <xsl:value-of select="LASTDATE" />
				</td>
			</tr>
      <!--IP - 04/05/10 - UAT(983)UAT5.2 Do not display Chargeable Price-->
			<!--<tr>
				<td class="RFHead2" >Chargeable Price:</td>
				<td class="RFBody" >Cash Price - down Payment</td>
			</tr>
			<tr>
				<td></td>
				<td class="RFBody">
					<xsl:value-of select="CHARGEABLEPRICE" />
				</td>
			</tr>-->
		</table>
	</xsl:template>

	<xsl:template match="INSTALMENTS">
		<xsl:apply-templates select="INSTALMENT" />
	</xsl:template>

	<xsl:template match="INSTALMENT">
		<tr class="RFBody" >
			<td width="14%" align="center">
				<xsl:value-of select="NO" />
			</td>
			<td width="14%" align="center">
				<xsl:value-of select="DUEDATE" />
			</td>
			<td width="14%" align="center">
				<xsl:value-of select="MONTHLYINSTALMENT" />
			</td>
			<!--<td width="14%" align="center">
				<xsl:value-of select="MONTHLYCHARGE" />
			</td>
			<td width="14%" align="center">
				<xsl:value-of select="MONTHLYCAPITAL" />
			</td>-->
			<td width="14%" align="center">
				<xsl:value-of select="TOTALPAYMENT" />
			</td>
			<!--<td width="14%" align="center">
				<xsl:value-of select="CLOSINGBALANCE" />
			</td>-->
		</tr>
	</xsl:template>

	<xsl:template match="FOOTER">
		<table width="700" class="RFHead2">
			<tr>
				<td width="14%" align="center"></td>
				<td width="14%" align="center"></td>
				<td width="14%" align="center">
					<xsl:value-of select="TOTAL" />
				</td>
				<!--<td width="14%" align="center">
					<xsl:value-of select="CHARGETOTAL" />
				</td>
				<td width="14%" align="center">
					<xsl:value-of select="CAPITALTOTAL" />
				</td>-->
				<td width="14%" align="center"></td>
				<!--<td width="14%" align="center"></td>-->
			</tr>
		</table>
		<p></p>
		<table class="RFHead1" width="700">
			<tr>
				<td align="center">
					The above arrangement schedule is valid subject to every payment of instalment<br></br>being made in full and on the due date
				</td>
			</tr>
		</table>
	</xsl:template>

	<xsl:template match="LINEITEMS">
		<xsl:apply-templates select="LINEITEM" />
	</xsl:template>

	<xsl:template match="LINEITEM">
		<tr>
			<td></td>
			<td class="RFBody">
				<xsl:value-of select="ITEMNO" />
			</td>
			<td class="RFBody">
				<xsl:value-of select="DESCR" />
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>
