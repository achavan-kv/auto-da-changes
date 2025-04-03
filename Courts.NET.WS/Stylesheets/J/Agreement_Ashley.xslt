<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<head>
				<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"></meta>
				<style type="text/css" media="all">
                    @import url(<xsl:value-of select="AGREEMENTS/@CSSPATH"/>	);
                </style>
			</head>
			<BODY>	
				<xsl:apply-templates select="AGREEMENTS" />
			</BODY>
		</HTML>		
	</xsl:template>
	
	<xsl:template match="AGREEMENTS">	
		<xsl:apply-templates select="AGREEMENT" />		
		<P></P>
	</xsl:template>
	
	<xsl:template match="AGREEMENT">		
			<xsl:apply-templates select="PAGE" />	
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'TRUE'">
				<br class="pageBreak" />
			</xsl:if>	
			<P></P>		
	</xsl:template>
	
	<xsl:template match="PAGE">
		<div style="position:relative">		<!-- resets the position context -->
			<xsl:apply-templates select="HEADER" />		
			<P></P>									
			<xsl:apply-templates select="LINEITEMS" />
			<P></P>	
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'True'">
				<br class="pageBreak" />
			</xsl:if>						
			<xsl:if test="$last = 'True'">
				<div style="WIDTH: 18.5cm;
						POSITION: absolute;
						TOP: 12cm;
						HEIGHT: 3.5cm">
					<xsl:apply-templates select="../FOOTER" />
				</div>
				<div style="LEFT: 11.5cm;
								WIDTH: 7.5cm;
								POSITION: absolute;
								TOP: 16.5cm;
								HEIGHT: 4cm">
					<TABLE class="normal" width="100%" border="0">
						<TR>
							<TD>
								<xsl:variable name="ninety" select="../FOOTER/NINETYDAYS" />
								<xsl:if test="$ninety != ''">
									Courts will rebate to the Total Regular Price if payments of 
									<xsl:value-of select="../FOOTER/NINETYDAYS" />
									are made 30, 60 and 90 days after delivery
								</xsl:if> 	
								<xsl:variable name="paymentholiday" select="../FOOTER/PAYMENTHOLIDAYS" />
								<xsl:if test="$paymentholiday != ''">
									<xsl:value-of select="../FOOTER/PAYMENTHOLIDAYS" />
								</xsl:if>				
							</TD>
						</TR>
					</TABLE>
				</div>
			</xsl:if>
		</div>
	</xsl:template>
	
	<xsl:template match="HEADER">	
		<div style="LEFT: 12cm;
						WIDTH: 8cm;
						POSITION: absolute;
						TOP: 0cm;
						HEIGHT: 4cm">					
			<TABLE class="Narrow" width="100%">
				<TR>
					<TD width="100%">
						<xsl:value-of select="ACCTNO" />
					</TD>
				</TR>
				<TR>
					<TD width="100%">
						<xsl:value-of select="DATE" />
					</TD>
				</TR>
				<TR>
					<TD width="100%">
						<xsl:value-of select="NAME" />
					</TD>
				</TR>
				<TR>
					<TD width="100%">
						<xsl:value-of select="JOINTNAME" />
					</TD>
				</TR>
				<TR>
					<TD width="100%">
						<xsl:value-of select="ADDR1" />
					</TD>
				</TR>
				<TR>
					<TD width="100%">
						<xsl:value-of select="ADDR2" />
					</TD>
				</TR>
				<TR>
					<TD width="100%">
						<xsl:value-of select="ADDR3" />
					</TD>
				</TR>
				<TR>
					<TD width="100%">
						<xsl:value-of select="POSTCODE" />
					</TD>
				</TR>
				<TR>
					<TD width="100%">
						ID/IC No: <xsl:value-of select="CUSTID" />
					</TD>				
				</TR>
			</TABLE>
		</div>
		<P></P>
	</xsl:template>
	
	<xsl:template match="FOOTER">
		<TABLE class="normalSmall" width="100%" border="0">
			<TR>					
				<TD width="45%" valign="top">
					<TABLE class="Narrow" width="100%" border="0" cellspacing="20" >
						<TR>
							<TD>
                                Balance payable by <xsl:value-of select="../FOOTER/INSTALNO" /> instalments of <xsl:value-of select="../FOOTER/FIRSTINST" /> and a final instalment of <xsl:value-of select="../FOOTER/FINALINST" />.
                            </TD>
						</TR>
						<TR>
							<TD>
								Agree minimum payment <xsl:value-of select="TOPAY" />
							</TD>
						</TR>
						<TR>
							<TD>
								<xsl:variable name="servicePrint" select="SERVICEPRINT" />
								<xsl:if test="$servicePrint = 'A'">
									Interest Rate: <xsl:value-of select="INTERESTRATE" /> per annum.
								</xsl:if>
								<xsl:if test="$servicePrint = 'M'">
									Interest Rate: <xsl:value-of select="INTERESTRATE" /> per month.
								</xsl:if>	
								<xsl:if test="$servicePrint = 'L'">
									Interest Rate: <xsl:value-of select="INTERESTRATE" /> per month.
								</xsl:if>							
							</TD>
						</TR>
					</TABLE>
				</TD>
				<TD width="55%">
					<TABLE width="100%" border="0">
						<TR>
							<TD width="65%" align="right" height="19">
								Goods Value:
							</TD>
							<xsl:apply-templates select="GOODSVAL" />
						</TR>
						<TR>
							<TD width="65%" align="right">
								Deposit:
							</TD>
							<xsl:apply-templates select="DEPOSIT" />
						</TR>
						<TR>
							<TD width="65%" align="right">
								Balance/Credit Extended:
							</TD>
							<xsl:apply-templates select="CREDIT" />
						</TR>
						<TR>
							<TD width="65%" align="right">
								Charge for Credit:
							</TD>
							<xsl:apply-templates select="DT" />
						</TR>
						<TR>
							<TD width="65%" align="right">
								Balance Payable:
							</TD>
							<xsl:apply-templates select="BALANCE" />
						</TR>
						<TR>
							<TD width="65%" align="right">
								Total Price:
							</TD>
							<xsl:apply-templates select="TOTAL" />
						</TR>
					</TABLE>
				</TD>		
			</TR>
		</TABLE>		
	</xsl:template>
	
	<xsl:template match="LINEITEMS">
		<div class="normal" 
			style="LEFT: 0cm;
			WIDTH: 18.5cm;
			POSITION: absolute;
			TOP: 5.5cm;
			HEIGHT: 6cm">
			<xsl:apply-templates select="LINEITEM" />
		</div>
	</xsl:template>
	
	<xsl:template match="LINEITEM">
		<xsl:variable name="addTo" select="ADDTO" />
		<xsl:if test="$addTo = 'True'">
			<TABLE class="normalSmall" width="100%" border="0">
				<TR>
					<xsl:apply-templates select="QUANTITY" />	
					<TD align="left" width="85%">
						<xsl:value-of select="DESC" />
						 (<xsl:value-of select="ACCTNO" />)						
					</TD>		
				</TR>
			</TABLE>
		</xsl:if>
		<xsl:if test="$addTo != 'True'">
			<TABLE class="normalSmall" id="Items" width="100%" border="0">
				<TR>
					<xsl:apply-templates select="QUANTITY" />			
					<xsl:apply-templates select="DESC" />
					<xsl:apply-templates select="VALUE" />
				</TR>
				<xsl:if test="TRIM != '' and TRIM != ' '">
					<TR>
						<TD width="15%"></TD>
						<TD width="65%"><xsl:value-of select="TRIM" /></TD>
						<TD width="20%"></TD>
					</TR>
				</xsl:if>
				<xsl:if test="DESC2 != '' and DESC2 != ' '">
					<TR>
						<TD width="15%"></TD>
						<TD width="65%"><xsl:value-of select="DESC2" /></TD>
						<TD width="20%"></TD>
					</TR>
				</xsl:if>				
			</TABLE>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="QUANTITY">
		<TD align="left" width="15%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="DESC">
		<TD align="left" width="65%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>

	<xsl:template match="VALUE">
		<TD align="right" width="20%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="ACCTNO">	
		<TD width="40%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="DATE">	
		<TD width="40%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="NAME">	
		<TD width="40%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="ADDR1">
		<TD width="40%">
			<xsl:apply-templates />
		</TD>	
	</xsl:template>
	
	<xsl:template match="ADDR2">	
		<TD width="40%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="ADDR3">	
		<TD width="40%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="POSTCODE">	
		<TD width="40%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="CUSTID">	
		<xsl:apply-templates />
	</xsl:template>	
	
	<xsl:template match="GOODSVAL">	
		<TD align="right" height="19" width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="DEPOSIT">	
		<TD align="right" width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="CREDIT">	
		<TD align="right" width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="DT">	
		<TD align="right" width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="BALANCE">	
		<TD align="right" width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="TOTAL">	
		<TD align="right" width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
</xsl:stylesheet>
