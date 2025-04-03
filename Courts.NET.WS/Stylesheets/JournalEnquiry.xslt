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
				<xsl:apply-templates select="JOURNALENQUIRY" />
			</BODY>
		</HTML>		
	</xsl:template>
	
	<xsl:template match="JOURNALENQUIRY">
		<xsl:apply-templates select="PAGE" />				
		<P></P>	
		<br></br>
		<xsl:apply-templates select="TOTALS" />
	</xsl:template>

	<xsl:template match="PAGE">
		<div style="position:relative">
			<table width="700" class="RFHead1">
				<tr>
					<td align="center">JOURNAL ENQUIRY - PRINTED: <xsl:value-of select="HEADER/DATE" /></td>
				</tr>
			</table>
			<p></p>
			<table width="700">
				<tr class="RFHead1" >
					<td width="14%" align="center">Branch</td>
					<td width="14%" align="center">Emp</td>
					<td width="14%" align="center">Date/Time</td>
					<td width="14%" align="center">Ref</td>
					<td width="14%" align="center">Account</td>
					<td width="14%" align="center">Type</td>
					<td width="14%" align="center">Value</td>
				</tr>
				<xsl:apply-templates select="TRANSACTIONS" />
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
		<tr class="RFBody" >
			<td width="14%" align="center"><xsl:value-of select="BRANCH" /></td>
			<td width="14%" align="center"><xsl:value-of select="EMPLOYEE" /></td>
			<td width="14%" align="center"><xsl:value-of select="DATETRANS" /></td>
			<td width="14%" align="center"><xsl:value-of select="REFNO" /></td>
			<td width="14%" align="center"><xsl:value-of select="ACCTNO" /></td>
			<td width="14%" align="center"><xsl:value-of select="TRANSTYPE" /></td>
			<td width="14%" align="center"><xsl:value-of select="TRANSVALUE" /></td>
		</tr>

	</xsl:template>
		<xsl:template match="TOTALS">
		<xsl:apply-templates select="TOTAL" />
	</xsl:template>
	
	<xsl:template match="TOTAL">
		<table width="700" class="RFBody">
			<tr>
			<td width="14%" align="center"><xsl:value-of select="TRANSTYPE" />:</td>
			<td width="14%" align="right"><xsl:value-of select="TRANSVALUE" /></td>
			<td width="14%"></td>
			<td width="14%"></td>
			<td width="14%"></td>
			<td width="14%"></td>
			<td width="14%"></td>
			</tr>
		</table>
	</xsl:template>
</xsl:stylesheet>

  