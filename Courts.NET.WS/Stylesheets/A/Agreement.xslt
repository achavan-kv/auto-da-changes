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
			<div style="WIDTH: 18.5cm; POSITION: absolute; TOP: 13cm; HEIGHT: 6cm">
				<xsl:apply-templates select="LINEITEMS" />			
			</div>	
			
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'True'">
				<br class="pageBreak" />
			</xsl:if>						
			<xsl:if test="$last = 'True'">
				<!-- Instalments summary -->
				<div class="normalSmall" style="WIDTH: 8.2cm; POSITION: absolute; TOP: 19.2cm; HEIGHT: 0.5cm">
					The balance to be paid by monthly rents of
				</div>
				<div class="normalSmall" style="WIDTH: 2.5cm; POSITION: absolute; TOP: 19.7cm; HEIGHT: 0.5cm">
					<xsl:value-of select="../FOOTER/FIRSTINST" />
				</div>
				<div class="normalSmall" style="LEFT: 2cm; WIDTH: 5.5cm; POSITION: absolute; TOP: 19.7cm; HEIGHT: 0.5cm">
					payable on the
				</div>
				<div class="normalSmall" style="LEFT: 5.5cm; WIDTH: 2.5cm; POSITION: absolute; TOP: 19.7cm; HEIGHT: 0.5cm">
					day of each
				</div>
				<div class="normalSmall" style="LEFT: 2cm; WIDTH: 5.5cm; POSITION: absolute; TOP: 20.2cm; HEIGHT: 0.5cm">
					commencing on the
				</div>
				<div class="normalSmall" style="LEFT: 6cm; WIDTH: 1.5cm; POSITION: absolute; TOP: 20.2cm; HEIGHT: 0.5cm">
					day of
				</div>
				<div class="normalSmall" style="LEFT: 2cm; WIDTH: 5.5cm; POSITION: absolute; TOP: 20.7cm; HEIGHT: 0.5cm">
					payable at the owners place of business
				</div>
				
				<!-- Totals -->
				<div style="LEFT: 9.3cm; WIDTH: 9.2cm; POSITION: absolute; TOP: 19.5cm; HEIGHT: 3cm">
					<table width="100%">
						<tr>
							<td class="normalSmall" width="70%">
								Deposit:
							</td>
							<td class="normal" width="30%" align="right">
								<xsl:value-of select="../FOOTER/DEPOSIT" />
							</td>
						</tr>
						<tr>
							<td class="normalSmall" width="70%">
								Service Charge:
							</td>
							<td class="normal" width="30%" align="right">
								<xsl:value-of select="../FOOTER/DT" />
							</td>
						</tr>
						<tr>
							<td class="normalSmall" width="70%">
								Amount Deferred:
							</td>
							<td class="normal" width="30%" align="right">
								<xsl:value-of select="../FOOTER/CREDIT" />
							</td>
						</tr>
						<tr>
							<td class="normalSmall" width="70%">
								Total Hire Purchase Price:
							</td>
							<td class="normal" width="30%" align="right">
								<xsl:value-of select="../FOOTER/TOTAL" />
							</td>
						</tr>
					</table>
				</div>
			</xsl:if>						
		</div>		
	</xsl:template>	
	
	<xsl:template match="HEADER">
		<!-- custid and accountno -->
		<div style="LEFT: 12cm; WIDTH: 5cm; POSITION: absolute; TOP: 5.5cm; HEIGHT: 0.5cm">
			<xsl:value-of select="CUSTID"></xsl:value-of>
		</div>
		<div style="LEFT: 12cm; WIDTH: 5cm; POSITION: absolute; TOP: 6.5cm; HEIGHT: 0.5cm">
			<xsl:value-of select="ACCTNO"></xsl:value-of>
		</div>
		
		<!-- Name and Address -->
		<div style="LEFT: 10cm; WIDTH: 8cm; POSITION: absolute; TOP: 10cm; HEIGHT: 2.5cm">
			<xsl:value-of select="NAME"></xsl:value-of><br />
			<xsl:value-of select="ADDR1"></xsl:value-of>,<br />
			<xsl:value-of select="ADDR2"></xsl:value-of>,<br />
			<xsl:value-of select="ADDR3"></xsl:value-of>,<br />
			<xsl:value-of select="POSTCODE"></xsl:value-of>
		</div>
	</xsl:template>
	
	<xsl:template match="LINEITEMS">
		<xsl:apply-templates select="LINEITEM" />
	</xsl:template>
	
	<!--<xsl:template match="LINEITEM">
		<xsl:variable name="addTo" select="ADDTO" />
		<xsl:if test="$addTo = 'True'">
			<TABLE class="normalSmall" width="100%">
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
			<table class="normal" width="100%">
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
						<xsl:value-of select="VALUE" />
					</td>
				</tr>	
			</table>
		</xsl:if>
	</xsl:template>-->

    <!--IP 12/01/12 - #8649 - LW74016-->
    <xsl:template match="LINEITEM">
        <xsl:variable name="addTo" select="ADDTO" />
        
        <xsl:if test="$addTo = 'True'">
            <xsl:variable name="type" select="TYPE" />

            <xsl:if test="$type != 'Component'">
            <TABLE class="normalSmall" width="100%">
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

            <xsl:if test="$type = 'Component'">
                <TABLE class="kitComponentSS" width="100%">
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
            
        </xsl:if>
        
        <xsl:if test="$addTo != 'True'">
            <xsl:variable name="type" select="TYPE" />

            <xsl:if test="$type != 'Component'">
            <table class="normal" width="100%">
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
                        <xsl:value-of select="VALUE" />
                    </td>
                </tr>
            </table>
            </xsl:if>

            <xsl:if test="$type = 'Component'">
                <table class="kitComponentSS" width="100%">
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
                            <xsl:value-of select="VALUE" />
                        </td>
                    </tr>
                </table>
            </xsl:if>
            
        </xsl:if>
    </xsl:template>
	
</xsl:stylesheet>
