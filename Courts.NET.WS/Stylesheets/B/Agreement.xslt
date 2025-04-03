<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<head>				
				<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
        </meta>
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
		<P>
    </P>
	</xsl:template>
	
	<xsl:template match="AGREEMENT">	
			<xsl:apply-templates select="PAGE" />	
			<P>
      </P>		
	</xsl:template>
	<xsl:template match="PAGE">
		<div style="position:relative">
			<div style="WIDTH: 5.5cm; POSITION: absolute; HEIGHT: 1.4cm">
        <IMG style="WIDTH: 100%; HEIGHT: 100%" src="{//AGREEMENTS/@IMAGEPATH}purpleLogo.jpg" />
			</div>
			<div style="LEFT: 9px; WIDTH: 205px; POSITION: absolute; TOP: 70px; HEIGHT: 23px">
        <span class="RFHead1">
				HIRE PURCHASE AGREEMENT</span>
      </div>
			<div style="LEFT: 250; WIDTH: 200px; POSITION: absolute; TOP: 0.927cm; HEIGHT: 16px" align="right">
        <xsl:value-of select="HEADER/DATE" />
      </div>	
			<div class="smallSS" style="LEFT: 619px; WIDTH: 98px; POSITION: absolute; TOP: 19px; HEIGHT: 16px" align="right">
        <b>AGREEMENT 
					NO:</b>
			</div>
			<div style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 5px; BORDER-TOP: gray thin solid; PADDING-LEFT: 5px; LEFT: 14.023cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray thin solid; WIDTH: 5cm; PADDING-TOP: 5px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 0.927cm; HEIGHT: 0.85cm; TEXT-ALIGN: center">
        <xsl:value-of select="HEADER/ACCTNO" />
      </div>
			<div class="smallSS" style="LEFT: 9px; WIDTH: 367px; POSITION: absolute; TOP: 98px; HEIGHT: 47px">
        This agreement sets out the terms on which Unicomer (Barbados) Ltd. 
        whose registered office is at St George Street, Bridgetown
        <b>("Courts")</b>
        agrees to supply the Goods listed in the Schedule below to
        <b>("You")</b>:
      </div>
			<div class="smallSS" style="LEFT: 200px; WIDTH: 159px; POSITION: absolute; TOP: 153px; HEIGHT: 31px"><b>Title, 
					First Name, Surname, Address<br/>
					("You")</b>
			</div>
			<div class="smallSS" style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 4px; BORDER-TOP: gray thin solid; PADDING-LEFT: 4px; LEFT: 10.196cm; PADDING-BOTTOM: 4px; BORDER-LEFT: gray thin solid; WIDTH: 8.834cm; PADDING-TOP: 4px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 1.88cm; HEIGHT: 3.477cm"><xsl:value-of select="HEADER/NAME" /><br/>
				<xsl:value-of select="HEADER/JOINTNAME" />
        <br/>
				<xsl:value-of select="HEADER/ADDR1" />
        <br/>
				<xsl:value-of select="HEADER/ADDR2" />
        <br/>
				<xsl:value-of select="HEADER/ADDR3" />
        <br/>
				<xsl:value-of select="HEADER/POSTCODE" />
      </div>
			<div class="smallSS" style="LEFT: 0px; WIDTH: 125px; POSITION: absolute; TOP: 204px; HEIGHT: 15px">
        <b>SCHEDULE 
					OF GOODS</b>
      </div>
			<div style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 5px; BORDER-TOP: gray thin solid; PADDING-LEFT: 1px; LEFT: 0cm; BORDER-LEFT: gray thin solid; WIDTH: 19cm; PADDING-TOP: 20px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 5.794cm; HEIGHT: 5.3cm">
        <xsl:apply-templates select="LINEITEMS" />
      </div>
			<div style="BORDER-RIGHT: gray thin solid; LEFT: 0cm; WIDTH: 1.5cm; POSITION: absolute; TOP: 5.794cm; HEIGHT: 5.3cm">
      </div>
			<div style="BORDER-RIGHT: gray thin solid; LEFT: 0cm; WIDTH: 14.8cm; POSITION: absolute; TOP: 5.794cm; HEIGHT: 5.3cm">
      </div>
			<div style="LEFT: 0cm; WIDTH: 19cm; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 5.794cm; HEIGHT: 0.6cm">
      </div>
			<div class="smallSS" style="LEFT: 17px; WIDTH: 29px; POSITION: absolute; TOP: 224px; HEIGHT: 17px">
        <b>QTY</b>
			</div>
			<div class="smallSS" style="LEFT: 59px; WIDTH: 187px; POSITION: absolute; TOP: 224px; HEIGHT: 16px" align="center">
        <b>DESCRIPTION 
					OF GOODS</b>
			</div>
			<div class="smallSS" style="LEFT: 570px; WIDTH: 45px; POSITION: absolute; TOP: 224px; HEIGHT: 16px">
        <b>PRICE</b>
			</div>
			<div style="BORDER-RIGHT: gray thin solid; BORDER-TOP: gray thin solid; LEFT: 11.078cm; BORDER-LEFT: gray thin solid; WIDTH: 7.92cm; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 14.457cm; HEIGHT: 2.556cm">
      </div>
			<DIV style="BORDER-RIGHT: gray thin solid; BORDER-TOP: gray thin; LEFT: 0px; BORDER-LEFT: gray thin solid; WIDTH: 8.6cm; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 11.079cm; HEIGHT: 2.376cm">
      </DIV>
			<DIV style="BORDER-RIGHT: gray thin solid; BORDER-TOP: gray thin; LEFT: 323px; BORDER-LEFT: gray thin; WIDTH: 6.233cm; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 11.079cm; HEIGHT: 2.373cm">
      </DIV>
			<DIV style="BORDER-RIGHT: gray thin solid; BORDER-TOP: gray thin; LEFT: 558px; BORDER-LEFT: gray thin; WIDTH: 4.237cm; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 11.079cm; HEIGHT: 2.373cm">
      </DIV>
			<div class="smallSS" style="LEFT: 491px; WIDTH: 151px; POSITION: absolute; TOP: 14.6cm; HEIGHT: 22px" align="center">
        <b>Special 
					Conditions</b>
			</div>
			<div class="smallSS" style="LEFT: 480px; WIDTH: 165px; POSITION: absolute; TOP: 15cm; HEIGHT: 22px" align="left">
				<xsl:variable name="ninety" select="../FOOTER/NINETYDAYS" />
				<xsl:if test="$ninety != ''">
					Courts will rebate to the Total Regular Price if payments of 
					<xsl:value-of select="../FOOTER/NINETYDAYS" />
					are made 30, 60 and 90 days after delivery
				</xsl:if>
				<xsl:variable name="paymentholiday" select="../FOOTER/PAYMENTHOLIDAYS" />
				<xsl:if test="$paymentholiday != ''">
					<xsl:value-of select="../FOOTER/PAYMENTHOLIDAYS" />
				</xsl:if>
			</div>
			<div class="smallSS" style="LEFT: 9px; WIDTH: 305px; POSITION: absolute; TOP: 11.2cm; HEIGHT: 73px" align="left"><b>Schedule 
					of Payments</b><br/>
				<br/>
				Deposit/Initial Payment -[<xsl:value-of select="../FOOTER/DEPOSIT" />
				]<br/>
				[<xsl:value-of select="../FOOTER/INSTALNO" />
				] Monthly payments of [<xsl:value-of select="../FOOTER/FIRSTINST" />
				]<br/>
				Final Payment of [<xsl:value-of select="../FOOTER/FINALINST" />]<br/>
				<xsl:variable name="servicePrint" select="../FOOTER/SERVICEPRINT" />
				<xsl:if test="$servicePrint = 'A'">
					Interest Rate: <xsl:value-of select="../FOOTER/INTERESTRATE" /> per annum.
				</xsl:if>
				<xsl:if test="$servicePrint = 'M'">
					Interest Rate: <xsl:value-of select="../FOOTER/INTERESTRATE" /> per month.
				</xsl:if>
				<xsl:if test="$servicePrint = 'L'">
					Interest Rate: <xsl:value-of select="../FOOTER/INTERESTRATE" /> per month.
				</xsl:if>
			</div>
			<div class="smallSS" style="LEFT: 10cm; WIDTH: 8.8cm; POSITION: absolute; TOP: 11.1cm; HEIGHT: 73px" align="left">
				<table width="100%" class="smallSS" >
					<tr>
						<td width="50%">
							<b>Goods Value</b><br/>
							<b>Deposit</b><br/>
							<b>Balance / Credit Extended</b><br/>
							<b>Charge for Credit</b><br/>
							<b>Balance Payable</b><br/>
							<b>Total Price</b><br/>
						</td>
						<td width="50%" align="right">
							<xsl:value-of select="../FOOTER/GOODSVAL" /><br/>
							<xsl:value-of select="../FOOTER/DEPOSIT" /><br/>
							<xsl:value-of select="../FOOTER/CREDIT" /><br/>
							<xsl:value-of select="../FOOTER/DT" /><br/>
							<xsl:value-of select="../FOOTER/BALANCE" /><br/>
							<xsl:value-of select="../FOOTER/TOTAL" /><br/>
						</td>
					</tr>
				</table>
			</div>
			<div class="smallSS" style="LEFT: 0px; WIDTH: 717px; POSITION: absolute; TOP: 13.7cm; HEIGHT: 23px" align="left">The 
				Deposit/Initial Payment shall be made when this Agreement is signed and the 
				first monthly payment shall be made one month after the goods are delivered by 
				Courts to You, with subsequent monthly payments made on the same day of each 
				following month.
			</div>
			<div class="smallSS" style="LEFT: 2px; WIDTH: 163px; COLOR: blue; POSITION: absolute; TOP: 14.4cm; HEIGHT: 16px" align="left"><b>TERMS 
					AND CONDITIONS</b>
			</div>
			<div class="SmallSS" style="LEFT: 0px; WIDTH: 385px; POSITION: absolute; TOP: 14.7cm; HEIGHT: 81px" align="left">In 
				these Terms and Conditions, the following words have the following meanings:
				<br/>
				<b>"Courts"</b> is the owner of the Goods listed in the Schedule above.
				<br/>
				<b>"You"</b> are the hirer of the Goods listed in the Schedule above.
				<br/>
				<b>"Agreement Price"</b> is the total (amount payable under the Agreement) for
        the Goods listed in the Schedule of Goods.
      </div>
			<div class="smallSS" style="LEFT: 49px; WIDTH: 343px; POSITION: absolute; TOP: 17.25cm; HEIGHT: 99px" align="justify">You 
				will pay to Courts the amounts set out in the Schedule of Payments above, on 
				the dates indicated.
				<br/>
				Monthly payments set out in the above Schedule of Payments may be made to any 
				Courts store on the island of Barbados or such other place advised to you in 
				writing by Courts.
				<br/>
				In addition to the payments set out in the Schedule of Payments above, you will 
				also have to pay any costs or expenses incurred by Courts in the following 
				circumstances where you have breached a term of this Agreement:
			</div>
			<DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT: 24px; WIDTH: 297px; COLOR: blue; POSITION: absolute; TOP: 16.9cm; HEIGHT: 16px" align="justify"><B>Payments 
					and Expenses</B>
			</DIV>
			<DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT: 427px; WIDTH: 293px; COLOR: blue; POSITION: absolute; TOP: 17.528cm; HEIGHT: 15px" align="justify"><B>Late 
					Payments<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
        
					Location of Goods </B>
			</DIV>
			<DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT: 24px; WIDTH: 293px; COLOR: blue; POSITION: absolute; TOP: 23.19cm; HEIGHT: 15px" align="justify">
				<P><B>Ownership</B></P>
			</DIV>
			<div class="smallSS" style="FONT-WEIGHT: bold; LEFT: 0px; WIDTH: 15px; COLOR: blue; POSITION: absolute; TOP: 16.9cm; HEIGHT: 15px" align="justify">1</div>
			<DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT: 405px; WIDTH: 15px; COLOR: blue; POSITION: absolute; TOP: 17.528cm; HEIGHT: 15px" align="justify">3<br/>
				<br/>
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
        5
      </DIV>
			<DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT: 0px; WIDTH: 15px; COLOR: blue; POSITION: absolute; TOP: 23.19cm; HEIGHT: 15px" align="justify">2</DIV>
			<div class="smallSS" style="LEFT: 49px; WIDTH: 343px; POSITION: absolute; TOP: 23.5cm; HEIGHT: 43px" align="justify">
        Ownership of the Goods shall remain with Courts until you have made all the
        payments set out in the Schedule of Payments above and paid any other sums due
        from You to Courts under the terms of this Agreement.
        <br/>
        Once
        all such payments have been made, the ownership of the Goods will pass to You.
        You should not sell, change or attempt to dispose of the Goods until ownership has
        passed to You.
        <br/>
        You may, at any time, pay to Courts the Agreement Price as set out in the above
        Schedule, along with any other payments due under the terms of this Agreement
        less any payments already made by You. If You do this, ownership of the Goods
        will pass to You.
      </div>
			<DIV class="smallSS" style="LEFT: 456px; WIDTH: 293px; POSITION: absolute; TOP: 18cm; HEIGHT: 76px" align="justify">
        If you fail to make any payment set out in the above Schedule of Payments on
        the due date, Courts will charge interest on that overdue payment at the rate
        of interest set out in the Schedule of Payments from the due date.
        <br/>
				Interest under Clause 3.1 above will be calculated on a daily basis from the 
				due date until payment is received by Courts.
				<br/>
				<br/>
        <br/>
				You must keep the Goods at your address as stated in this Agreement at all 
				times. The Goods must not be removed from that address without first informing 
				Courts in writing.
        <br/>
        <br/>
        <br/>
        You must keep the Goods in good condition and, apart from fair wear and tear and force majeure,
        you will be responsible for all loss and damage to the Goods however such is
        caused.
        <br/>
        Depending on whether you have purchased the Goods using Courts Options Bronze, 
        Silver, Gold or Ultimate, included in the monthly service charge is an amount 
        of 0.05%, 0.1%, 0.2% or 0.25% respectively of the Agreement Price to purchase 
        a Consumer Protection Insurance Policy on your behalf and in your name with 
        United Insurance Company Limited under Master Policy number AA05036205BU.
        <B>This insurance coverage is not optional.</B>
      </DIV>
			<DIV class="smallSS" style="LEFT: 23px; WIDTH: 30px; POSITION: absolute; TOP: 17.25cm; HEIGHT: 97px" align="justify">
				<P>1.1
					<br/>
					<br/>
					1.2
					<br/>
					<br/>
					<br/>
					1.3
				</P>
			</DIV>
			<DIV class="smallSS" style="LEFT: 23px; WIDTH: 30px; POSITION: absolute; TOP: 23.5cm; HEIGHT: 16px" align="justify">
				<P>2.1
					<br/>
          <br/>
          <br/>
          <br/>
          2.2
          <br/>
          <br/>
          <br/>
          2.3
				</P>
			</DIV>
			<DIV class="smallSS" style="LEFT: 430px; WIDTH: 30px; POSITION: absolute; TOP: 18cm; HEIGHT: 16px" align="justify">
				<P>
					3.1<br/>
					<br/>
					<br/>
					<br/>
					3.2<br/>
					<br/>
					<br/>
          <br/>
					4.1
          <br/>
          <br/>
          <br/>
          <br/>
          <br/>
          5.1
          <br/>
          <br/>
          <br/>
          5.2
        </P>
			</DIV>
			<DIV class="smallSS" style="LEFT: 78px; WIDTH: 310px; POSITION: absolute; TOP: 20.3cm; HEIGHT: 130px" align="justify">
				Determining the whereabouts of the Goods;
        <br/>
				Recovering possession of the Goods from You or from any other person (including 
				any payment made by Courts to discharge any claim or interest to or in the Goods by any other 
				person);or
				<br/>
				Enforcing any term of this Agreement, including legal costs incurred by Courts 
				before or after any proceedings are issued and including but not limited to 
				Courts own costs, Bailiffs fees, debt collection commission charges and any sums due under 
				Clause 3 below.
			</DIV>
			<DIV class="smallSS" style="LEFT: 48px; WIDTH: 30px; POSITION: absolute; TOP: 20.3cm; HEIGHT: 130px" align="justify">
				<P>1.3.1
					<br/>
					1.3.2<br/>
					<br/>
					<br/>
					1.3.3
				</P>
			</DIV>
      <DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT: 427px; WIDTH: 293px; COLOR: blue; POSITION: absolute; TOP: 21.95cm; HEIGHT: 15px" align="justify">
        <B>Insurance and Care of Goods</B>
      </DIV>
      
      
      
			<br class="pageBreak"/>
			<div style="POSITION: relative">
				
				<DIV class="smallSS" style="LEFT: 160px; WIDTH: 356px; COLOR: black; POSITION: absolute; TOP: 14.5cm; HEIGHT: 19px" align="left"><B>NOTICE 
						(PURSUANT TO THE HIRE PURCHASE ACT CAP.(328)</B></DIV>
				<DIV class="smallSS" style="LEFT: 23px; WIDTH: 30px; POSITION: absolute; TOP: 0.17cm; HEIGHT: 97px" align="justify">
					5.3
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
					<br/>
					<br/>
					<br/>
					5.4
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
          5.5
          <br/>
          <br/>
          <br/>
          <br/>
          <br/>
          <br/>
          <br/>
          <br/>
          <br/>
          5.6
          <br/>
          <br/>
          <br/>
          <br/>
          <br/>
          <br/>
          <br/>
          6.1
          <br/>
          <br/>
          <br/>
          6.2
				</DIV>
				<div class="smallSS" style="LEFT: 49px; WIDTH: 325px; POSITION: absolute; TOP: 0.17cm; HEIGHT: 43px" align="justify">
					The Insurance Policy purchased under Clause 5.2 above is designed to provide 
					you with insurance in the event of your death. In the case of Courts Options 
					Silver, Gold and Ultimate the Insurance Policy is also designed to provide you 
					with insurance in the event of your total disablement following an accident or 
					sickness or the Goods being destroyed or lost as a result of fire, flood or 
					hurricane. Additionally, in the case of Courts Options Gold and Ultimate, the 
					Insurance Policy is designed to provide you with insurance in the event that 
					the Goods are lost as a result of the burglary of your home. In the case of 
					Courts Option Ultimate, the Insurance Policy is designed to provide you with 
					25% of the original cash price back in Courts Cash if your product is damaged 
					by fire, flooding or storm in the first 24 months.
					<br/>
					In any of the circumstances set out at Clause 5.3 above, all outstanding 
					payments due under this Agreement (apart from any arrears or overdue payments) 
					will be met by Insurers on your behalf and paid direct to Courts. That payment 
					will be credited to your account with Courts and, subject to Clause 5.6 below, 
					will release you or your estate from your obligations to make further payments 
					under this Agreement.
					<br/>
          In addition to the circumstances set out in Clause 5.3 above, if you have
          purchased the Goods using Courts Options Gold or Ultimate, one monthly payment for each month
          of redundancy up to a maximum of 12 of the
          monthly payments due under this Agreement will be met by Insurers and paid to
          Courts on your behalf if you are made redundant, and one monthly payment for each
          month of hospitalisation up to a maximum of 3 of the monthly
          payments due under this Agreement will be met by Insurers and paid to Courts on
          your behalf if you are hospitalised as a result of an accident.
          <br/>
          The Insurance Policy taken out under Clause 5.2 above will not pay out in
          respect of any arrears or overdue payments outstanding at the date of loss of
          the goods, or your death, and you will remain liable to pay all such
          outstanding sums. Under the terms of the Insurance Policy (a copy is available
          on request) all sums are paid directly to Courts and not to you.
          <br/>
          <br/>
          Courts may terminate this Agreement if you breach any of its terms. In
          addition, please see the Notice at the end of these Terms and Conditions which
          also deals with termination.
          <br/>
          On termination of this Agreement by either party for any reason, you agree that
          you will immediately make the Goods available to Courts for repossession.
        </div>
				<DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT: 24px; WIDTH: 293px; COLOR: blue; POSITION: absolute; TOP: 12.1cm; HEIGHT: 10px" align="justify">
					<B>Termination and Repossession</B>
				</DIV>
        <DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT: 11cm; WIDTH: 293px; COLOR: blue; POSITION: absolute; TOP: 1.52cm; HEIGHT: 10px" align="justify">
          <B>Binding Agreement</B>
        </DIV>
        <DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT: 11cm; WIDTH: 293px; COLOR: blue; POSITION: absolute; TOP: 6.63cm; HEIGHT: 10px" align="justify">
          <B>$12,000 Limit</B>
        </DIV>
        <DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT: 11cm; WIDTH: 293px; COLOR: blue; POSITION: absolute; TOP: 8.67cm; HEIGHT: 10px" align="justify">
          <B>Rebates</B>
        </DIV>
        <DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT: 11cm; WIDTH: 293px; COLOR: blue; POSITION: absolute; TOP: 9.7cm; HEIGHT: 10px" align="justify">
          <B>Third Party Disclosure</B>
        </DIV>
				<DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT:0cm; WIDTH: 15px; COLOR: blue; POSITION: absolute; TOP: 12.1cm; HEIGHT: 10px" align="justify">
					6
				</DIV>
        <DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT:10.5cm; WIDTH: 15px; COLOR: blue; POSITION: absolute; TOP: 1.52cm; HEIGHT: 10px" align="justify">
          7
        </DIV>
        <DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT:10.5cm; WIDTH: 15px; COLOR: blue; POSITION: absolute; TOP: 6.63cm; HEIGHT: 15px" align="justify">
          8
        </DIV>
        <DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT:10.5cm; WIDTH: 15px; COLOR: blue; POSITION: absolute; TOP: 8.67cm; HEIGHT: 15px" align="justify">
          9
        </DIV>
        <DIV class="smallSS" style="FONT-WEIGHT: bold; LEFT:10.5cm; WIDTH: 15px; COLOR: blue; POSITION: absolute; TOP: 9.7cm; HEIGHT: 15px" align="justify">
          10
        </DIV>
				<DIV class="smallSS" style="LEFT: 11cm; WIDTH: 30px; POSITION: absolute; TOP: 0.17cm; HEIGHT: 97px" align="justify">
					6.3
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					7.1
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					7.2
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					<br/>
					8.1
					<br/>
					<br/>
					8.2
          <br/>
          <br/>
          <br/>
          <br/>
          9.1
          <br/>
          <br/>
          <br/>
          10.1
        </DIV>
				<div class="smallSS" style="LEFT: 440px; WIDTH: 320px; POSITION: absolute; TOP: 0.17cm; HEIGHT: 43px" align="justify">
					If you breach any term of this Agreement and Courts choose not to terminate the 
					Agreement under Clause 6.1, this will not affect Courts’ right to terminate the 
					Agreement in the future for that or any subsequent breach by you.
					<br/>
					<br/>
          This Agreement is conditional on you satisfying Courts' credit sanction procedure
          and does not become binding on Courts until such time as you shall have satisfied
          this condition. If you fail to satisfy this condition you will be notified in writing
          by Courts at which time this Agreement will be void and shall be of no further effect 
          and any initial payment or deposit which you have made will be returned to you by 
          Courts. Courts will only deliver the goods to your address as stated above when you
          have satisfied the credit sanction procedure.
          <br/>
          If, within a period of 7 days of signing this Agreement, You have not received delivery
          of the Goods due to unforeseen stock unavailability, the Agreement will automatically be
          cancelled and You will be entitled to a refund of any payments made to Courts. You will
          have no obligations to Courts nor Courts to you.
          <br/>
					<br/>
					Please note that the Notice below only applies if the Agreement Price shown in 
					the Schedule of Payments does not exceed $12,000.
					<br/>
					If the Agreement Price shown in the Schedule of Payments exceeds $12,000, the 
					Notice in the remainder of this Clause 8 will have no effect.
          <br/>
          <br/>
          Rebates of service charge are calculated using an adjusted rule of 78.
          <br/>
          <br/>
          I/We warrant and confirm the information given herein is true and correct and 
          I/We understand it is being used to determine My/Our credit responsibility. 
          I/We further confirm that no information, which might affect Unicomer’s decision
          to make the loan, has been withheld. I/We hereby authorize and consent to Unicomer 
          receiving and exchanging any financial and other information which it may have in 
          its possession about Me/Us with third party credit institutions and marketing agencies
          with whom I/We may have or propose to have financial dealings from time to time. I/We 
          indemnify Unicomer against any loss, claims, damages, liabilities, actions and proceedings,
          legal and or other expense which may be directly and reasonably incurred as a consequence 
          of the disclosure of the financial or other information
        </div>
				<div class="smallSS" style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 10px; BORDER-TOP: gray thin solid; PADDING-LEFT: 10px; RIGHT: 18px; PADDING-BOTTOM: 10px; BORDER-LEFT: gray thin solid; WIDTH: 750px; PADDING-TOP: 1px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 22.281cm; HEIGHT: 0.92cm" align="justify">
					<span style="FONT-WEIGHT: bold; COLOR: blue" class="smallSS">SIGNATURE OF THE 
						PARTIES</span><br/>
					I have read and understand the contents of this Agreement and agree to be bound 
					by this Agreement and its terms:
					<br/>
          <br/>
					Your Signature:
					<br/>
					<br/>
          Signed for and on behalf of Unicomer (Barbados) Ltd. "Courts":
          <br/>
					<br/>
					Witness to Signatures:
					<br/>
					<br/>
					Name:
					<br/>
					<br/>
					Address:
				</div>
				<DIV class="smallSS" style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 10px; BORDER-TOP: gray thin solid; PADDING-LEFT: 10px; RIGHT: 18px; PADDING-BOTTOM: 10px; BORDER-LEFT: gray thin solid; WIDTH: 750px; PADDING-TOP: 10px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 14.47cm; HEIGHT: 7.88cm" align="justify">
					<span style="FONT-WEIGHT: bold; COLOR: blue" class="smallSS">RIGHT OF THE CUSTOMER 
						TO TERMINATE THE AGREEMENT</span><br/>
					1.<br/>
					<br/>
					2.<br/>
					<br/>
					3.<br/>
					<br/>
					<br/>
					4.<br/>
					<br/>
					5.<br/>
					<br/>
					<span style="FONT-WEIGHT: bold; COLOR: blue" class="smallSS">COURTS' RIGHTS TO 
						RECOVER GOODS</span>
					<br/>
					1.<br/>
					<br/>
					2.<br/>
					<br/>
					<br/>
					<br/>
					3.
				</DIV>
				<div style="BORDER-TOP: silver 1px solid; LEFT: 2.75cm; WIDTH: 16.651cm; POSITION: absolute; TOP: 23.63cm; HEIGHT: 0.5cm">
				</div>
				<div style="BORDER-TOP: silver 1px solid; LEFT: 8.75cm; WIDTH: 10.650cm; POSITION: absolute; TOP: 24.38cm; HEIGHT: 0.5cm">
				</div>
				<div style="BORDER-TOP: silver 1px solid; LEFT: 3.62cm; WIDTH: 15.736cm; POSITION: absolute; TOP: 25.05cm; HEIGHT: 0.5cm">
				</div>
				<div style="BORDER-TOP: silver 1px solid; LEFT: 1.634cm; WIDTH: 17.750cm; POSITION: absolute; TOP: 25.7cm; HEIGHT: 0.5cm">
				</div>
				<div style="BORDER-TOP: silver 1px solid; LEFT: 2.097cm; WIDTH: 17.334cm; POSITION: absolute; TOP: 26.4cm; HEIGHT: 0.5cm">
				</div>
				<DIV class="smallSS" style="BORDER-RIGHT: gray thin; PADDING-RIGHT: 0px; BORDER-TOP: gray thin; PADDING-LEFT: 0px; LEFT: 49px; PADDING-BOTTOM: 0px; BORDER-LEFT: gray thin; WIDTH: 700px; PADDING-TOP: 0px; BORDER-BOTTOM: gray thin; POSITION: absolute; TOP: 15.2cm; HEIGHT: 6.04cm" align="justify">You 
					can terminate this Agreement at any time by telling Courts in writing that you 
					wish to do so. If you choose to do this, you must deliver the Goods to Courts 
					either at the same time or before your written Notice is given to Courts.<br/>
					At the same time as giving Courts Notice in writing, You must also pay any 
					overdue instalments or other payments due under this Agreement at that date.<br/>
					If: * You fail to deliver the Goods to Courts; and/or * You fail to pay any 
					overdue instalments or other payments due under this Agreement; at the same 
					time, or before delivering your Notice of termination to Courts, your Notice of 
					termination will be ineffective and this Agreement and all its terms will 
					remain in force.
					<br/>
					If when you return the Goods they have been damaged, Courts has the right to 
					seek compensation, including but not limited to court action, from you for the 
					amount of the damage unless you can agree that amount with Courts.
					<br/>
					If this Agreement contains terms which allow You to terminate the Agreement on 
					more favourable terms than those set out at paragraphs 1-4 above, You are 
					entitled to terminate the Agreement on those more favourable terms.<br/>
					<br/>
					Once you have paid $[__________], unless You terminate the Agreement yourself, then 
					Courts cannot take back the Goods from you without an Order of the Court unless 
					You agree to let them do so.
					<br/>
					If you have paid less than $[__________] then Courts can take the Goods back from You 
					without your consent once Courts has given you 21 days written notice of its 
					intention to do so, unless: * You have put an end to the Agreement; or * You 
					have committed a breach of the Agreement other than a failure to make a payment 
					when due as set out in the Schedule of Payments above. (If either of the above 
					conditions exist, Courts may repossess the goods without your consent and 
					without giving 21 days written notice.)
					<br/>
					If, within the period of 21 days notice that Courts must give you of their 
					intention to repossess the goods, You make all payments that are due under the 
					Schedule of Payments, the Agreement will continue in force and Courts will not 
					be entitled to repossess the goods without your consent.</DIV>
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
              <div>
                <xsl:value-of select="DESC" />
              </div>
              <xsl:choose>
                <xsl:when test="DESC2 != ''">
                    <xsl:value-of select="DESC2" />
                </xsl:when>
              </xsl:choose>
					</td>
					<td align="right" width="30%">
						<xsl:value-of select="VALUE" />
					</td>
				</tr>
			</table>	
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>

  