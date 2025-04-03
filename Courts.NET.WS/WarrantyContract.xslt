<?xml version="1.0" encoding="UTF-8" ?>
<!DOCTYPE xsl:stylesheet [<!ENTITY nbsp " ">]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<HEAD>
				<TITLE></TITLE>
				<META NAME="GENERATOR" Content="Microsoft Visual Studio 7.0"></META>
				<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"></meta>
				<style type="text/css" media="all"> @import url(styles.css); 
				</style>
			</HEAD>
			<BODY>
				<xsl:apply-templates select="CONTRACTS" />
			</BODY>
		</HTML>		
	</xsl:template>
	
	<xsl:template match="CONTRACTS">	
		<xsl:apply-templates select="CONTRACT" />		
	</xsl:template>
	
	<xsl:template match="CONTRACT">	
		<div style="position:relative">
			<div class="smaller" style="LEFT: 15.25cm; WIDTH: 4cm; POSITION: absolute; TOP: 0.5cm; HEIGHT: 0.5cm">
				<xsl:value-of select="COPY" />
			</div>	
			<div style="LEFT: 16cm; WIDTH: 2cm; POSITION: absolute; TOP: 1.8cm; HEIGHT: 0.5cm">
				<xsl:value-of select="CONTRACTNO" />
			</div>
			<div style="LEFT: 15.5cm; WIDTH: 3cm; POSITION: absolute; TOP: 6.85cm; HEIGHT: 0.5cm">
				<xsl:value-of select="SOLDBY" />
			</div>
			<div style="LEFT: 15.5cm; WIDTH: 3cm; POSITION: absolute; TOP: 8.2cm; HEIGHT: 0.5cm">
				<xsl:value-of select="BRANCHNAME" />
			</div>
			<div style="LEFT: 15.5cm; WIDTH: 3cm; POSITION: absolute; TOP: 9.6cm; HEIGHT: 0.5cm">
				<xsl:value-of select="STORENO" />
			</div>
			<div style="LEFT: 15.5cm; WIDTH: 3cm; POSITION: absolute; TOP: 11cm; HEIGHT: 0.5cm">
				<xsl:value-of select="TODAY" />
			</div>
			<div style="LEFT: 15.5cm; WIDTH: 3cm; POSITION: absolute; TOP: 12.4cm; HEIGHT: 0.5cm">
				<xsl:value-of select="SOLDBYNAME" />
			</div>
			<div style="LEFT: 2.6cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO1" />
			</div>
			<div style="LEFT: 3.4cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO2" />
			</div>
			<div style="LEFT: 4.2cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO3" />
			</div>
			<div style="LEFT: 5cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO4" />
			</div>
			<div style="LEFT: 5.8cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO5" />
			</div>
			<div style="LEFT: 6.6cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO6" />
			</div>
			<div style="LEFT: 7.4cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO7" />
			</div>
			<div style="LEFT: 8.2cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO8" />
			</div>
			<div style="LEFT: 9cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO9" />
			</div>
			<div style="LEFT: 9.8cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO10" />
			</div>
			<div style="LEFT: 10.6cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO11" />
			</div>
			<div style="LEFT: 11.4cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO12" />
			</div>
			<div style="LEFT: 3cm; WIDTH: 5cm; POSITION: absolute; TOP: 7.25cm; HEIGHT: 0.5cm">
				<xsl:value-of select="FIRSTNAME" />
			</div>
			<div style="LEFT: 7.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 7.25cm; HEIGHT: 0.5cm">
				<xsl:value-of select="LASTNAME" />
			</div>
			<div style="LEFT: 3cm; WIDTH: 10cm; POSITION: absolute; TOP: 8.25cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ADDRESS1" />
			</div>
			<div style="LEFT: 3cm; WIDTH: 10cm; POSITION: absolute; TOP: 8.75cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ADDRESS2" />
			</div>
			<div style="LEFT: 3cm; WIDTH: 10cm; POSITION: absolute; TOP: 9.25cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ADDRESS3" />
			</div>
			<div style="LEFT: 3cm; WIDTH: 10cm; POSITION: absolute; TOP: 9.75cm; HEIGHT: 0.5cm">
				<xsl:value-of select="POSTCODE" />
			</div>
			<div style="LEFT: 3cm; WIDTH: 5cm; POSITION: absolute; TOP: 11cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WORKTEL" />
			</div>
			<div style="LEFT: 7.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 11cm; HEIGHT: 0.5cm">
				<xsl:value-of select="HOMETEL" />
			</div>
			<div style="LEFT: 1cm; WIDTH: 5cm; POSITION: absolute; TOP: 15.25cm; HEIGHT: 0.5cm">
				<xsl:value-of select="DATEOFPURCHASE" />
			</div>
			<div style="LEFT: 7.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 15.25cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ITEMNO" />
			</div>
			<div style="LEFT: 1cm; WIDTH: 16cm; POSITION: absolute; TOP: 16.4cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ITEMDESC1" />
			</div>
			<div style="LEFT: 1cm; WIDTH: 16cm; POSITION: absolute; TOP: 16.9cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ITEMDESC2" />
			</div>
			<div style="LEFT: 4cm; WIDTH: 16cm; POSITION: absolute; TOP: 17.8cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ITEMPRICE" />
			</div>
			<div style="LEFT: 1cm; WIDTH: 5cm; POSITION: absolute; TOP: 18.9cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WARRANTYNO" />
			</div>
			<div style="LEFT: 7.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 18.9cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WARRANTYPRICE" />
			</div>
			<div style="LEFT: 1cm; WIDTH: 16cm; POSITION: absolute; TOP: 20cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WARRANTYDESC1" />
			</div>
			<div style="LEFT: 1cm; WIDTH: 16cm; POSITION: absolute; TOP: 20.5cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WARRANTYDESC2" />
			</div>
			<div style="LEFT: 1cm; WIDTH: 5cm; POSITION: absolute; TOP: 22cm; HEIGHT: 0.5cm">
				<xsl:value-of select="PLANNEDDELIVERY" />
			</div>
			<div style="LEFT: 10cm; WIDTH: 5cm; POSITION: absolute; TOP: 22cm; HEIGHT: 0.5cm">
				<xsl:value-of select="EXPIRYOFWARRANTY" />
			</div>
			<div style="LEFT: 16.25cm; WIDTH: 5cm; POSITION: absolute; TOP: 25.5cm; HEIGHT: 0.5cm">
				<xsl:value-of select="TODAY" />
			</div>
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'TRUE'">
				<br class="pageBreak" />
			</xsl:if>
		</div>
	</xsl:template>
	
</xsl:stylesheet>

  