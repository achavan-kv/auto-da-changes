<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<HEAD>
				<TITLE></TITLE>
				<meta name="vs_showGrid" content="True" />
				<META content="Microsoft Visual Studio 7.0" name="GENERATOR" />
				<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
				<style type="text/css" media="all"> @import url(styles.css); 
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
			<DIV style="Z-INDEX: 139; LEFT: 0.1cm; WIDTH: 3.832cm; POSITION: absolute; TOP: 0.503cm; HEIGHT: 0.11cm"><IMG style="WIDTH: 338px; HEIGHT: 151px" height="151" alt="" src="radioshack.png" width="338" /></DIV>
			<DIV class="RFHead2" style=" Z-INDEX: 140; LEFT: 11.1cm;PADDING-TOP: 10px; POSITION: absolute; TOP: 0.1cm; HEIGHT: 2cm" align="center">
				<img src="SSLogoNoCourts.jpg" width="200px"/>
			</DIV>
			<DIV class="RFHead1" style="BORDER-RIGHT: gray 1px solid; PADDING-RIGHT: 5px; BORDER-TOP: gray 1px solid; PADDING-LEFT: 5px; Z-INDEX: 138; LEFT: 11cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 6.886cm; PADDING-TOP: 5px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 2.3cm; HEIGHT: 1.25cm">
				<TABLE class="RFHead1" id="Table20" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD width="50%">Service<br/>
							Contract No:</TD>
						<TD width="50%">
							<xsl:value-of select="CONTRACTNO" />
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 137; LEFT: 0.61cm; WIDTH: 18cm; POSITION: absolute; TOP: 4.518cm; HEIGHT: 1.3cm" align="center"><h1 style="font-size:48px;">SERVICE CONTRACT</h1><!--<IMG style="WIDTH: 656px; HEIGHT: 51px" height="51" alt="" src="ServiceContract.jpg" width="656" />--></DIV>
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 136; LEFT: 0cm; BORDER-LEFT: gray 1px solid; WIDTH: 19cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 6.3cm; HEIGHT: 7.4cm"></DIV>
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 135; LEFT: 0cm; BORDER-LEFT: gray 1px solid; WIDTH: 19cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 14.4cm; HEIGHT: 11.88cm"></DIV>
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 130; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 6.5cm; HEIGHT: 0.003cm">
				<TABLE class="RFHead2" id="Table1" height="40" cellSpacing="1" cellPadding="6" width="100%">
					<TR>
						<TD align="middle" width="50%" bgColor="silver">SALES No.</TD>
						<TD width="50%">
							<xsl:value-of select="SOLDBY" />
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 131; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 7.9cm; HEIGHT: 1.15cm">
				<TABLE class="RFHead2" id="Table2" height="40" cellSpacing="1" cellPadding="6" width="100%">
					<TR>
						<TD align="middle" width="50%" bgColor="silver">BRANCH</TD>
						<TD width="50%">
							<xsl:value-of select="BRANCHNAME" />
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 132; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 9.35cm; HEIGHT: 1.15cm">
				<TABLE class="RFHead2" id="Table3" height="40" cellSpacing="1" cellPadding="6" width="100%">
					<TR>
						<TD align="middle" width="50%" bgColor="silver">STORE No.</TD>
						<TD width="50%">
							<xsl:value-of select="STORENO" />
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 133; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 10.8cm; HEIGHT: 1.15cm">
				<TABLE class="RFHead2" id="Table4" height="40" cellSpacing="1" cellPadding="6" width="100%">
					<TR>
						<TD align="middle" width="50%" bgColor="silver">DATE</TD>
						<TD width="50%">
							<xsl:value-of select="TODAY" />
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 134; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.2cm; HEIGHT: 1.15cm">
				<TABLE class="RFHead2" id="Table5" height="40" cellSpacing="1" cellPadding="6" width="100%">
					<TR>
						<TD align="middle" width="50%" bgColor="silver">SOLD BY</TD>
						<TD width="50%">
							<xsl:value-of select="SOLDBYNAME" />
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 118; LEFT: 2.55cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center"><b><xsl:value-of select="ACCTNO1" /></b></DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 119; LEFT: 3.3cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center"><b><xsl:value-of select="ACCTNO2" /></b></DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 120; LEFT: 4.05cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center"><b><xsl:value-of select="ACCTNO3" /></b></DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 121; LEFT: 4.8cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center"><b><xsl:value-of select="ACCTNO4" /></b></DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 122; LEFT: 5.55cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center"><b><xsl:value-of select="ACCTNO5" /></b></DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 123; LEFT: 6.3cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center"><b><xsl:value-of select="ACCTNO6" /></b></DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 124; LEFT: 7.05cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center"><b><xsl:value-of select="ACCTNO7" /></b></DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 125; LEFT: 7.8cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center"><b><xsl:value-of select="ACCTNO8" /></b></DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 126; LEFT: 8.55cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center"><b><xsl:value-of select="ACCTNO9" /></b></DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 127; LEFT: 9.3cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center"><b><xsl:value-of select="ACCTNO10" /></b></DIV>
			<DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 128; LEFT: 10.05cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center"><b><xsl:value-of select="ACCTNO11" /></b></DIV>
			<DIV class="WarrantyHeader" style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 129; LEFT: 10.8cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center"><b><xsl:value-of select="ACCTNO12" /></b></DIV>
			<DIV style="Z-INDEX: 115; LEFT: 0.3cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 6.4cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table6" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="20%">Mr.<br/>
							Mrs.<br/>
							Ms.</TD>
						<TD vAlign="top" width="40%">
							FIRST NAME<br />
							<br />
							<b><xsl:value-of select="FIRSTNAME" /></b>
						</TD>
						<TD vAlign="top" width="40%">
							LAST NAME<br />
							<br />
							<b><xsl:value-of select="LASTNAME" /></b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 116; LEFT: 0.3cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 8.2cm; HEIGHT: 2.1cm">
				<TABLE class="WarrantyHeader" id="Table7" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="20%">ADDRESS</TD>
						<TD vAlign="top" width="80%">
							<b><xsl:value-of select="ADDRESS1" /><br/>
							<xsl:value-of select="ADDRESS2" /><br />
							<xsl:value-of select="ADDRESS3" /><br />
							<xsl:value-of select="POSTCODE" />
							</b>							
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 117; LEFT: 0.3cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 10.5cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table8" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="20%">CONTACT<br/>
							NUMBERS</TD>
						<TD vAlign="top" width="40%">
							WORK<br/><br/>
							<b><xsl:value-of select="WORKTEL" /></b>
						</TD>
						<TD vAlign="top" width="40%">
							HOME<br/><br/>
							<b><xsl:value-of select="HOMETEL" /></b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 100; LEFT: 0.292cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 7.991cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 101; LEFT: 0.292cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 10.3cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 102; LEFT: 0.292cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 11.8cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="Z-INDEX: 114; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 14.7cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table9" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="40%">
							DATE OF PRODUCT PURCHASE<br/><br/>
							<b><xsl:value-of select="DATEOFPURCHASE" /></b>
						</TD>
						<TD vAlign="top" width="40%">
							PRODUCT CODE<br/><br/>
							<b><xsl:value-of select="ITEMNO" /></b>
						</TD>
						<TD vAlign="top" width="20%" align="middle">OFFICE USE ONLY</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 103; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 16.1cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table10" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="40%">
							PRODUCT DESCRIPTION<br/><br/>
							<b><xsl:value-of select="ITEMDESC1" /><br/>
							<xsl:value-of select="ITEMDESC2" />
							</b>
							
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 104; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 18cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table11" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="20%">PURCHASE PRICE</TD>
						<TD vAlign="top" width="80%">
							<b><xsl:value-of select="ITEMPRICE" /></b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 105; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 18.7cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table12" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="40%">
							SERVICE CONTRACT CODE<br/>
							<b><xsl:value-of select="WARRANTYNO" /></b>
						</TD>
						<TD vAlign="top" width="60%">
							SERVICE CONTRACT PRICE*<br/>
							<b><xsl:value-of select="WARRANTYPRICE" /></b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 106; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 19.7cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table13" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="40%">
							SERVICE CONTRACT DESCRIPTION<br/>
							<b><xsl:value-of select="WARRANTYDESC1" /><br/>
							<xsl:value-of select="WARRANTYDESC2" /></b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<xsl:if test="TERMSTYPE='WC'">
				<DIV class="smallPrint" style="Z-INDEX: 106; LEFT: 0.3cm; WIDTH: 10cm; POSITION: absolute; TOP: 20.9cm; HEIGHT: 0.317cm">
					Warranty purchased on credit. Customer has <xsl:value-of select="WARRANTYCREDIT" /> days after purchase of stock item to pay for warranty otherwise warranty will expire.
				</DIV>
			</xsl:if>
			<DIV style="Z-INDEX: 107; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 21.4cm; HEIGHT: 0.317cm">
				<TABLE class="smallPrint" id="Table14" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="40%">*includes sales tax where applicable</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="Z-INDEX: 108; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 22cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table15" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="50%">
							PLANNED DATE OF DELIVERY<br/>
							<b><xsl:value-of select="PLANNEDDELIVERY" /></b>
						</TD>
						<TD vAlign="top" width="50%">
							EXPIRY OF SERVICE CONTRACT<br/>
							<b><xsl:value-of select="EXPIRYOFWARRANTY" /></b>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 109; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 15.982cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 110; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 17.833cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 111; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 18.6cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 112; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 19.632cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 113; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 20.933cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 113; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 22.933cm; HEIGHT: 0.2cm"></DIV>
			<DIV style="LEFT: 1.295cm; WIDTH: 16.5cm; POSITION: absolute; TOP: 23.099cm; HEIGHT: 1cm">
				<TABLE class="WarrantyFooter" id="Table16" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" align="middle" width="40%">IMPORTANT - PLEASE READ THE TERMS &amp; 
							CONDITIONS BEFORE SIGNING</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 24.369cm; HEIGHT: 0.317cm">
				<TABLE class="smaller" id="Table17" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" width="40%">I hereby acknowledge having read and understood and 
							accept the terms and conditions.</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 25cm; HEIGHT: 0.317cm">
				<TABLE class="WarrantyHeader" id="Table18" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="bottom" width="40%">CUSTOMER'S SIGNATURE</TD>
						<TD vAlign="bottom" width="40%">SIGNED ON BEHALF OF RadioShack(xxx) LTD</TD>
						<TD vAlign="bottom" align="middle" width="20%">
							<b><xsl:value-of select="TODAY" /><br/><br/></b>
							DATE
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="LEFT: 14.3cm; WIDTH: 4.681cm; POSITION: absolute; TOP: 14.394cm; HEIGHT: 7.564cm" ms_positioning="FlowLayout"><IMG style="WIDTH: 177px; HEIGHT: 286px" height="286" alt="" src="grey.jpg" width="177" /></DIV>
			<br class="pageBreak" />
			<div style="POSITION: relative">
				<DIV class="smallSS" style="FONT-WEIGHT: bold; Z-INDEX: 100; WIDTH: 20cm; POSITION: absolute; HEIGHT: 1cm; TEXT-ALIGN: center" ms_positioning="FlowLayout">COURTS 
					(<xsl:value-of select="COUNTRYNAME" />) LIMITED<br/>
					<p font-size="28px">SERVICE CONTRACT TERMS AND CONDITIONS</p><br/>
					<h2 font-size="38px">PLEASE CHECK ALL SECTIONS ARE COMPLETE</h2></DIV>
				<DIV class="smallPrint" style="LIST-STYLE-POSITION: outside; Z-INDEX: 101; LEFT: 37px; WIDTH: 7.766cm; LIST-STYLE-TYPE: disc; POSITION: absolute; TOP: 3cm; HEIGHT: 5.279cm; TEXT-ALIGN: justify" ms_positioning="FlowLayout"><u>PERIOD 
						OF COVER</u><br/>
					<br/>
					This Service Contract (“contract”) extends the RadioShack (“we”, “our”) initial 
					warranty for a further 2-years on electrical products and one year on select small
					domestic appliances. This means that inclusive of the
					RadioShack initial warranty period, your product is covered for a total of 3
					years or 2years starting on the date of delivery.
					Coverage is conditional on you paying the applicable fee.<br/>
					<br/>
					<br/>
					<br/>
					<u>WHAT WE COVER</u><br/>
					<br/>
					We will repair to normal operating condition, or replace at our discretion a
					covered electrical product, after it has suffered a covered breakdown
					caused by a failure in materials, workmanship or performance during
					normal use. This contract covers the cost of Parts, Labour and call out
					charges (where applicable) on non-portable products.
					<br/>
					<br/>
					Original supplied remote controls are covered as above on a carry-in basis 
					only.
					<br/>
					Instant replacement for select small domestic
					appliances
					<br/>
					<br/>
					This contract may only be purchased within 90 days of delivery of a
					covered product and is only valid if
					<br/>
					<br/>
					<UL>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">The product is purchased new from us and has
								a RadioShack initial warranty
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Cover is not valid on products such as
								freezers, dryers, washers, dishwashers and
								vinyl lounge items intended for commercial,
								rental or profit generation purposes.
							</DIV>
						</LI>
					</UL>
					<br/>
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
								not covered under RadioShack initial warranty.
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
								adjustments. Damage resulting from power outage/ surge, 
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
							<DIV style="MARGIN-LEFT: -20px">Damage caused by any external influence such as theft, computer virus,
								acts of god, corrosion/moisture, heat/ fire, sand, animal or
								insect infestation
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">The cost of repair if you use an unauthorized repairer or if no fault
								is found with the product.
							</DIV>
						</LI>
					</UL>
				</DIV>
				<DIV class="smallPrint" style="LEFT: 10cm; WIDTH: 9cm; POSITION: absolute; TOP: 3cm; HEIGHT: 18.737cm; TEXT-ALIGN: justify" ms_positioning="FlowLayout">
					
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
							<DIV style="MARGIN-LEFT: -20px">Contact your local RadioShack Store, or ring 
								800-4FIX/800-4CCC for assistance. You will be told how to proceed with the repair.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">We will provide in-home service on major appliances 
								except portable items. You will be told when you report the claim 
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
					This contract is not transferable.
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
					FOR CLAIMS AND INQUIRIES CALL YOUR LOCAL RADIOSHACK STORE OR PHONE 800-4FIX/800-4CCC
					<br/>
					<br/>
					RadioShack offer this contract. We reserve the right to amend 
					the terms and conditions.
				</DIV>
			</div>
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'TRUE'">
				<br class="pageBreak" />
			</xsl:if>
		</div>
	</xsl:template>
</xsl:stylesheet>

  