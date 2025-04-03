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
			<div class="smaller" style="LEFT: 15cm; WIDTH: 4cm; POSITION: absolute; TOP: -0cm; HEIGHT: 0.5cm">
				<xsl:value-of select="COPY" />
			</div>	
			<div style="LEFT: 15.25cm; WIDTH: 2cm; POSITION: absolute; TOP: 0.6cm; HEIGHT: 0.5cm">
				<xsl:value-of select="CONTRACTNO" />
			</div>
			<div style="LEFT: 14.75cm; WIDTH: 3cm; POSITION: absolute; TOP: 5.25cm; HEIGHT: 0.5cm">
				<xsl:value-of select="SOLDBY" />
			</div>
			<div style="LEFT: 14.75cm; WIDTH: 3cm; POSITION: absolute; TOP: 6.3cm; HEIGHT: 0.5cm">
				<xsl:value-of select="BRANCHNAME" />
			</div>
			<div style="LEFT: 14.75cm; WIDTH: 3cm; POSITION: absolute; TOP: 7.9cm; HEIGHT: 0.5cm">
				<xsl:value-of select="STORENO" />
			</div>
			<div style="LEFT: 14.75cm; WIDTH: 3cm; POSITION: absolute; TOP: 9.2cm; HEIGHT: 0.5cm">
				<xsl:value-of select="TODAY" />
			</div>
			<div style="LEFT: 14.75cm; WIDTH: 3cm; POSITION: absolute; TOP: 10.3cm; HEIGHT: 0.5cm">
				<xsl:value-of select="SOLDBYNAME" />
			</div>
			<div style="LEFT: 1.95cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.45cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO1" />
			</div>
			<div style="LEFT: 2.75cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.45cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO2" />
			</div>
			<div style="LEFT: 3.55cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.45cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO3" />
			</div>
			<div style="LEFT: 4.35cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.45cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO4" />
			</div>
			<div style="LEFT: 5.15cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.45cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO5" />
			</div>
			<div style="LEFT: 5.95cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.45cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO6" />
			</div>
			<div style="LEFT: 6.75cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.45cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO7" />
			</div>
			<div style="LEFT: 7.55cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.45cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO8" />
			</div>
			<div style="LEFT: 8.35cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.45cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO9" />
			</div>
			<div style="LEFT: 9.15cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.45cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO10" />
			</div>
			<div style="LEFT: 9.95cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.45cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO11" />
			</div>
			<div style="LEFT: 10.75cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.45cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO12" />
			</div>
			<div style="LEFT: 1.3cm; WIDTH: 5cm; POSITION: absolute; TOP: 5.65cm; HEIGHT: 0.5cm">
				<xsl:value-of select="FIRSTNAME" />
			</div>
			<div style="LEFT: 5.8cm; WIDTH: 5cm; POSITION: absolute; TOP: 5.65cm; HEIGHT: 0.5cm">
				<xsl:value-of select="LASTNAME" />
			</div>
			<div style="LEFT: 1.5cm; WIDTH: 10cm; POSITION: absolute; TOP: 6.35cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ADDRESS1" />
			</div>
			<div style="LEFT: 1.5cm; WIDTH: 10cm; POSITION: absolute; TOP: 6.85cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ADDRESS2" />
			</div>
			<div style="LEFT: 1.5cm; WIDTH: 10cm; POSITION: absolute; TOP: 7.35cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ADDRESS3" />
			</div>
			<div style="LEFT: 1.5cm; WIDTH: 10cm; POSITION: absolute; TOP: 7.85cm; HEIGHT: 0.5cm">
				<xsl:value-of select="POSTCODE" />
			</div>
			<div style="LEFT: 1.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 9.1cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WORKTEL" />
			</div>
			<div style="LEFT: 6.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 9.1cm; HEIGHT: 0.5cm">
				<xsl:value-of select="HOMETEL" />
			</div>
			<div style="LEFT: 0.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 12.95cm; HEIGHT: 0.5cm">
				<xsl:value-of select="DATEOFPURCHASE" />
			</div>
			<div style="LEFT: 7cm; WIDTH: 5cm; POSITION: absolute; TOP: 12.95cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ITEMNO" />
			</div>
			<div style="LEFT: 2.5cm; WIDTH: 16cm; POSITION: absolute; TOP: 13.95cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ITEMDESC1" />
			</div>
			<div style="LEFT: 1cm; WIDTH: 16cm; POSITION: absolute; TOP: 14.45cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ITEMDESC2" />
			</div>
			<div style="LEFT: 4cm; WIDTH: 16cm; POSITION: absolute; TOP: 15.35cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ITEMPRICE" />
			</div>
			<div style="LEFT: 1cm; WIDTH: 5cm; POSITION: absolute; TOP: 16.47cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WARRANTYNO" />
			</div>
			<div style="LEFT: 7.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 16.47cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WARRANTYPRICE" />
			</div>
			<div style="LEFT: 1cm; WIDTH: 16cm; POSITION: absolute; TOP: 17.4cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WARRANTYDESC1" />
			</div>
			<div style="LEFT: 1cm; WIDTH: 16cm; POSITION: absolute; TOP: 17.8cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WARRANTYDESC2" />
			</div>
			<xsl:if test="TERMSTYPE='WC'">
				<DIV class="smallPrint" style="Z-INDEX: 106; LEFT: 0.5cm; WIDTH: 10cm; POSITION: absolute; TOP: 20.7cm; HEIGHT: 0.317cm">
					Warranty purchased on credit. Customer has <xsl:value-of select="WARRANTYCREDIT" /> days after purchase of stock item to pay for warranty otherwise warranty will expire.
				</DIV>
			</xsl:if>
			<div style="LEFT: 1cm; WIDTH: 5cm; POSITION: absolute; TOP: 19.4cm; HEIGHT: 0.5cm">
				<xsl:value-of select="PLANNEDDELIVERY" />
			</div>
			<div style="LEFT: 9cm; WIDTH: 5cm; POSITION: absolute; TOP: 19.2cm; HEIGHT: 0.5cm">
				<xsl:value-of select="EXPIRYOFWARRANTY" />
			</div>
			<div style="LEFT: 15.9cm; WIDTH: 5cm; POSITION: absolute; TOP: 22.7cm; HEIGHT: 0.5cm">
				<xsl:value-of select="TODAY" />
			</div>
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'TRUE'">
				<br class="pageBreak" />
			</xsl:if>
		</div>
	</xsl:template>
	
</xsl:stylesheet>

  