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
				<xsl:apply-templates select="ONEFORONEREPLACEMENTNOTE" />
			</BODY>
		</HTML>
	</xsl:template>

	<xsl:template match="ONEFORONEREPLACEMENTNOTE">
		<xsl:apply-templates select="HEADER" />		
		<P></P>
		<xsl:apply-templates select="PRODUCTDETAILS" />		
		<P></P>
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD width="100%" align="center">sp: <xsl:value-of select="HEADER/USERNAME" />
						(<xsl:value-of select="HEADER/USER"/>)	
				</TD>
			</TR>
		</TABLE>
	</xsl:template>

	<xsl:template match="HEADER">
		<P><FONT face="Tahoma" color="#ffffff" size="4">
			<STRONG>
				<TABLE id="Table2" style="WIDTH: 436px; HEIGHT: 38px" borderColor="black" cellSpacing="0" cellPadding="0" width="436" align="center" bgColor="white" border="0">
					<TR>
						<TD style="WIDTH: 445px" align="middle"><STRONG>INSTANT REPLACEMENT ITEM DETAILS</STRONG></TD>
					</TR>
				</TABLE>
			</STRONG>
			</FONT>
		</P>

		<TABLE id="Table1" style="WIDTH: 650px;" border="0">
			<TR>
				<TD width="20%"><FONT face="Tahoma">Stock Location:</FONT></TD>
				<xsl:apply-templates select="BRANCHNO" />
				<TD width="10%"><FONT face="Tahoma">Acct No:</FONT></TD>
				<xsl:apply-templates select="ACCTNO" />
			</TR>
			<TR>
				<TD width="20%"><FONT face="Tahoma">Printed:</FONT></TD>
				<xsl:apply-templates select="PRINTED" />
				<TD width="10%"><FONT face="Tahoma"></FONT></TD>
				<xsl:apply-templates select="CUSTID" />
			</TR>
			<TR>
				<TD width="20%"><FONT face="Tahoma"></FONT></TD>
				<TD width="35%"><FONT face="Tahoma"></FONT></TD>
				<TD width="10%"><FONT face="Tahoma"></FONT></TD>
				<TD width="35%" id="name" align="left">
				</TD>
			</TR>
		</TABLE>
		
		<TABLE style="WIDTH: 650px;" border="0">
			<TR>
				<TD valign="top" width="20%">Notes: 
				</TD>
				<TD width="80%"><xsl:value-of select="NOTES" />
				</TD>
			</TR>
		</TABLE>
		
	</xsl:template>

	<xsl:template match="PRODUCTDETAILS">
		<TABLE id="productdetails" width="662" border="0">
			<TR>
				<TD width="35%"><FONT face="Tahoma">Item No:</FONT></TD>
				<xsl:apply-templates select="ITEMNO" />
			</TR>
			<TR>
				<TD width="35%"><FONT face="Tahoma">Return Reason:</FONT></TD>
				<xsl:apply-templates select="REASON" />
			</TR>
			<TR>
				<TD width="35%"><FONT face="Tahoma">Return Date:</FONT></TD>
				<xsl:apply-templates select="DATERETURN" />
			</TR>
			<!--<TR>
				<TD width="35%"><FONT face="Tahoma">Instant Replacement Time Period:</FONT></TD>
				<xsl:apply-templates select="ONEFORONETIMEPERIOD" />
			</TR>-->
			<TR>
				<TD width="35%"><FONT face="Tahoma">Product Description:</FONT></TD>
				<xsl:apply-templates select="PRODUCTDESCRIPTION" />
			</TR>
		</TABLE>
		<xsl:apply-templates select="PB" />
	</xsl:template>

	<xsl:template match="ITEMNO">
		<TD id="itemno" align="left" width="65%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>

	<xsl:template match="REASON">
		<TD id="reason" align="left" width="65%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>

	<xsl:template match="ONEFORONETIMEPERIOD">
		<TD id="oneforonetimeperiod" align="left" width="65%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>

	<xsl:template match="DATERETURN">
		<TD id="datereturn" align="left" width="65%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>

	<xsl:template match="PRODUCTDESCRIPTION">
		<TD id="desc1" align="left" width="65%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>

	<xsl:template match="BRANCHNO">	
		<TD width="20%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>

	<xsl:template match="ACCTNO">		
		<TD width="35%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>

	<xsl:template match="FIRSTNAME">		
		<FONT face="Tahoma"><xsl:apply-templates /></FONT>
	</xsl:template>

	<xsl:template match="LASTNAME">		
		<FONT face="Tahoma"><xsl:apply-templates /></FONT>
	</xsl:template>

	<xsl:template match="ADDRESS1">	
		<TD width="35%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>

	<xsl:template match="ADDRESS2">	
		<TD width="35%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>

	<xsl:template match="ADDRESS3">	
		<TD width="35%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>

	<xsl:template match="POSTCODE">	
		<TD width="35%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>

	<xsl:template match="PRINTED">	
		<TD width="35%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>

	<xsl:template match="CUSTID">	
		<TD width="35%">
			<FONT face="Tahoma"><xsl:apply-templates /></FONT>
		</TD>
	</xsl:template>
		
	<xsl:template match="PB">
		<br class="pageBreak" />
		<xsl:apply-templates select="../../../HEADER" />	
		<P></P>
	</xsl:template>

</xsl:stylesheet>  