<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<head>
				<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"></meta>
				<style type="text/css" media="all">
                    @import url(<xsl:value-of select="DELIVERYNOTES/@CSSPATH"/>	);
                </style>
			</head>
			<BODY>	
				<xsl:apply-templates select="DELIVERYNOTES" />
			</BODY>
		</HTML>
	</xsl:template>
	
	<xsl:template match="DELIVERYNOTES">
		<xsl:apply-templates select="DELIVERYNOTE" />		
	</xsl:template>

	<xsl:template match="DELIVERYNOTE">
		<xsl:apply-templates select="HEADER" />		
		<P></P>	
		<xsl:apply-templates select="FOOTER" />
		<P></P>		
		<xsl:apply-templates select="LINEITEMS" />
		<P></P>			
		<xsl:variable name="last" select="LAST" />
		<xsl:if test="$last != 'TRUE'">
			<br class="pageBreak" />
		</xsl:if>
	</xsl:template>

	<xsl:template match="HEADER">
		<TABLE class="normal" id="Address" height="7" width="600" border="0">
			<TR>
				<xsl:apply-templates select="DELTEXT" />
				<TD width="35%" align="left">
					<xsl:apply-templates select="BRANCH" />
					<xsl:apply-templates select="BUFFNO" />
				</TD>
			</TR>
		</TABLE>
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD width="10%" align="left" colspan="2">Stock Location: <xsl:apply-templates select="LOCATION" />
				</TD>
			</TR>
		</TABLE>
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD align="left">AT:
					<xsl:apply-templates select="PRINTED" />
					<xsl:apply-templates select="ACCTNO" />
				</TD>
			</TR>
		</TABLE>
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD align="left" colspan="2">
					DUE: <xsl:value-of select="DELDATE" />
				</TD>
			</TR>
		</TABLE>
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD width="10%" align="left">
					<STRONG><xsl:apply-templates select="PRINTTEXT" /></STRONG>
				</TD>
			</TR>
		</TABLE>
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD width="65%"></TD>
				<TD width="35%"></TD>
			</TR>
			<TR>
				<TD width="65%"></TD>
				<TD width="35%" id="name" align="left">
					<xsl:apply-templates select="FIRSTNAME" />
					<xsl:apply-templates select="LASTNAME" />
				</TD>
			</TR>
			<TR>
				<TD width="65%"></TD>
				<xsl:apply-templates select="ADDRESS1" />
			</TR>
			<TR>
				<TD width="65%"></TD>
				<xsl:apply-templates select="ADDRESS2" />
			</TR>
			<TR>
				<TD width="65%"></TD>
				<xsl:apply-templates select="ADDRESS3" />
			</TR>
			<TR>
				<TD width="65%"></TD>
				<xsl:apply-templates select="POSTCODE" />
			</TR>
			<TR>
				<TD width="65%"></TD>
				<xsl:apply-templates select="HOMETEL" />
			</TR>
			<TR>
				<TD width="65%"></TD>
				<xsl:apply-templates select="WORKTEL" />
			</TR>
		</TABLE>
	</xsl:template>

	<xsl:template match="LINEITEMS">
		<xsl:apply-templates select="LINEITEM" />
	</xsl:template>

	<xsl:template match="LINEITEM">
		<TABLE class="normal" id="Items" width="650" border="0">
			<TR>
				<xsl:apply-templates select="QUANTITY" />
				<xsl:apply-templates select="ITEMNO" />
				<xsl:apply-templates select="DESC1" />
				<xsl:apply-templates select="PRICE" />
			</TR>
			<xsl:apply-templates select="DESC2" />
			<xsl:apply-templates select="NOTES" />
		</TABLE>
		<xsl:apply-templates select="PB" />
	</xsl:template>

	<xsl:template match="FOOTER">
		<div id="SalesPerson" style="position:relative; top=70%; left=0%; width=100%" align="center">
			sp: <xsl:value-of select="USERNAME" />
					(<xsl:value-of select="USER"/>)	
		</div>
		<div style="position:relative; top=60%; left=0%; width=100%" align="right">
			<TABLE width="394" align="right" border="0">
				<TR>
					<TD><xsl:value-of select="CUSTNOTES" /></TD>
				</TR>
			</TABLE>
		</div>
	</xsl:template>

	<xsl:template match="QUANTITY">
		<TD id="quantity" align="left" width="10%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="ITEMNO">
		<TD id="itemno" align="left" width="10%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="DESC1">
		<TD id="desc1" align="left" width="55%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="PRICE">
		<TD id="price" align="right" width="25%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="DESC2">
		<TR>
			<td width="10%"></td>
			<td width="10%"></td>
			<td id="descr2" align="left" width="55%">
				<xsl:apply-templates />
			</td>
			<td width="25%"></td>
		</TR>
	</xsl:template>

	<xsl:template match="NOTES">
		<TR>
			<td width="10%"></td>
			<td width="10%"></td>
			<td id="notes" align="left" width="55%">
				<xsl:apply-templates />
			</td>
			<td width="25%"></td>
		</TR>
	</xsl:template>

	<xsl:template match="DELTEXT">
		<TD width="65%" id="delText" align="left">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="BRANCH">		
		<xsl:apply-templates />/
	</xsl:template>

	<xsl:template match="BUFFNO">		
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="ACCTNO">		
		<TD width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="FIRSTNAME">		
		<xsl:apply-templates /> 
	</xsl:template>

	<xsl:template match="LASTNAME">		
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="ADDRESS1">	
		<TD width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="ADDRESS2">	
		<TD width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="ADDRESS3">	
		<TD width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="POSTCODE">	
		<TD width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="HOMETEL">	
		<TD width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="WORKTEL">	
		<TD width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="PRINTED">		
		<TD width="62%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="LOCATION">		
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="PRINTTEXT">		
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="PB">
		<br class="pageBreak" />
		<xsl:apply-templates select="../../../HEADER" />	
		<P></P>
	</xsl:template>

</xsl:stylesheet>