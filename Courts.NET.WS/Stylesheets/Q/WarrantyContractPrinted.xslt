<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<HEAD>
				<TITLE></TITLE>
				<meta name="vs_showGrid" content="True" />
				<META content="Microsoft Visual Studio 7.0" name="GENERATOR" />
				<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
				<style type="text/css" media="all">
					@import url(styles.css);
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
			<!--BOC BCX Change of Image-->
			<!--<DIV style="Z-INDEX: 139; LEFT: 0.1cm; WIDTH: 3.832cm; POSITION: absolute; TOP: 0.503cm; HEIGHT: 0.11cm"><IMG style="WIDTH: 426px; HEIGHT: 110px" height="110" alt="" src="SSLogo.jpg" width="426" /></DIV>-->
			<DIV style="Z-INDEX: 139; LEFT: 0.1cm; WIDTH: 3.832cm; POSITION: absolute; TOP: 0.503cm; HEIGHT: 0.11cm">
				<IMG style="WIDTH: 426px; HEIGHT: 110px" height="110" alt="" src="supershell.png" width="426" />
			</DIV>
			<!--EOC BCX Change of Image-->
			<DIV class="RFHead2" style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 140; LEFT: 14.9cm; BORDER-LEFT: gray 1px solid; WIDTH: 4cm; PADDING-TOP: 10px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 0.1cm; HEIGHT: 1cm" align="center">
				<!--<xsl:value-of select="COPY" />-->
			</DIV>
			<DIV class="RFHead1" style="BORDER-RIGHT: gray 1px solid; PADDING-RIGHT: 5px; BORDER-TOP: gray 1px solid; PADDING-LEFT: 5px; Z-INDEX: 138; LEFT: 12cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 6.886cm; PADDING-TOP: 5px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 1.3cm; HEIGHT: 1.25cm">
				<TABLE class="RFHead1" id="Table20" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD width="50%">
							Service<br/>
							Contract No:
						</TD>
						<TD width="50%">
							<xsl:value-of select="CONTRACTNO" />
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 137; LEFT: 0.61cm; WIDTH: 18cm; POSITION: absolute; TOP: 3.518cm; HEIGHT: 1.3cm" align="center">
				<IMG style="WIDTH: 656px; HEIGHT: 51px" height="51" alt="" src="ServiceContract.jpg" width="656" />
			</DIV>
			<!--BCX<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 136; LEFT: 0cm; BORDER-LEFT: gray 1px solid; WIDTH: 19cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 6.3cm; HEIGHT: 7.4cm">1</DIV>-->
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 136; LEFT: 0cm; BORDER-LEFT: gray 1px solid; WIDTH: 19cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 5.4cm; HEIGHT: 7.4cm"></DIV>
			<!--<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 135; LEFT: 0cm; BORDER-LEFT: gray 1px solid; WIDTH: 19cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 14.4cm; HEIGHT: 11.88cm">2</DIV>-->
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 135; LEFT: 0cm; BORDER-LEFT: gray 1px solid; WIDTH: 19cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 13.2cm; HEIGHT: 11.6cm"></DIV>
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 130; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 5.6cm; HEIGHT: 0.003cm">
				<TABLE class="RFHead2" id="Table1" height="40" cellSpacing="1" cellPadding="6" width="100%">
					<TR>
						<TD align="middle" width="50%" bgColor="silver">SALES No.</TD>
						<TD width="50%">
							<xsl:value-of select="SOLDBY" />
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 131; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 7.0cm; HEIGHT: 1.15cm">
				<TABLE class="RFHead2" id="Table2" height="40" cellSpacing="1" cellPadding="6" width="100%">
					<TR>
						<TD align="middle" width="50%" bgColor="silver">BRANCH</TD>
						<TD width="50%">
							<xsl:value-of select="BRANCHNAME" />
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 132; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 8.45cm; HEIGHT: 1.15cm">
				<TABLE class="RFHead2" id="Table3" height="40" cellSpacing="1" cellPadding="6" width="100%">
					<TR>
						<TD align="middle" width="50%" bgColor="silver">STORE No.</TD>
						<TD width="50%">
							<xsl:value-of select="STORENO" />
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 133; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 9.9cm; HEIGHT: 1.15cm">
				<TABLE class="RFHead2" id="Table4" height="40" cellSpacing="1" cellPadding="6" width="100%">
					<TR>
						<TD align="middle" width="50%" bgColor="silver">DATE</TD>
						<TD width="50%">
							<xsl:value-of select="TODAY" />
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 134; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.3cm; HEIGHT: 1.15cm">
				<TABLE class="RFHead2" id="Table5" height="40" cellSpacing="1" cellPadding="6" width="100%">
					<TR>
						<TD align="middle" width="50%" bgColor="silver">SOLD BY</TD>
						<TD width="50%">
							<xsl:value-of select="SOLDBYNAME" />
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 118; LEFT: 2.55cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.15cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
				<b>
					<xsl:value-of select="ACCTNO1" />
				</b>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 119; LEFT: 3.3cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.15cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
				<b>
					<xsl:value-of select="ACCTNO2" />
				</b>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 120; LEFT: 4.05cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.15cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
				<b>
					<xsl:value-of select="ACCTNO3" />
				</b>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 121; LEFT: 4.8cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.15cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
				<b>
					<xsl:value-of select="ACCTNO4" />
				</b>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 122; LEFT: 5.55cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.15cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
				<b>
					<xsl:value-of select="ACCTNO5" />
				</b>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 123; LEFT: 6.3cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.15cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
				<b>
					<xsl:value-of select="ACCTNO6" />
				</b>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 124; LEFT: 7.05cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.15cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
				<b>
					<xsl:value-of select="ACCTNO7" />
				</b>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 125; LEFT: 7.8cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.15cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
				<b>
					<xsl:value-of select="ACCTNO8" />
				</b>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 126; LEFT: 8.55cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.15cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
				<b>
					<xsl:value-of select="ACCTNO9" />
				</b>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 127; LEFT: 9.3cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.15cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
				<b>
					<xsl:value-of select="ACCTNO10" />
				</b>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 128; LEFT: 10.05cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.15cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
				<b>
					<xsl:value-of select="ACCTNO11" />
				</b>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 129; LEFT: 10.8cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 11.15cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
				<b>
					<xsl:value-of select="ACCTNO12" />
				</b>
			</DIV>
			<!--BCX<DIV style="Z-INDEX: 115; LEFT: 0.3cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 6.4cm; HEIGHT: 0.317cm">-->
			<DIV style="Z-INDEX: 115; LEFT: 0.3cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 5.5cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table6" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="20%">
							Mr.<br/>
							Mrs.<br/>
							Ms.
						</TD>
						<TD vAlign="top" width="40%">
							FIRST NAME<br />
							<br />
							<b>
								<xsl:value-of select="FIRSTNAME" />
							</b>
						</TD>
						<TD vAlign="top" width="40%">
							LAST NAME<br />
							<br />
							<b>
								<xsl:value-of select="LASTNAME" />
							</b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 116; LEFT: 0.3cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 7.0cm; HEIGHT: 2.1cm">
				<TABLE class="WarrantyHeader" id="Table7" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="20%">ADDRESS</TD>
						<TD vAlign="top" width="80%">
							<b>
								<xsl:value-of select="ADDRESS1" />
								<br/>
								<xsl:value-of select="ADDRESS2" />
								<br />
								<xsl:value-of select="ADDRESS3" />
								<br />
								<xsl:value-of select="POSTCODE" />
							</b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 117; LEFT: 0.3cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 9.6cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table8" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="20%">
							CONTACT<br/>
							NUMBERS
						</TD>
						<TD vAlign="top" width="40%">
							WORK<br/><br/>
							<b>
								<xsl:value-of select="WORKTEL" />
							</b>
						</TD>
						<TD vAlign="top" width="40%">
							HOME<br/><br/>
							<b>
								<xsl:value-of select="HOMETEL" />
							</b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 100; LEFT: 0.292cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 7.0cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 101; LEFT: 0.292cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 9.4cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 102; LEFT: 0.292cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 10.9cm; HEIGHT: 0.2cm"></DIV>
			<!--BCX<DIV style="Z-INDEX: 114; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 14.7cm; HEIGHT: 0.317cm">-->
			<DIV style="Z-INDEX: 114; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 13.2cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table9" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="40%">
							DATE OF PRODUCT PURCHASE<br/><br/>
							<b>
								<xsl:value-of select="DATEOFPURCHASE" />
							</b>
						</TD>
						<TD vAlign="top" width="40%">
							PRODUCT CODE<br/><br/>
							<b>
								<xsl:value-of select="ITEMNO" />
							</b>
						</TD>
						<TD vAlign="top" width="20%" align="middle">OFFICE USE ONLY</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 103; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 15.1cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table10" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="40%">
							PRODUCT DESCRIPTION<br/><br/>
							<b>
								<xsl:value-of select="ITEMDESC1" />
								<br/>
								<xsl:value-of select="ITEMDESC2" />
							</b>

						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 104; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 17cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table11" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="20%">PURCHASE PRICE</TD>
						<TD vAlign="top" width="80%">
							<b>
								<xsl:value-of select="ITEMPRICE" />
							</b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 105; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 17.7cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table12" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="40%">
							SERVICE CONTRACT CODE<br/>
							<b>
								<xsl:value-of select="WARRANTYNO" />
							</b>
						</TD>
						<TD vAlign="top" width="60%">
							SERVICE CONTRACT PRICE*<br/>
							<b>
								<xsl:value-of select="WARRANTYPRICE" />
							</b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 106; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 18.7cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table13" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="40%">
							SERVICE CONTRACT DESCRIPTION<br/>
							<b>
								<xsl:value-of select="WARRANTYDESC1" />
								<br/>
								<xsl:value-of select="WARRANTYDESC2" />
							</b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<xsl:if test="TERMSTYPE='WC'">
				<DIV class="smallPrint" style="Z-INDEX: 106; LEFT: 0.3cm; WIDTH: 10cm; POSITION: absolute; TOP: 20.9cm; HEIGHT: 0.317cm">
					Warranty purchased on credit. Customer has <xsl:value-of select="WARRANTYCREDIT" /> days after purchase of stock item to pay for warranty otherwise warranty will expire.
				</DIV>
			</xsl:if>
			<DIV style="Z-INDEX: 107; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 20.4cm; HEIGHT: 0.317cm">
				<TABLE class="smallPrint" id="Table14" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="40%">*includes sales tax where applicable</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 108; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 21cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table15" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="50%">
							PLANNED DATE OF DELIVERY<br/>
							<b>
								<xsl:value-of select="PLANNEDDELIVERY" />
							</b>
						</TD>
						<TD vAlign="top" width="50%">
							EXPIRY OF SERVICE CONTRACT<br/>
							<b>
								<xsl:value-of select="EXPIRYOFWARRANTY" />
							</b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 109; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 14.982cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 110; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 16.833cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 111; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 17.6cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 112; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 18.632cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 113; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 20.933cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 113; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 21.933cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="LEFT: 1.295cm; WIDTH: 16.5cm; POSITION: absolute; TOP: 22.0cm; HEIGHT: 1cm">
				<TABLE class="WarrantyFooter" id="Table16" cellSpacing="1" cellPadding="1" width="100%" border="0" style="font-size: 12pt;">
					<TR>
						<TD vAlign="top" align="middle" width="40%">
							IMPORTANT - PLEASE READ THE TERMS &amp;
							CONDITIONS BEFORE SIGNING
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<!--BCX<DIV style="LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 24.369cm; HEIGHT: 0.317cm">-->
			<DIV style="LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 22.8cm; HEIGHT: 0.317cm">
				<TABLE class="smaller" id="Table17" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="40%">
							I hereby acknowledge having read and understood and
							accept the terms and conditions.
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 23.7cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table18" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="bottom" width="40%">CUSTOMER'S SIGNATURE</TD>
						<TD vAlign="bottom" width="40%">SIGNED ON BEHALF OF UNICOMER(<xsl:value-of select="COUNTRYNAME" />) B.V.
						</TD>
						<TD vAlign="bottom" align="middle" width="20%">
							<b>
								<xsl:value-of select="TODAY" />
								<br/>
								<br/>
							</b>
							DATE
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<!--BCX<DIV style="LEFT: 1.295cm; WIDTH: 16.5cm; POSITION: absolute; TOP: 26.67cm; HEIGHT: 1cm">-->
			<DIV style="LEFT: 1.295cm; WIDTH: 16.5cm; POSITION: absolute; TOP: 24.9cm; HEIGHT: 1cm">
				<TABLE class="WarrantyFooter" id="Table19" cellSpacing="1" cellPadding="1" width="100%" border="0" style="font-size: 10pt;">
					<TR>
						<TD vAlign="top" align="middle">PLEASE CHECK ALL SECTIONS ARE COMPLETE</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="LEFT: 14.3cm; WIDTH: 4.681cm; POSITION: absolute; TOP: 13.2cm; HEIGHT: 7.6cm" ms_positioning="FlowLayout">
				<IMG style="WIDTH: 177px; HEIGHT: 292px" height="292" alt="" src="grey.jpg" width="177" />
			</DIV>
			<br class="pageBreak" />
			<!--BOC BCX Change in Terms & Conditions-->
			<!--<div style="POSITION: relative">
				<DIV class="smallSS" style="FONT-WEIGHT: bold; Z-INDEX: 100; WIDTH: 20cm; POSITION: absolute; HEIGHT: 1cm; TEXT-ALIGN: center" ms_positioning="FlowLayout">COURTS 
					(<xsl:value-of select="COUNTRYNAME" />) LIMITED<br/>
					SERVICE CONTRACT TERMS AND CONDITIONS</DIV>
				<DIV class="smallPrint" style="LIST-STYLE-POSITION: outside; Z-INDEX: 101; LEFT: 37px; WIDTH: 7.766cm; LIST-STYLE-TYPE: disc; POSITION: absolute; TOP: 3cm; HEIGHT: 5.279cm; TEXT-ALIGN: justify" ms_positioning="FlowLayout"><u>PERIOD 
						OF COVER</u><br/>
					<br/>
					This Service Contract (“contract”) extends the Courts (“we”, “our”) initial 
					warranty for a further 2-years on furniture and 2 or 4 years on electrical 
					products. This means that inclusive of the Courts initial warranty period, your 
					product is covered for a total of 3 or 5 years starting on the date of 
					delivery. Coverage is conditional on you paying the applicable fee.<br/>
					<br/>
					<br/>
					<br/>
					<u>WHAT WE COVER</u><br/>
					<br/>
					We will repair to normal operating condition, or replace at our discretion a 
					covered electrical product, after it has suffered a covered breakdown caused by 
					a failure in materials, workmanship or performance during normal use. This 
					contract covers the cost of Parts, Labour and call out charges (where 
					applicable) on non-portable products.
					<br/>
					<br/>
					Original supplied remote controls are covered as above on a carry-in basis 
					only.
					<br/>
					<br/>
					On refrigeration products which suffer a covered refrigeration failure, this 
					contract covers the cost of replacing food spoilt as a result, up to a maximum 
					of $xxx.xx per incidence.
					<br/>
					<br/>
					We will repair to normal operating condition, or replace at our discretion, 
					covered furniture after it has suffered structural damage caused by a defect in 
					materials, workmanship or performance during normal use, as follows:<br/>
					<br/>
					(1) Frame failure caused by warpage and breakage.<br/>
					(2) Bending and breakage of metal components, hinges and casters.<br/>
					(3) Failure of mechanical and electrical recliners.<br/>
					(4) Failure of drawer mechanisms.<br/>
					(5) Separation of seams or upholstery beading.<br/>
					(6) Lifting or peeling of veneer on wooden furniture.<br/>
					(7) Lifting or peeling of hide on leather furniture.<br/>
					<br/>
					<br/>
					<br/>
					<u>CONDITIONS</u><br/>
					<br/>
					This contract may only be purchased within 90 days of delivery of a covered 
					product and is only valid if:<br/>
					<br/>
					<UL>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">The product is purchased new from us and has a 
								Courts initial warranty
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">The product is for domestic and personal use only. 
								Cover is not valid on products intended for commercial, rental or profit 
								generation purposes. Office products such as facsimile machines and computers 
								are covered for home office use.
							</DIV>
						</LI>
					</UL>
					<br/>
					<u>WHAT WE EXCLUDE</u>
					<br/>
					<br/>
					<UL>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Damage caused before or during delivery or cover 
								provided by any other insurance or manufacturer or repairers repair guarantee 
								or warranty.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Manufacturer’s recall or modifications or damage 
								not covered under Courts initial warranty.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Damage caused by abuse, misuse or neglect including 
								removal or alteration of serial numbers.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Accessories used in or with the product unless 
								covered under a separate contract.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Replacement of any consumable or component intended 
								by the manufacturer to be self-replaced, such as electrical plugs, vacuum belts 
								and bags, light bulbs, batteries, external fuses, filters, toner, print 
								cartridges, or software incorporated into a covered product.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Routine maintenance, cleaning and external 
								adjustments.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Damage resulting from power outage/ surge, 
								inadequate voltage or current, reception or transmission problems.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">The cost of repairing, restoring or reconfiguring 
								computer software.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Failure to follow the manufacturers or suppliers 
								instructions for care of the product.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Accidental, intentional or cosmetic damage 
								including chipping, denting, scratching or puncturing, or damage caused by the 
								incorrect use or application of any cleaning substance or material.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">General wear and tear to furniture consistent with 
								age and usage, including soiling, perspiration, hair/ body oils and odours.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Repairs to scars, bites, or any other natural 
								characteristic of leather furniture.
							</DIV>
						</LI>
					</UL>
				</DIV>
				<DIV class="smallPrint" style="LEFT: 10cm; WIDTH: 9cm; POSITION: absolute; TOP: 3cm; HEIGHT: 18.737cm; TEXT-ALIGN: justify" ms_positioning="FlowLayout">
					<ul>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Loss of resilience or shape of furniture interior 
								fillings, fading or colour loss.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Damage caused by any external influence such as 
								theft, computer virus, acts of god, corrosion/moisture, heat/ fire, sand, 
								animal or insect infestation.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">The cost of repair if you use an unauthorised 
								repairer or if no fault is found with the product.
							</DIV>
						</LI>
					</ul>
					<u>
						<br/>
						IN THE EVENT OF A BREAKDOWN</u>
					<ul>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Review the manufacturer’s instructions or care 
								guidelines to determine if the problem can be rectified by you.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Check that the contract is still in force. You must 
								report the breakdown within 5 days of the breakdown for the claim to be valid.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Contact your local Courts Store, or ring 
								800#xxxxxxx for assistance. You will be told how to proceed with the repair.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">We will provide in-home service on major appliances 
								and furniture except portable items. You will be told when you report the claim 
								which products qualify for in-home service. All other repairs will be on a 
								carry-in basis only.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">We will need to see this document to proceed with 
								the repair.
							</DIV>
						</LI>
					</ul>
					<br/>
					<br/>
					<u>LIABILITY</u>
					<br/>
					<br/>
					We will repair or replace your product with an identical or similar model at 
					our discretion, up to a maximum value not exceeding the original purchase 
					price.
					<br/>
					<br/>
					If we replace it, the original product becomes our property and the contract 
					terminates with no refund of fee. Any remaining instalments will still be due 
					to us.
					<br/>
					<br/>
					We are not responsible for any consequential or incidental damages arising from 
					the use or loss of use of the product. This does not affect your statutory 
					rights.<br/>
					<br/>
					In the event of covered food loss, the repairer will verify and inspect the 
					spoilt food within two working days and document the loss. The claims limit is 
					$xxx.xx per incidence.
					<br/>
					<br/>
					This contract is only valid in the country of issue and is not transferable.
					<br/>
					<br/>
					THIS CONTRACT IS NOT AN INSURANCE POLICY OR GUARANTEE. COVER IS LIMITED TO 
					PRODUCTS WHICH FAIL DUE SOLELY TO MATERIALS, WORKMANSHIP OR PERFORMANCE.<br/>
					<br/>
					<br/>
					<u>
						<br/>
						CANCELLATION</u>
					<br/>
					<br/>
					You may cancel this contract for any reason within 15 days of the date of 
					purchase shown overleaf, by giving us a notice of cancellation and this 
					document. You will be eligible for a 100% refund of the fee you paid.
					<br/>
					<br/>
					We may cancel the contract at any time by giving you 14 days notice in writing 
					to your last known address. You will be eligible for a pro-rata refund of the 
					fee you paid.
					<br/>
					<br/>
					In the event of theft, fraud, sale or return of the product to us, we will 
					cancel this contract with no refund. If any instalment payments are in default 
					and unpaid for more than one month we may cancel the contract at our 
					discretion.
					<br/>
					<br/>
					FOR CLAIMS AND INQUIRIES CALL YOUR LOCAL COURTS STORE OR PHONE x800xxx xxxx
					<br/>
					<br/>
					Courts (xxxxxxxx) Limited offer this contract. We reserve the right to amend 
					the terms and conditions.
				</DIV>
			</div>-->
			<div style="POSITION: relative">
				<DIV class="smallSS" style="FONT-WEIGHT: bold; Z-INDEX: 100; WIDTH: 20cm; POSITION: absolute; HEIGHT: 2cm; TEXT-ALIGN: center" ms_positioning="FlowLayout">
					UNICOMER(<xsl:value-of select="COUNTRYNAME" />) B.V.
					<br/>
					<br/>
					Algemene Voorwaarden
				</DIV>
				<DIV class="smallPrint" style="LIST-STYLE-POSITION: outside; Z-INDEX: 101; LEFT: 37px; WIDTH: 16.766cm; LIST-STYLE-TYPE: disc; POSITION: absolute; TOP: 2cm; HEIGHT: 7.0cm; TEXT-ALIGN: justify; font-size: 10pt; line-height: 9.5pt;" ms_positioning="FlowLayout">
					<br/>
					In dit Extended Warranty Plan verwijzen de woorden “wij”, “Ons” en “onze” naar OMNI/Unicomer
					Curacao B.V. kantoorhoudende aan de Kaya Jacob Posner # 19 te Willemstad. De woorden “u” en “uw”
					verwijzen naar de koper van dit Plan. Reparatie en garantie: OMNI/Unicomer Curacao B.V. za l trachten
					alle produkten met een Extended Warranty zo accuraat en efficient mogelijk te repareren. Voor melding
					en een spoedig verloop van de service en reparatie, dient u contactop te nemen met onze Service
					Afdeling 4616600 ext. 201,202,203 en uw EW# melden. Reparaties zullen worden uitgevoerd door het
					OMNI/Unicomer Curacao B.V. service center of door een OMNI/Unicomer Curacao B.V. geautoriseerd
					persoon. Dit garantieplan dekt of omvat de volgende criteria:
					<br/>
					<br/>					
					<ul>
						<LI>
							<DIV style="MARGIN-LEFT: -5px">
								De garantie van dit plan begint op de factuurdatum van de aankoop van het produkt en geldt
								voor de duur van de garantie die u heeft gekocht en die ook vermeld staat op uw ‘Garantie
								Certificaat’<br/>
							</DIV>
						</LI>
						<br/>
						<LI>
							<DIV style="MARGIN-LEFT: -5px">
								Dit plan omvat tevens de fabrieksgarantie
							</DIV>
						</LI>
						<br/>
						<LI>
							<DIV style="MARGIN-LEFT: -5px">
								Dit plan dekt fabrieksgebreken in onderdelen en arbeid die het gevolg zijn van normaal gebruik
								van het product.
							</DIV>
						</LI>
						<br/>
						<LI>
							<DIV style="MARGIN-LEFT: -5px">
								Dit plan dekt tevens fabrieksgebreken in onderdelen en arbeid die het gevolg zijn van slijtage bij
								normaal gebruik.
							</DIV>
						</LI>
						<br/>
						<LI>
							<DIV style="MARGIN-LEFT: -5px">
								Produkten, inclusief produkten die nog vallen onder de originele fabrieksgarantie periode,
								worden gerepareerd of vervangen met origínele of vergelijkbare produkten.
							</DIV>
						</LI>
						<br/>
						<LI>
							<DIV style="MARGIN-LEFT: -5px">
								De te vervangen onderdelen kunnen nieuw, gereviseerd of vergelijkbare niet-originele
								onderdelen zijn die voldoen aan de specificaties van de fabriek of van het produkt.
							</DIV>
						</LI>
					</ul>
					<br/>
					Aankoop registratie: Ondanks het feit dat OMNI/Unicomer Curacao B.V. een registratie bijhoudt van de
					aankoop en uw Extended Warranty Plan in haar computersysteem, dient u dit originele certificaat en de
					originele aankoopbon altijd ter inzage te overhandigen om in aanmerking te komen voor reparaties of
					een produktvervanging welke betrekking hebben op het originele produkt waarop dit Plan van
					toepassing is. De rechten en vorderingen op grond van deze garantie komen u als koper toe
					onverminderd de rechten en vorderingen die de wet u als koper toekent<br/>
					<br/>
					<br/>
					No Lemon Policy: Indien OMNI/Unicomer Curacao B.V. heeft bepaald en vastgelegd dat het origínele
					produkt waarop dit Plan van toepassing is, reeds vier (4) afgeronde reparaties heeft ondergaan door het
					Service Center en een vijfde (5) reparatie noodzakelijk is , zullen wij het produkt vervangen met
					eenzelfde of een produkt van vergelijkbare uitvoering. Preventieve controles, schoonmaak, produkt
					diagnose, klanten informatie, reparatie/vervanging van accessories, problemen gerelateerd aan
					computer software en reparaties gedaan buiten Curaçao, worden niet beschouwd als reparaties zoals
					omschreven in de No Lemon Policy. Het originele produkt met de originele aankoopbon dienen bij
					omruiling geretourneerd te worden aan OMNI/Unicomer Curacao B.V. samen met de service/reparatie
					werkbonnen van de vier (4) afzonderlijke en reeds afgeronde reparaties. Bewaar uw service/reparatie
					werkbonnen! Kopieën van service en reparatiebonnen kunnen door ons niet worden afgegeven. Door
					technologische vernieuwingen en markt ontwikkelingen is het mogelijk dat uw vervangingsprodukt een
					lagere prijs heeft dan het origineel gekochte produkt. Vervangingsprodukten mogen nieuwe of
					gereviseerde produkten zijndie naar ons oordeel aan de fabriekspecificaties van het originele produkt
					kunnen voldoen. Als het originele produkt is vervangen, zal de resterende dekkingstermijn van dit Plan
					blijven gelden voor het oude originele produkt. Voor het vervangingsprodukt dient u een nieuw
					garantieplan aan te schaffen.
					<br/>
					<br/>
					<br/>
					Algemene uitsluiting: Dit onderdelen - &amp; arbeidsgarantie plan dekt geen reparaties veroorzaakt door
					toevallige of opzettelijke fysieke schade. Dit Plan dekt geen reparaties veroorzaakt door gemorste of
					schadelijke vloeistoffen evenals schade veroorzaakt door insecten, ratten of andere dieren. Dit Plan
					dekt geen reparaties veroorzaakt door misbruik of schade ontstaan of veroorzaakt door derden niet
					geautoriseerd door OMNI/Unicomer Curacao B.V., even als reparaties aan produkten welke zijn
					geopend of ontmanteld door derden die niet geautoriseerd zijn door OMNI/Unicomer Curacao B.V. Dit
					Plan dekt geen vervangingskosten veroorzaakt door verlies of verteerbare onderdelen zoals knopjes,
					afstand bedieningen, batterijen, zakken, riemen etc. Dit Plan geeft geen dekking tegen roest, tenzij u
					kunt bewijzen dat dit veroorzaakt is door zweten. Indien er sprake is van onvoldoende of achterstallig
					onderhoud dekt dit Plan niet de gevolgschade. Het Plan dekt geen schade veroorzaakt door “Acts of
					God” zoals o.a. bliksem, overstroming, aardbeving etc. of de gevolgen van fluctuaties in de stroom
					aanvoer. De plannen zijn niet verkrijgbaar of geldig voor produkten die gebruikt worden voor
					commerciële (multi-gebruik etc. ), verhuur of gemeenschappelijk gebruik . Het gebruik van een produkt
					voor dergelijke doeleinden maakt dit Plan ongeldig . Dit Plan dekt geen glasschade of vervanging
					daarvan. Dit Plan dekt geen schade ontstaan door ruw of verkeerd gebruik, schade ontstaan door
					incorrect aansluiten van het product of schade door het niet opvolgen van de door de fabricant gestelde
					aanwijzingen. Dit garantieplan geldt alleen voor de overeengekomen looptijd.<br/>
				</DIV>
			</div>
			<!--<br class="pageBreak" />-->

			<!--<div style="POSITION: relative">
				<br/>
				<br/>
				<DIV class="smallPrint" style="LIST-STYLE-POSITION: outside; Z-INDEX: 101; LEFT: 37px; WIDTH: 16.766cm; LIST-STYLE-TYPE: disc; POSITION: absolute; TOP: 3cm; HEIGHT: 5.279cm; TEXT-ALIGN: justify" ms_positioning="FlowLayout">

					technologische vernieuwingen en markt ontwikkelingen is het mogelijk dat uw vervangingsprodukt een
					lagere prijs heeft dan het origineel gekochte produkt. Vervangingsprodukten mogen nieuwe of
					gereviseerde produkten zijndie naar ons oordeel aan de fabriekspecificaties van het originele produkt
					kunnen voldoen. Als het originele produkt is vervangen, zal de resterende dekkingstermijn van dit Plan
					blijven gelden voor het oude originele produkt. Voor het vervangingsprodukt dient u een nieuw
					garantieplan aan te schaffen.<br/>
					<br/>
					<br/>
					<br/>

					<br/>
					<br/>
					<br/>
					Algemene uitsluiting: Dit onderdelen - &amp; arbeidsgarantie plan dekt geen reparaties veroorzaakt door
					toevallige of opzettelijke fysieke schade. Dit Plan dekt geen reparaties veroorzaakt door gemorste of
					schadelijke vloeistoffen evenals schade veroorzaakt door insecten, ratten of andere dieren. Dit Plan
					dekt geen reparaties veroorzaakt door misbruik of schade ontstaan of veroorzaakt door derden niet
					geautoriseerd door OMNI/Unicomer Curacao N.V., even als reparaties aan produkten welke zijn
					geopend of ontmanteld door derden die niet geautoriseerd zijn door OMNI/Unicomer Curacao N.V. Dit
					Plan dekt geen vervangingskosten veroorzaakt door verlies of verteerbare onderdelen zoals knopjes,
					afstand bedieningen, batterijen, zakken, riemen etc. Dit Plan geeft geen dekking tegen roest, tenzij u
					kunt bewijzen dat dit veroorzaakt is door zweten. Indien er sprake is van onvoldoende of achterstallig
					onderhoud dekt dit Plan niet de gevolgschade. Het Plan dekt geen schade veroorzaakt door “Acts of
					God” zoals o.a. bliksem, overstroming, aardbeving etc. of de gevolgen van fluctuaties in de stroom
					aanvoer. De plannen zijn niet verkrijgbaar of geldig voor produkten die gebruikt worden voor
					commerciële (multi-gebruik etc. ), verhuur of gemeenschappelijk gebruik . Het gebruik van een produkt
					voor dergelijke doeleinden maakt dit Plan ongeldig . Dit Plan dekt geen glasschade of vervanging
					daarvan. Dit Plan dekt geen schade ontstaan door ruw of verkeerd gebruik, schade ontstaan door
					incorrect aansluiten van het product of schade door het niet opvolgen van de door de fabricant gestelde
					aanwijzingen. Dit garantieplan geldt alleen voor de overeengekomen looptijd.<br/>
					<br/>
					<br/>
				</DIV>
			</div>-->
			<!--EOC BCX Change in Terms & Conditions-->
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'TRUE'">
				<br class="pageBreak" />
			</xsl:if>
		</div>
	</xsl:template>
</xsl:stylesheet>

