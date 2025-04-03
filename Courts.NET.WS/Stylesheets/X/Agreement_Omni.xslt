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
      <div style="LEFT: 0cm; WIDTH: 18.5cm; POSITION: absolute; TOP: 0cm; HEIGHT: 2cm">
        <IMG src="{//AGREEMENTS/@IMAGEPATH}omni.png" height="35px" />
      </div>
      <div class="normal" style="LEFT: 7cm; WIDTH: 10.5cm; POSITION: absolute; TOP: 0cm; HEIGHT: 2cm">
        AGREEMENT OF HIRE-PURCHASE (HUURKOOP)
      </div>
      <div class="normal" style="LEFT: 9cm; WIDTH: 3.5cm; POSITION: absolute; TOP: 1.7cm; HEIGHT: 2cm">
        ACCOUNT NUMBER
      </div>
      <div style="BORDER-RIGHT: gray thin solid; PADDING-RIGHT: 5px; BORDER-TOP: gray thin solid; PADDING-LEFT: 5px; LEFT: 12.5cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray thin solid; WIDTH: 5cm; PADDING-TOP: 5px; BORDER-BOTTOM: gray thin solid; POSITION: absolute; TOP: 1.7cm; HEIGHT: 1cm; TEXT-ALIGN: center">
        <xsl:value-of select="HEADER/ACCTNO" />
      </div>
      <div class="smallSS" style="LEFT: 0cm; WIDTH: 18.5cm; POSITION: absolute; TOP: 1.5cm; HEIGHT: 2cm">
        Unicomer Curacao.B.V. Omni St. Maarten
        <br/>
        Orange Grove Shopping Center #9 Colebay, St. Maarten
        <br/>
        Telephone no 1-721-544-2190
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
      <div style="LEFT: 0cm; TOP: 5.5cm; WIDTH: 18.5cm; HEIGHT: 22cm; POSITION: absolute">
      </div>
      <div style="BORDER-TOP: gray thin solid; PADDING-RIGHT: 5px; PADDING-LEFT: 1px; LEFT: 0cm; WIDTH: 18cm; POSITION: absolute; TOP: 6cm; HEIGHT: 7cm">
        <xsl:apply-templates select="LINEITEMS" />
      </div>
      <div style="LEFT: 0cm; WIDTH: 18.5cm; BORDER-TOP: gray thin solid; POSITION: absolute; TOP: 5.5cm">
      </div>
      <div style="LEFT: 0cm; WIDTH: 18.5cm; BORDER-TOP: gray thin solid; POSITION: absolute; PADDING-RIGHT: 5px; TOP: 11cm; HEIGHT: 3.5cm">
        <table class="smallSS" style="border-collapse: collapse; border-spacing: 0;" width="90%" ID="Table1">
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
        It is understood and agreed that the object of hire purchase sale will remain in the ownership of the Seller unless and until the purchaser has paid the
        entire purchase price plus interest and costs if any.
      </div>
      <div style="LEFT: 6cm; WIDTH:7cm; POSITION: absolute; TOP:14.8cm; HEIGHT:0.5cm; font-style:bold" align="center">
      Terms and Conditions Hire Purchase
      </div>
      <div class="SmallPrintCuracao" style="LEFT: 0.5cm; WIDTH: 8cm; POSITION: absolute; TOP: 15.4cm; HEIGHT: 12.5cm; font-size:9px;" align="justify" id="Div3" language="javascript" onclick="return DIV1_onclick()">
        <b>Article 1</b>
        <br />
        On this day, aforementioned object purchased on the basis of hire
        purchase has been brought under the actual control and in the actual
        possession of the purchaser in good condition and without any damage,
        which the purchaser hereby acknowledges. The purchaser declares to
        have thoroughly examined the object purchased on the basis of hire
        purchase and not to hold Unicomer liable in any event for any visible or
        unhidden defect or for any noticeable external defectiveness of any nature
        whatsoever. Unicomer retains ownership of the object sold, until the
        purchaser will have paid the aforementioned remainder of the
        hire-purchase sum, as well as all that he owes or will owe pursuant to this
        agreement on account of expenses, penalties, or otherwise.
        <br />
        <br />
        <b>Article 2</b>
        <br />
        As soon as the purchaser has paid everything that he owes or will owe
        under this agreement, he will obtain the ownership of the object sold,
        without any statement on the part of Unicomer or transfer of title in another
        way being required.
        <br />
        <br />
        <b>Article 3</b>
        <br />
        During the period that the property rights of the object purchased on the
        basis of hire purchase (hereinafter: “the object”) belongs to Unicomer:
        - the purchaser is obligated to exercise caution when utilizing the
        object;- Purchaser is not to transfer its ownership, trade, pledge, give it in
        use to third parties or to perform any action that could undermine or affect
        the ownership rights belonging to Unicomer;
        - the purchaser is obligated to utilize the object with due care, to maintain
        it properly, to arrange for all necessary repairs, of any nature whatsoever,
        and which have become necessary due to any cause whatsoever, to be
        carried out;
        - the purchaser is not permitted to utilize the object in a manner that is not
        its normal intended use. Neither is the purchaser permitted to modify said
        object or its accessories.
      </div>
      <div class="SmallPrintCuracao" style="LEFT: 9.75cm; WIDTH: 8cm; POSITION: absolute; TOP: 15.4cm; HEIGHT: 12.5cm; font-size:9px" align="justify" id="Div5" language="javascript">
        <b>Article 4</b>
        <br />
        From the moment of the delivery of the object, the object is entirely for the account
        and risk of the purchaser. Notwithstanding any depreciation, damage or loss, by
        whatever cause, he will be obligated to fulfill all obligations arising for him from this
        agreement and the law. Unicomer is not obligated to any indemnification
        whatsoever.
        <br />
        <br />
        <b>Article 5</b>
        <br />
        The purchaser must pay aforementioned remainder of the hire-purchase sum
        in the specified consecutive monthly installments on the due date mentioned.
        Purchaser agrees to the use of Unicomer’s administration as determining proof of
        payment, barring refuting evidence.
        <br />
        <br />
        <b>Article 6</b>
        <br />
        The Purchaser is free to make early payments. In case of early payment, the
        purchaser is entitled to a deduction of up to one percent per installment amount
        paid early. 
        <br />
        <br />
        <b>Article 7</b>
        <br />
        If at any time, the purchaser fails to pay one or more hire-purchase installments or
        parts thereof on time and/or completely after having been given notice of default,
        the purchaser will owe Unicomer 1.5% interest per month in respect of all
        installments still unpaid at that time, including the installments not yet due.
        Furthermore, the purchaser will owe Unicomer extra judicial collection costs
        amounting to 15% of all unpaid installments, including the installments not yet
        due
        <br />
        <br />
        <b>Article 8</b>
        <br />
        Unicomer can agree to a payment postponement, if requested by purchaser in
        writing and before passing of the period in which the payment is to be made, this
        will not affect in any way the rights of Unicomer arising from this agreement, and
        the purchaser can never derive any rights from such postponement.
      </div>
      

      <br class="pageBreak" />
      <div class="SmallPrintCuracao" style="LEFT: 0.5cm; WIDTH: 8cm; POSITION: absolute; TOP: 1cm; HEIGHT: 12.5cm; font-size:9px;" align="justify" id="Div3" language="javascript" onclick="return DIV1_onclick()">
        <b>Article 9</b>
        <br />
        In all cases in which:
        a.the purchaser fails to pay one or more installments of the
        hire-purchase sum after having been given notice of default on account
        thereof;<br/>
        b.the purchaser does not fulfill any obligation to which he is subject
        pursuant to the law or this agreement, after having been given notice of
        default on account thereof, and continues to fail to do so;<br/>
        c.the purchaser acts contrary to any obligation to which he is subject;<br/>
        d.the purchaser is declared bankrupt, proceeds to the assignment of
        his estate, is placed under conservatorship, comes to an extra judicial
        agreement with his creditors, or files an application to obtain a
        moratorium;<br/>
        e.attachment is levied on all or part of his assets or the object;<br/>
        f.the purchaser passes away or proceeds to liquidate his assets;<br/>
        g.the purchaser leaves the island of Curaçao to take up residence<br/>
        elsewhere, without having paid the full hire-purchase sum and all that he
        owes on account thereof at that time;<br/>
        h.the purchaser uses and/or allows others to use the object for a purpose
        other than for which it is intended; as well as:<br/>
        i.if the object is lost by whatever cause, including theft and
        embezzlement;<br/>
        j.if the object is damaged to such an extent that, in the opinion of an
        expert to be designated by Unicomer, which opinion is binding on both
        parties, repair is no longer justified;<br/>
        and after having given written notice of default, Unicomer will be entitled
        to repossess the object, at its discretion and for the account of the
        purchaser, with or without judicial intervention,
        whether or not accompanied with the dissolution of this hire-purchase
        agreement, or to claim the amount payable under this agreement
        immediately and fully.<br/>
        Furthermore, irrespective of the choice made by Unicomer, the
        purchaser will be obligated, in the cases mentioned in sub paragraphs
        a, b, c, g, h and j, to pay a penalty of 8% in respect of the remaining
        hire-purchase sum,
        irrespective of the right of Unicomer to full recovery of all losses and
        damages suffered by it. In case of death of the purchaser, the entire
        amount then still payable will be an obligation for his heirs, so that, if
        full payment thereof has not been made within one month after having
        claimed same, Unicomer may claim and recover the full amount still
        payable from his heirs.
        <br />
        <br />
        <b>Article 10</b>
        <br />
        If Unicomer has repossessed the object purchased in the manner set
        forth above, with or without judicial intervention, the value will be
        determined by an expert to be designated by Unicomer, or, if the
        purchaser expresses the wish to do so in writing, at the amount that the
        object will yield at a public sale after deduction of all associated costs. In
        both cases, the part of the purchase sum still unpaid, plus interest,
        penalty, costs of the repossession, and any other costs, will be reduced
        by the value determined and/or the proceeds of the object purchased,
        and the purchaser will be obligated to pay Unicomer immediately the
        balance thus determined with interest and other additional costs, without
        demand.
      </div>
      <div class="SmallPrintCuracao" style="LEFT: 9.75cm; WIDTH: 8cm; POSITION: absolute; TOP: 1cm; HEIGHT: 12.5cm; font-size:9px" align="justify" id="Div5" language="javascript">
        <b>Article 11</b>
        <br />
        As long as the ownership of the object has not been transferred to the
        purchaser, he is obligated - if Unicomer so demands - to insure it, for his
        account, with an insurance company to be designated by Unicomer, against
        loss and damage and all other risks of which insurance is deemed necessary
        by Unicomer, in the name and for the benefit of Unicomer as owner. The policy
        of this insurance will be kept by Unicomer, until the purchaser has fulfilled
        all his obligations arising from this agreement. Afterwards, Unicomer will be
        obligated to change the name of the policy into that of the purchaser or a third
        party to be designated by him. Unicomer will also be entitled to take out this
        insurance itself in its own name and for its own benefit, for the account of the
        purchaser.
        <br />
        <br />
        <b>Article 12</b>
        <br />
        The purchaser is obligated to immediately inform Unicomer in case of damage
        to the object, in case attachment is levied on the object or in case there is an
        attachment about to be levied on the object.
        <br />
        <br />
        <b>Article 13</b>
        <br />
        All costs to be incurred by Unicomer to enforce its rights under this agreement,
        under whatever name, will be payable by the purchaser, also including the
        costs of repossession pursuant to Article 9 of this agreement and all judicial
        and extra judicial costs, including registration costs, as well as the costs arising
        from or relating to any assignment of the claims arising from this agreement
        by Unicomer to third parties. Likewise, all present and future payments arising
        from the possession or use of the object purchased will be payable by the
        purchaser. The purchaser will also bear the stamp duties of this agreement,
        if any
        <br />
        <br />
        <b>Article 14</b>
        <br />
        The purchaser may never avoid payment of any installments of the purchase
        price, neither by invoking set-off or any discount or based on any
        counter claim by whatever name, nor by levying attachment against himself,
        while the purchaser will also explicitly renounce any action for the dissolution of
        this agreement on account of breach of contract.
        <br />
        <br />
        <b>Article 15</b>
        <br />
        In case his actual residence is changed, the purchaser is obligated to
        accurately notify Unicomer hereof by certified letter within 72 hours
        <br />
        <br />
        <b>Article 16</b>
        <br />
        All arrangements outside this agreement are invalid. Each warranty of
        Unicomer is excluded, unless explicitly laid down in a separate letter of
        Unicomer to the purchaser
        <br />
        <br />
        <b>Article 17</b>
        <br />
        The purchaser declares to have received a copy of this agreement in good
        order, signed by Unicomer.
      </div>
      <div class="line" style="border-bottom: 1px solid gray; width: 19cm; top: 18cm; position:absolute;"></div>
      <div style="top:18.5cm; position:absolute; left: 0,5cm; font-size:9pt; font-weight:bold;"> 
        The undersigned purchaser acknowledges and agrees with the aforementioned terms and conditions of this hire purchase agreement
      </div>
      <div style="top: 19.5cm; position: absolute">
        Curacao <br/>
      </div>
      <div class="SmallPrint" style="LEFT: 0cm; WIDTH: 8cm; POSITION: absolute; TOP: 20.5cm;font-size:13px" align="justify" id="Div5" language="javascript">
          <br />
          __________________________________________
          <br />
          <br />
          Seller
          <br />
          Unicomer (Curaçao) B.V.
          <br />
          Omni St. Maarten
      </div>
      <div class="SmallPrint" style="LEFT: 9.75cm; WIDTH: 8cm; POSITION: absolute; TOP: 20.5cm; font-size:13px" align="justify" id="Div5" language="javascript">
        <br />
        __________________________________________
        <br />
        <br />
        Purchaser
      </div>


      <xsl:variable name="lastPage" select="LAST" />
      <xsl:variable name="lastAgreement" select="../LAST" />
      <xsl:if test="$lastPage = 'False' or $lastAgreement = 'FALSE' ">
        <br class="pageBreak" />
        <!-- if it's not the last page -->
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

