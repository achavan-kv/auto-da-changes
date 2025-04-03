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
		<div class="normal" style="position:relative">		<!-- resets the position context -->			
			
			<xsl:apply-templates select="HEADER" />	
			
			<!-- Line Items -->
			<xsl:if test="HEADER/JOINTNAME!=''">
				<div style="WIDTH: 18.5cm; POSITION: absolute; TOP: 6.4cm; HEIGHT: 6cm">
					<xsl:apply-templates select="LINEITEMS" />			
				</div>	
			</xsl:if>
			<xsl:if test="HEADER/JOINTNAME=''">
				<div style="WIDTH: 18.5cm; POSITION: absolute; TOP: 5.4cm; HEIGHT: 6cm">
					<xsl:apply-templates select="LINEITEMS" />			
				</div>	
			</xsl:if>
			
			
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'True'">
				<br class="pageBreak" />
			</xsl:if>						
			<xsl:if test="$last = 'True'">
							
				<!-- Totals -->
				<xsl:if test="HEADER/JOINTNAME != ''">
					<div style="LEFT: 9.3cm; WIDTH: 9.2cm; POSITION: absolute; TOP: 12.4cm; HEIGHT: 6cm">
						<table style="LINE-HEIGHT: 16pt" class="normal" width="100%" cellSpacing="0">
							<tr>
								<td width="30%" align="right">
									<xsl:value-of select="../FOOTER/PRETAXGOODSVAL" /><br/>
									<xsl:value-of select="../FOOTER/GOODSVALTAX" /><br/>
									<xsl:value-of select="../FOOTER/GOODSVAL" /><br/>
									<xsl:value-of select="../FOOTER/DEPOSIT" /><br/>
									<xsl:value-of select="../FOOTER/CREDIT" /><br/>
									<xsl:value-of select="../FOOTER/PRETAXDT" /><br/>
									<xsl:value-of select="../FOOTER/DTTAX" /><br/>
									<xsl:value-of select="../FOOTER/BALANCE" /><br/>
									<xsl:value-of select="../FOOTER/TOTAL" />
								</td>
							</tr>
						</table>
					</div>
				</xsl:if>
				<xsl:if test="HEADER/JOINTNAME = ''">
					<div style="LEFT: 9.3cm; WIDTH: 9.2cm; POSITION: absolute; TOP: 11.4cm; HEIGHT: 6cm">
						<table style="LINE-HEIGHT: 16pt" class="normal" width="100%" cellSpacing="0">
							<tr>
								<td width="30%" align="right">
									<xsl:value-of select="../FOOTER/PRETAXGOODSVAL" /><br/>
									<xsl:value-of select="../FOOTER/GOODSVALTAX" /><br/>
									<xsl:value-of select="../FOOTER/GOODSVAL" /><br/>
									<xsl:value-of select="../FOOTER/DEPOSIT" /><br/>
									<xsl:value-of select="../FOOTER/CREDIT" /><br/>
									<xsl:value-of select="../FOOTER/PRETAXDT" /><br/>
									<xsl:value-of select="../FOOTER/DTTAX" /><br/>
									<xsl:value-of select="../FOOTER/BALANCE" /><br/>
									<xsl:value-of select="../FOOTER/TOTAL" />
								</td>
							</tr>
						</table>
					</div>
				</xsl:if>
				
				<!-- Interest Rate -->
				<xsl:if test="HEADER/JOINTNAME != ''">
					<div class="normal" style="LEFT: 8cm; WIDTH: 1cm; POSITION: absolute; TOP: 15.3cm; HEIGHT: 0.5cm">
						<xsl:value-of select="../FOOTER/INTERESTRATE" />
					</div>
				</xsl:if>
				<xsl:if test="HEADER/JOINTNAME = ''">
					<div class="normal" style="LEFT: 8cm; WIDTH: 1cm; POSITION: absolute; TOP: 14.3cm; HEIGHT: 0.5cm">
						<xsl:value-of select="../FOOTER/INTERESTRATE" />
					</div>
				</xsl:if>
				
			</xsl:if>						
		</div>		
	</xsl:template>	
	
	<xsl:template match="HEADER">
		
		<!-- accountno -->
		<div style="LEFT: 10.5cm; WIDTH: 4cm; POSITION: absolute; TOP: 0.7cm; HEIGHT: 0.5cm">
			<xsl:value-of select="ACCTNO"></xsl:value-of>
		</div>
			
		<xsl:if test="JOINTNAME = ''">
			<!-- Name -->
			<div style="LEFT: 1.2cm; WIDTH: 9cm; POSITION: absolute; TOP: 2.6cm; HEIGHT: 0.5cm">
				<xsl:value-of select="NAME" />
			</div>
			
			<!-- custid -->
			<div style="LEFT: 2cm; WIDTH: 7cm; POSITION: absolute; TOP: 3.2cm; HEIGHT: 0.5cm">
				<xsl:value-of select="CUSTID"></xsl:value-of>
			</div>		
			
			<!-- Address -->
			<div style="LEFT: 2cm; WIDTH: 15cm; POSITION: absolute; TOP: 3.7cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ADDR1" />,<xsl:value-of select="ADDR2" />,<xsl:value-of select="ADDR3" />,<xsl:value-of select="POSTCODE" />
			</div>
		</xsl:if>
	
		<xsl:if test="JOINTNAME != ''">
			<!-- Name -->
			<div style="LEFT: 1.2cm; WIDTH: 9cm; POSITION: absolute; TOP: 2.4cm; HEIGHT: 0.5cm">
				<xsl:value-of select="NAME" />
			</div>
			
			<!-- custid -->
			<div style="LEFT: 2cm; WIDTH: 7cm; POSITION: absolute; TOP: 2.85cm; HEIGHT: 0.5cm">
				<xsl:value-of select="CUSTID"></xsl:value-of>
			</div>		
			
			<!-- Address -->
			<div style="LEFT: 2cm; WIDTH: 15cm; POSITION: absolute; TOP: 3.3cm; HEIGHT: 0.5cm">
				<xsl:value-of select="ADDR1" />,<xsl:value-of select="ADDR2" />,<xsl:value-of select="ADDR3" />,<xsl:value-of select="POSTCODE" />
			</div>
			<!-- Joint applicant details -->		
			<!-- Name -->
			<div style="LEFT: 1.2cm; WIDTH: 9cm; POSITION: absolute; TOP: 3.8cm; HEIGHT: 0.5cm">
				<xsl:value-of select="JOINTNAME" />
			</div>
			
			<!-- custid -->
			<div style="LEFT: 2cm; WIDTH: 7cm; POSITION: absolute; TOP: 4.35cm; HEIGHT: 0.5cm">
				<xsl:value-of select="JOINTID"></xsl:value-of>
			</div>		
			
			<!-- Address -->
			<div style="LEFT: 2cm; WIDTH: 15cm; POSITION: absolute; TOP: 4.9cm; HEIGHT: 0.5cm">
				<xsl:value-of select="JOINTADDR1" />,<xsl:value-of select="JOINTADDR2" />,<xsl:value-of select="JOINTADDR3" />,<xsl:value-of select="JOINTPOSTCODE" />
			</div>
		</xsl:if>	
		
	</xsl:template>
	
	<xsl:template match="LINEITEMS">
		<xsl:apply-templates select="LINEITEM" />
	</xsl:template>
	
	<xsl:template match="LINEITEM">
		<xsl:variable name="addTo" select="ADDTO" />
		<xsl:if test="$addTo = 'True'">
			<TABLE class="normalSmall" width="100%" cellSpacing="0">
				<TR>
					<TD align="left" width="15%">
						<xsl:value-of select="QUANTITY" />
					</TD>
					<TD align="left" width="85%">
						<xsl:value-of select="DESC" />
						(<xsl:value-of select="ACCTNO" />)
					</TD>					
				</TR>
			</TABLE>
		</xsl:if>
		<xsl:if test="$addTo != 'True'">
			<table class="normal" width="100%" cellSpacing="0">
				<tr>
					<td width="10%">
						<xsl:value-of select="QUANTITY" />
					</td>
					<td width="15%">
						<xsl:value-of select="ITEMNO" />
					</td>
					<td width="60%">
						<xsl:value-of select="DESC" />
					</td>
					<td width="15%" align="right">
						<xsl:value-of select="PRETAXVALUE" />
					</td>
				</tr>	
			</table>
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>
