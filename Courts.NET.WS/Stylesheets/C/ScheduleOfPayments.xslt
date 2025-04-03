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
				<xsl:apply-templates select="SCHEDULEOFPAYMENTS" />
			</BODY>
		</HTML>		
	</xsl:template>
	
	<xsl:template match="SCHEDULEOFPAYMENTS">
		<xsl:apply-templates select="HEADER" />
		<p></p>
		<table width="700">
			<tr class="RFHead1" >
				<td width ="25%">ECHEANCE</td>
				<td width ="75%">MENSUALITES</td>
			</tr>
			<xsl:apply-templates select="INSTALMENTS" />
		</table>
		<p></p>		
		<xsl:apply-templates select="FOOTER" />
	</xsl:template>	
	
	<xsl:template match="HEADER">
		<table width="700" class="RFHead1">
			<tr>
				<td>Courts (Madagascar) SARL</td>
			</tr>
		</table>
		<p></p>
		<table width="700">
			<tr>
				<td class="RFHead1" width="20%">ANNEXE 1</td>
				<td class="RFBody"  width="80%"></td>
			</tr>
			<tr>
				<td class="RFHead2" >Nom do Client:</td>
				<td class="RFBody"  ><xsl:value-of select="CUSTOMERNAME" /></td>
			</tr>
			<tr>
				<td class="RFHead2" >N de compte:</td>
				<td class="RFBody"><xsl:value-of select="ACCTNO" /></td>
			</tr>
			<tr>
				<td class="RFHead2" >Date prevue de livraison:</td>
				<td class="RFBody"><xsl:value-of select="AGRDATE" /></td>
			</tr>
			<tr>
				<td class="RFHead2" >Valeur Total:</td>
				<td class="RFBody"><xsl:value-of select="TOTAL" /></td>
			</tr>
			<tr>
				<td class="RFHead2" >A Compte:</td>
				<td class="RFBody"><xsl:value-of select="DEPOSIT" /></td>
			</tr>
			<tr>
				<td class="RFHead2" >Solde dû:</td>
				<td class="RFBody"><xsl:value-of select="BALANCE" /></td>
			</tr>
		</table>
		<p></p>
	</xsl:template>
	
	<xsl:template match="INSTALMENTS">
		<xsl:apply-templates select="INSTALMENT" />
	</xsl:template>
	
	<xsl:template match="INSTALMENT">
		<tr class="RFBody" >
			<td><xsl:value-of select="DUEDATE" /></td>
			<td><xsl:value-of select="MONTHLYINSTALMENT" /></td>
		</tr>
	</xsl:template>
	
	<xsl:template match="FOOTER">
		<table width="700" class="RFHead2">
			<tr>
				<td width="25%"></td>
				<td width="75%"></td>
			</tr>
			<tr>
				<td class="RFHead2" >TOTAL:</td>
				<td class="RFBody"  ><xsl:value-of select="TOTAL" /></td>
			</tr>
		</table>

		<p></p>

		<table width="700" class="RFHead2">
			<tr>
				<td width="50%" align="center"></td>
				<td width="50%" align="center"></td>
			</tr>
			<tr><td ></td><td ></td></tr>
			<tr><td ></td><td ></td></tr>
			<tr><td ></td><td ></td></tr>
			<tr><td ></td><td ></td></tr>
			<tr><td ></td><td ></td></tr>
			<tr><td ></td><td ></td></tr>
			<tr><td ></td><td ></td></tr>
			<tr>
				<td class="RFHead1" width="50%" align="center">____________________</td>
				<td class="RFHead1" width="50%" align="center">____________________</td>
			</tr>
		</table>
		<p></p>
		<table width="700" class="RFHead2">
			<tr>
				<td class="RFHead1" width="50%" align="center">Signature de Client</td>
				<td class="RFHead1" width="50%" align="center">Signature de Vendeur</td>
			</tr>
		</table>
		<p></p>
	</xsl:template>
</xsl:stylesheet>

  

  

  