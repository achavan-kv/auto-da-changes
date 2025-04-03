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
		<div style="position:relative">		<!-- resets the position context -->
			<xsl:apply-templates select="HEADER" />		
			<P></P>									
			<xsl:apply-templates select="LINEITEMS" />
			<P></P>	
			<div style="position:absolute; top:45%">
				<xsl:apply-templates select="FOOTER" />
			</div>			
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'TRUE'">
				<br class="pageBreak" />
			</xsl:if>
		</div>
	</xsl:template>
	
	<xsl:template match="HEADER">			
		<TABLE class="normal" width="700" border="0" cellspacing="10">
			<TR>
				<TD width="60%"></TD>
				<xsl:apply-templates select="ACCTNO" />
			</TR>
			<TR>
				<TD width="60%"></TD>
				<xsl:apply-templates select="DATE" />
			</TR>
		</TABLE>
		<TABLE class="Narrow" width="700" border="0">
			<TR>
				<TD width="60%"></TD>
				<xsl:apply-templates select="NAME" />
			</TR>
			<TR>
				<TD width="60%"></TD>
				<TD width="40%"></TD>
			</TR>
			<TR>
				<TD width="60%"></TD>
				<xsl:apply-templates select="ADDR1" />
			</TR>
			<TR>
				<TD width="60%"></TD>
				<xsl:apply-templates select="ADDR2" />
			</TR>
			<TR>
				<TD width="60%"></TD>
				<xsl:apply-templates select="ADDR3" />
			</TR>
			<TR>
				<TD width="60%"></TD>
				<xsl:apply-templates select="POSTCODE" />
			</TR>
			<TR>
				<TD width="60%"></TD>
				<TD width="40%" align="left">
					ID/IC No: 
					<xsl:apply-templates select="CUSTID" />
				</TD>				
			</TR>
		</TABLE>
		<P></P>
	</xsl:template>
	
	<xsl:template match="FOOTER">	
		<TABLE class="normal" width="700" border="0">
			<TR>					
				<TD width="40%" valign="top">
					<TABLE class="Narrow" width="100%" border="0" cellspacing="20" >
						<TR>
							<TD>
								<xsl:value-of select="INSTALMENTS" />
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
							</TD>
						</TR>
					</TABLE>
				</TD>
				<TD width="60%">
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
						<TR>
							<TD width="65%" align="right">
								<br/>
								<br/>
								<br/>
								<xsl:value-of select="TOPAY" />
								<br/>
								<br/>
								<br/>
								<br/>
								<br/>
								<br/>
								<br/>
								<br/>
								<br/>
								<xsl:value-of select="TOPAY" />
							</TD>
							<TD width="35%"></TD>
						</TR>
					</TABLE>
				</TD>		
			</TR>
		</TABLE>
		<P></P>
		<P></P>
		<TABLE class="normal" width="700" border="0">
			<TR>
				<TD width="60%"></TD>
				<TD width="40%">
					<xsl:variable name="ninety" select="NINETYDAYS" />
					<xsl:if test="$ninety != ''">
						Courts will rebate to the Total Regular Price if payments of 
						<xsl:value-of select="NINETYDAYS" />
						are made 30, 60 and 90 days after delivery
					</xsl:if> 					
				</TD>
			</TR>
		</TABLE>
		<xsl:apply-templates select="PB" />
	</xsl:template>
	
	<xsl:template match="LINEITEMS">
		<xsl:apply-templates select="LINEITEM" />
	</xsl:template>
	
	<xsl:template match="LINEITEM">
		<TABLE class="normal" id="Items" width="700" border="0">
			<TR>
				<xsl:apply-templates select="QUANTITY" />			
				<xsl:apply-templates select="DESC" />
				<xsl:apply-templates select="VALUE" />
			</TR>
			<TR>
				<TD width="15%"></TD>
				<TD width="65%">
					<xsl:value-of select="TRIM" />
				</TD>
				<TD width="20%"></TD>
			</TR>
			<TR>
				<TD width="15%"></TD>
				<TD width="65%">
					<xsl:value-of select="DESC2" />
				</TD>
				<TD width="20%"></TD>
			</TR>
		</TABLE>
		<xsl:apply-templates select="PB" />
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
	
	<xsl:template match="PB">		
		<br class="pageBreak" />
		<xsl:apply-templates select="../../../HEADER" />	
		<P></P>
	</xsl:template>
	
</xsl:stylesheet>
