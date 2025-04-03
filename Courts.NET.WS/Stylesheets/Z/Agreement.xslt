<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<head>
				<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"></meta>
				<style type="text/css" media="all">
                    <!--@import url(styles.css);-->
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
			<P></P>		
	</xsl:template>
	
	<xsl:template match="PAGE">
		<div style="position:relative">
			<div style="LEFT: -6px; WIDTH: 150px; POSITION: absolute; TOP: 0px; HEIGHT: 30px"><IMG style="WIDTH: 100%; HEIGHT: 49px" height="49" src="{//AGREEMENTS/@IMAGEPATH}smallLogo.jpg" width="150" />
			</div>
			<div class="smallSS" style="LEFT: 148px; WIDTH: 172px; POSITION: absolute; TOP: 15px; HEIGHT: 25px"><i>(BELIZE) 
					Ltd.</i>
			</div>
			<div class="smallSS" style="LEFT: 450px; WIDTH: 172px; POSITION: absolute; TOP: 15px; HEIGHT: 25px">Account Number
					
			</div>
			<div class="smallSS" style="LEFT: 621px; WIDTH: 100px; POSITION: absolute; TOP: 15px; HEIGHT: 21px" align="center">Original
			</div>
			<div style="LEFT: 1px; WIDTH: 300px; POSITION: absolute; TOP: 43px; HEIGHT: 30px"><span class="HPHeader">THIS AGREEMENT 
					is made</span>
			</div>
			<div class="DateBold" style="LEFT: 290px; WIDTH: 100px; POSITION: absolute; TOP: 43px; HEIGHT: 30px"> 
				        <xsl:value-of select="HEADER/DATE" /> 
			</div>
			<div style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 5px; BORDER-TOP: gray thin solid; PADDING-LEFT: 5px; LEFT: 10.5cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray thin solid; WIDTH: 5cm; PADDING-TOP: 5px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 1.15cm; HEIGHT: 0.85cm; TEXT-ALIGN: center">
				<xsl:value-of select="HEADER/ACCTNO" />
			</div>
			<div style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 5px; BORDER-TOP: gray thin solid; PADDING-LEFT: 5px; LEFT: 17.3cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray thin solid; WIDTH: 0.85cm; PADDING-TOP: 5px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 1.15cm; HEIGHT: 0.85cm; TEXT-ALIGN: center"></div>
			<div class="smallSS" style="LEFT: 1px; WIDTH: 718px; POSITION: absolute; TOP: 88px; HEIGHT: 31px">
				<b>Between UNICOMER (BELIZE) 
					LIMITED</b> whose principal place of business is at Northern Highway in Belize City (hereinafter called the "Owner" which expression 
				shall include their successors and assigns)
			</div>
			<div class="smallSS" style="LEFT: 70px; WIDTH: 159px; POSITION: absolute; TOP: 140px; HEIGHT: 31px">
				<b>Title, First Name, Surname, Address<br/>
					(hereinafter called "The Customer")</b>
			</div>
			<div class="smallSS" style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 4px; BORDER-TOP: gray thin solid; PADDING-LEFT: 4px; LEFT: 6.5cm; PADDING-BOTTOM: 4px; BORDER-LEFT: gray thin solid; WIDTH: 12.5cm; PADDING-TOP: 4px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 3.15cm; HEIGHT: 2.3cm">
				<xsl:value-of select="HEADER/NAME" /><br/>
				<xsl:value-of select="HEADER/JOINTNAME" /><br />
				<xsl:value-of select="HEADER/ADDR1" /><br/>
				<xsl:value-of select="HEADER/ADDR2" /><br/>
				<xsl:value-of select="HEADER/ADDR3" /><br/>
				<xsl:value-of select="HEADER/POSTCODE" />
			</div>
			<div class="smallSS" style="LEFT: 13cm; POSITION: absolute; TOP: 3.35cm; HEIGHT: 2.3cm">
				ID/IC No. <xsl:value-of select="HEADER/CUSTID" />
			</div>
			<div class="smallSS" style="LEFT: 0px; WIDTH: 108px; POSITION: absolute; TOP: 200px; HEIGHT: 17px">
				<b>Schedule of Goods</b>
			</div>
			<div style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 5px; BORDER-TOP: gray thin solid; PADDING-LEFT: 1px; LEFT: 0cm; BORDER-LEFT: gray thin solid; WIDTH: 19cm; PADDING-TOP: 20px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 5.70cm; HEIGHT: 8.5cm">
				<xsl:apply-templates select="LINEITEMS" />
			</div>
			<div style="BORDER-RIGHT: gray thin solid; LEFT: 0cm; WIDTH: 1.5cm; POSITION: absolute; TOP: 5.7cm; HEIGHT: 8.5cm">
			</div>
			<div style="BORDER-RIGHT: gray thin solid; LEFT: 0cm; WIDTH: 14.8cm; POSITION: absolute; TOP: 5.7cm; HEIGHT: 8.5cm">
			</div>
			<div style="LEFT: 0cm; WIDTH: 19cm; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 5.7cm; HEIGHT: 0.6cm">
			</div>
			<div style="LEFT: 0cm; WIDTH: 19cm; BORDER-TOP: gray thin solid; POSITION: absolute; PADDING-TOP: 5px; PADDING-RIGHT: 5px; TOP: 13.0cm; HEIGHT: 1.35cm">
				<table width="100%" ID="Table1">
					<tr>
						<td width="80%"></td>
						<td align="right" width="20%">
							<xsl:value-of select="../FOOTER/GOODSVAL" />
						</td>
					</tr>
				</table>
			</div>
			<div class="smallSS" style="LEFT: 17px; WIDTH: 29px; POSITION: absolute; TOP: 220px; HEIGHT: 17px">
				<b>QTY</b>
			</div>
			<div class="smallSS" style="LEFT: 197px; WIDTH: 187px; POSITION: absolute; TOP: 220px; HEIGHT: 16px" align="center">
				<b>DESCRIPTION OF GOODS</b>
			</div>
			<div class="smallSS" style="LEFT: 618px; WIDTH: 45px; POSITION: absolute; TOP: 220px; HEIGHT: 16px">
				<b>PRICE</b>
			</div>
			<div class="smallSS" style="LEFT: 360px; WIDTH: 180px; POSITION: absolute; TOP: 505px; HEIGHT: 20px" align="right">
				Price of Goods Supplied
			</div>
			<div style="BORDER-RIGHT: gray thin solid; BORDER-TOP: gray thin solid; LEFT: 10.4cm; BORDER-LEFT: gray thin solid; WIDTH: 8.6cm; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 14.7cm; HEIGHT: 2.5cm">
			</div>
			<div class="SmallPrint" style="LEFT: 400px; WIDTH: 180px; POSITION: absolute; TOP: 620px; HEIGHT: 5px" align="Left">
				<table class="SmallPrint" width="100%">
					<tr>
						<td width="70%" align="left">Deposit:</td>
						<td width="30%" align="left"><xsl:value-of select="../FOOTER/DEPOSIT" /></td>						
					</tr>
				</table>
			</div>
            <div class="RFHead1" style="LEFT: 0px; WIDTH: 400px; POSITION: absolute; TOP: 590px; HEIGHT: 125px" align="left">
				<b>Adding Value EVERY day!</b><br/>
            </div>

			<xsl:variable name="ninety" select="../FOOTER/NINETYDAYS" />
			<xsl:if test="$ninety != ''">
				<div class="smallSS" style="LEFT: 200px; WIDTH: 170px; POSITION: absolute; TOP: 607px; HEIGHT: 85px" align="Left">
					<b>Courts will rebate to the Total Regular Price if payments of
					<xsl:value-of select="../FOOTER/NINETYDAYS" /> are made 30, 60 and 90 days after delivery.</b>
				</div>
			</xsl:if>

			<div class="smallSS" style="LEFT: 400px; WIDTH: 310px; POSITION: absolute; TOP: 560px; HEIGHT: 85px" align="Left">
				<b>Schedule of Payments</b><br/>
				<br/>
				Balance payable by [<xsl:value-of select="../FOOTER/INSTALNO" /> ] instalments of $[<xsl:value-of select="../FOOTER/FIRSTINST" /> ]
				and a <br/>
				final instalment of $[<xsl:value-of select="../FOOTER/FINALINST" />]
			</div>
			<div class="smallSS" style="LEFT: 0px; WIDTH: 717px; POSITION: absolute; TOP: 655px; HEIGHT: 31px" align="center">
				WHEREBY UNICOMER will supply and the customer will take the goods listed in the  
				Schedule upon the following terms and conditions: 
				<br/> 
				<br/>
			</div>
			<div style="BORDER-RIGHT: gray thin solid; BORDER-TOP: gray thin solid; LEFT: 0cm; BORDER-LEFT: gray thin solid; WIDTH: 19cm; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 18.45cm; HEIGHT: 0.5cm">
			</div>
			<div class="RFHead1" style="LEFT: 253px; WIDTH: 192px; POSITION: absolute; TOP: 700px; HEIGHT: 19px" align="center" id="DIV1" language="javascript" onclick="return DIV1_onclick()">
				<b>TERMS AND CONDITIONS</b>
			</div>
			<div class="SmallPrint" style="LEFT: 0px; WIDTH: 329px; POSITION: absolute; TOP: 730px; HEIGHT: 99px" align="left" id="Div2" language="javascript" onclick="return DIV1_onclick()">
				In these Terms and Conditions, the following words have the following meanings:
				<br/>
				<br/>
				<b>"Owner"</b> is the owner of the Goods listed in the Schedule above.
				<br/>
				<br/>
				<b>"Customer"</b> are the hirer of the Goods listed in the Schedule above.
				<br/>
				<br/>
				<b>"Price"</b> is the total purchase price/amount payable for 
				the Goods listed in the Schedule of Goods.
                                <br/>
                                <br/>
                                <b>IN THIS AGREEMENT</b>
			</div>
			<div class="SmallPrint" style="LEFT: 36px; WIDTH: 650px; POSITION: absolute; TOP: 840px; HEIGHT: 99px" align="justify" id="Div3" language="javascript" onclick="return DIV1_onclick()">
				The Customer has paid to the owner on the signing hereto the deposit of initial installment stated in the above schedule and undertakes to pay the Owner:
				<br/>
				<br/>
                                <br/>
				Punctually and without previous demand over the period of this agreement, commencing one month after delivery,
				such payments to be made to the Owner at Northern Highway, in Belize City, or at such other address as the Owner may direct;
				<br/>
				<br/>
                                <br/>
				On demand, any expenses incurred by the Owner in ascertaining the whereabouts of the customer, in the collection 
				of overdue payments, or in applying for the enforcing and payment of sum payable hereunder including reasonable attorneys fees, bailiff fees, late fees and any
				other costs (whether legal or otherwise) incurred by the Owner in respect to any of the foregoing matters whether legal proceddings shall
                                been instituted or not, and irrespective of the method of recovery;
                                <br/>
                                <br/>
				<br/>
			</div>
			<div class="SmallPrint" style="LEFT: 0px; WIDTH: 30px; POSITION: absolute; TOP: 840px; HEIGHT: 152px" align="justify" id="Div4" language="javascript" onclick="return DIV1_onclick()">
				1
				<br/>
				<br/>
                                <br/>
                                i)
				<br/>
				<br/>
				<br/>
                                <br/>
                                ii)
				<br/>
				<br/>
				<br/>
			</div>
			<br class="pageBreak"/>
			<div style="position:relative">
			<div class="SmallPrint" style="LEFT: 2cm; WIDTH: 15.2cm; POSITION: absolute; TOP: -1.25cm; HEIGHT: 18cm" align="justify">
				<br/>
                                <br/>
                                <br/>
                                <br/>
                                <br/>
                                In default of payment of any of the monthly payments interest will be charged 
				thereon on a monthly basis at the same annual percentage rate applicable to  
				this lease agreement.
				<br/>
				<br/>
                                <br/>
				The goods are, and shall at all times be and remain, the sole and exclusive 
				personal property of the Owner, and the Customer shall have no right, title 
				or interest therein or thereto except as is expressly set forth in this 
				agreement. 
				<br/>
				<br/>
                                <br/>
				Subject to clause 2 above, upon the payment of the last monthly payment due  
				under this agreement inclusive of any additional charges as a result of late payment of 
				any monthly payments or otherwise, the Customer may with the Owner's permission, retain 
				possession of and have use of the goods for the lifetime of the goods the subject matter of this hire,
                                without the necessity to affect any further payment.
                                <br/>
                                <br/>
                                <br/>
                                The Customer shall keep the goods specified in the Schedule in good condition and  
				shall not suffer any of them to be taken in any distress, execution or legal process 
				or be burdened with any priority claim over the claim of the Owner for the value of
                                the goods. The Customer will not remove the goods from the country of Belize, and will advise
                                the Owner immediately of any new address to which the customer moves or the goods are moved.
				<br/>
				<br/>
				<br/>
				Depending on whether the purchase has been made for a short or long term period with Unicomer, <b>included in the service charge is
				an amount of, in case of short term, 0.1% of the total service charge value or, in the
				case of long term, 0.2% of the total service charge value relating </b>to
				the purchase of insurance by the customer from RFG Insurance Co. Ltd
				under Master Policy No. CIO200.  In the event of the goods being destroyed by All
                                Risks or the customer dying or becoming disabled, or, additionally under the Unicomer long
                                term Payment Protection Plan, the loss results from burglary or the customer being
                                made redundant or retrenched, the outstanding payments due will be paid by the insurer subject
                                to the terms and conditions of the Policy. A copy of the Policy wording is available on request.
                                <br/>
				<br/>
				<br/>
				Payment of any policy benefit shall be made to Unicomer (Belize) Limited as Trustee
				for the Customer to be credited to the appropriate account of the Customer and shall
				be in complete discharge of the liability with respect to the claim to which such
				benefits relate.
				<br/>
				<br/>
				<br/>
				If the conditions on the policy for insurance of the goods against Fire, Floods, Hurricane
				&amp; Theft or death of the Customer are not fulfilled by the Customer, his/her heirs or
				representative will be responsible for the payment of the whole balance outstanding.
				<br/>
				<br/>
				<br/>
				No forebearance, extension of time, acceptance of lower amount than that prescribed as
				a monthly payment, or waiver shown to the Customer shall prejudice the Owner's rights
				hereunder and no waiver of any breach of this agreement whatsoever shall operate as
				notice of waiver of any such breach thereafter committed or suffered, unless in writing.
				<br/>
				<br/>
				<br/>
				This agreement is conditional upon the requests for payment plan being approved by the 
				Owner's Directors, such approval to be signified by delivery of the goods.  If 
				so approved, the Agreement shall be binding and of full effect, or otherwise, to be 
                                void and of no effect.
				<br/>
				<br/>
				<br/>
				In the event that the payment due under this agreement are overdue, the Owner may terminate 
				this agreement by giving written notice of termination to the Customer(notice sent by 
                                prepaid postage or delivered to the above address of the Customer shall constitute sufficient
                                service), and upon termination the Owner shall be entitled, without prejudice to clause
                                1 above, but in addition thereto, to enter any premises where the goods may be kept without 
                                permission of the Customer or without let or hindrance and to collect the goods there
                                from and the Customer shall have no recourse or redress against the Owner.  The Owner is
                                thereafter entitled to dispose of the goods in anyway without compensation to the Customer.
				<br/>
				<br/>
				<br/>
				The Customer, at his/her own cost and expense shall, keep the goods in repair, condition
				and working order and shall furnish any and all parts, mechanisms and devices required
                                to keep the goods in repair.
                                <br/>
                                <br/>
                                <br/>
				Without the written consent of the Owner, the Customer shall not:
				<br/>
				<br/>
				<br/>
				assign, transfer, pledge or hypothecate this agreement, the goods or any part thereof,
                                or any interest therein, or
				<br/>
				<br/>
				<br/>
				sublet or lend the goods or any part thereof, or permit the goods or any part thereof 
				to be used by anyone other than the Customer or the Customer's employee 
				<br/>
				<br/>
				<br/>
				This agreement constitutes the entire agreement between the Owner and the 
				Customer and the same shall not be varied or added to except by written 
				instrument signed by the Owner and the Customer.
				<br/>
				<br/>
				<br/>
			</div>
			<div class="SmallPrint" style="LEFT: 1cm; WIDTH: 30px; POSITION: absolute; TOP: -1.25cm; HEIGHT: 18cm" align="justify">
				<br/>
                                <br/>
                                <br/>
                                <br/>
                                <br/>
                                iii)
                                <br/>
                                <br/>
                                <br/>
                                <br/>
                                2
                                <br/>
                                <br/>
                                <br/>
                                <br/>
                                3
                                <br/>
                                <br/>
                                <br/>
                                <br/>
                                <br/>
                                <br/>
                                4
				<br/>
				<br/>
				<br/>
				<br/>
                                <br/>
                                <br/>
                                5
				<br/>
				<br/>
				<br/>
				<br/>
				<br/>
				<br/>
                                <br/>
                                <br/>
                                <br/>                                
                                <br/>
                                6
				<br/>
				<br/>
				<br/>
				<br/>
                                7
				<br/>
				<br/>
				<br/>
				<br/>
				8
				<br/>
				<br/>
				<br/>
				<br/>
				<br/>
				9
				<br/>
				<br/>
				<br/>
                                <br/>
                                <br/>
				10
				<br/>
				<br/>
				<br/>
                                <br/> 
                                <br/>                                
                                <br/>
                                <br/>
                                <br/>
                                11
                                <br/>
                                <br/>
                                <br/>
                                <br/>
                                12
                                <br/>
                                <br/>
                                <br/>
                                a)
				<br/>
				<br/>
                                <br/>
                                b)
				<br/>
				<br/>
				<br/>
                                <br/>
                                13
				<br/>
				<br/>
				<br/>
                                <br/>
			</div>
			<div class="SmallPrint" style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 10px; BORDER-TOP: gray thin solid; PADDING-LEFT: 10px; LEFT: 0cm; PADDING-BOTTOM: 10px; BORDER-LEFT: gray thin solid; WIDTH: 9.15cm; PADDING-TOP: 10px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 18.3cm; HEIGHT: 6.65cm" align="justify">
				IN WITNESS WHEREOF, the parties hereto have set their hands the day of the year   
				first above written.
				<br/>
				<br/>
				[<xsl:value-of select="../FOOTER/INSTALNO" /> ] instalments of $[<xsl:value-of select="../FOOTER/FIRSTINST" /> ]
				and a <br/>
				final instalment of $[<xsl:value-of select="../FOOTER/FINALINST" />]											
				<br/>
				<br/>
				<br/>
				Print Name:
				<br/>
				<br/>
				<br/>
                                Signature of the Customer:
                                <br/>
                                <br/>
                                <br/>
				Witness to Customer's Signature:
				<br/>
				<br/>
				<br/>
				Name:
				<br/>
				<br/>
				<br/>
				Address:
				<br/>
				<br/>
				<br/>
				For and on behalf of the Owner:
			</div>			
			<!-- add the dotted line divs to the customer signature box -->			
			<div style="BORDER-TOP: silver 1px solid; LEFT: 1.9cm; WIDTH: 6.9cm; POSITION: absolute; TOP: 20.5cm; HEIGHT: 0.5cm">
			</div>
			<div style="BORDER-TOP: silver 1px solid; LEFT: 4.0cm; WIDTH: 4.8cm; POSITION: absolute; TOP: 21.2cm; HEIGHT: 0.5cm">
			</div>
			<div style="BORDER-TOP: silver 1px solid; LEFT: 4.1cm; WIDTH: 4.7cm; POSITION: absolute; TOP: 21.9cm; HEIGHT: 0.5cm">
			</div>
			<div style="BORDER-TOP: silver 1px solid; LEFT: 1.3cm; WIDTH: 7.5cm; POSITION: absolute; TOP: 22.6cm; HEIGHT: 0.5cm">
			</div>
			<div style="BORDER-TOP: silver 1px solid; LEFT: 1.5cm; WIDTH: 7.3cm; POSITION: absolute; TOP: 23.3cm; HEIGHT: 0.5cm">
			</div>
			<div style="BORDER-TOP: silver 1px solid; LEFT: 4.0cm; WIDTH: 4.8cm; POSITION: absolute; TOP: 24.0cm; HEIGHT: 0.5cm">
			</div>			
			<div class="SmallPrint" style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 10px; BORDER-TOP: gray thin solid; PADDING-LEFT: 10px; LEFT: 9.8cm; PADDING-BOTTOM: 10px; BORDER-LEFT: gray thin solid; WIDTH: 9.15cm; PADDING-TOP: 10px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 18.3cm; HEIGHT: 6.65cm" align="justify">
				<b>GUARANTEE</b> (Where required by Courts)
				<br/>
				<br/>
				In return for Unicomer (Belize) Limited entering into this Agreement with You at my request, I agree to guarantee the performance of all your obligations under this Agreement and agree to be jointly and severally liable to Unicomer under this Agreement, which I have read and understood, if You fail to perform your obligations under this Agreement.
				<br/>
				<br/>
				Print Name:
				<br/>
				<br/>
                                <br/>
				Signature:
				<br/>
				<br/>
                                <br/>
                                <br/>
				Dated the __________________  day of _______________ 2007
				<br/>
				<br/>
				Address:
				<br/><br/><br/><br/>
				Signature of Witness:
				<br/>
                <br/>
				<br/>
				Address of Witness:
			</div>
			</div>
			<xsl:variable name="lastPage" select="LAST" />	
			<xsl:variable name="lastAgreement" select="../LAST" />				
			<xsl:if test="$lastPage = 'False' or $lastAgreement = 'FALSE' ">				
				<br class="pageBreak" />	<!-- if it's not the last page -->
			</xsl:if>
		</div>
	</xsl:template>		
	
	<xsl:template match="LINEITEMS">
		<xsl:apply-templates select="LINEITEM" />
	</xsl:template>
	
	<xsl:template match="LINEITEM">
		<xsl:variable name="addTo" select="ADDTO" />
		<xsl:if test="$addTo = 'True'">
			<TABLE class="normalSmall" width="100%" border="0">
				<TR>
					<td align="center" width="8%">
						<xsl:value-of select="QUANTITY" />
					</td>
					<td width="92%">
						<xsl:value-of select="DESC" />
						(<xsl:value-of select="ACCTNO" />)
					</td>
				</TR>
			</TABLE>
		</xsl:if>
		<xsl:if test="$addTo != 'True'">
			<table class="smallSS" width="100%">
				<tr>
					<td align="center" width="8%">
						<xsl:value-of select="QUANTITY" />
					</td>
					<td width="62%">
						<xsl:value-of select="DESC" />
					</td>
					<td align="right" width="30%">
						<xsl:value-of select="VALUE" />
					</td>
				</tr>
			</table>
		</xsl:if>	
	</xsl:template>
	
</xsl:stylesheet>

  