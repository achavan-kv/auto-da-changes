<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<HEAD>
				<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"></meta>
				<style type="text/css" media="all">
					@import url(styles.css);
				</style>
			</HEAD>
			<BODY>					
				<xsl:apply-templates select="ACCOUNTNOS" />
			</BODY>
		</HTML>
	</xsl:template>
	
	<xsl:template match="ACCOUNTNOS">
		<TABLE cellSpacing="1" cellPadding="1" width="600" align="center" border="0">
			<TR>
				<TD class="RFHead1" align="left"><U>PRE-PRINTED ACCOUNT NUMBERS</U><br/><br/></TD>
			</TR>
			<xsl:apply-templates select="ACCOUNTNO" />
		</TABLE>
	</xsl:template>
	
	<xsl:template match="ACCOUNTNO">
		<TR>
			<TD class="normal" align="left">
				<xsl:apply-templates />
			</TD>
		</TR>
	</xsl:template>
	
</xsl:stylesheet>

  

  