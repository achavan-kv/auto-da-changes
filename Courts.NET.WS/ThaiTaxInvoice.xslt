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
				<xsl:apply-templates select="TAXINVOICES" />
			</BODY>
		</HTML>
	</xsl:template>
	
	<xsl:template match="TAXINVOICES">	
		<xsl:apply-templates select="TAXINVOICE" />		
		<P></P>
	</xsl:template>

	<xsl:template match="TAXINVOICE">
		<div style="position:relative; TOP:4.5cm">
			<xsl:apply-templates select="HEADER" />	
			<div style="POSITION: absolute; TOP: 6cm">	
				<xsl:apply-templates select="LINEITEMS" />
				<P></P>
				<p></p>	
			</div>
			<div style="POSITION: absolute; TOP: 12.5cm">				
				<TABLE class="ThaiLineItem" height="7" width="660" border="0" cellspacing="6">
					<TR>		<!-- Total with tax -->
						<TD width="80%" align="center"></TD>
						<TD width="20%" align="right">
							<xsl:value-of select="format-number(sum(.//LINEITEM[TYPE!='KitDiscount' and TYPE!='Component']/ORDERVALUE), '#,##0.00')" />
						</TD>
					</TR>
				</TABLE>
				<TABLE class="ThaiLineItem" height="7" width="660" border="0" cellspacing="8">
					<tr>		<!-- Total before tax -->
						<TD width="100%" align="right">
							<xsl:value-of select="format-number(sum(.//LINEITEM[TYPE!='KitDiscount' and TYPE!='Component']/ORDERVALUE) - sum(.//LINEITEM[TYPE!='KitDiscount' and TYPE!='Component']/TAXAMOUNT), '#,##0.00')" />
						</TD>
					</tr>
					<xsl:if test="CREDITNOTE='True'">
						<tr>		<!-- Correct Price -->
							<TD width="100%" align="right">0</TD>
						</tr>
						<tr>		<!-- Total before tax -->
							<TD width="100%" align="right">
								<xsl:value-of select="format-number(sum(.//LINEITEM[TYPE!='KitDiscount' and TYPE!='Component']/ORDERVALUE) - sum(.//LINEITEM[TYPE!='KitDiscount' and TYPE!='Component']/TAXAMOUNT), '#,##0.00')" />
							</TD>
						</tr>
					</xsl:if>
					<tr>		<!-- Amount of tax -->			
						<TD width="100%" align="right">
							<xsl:value-of select="format-number(sum(.//LINEITEM[TYPE!='KitDiscount' and TYPE!='Component']/TAXAMOUNT), '#,##0.00')" />
						</TD>
					</tr>	
					<tr>		<!-- Total Again -->		
						<TD width="100%" align="right">
							<xsl:value-of select="format-number(sum(.//LINEITEM[TYPE!='KitDiscount' and TYPE!='Component']/ORDERVALUE), '#,##0.00')" />
						</TD>
					</tr>
				</TABLE>
			</div>
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'TRUE'">
				<br class="pageBreak" />
			</xsl:if>
		</div>
	</xsl:template>

	<xsl:template match="HEADER">
		<TABLE class="normal" height="5" width="660" border="0">
			<tr>
				<td width="10%" >
				</td>
				<td width="70%" align="center">
					<xsl:value-of select="DATE" />
				</td>
				<td width="20%">
					<xsl:value-of select="ACCTNO" />
				</td>							
			</tr>
			<tr>
				<td width="10%" >
				</td>
				<td width="70%"> 
				</td>
				<td width="20%">
					<xsl:value-of select="BUFFNO" />
				</td>							
			</tr>		
		</TABLE>
		<TABLE class="normal" height="7" width="660" border="0" cellspacing="15">
			<xsl:if test="CUSTOMERID != 'PAID &amp; TAKEN'">
				<TR>
					<td width="10%"></td>
					<TD width="90%">
						<xsl:apply-templates select="FIRSTNAME" />
						<xsl:apply-templates select="LASTNAME" />
					</TD>				
				</TR>
				<TR>
					<td width="10%"></td>
					<td width="90%">
						<xsl:value-of select="ADDR1" />,
						<xsl:value-of select="ADDR2" />, 
						<xsl:value-of select="ADDR3" />, 
						<xsl:value-of select="POSTCODE" /> 
					</td>
				</TR>			
			</xsl:if>			
		</TABLE>
		<P>
		</P>
	</xsl:template>

	<xsl:template match="LINEITEMS">
		<xsl:apply-templates select="LINEITEM">
			<xsl:sort select="INDEX" />		<!-- sort the remaining items on the index node -->
		</xsl:apply-templates>	
	</xsl:template>	
	
	<xsl:template match="LINEITEM">		
		<xsl:choose>
			<xsl:when test="TYPE!='KitDiscount' and TYPE!='Component'" >
				<TABLE class="ThaiLineItem" width="660" border="0">
					<TR>
						<td width="6%">
							<xsl:value-of select="NUMBER" />
						</td>
						<td width="9%">
							<xsl:value-of select="ITEMNO" />
						</td>
						<td width="50%">
							<xsl:value-of select="DESC" />
						</td>
						<td width="5%" align="center">
							<xsl:value-of select="QUANTITY" />
						</td>
						<td width="15%" align="right">
							<xsl:value-of select="format-number(UNITPRICE, '#,##0.00')" />
						</td>
						<td width="15%" align="right">
							<xsl:value-of select="format-number(ORDERVALUE, '#,##0.00')" />
						</td>
					</TR>
					<TR>
						<td width="6%"></td>
						<td width="9%"></td>
						<td width="50%">
							<xsl:value-of select="DESC2" />
						</td>
						<td width="5%" align="center"></td>
						<td width="15%" align="right"></td>
						<td width="15%" align="right"></td>
					</TR>
				</TABLE>
				<xsl:apply-templates select="RELATED" />
			</xsl:when>
			<xsl:otherwise>
				<TABLE class="ThaiComponent" width="660" border="0">
					<TR>
						<td width="5%">
							<xsl:value-of select="NUMBER" />
						</td>
						<td width="10%">
							<xsl:value-of select="ITEMNO" />
						</td>
						<td width="50%">
							<xsl:value-of select="DESC" />
						</td>
						<td width="5%" align="center">
							<xsl:value-of select="QUANTITY" />
						</td>
						<td width="15%"></td>
						<td width="15%"></td>
					</TR>
					<TR>
						<td width="5%"></td>
						<td width="10%"></td>
						<td width="50%">
							<xsl:value-of select="DESC2" />
						</td>
						<td width="5%" align="center"></td>
						<td width="15%"></td>
						<td width="15%"></td>
					</TR>
				</TABLE>
				<xsl:apply-templates select="RELATED" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xsl:template match="RELATED">
		<xsl:apply-templates select="LINEITEM" />
	</xsl:template>
	
	<xsl:template match="BRANCHNAME">	
		<TD width="70%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="ACCTNO">	
		<TD width="30%">
			TAX INVOICE: <xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="BRANCHADDR1">	
		<TD width="70%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="SERIALNO">	
		<TD width="30%">
			Serial No: <xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="BRANCHADDR2">	
		<TD width="70%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="BRANCHADDR3">	
		<TD width="70%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="FIRSTNAME">	
			<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="LASTNAME">	
			<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="ADDR1">
		<td width="20%"></td>	
		<TD width="80%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="ADDR2">
		<td width="20%"></td>	
		<TD width="80%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="ADDR3">	
		<td width="20%"></td>
		<TD width="80%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="POSTCODE">	
		<td width="20%"></td>
		<TD width="80%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	


</xsl:stylesheet>

  