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
				<xsl:apply-templates select="COMMISSIONS" />
			</BODY>
		</HTML>
	</xsl:template>

	<xsl:template match="COMMISSIONS">
		<xsl:apply-templates select="COMMISSION" />		
	</xsl:template>
	
	<xsl:template match="COMMISSION">
		<xsl:apply-templates select="DETAILS" />	
		<P></P>			
		<xsl:variable name="last" select="LAST" />
		<xsl:if test="$last != 'TRUE'">
			<br class="pageBreak" />
		</xsl:if>
	</xsl:template>

	<xsl:template match="DETAILS">
		<TABLE class="normalBold" width="600" border="0">
			<TR>
				<TD align="left" >REPRINT</TD>
			</TR>
		</TABLE>
		
		<TABLE width="600" border="0" ID="Table1">
			<TR>
				<TD class="normalBold" align="center" >Commission Cheque Requisition</TD>
			</TR>
		</TABLE>

		<P></P>	
		<P></P>	
		<P></P>	
	
		<TABLE class="normal" width="600" border="0">
			<TR>
				<TD colspan="2" align="left" >Bailiff: <xsl:value-of select="BAILIFF" /></TD>
			</TR>
			<TR>
				<TD colspan="2" align="left" >Amount: <xsl:value-of select="AMOUNT" /></TD>
			</TR>
			<TR>
				<TD colspan="2" align="left" >Authorised By: <xsl:value-of select="USER" /></TD>
			</TR>
		</TABLE>

		<P></P>	
		<P></P>	
		
		<TABLE class="normal" width="600" border="0">
			<TR>
				<TD align="right">Signed:</TD>
			</TR>
			<TR>
				<TD align="right">Date:</TD>
			</TR>
		</TABLE>
		<P></P>	
		<TABLE class="normal" width="600" border="0">
			<TR>
				<TD align="right">Received:</TD>
			</TR>
			<TR>
				<TD align="right">Date:</TD>
			</TR>
		</TABLE>
	</xsl:template>
</xsl:stylesheet>