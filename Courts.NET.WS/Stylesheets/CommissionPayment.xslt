<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<head>
				<TITLE></TITLE>
				<META content="Microsoft Visual Studio 7.0" name="GENERATOR"></META>
				<META content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"></META>
				<style type="text/css" media="all"> @import url(styles.css); 
				</style>
			</head>
			<BODY>	
				<xsl:apply-templates select="COMMISSION" />
			</BODY>
		</HTML>
	</xsl:template>
	
	<xsl:template match="COMMISSION">
		<xsl:apply-templates select="HEADER" />	
		<P></P>	
		<xsl:apply-templates select="TRANSACTIONS" />					
		<P></P>	
		<P></P>	
		<P></P>
		<xsl:apply-templates select="FOOTER" />	
	</xsl:template>

	<xsl:template match="HEADER">
		<TABLE width="600" border="0" ID="Table1">
			<TR>
				<TD class="normalBold" align="center" >Commission Cheque Requisition</TD>
			</TR>
		</TABLE>
		<P></P>	
		<P></P>	
		<P></P>	
		<TABLE width="600" border="0">
			<TR>
				<TD class="normal" align="center" >Bailiff Commission Transactions</TD>
			</TR>
		</TABLE>
		<TABLE class="normal" width="600" border="0">
			<TR>
				<TD><xsl:value-of select="BAILIFF" /></TD>
			</TR>
		</TABLE>

		<TABLE class="normal" width="600" border="0">
			<TR>
				<TD><xsl:value-of select="DATE" /></TD>
			</TR>
		</TABLE>

		<P></P>	
		<P></P>	

		<TABLE class="RFHead2" width="600" border="0">
			<TR>
				<TD width="14.28%">Account No</TD>
				<TD width="14.28%">Date</TD>
				<TD width="14.28%">Value</TD>
				<TD width="14.28%">Chq</TD>
				<TD width="14.28%">Status</TD>
				<TD width="14.28%">Type</TD>
				<TD width="14.28%">On Amount</TD>
			</TR>		
		</TABLE>
	</xsl:template>

	<xsl:template match="TRANSACTIONS">
		<xsl:apply-templates select="TRANSACTION" />	
	</xsl:template>
	
	<xsl:template match="TRANSACTION">
		<TABLE class="RFBody" width="600" border="0">
			<TR>
				<TD width="14.28%"><xsl:value-of select="ACCTNO" /></TD>
				<TD width="14.28%"><xsl:value-of select="DATETRANS" /></TD>
				<TD width="14.28%"><xsl:value-of select="VALUE" /></TD>
				<TD width="14.28%"><xsl:value-of select="CHEQUE" /></TD>
				<TD width="14.28%"><xsl:value-of select="STATUS" /></TD>
				<TD width="14.28%"><xsl:value-of select="TYPE" /></TD>
				<TD width="14.28%"><xsl:value-of select="AMOUNT" /></TD>
			</TR>
		</TABLE>
		<xsl:apply-templates select="PB" />
	</xsl:template>
	
	<xsl:template match="FOOTER">
		<TABLE class="normal" width="600" border="0">
			<TR>
				<TD colspan="2">Bailiff: <xsl:value-of select="EMPNAME" /></TD>
			</TR>
		</TABLE>

		<TABLE class="normal" width="600" border="0">
			<TR>
				<TD colspan="2">Total Commission Value: <xsl:value-of select="COMMNVALUE" /></TD>
			</TR>
		</TABLE>

		<TABLE class="normal" width="600" border="0">
			<TR>
				<TD colspan="2">Prepared By: <xsl:value-of select="USER" /></TD>
			</TR>
		</TABLE>

		<P></P>	
		<P></P>	
		
		<TABLE class="normalBold" width="600" border="0">
			<TR>
				<TD align="right">Signed:</TD>
			</TR>
			<TR>
				<TD align="right">Date:</TD>
			</TR>
		</TABLE>
		<P></P>	
		<TABLE class="normalBold" width="600" border="0">
			<TR>
				<TD align="right">Received:</TD>
			</TR>
			<TR>
				<TD align="right">Date:</TD>
			</TR>
		</TABLE>
	</xsl:template>
	
	<xsl:template match="PB">
		<br class="pageBreak" />
		<xsl:apply-templates select="//HEADER" />	
		<P></P>
	</xsl:template>

</xsl:stylesheet>