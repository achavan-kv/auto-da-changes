<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:template match="/">
		<HTML>
			<head>
				<TITLE></TITLE>
				<META content="Microsoft Visual Studio 7.0" name="GENERATOR"></META>
				<META content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"></META>
				<style type="text/css" media="all"> @import url(styles.css); 
				</style>
			</head>
			<BODY>	
				<xsl:apply-templates select="RFTERMS" />
			</BODY>
		</HTML>		
	</xsl:template>

	<xsl:template match="RFTERMS">
		<div class="RFHead1" align="center">
			<u>Ready Finance - Terms &amp; Conditions</u>
			<br/>
			<br/>
		</div>
		<div style="LEFT: 0.3cm; WIDTH: 3cm; POSITION: absolute; HEIGHT: 1cm"><IMG style="HEIGHT: 50px" src="smallLogo.jpg" />
		</div>
		<div class="smallSS" style="LEFT: 0.5cm; POSITION: absolute; TOP: 4.0cm">This agreement explains 
			the terms of our Ready Finance plan and governs its use at all times. In this 
			agreement the word “you” and “your” refer to each and all of the persons who 
			use a Ready Finance card issued by us, under an account that we hold. The word 
			“we”, “us” and “our” means Courts Jamaica Limited.
			<br/>
			When you accept or use our card, you agree to the terms in this agreement.
		</div>
		<div class="smallSS" style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px; LEFT: 6.5cm; PADDING-BOTTOM: 5px; WIDTH: 4cm; PADDING-TOP: 6px; POSITION: absolute; TOP: 5.414cm; HEIGHT: 4.7cm">
			<xsl:value-of select="ACCOUNTNO"/>
			<br />
			<br />
			<xsl:value-of select="NAME"/>
			<br />
			<xsl:value-of select="ADDRESS1"/>
			<br />
			<xsl:value-of select="ADDRESS2"/>
			<br />
			<xsl:value-of select="ADDRESS3"/>
			<br />
			<xsl:value-of select="POSTCODE"/>
			<br />
			<br />
			<br />
			<xsl:value-of select="LIMIT"/>
			<br />
			<br />
			<xsl:value-of select="VALID"/>
		</div>
		<div class="smallSS" style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 5px; BORDER-TOP: gray thin solid; PADDING-LEFT: 5px; LEFT: 2.5cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray thin solid; WIDTH: 11.3cm; PADDING-TOP: 5px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 5.414cm; HEIGHT: 4.7cm">Account 
			Number:
			<br/>
			<br/>
			Name:
			<br/>
			Address:
			<br/>
			<br/>
			<br/>
			<br/>
			<br/>
			<br/>
			Spend Limit:
			<br/>
			<br/>
			Valid From: 
		</div>
		<div class="smallSS" style="LEFT: 0.5cm; WIDTH: 1cm; POSITION: absolute; TOP: 11cm; TEXT-ALIGN: justify"><b>1</b>
			<br/>
			1.1
			<br/>
			1.2
			<br/>
			<br/>
			<br/>
			<b>2</b>
			<br/>
			2.1
			<br/>
			<br/>
			2.2
			<br/>
			<br/>
			<br/>
			<br/>
			<b>3</b>
			<br/>
			3.1
			<br/>
			<br/>
			<b>4</b>
			<br/>
			4.1
			<br/>
			<br/>
			<br/>
			4.2
			<br/>
			<br/>
			4.3
			<br/>
			<br/>
			<br/>
			4.4
			<br/>
			<br/>
			<br/>
			<br/>
			<b>5</b>
			<br/>
			5.1
		</div>
		<div class="smallSS" style="LEFT: 1.5cm; WIDTH: 8cm; POSITION: absolute; TOP: 11cm; TEXT-ALIGN: justify"><b>Ready 
				Finance Card</b>
			<br/>
			The card remains the property of Courts Jamaica Limited.
			<br/>
			You may use the card to purchase goods at any Branch of Courts Jamaica Limited.
			<br/>
			<br/>
			<b>Account Structure</b>
			<br/>
			You will have one master Ready finance account with a spending limit.
			<br/>
			Each purchase that you make under this account will be referred to as a 
			"sub-agreement" and covered by an individual Hire Purchase contract.
			<br/>
			<br/>
			<b>Hire Purchase Act</b>
			<br/>
			All your transactions will be subject to the Hire Purchase Act.
			<br/>
			<br/>
			<b>Payments</b>
			<br/>
			You will make one single payment on or before the agreed payment date each month 
			to cover the monthly instalment on your ready finance account.
			<br/>
			Your monthly payment date will be one month after the delivery of the first 
			item of goods by us.
			<br/>
			Unless specifically requested by you the monthly payment will be applied on a 
			pro rata basis across all outstanding hire purchase agreements.
			<br/>
			If your payment cheque is returned unpaid for any reason, we will charge your 
			account a returned cheque fee for each returned cheque.
			<br/>
			<br/>
			<b>Spending Limit</b>
			<br/>
			Your spending limit is granted on the basis of information supplied. If your 
			personal information proves to have been falsely supplied we reserve the right 
			to withdraw your spending limit and refuse credit with us.
		</div>
		<div class="smallSS" style="LEFT: 10.5cm; WIDTH: 1cm; POSITION: absolute; TOP: 11cm; TEXT-ALIGN: justify">5.2
			<br/>
			<br/>
			5.3
			<br/>
			<br/>
			5.4
			<br/>
			<br/>
			<br/>
			<b>6</b>
			<br/>
			6.1
			<br/>
			<br/>
			<br/>
			<br/>
			6.2
			<br/>
			<br/>
			<br/>
			<br/>
			<br/>
			<br/>
			<b>7</b>
			<br/>
			7.1
			<br/>
			<br/>
			7.2
			<br/>
			7.3
			<br/>
			<br/>
			<br/>
			<b>8</b>
			<br/>
			8.1
			<br/>
			<br/>
			<br/>
			<br/>
			<b>9</b>
			<br/>
			9.1
		</div>
		<div class="smallSS" style="LEFT: 11.5cm; WIDTH: 8cm; POSITION: absolute; TOP: 11cm; TEXT-ALIGN: justify">Your 
			spending limit is valid for 12 months from the date of application.
			<br/>
			If your spending limit is not used during the 12 months it will be cancelled 
			and your account will be closed.
			<br/>
			The total amount of credit outstanding at any time, together with charges, must 
			not be more than your spending limit.
			<br/>
			<br/>
			<b>Failure to pay on time</b>
			<br/>
			Your spending limit will be suspended if your monthly payment is not made on 
			time and your account becomes more than 1 month overdue. Once your account is 
			paid up your spending limit will be reinstated.
			<br/>
			If your account becomes more than 2 months overdue your Ready Finance account 
			will be barred from making additional purchases and your available spending 
			limit terminated. Each sub-contract will be treated as individual Hire Purchase 
			contracts and your account will be allocated for collection.
			<br/>
			<br/>
			<b>Courts' rights</b>
			<br/>
			We reserve the right to revise/cancel your spending limit at any time.
			<br/>
			Your spending limit will be allocated at our discretion.
			<br/>
			We may investigate your credit history, employment and income and verify your 
			credit references.
			<br/>
			<br/>
			<b>Proof of identity</b>
			<br/>
			In the event that your fail to present adequate proof of identity when using 
			your Ready Finance card, this will result in the card being refused and the 
			purchase denied.
			<br/>
			<br/>
			<b>Lost/Stolen card</b>
			<br/>
			In the event that your card is lost/stolen please report this to any Branch of 
			Courts Jamaica Limited.
		</div>
		<div class="smallSS" style="LEFT: 1cm; WIDTH: 8cm; POSITION: absolute; TOP: 23cm; TEXT-ALIGN: justify">
			<br />
			Signature:
			<br />
			<br />
			<br />
			Date:
                        <br />
                        <br />
						<br />
                        Witness to Customer's Signature: 

		</div>
	</xsl:template>
	
</xsl:stylesheet>

  