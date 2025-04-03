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
				<xsl:apply-templates select="AGREEMENTS" />
			</BODY>
		</HTML>		
	</xsl:template>
	
	<xsl:template match="AGREEMENTS">	
		<xsl:apply-templates select="AGREEMENT" />		
		<P></P>
	</xsl:template>
	
	<xsl:template match="AGREEMENT">	
		<div style="position:relative">	
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
		<TABLE class="normal" id="Address" height="7" width="600" border="0">
			<TR>
				<TD width="70%"></TD>
				<xsl:apply-templates select="ACCTNO" />
			</TR>
			<TR>
				<TD width="70%"></TD>
				<xsl:apply-templates select="DATE" />
			</TR>
			<TR>
				<TD width="70%"></TD>
				<xsl:apply-templates select="NAME" />
			</TR>
			<TR>
				<TD width="70%"></TD>
				<TD width="30%"></TD>
			</TR>
			<TR>
				<TD width="70%"></TD>
				<xsl:apply-templates select="ADDR1" />
			</TR>
			<TR>
				<TD width="70%"></TD>
				<xsl:apply-templates select="ADDR2" />
			</TR>
			<TR>
				<TD width="70%"></TD>
				<xsl:apply-templates select="ADDR3" />
			</TR>
			<TR>
				<TD width="70%"></TD>
				<xsl:apply-templates select="POSTCODE" />
			</TR>
			<TR>
				<TD width="70%"></TD>
				<TD width="30%" align="left">
					ID/IC No: 
					<xsl:apply-templates select="CUSTID" />
				</TD>				
			</TR>
		</TABLE>
		<P></P>
	</xsl:template>
	
	<xsl:template match="FOOTER">
		<TABLE class="normal" width="600" border="0">
			<TR>
				<xsl:apply-templates select="INSTALMENTS" />	
				<TD width="65%">
					<TABLE width="100%" border="0">
						<TR>
							<TD width="70%" align="right" height="19">
								ส่วนที่ 3 มูลค่าสินค้า:
							</TD>
							<xsl:apply-templates select="GOODSVAL" />
						</TR>
						<TR>
							<TD width="70%" align="right">
								เงินมัดจำ:
							</TD>
							<xsl:apply-templates select="DEPOSIT" />
						</TR>
						<TR>
							<TD width="70%" align="right">
								ยอดสินค้าค้างชำระ:
							</TD>
							<xsl:apply-templates select="CREDIT" />
						</TR>
						<TR>
							<TD width="70%" align="right">
								ค่าใช้จ่ายในการผ่อนชำระ:
							</TD>
							<xsl:apply-templates select="DT" />
						</TR>
						<TR>
							<TD width="70%" align="right">
								ยอดรวมผ่อนชำระ:
							</TD>
							<xsl:apply-templates select="BALANCE" />
						</TR>
						<TR>
							<TD width="70%" align="right">
								ยอดรวมทั้งสิ้น:
							</TD>
							<xsl:apply-templates select="TOTAL" />
						</TR>
					</TABLE>
				</TD>		
			</TR>
		</TABLE>
		<xsl:apply-templates select="PB" />
	</xsl:template>
	
	<xsl:template match="LINEITEMS">	
		<xsl:apply-templates select="LINEITEM" />
	</xsl:template>
	
	<xsl:template match="LINEITEM">
		<TABLE class="normal" id="Items" width="600" border="0">
			<TR>
				<xsl:apply-templates select="QUANTITY" />			
				<xsl:apply-templates select="DESC" />
				<xsl:apply-templates select="VALUE" />
			</TR>
			<TR>
				<TD align="left" width="15%"></TD>		
				<TD align="left" width="65%">
					<xsl:value-of select="DESC2" />
				</TD>
				<TD align="right" width="25%"></TD>
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
		<TD align="right" width="25%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="ACCTNO">	
		<TD width="30%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="DATE">	
		<TD width="30%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="NAME">	
		<TD width="30%">
			<xsl:apply-templates />
		</TD>
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
	
	<xsl:template match="CUSTID">	
		<xsl:apply-templates />
	</xsl:template>	
	
	<xsl:template match="INSTALMENTS">	
		<TD valign="top" width="35%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="GOODSVAL">	
		<TD align="right" height="19" width="30%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="DEPOSIT">	
		<TD align="right" width="30%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="CREDIT">	
		<TD align="right" width="30%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="DT">	
		<TD align="right" width="30%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="BALANCE">	
		<TD align="right" width="30%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="TOTAL">	
		<TD align="right" width="30%">
			<xsl:apply-templates />
		</TD>
	</xsl:template>
	
	<xsl:template match="PB">
		<br class="pageBreak" />
		<xsl:apply-templates select="../../../HEADER" />	
		<P></P>
	</xsl:template>
	
</xsl:stylesheet>
