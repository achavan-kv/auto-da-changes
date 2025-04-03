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
				<TABLE id="Table1" cellSpacing="1" cellPadding="1" width="600" align="center" border="0">
					<TR>
						<TD class="RFHead1" align="middle">Delivery Schedule</TD>
					</TR>
				</TABLE>
			<BODY>	
				<xsl:apply-templates select="DELIVERYSCHEDULES" />
			</BODY>
			</BODY>
		</HTML>
	</xsl:template>
				
	<xsl:template match="DELIVERYSCHEDULES">
		<xsl:apply-templates select="DELIVERYSCHEDULE" />		
	</xsl:template>

	<xsl:template match="DELIVERYSCHEDULE">
		<P></P>
		<xsl:apply-templates select="HEADER" />		
		<P></P>
		<xsl:apply-templates select="CUSTOMERS" />		
		<P></P>
		<P></P>
		<P></P>
		<xsl:apply-templates select="FOOTER" />		
	</xsl:template>

	<xsl:template match="HEADER">
		<TABLE class="normal" id="Header" width="650" border="0">
			<TR>
				<TD colspan="2"><STRONG><xsl:value-of select="PRINTTEXT" /></STRONG></TD>
			</TR>
			<TR>
				<TD colspan="2"><STRONG>Scheduling Branch: </STRONG><xsl:value-of select="BRANCH" /></TD>
			</TR>
			<TR>
				<TD colspan="2"><STRONG>Date: </STRONG><xsl:value-of select="DELIVERYDATE" /></TD>
			</TR>
			<TR>
				<TD colspan="2"><STRONG>Load: </STRONG><xsl:value-of select="LOADNO" /></TD>
			</TR>
			<TR>
				<TD colspan="2"><STRONG>Delivery Vehicle: </STRONG><xsl:value-of select="TRUCKID" /></TD>
			</TR>
			<TR>
				<TD colspan="2"><STRONG>Driver: </STRONG><xsl:value-of select="DRIVERNAME" /></TD>
			</TR>
		</TABLE>
		<P></P>			
		<P></P>			
		<TABLE class="normal" id="Headings" width="650" border="0">
			<TR class="normalBold">
        <TD width="10%">Stock<BR></BR>Locn</TD>
				<TD width="10%">Buff</TD>
				<TD width="60%">Customer</TD>
				<TD width="20%">Account No.</TD>
			</TR>
		</TABLE>
		<P></P>			
	</xsl:template>

	<xsl:template match="CUSTOMERS">
		<xsl:apply-templates select="CUSTOMER" />
	</xsl:template>

	<xsl:template match="CUSTOMER">
		<TABLE class="normal" id="Customers" width="650" border="0">
			<TR>
        <TD width="10%" class="normal"><xsl:value-of select="LOCN" /></TD>
				<TD width="10%" class="normal"><xsl:value-of select="BUFFNO" /></TD>
				<TD width="60%" class="normal"><xsl:value-of select="NAME" /></TD>
				<TD width="20%" class="normal"><xsl:value-of select="ACCTNO" /></TD>
			</TR>
		</TABLE>
		<xsl:apply-templates select="LINEITEMS" />
		<P></P>			
		<P></P>			
	</xsl:template>	

	<xsl:template match="LINEITEMS">
		<xsl:apply-templates select="LINEITEM" />
	</xsl:template>

	<xsl:template match="LINEITEM">
		<TABLE class="normalSmall" id="Items" width="650" border="0">
			<TR style="vertical-align:top;">
                <TD width="3%"> </TD>
                <TD width="10%"><xsl:value-of select="QUANTITY" /></TD>
				<TD width="12%"><xsl:value-of select="ITEMNO" /></TD>
				<TD width="55%"><xsl:value-of select="DESC" /></TD>	
				<TD width="20%" align="right"><xsl:value-of select="PRICE" /></TD>	
			</TR>

		</TABLE>
		<xsl:apply-templates select="PB" />
	</xsl:template>
	
	<xsl:template match="FOOTER">
		<TABLE class="normal" id="Footer" width="650" border="0">
			<TR>
				<TD width="10%"></TD>
				<TD width="20%"></TD>
				<TD width="50%"><STRONG>Total Value</STRONG></TD>	
				<TD width="20%" align="right" ><xsl:value-of select="TOTAL" /></TD>
			</TR>
		</TABLE>
	</xsl:template>	
	
	<xsl:template match="PB">
		<br class="pageBreak" />
		<xsl:apply-templates select="//HEADER" />	
		<P></P>
	</xsl:template>
</xsl:stylesheet>
