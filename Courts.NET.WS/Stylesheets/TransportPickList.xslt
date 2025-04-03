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
				<xsl:apply-templates select="PICKLISTS" />
			</BODY>
		</HTML>
	</xsl:template>
	
	<xsl:template match="PICKLISTS">
		<xsl:apply-templates select="PICKLIST" />		
	</xsl:template>
	
	<xsl:template match="PICKLIST">
		<div style="position:relative">
			<xsl:apply-templates select="HEADER" />		
			<P></P>	
			<xsl:apply-templates select="DELIVERYNOTES" />
			<P></P>	
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'TRUE'">
				<br class="pageBreak" />
			</xsl:if>
		</div>
	</xsl:template>

	<xsl:template match="HEADER">
		<TABLE class="normalBold" id="Address" height="7" width="600" border="0">
			<TR>
				<TD align="center">CoSACS Transport Picking List</TD>
			</TR>
		</TABLE>
		<P></P>			
		<P></P>			
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD align="left"><xsl:value-of select="PICKTEXT" /></TD>
			</TR>
		</TABLE>
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD width="10%" align="left" colspan="2">Transport Pick List Number <xsl:value-of select="PICKNUMBER" />
				</TD>
			</TR>
		</TABLE>
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD align="left">
					<xsl:value-of select="DELNOTEBRANCH" />
				</TD>
			</TR>
		</TABLE>
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD align="left">
					Printed For: Branch <xsl:value-of select="BRANCH" /> On 
					<xsl:value-of select="PRINTED" />
				</TD>
			</TR>
		</TABLE>
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD align="left" colspan="2">
					Printed By: <xsl:value-of select="USERNAME" />(<xsl:value-of select="USER"/>)
				</TD>
			</TR>
		</TABLE>
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD align="left"><xsl:value-of select="BRANCHNAME" /></TD>
			</TR>
		</TABLE>
		<P></P>			
		<P></P>			
		<TABLE class="normalBold" height="7" width="600" border="0">
			<TR>
				<TD align="center"> <xsl:value-of select="DELTEXT" /> </TD>
			</TR>
		</TABLE>
		<P></P>			
		<P></P>			
		<TABLE class="normal" id="Headings" width="650" border="0">
			<TR class="normalBold">
				<TD width="10%">D/N No's</TD>
				<TD width="15%">Product Category</TD>
				<TD width="15%">Product Code</TD>
        <TD width="15%">Stock Location</TD>
        <TD width="25%">Description</TD>
				<TD width="15%">Order Quantity</TD>
				<TD width="5%">Load No.</TD>
			</TR>
		</TABLE>
		<P></P>			
	</xsl:template>

	<xsl:template match="DELIVERYNOTES">
		<xsl:apply-templates select="DELIVERYNOTE" />		
	</xsl:template>

	<xsl:template match="DELIVERYNOTE">
		<TABLE class="normal" id="Delivery" width="650" border="0">
			<TR>
				<xsl:apply-templates select="BUFFNO" />
				<xsl:apply-templates select="CATEGORY" />
				<xsl:apply-templates select="ITEMNO" />
				<xsl:apply-templates select="LOCN" />
				<xsl:apply-templates select="DESC1" />
				<xsl:apply-templates select="QUANTITY" />
				<xsl:apply-templates select="LOADNO" />
			</TR>
		</TABLE>
		<xsl:apply-templates select="PB" />
	</xsl:template>

	<xsl:template match="BUFFNO">
		<TD id="buffno" align="left" width="10%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="CATEGORY">
		<TD id="category" align="left" width="15%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="ITEMNO">
		<TD id="itemno" align="left" width="15%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

  <xsl:template match="LOCN">
    <TD id="load" align="left" width="15%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="DESC1">
		<TD id="desc1" align="left" width="25%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="QUANTITY">
		<TD id="price" align="left" width="15%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="LOADNO">
		<TD id="load" align="left" width="5%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="PB">
		<br class="pageBreak" />
		<xsl:apply-templates select="../../../HEADER" />	
		<P></P>
	</xsl:template>

</xsl:stylesheet>