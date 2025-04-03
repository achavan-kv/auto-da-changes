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
				<xsl:apply-templates select="REBATEFORECAST" />
			</BODY>
		</HTML>
	</xsl:template>
	
	<xsl:template match="REBATEFORECAST">
		<div style="position:relative">
			<TABLE class="RFHead1" border="0">
			<TR>
				<td>
					<xsl:value-of select="TITLE" />
				</td>
			</TR>
		</TABLE>
		<P/>
		<TABLE id="table" border="0">
			<xsl:apply-templates select="HEADER" />	
			<xsl:apply-templates select="DATA" />	
		</TABLE>
		</div>
	</xsl:template>

	<xsl:template match="DATA">
		<xsl:apply-templates select="ROW" />	
	</xsl:template>
	
	<xsl:template match="ROW">
		<TR class="RFBody">
			<TD width="9.09%"><xsl:value-of select="COL1" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL2" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL3" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL4" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL5" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL6" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL7" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL8" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL9" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL10" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL11" /></TD>
		</TR>
	</xsl:template>
	
	<xsl:template match="HEADER">
		<TR class="RFHead2">
			<TD width="9.09%"><xsl:value-of select="COL1" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL2" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL3" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL4" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL5" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL6" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL7" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL8" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL9" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL10" /></TD>
			<TD align="right" width="9.09%"><xsl:value-of select="COL11" /></TD>
		</TR>
		<TR>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
		</TR>		
	</xsl:template>
</xsl:stylesheet>