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
			<div style="LEFT: -6px; WIDTH: 150px; POSITION: absolute; TOP: 53px; HEIGHT: 30px"><IMG style="WIDTH: 100%; HEIGHT: 49px" height="49" src="smallLogo.jpg" width="150" />
			</div>
			<div class="smallSS" style="LEFT: 148px; WIDTH: 172px; POSITION: absolute; TOP: 67px; HEIGHT: 25px"><i>(St Kitts 
					&amp; Nevis) Ltd.</i>
			</div>
			<div class="smallSS" style="LEFT: 621px; WIDTH: 100px; POSITION: absolute; TOP: 74px; HEIGHT: 21px" align="center">Original
			</div>
			<div style="LEFT: 1px; WIDTH: 300px; POSITION: absolute; TOP: 95px; HEIGHT: 30px"><span class="HPHeader">Hire-Purchase 
					Agreement</span>
			</div>
			<div class="smallSS" style="LEFT: 290px; WIDTH: 100px; POSITION: absolute; TOP: 93px; HEIGHT: 30px">Agreement 
				No. (Insert No.)
			</div>
			<div style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 5px; BORDER-TOP: gray thin solid; PADDING-LEFT: 5px; LEFT: 10.5cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray thin solid; WIDTH: 5cm; PADDING-TOP: 5px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 2.5cm; HEIGHT: 0.85cm; TEXT-ALIGN: center">
				<xsl:value-of select="HEADER/ACCTNO" />
			</div>
			<div style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 5px; BORDER-TOP: gray thin solid; PADDING-LEFT: 5px; LEFT: 17.3cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray thin solid; WIDTH: 0.85cm; PADDING-TOP: 5px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 2.5cm; HEIGHT: 0.85cm; TEXT-ALIGN: center"></div>
			<div class="smallSS" style="LEFT: 1px; WIDTH: 718px; POSITION: absolute; TOP: 138px; HEIGHT: 31px">
				This agreement sets out the terms on which <b>Courts (St Kitts &amp; Nevis) 
					Limited</b> whose registered office is at College Street, <b>("Courts")</b>
				agrees to supply the Goods listed in the Schedule below to:
			</div>
			<div class="smallSS" style="LEFT: 70px; WIDTH: 159px; POSITION: absolute; TOP: 185px; HEIGHT: 31px">
				<b>Title, First Name, Surname, Address<br />
					("You")</b>
			</div>
			<div class="smallSS" style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 4px; BORDER-TOP: gray thin solid; PADDING-LEFT: 4px; LEFT: 6.5cm; PADDING-BOTTOM: 4px; BORDER-LEFT: gray thin solid; WIDTH: 12.5cm; PADDING-TOP: 4px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 4.5cm; HEIGHT: 2.3cm">
				<xsl:value-of select="HEADER/NAME" /><br />
				<xsl:value-of select="HEADER/ADDR1" /><br />
				<xsl:value-of select="HEADER/ADDR2" /><br />
				<xsl:value-of select="HEADER/ADDR3" /><br />
				<xsl:value-of select="HEADER/POSTCODE" />
			</div>
			<div class="smallSS" style="LEFT: 0px; WIDTH: 108px; POSITION: absolute; TOP: 249px; HEIGHT: 17px">
				<b>Schedule of Goods</b>
			</div>
			<div style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 5px; BORDER-TOP: gray thin solid; PADDING-LEFT: 1px; LEFT: 0cm; BORDER-LEFT: gray thin solid; WIDTH: 19cm; PADDING-TOP: 20px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 7cm; HEIGHT: 8.5cm">
				<xsl:apply-templates select="LINEITEMS" />
			</div>
			<div style="BORDER-RIGHT: gray thin solid; LEFT: 0cm; WIDTH: 1.5cm; POSITION: absolute; TOP: 7cm; HEIGHT: 8.5cm">
			</div>
			<div style="BORDER-RIGHT: gray thin solid; LEFT: 0cm; WIDTH: 14.8cm; POSITION: absolute; TOP: 7cm; HEIGHT: 8.5cm">
			</div>
			<div style="LEFT: 0cm; WIDTH: 19cm; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 7cm; HEIGHT: 0.6cm">
			</div>
			<div style="LEFT: 0cm; WIDTH: 19cm; BORDER-TOP: gray thin solid; POSITION: absolute; PADDING-TOP: 5px; PADDING-RIGHT: 5px; TOP: 14.25cm; HEIGHT: 1.35cm">
				<table width="100%" ID="Table1">
					<tr>
						<td width="80%"></td>
						<td align="right" width="20%">
							<xsl:value-of select="FOOTER/GOODSVAL" />
						</td>
					</tr>
				</table>
			</div>
			<div class="smallSS" style="LEFT: 17px; WIDTH: 29px; POSITION: absolute; TOP: 270px; HEIGHT: 17px">
				<b>QTY</b>
			</div>
			<div class="smallSS" style="LEFT: 197px; WIDTH: 187px; POSITION: absolute; TOP: 270px; HEIGHT: 16px" align="center">
				<b>DESCRIPTION OF GOODS</b>
			</div>
			<div class="smallSS" style="LEFT: 618px; WIDTH: 45px; POSITION: absolute; TOP: 270px; HEIGHT: 16px">
				<b>PRICE</b>
			</div>
			<div class="smallSS" style="LEFT: 360px; WIDTH: 180px; POSITION: absolute; TOP: 550px; HEIGHT: 20px" align="right">
				Total Price of Goods Supplied
			</div>
			<div style="BORDER-RIGHT: gray thin solid; BORDER-TOP: gray thin solid; LEFT: 10.4cm; BORDER-LEFT: gray thin solid; WIDTH: 8.6cm; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 15.9cm; HEIGHT: 2.5cm">
			</div>
			<div class="smallSS" style="LEFT: 487px; WIDTH: 132px; POSITION: absolute; TOP: 604px; HEIGHT: 20px" align="center">
				<b>Special Conditions</b>
			</div>
			<div class="smallSS" style="LEFT: 0px; WIDTH: 400px; POSITION: absolute; TOP: 607px; HEIGHT: 73px" align="left">
				<b>Schedule of Payments</b><br />
				<br />
				Deposit/Initial Payment -[<xsl:value-of select="FOOTER/DEPOSIT" />
				]<br />
				[<xsl:value-of select="FOOTER/INSTALNO" />
				] Monthly payments of [<xsl:value-of select="FOOTER/FIRSTINST" />
				]<br />
				Final Payment of [<xsl:value-of select="FOOTER/FINALINST" />]
			</div>
			<div class="smallSS" style="LEFT: 0px; WIDTH: 717px; POSITION: absolute; TOP: 700px; HEIGHT: 31px" align="left">
				The Deposit/Initial Payment shall be made when this Agreement is signed and the 
				first monthly payment shall be made one month after the Goods are delivered by 
				Courts to You, with subsequent monthly payments made on the same day of each 
				following month.
			</div>
			<div style="BORDER-RIGHT: gray thin solid; BORDER-TOP: gray thin solid; LEFT: 0cm; BORDER-LEFT: gray thin solid; WIDTH: 19cm; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 19.5cm; HEIGHT: 0.5cm">
			</div>
			<div class="RFHead1" style="LEFT: 253px; WIDTH: 192px; POSITION: absolute; TOP: 741px; HEIGHT: 19px" align="center" id="DIV1" language="javascript" onclick="return DIV1_onclick()">
				<b>TERMS AND CONDITIONS</b>
			</div>
			<div class="SmallPrint" style="LEFT: 0px; WIDTH: 329px; POSITION: absolute; TOP: 768px; HEIGHT: 99px" align="left" id="Div2" language="javascript" onclick="return DIV1_onclick()">
				In these Terms and Conditions, the following words have the following meanings:
				<br />
				<br />
				<b>"Courts"</b> is the owner of the Goods listed in the Schedule above.
				<br />
				<br />
				<b>"You"</b> are the hirer of the Goods listd in the Schedule above.
				<br />
				<br />
				<b>"Agreement Price"</b> is the total hire purchase price/amount payable for 
				the Goods listed in the Schedule of Goods.
			</div>
			<div class="SmallPrint" style="LEFT: 36px; WIDTH: 293px; POSITION: absolute; TOP: 878px; HEIGHT: 99px" align="justify" id="Div3" language="javascript" onclick="return DIV1_onclick()">
				<b>Payments and Expenses</b>
				<br />
				<br />
				You will pay to Courts the amounts set out in the Schedule of Payments above, 
				on the dates indicated.
				<br />
				<br />
				Monthly payments set out in the above Schedule of Payments may be made to any 
				Courts store on the island of Antigua and Barbuda.
				<br />
				<br />
				In addition to the payments set out in the Schedule of Payments above, you will 
				also have to pay any costs or expenses incurred by Courts in the following 
				circumstances where you have breached a term of this Agreement:
				<br />
				<br />
				Determining your whereabouts or the whereabouts of the Goods;
			</div>
			<div class="SmallPrint" style="LEFT: 0px; WIDTH: 30px; POSITION: absolute; TOP: 878px; HEIGHT: 152px" align="justify" id="Div4" language="javascript" onclick="return DIV1_onclick()">
				1
				<br />
				<br />
				1.1
				<br />
				<br />
				<br />
				1.2
				<br />
				<br />
				<br />
				1.3
				<br />
				<br />
				<br />
				<br />
				<br />
				1.3.1
			</div>
			<div class="SmallPrint" style="LEFT: 390px; WIDTH: 327px; POSITION: absolute; TOP: 768px; HEIGHT: 261px" align="justify" id="Div5" language="javascript">
				Recovering possession of the Goods from You or from any other person (including 
				any payment made by Courts to discharge claim to the Goods by any other 
				person);or
				<br />
				<br />
				Enforcing any term of this Agreement, including legal costs incurred by Courts 
				before or after any proceedings are issued and including but not limited to 
				Courts own costs, Bailiffs fees, debt collection charges and any sums due under 
				Clause 3 below.
				<br />
				<br />
				<b>Ownership</b>
				<br />
				<br />
				Ownership of the Goods shall remain with Courts until you have made all the 
				payments set out in the Schedule of Payments above and paid any other sums due 
				from You to Courts under the terms of this Agreement. Once all such payments 
				have been made, the ownership of the Goods will pass to You. You should not 
				sell or attempt to dispose of the Goods until ownership has passed to You.
				<br />
				<br />
				You may, at any time, pay to Courts the Agreement Price as set out in the above 
				Schedule, along with any other payments due under the terms of this Agreement 
				less any payments already made by You. If You do this, ownership of the Goods 
				will pass to You.
			</div>
			<div class="SmallPrint" style="LEFT: 357px; WIDTH: 30px; POSITION: absolute; TOP: 769px; HEIGHT: 264px" align="justify" id="Div6" language="javascript" onclick="return DIV1_onclick()">
				1.3.2
				<br />
				<br />
				<br />
				<br />
				1.3.3
				<br />
				<br />
				<br />
				<br />
				<br />
				2
				<br />
				<br />
				2.1
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				2.2
			</div>
			<br class="pageBreak" />
			<div style="position:relative">
				<div class="SmallPrint" style="LEFT: 1cm; WIDTH: 8.2cm; POSITION: absolute; TOP: 1.5cm; HEIGHT: 18cm" align="justify">
					<b>Late Payments</b>
					<br />
					<br />
					If you fail to make any payment set out in the above Schedule of Payments on 
					the due date, Courts will charge interest on that overdue payment at the rate 
					of interest set out in the Schedule of Payments from the due date.
					<br />
					<br />
					Interest under Clause 3.1 above will be calculated on a daily basis from the 
					due date until payment is received by Courts.
					<br />
					<br />
					<b>Location of Goods</b>
					<br />
					<br />
					You must keep the Goods at your address as stated in this Agreement at all 
					times. Notice must be given to Courts as to your new address if you relocate.
					<br />
					<br />
					<b>Insurance and Care of Goods</b>
					<br />
					<br />
					You must keep the Goods in good condition and, apart from fair wear and tear, 
					you will be responsible for all loss and damage to the Goods however it is 
					caused.
					<br />
					<br />
					Depending on whether you have purchased the Goods using Easy Pay Silver or 
					Courts Options Gold included in the monthly service charge is an amount of 
					either 0.1% or 0.2% of the Agreement Price to purchase a Consumer Protection 
					Insurance policy on your behalf and in your name with Caribbean Alliance 
					Insurance Company under Master Policy number 15CC887374.
					<br />
					<br />
					The Insurance Policy purchased under Clause 5.2 above is designed to provide 
					You with insurance in the event of the Goods being destroyed or lost by fire, 
					flood, hurricane or robbery or in the event of your death or total disablement 
					following an accident or sickness.
					<br />
					<br />
					In any of the circumstances set out in Clause 5.3 above, all outstanding 
					payments due under this Agreement (apart from any arrears or overdue payments) 
					will be met by the insurers on your behalf and paid direct to Courts. That 
					payment will be credited to your account with Courts and, subject to Clause 5.6 
					below, will release you and/or your estate from your obligations to make 
					further payments under this Agreement.
					<br />
					<br />
					In addition to the circumstances set out in Clause 5.3 above, if you purchase 
					the Goods using Courts Options Gold, up to six of the monthly payments due 
					under this Agreement will be met by Insurers and paid to Courts on your behalf 
					if you are made redundant and up to three of the monthly payments due under 
					this Agreement will be met by Insurers and paid to Courts on your behalf if you 
					are hospitalised as a result of an accident.
					<br />
					<br />
					The Insurance Policy taken out under Clause 5.2 above will pay out in respect 
					of any arrears or overdue payments outstanding at the date of loss of the 
					Goods, your death or total disablement or (if applicable) redundancy or 
					hospitalisation, and you will remain liable to pay all such outstanding sums. 
					Under the terms of the Insurance Policy (a copy of which is available on 
					request) all sums will be paid directly to Courts on your behalf.
					<br />
					<br />
					<b>Termination and Repossession</b>
					<br />
					<br />
					Courts may terminate this Agreement if you breach any of its terms. In 
					addition, please see the Notice at the end of these Terms and Conditions which 
					also deals with termination.
					<br />
					<br />
					On termination of the Agreement by either party for any reason, you agree that 
					you will immediately make the Goods available to Courts for repossession.
					<br />
					<br />
					If you breach the terms of this Agreement and Courts choose not to terminate 
					the Agreement under Clause 6.1, this will not affect Courts' right to terminate 
					the Agreement in the future for that or any subsequent breach by you.
				</div>
				<div class="SmallPrint" style="LEFT: 10.8cm; WIDTH: 8.2cm; POSITION: absolute; TOP: 1.5cm; HEIGHT: 18cm" align="justify">
					Courts may terminate this Agreement if you have given or give us false or 
					incomplete information when applying to enter into this Agreement, uncluding, 
					but not limitied to, not advising us of any changes to your details as 
					specified on any Ready Finance statement issued by Courts to you.
					<br />
					<br />
					<b>Binding Agreement</b>
					<br />
					<br />
					This Agreement does not become binding on Courts until the Goods are delivered 
					to your address, as stated above, by Courts. Until delivery of the Goods takes 
					place, Courts may cancel this Agreement by notifying you that they no longer 
					wish to supply the goods. If that happens, this Agreement shall be void and 
					shall have no effect on either party. Any initial payment or deposit already 
					made by You will be returned to You by Courts.
					<br />
					<br />
					If, within a period of 60 days of signing this Agreement, You have not received 
					the delivery of the Goods, the Agreement will automatically be cancelled and 
					You will be entitled to a refund of any payments made to Courts.
					<br />
					<br />
					Agreement Price is
					<xsl:value-of select="FOOTER/TOTAL" />
					<br />
					<br />
					Interest Rate: Option Gold 1.35% per month, Easy Pay Silver, Take Ten, Take 
					Three 1.15% per month.
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<b>You can terminate this Agreement at any time by telling Courts in writing that 
						you wish to do so. If you choose to do this, you must deliver the Goods to 
						Courts either at the same time or before your written Notice is given to 
						Courts.</b>
					<br />
					<br />
					<b>At the same time as giving Courts Notice in writing, You must also pay any 
						overdue installments or other payments due under this Agreement at that date.</b>
					<br />
					<br />
					Unless termination is affected by you as stated herein, your notice of 
					termination will be ineffective and this Agreement and all its terms will 
					remain in force.
					<br />
					<br />
					If when you return the Goods they have been damaged, Courts have the right to 
					seek compensation, including by way of Legal action, from you for the amount of 
					the damage unless you can agree that amount with Courts.
					<br />
					<br />
					If this Agreement contains terms which allow You to determine the Agreement on 
					more favourable terms than those set out at paragraphs 1-4 above, You are 
					entitled to terminate the Agreement on those more favourable terms.
					<br />
					<br />
					<br />
					<br />
					If you have paid less than 100% of the Agreement Price then Courts can take the 
					Goods back from You without your consent once Courts has given you 21 days 
					written notice of its intention to do so, unless:
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					If, within the period of 21 days notice that Courts must give you of their 
					intention to repossess the Goods, You make all payments that are due under the 
					Schedule of Payments, the Agreement will continue in force (and Courts will not be entitled to reposess the Goods without your consent).
				</div>
				<div class="SmallPrint" style="LEFT: 10.7cm; WIDTH: 7.6cm; POSITION: absolute; TOP: 15.8cm; HEIGHT: 5cm" align="justify">
					<ul style="LIST-STYLE-POSITION: outside; LIST-STYLE-TYPE: disc">
						<li>
							You have put an end to the Agreement; or<br />
							<br />
						</li>
						<li>
							You have committed a breach of the Agreement other than a failure to make a 
							payment when due as set out in the Schedule of Payments above.</li>
					</ul>
				</div>
				<div class="SmallPrint" style="LEFT: 0cm; WIDTH: 30px; POSITION: absolute; TOP: 1.5cm; HEIGHT: 18cm" align="justify">
					3
					<br />
					<br />
					3.1
					<br />
					<br />
					<br />
					<br />
					<br />
					3.2
					<br />
					<br />
					<br />
					4
					<br />
					<br />
					4.1
					<br />
					<br />
					<br />
					<br />
					5
					<br />
					<br />
					5.1
					<br />
					<br />
					<br />
					<br />
					5.2
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					5.3
					<br />
					<br />
					<br />
					<br />
					<br />
					5.4
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					5.5
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					5.6
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					6
					<br />
					<br />
					6.1
					<br />
					<br />
					<br />
					<br />
					6.2
					<br />
					<br />
					<br />
					<br />
					6.3
				</div>
				<div class="SmallPrint" style="LEFT: 9.9cm; WIDTH: 9cm; POSITION: absolute; TOP: 7.95cm; HEIGHT: 18cm" align="justify">
					<b>NOTICE (PURSUANT TO THE HIRE PURCHASE ACT CAP.(201)</b>
					<br />
					<br />
					<b>RIGHT OF THE CUSTOMER TO TERMINATE AGREEMENT</b>
					<br />
					<br />
					<b>1.</b>
					<br />
					<br />
					<br />
					<br />
					<br />
					<b>2.</b>
					<br />
					<br />
					<br />
					<br />
					3.
					<br />
					<br />
					<br />
					<br />
					4.
					<br />
					<br />
					<br />
					<br />
					<br />
					5.
					<br />
					<br />
					<br />
					<br />
					<br />
					<b>COURTS' RIGHTS TO RECOVER GOODS</b>
					<br />
					<br />
					1.
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
					2.
				</div>
				<div class="SmallPrint" style="LEFT: 9.9cm; WIDTH: 30px; POSITION: absolute; TOP: 1.5cm; HEIGHT: 18cm" align="justify">
					6.4
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					7
					<br />
					<br />
					7.1
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					<br />
					7.2
					<br />
					<br />
					<br />
					<br />
					<br />
					7.3
					<br />
					<br />
					7.4
				</div>
				<div class="SmallPrint" style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 10px; BORDER-TOP: gray thin solid; PADDING-LEFT: 10px; LEFT: 0cm; PADDING-BOTTOM: 10px; BORDER-LEFT: gray thin solid; WIDTH: 9.15cm; PADDING-TOP: 10px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 19.3cm; HEIGHT: 6.65cm" align="justify">
					I have read and understand the contents of this Agreement and agree to be bound 
					by this Agreement and its terms:
					<br />
					<br />
					<br />
					<br />
					Signature of Customer:
					<br />
					<br />
					<br />
					Signed for and on behalf of Courts:
					<br />
					<br />
					<br />
					Witness to Signatures:
					<br />
					<br />
					<br />
					Name:
					<br />
					<br />
					<br />
					Address:
				</div>
				<!-- add the dotted line divs to the customer signature box -->
				<div style="BORDER-TOP: silver 1px solid; LEFT: 3cm; WIDTH: 5.8cm; POSITION: absolute; TOP: 21cm; HEIGHT: 0.5cm">
				</div>
				<div style="BORDER-TOP: silver 1px solid; LEFT: 4.3cm; WIDTH: 4.5cm; POSITION: absolute; TOP: 21.7cm; HEIGHT: 0.5cm">
				</div>
				<div style="BORDER-TOP: silver 1px solid; LEFT: 3cm; WIDTH: 5.8cm; POSITION: absolute; TOP: 22.4cm; HEIGHT: 0.5cm">
				</div>
				<div style="BORDER-TOP: silver 1px solid; LEFT: 1.1cm; WIDTH: 7.7cm; POSITION: absolute; TOP: 23.1cm; HEIGHT: 0.5cm">
				</div>
				<div style="BORDER-TOP: silver 1px solid; LEFT: 1.3cm; WIDTH: 7.5cm; POSITION: absolute; TOP: 23.8cm; HEIGHT: 0.5cm">
				</div>
				<div style="BORDER-TOP: silver 1px solid; LEFT: 0.3cm; WIDTH: 8.5cm; POSITION: absolute; TOP: 24.5cm; HEIGHT: 0.5cm">
				</div>
				<div style="BORDER-TOP: silver 1px solid; LEFT: 0.3cm; WIDTH: 8.5cm; POSITION: absolute; TOP: 25.2cm; HEIGHT: 0.5cm">
				</div>
				<div class="SmallPrint" style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 10px; BORDER-TOP: gray thin solid; PADDING-LEFT: 10px; LEFT: 9.8cm; PADDING-BOTTOM: 10px; BORDER-LEFT: gray thin solid; WIDTH: 9.15cm; PADDING-TOP: 10px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 19.3cm; HEIGHT: 6.65cm" align="justify">
					<b>GUARANTEE</b> (Where required by Courts)
					<br />
					<br />
					In return for Courts entering into this Agreement with You at my request, I 
					agree to guarantee the performance of all your obligations under this Agreement 
					and agree to be jointly and severally liable to Courts under this Agreement, 
					which I have read and understood, if You fail to perform your obligations under 
					this Agreement.
					<br />
					<br />
					Print Name:
					<br />
					<br />
					Signature:
					<br />
					<br />
					Dated the ___________ day of ____________ 2003
					<br />
					<br />
					Address:
					<br />
					<br />
					<br />
					<br />
					<br />
					Signature of Witness:
					<br />
					<br />
					Address of Witness:
				</div>
			</div>
			<xsl:variable name="last" select="LAST" />
			<xsl:if test="$last != 'TRUE'">
				<br class="pageBreak" />
			</xsl:if>
		</div>
	</xsl:template>		
	
	<xsl:template match="LINEITEMS">
		<xsl:apply-templates select="LINEITEM" />
	</xsl:template>
	
	<xsl:template match="LINEITEM">
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
		<xsl:apply-templates select="PB" />
	</xsl:template>
	
</xsl:stylesheet>

  