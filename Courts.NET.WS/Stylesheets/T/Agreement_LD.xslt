<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <HTML>
            <head>
                <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"></meta>
                <style type="text/css" media="all">
                    @import url(<xsl:value-of select="AGREEMENTS/@CSSPATH"/>	);
                    body,table{font-family:Helvetica; font-size:10}
                    .allborders{border: 1px solid black;}
                    .verticalborders{border-left: 1px solid black;border-right: 1px solid black;}
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

            <!-- start new page template -->

            <table>
                <tr>
                    <td>
                        <table width="100%">
                            <tr>
                                <td align="left">
                                    <img src="{//AGREEMENTS/@IMAGEPATH}LuckyDollarLogo.png" height="100px" width="185px" />
                                </td>
                                <td align="center">
                                    <img src="{//AGREEMENTS/@IMAGEPATH}StampDuty.jpg" height="73px" width="75px" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <h3>Hire Purchase Agreement</h3>
                    </td>
                </tr>
                <tr>
                    <td>
                        This agreement is made between Unicomer (Trinidad) Ltd. Title, First Name,
                        Surname trading as Lucky Dollar, whose registered office is situated at Corner Don Miguel Rd and Churchill Roosevlt Highway,El Socorro, San Juan, Trinidad (hereinafter Address called <b>“Lucky Dollar”</b>)
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td>
                                                <b>Account Number</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Title, First Name, Surname</td>
                                        </tr>
                                        <tr>
                                            <td>Address</td>
                                        </tr>
                                        <tr>
                                            <td>
                                                (hereinafter called <b>"The Customer"</b>)
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <div style="border:1px solid black;">
                                        <table>
                                            <tr>
                                                <td>
                                                    <xsl:value-of select="HEADER/ACCTNO" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <xsl:value-of select="HEADER/NAME" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <xsl:value-of select="HEADER/JOINTNAME" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <xsl:value-of select="HEADER/ADDR1" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <xsl:value-of select="HEADER/ADDR2" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <xsl:value-of select="HEADER/ADDR3" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <xsl:value-of select="HEADER/POSTCODE" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>



            <table  width="100%">
                <tr>
                    <td>
                        <b>SCHEDULE</b>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table  width="100%" border="0" style="border-collapse:collapse;">

                            <tr>
                                <td colspan="4" style="padding:0;">
                                    <xsl:apply-templates select="LINEITEMS" />
                                </td>
                            </tr>


                            <tr>
                                <td colspan="2"  width="50%" class="allborders">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <xsl:value-of select="../FOOTER/INSTALMENTS" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Agree minimum payment <xsl:value-of select="../FOOTER/TOPAY" />
                                                <div>
                                                    <b>Schedule of Payments</b><br />
                                                    <br />
                                                    Deposit/Initial Payment -[<xsl:value-of select="../FOOTER/DEPOSIT" />]
                                                    <br />
                                                    [<xsl:value-of select="../FOOTER/INSTALNO" />
                                                    ] Monthly payments of [<xsl:value-of select="../FOOTER/FIRSTINST" />]<br />
                                                    Final Payment of [<xsl:value-of select="../FOOTER/FINALINST" />]
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
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
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td colspan="2"  width="50%" class="allborders">

                                    <table width="100%">
                                        <tr>
                                            <td>
                                                Goods Value:
                                            </td>
                                            <td>
                                                <xsl:value-of select="../FOOTER/GOODSVAL" />
                                            </td>

                                        </tr>
                                        <tr>
                                            <td>
                                                Deposit:
                                            </td>
                                            <td>
                                                <xsl:value-of select="../FOOTER/DEPOSIT" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Balance/Credit Extended:
                                            </td>
                                            <td>
                                                <xsl:value-of select="../FOOTER/CREDIT" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Charge for Credit:
                                            </td>
                                            <td>
                                                <xsl:value-of select="../FOOTER/DT" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Balance Payable:
                                            </td>
                                            <td>
                                                <xsl:value-of select="../FOOTER/BALANCE" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Total Price:
                                            </td>
                                            <td>
                                                <xsl:value-of select="../FOOTER/TOTAL" />
                                            </td>
                                        </tr>
                                    </table>


                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" class="allborders">
                                    WHEREBY <b>Lucky Dollar</b> will supply and the <b>Customer</b> will
                                    take the goods listed in the schedule upon the following terms and condition.
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" class="allborders">
                                    <p>
                                        <b>IN THIS AGREEMENT</b>
                                    </p>
                                    <p>
                                        <b>“Lucky Dollar”</b> is the owner of the goods listed in the Schedule to this
                                        agreement.
                                    </p>
                                    <p>
                                        <b>“The Customer”</b> is the hirer of the goods listed in the Schedule to this
                                        agreement
                                    </p>
                                    <p>
                                        <b>“Agreement Price”</b> refers to the total hire purchase price of the said
                                        goods listed in the Schedule.
                                    </p>
                                    <p>
                                        <b>“Regular Price”</b> refers to the price of the said goods before any cash
                                        discounts may be applied.
                                    </p>
                                    <p>
                                        <b>“Total disablement”</b> includes any disability preventing the Customer from
                                        doing the job which he/she was carrying-on for profit immediately prior to the
                                        disability and not doing any other paid or profitable job, and he/she receives
                                        regular care and continuing treatment from a registered medical practitioner
                                        for the sickness , disease or bodily injury causing the disability.
                                    </p>
                                    <p>
                                        <b>“Total loss of goods”</b> includes damage to the goods that is beyond
                                        repair.
                                    </p>
                                </td>
                                <td colspan="2" valign="top" class="allborders">
                                    <b>SPECIAL CONDITIONS</b>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

			<br class="pageBreak"/>

            <table border="0">
                <tr valign="top">
                    <td colspan="4">
                        <b>PAYMENTS</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td>1.</td>
                    <td colspan="3">
                        The <b>Customer</b> will pay to <b>Lucky Dollar</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(a)</td>
                    <td colspan="2">
                        on the signing of this Agreement the above deposit as an initial
                        payment.
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(b)</td>
                    <td colspan="2">
                        Thereafter punctually and without further demand the above
                        installment commencing one month after the delivery of the good by Lucky Dollar
                        to the Customer, such payment to be made to Lucky Dollar at any of their stores
                        in the island of Trinidad and Tobago.
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>EXPENSES</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(c)</td>
                    <td colspan="2">
                        on demand, any expense incurred by <b>Lucky Dollar</b> in
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td></td>
                    <td>(i)</td>
                    <td>
                        determining the whereabouts of the <b>Customer</b>, or of the goods, or
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td></td>
                    <td>(ii)</td>
                    <td>
                        recovering possession of the goods from the <b>Customer</b> or from any other
                        person (including any payment made by <b>Lucky Dollar</b> to discharge or
                        satisfy any lien or claim to the goods) or
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td></td>
                    <td>(iii)</td>
                    <td>
                        enforcing payment of any instalment or any other sum payable under this
                        Agreement including any legal charges, incurred by Lucky Dollar whether legal
                        proceedings have been instituted or not, as well as Lucky Dollar costs and
                        Bailiff’s fees, and any amount due to Lucky Dollar pursuant to Clause 2 (c)
                        below.
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>OWNERSHIP</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td>2.</td>
                    <td>(a)</td>
                    <td colspan="2">
                        The customer may at any time pay to <b>Lucky Dollar</b> the full
                        agreement price set out in the Schedule to the Agreement along with any payment
                        due under Clause (1) above, at which time the goods will become the property of
                        the Customer.
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>REBATES</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(b)</td>
                    <td colspan="2">
                        Where the full balance of the Agreement price is paid to <b>
                            Lucky
                            Dollar
                        </b> not less than one month before it is due a rebate in the price
                        of the goods shall be allowed by <b>Lucky Dollar</b> to the <b>Customer</b> which will be automatically calculated on the system.
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>FAILURE TO PAY ON TIME</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(c)</td>
                    <td colspan="2">
                        Where the full balance of the Agreement Price remains unpaid for
                        more than one month after the date on which it is due, <b>Lucky Dollar</b> may
                        charge interest on the unpaid sum at the rate at 5% per annum or as stated
                        overleaf calculated from the date the amount falls due until the date of
                        payment
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(d)</td>
                    <td colspan="2">
                        Where any instalment of the Agreement Price remains unpaid after
                        the date on which it is due <b>Lucky Dollar</b> shall have the right to charge
                        interest at the rate of interest as stated overleaf in this Agreement. This
                        interest shall be calculated on a daily basis from the due date until the
                        monthly instalment is received by <b>Lucky Dollar</b>.
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>CARE OF THE GOODS</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(e)</td>
                    <td colspan="2">
                        Until the full payment is made to <b>Lucky Dollar</b>, the <b>Customer</b>
                        is responsible for maintaining the goods specified in the schedule, in good
                        condition and
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(f)</td>
                    <td colspan="2">
                        shall be responsible for all loss or damage however occasioned
                        (fair wear and tear expected)
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>INSURANCE (OPTIONAL)</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td>3.</td>
                    <td colspan="3">
                        A proportion of the Total Agreement relates to the purchase of
                        insurance by the customer with Guardian General Limited under
                        Policy No.CPI 50000247 under which all outstanding payments will be paid by the
                        insurers Unicomer (Trinidad) Limited as Trustee for the customer. The for
                        relevant percentage is as allows:
                        <table>
                            <tr>
                                <td>
                                    <b>Plan 1</b>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Plan 2</b>
                                </td>
                            </tr>
                        </table>
                        The settlement is subjected to the terms and conditions of the policy, full
                        details of which are contained in the Protection Plan Certificate.
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>ADDRESS OF THE GOODS</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td>4.</td>
                    <td colspan="3">
                        The Customer shall keep the goods at the address set out in this
                        Agreement and shall not allow them to be removed without first informing <b>
                            Lucky
                            Dollar
                        </b> in writing.
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>TERMINATION &amp; REPOSSESSION</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td>5.</td>
                    <td colspan="3">
                        On termination of this Agreement for any reason whatsoever, the
                        Customer agrees that he/she will make the goods available to <b>Lucky Dollar</b>
                        for repossession.  Lucky Dollar will not repossess from customer unless a 21-day written notice is forwarded to same advising of our intention to repossess.
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>LAPSED AGREEMENT</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td>6.</td>
                    <td colspan="3">
                        No leniency or waiver shown to the <b>Customer</b> shall prejudice
                        the strict rights of <b>Lucky Dollar</b> under this Agreement.
                    </td>
                </tr>
                <tr valign="top">
                    <td>7.</td>
                    <td colspan="3">
                        If the Customer has not taken delivery of the goods for a period of
                        60 days after this Agreement is made, the Agreement will be of no effect and
                        the <b>Customer</b> will then be entitled to the refund of any instalment paid
                        to <b>Lucky Dollar</b>.
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>NOTICE</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>CUSTOMER’S RIGHTS TERMINATION</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td>1.</td>
                    <td colspan="3">
                        The Customer may put an end to the Agreement by giving notice of
                        termination in writing to <b>Lucky Dollar</b>.  I fully accept that in returning item(s) does not prevent Lucky Dollar from pursuing it’s right and/or remedies to recover any balance outstanding.  Pursuant to this agreement this surrender does not release the customer from any liability to Lucky Dollar for any damage upon and as at the date of termination.
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>AGREED MINIMUM PAYMENT</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td>2.</td>
                    <td colspan="3">
                        The <b>Customer</b> must then pay any instalments, which are in
                        arrears at the time when he/she gives notice. If, when he/she has paid those
                        instalments, the total amount paid under the Agreement is less than the Agreed
                        minimum payment overleaf he/she must also pay enough to make up that sum,
                        unless the Lucky Dollar determines that a smaller sum would be equal to <b>
                            Lucky
                            Dollar’s
                        </b> loss.
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>DAMAGE TO GOODS</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td>3.</td>
                    <td colspan="3">
                        If the goods have been damaged owing to the <b>Customer</b> having
                        failed to take reasonable care of them, <b>Lucky Dollar</b> may sue him/her for
                        the amount of the damage unless that amount can be agreed between the <b>Customer</b>
                        and <b>Lucky Dollar</b>.
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>GENERAL PROVISION</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td>4.</td>
                    <td colspan="3">
                        Unless the <b>Customer</b> has himself/herself put an end to the
                        Agreement, <b>Lucky Dollar</b> cannot take back the goods from the Customer
                        without the <b>Customer’s</b> consent unless <b>Lucky Dollar</b> complies with
                        the requirement Hire Purchase Act Chap 82.33 Additionally after a total of 70% of the total hire purchase agreement is paid Lucky Dollar cannot repossess item(s) unless a court order is obtained and presented.
                    </td>
                </tr>
                <tr valign="top">
                    <td>5.</td>
                    <td colspan="3">
                        For agreement exceeding a total hire purchase of $15,000 Lucky can without any notice take possession of the goods and shall be entitled to freely enter into and upon
                        Any premises recognized by this agreement to exercise out right to repossess.

                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="4">
                        <b>LUCKY DOLLAR RIGHTS</b>
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td colspan="3">
                        The <b>Customer</b> shall have quiet possession of the goods and <b>
                            Lucky
                            Dollar
                        </b> may not repossess the goods except:-
                    </td>
                </tr>
                <tr valign="top">
                    <td>1.</td>
                    <td colspan="3">
                        An instalment is overdue for the period prescribed by the Hire
                        Purchase Act and <b>Lucky Dollar</b> has served the <b>Customer</b> the
                        prescribed notice under the Hire Purchase Act.
                    </td>
                </tr>
                <tr valign="top">
                    <td>2.</td>
                    <td colspan="3">
                        The <b>Customer</b> disposes of or attempts to dispose of the
                        goods.
                    </td>
                </tr>
                <tr valign="top">
                    <td>3.</td>
                    <td colspan="3">
                        The <b>Customer</b> terminates the Agreement. <b>
                            Prescribed period
                            means:
                        </b>
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(a)</td>
                    <td colspan="2">
                        Where 2/3 of the total agreement price has been paid- 3 (three)
                        months.
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(b)</td>
                    <td colspan="2">
                        Where less than 2/3 of the total agreement price has been paid -2
                        (two) months.
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(c)</td>
                    <td colspan="2">
                        Where there has been a previous default and the prescribed notices
                        have been given - 1 (one) month.
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td colspan="3">
                        <b>The prescribed notices are: </b>
                    </td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(a)</td>
                    <td colspan="2">Notice of Default; and</td>
                </tr>
                <tr valign="top">
                    <td></td>
                    <td>(b)</td>
                    <td colspan="2">Notice of Repossession</td>
                </tr>
                <tr>
                    <td colspan="4">
                        <b>NOTE:</b> If <b>Lucky Dollar</b> serves the <b>Customer</b> a
                        Notice of Default and within the time specified in the Notice (usually 21 Days), Lucky Dollar will accept any amount of monies paid without prejudice.
                    </td>
                </tr>
            </table>


            <table>
                <tr>
                    <td>
                        <table style="border:1px solid black" height="180px">
                            <tr>
                                <td>
                                    This Agreement is subject to verification of customer information and final
                                    credit approval by Lucky Dollar. Delivery of goods will be made after final
                                    approval is granted.
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    I/We have been informed of the regular price of the goods before entering into
                                    this Agreement and understand the contents of the Agreement and Schedule.
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <nobr>Signature of Customer (1) ______________________________________________</nobr>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <nobr>Signature of Customer (2) ______________________________________________</nobr>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <nobr>Witness to Customer’s Signature _________________________________________</nobr>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <nobr>Name ______________________________________________________________</nobr>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <nobr>Address ____________________________________________________________</nobr>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <nobr>For and on behalf of Lucky Dollar ________________________________________</nobr>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <table style="border:1px solid black" height="180px">
                            <tr>
                                <td>GUARANTEE (Where required by Lucky Dollar)</td>
                            </tr>
                            <tr>
                                <td>
                                    In consideration of Lucky Dollar making this Agreement with the Customer as per
                                    my request, hereby guarantee the due performance of all the Customer’s
                                    obligations and will be jointly and severally liable to Lucky Dollar under this
                                    Agreement which I have read and understood.
                                </td>
                            </tr>
                            <tr>
                                <td>Dated the day of 20</td>
                            </tr>
                            <tr>
                                <td>
                                    <nobr>Signature_______________________________________________________</nobr>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <nobr>Signature of Witness______________________________________________</nobr>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <nobr>Address_________________________________________________________</nobr>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <!-- end new page template -->


            <xsl:variable name="lastPage" select="LAST" />
            <xsl:variable name="lastAgreement" select="../LAST" />
            <xsl:if test="$lastPage = 'False' or $lastAgreement = 'FALSE' ">
                <br class="pageBreak" />
                <!-- if it's not the last page -->
            </xsl:if>
        </div>
    </xsl:template>

    <xsl:template match="LINEITEMS">
        <table width="100%" border="0" style="height:7cm;border-collapse:collapse;border: 1px solid black;">
            <tr height="20px">
                <td class="allborders" align="center">
                    <b>QTY.</b>
                </td>
                <td colspan="2" class="allborders" align="center">
                    <b>DESCRIPTION OF GOODS</b>
                </td>
                <td class="allborders" align="center">
                    <b>PRICE</b>
                </td>
            </tr>
            <xsl:apply-templates select="LINEITEM" />
        </table>
    </xsl:template>



    <xsl:template match="LINEITEM">

        <xsl:variable name="addTo" select="ADDTO" />
        <xsl:if test="$addTo = 'True'">
            <tr>
                <td class="verticalborders">
                    <xsl:value-of select="QUANTITY"/>
                </td>
                <td colspan="2" class="verticalborders">
                    <xsl:value-of select="DESC" />
                    (<xsl:value-of select="ACCTNO" />)
                </td>
                <td class="verticalborders"></td>
            </tr>


        </xsl:if>
        <xsl:if test="$addTo != 'True'">
            <tr>
                <td class="verticalborders">
                    <xsl:value-of select="QUANTITY"/>
                </td>
                <td colspan="2" class="verticalborders">
                    <xsl:value-of select="DESC"/>
                </td>
                <td class="verticalborders">
                    <xsl:value-of select="VALUE"/>
                </td>
            </tr>
            <xsl:if test="TRIM != '' and TRIM != ' '">
                <tr>
                    <td class="verticalborders"></td>
                    <td colspan="2" class="verticalborders">
                        <xsl:value-of select="TRIM" />
                    </td>
                    <td class="verticalborders">
                    </td>
                </tr>
            </xsl:if>
            <xsl:if test="DESC2 != '' and DESC2 != ' '">
                <tr>
                    <td class="verticalborders"></td>
                    <td colspan="2" class="verticalborders">
                        <xsl:value-of select="DESC2" />
                    </td>
                    <td class="verticalborders">
                    </td>
                </tr>
            </xsl:if>

        </xsl:if>
        <tr>
            <td class="verticalborders">
                <br/>
            </td>
            <td colspan="2" class="verticalborders">

            </td>
            <td class="verticalborders">
            </td>
        </tr>
    </xsl:template>

</xsl:stylesheet>




