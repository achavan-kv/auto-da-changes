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

    <div style="position:relative; top: 50px;">
        <!-- resets the position context -->
        <xsl:apply-templates select="HEADER" />
        
      <div class="normal" style="LEFT: 0cm; WIDTH: 6.5cm; POSITION: absolute; TOP: 1.5cm; HEIGHT: 2cm">
        <h2 style="font-size:18px; font-family: 'Sans-Serif'">Hire Purchase Agreement</h2>
      </div>
      <div class="normal" style="LEFT: 8.5cm; WIDTH: 5.5cm; POSITION: absolute; TOP: 1.5cm; HEIGHT: 2cm">
        <h2 style="font-size:12px; font-family: 'Sans-Serif'">Account Number</h2>
      </div>
      <div class="smallSS" style="LEFT: 0cm; WIDTH: 18.5cm; POSITION: absolute; TOP: 3cm; HEIGHT: 2cm; font-size:9px">
        This agreement is made
        <br/> Between UNICOMER (
        <xsl:value-of select="HEADER/COUNTRYNAME" />) Ltd trading as RadioShack whose registered
        <br/> office is situated at 79 to 81a Slipe Road, Kingston 5, Jamaica
        <br /> (hereinafter called “RadioShack”)
      </div>
      <div class="smallSS" style="WIDTH: 350px; POSITION: absolute; TOP: 5cm; HEIGHT: 3cm">
        <p style="font-weight:bold;">SCHEDULE</p>
      </div>
      <div class="smallSS" style="LEFT: 8.5cm; WIDTH: 3cm; POSITION: absolute; TOP: 3cm; HEIGHT: 2cm; font-size:8px">
        Title, First Name, Surname &amp; TRN
        <br/>
        <br/> Address:
        <br/> (hereinafter called “The Customer”)
      </div>
      <div class="smallSS" style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 4px; BORDER-TOP: gray thin solid; BORDER-BOTTOM: gray thin solid;PADDING-LEFT: 4px; LEFT: 11.5cm; BORDER-LEFT: gray thin solid; WIDTH: 7cm; POSITION: absolute; TOP: 1cm; HEIGHT: 3.75cm">
        <xsl:value-of select="HEADER/ACCTNO" />
        <br />
        <br />
        <xsl:value-of select="HEADER/NAME" />
        <br />
        <xsl:value-of select="HEADER/JOINTNAME" />
        <br />
        <xsl:value-of select="HEADER/ADDR1" />
        <br />
        <xsl:value-of select="HEADER/ADDR2" />
        <br />
        <xsl:value-of select="HEADER/ADDR3" />
        <br />
        <xsl:value-of select="HEADER/POSTCODE" />
      </div>
      <div style="BORDER: grey thin solid; LEFT: 0cm; TOP: 5.5cm; WIDTH: 18.5cm; HEIGHT: 13cm; POSITION: absolute">
      </div>
      <div style="BORDER-TOP: gray thin solid; PADDING-RIGHT: 5px; PADDING-LEFT: 1px; LEFT: 0cm; WIDTH: 18.5cm; POSITION: absolute; TOP: 6cm; HEIGHT: 7cm">
        <xsl:apply-templates select="LINEITEMS" />
      </div>
      <div style="LEFT: 0cm; WIDTH: 18.5cm; BORDER-TOP: gray thin solid; POSITION: absolute; TOP: 5.5cm">
      </div>
      <div style="LEFT: 0cm; WIDTH: 18.5cm; BORDER-TOP: gray thin solid; gray thin solid; POSITION: absolute; PADDING-RIGHT: 5px; TOP: 11cm; HEIGHT: 3.5cm">
        <table class="smallSS" style="border-collapse: collapse; border-spacing: 0;" width="100%" ID="Table1">
          <tr>
            <td rowspan="6" style="padding-left: 10px" width="40%">
              Balance payable by [
              <xsl:value-of select="../FOOTER/INSTALNO" />] instalments of [
              <xsl:value-of select="../FOOTER/FIRSTINST" />] and a final instalment of [
              <xsl:value-of select="../FOOTER/FINALINST" />]Balance payable by
              <xsl:value-of select="../FOOTER/INSTALNO" /> instalments of
              <xsl:value-of select="../FOOTER/FIRSTINST" /> and a final instalment of
              <xsl:value-of select="../FOOTER/FINALINST" />
              <br />
              <br /> Agree minimum payment
              <xsl:value-of select="../FOOTER/TOPAY" />
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
      <div style="TOP: 11cm; HEIGHT: 2.5cm; LEFT: 0cm; WIDTH: 9cm; BORDER-RIGHT: gray thin solid; BORDER-RIGHT: gray thin solid; POSITION: absolute">
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
      <div class="smallSS" style="LEFT: 0cm; WIDTH: 18.5cm; POSITION: absolute; TOP: 13.5cm; HEIGHT: 0.5cm; BORDER-TOP: gray thin solid; BORDER-BOTTOM: gray thin solid font-size:9px">
        WHEREBY <b>RadioShack</b> will supply and the Customer will take the goods listed in the schedule upon the following
        terms and conditions:
      </div>
      <div class="SmallPrint" style="LEFT: 0.1cm; WIDTH: 9.6cm; POSITION: absolute; TOP: 14cm; HEIGHT: 4.4cm; border-RIGHT: gray thin solid; font-size:9px; font-family: sans-serif; line-height:10px"
        id="Div2" language="javascript" onclick="return DIV1_onclick()">
        <b style="margin-top:2px;">IN THIS AGREEMENT</b><br />
        <b>&quot;Cash Price&quot;</b> means the price in which the goods may be purchased by the purchaser for cash<br />
        <b>&quot;RadioShack&quot;</b> is the owner of the goods listed in the Schedule to this agreement.<br />
        <b>&quot;The Customer&quot;</b> is the hirer of the goods listed in the Schedule to this agreement.<br />
        <b>&quot;Agreement Price&quot;</b> refers to the total Hire Purchase Price of the said goods listed in the Schedule.<br
        />
        <b>&quot;Regular Price&quot;</b> refers to the price of the said goods before any cash discounts may be applied.<br
        />
        <br /> &quot;total disablement&quot; includes any disability preventing the Customer from doing the job which he/she
        was carrying-on for profit immediately prior to the disability and not doing any other paid or profitable job, and
        he/she receives regular care and continuing treatment from a registered medical practitioner for the sickness, disease
        or bodily injury causing the disability. &quot;total loss of goods&quot; includes damage to the goods that is beyond
        repair.

      </div>
      <div class="SmallPrint" style="LEFT: 9.75cm; WIDTH: 6.4cm; POSITION: absolute; TOP: 14cm; HEIGHT: 5cm" align="center" id="Div2"
        language="javascript" onclick="return DIV1_onclick()">
        <h2 style="font-size:12px; font-weight:bold; text-decoration:underline; margin-top:2px;">SPECIAL CONDITIONS</h2>
      </div>
      <div class="SmallPrint" style="LEFT: 9.75cm; WIDTH: 8cm; POSITION: absolute; TOP: 20cm; HEIGHT: 3.5cm" align="justify" id="Div5"
        language="javascript">
      </div>
      <div class="SmallPrint" style="LEFT: 0.1cm; WIDTH: 9cm; POSITION: absolute; TOP: 18.6cm; HEIGHT: 7cm; font-family: sans-serif; line-height:13px"
        id="Div2" language="javascript" onclick="return DIV1_onclick()">
        <b>PAYMENTS</b><br /> 1. The <b>Customer</b> will pay <b>RadioShack</b>
        <div style="margin-left:0.7cm">
          (a) on the signing of this Agreement the above deposit as an initial payment.
          <br /> (b) Thereafter or unless otherwise agreed in writing punctually, and without further demand the above installments
          commencing one month after the delivery of the good by RadioShack to the Customer, such payment to be made to RadioShack
          at any of their stores in the island of Jamaica.
        </div>
        <b>EXPENSES</b><br />
        <div style="margin-left:0.7cm">
          (c) on demand, any expense incurred by <b>RadioShack</b> in
          <br />
          <div style="margin-left:0.7cm">
            (i) determining the whereabouts of the <b>Customer</b>, or of the goods, or
            <br /> (ii) recovering possession of the goods from the Customer or from any other person (including any payment
            made by <b>RadioShack</b> to discharge or satisfy any lien or claim to the goods) or
          </div>

        </div>
      </div>
      <div class="SmallPrint" style="LEFT: 9.6cm; WIDTH: 9cm; POSITION: absolute; TOP: 18.6cm; HEIGHT: 7cm; font-family: sans-serif; line-height:13px"
        id="Div2" language="javascript" onclick="return DIV1_onclick()">
        <div style="margin-left:0.7cm">
          <br />
          <div style="margin-left:0.7cm">
            (iii) enforcing payment of any installment or any other<br />
             sum payable under this Agreement including any <br />
              legal charges, incurred
            by <b>RadioShack</b> whether legal <br />
             proceedings have been instituted or not, as well as <br />
             <b>RadioShack</b> costs and Bailiff’s fees, and any <br /> 
             amount due to <b>RadioShack</b> pursuant to <br />
             Clause 2 (c) below.
          </div>

        </div>
        <b>OWNERSHIP</b><br /> 2.
        <span style="margin-left:0.7cm">
        (a) The customer may at any time pay to RadioShack the <br /> 
        full Agreement Price set out in the Schedule to the Agreement along <br />
         with any payment due under Clause (1) above, at which time the goods <br /> 
         will become the property of the Customer.
        </span>
        <br />
        <b>REBATES</b><br />
        <div style="margin-left:0.7cm">
          (b) Where the full balance of the Agreement Price is paid to RadioShack not less than one month before it is due a rebate <br />

          in the price of the goods shall be allowed by RadioShack <br />
          to the Customer at the rate of no less than 5% per annum.
        </div>
      </div>

      <br class="pageBreak" />
      <div style="position:relative">
        <div class="SmallPrint" style="LEFT: 9.6cm; WIDTH: 9cm; POSITION: absolute; TOP: 0cm; HEIGHT: 22cm; font-family: sans-serif; line-height:13px"
          id="Div2" language="javascript" onclick="return DIV1_onclick()">
          <div style="left:3cm; text-align:center;">
            <b>Notice</b><br />
            Pursuant to the Hire Purchase Act
          </div>
          <br /> 
          A.
          <span style="margin-left:0.2cm">
          The <b>Customer</b> may put an end to the Agreement by giving notice of termination in writing to <b>RadioShack</b>.
          </span>
          <br /> 
          <br /> 
          B.
          <span style="margin-left:0.2cm">
          The <b>Customer</b> must then pay any installments which are in arrears at the time when he/she gives notice. 
          If, when he/she has paid those installments, the total amount paid under the Agreement is less than the agreed minimum payment 
          overleaf he/she must also pay enough to make up that sum, unless the Court determines that a smaller sum would be equal to <b>RadioShack’s</b> loss.
          </span>
          <br /> 
          <br /> 
          C.
          <span style="margin-left:0.2cm">
          C.	If the goods have been damaged owing to the <b>Customer</b> having failed to take reasonable care of them, <b>RadioShack</b> may sue him/her 
          for the amount of the damage unless that amount can be agreed between the <b>Customer</b> and <b>RadioShack</b>
          </span>
          <br /> 
          <br /> 
          D.
          <span style="margin-left:0.2cm">
          The <b>Customer</b> should see whether this agreement contains provisions allowing him/her to put an end to the agreement on terms more favourable than those just mentioned. If it does, he/she may put an end to the agreement on those terms.
          </span>
          <br /> 
          <br /> 
          E.
          <span style="margin-left:0.2cm">
          Unless the <b>Customer</b> has himself/herself put an end to the Agreement,
          <b>RadioShack</b> cannot take back the goods from the <b>Customer</b> without the <b>Customer</b>’s consent unless <b>RadioShack</b> complies with any applicable requirement of Jamaican Law including the Security Interests in Personal Property Act 2013 (The SIPP Act”) (which repealed Part III of the Hire Purchase Act. 
          </span>
          
          <br />
          Section 34 (2) of the SIPP Act provides as follows<br />
          (2) On default under a security contract <br />
          (a)
          <span style="margin-left:0.5cm">
            (a)	the secured creditor has, unless otherwise agreed between the parties, the right to take possession of the secured property or otherwise enforce the security contract by any method permitted by law;
          </span>
          <br />
          (b)
          <span style="margin-left:0.5cm">
            (b)	if the security interest is perfected by registration and the secured property is of a kind that cannot be readily moved from the premises where the property is located or is of a kind for which adequate storage facilities are not readily available, the secured creditor may seize or repossess the secured property, without removing it from those premises, in any manner by which a writ in aid of execution may provide for seizure without removal; and
            (c)	where paragraph (b) applies, the secured creditor may dispose of secured property on the premises concerned, but shall not cause the person in possession of the premises any greater inconvenience and cost than is necessarily incidental to the disposal.
          </span>
          <br />
          <br />
          <br />
          <br />
            I/We warrant and confirm the information given herein is true and correct and I/We understand it is being used to determine My/Our credit responsibility. I/We further confirm that no information, which might affect Unicomer’s decision to make the sale, has been withheld. I/We hereby authorize and consent to Unicomer receiving and exchanging any financial and other information which it may have in its possession about Me/Us with any of its subsidiaries, agents, third party assignees, other financial institutions, Credit Bureaus or other person of Corporation/s or with whom I/We may have or propose to have financial dealings from time to time. I/We indemnify Unicomer against any loss, claims, damages, liabilities, actions and proceedings, 
            legal and or other expense which may be directly and reasonably incurred as a consequence of the disclosure of the financial or other information
        </div>
        <div class="SmallPrint" style="LEFT: 0.1cm; WIDTH: 9cm; POSITION: absolute; TOP: 0cm; HEIGHT: 24cm; font-family: sans-serif; line-height:13px"
          id="Div2" language="javascript" onclick="return DIV1_onclick()">
          <b>FAILURE TO PAY ON TIME</b><br />
          <div style="margin-left:1cm">
            (c) Where the full balance of the Agreement Price remains unpaid for more than one month after the date on which it is due,
            RadioShack may charge interest at the same rate as the service charge rate of this Hire Purchase Contract calculated
            from the date the amount falls due until the date of payment.
            <br /> (d) Where any installment of the Agreement Price remains unpaid after the date on which it is due RadioShack
            shall have the right to treat this default as a breach of this Agreement and charge interest at the same rate
            as the service charge rate of this Hire Purchase Contract. This interest shall be calculated on a daily basis
            from the due date until the monthly installment is received by <b>RadioShack</b>.
          </div>
          <br />
          <b>CARE AND CUSTODY OF THE GOODS</b><br />
          <div style="margin-left:1cm">
            (e) Until the full payment is made to RadioShack, the Customer is responsible for maintaining the goods specified in the
            schedule, in good condition and shall be responsible for all loss or damage however occasioned
            <br /> (f) Until full payment is made to RadioShack, the Customer shall not dispose, part with possession of,
            sell, lease or encumber the goods or use the goods as collateral in any transaction
          </div>
          <br />
          <b>ADDRESS OF THE GOODS</b><br /> 3. <span style="margin-left:1cm">
        The <b>customer</b> may at any time pay to RadioShack the full Agreement Price set out in the Schedule to the Agreement
         along with any payment due under Clause (1) above, at which time the goods will become the property of the <b>Customer</b>.
        </span>
          <br />
          <br />
          <b>INSURANCE</b><br /> 4. <span style="margin-left:1cm">
        A portion of the Hire Purchase Price relates to the purchase of insurance by the <b>CUSTOMER</b> with British Caribbean Insurance Co. Ltd. under Policy No. CPI 57328 under which all outstanding payments due will be paid by the insurers to Unicomer (Jamaica) Ltd. as Trustee for the <b>CUSTOMER</b>. 
        <br />
        Contracts 12 months and over 4.71% of goods + GCT 
        <br />
        The settlement is subject to the terms and conditions of the policy, full details of which are contained in the respective Payment Protection Plans.

        </span>
          <br />
          <br />
          <b>TERMINATION &amp; REPOSSESSION</b><br /> 5. <span style="margin-left:1cm">
        On termination of this Agreement for any reason whatsoever, the <b>Customer</b> agrees that he/she will make the goods available to <b>RadioShack</b> for repossession. 
        RadioShack may terminate this agreement as a result of any default by the Customer including any breach of the warranties given by the Customer below
        </span>
          <br />
          <br />
          <b>LAPSED AGREEMENT</b><br /> 6. <span style="margin-left:1cm">
        No leniency or waiver shown to the <b>Customer</b> shall prejudice the strict rights of <b>RadioShack</b> under this Agreement.
        If the <b>Customer</b> has not taken delivery of the goods for a period of 60 days after this Agreement is made, 
        the Agreement will be of no effect and the Customer will then be entitled to the refund of any installment paid to <b>RadioShack</b>. 

        </span>
        </div>

        <div style="position:absolute; left:0cm; width: 9.2cm; height:6cm; top: 19.5cm; border:gray thin solid; font-size:10px; font-family:sans-serif; padding:10px;">
          This Agreement is subject to verification of customer information and final credit approval by RadioShack. 
          Delivery of goods will be made after final approval is granted.  
          <br />I/We have been informed of the regular price of the goods before entering into this agreement and understand the contents of this agreement and schedule.
          <br />
          <br />
          Signature of Customer (1) ___________________________
          <br />
          <br />
          Signature of Customer (2) ___________________________
          <br />
          <br />
          Witness to Customer's Signature _______________________
          <br />
          <br />
          Name ___________________________
          <br />
          <br />
          Address ___________________________
          <br />
          <br />
          For and on behalf of RadioShack _____________________
        </div>
      </div>

      <xsl:variable name="lastPage" select="LAST" />
      <xsl:variable name="lastAgreement" select="../LAST" />
      <xsl:if test="$lastPage = 'False' or $lastAgreement = 'FALSE' ">
        <br class="pageBreak" />
        <!-- if it's not the last page -->
      </xsl:if>
    </div>
  </xsl:template>

    <xsl:template match="HEADER">
        <!--<div style="LEFT: 1px; WIDTH: 185px; POSITION: absolute; TOP: 53px; HEIGHT: 2cm;">-->
        <div style="LEFT: 1px; WIDTH: 185px; POSITION: absolute;  HEIGHT: 2cm;">
            <IMG src="{//AGREEMENTS/@IMAGEPATH}radioshack.png"  height="50px" />
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
            <xsl:value-of select="DESC" /> (
            <xsl:value-of select="ACCTNO" />)
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