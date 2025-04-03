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
			<xsl:apply-templates select="FOOTER" />	
		</TABLE>
		</div>
	</xsl:template>

	<xsl:template match="DATA">
		<xsl:apply-templates select="ROW" />	
	</xsl:template>
	
	<xsl:template match="ROW">
		<TR class="RFBody">
			<TD width="15%"><xsl:value-of select="LEVEL" /></TD>
			<TD align="right" width="7.7%"><xsl:value-of select="P1" /></TD>
			<TD align="right" width="7.7%"><xsl:value-of select="P2" /></TD>
			<TD align="right" width="7.7%"><xsl:value-of select="P3" /></TD>
			<TD align="right" width="7.7%"><xsl:value-of select="P4" /></TD>
			<TD align="right" width="7.7%"><xsl:value-of select="P5" /></TD>
			<TD align="right" width="7.7%"><xsl:value-of select="P6" /></TD>
			<TD align="right" width="7.7%"><xsl:value-of select="P7" /></TD>
			<TD align="right" width="7.7%"><xsl:value-of select="P8" /></TD>
			<TD align="right" width="7.7%"><xsl:value-of select="P9" /></TD>
			<TD align="right" width="7.7%"><xsl:value-of select="P10" /></TD>
			<TD align="right" width="7.7%"><xsl:value-of select="P11" /></TD>
			<TD align="right" width="7.7%"><xsl:value-of select="P12" /></TD>
		</TR>
	</xsl:template>
	
	<xsl:template match="HEADER">
		<TR class="RFHead2">
			<TD width="15%"><xsl:value-of select="LEVEL" /></TD>
			<TD width="7.7%"><xsl:value-of select="P1" /></TD>
			<TD width="7.7%"><xsl:value-of select="P2" /></TD>
			<TD width="7.7%"><xsl:value-of select="P3" /></TD>
			<TD width="7.7%"><xsl:value-of select="P4" /></TD>
			<TD width="7.7%"><xsl:value-of select="P5" /></TD>
			<TD width="7.7%"><xsl:value-of select="P6" /></TD>
			<TD width="7.7%"><xsl:value-of select="P7" /></TD>
			<TD width="7.7%"><xsl:value-of select="P8" /></TD>
			<TD width="7.7%"><xsl:value-of select="P9" /></TD>
			<TD width="7.7%"><xsl:value-of select="P10" /></TD>
			<TD width="7.7%"><xsl:value-of select="P11" /></TD>
			<TD width="7.7%"><xsl:value-of select="P12" /></TD>
		</TR>		
	</xsl:template>

	<xsl:template match="FOOTER">
		<TR>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
			<TD><BR/></TD>
		</TR>
		<TR>
			<TD class="RFHead2" width="15%"><xsl:value-of select="LEVEL" /></TD>
			<TD align="right" class="RFBody" width="7.7%"><xsl:value-of select="P1" /></TD>
			<TD align="right" class="RFBody" width="7.7%"><xsl:value-of select="P2" /></TD>
			<TD align="right" class="RFBody" width="7.7%"><xsl:value-of select="P3" /></TD>
			<TD align="right" class="RFBody" width="7.7%"><xsl:value-of select="P4" /></TD>
			<TD align="right" class="RFBody" width="7.7%"><xsl:value-of select="P5" /></TD>
			<TD align="right" class="RFBody" width="7.7%"><xsl:value-of select="P6" /></TD>
			<TD align="right" class="RFBody" width="7.7%"><xsl:value-of select="P7" /></TD>
			<TD align="right" class="RFBody" width="7.7%"><xsl:value-of select="P8" /></TD>
			<TD align="right" class="RFBody" width="7.7%"><xsl:value-of select="P9" /></TD>
			<TD align="right" class="RFBody" width="7.7%"><xsl:value-of select="P10" /></TD>
			<TD align="right" class="RFBody" width="7.7%"><xsl:value-of select="P11" /></TD>
			<TD align="right" class="RFBody" width="7.7%"><xsl:value-of select="P12" /></TD>
		</TR>		
	</xsl:template>
	
</xsl:stylesheet>