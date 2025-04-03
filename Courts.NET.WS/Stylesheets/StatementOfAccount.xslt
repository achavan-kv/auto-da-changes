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
				<xsl:apply-templates select="STATEMENT" />
			</BODY>
		</HTML>		
	</xsl:template>
	
	<xsl:template match="STATEMENT">
		<xsl:apply-templates select="PAGE" />				
		<P></P>	
	</xsl:template>
	
	<xsl:template match="PAGE">
		<div style="position:relative">
			<table align="center" width="600" class="WarrantyFooter">
				<tr>
					<td align="center"><xsl:value-of select="HEADER/COUNTRYNAME" /></td>
				</tr>
			</table>
			<p></p>
			<table align="center" width="600">
				<tr>
					<td class="RFBody" width="50%"><xsl:value-of select="HEADER/CUSTOMERNAME" /></td>
					<td class="RFBody" width="25%">Instalment:</td>
					<td class="RFBody" width="25%"><xsl:value-of select="HEADER/INSTALMENT" /></td>
				</tr>
				<tr>
					<td class="RFBody" width="50%"><xsl:value-of select="HEADER/ADDR1" /></td>
					<td class="RFBody" width="25%"></td>
					<td class="RFBody" width="25%"></td>
				</tr>
				<tr>
					<td class="RFBody" width="50%"><xsl:value-of select="HEADER/ADDR2" /></td>
					<td class="RFBody" width="25%">Date Due:</td>
					<td class="RFBody" width="25%"><xsl:value-of select="HEADER/DUEDATE" /></td>
				</tr>
				<tr>
					<td class="RFBody" width="50%"><xsl:value-of select="HEADER/ADDR3" /></td>
          <td class="RFBody" width="25%">Agreement Total:</td>
          <td class="RFBody" width="25%"><xsl:value-of select="HEADER/AGREEMENTTOTAL" />
          </td>
        </tr>
				<tr>
					<td class="RFBody" width="50%"><xsl:value-of select="HEADER/POSTCODE" /></td>
          <td class="RFBody" width="25%">Outstanding Balance:</td>
          <td class="RFBody" width="25%"><xsl:value-of select="HEADER/OUTSTANDINGBAL" />
          </td>
        </tr>	
				<tr>
					<td class="RFBody" width="50%">Date <xsl:value-of select="HEADER/DATE" /></td>
					<td class="RFBody" width="25%">Arrears:</td>
					<td class="RFBody" width="25%"><xsl:value-of select="HEADER/ARREARS" /></td>
				</tr>		
			</table>
			<p></p>
			<br></br>
			<DIV style="BORDER-RIGHT: gray thin solid; BORDER-TOP: gray thin solid; LEFT: -0.75cm; BORDER-LEFT: gray thin solid; WIDTH: 17.5cm; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 4.5cm; HEIGHT: 19.5cm" ms_positioning="FlowLayout"></DIV>
			<DIV style="BORDER-TOP: gray thin solid; LEFT: -0.75cm; WIDTH: 17.5cm; POSITION: absolute; TOP: 6.0cm; HEIGHT: 0.2cm" ms_positioning="FlowLayout"></DIV>
			<table  align="center" width="600">
				<tr>
					<td align="center" class="RFHead1" width="100%">Statement of Account</td>
				</tr>
				<tr>
					<td align="center" class="RFBody" width="100%">Account No: <xsl:value-of select="HEADER/ACCTNO" /></td>
				</tr>
				<tr>
					<td>
						<table class="RFBody" width="100%">
							<tr>
								<td><b>Date</b></td>
								<td><b>Transaction</b></td>
								<td align="right"><b>Debit</b></td>
								<td align="right"><b>Credit</b></td>
								<td align="right"><b>Balance</b></td>
							</tr>
							<!--<br></br>-->
							<xsl:apply-templates select="TRANSACTIONS" />
						</table>
					</td>
				</tr>			
			</table>	
		</div>	
		<xsl:variable name="last" select="LAST" />
		<xsl:if test="$last != 'True'">
			<br class="pageBreak" />
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="TRANSACTIONS">
		<xsl:apply-templates select="TRANSACTION" />
	</xsl:template>
	
	<xsl:template match="TRANSACTION">
		<tr>
			<td><xsl:value-of select="DATE" /></td>
			<td><xsl:value-of select="TRANSTYPE" /></td>
			<td align="right"><xsl:value-of select="DEBIT" /></td>
			<td align="right"><xsl:value-of select="CREDIT" /></td>
			<td align="right"><xsl:value-of select="BALANCE" /></td>
		</tr>
	</xsl:template>
	
</xsl:stylesheet>

  