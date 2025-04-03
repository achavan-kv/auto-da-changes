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
	<!--BOC CR 2018-13-->
	<xsl:template match="TAXINVOICE">
		<div style="position:relative" >
			<!-- resets the position context -->
			<xsl:apply-templates select="HEADER" />
			<xsl:apply-templates select="LINEITEMS" />
			<!-- Separate Invoice Total -->
			<hr/>
			<!-- jec 03/01/08 -->
			<xsl:apply-templates select="FOOTER" />
			<xsl:apply-templates select="PAY1" />
			<xsl:apply-templates select="PAYMETHODS" />
			<xsl:apply-templates select="PAY2" />
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'TRUE'">
				<br class="pageBreak" />
			</xsl:if>
		</div>
	</xsl:template>

	<xsl:template match="FOOTER">
		<!--<div style="position:absolute; top=60%; left=0%; width=100%" align="right">	 jec 02/01/08 overprinting-->
		<TABLE class="normal" height="7" width="700" border="0">
			<TR>
				<TD width="45%" align="left">					
				</TD>
				<TD width="10%"></TD>
				<TD width="45%" align="right" >
					<TABLE>
						<TR>
							<TD width="50%" align="left" class="bold">Total Amount: </TD>
							<TD width="50%" align="right">
								<xsl:value-of select="EXTOTAL" />
							</TD>
						</TR>
						<TR>
							<TD width="50%" align="left" class="bold">
								Tax Amount(<xsl:value-of select="TAXRATE" />%):
							</TD>
							<TD width="50%" align="right">
								<xsl:choose>
									<xsl:when test="TAXTOTAL='BZ$0.00'">
										Zero Rated
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="TAXTOTAL" />
									</xsl:otherwise>
								</xsl:choose>
							</TD>
						</TR>
						<TR>
							<TD width="50%" align="left" class="bold">Invoice Total: </TD>
							<TD width="50%" align="right">
								<xsl:value-of select="INCTOTAL" />
							</TD>
						</TR>
					</TABLE>
				</TD>
			</TR>
		</TABLE>

		<!--<TABLE>
						<TR>
							<TD width="60%"></TD>
							<TD width="25%" align="left" class="bold">Total Amount: </TD>
							<TD width="15%" align="right">
								<xsl:value-of select="EXTOTAL" />
							</TD>
						</TR>
						<TR>
							<TD width="60%"></TD>
							<TD width="25%" align="left" class="bold">
								Tax Amount(<xsl:value-of select="TAXRATE" />%):
							</TD>
							<TD width="15%" align="right">
								<xsl:choose>
									<xsl:when test="TAXTOTAL='BZ$0.00'">
										Zero Rated
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="TAXTOTAL" />
									</xsl:otherwise>
								</xsl:choose>
							</TD>
						</TR>
						<TR>
							<TD width="60%"></TD>
							<TD width="25%" align="left" class="bold">Invoice Total: </TD>
							<TD width="15%" align="right">
								<xsl:value-of select="INCTOTAL" />
							</TD>
						</TR>
					</TABLE>-->
		<!--</div> overprinting-->
	</xsl:template>

	<xsl:template match="HEADER">		
		<TABLE class="normal" height="7" width="600" border="0">			
			<TR>
				<td colspan="2" align="center">
					<br></br>
				</td>
			</TR>
			<TR>
				<td></td>
				<td>
					Unicomer <xsl:apply-templates select="COUNTRY" />
				</td>
			</TR>
			<TR>
				<td>
					<xsl:apply-templates select="BRANCHNAME" />
				</td>
			</TR>
			<TR>
				<td>
					<xsl:apply-templates select="BRANCHADDR1" />
				</td>
			</TR>
			<TR>
				<td>
					<xsl:apply-templates select="BRANCHADDR2" />
				</td>
			</TR>
			<TR>
				<td>
					<xsl:apply-templates select="BRANCHCITY" />
				</td>
			</TR>
			<TR>
				<td></td>
				<td align="left">
					<xsl:apply-templates select="REGNO" />
				</td>
			</TR>
			<TR>
				<TD>
					<br></br>
				</TD>
			</TR>
			<TR>
				<td></td>
				<td>
					<b>
						<font size="5">Tax Invoice<xsl:apply-templates select="REPRINTCOPY" /></font>
					</b>
				</td>
			</TR>
		</TABLE>

		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<b>
					<hr align="left" width="60%"></hr>
				</b>
			</TR>
			<TR>
				<td>
					Branch:
				</td>
				<td>
					<xsl:apply-templates select="BRANCHNO" />
				</td>
				<td></td>
				<td></td>
			</TR>
			<TR>
				<td>
					Ord/Invoice No:
				</td>
				<td>
					<xsl:apply-templates select="INVOICENO" />
				</td>
				<td>Invoice Date:</td>
				<td>
					<xsl:apply-templates select="INVOICEDATE" />
				</td>
			</TR>
			<TR>
				<td>
					Cashier:
				</td>
				<td>
					<xsl:apply-templates select="CASHIER" />
				</td>
				<td>
					Salesman:
				</td>
				<td>
					<xsl:apply-templates select="SALESMAN" />
				</td>
			</TR>
			<TR>
				<td>
					Customer Name:
				</td>
				<td>
					<xsl:apply-templates select="CUSTOMERNAME" />
				</td>
				<td></td>
				<td></td>
			</TR>
			<TR>
				<td>
					Customer Address:
				</td>
				<td>
					<xsl:apply-templates select="CUSTOMERADDR" />
				</td>
				<td></td>
				<td></td>
			</TR>
			<TR>
				<td>
					Account No:
				</td>
				<td>
					<xsl:apply-templates select="ACCTNO" />
				</td>
				<td></td>
				<td></td>
			</TR>
			<TR>
				<td>
					Account Balance:
				</td>
				<td>
					<xsl:apply-templates select="ACCTBLNC" />
				</td>
				<td>
					Available Spend:
				</td>
				<td>
					<xsl:apply-templates select="AVAILABLESPEND" />
				</td>
			</TR>
			<TR>
				<td>
					Date Printed:
				</td>
				<td>
					<xsl:value-of select="NOW" />
				</td>
				<td></td>
				<td></td>
			</TR>
		</TABLE>		

		<TABLE class="normal" width="700" border="0">
			<!--<TR>
				<TD>
					<xsl:apply-templates select="SALETEXT" />
				</TD>
			</TR>-->			
			<TR>
				<TD colspan="5">
					<hr align="left" width="100%"></hr>
				</TD>
			</TR>
			<TR>
				<TD class="heading" width="10%">Qty</TD>
				<TD class="heading" width="50%" align="left">Item Description</TD>
				<TD class="heading" width="15%" align="left">Amount</TD>
				<TD class="heading" width="15%" align="left">Tax </TD>

				<!--<xsl:apply-templates select="TAXNAME" />-->
				<TD class="heading" width="10%" align="right">NetAmt</TD>
			</TR>
		</TABLE>
	</xsl:template>

	<xsl:template match="LINEITEMS">
		<xsl:apply-templates select="LINEITEM">
			<!--<xsl:sort select="INDEX" />-->
			<!-- sort the remaining items on the index node -->
		</xsl:apply-templates>
	</xsl:template>

	<xsl:template match="LINEITEM">
		<xsl:choose>
			<xsl:when test="TYPE!='KitDiscount' and TYPE!='Component'" >
				<P></P>
				<TABLE class="normal" width="700" border="0">
					<TR>
						<TD width="10%"></TD>
						<TD width="50%"></TD>
						<TD width="15%"></TD>
						<TD width="15%">
							(<xsl:value-of select="TAXRATE" />%)
						</TD>
						<TD width="10%"></TD>
					</TR>
					<TR class="bold">
						<!--<TD width="5%"><xsl:apply-templates select="QUANTITY" /></TD>-->
						<xsl:apply-templates select="QUANTITY" />
						<TD width="50%" align="left">
							(<xsl:value-of select="ITEMNO" />)
							<xsl:value-of select="DESC" /> -
							<xsl:text> </xsl:text>
							<xsl:value-of select="DESC2" />
							<xsl:value-of select="MODEL"/>
						</TD>
						<!--<td width="15%" align="left">
							<xsl:value-of select="ITEMNO" />
						</td>-->
						<TD width="15%">
							<xsl:value-of select="ORDERVALUEEXTAX" />
						</TD>
						<TD width="15%">
							<xsl:choose>
								<xsl:when test="TAXAMOUNT='BZ$0.00'">
									Zero Rated
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="TAXAMOUNT" />
								</xsl:otherwise>
							</xsl:choose>
						</TD>
						<!--<TD width="15%"></TD>-->
						<TD width="10%">
							<xsl:value-of select="ORDERVALUE" />
						</TD>
					</TR>
					<xsl:if test="CONTRACTNO != ''">
						<tr>
							<td></td>
							<td></td>
							<td>
								Contract No - <xsl:value-of select="CONTRACTNO" />
							</td>
						</tr>
					</xsl:if>
				</TABLE>
				<TABLE class="normal" width="700" border="0">
					<TR>
						<TD width="10%"></TD>
						<td width="20%" align="left">
							<!--<xsl:value-of select="TAXRATE" />%-->
						</td>
						<td width="30%" align="right">
							<!--<xsl:value-of select="ORDERVALUEEXTAX" />-->
						</td>
						<!--<td width="20%" align="right">
							<xsl:choose>
								<xsl:when test="TAXAMOUNT='BZ$0.00'">
									Zero Rated
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="TAXAMOUNT" />
								</xsl:otherwise>
							</xsl:choose>
						</td>-->
						<td width="20%" align="right">
							<!--<xsl:value-of select="ORDERVALUE" />-->
						</td>
					</TR>
				</TABLE>
				<xsl:if test="(TYPE='Warranty' or TYPE='KitWarranty') and TERMSTYPE='WC'">
					<TABLE class="normal" width="600" border="0">
						<TR>
							<TD width="20%"></TD>
							<td width="80%" align="left">
								Warranty to Pay
							</td>
						</TR>
					</TABLE>
				</xsl:if>
				<xsl:apply-templates select="SERIALNOS" />
				<xsl:apply-templates select="PB" />
				<xsl:apply-templates select="RELATED" />
			</xsl:when>
			<xsl:otherwise>
				<TABLE class="smaller" width="600" border="0">
					<TR>
						<xsl:apply-templates select="QUANTITY" />
						<td width="15%" align="left">
							<xsl:value-of select="ITEMNO" />
						</td>
						<TD width="75%" align="left">
							<xsl:value-of select="DESC" />
							<xsl:text> </xsl:text>
							<xsl:value-of select="DESC2" />
						</TD>
					</TR>
				</TABLE>

				<xsl:apply-templates select="RELATED" />
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>
	<!--BOC CR 2018-13-->
	<xsl:template match="PAYMETHODS">
		<xsl:apply-templates select="PAYMETHOD">
		</xsl:apply-templates>
	</xsl:template>
	
	<xsl:template match="PAY1">
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD width="50%">Paid By:</TD>
				<TD width="50%"></TD>
			</TR>
		</TABLE>
	</xsl:template>
	<xsl:template match="PAYMETHOD">
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD width="30%" align="left" class="bold">
					<xsl:value-of select="DESCRIPTION" />:
				</TD>
				<TD width="30%" align="right">
					<xsl:value-of select="AMOUNT" />
				</TD>
				<TD width="40%"></TD>
			</TR>
			<TR></TR>
		</TABLE>
	</xsl:template>
	
	<xsl:template match="PAY2">		
		<TABLE class="normal" height="7" width="600" border="0">
			<TR>
				<TD width="30%"><b>Total Amt Paid:</b></TD>
				<TD width="30%" align="right"><xsl:apply-templates select="TOTALAMTPAID" /></TD>
				<TD width="40%"></TD>
			</TR>
		</TABLE>
	</xsl:template>
	<!--EOC CR 2018-13-->
	<xsl:template match="RELATED">
		<xsl:apply-templates select="LINEITEM" />
	</xsl:template>

	<xsl:template match="BRANCHNAME">
		<TD width="70%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="ACCTNO">
		<!--<TD width="30%">-->
			<xsl:apply-templates />
		<!--</TD>-->
	</xsl:template>

	<xsl:template match="BRANCHADDR1">
		<TD width="70%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="BUFFNO">
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
		<TD width="30%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="ADDR2">
		<TD width="30%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="ADDR3">
		<TD width="30%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="POSTCODE">
		<TD width="30%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="QUANTITY">
		<TD width="10%" align="left">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="DESC">
		<TD width="40%" align="left">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="DESC2">
		<TD width="40%" align="left">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="TAXRATE">
		<TD width="20%" align="left">
			<xsl:apply-templates />%
		</TD>
	</xsl:template>

	<xsl:template match="PRICE">
		<TD width="20%" align="right">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="TAXAMOUNT">
		<TD width="20%" align="right">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="TAXNAME">
		<TD class="heading" width="20%" align="right">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="INCLUSIVE">
		<TD width="20%" align="right">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="EXTOTAL">
		<TD width="25%" align="right">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="INCTOTAL">
		<TD width="25%" align="right">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="TAXTOTAL">
		<TD width="25%" align="right">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="SALETEXT">
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="PB">
		<br class="pageBreak" />
		<xsl:apply-templates select="../../../HEADER" />
	</xsl:template>	

</xsl:stylesheet>