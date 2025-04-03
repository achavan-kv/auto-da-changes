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
			<div class="smaller" style="LEFT: 15.25cm; WIDTH: 4cm; POSITION: absolute; TOP: 0.00cm; HEIGHT: 0.5cm">
				<xsl:value-of select="COPY" />
			</div>	
			<div style="LEFT: 17.5cm; WIDTH: 2cm; POSITION: absolute; TOP: 0.1cm; HEIGHT: 0.5cm">
				<xsl:value-of select="CONTRACTNO" />
			</div>
			<div style="LEFT: 16.4cm; WIDTH: 3cm; POSITION: absolute; TOP: 5cm; HEIGHT: 0.5cm">
				<xsl:value-of select="SOLDBY" />
			</div>
			<div style="LEFT: 16.4cm; WIDTH: 3cm; POSITION: absolute; TOP: 6.4cm; HEIGHT: 0.5cm">
				<xsl:value-of select="BRANCHNAME" />
			</div>
			<div style="LEFT: 16.4cm; WIDTH: 3cm; POSITION: absolute; TOP: 8cm; HEIGHT: 0.5cm">
				<xsl:value-of select="STORENO" />
			</div>
			<div style="LEFT: 16.4cm; WIDTH: 3cm; POSITION: absolute; TOP: 9.2cm; HEIGHT: 0.5cm">
				<xsl:value-of select="TODAY" />
			</div>
			<div style="LEFT: 16.4cm; WIDTH: 3cm; POSITION: absolute; TOP: 10.5cm; HEIGHT: 0.5cm">
				<xsl:value-of select="SOLDBYNAME" />
			</div>
			<div style="LEFT: 3cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO1" />
			</div>
			<div style="LEFT: 3.5cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO2" />
			</div>
			<div style="LEFT: 4cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO3" />
			</div>
			<div style="LEFT: 4.5cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO4" />
			</div>
			<div style="LEFT: 5cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO5" />
			</div>
			<div style="LEFT: 5.5cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO6" />
			</div>
			<div style="LEFT: 6cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO7" />
			</div>
			<div style="LEFT: 6.5cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO8" />
			</div>
			<div style="LEFT: 7cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO9" />
			</div>
			<div style="LEFT: 7.5cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO10" />
			</div>
			<div style="LEFT: 8cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO11" />
			</div>
			<div style="LEFT: 8.5cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 10.5cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
				<xsl:value-of select="ACCTNO12" />
			</div>
			<div style="LEFT: 2.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 5.1cm; HEIGHT: 0.5cm">
				<xsl:value-of select="FIRSTNAME" />
			</div>
			<div style="LEFT: 7.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 5.1cm; HEIGHT: 0.5cm">
				<xsl:value-of select="LASTNAME" />
			</div>
			<div style="LEFT: 3.5cm; WIDTH: 10cm; POSITION: absolute; TOP: 6.5cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ADDRESS1" />
			</div>
			<div style="LEFT: 3.5cm; WIDTH: 10cm; POSITION: absolute; TOP: 7cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ADDRESS2" />
			</div>
			<div style="LEFT: 3.5cm; WIDTH: 10cm; POSITION: absolute; TOP: 7.5cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ADDRESS3" />
			</div>
			<div style="LEFT: 3.5cm; WIDTH: 10cm; POSITION: absolute; TOP: 8cm; HEIGHT: 0.5cm">
				<xsl:value-of select="POSTCODE" />
			</div>
			<div style="LEFT: 3cm; WIDTH: 5cm; POSITION: absolute; TOP: 9.3cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WORKTEL" />
			</div>
			<div style="LEFT: 7.2cm; WIDTH: 5cm; POSITION: absolute; TOP: 9.3cm; HEIGHT: 0.5cm">
				<xsl:value-of select="HOMETEL" />
			</div>
			<div style="LEFT: 3.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 13.5cm; HEIGHT: 0.5cm">
				<xsl:value-of select="DATEOFPURCHASE" />
			</div>
			<div style="LEFT: 8.3cm; WIDTH: 5cm; POSITION: absolute; TOP: 13.5cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ITEMNO" />
			</div>
			<div style="LEFT: 4cm; WIDTH: 16cm; POSITION: absolute; TOP: 14.5cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ITEMDESC1" />
			</div>
			<div style="LEFT: 4cm; WIDTH: 16cm; POSITION: absolute; TOP: 15.3cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ITEMDESC2" />
			</div>
			<div style="LEFT: 3.5cm; WIDTH: 16cm; POSITION: absolute; TOP: 16.2cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ITEMPRICE" />
			</div>
			<div style="LEFT: 5cm; WIDTH: 5cm; POSITION: absolute; TOP: 17.3cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WARRANTYNO" />
			</div>
			<div style="LEFT: 11.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 17.3cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WARRANTYPRICE" />
			</div>
			<div style="LEFT: 3.65cm; WIDTH: 16cm; POSITION: absolute; TOP: 18.5cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WARRANTYDESC1" />
			</div>
			<div style="LEFT: 3.65cm; WIDTH: 16cm; POSITION: absolute; TOP: 19.3cm; HEIGHT: 0.5cm">
				<xsl:value-of select="WARRANTYDESC2" />
			</div>
			<xsl:if test="TERMSTYPE='WC'">
				<DIV class="smallPrint" style="Z-INDEX: 106; LEFT: 0.5cm; WIDTH: 10cm; POSITION: absolute; TOP: 20.7cm; HEIGHT: 0.317cm">
					Warranty purchased on credit. Customer has <xsl:value-of select="WARRANTYCREDIT" /> days after purchase of stock item to pay for warranty otherwise warranty will expire.
				</DIV>
			</xsl:if>
			<div style="LEFT: 5cm; WIDTH: 5cm; POSITION: absolute; TOP: 20.2cm; HEIGHT: 0.5cm">
				<xsl:value-of select="PLANNEDDELIVERY" />
			</div>
			<div style="LEFT: 15cm; WIDTH: 5cm; POSITION: absolute; TOP: 20.2cm; HEIGHT: 0.5cm">
				<xsl:value-of select="EXPIRYOFWARRANTY" />
			</div>
			<div style="LEFT: 17cm; WIDTH: 5cm; POSITION: absolute; TOP: 24.5cm; HEIGHT: 0.5cm">
				<xsl:value-of select="TODAY" />
			</div>
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'TRUE'">
				<br class="pageBreak" />
			</xsl:if>
		</div>
	</xsl:template>
	
</xsl:stylesheet>

  