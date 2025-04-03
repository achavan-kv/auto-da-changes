<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<BODY>
				<TABLE id="Address" height="7" width="600" border="0">
					<TR>
						<TD width="70%"></TD>
						<TD width="30%" id="txtAccountNo" align="left"><xsl:value-of select="AGREEMENT/HEADER/ACCTNO" /></TD>
					</TR>
					<TR>
						<TD width="70%"></TD>
						<TD width="30%" id="txtDate" align="left"><xsl:value-of select="AGREEMENT/HEADER/DATE" /></TD>
					</TR>
					<TR>
						<TD width="70%"></TD>
						<TD width="30%" id="txtName" align="left"><xsl:value-of select="AGREEMENT/HEADER/NAME" /></TD>
					</TR>
					<TR>
						<TD width="70%"></TD>
						<TD width="30%" align="left"></TD>
					</TR>
					<TR>
						<TD width="70%"></TD>
						<TD width="30%" id="txtAddress1" align="left"><xsl:value-of select="AGREEMENT/HEADER/ADDR1" /></TD>
					</TR>
					<TR>
						<TD width="70%"></TD>
						<TD width="30%" id="txtAddress2" align="left"><xsl:value-of select="AGREEMENT/HEADER/ADDR2" /></TD>
					</TR>
					<TR>
						<TD width="70%"></TD>
						<TD width="30%" id="txtAddress3" align="left"><xsl:value-of select="AGREEMENT/HEADER/ADDR3" /></TD>
					</TR>
					<TR>
						<TD width="70%"></TD>
						<TD width="30%" id="txtPostCode" align="left"><xsl:value-of select="AGREEMENT/HEADER/POSTCODE" /></TD>
					</TR>
					<TR>
						<TD width="70%"></TD>
						<TD width="30%" id="txtCustID" align="left">ID/IC No:
							<xsl:value-of select="AGREEMENT/HEADER/CUSTID" />
						</TD>
					</TR>
				</TABLE>
				<P></P>
				<TABLE id="Items" width="600" border="0">
					<xsl:for-each select="AGREEMENT/LINEITEMS/LINEITEM">
						<TR>
							<TD id="txtQuantity" align="left" width="75"><xsl:value-of select="QUANTITY" /></TD>
							<TD id="txtDescription" align="left" width="622"><xsl:value-of select="DESC" /></TD>
							<TD id="txtPrice" align="right"><xsl:value-of select="VALUE" /></TD>
						</TR>
					</xsl:for-each>
				</TABLE>
				<P></P>
				<P>
					<TABLE id="Table1" width="600" border="0">
						<TR>
							<TD vAlign="top" width="35%"><xsl:value-of select="AGREEMENT/FOOTER/INSTALMENTS" /></TD>
							<TD width="65%">
								<TABLE id="Table2" width="100%" border="0">
									<TR>
										<TD align="right" width="70%" height="19">PART III Goods Value:</TD>
										<TD width="30%" id="txtGoodsValue" align="right" height="19"><xsl:value-of select="AGREEMENT/FOOTER/GOODSVAL" /></TD>
									</TR>
									<TR>
										<TD align="right" width="70%">Deposit:</TD>
										<TD width="30%" id="txtDeposit" align="right"><xsl:value-of select="AGREEMENT/FOOTER/DEPOSIT" /></TD>
									</TR>
									<TR>
										<TD align="right" width="70%">Balance/Credit Extended:</TD>
										<TD width="30%" id="txtCredit" align="right"><xsl:value-of select="AGREEMENT/FOOTER/CREDIT" /></TD>
									</TR>
									<TR>
										<TD align="right" width="70%">Charge For Credit:</TD>
										<TD width="30%" id="txtDT" align="right"><xsl:value-of select="AGREEMENT/FOOTER/DT" /></TD>
									</TR>
									<TR>
										<TD align="right" width="70%">Balance Payable:</TD>
										<TD width="30%" id="txtBalance" align="right"><xsl:value-of select="AGREEMENT/FOOTER/BALANCE" /></TD>
									</TR>
									<TR>
										<TD align="right" width="70%">Total Purchase Price:</TD>
										<TD width="30%" id="txtTotal" align="right"><xsl:value-of select="AGREEMENT/FOOTER/TOTAL" /></TD>
									</TR>
								</TABLE>
							</TD>
						</TR>
					</TABLE>
				</P>
			</BODY>
		</HTML>
	</xsl:template>
</xsl:stylesheet>
