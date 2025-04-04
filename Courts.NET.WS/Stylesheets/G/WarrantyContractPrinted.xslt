﻿<?xml version="1.0" encoding="UTF-8" ?>
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
    <div style="position:relative;">
      <DIV style="Z-INDEX: 139; LEFT: 0.8cm; WIDTH: 3.832cm; POSITION: absolute; TOP: 0.503cm; HEIGHT: 0.11cm">
        <IMG  alt="" src="Supashield logo BW final.jpg" style="height:100px;width:100px;" />
      </DIV>
      <DIV class="RFHead2" style="Z-INDEX: 140; LEFT: 12cm; PADDING-TOP: 10px; POSITION: absolute; TOP: 0.1cm;">
        <!--<xsl:value-of select="COPY" />-->Business Registration No: C07004333
      </DIV>
			<DIV class="RFHead1" style="BORDER-RIGHT: gray 1px solid; PADDING-RIGHT: 5px; BORDER-TOP: gray 1px solid; PADDING-LEFT: 5px; Z-INDEX: 138; LEFT: 12cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 6.886cm; PADDING-TOP: 5px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 1.3cm; HEIGHT: 1.25cm">
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
			<DIV style="Z-INDEX: 137; LEFT: 0.61cm; WIDTH: 18cm; POSITION: absolute; TOP: 3.518cm; HEIGHT: 1.3cm" align="center"><IMG style="WIDTH: 656px; HEIGHT: 51px" height="51" alt="" src="ServiceContract.jpg" width="656" /></DIV>
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
						<TD vAlign="top" width="40%">*includes VAT where applicable</TD>
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
			<DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 113; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 21.933cm; HEIGHT: 0.2cm"></DIV>
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
						<TD vAlign="bottom" width="40%">SIGNED ON BEHALF OF COURTS(Mauritius) LTD</TD>
						<TD vAlign="bottom" align="middle" width="20%">
							<b><xsl:value-of select="TODAY" /><br/><br/></b>
							DATE
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="LEFT: 1.295cm; WIDTH: 16.5cm; POSITION: absolute; TOP: 26.67cm; HEIGHT: 1cm">
				<TABLE class="WarrantyFooter" id="Table19" cellSpacing="1" cellPadding="1" width="100%" border="0">
					<TR>
						<TD vAlign="top" align="middle">PLEASE CHECK ALL SECTIONS ARE COMPLETE</TD>
					</TR>
				</TABLE>
			</DIV>
			<DIV style="LEFT: 14.3cm; WIDTH: 4.681cm; POSITION: absolute; TOP: 10.394cm; HEIGHT: 5.564cm" ms_positioning="FlowLayout"><IMG style="WIDTH: 177px; HEIGHT: 286px" height="286" alt="" src="grey.jpg" width="177" /></DIV>
			<br class="pageBreak" />
			<div style="POSITION: relative">
				<DIV class="smallSS" style="FONT-WEIGHT: bold; Z-INDEX: 100; WIDTH: 20cm; POSITION: absolute; HEIGHT: 1cm; TEXT-ALIGN: center" ms_positioning="FlowLayout">COURTS 
					(<xsl:value-of select="COUNTRYNAME" />) LIMITED<br/>
					TERMS AND CONDITIONS</DIV>
				<DIV class="smallPrint" style="LIST-STYLE-POSITION: outside; Z-INDEX: 101; LEFT: 37px; WIDTH: 7.766cm; LIST-STYLE-TYPE: disc; POSITION: absolute; TOP: 1cm; HEIGHT: 3.279cm; TEXT-ALIGN: justify" ms_positioning="FlowLayout"><u>This 
						Service Contract</u><br/>
					<br/>
					•This contract applies to parts and labour in respect of the product stated on 
 					 the Courts Warranty Sales receipt (the “Product”) for mechanical, electrical and 
					 structural defects only and only to the extent provided by the manufacturer of 
					 the product and<br/>
					•Extends the warranty cover for a further period commencing on the date of expiry 
					 of the manufacturer’s initial warranty period and ending on the Date of Expiry 
					 stated on the Courts Warranty Sales receipt for that product (the “Term”).  This 
					 means that, inclusive of the manufacturer’s warranty period, the Product will 
					 have a total of either five (5) years warranty, three (3) years warranty or (2) 
				 	 years warranty from that date of sale based on your payment of the applicable fee,
					 and selections indicated on the front of this agreement and<br/>
					•During the Term, portable items should be brought into the nominated repairer.  
					 All other items will be subject to call out service and<br/>
					•Is personal to you, the purchaser and is not transferable
					<br/>
					<br/>
					<br/>
					<u>Product Eligibility</u><br/>
					<br/>
					A Courts Extended Warranty Service Contract may only be purchased within 30 days of 
					delivery of a Product and only covers a Product which:<br/>
					•is purchased/hired new from Courts(Mtius) Ltd<br/>
					•is manufactured for use in Mauritius<br/>
					•included at the time of purchase/hire the manufacturer’s complete and original 
					 warranty valid in Mauritius.<br/>
					•in case of customer products such as furniture items, electronics and major appliances, 
					 the use is or has been limited to domestic and personal use.  Office products such as 
					 facsimile machines, copiers, scanners and computers are also covered for home/office 
					 use.
					<br/>
					<br/>
					<br/>
					<u>Coverage</u><br/>
					<br/>
					Courts will repair to normal operating condition or replace at our discretion, a covered
					item after it has suffered mechanical or structural damage due to a manufacturing defect 
					in material or workmanship during normal use.  Structural damage refers to :
					<br/>
					<br/>
					1.Frame damage caused by warpage and breakage<br/>
					2.Bending and breakage of metal components<br />
					3.Failure of mechanical recliners<br />
					4.Damage on non-electrical mechanisms<br />
					5.Seperation of seams<br />
					6.Lifting or peeling of wooden veneer<br />
					7.Lifting and peeling of semi-leather or vinyl furniture veneer<br />
					<br/>
					<br/>
					This contract covers the labour, parts and call out charge necessary to repair the item, subject to the exclusions listed below.
					<br/>

					<br/>
					<u>Exclusions from the Extended Warranty Cover</u>
					<br/>
					<br/>
					<UL>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Products that are still covered by the original manufacturer’s warranty 
							or any other insurance, suppliers’ or repairers’ guarantee or warranty
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Non operating and cosmetic items, paint or product finish, accessories 
							used in or with the Product unless covered under a separate Extended Warranty Service Contract, cables, cords, print heads, cartridges, and styli, other consumables such as toner, ribbons, drums, belts, tapes, tyres or software, add-on options incorporated in a product for which this contract was purchased or normal wear and tear items not integral to the functioning of the product, or routine service.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Routine maintenance, cleaning, lubrication, adjustments or alignments and the removal of odours.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Damage caused by theft, burglary and accident including earthquake, cyclone, storm and or tempest, neglect, abuse, misuse, theft, sand, water, negligence, fire, flood, lightning, malicious damage, aircraft, impact, corrosion, battery leakage, acts of God, power outages or surges, inadequate or improper voltage or current, animal or insect infestation or intrusion.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Cost of removal or re-installation of the Product/Furniture unless specifically included in your contract.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Reception or transmission problems resulting from external causes.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Problems or defects not covered under the manufacturer’s primary written warranty.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Batteries, globes, internal and external to the product.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Breakdowns caused by computer virus or realignments to products.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Damage caused before or during delivery 
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Inherent design and manufacturing defects, and defects which are subject to a manufacturer’s recall or which are covered under a manufacturer’s program reimbursement.
							</DIV>
						</LI>

						<LI>
							<DIV style="MARGIN-LEFT: -20px">General wear and tear consistent with the age and usage, including soiling, perspiration, hair and body oils.
							</DIV>
						</LI>

						<LI>
							<DIV style="MARGIN-LEFT: -20px">Accidental or intentional damage, including chipping, denting, scratching or puncturing of the product.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Damage caused by neglect, misuse or abuse.
							</DIV>
						</LI>

					</UL>
				</DIV>
				<DIV class="smallPrint" style="LEFT: 10cm; WIDTH: 9cm; POSITION: absolute; TOP: 1cm; HEIGHT: 17.737cm; TEXT-ALIGN: justify" ms_positioning="FlowLayout">
					<ul>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Fading and colour loss.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Loss of resilience, shape and form of interior fillings.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Damage caused by animals, insects and pets.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Failure to comply with the manufacturer’s or supplier’s instructions for the care of the item.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Damage caused by the incorrect use or application of any cleaning substance or material.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Additional items or accessories used in or with the item, which are not part of the original set.
							</DIV>
						</LI>

						<LI>
							<DIV style="MARGIN-LEFT: -20px">Lifting, cracking, peeling or blemishing of leather, de-lamination, scars, bites or any other natural characteristics.
							</DIV>
						</LI>	
						<LI>
							<DIV style="MARGIN-LEFT: -20px">The cost of repairs carried out by anyone other than a Courts authorised repairer. 
							</DIV>
						</LI>
				
					</ul>
					<u>
						For Repairs</u>
					<ul>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">If the manufacturer’s warranty period for the product has not expired, contact any Courts store directly.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">If the manufacturer’s warranty period has expired and the Product is within the term.  Ring our personnel on 2071100. Repairs must only be carried out and is subject to authorisation by Courts (Mtius) Ltd.  You must not go to a repairer who is not nominated and authorised by Courts.
							</DIV>
						</LI>
						
					</ul>
					<br/>
					<b>
					FOR ALL REPAIRS, THE COURTS WARRANTY SALES RECEIPT FOR THE PRODUCT MUST BE PRESENTED WITH THIS CONTRACT.</b>
					<br/>
					<br/>
					<u>Limitation of Liability</u>
					<br/>
					<br/>
					Courts (Mtius) Ltd shall not be responsible for any loss or damage to a person or property, direct, 
					consequential or incidental arising from the use of, or inability to use the product to the extent 
					that such may be disclaimed by law. 
					<br/> 
					<br/>
					Courts (Mtius) Ltd is entitled to replace the product with the same or similar item rather 
					than repair the product if the cost of repairs is uneconomical.  The retail value of the 
					replacement will not exceed the purchase/hire price for that product which was actually paid 
					by the customer. The original service contract will terminate with the replacement of the 
					defective product and the Customer is recommended to purchase a new service contract to cover 
					his new product.  Any unpaid fee at the time of replacement will still be owing to Courts.
					<br/>
					<br/>
					In the event food is spoilt due to a covered breakdown of a refrigeration Product, Courts (Mtius) Ltd 
					will reimburse the Service Contract folder for such food spoilage up to a total of four thousand rupees 
					(Rs 4,000) per incidence provided the food spoilage has been verified and inspected within two working 
					days by the Authorised repairer and properly documented.
					<br/>
					<br/>
					<b>
					This Service Contract is not an Insurance Policy or Guarantee.  The contract only covers products
					and furniture which fail or have defects due solely to material workmanship or performance.
					</b>
					<br/>
					<br/>
					This Service Contract is not a guarantee or promise relating to the nature of the material
					workmanship or performance of the products covered by the Service Contract.
					<br/>
					<u>
						<br/>
						Cancellation</u>
					<br/>
					<br/>
					This Service Contract is only valid in Mauritius and is not transferable.  If the Customer 
					disposes of the Product during the term, Courts (Mtius) Ltd will on receipt of a written 
					application and the original Courts Warranty Sales Receipt, refund such proportion of the 
					warranty payment less 10% handling and administration charge, and less any claims paid. 
					Further Courts (Mtius) Ltd may refund the portion of warranty payment as noted above if the 
					product is returned to Courts (Mtius) Ltd.  In the event of theft, repossession, fraud, or 
					sale to another party, the Service Contract is cancelable by Courts (Mtius) Ltd.  If any 
					instalment payment is in default and unpaid for a period of three months, Courts (Mtius) Ltd 
					may cancel the Service Contract at their discretion.
					<br/>
					<u>
						<br/>
						Duty of Care</u>
					<br/>
					<br/>
					Regular cleaning and maintenance of the items is required. Furnishings should be cleaned, vacuumed 
					and rotated regularly and be kept out of direct sunlight. Neglect and lack of care by you may 
					result in denial of a claim.
					<br/>
					<u>
						<br/>
						Claims Procedure</u>	
					
					<br/>
					<ul>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Review the manufacturer instructions or care 
							guidelines to see if the problem can be rectified by you.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Check the item is within the term of the contract.
							If so, you must report the breakdown within 5 days of the occurrence or your claim 
							may be denied.
							</DIV>
						</LI>
						
						<LI>
							<DIV style="MARGIN-LEFT: -20px">Contact your local Courts store or the Service hotline.
							You will be told how to proceed with the repair.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">You will need to show your Courts warranty sales receipt 
							and contract to the repairer before the repair can be undertaken.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">In-home service will be provided for all types of product 
							except portable items, which you should bring into the Courts service centre.
							</DIV>
						</LI>
						<LI>
							<DIV style="MARGIN-LEFT: -20px">If the failure is not covered by the contract, 
							you will be charged for the cost of the repair
							</DIV>
						</LI>


					</ul>
					<b>FOR CLAIMS AND ENQUIRIES PHONE 2071100 OR CONTACT YOUR LOCAL STORE</b>
					<br/>
					<br/>
					This Contract is offered by Courts Mauritius Limited.  Courts reserve the right to vary, 
					modify, or otherwise amend the terms and conditions stated herein. In respect of all matters 
					or issues covered by this contract the decision of Courts shall be final.
					<br/>
					<b>
					FRAUD: THIS SERVICE CONTRACT IS VOID IF A FRAUDULENT DECLARATION OR CLAIM IS MADE.
					</b>
					<br/>
				</DIV>
			</div>
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'TRUE'">
				<br class="pageBreak" />
			</xsl:if>
		</div>
	</xsl:template>
</xsl:stylesheet>

  