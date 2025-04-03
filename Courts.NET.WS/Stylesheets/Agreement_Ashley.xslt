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
	</xsl:template>
	
	<xsl:template match="AGREEMENT">
			<xsl:apply-templates select="PAGE" />	
	</xsl:template>
	
	<xsl:template match="PAGE">
		<div style="position:relative">
			<div style="LEFT: 13cm; WIDTH: 4.5cm; POSITION: absolute; TOP: 0cm; HEIGHT: 1.5cm"><IMG src="{//AGREEMENTS/@IMAGEPATH}Ashley.png" />
			</div>
			<div style="LEFT: 0cm; WIDTH: 18.5cm; POSITION: absolute; TOP: 0cm; HEIGHT: 2cm"><span class="HPHeader">
				UNICOMER <br/>(<xsl:value-of select="HEADER/COUNTRYNAME" />) Ltd
		</span>
			</div>
			<div class="normal" style="LEFT: 4cm; WIDTH: 3.5cm; POSITION: absolute; TOP: 0.5cm; HEIGHT: 2cm">ACCOUNT NUMBER
			</div>
			<div style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 5px; BORDER-TOP: gray thin solid; PADDING-LEFT: 5px; LEFT: 7.5cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray thin solid; WIDTH: 5cm; PADDING-TOP: 5px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 0cm; HEIGHT: 1cm; TEXT-ALIGN: center">
				<xsl:value-of select="HEADER/ACCTNO" />
			</div>
			<div class="smallSS" style="LEFT: 0cm; WIDTH: 18.5cm; POSITION: absolute; TOP: 1.5cm; HEIGHT: 2cm">
				AGREEMENT made this ____________________ day of ____________________ 20 __________
				<br/>
				Between UNICOMER (<xsl:value-of select="HEADER/COUNTRYNAME" />) Ltd whose registered office is situated at Corner Don Miguel Road &amp; Churchill Roosevelt, Highway, El Socorro, San Juan, Trinidad
				<br/>
				(hereinafter called "the Owners" which expression shall include their successors and assigns) and (hereinafter called "the Hirer")
			</div>
			<div class="smallSS" style="WIDTH: 350px; POSITION: absolute; TOP: 3cm; HEIGHT: 3cm">
				"This agreement is subject to verification of customer information and final credit approval by UNICOMER.
				Delivery of goods will ONLY be made after final approval."
				<br/>
				Monthly instalment payment must be made one (1) month after delivery.
				<br/>
				I HAVE SEEN AND ACKNOWLEDGE AND AGREE TO THIS CONDITION.
				<br/>
				<br/>
				________________________________________
			</div>
			<div class="smallSS" style="LEFT: 10.25cm; WIDTH: 2cm; POSITION: absolute; TOP: 3.25cm; HEIGHT: 2cm">
				Name:
				<br/>
				<br/>
				Address:
			</div>
			<div class="smallSS" style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 4px; BORDER-TOP: gray thin solid; PADDING-LEFT: 4px; LEFT: 11.5cm; BORDER-LEFT: gray thin solid; WIDTH: 7cm; POSITION: absolute; TOP: 3cm; HEIGHT: 2.5cm">
				<xsl:value-of select="HEADER/NAME" /><br />
				<xsl:value-of select="HEADER/JOINTNAME" /><br />
				<xsl:value-of select="HEADER/ADDR1" /><br />
				<xsl:value-of select="HEADER/ADDR2" /><br />
				<xsl:value-of select="HEADER/ADDR3" /><br />
				<xsl:value-of select="HEADER/POSTCODE" />  
			</div>
			<div style="BORDER: grey thin solid; LEFT: 0cm; TOP: 5.5cm; WIDTH: 18.5cm; HEIGHT: 18cm; POSITION: absolute">
			</div>
			<div style="BORDER-TOP: gray thin solid; PADDING-RIGHT: 5px; PADDING-LEFT: 1px; LEFT: 0cm; WIDTH: 18.5cm; POSITION: absolute; TOP: 6cm; HEIGHT: 7cm">
				<xsl:apply-templates select="LINEITEMS" />
			</div>
			<div style="LEFT: 0cm; WIDTH: 18.5cm; BORDER-TOP: gray thin solid; POSITION: absolute; TOP: 5.5cm">
			</div>
			<div style="LEFT: 0cm; WIDTH: 18.5cm; BORDER-TOP: gray thin solid; POSITION: absolute; PADDING-RIGHT: 5px; TOP: 11cm; HEIGHT: 3.5cm">
        <table class="smallSS" style="border-collapse: collapse; border-spacing: 0;" width="100%" ID="Table1">
					<tr>
						<td rowspan="6" style="padding-left: 10px" width="40%">
              Balance payable by [<xsl:value-of select="../FOOTER/INSTALNO" />] instalments of [<xsl:value-of select="../FOOTER/FIRSTINST" />] and a final instalment of  [<xsl:value-of select="../FOOTER/FINALINST" />]Balance payable by <xsl:value-of select="../FOOTER/INSTALNO" /> instalments of <xsl:value-of select="../FOOTER/FIRSTINST" /> and a final instalment of <xsl:value-of select="../FOOTER/FINALINST" />
              <br />
              <br />
              Agree minimum payment <xsl:value-of select="../FOOTER/TOPAY" />
            </td>
            <td align="right" width="40%">Goods Value:</td>
						<td align="right" width="20%">
							<xsl:value-of select="../FOOTER/GOODSVAL" />
						</td>
					</tr>
          <tr>
            <td align="right" width="40%">Deposit:</td>
            <td align="right" width="20%">
              <xsl:value-of select="../FOOTER/DEPOSIT" />
            </td>
          </tr>
          <tr>
            <td align="right" width="40%">Balance/Credit Extended:</td>
            <td align="right" width="20%">
              <xsl:value-of select="../FOOTER/CREDIT" />
            </td>
          </tr>
          <tr>
            <td align="right" width="40%">Charge for Credit:</td>
            <td align="right" width="20%">
              <xsl:value-of select="../FOOTER/DT" />
            </td>
          </tr>
          <tr>
            <td align="right" width="40%">Balance Payable:</td>
            <td align="right" width="20%">
              <xsl:value-of select="../FOOTER/BALANCE" />
            </td>
          </tr>
          <tr>
            <td align="right" width="40%">Total Price:</td>
            <td align="right" width="20%">
              <xsl:value-of select="../FOOTER/TOTAL" />
            </td>
          </tr>
				</table>
			</div>
			<div style="LEFT: 1.5cm; WIDTH: 14cm; TOP: 5.5cm; HEIGHT: 5.5cm; BORDER-LEFT: gray thin solid; BORDER-RIGHT: gray thin solid; POSITION: absolute">
			</div>
			<div class="smallSS" style="LEFT: 0cm; WIDTH: 1.5cm; POSITION: absolute; TOP: 5.5cm; HEIGHT: 0.5cm; padding:3px" align="center">
				<b>QTY</b>
			</div>
			<div class="smallSS" style="LEFT: 1.5cm; WIDTH: 13.5cm; POSITION: absolute; TOP: 5.5cm; HEIGHT: 0.5cm; padding:3px" align="center">
				<b>DESCRIPTION OF GOODS</b>
			</div>
			<div class="smallSS" style="LEFT: 618px; WIDTH: 45px; POSITION: absolute; TOP: 5.5cm; HEIGHT: 0.5cm; padding:3px" align="center">
				<b>PRICE</b>
			</div>
			<div class="smallSS" style="LEFT: 0cm; WIDTH: 18.5cm; POSITION: absolute; TOP: 13.5cm; HEIGHT: 0.5cm; BORDER-TOP: gray thin solid; BORDER-BOTTOM: gray thin solid" align="center">
				WHEREBY UNICOMER will supply and the Customer will take the goods listed in the schedule upon the following terms and conditions:
			</div>
			<div class="SmallPrint" style="LEFT: 0.5cm; WIDTH: 7.5cm; POSITION: absolute; TOP: 14.5cm; HEIGHT: 10cm" align="center" id="Div2" language="javascript" onclick="return DIV1_onclick()">
				NOTICE<br />
				PURSUANT TO THE HIRE PURCHASE ACT CHAP. 82.33<br />
				RIGHT OF HIRER TO TERMINATE AGREEMENT
			</div>
			<div class="SmallPrint" style="LEFT: 0.5cm; WIDTH: 8cm; POSITION: absolute; TOP: 15.5cm; HEIGHT: 9cm" align="justify" id="Div3" language="javascript" onclick="return DIV1_onclick()">
				The Hirer may put an end to this Agreement by giving notice to termination in writing to any person who is entitled
				to collect or receive the hire-rent and at the same time or prior thereto delivering the goods to the Owners.
				<br />
				<br />
				He must then pay any installments which are in arrears at the time he gives notice.
				<br />
				<br />
				If the Hirer does not deliver the Articles to the Owners at the time mentioned in paragraph1 above the notice of 
				termination will be ineffective and the Agreement will remain in force.
				<br />
				<br />
				If the Articles have been damaged owing to the Hirer having failed to take reasonable care of them, the Owners may 
				sue him for the amount of the damage unless that amount can be agreed between the Hirer and the Owners.
				<br />
				<br />
				The Hirer should see whether this Agreement contains provisions allowing him to put an end to the Agreement on terms
				more favourable to him than those just mentioned. If it does he may put an end to the Agreement on those terms.
			</div>
			<div class="SmallPrint" style="LEFT: 0cm; WIDTH: 0.5cm; POSITION: absolute; TOP: 15.5cm; HEIGHT: 9cm" align="center" id="Div4" language="javascript" onclick="return DIV1_onclick()">
				1
				<br />
				<br />
				<br />
				<br />
				<br />
				2
				<br />
				<br />
				<br />
				3
				<br />
				<br />
				<br />
				<br />
				4
				<br />
				<br />
				<br />
				<br />
				<br />
				5
			</div>
			<div class="SmallPrint" style="LEFT: 9.75cm; WIDTH: 8cm; POSITION: absolute; TOP: 14.5cm; HEIGHT: 1.5cm" align="center" id="Div2" language="javascript" onclick="return DIV1_onclick()">
				RESTRICTION OF OWNERS RIGHT TO RECOVER GOODS WHERE SEVENTY PER CENTUM OF THE HIRE PURCHASE PRICE HAS 
				BEEN PAID ON AGREEMENTS UP TO $15000.00
			</div>
			<div class="SmallPrint" style="LEFT: 9.75cm; WIDTH: 8cm; POSITION: absolute; TOP: 15.5cm; HEIGHT: 4cm" align="justify" id="Div5" language="javascript">
				After the Hirer pays $ ____________________ then unless the Hirer has himself put an end to the Agreement or has 
				committed some breach of the Agreement the Owners of the goods cannot take them back from the Hirer without the 
				Hirer's consent unless the Owners obtain an order from the Court.
				<br />
				<br />
				If the owners apply to the Court for such an order, the Court may if it thinks it's just to do so allow the Hirer
				to keep either:<br/>
				(a) the whole of the goods on condition that the Hirer pay the balance of the price in a manner ordered by 
				the Court.<br/>
				(b) a fair proportion of the goods having regard to what the Hirer has already paid.
			</div>
			<div class="SmallPrint" style="LEFT: 9.75cm; WIDTH: 8cm; POSITION: absolute; TOP: 18.75cm; HEIGHT: 1.5cm" align="center" id="Div2" language="javascript" onclick="return DIV1_onclick()">
				RESTRICTION OF OWNERS RIGHT TO RECOVER GOODS WHERE SEVENTY PER CENTUM OF THE HIRE PURCHASE PRICE HAS 
				BEEN PAID ON AGREEMENTS UP TO $15000.00
			</div>
			<div class="SmallPrint" style="LEFT: 9.75cm; WIDTH: 8cm; POSITION: absolute; TOP: 20cm; HEIGHT: 3.5cm" align="justify" id="Div5" language="javascript">
				Where less than $ ____________________ has been paid, unless the Hirer has himself put an end to the Agreement
				or has committed some breach of the Agreement other than failure to pay an installment of the Hire Purchase 
				Price, the Owners of the goods cannot take them back from the Hirer without the Hirer's concent unless the 
				Owners have given to the Hirer twenty one clear days written notice of the intention to do so.
				<br />
				<br />
				If within the said period of twenty one days the Hirer pays to the Owners all installments of the Hire Purchase
				Price, due at the date of such notice the Agreement will continue in force.
			</div>
			<div class="SmallPrint" style="LEFT: 9.25cm; WIDTH: 0.5cm; POSITION: absolute; TOP: 15.5cm; HEIGHT: 9cm" align="center" id="Div6" language="javascript" onclick="return DIV1_onclick()">
				1
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				2
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				1
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				2
			</div>
			<div class="SmallPrint" style="LEFT: 0cm; WIDTH: 18.5cm; POSITION: absolute; TOP: 23.5cm; HEIGHT: 2.5cm">
			The Hirer agrees that the Owners have handed to me a complete copy of this Agreement. I have been informed of the cash price
			of the goods before entering into this Agreement and I understand the contents of this Agreement and of the schedule thereto.
			<br/>
			<br/>
			<br/>
				__________________________________________ NAME  _________________________________________________ SIGNATURE
			</div>
			
			<br class="pageBreak" />
			<div style="position:relative">
				<div class="SmallPrint" style="LEFT: 0cm; WIDTH: 18.5cm: POSITION:absolute; TOP: 0cm; HEIGHT: 2cm">
					WHEREBY the Owners let and the Hirer takes on hire the Articles and Effects specified in the Schedule hereto (hereinafter
					called "the Articles" subject to and upon the following terms and conditions:
				</div>
				<div class="SmallPrint" style="LEFT: 0.5cm; WIDTH: 18cm; POSITION: absolute; TOP: 1cm; HEIGHT: 18cm; font-size: 7.5pt" align="justify">
					The Hirer shall pay the Owners on the signing hereof the sum of $ ................... in consideration of the option
					to purchase hereby granted and (b) thereafter punctually and without previous demand the monthly rent of $ ...................
					on the ................... day of each month commencing next, such payment to be made to the Owners at their Office at
					Corner of Don Miguel Road and Churchill Roosevelt Highway, El Socorro, San Juan or at such other address as <u>they</u>
					may direct (c) on demand and expense incurred by the Owners in ascertaining the whereabouts of the Hirer or of the
					Articles or the preparing to recover or in recovering possession of the Articles from the Hirer or from any other
					person (including any payment made by the Owners in discharge or satisfaction of any lien or alleged lien on the goods)
					or in applying for or enforcing payment of any rent or other sums payable hereunder including any legal charges as between
					Attorney-at-Law and client incurred by the Owners in respect of any of the foregoing matters whether legal proceedings 
					shall have been instituted or not.
					<br />
					<br />
					When the total amount paid under Clause 1 (a) and (b) hereof shall equal the total amount of the Hire Purchase price shown in
					the Schedule hereto, the Articles shall become the sole and absolute property of the Hirer. The Hirer may at any time during
					the continuance of the hiring, purchase the Articles by payment to the Owners of an amount which together with all previous
					payments for hire and the amount paid for the option to purchase equals the agreed value of the whole of the Articles, but
					unless and until such payment has been made in full, the Articles shall remain the property of the Owners, and the Hirer
					shall remain the property of the Owners, and the Hirer shall keep the goods specified in the schedule in good condition and
					shall be responsible for all loss of damage hoever occasioned (fair wear and tear excepted).
					<br />
					<br />
					Depending on whether the purchase has been made under the terms of Courts EasyPay Silver or Courts EasyPay Gold, 0.1% or 0.2%
					of the monthly service charge, respectivelt, relates to the purchase of insurance on an installment basis by the customer from 
					Guardian General Insurance Limited under Master Policy NO GL-50000247.
					<ul style="margin-top: 4px; margin-bottom: 5px;">
						<li>In the event of the customer's death, thecustomer becoming disabled or the goods being destroyed by
						All Risk - EasyPay Silver</li>
						<li>Or additionally, the loss of the goods results from burglary or in the event of the customer being
						made redundant or retrenched - EasyPay Gold</li>
					</ul>
					<div >
						The outstanding payments date, less any arrears, will be paid by the insurer subject to the terms and conditions of the Policy
					</div>
					<ul style="margin-top: 4px; margin-bottom: 5px;">
						<li>Payment for death in the six months immediately following the Credit Agreement's inception is limited to death in "sudden
						or accidental" ciccumstances.</li>
						<li>Claims for disablement or death arising from any sexually trasmitted disease or HIV/AIDS related illness are excluded.</li>
						<li>Claims for total losses, disablement or death arising from terrorist acts are excluded.</li>
					</ul>
					Other terms &amp; conditions apply a copy of the policy wording will be made available on request.
					<br />
					The outstanding payments due will be paid by the insurer subject to the terms and conditions of the Policy. A copy of the Policy
					wording is available on request.
					<br />
					<br />
					The Hirer shall keep the Articles in the Hirer's own possession and control at his above-named address from which premises they
					shall not be removed without the previous written concent of the Owners who may inspect them at any time on demand.
					<br />
					<br />
					The Hirer will punctually and regularly pay or cause to be paid (and at any time on demand produce to the Owners all receipts for)
					the rents, rate and taxes of the premises whereon the Articles may be an shall keep the said Articles free and exempt from and not
					suffer them or or any of them to be taken in any distress for rent. Execution or any other legal process and shall keep the Articles
					properly insured against loss or damage by fire, theft, accident and such other risks as the Owners may require from time to time.
					<br />
					<br />
					The Hirer may (without prejudice to the Hirer's rights to determine the agreement as set out in the notice hereto), determine the
					hiring at any time by delivering the Articles to the Owners at the Owner's address, but in such cases, the Hirer shall remain liable
					for any arrears of hire rent and for any damage to the Articles.
					<br />
					<br />
					In case of any breach by the Hirer of any term hereof, or should the Hirer commit or suffer any act of bankruptcy or make or attempt
					to make any composition or arrangement with his creditors, or should the Hirer attempt to sell, assign or otherwise dispose of the
					Articles, or any of them or the benefit of this Agreement or the option of purchase herein contained or if a distress be levied or
					threatened upon the Articles or upon the premises where the Articles may be or if the Articles be seized or taken on for execution,
					or if the Hirer permit any judgement to remain unsatisfied, the Owners may (a) without prejudice to the Owner's calims for arrears
					of hire-rent or for damages for the breach of this Agreement (if any) forthwith without notice terminate the hiring and subject to
					the Hire Purchase Act Chap 82:33 retake possession of the Articles, or (b) by written notice either served personally on the Hirer 
					or sent (by post or otherwise) to the Hirer or the Hirer's last known address, forthwith and for all purposes absolutely determine
					this Agreement and the hiring hereby constituted and thereafter the Hirer shall cease to be in possession of the Articles with the
					Owner's consent and neither party shall thereafter have any rights unde this Agreement save the such determination shall not discharge
					any preexisting liability of the Hirer to the Owners.
					<br />
					<br />
					Should the hiring be terminated by the Owners under Clause 6 hereof and and the hire-purchase price exceed $15,000.00 the Owners may
					without any notice, retake possession of the Articles and for this purpose shall be entitled freely to enter into and upon any premises 
					recgnized by or under control of the Hirer.
					<br />
					<br />
					If the Hirer shall satisfy the owner that through permanent unemployment or serious illness the Hirer is unable to continue payment of
					the said monthly rent, the Owners may at their option allow the Hirer to retain articles to the value of the amount then paid after 
					making reasonable provision thereof for depreciation of the remaining articles, and cost of transport, and the Hirer may then return
					such remaining articles to the OWners in full discharge of all further liablility under this Agreeement, whereupon the articles which
					will be select for the Hirer  by the Owners (whose decision shall be final) shall become property of the Hirer.
					<br />
					<br />
					No relaxation, indulgence or waiver shown to the Hirer shall prejudice the Owner's strict right hereunder and no waiver of any breach 
					of this Agreement whatsoever shall operate as notice or waiver of any such breach thereafter commited or suffered.
					<br />
					<br />
					This Agreement is conditional upon the order being approved by the Owner's Directors, such approval to be signified by the delivery of
					the Articles. If so approved as aforsaid this Agreement shall become binding and of full effect and otherwise be void and of no effect.
					Any cash deposited on the signing hereof is accepted on this condition.
					<br />
					<br />
					If the Total Hirer Purchase price shown in the Schedule hereto exceeds $15,000.00 it is in event agreed between both parties that the
					Notice set out hereunder and the respective rights of the Owners and the Hirer set out that Notice shall not apply and this Agreement
					was signed.
					<br />
					<br />
					Where it is found necessary for the owners to repossess and/or to collect arrears of rent the cost of such repossession or collection
					will be charged to the hirer
					<br />
					<br />
					Without Prejudice to the generality of sub Clause c of Clause 1 and Clause 13 above it is hereby mutually agreed and understood between the
					owner and the hirer:- (a)  that all and any insternal administartive expenses, reasonable legal costs and other charges including
					but not limited to bailiffs fees, advertising and storage costs or other expenses connected with the locating or the hirer or the articles,
					if not at the hirer's address or any such address given by the hirer to the owner, properly incurred by the owner in recovering arrears of
					monthly rent or overdue amounts hereunder as a consequence of the default of the hirer to the owner: (b) that the hirer shall pay interest 
					and the same be chargeable to the hirer on all arrears on overdue monthly rent at the same rate as that used in the calculation of the total
					charge for credit as stated in this Agreement such interest to be calculate on a per diem basis from the due date of the hirer's monthly
					rent until payment thereof and the siad rate of interest shall be payable by the hirer both before and after judgment if any. 
					<br />
					<br />
					Whenever the hirer becomes liable to pay any of the fees, costs charges and expenses described in the preceding sub-paragraph, any monies
					received thereafter from the hirer shall first be applied in satisfaction of the aforementioned liability of the hirer to the owner and 
					only secondly towards payment of the hire purchase price. 
					<br/>
					<br />
					I/We authorize you to obtain further information on my/our credit and employment history and any such source is hereby authorized to provide
					the requested information. You are authorized to disclose to any Credit Bureau and other credit grantors any infomration about my/por credit
					history. I/we agree to jointly and severally idemnify you angainst any and all claims in damages or otherwise arising from disclosure on
					your part.
					<br />
					<br />
					IN WITNESS WHEREOF the parties hereto have set their hands the day of the year first herein above written.
				</div>
				<div class="SmallPrint" style="LEFT: 0cm; WIDTH: 30px; POSITION: absolute; TOP: 1cm; HEIGHT: 18cm; font-size: 7.5pt" align="justify">
					1.
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					2.
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					3.
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					4.
					<br />
					<br />
					<br />
					5.
					<br />
					<br />
					<br />
					<br />
					<br />
					6.
					<br />
					<br />
					<br />
					7.
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					8.
					<br />
					<br />
					<br />
					9.
					<br />
					<br />
					<br />
					<br />
					<br />
					10.
					<br />
					<br />
					<br />
					11.
					<br />
					<br />
					<br />
					<br />
					12.
					<br />
					<br />
					<br />
					13.
					<br />
					<br />
					14.1
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					14.2
					<br />
					<br />
					<br />
					15.
				</div>
			
				<div class="SmallPrint" style="LEFT: 1cm; WIDTH: 6cm; POSITION: absolute; TOP: 22.5cm; HEIGHT: 10cm" align="justify">
					For and on behalf of the Owners
					<br />
					<br />
					_____________________________________
					<br />
					<br />
					<br />
					_____________________________________
					<br />
					Signature of Guarantor
					<br />
					<br />
					_____________________________________
				</div>
				<div class="SmallPrint" style="LEFT: 7cm; WIDTH: 6cm; POSITION: absolute; TOP: 22.5cm; HEIGHT: 10cm" align="justify">
					Signature of Hirer
					<br />
					<br />
					_____________________________________
					<br />
					<br />
					<br />
					_____________________________________
					<br />
					<br />
					<br />
					_____________________________________
				</div>
				<div class="SmallPrint" style="LEFT: 13cm; WIDTH: 6cm; POSITION: absolute; TOP: 22.5cm; HEIGHT: 10cm" align="justify">
					Witness to Hirer's Signature
					<br />
					<br />
					Name _______________________________
					<br />
					<br />
					<br />
					Address _____________________________
					<br />
					<br />
					<br />
					_____________________________________
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
        <tr>
          <td width="8%"></td>
          <td width="62%">
            <xsl:value-of select="DESC2" />
          </td>
          <td width="30%"></td>
        </tr>
			</table>	
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>

	