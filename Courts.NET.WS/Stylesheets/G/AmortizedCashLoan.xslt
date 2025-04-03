<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">


    <html>
      <head>
        <style type="text/css" media="all">
          @import url(styles.css);
          body,table{font-family:Helvetica; font-size:12}
        </style>
      </head>
      <body>
        <xsl:apply-templates select="AGREEMENTS" />
      </body>
    </html>


  </xsl:template>


  <xsl:template match="AGREEMENTS">
    <xsl:apply-templates select="AGREEMENT" />

  </xsl:template>

  <xsl:template match="AGREEMENT">
    <xsl:apply-templates select="PAGE" />

  </xsl:template>


  <xsl:template match="PAGE">
    <div style="position:relative;">

      <!-- start new page template -->

      <table width="100%" style="font-size = 1.0em; margin: 0px; padding: 0px;" >
        <tr>
          <table width="100%" style="font-size = 1.0em; margin: 0px; padding: 0px;" >
            <tr>
              <td width ="35%" align="left">
                <b>PROMISSORY NOTE</b>
              </td>
              <!--<td width ="30%" align="center">
                <img src="StampDuty.jpg" width="70" height="70"/>
              </td>-->
              <td width ="35%" align="right">
                <b>
                  ACCOUNT NUMBER <xsl:value-of select="HEADER/ACCTNO" />
                </b>
              </td>
            </tr>
          </table>
        </tr>
        <tr>
          <td>To: UNICOMER ( Belize ) Limited (hereinafter referred to as the Lender)</td>
        </tr>
        <br/>
        <br/>
        <tr>
          <td>
            From: <xsl:value-of select="HEADER/NAME" /> (hereinafter referred to as the Borrower)
          </td>
        </tr>
        <br/>
        <br/>
        <tr>
          <td>
            For cash value received of <xsl:value-of select="../FOOTER/TOTAL" /> the undersigned jointly and severally (if more than one), promise to pay UNICOMER ( Belize ) Limited (the Company), the sum of <xsl:value-of select="../FOOTER/AMORTIZEDTOTAL" />, including interest of <xsl:value-of select="../FOOTER/MONTHLYINTERESTRATE" /> per month/<xsl:value-of select="../FOOTER/ANNUALINTERESTRATE" /> per annum on an amortizede basis and Administration/Processing Fee of <xsl:value-of select="../FOOTER/MONTHLYADMINRATE" /> per month/<xsl:value-of select="../FOOTER/ANNUALADMINRATE" /> per annum payable by installments as follows:
        </td>
        </tr>
        <br/>
        <tr>
          <td>
            <table border="1" style="border-collapse:collapse; margin: 0px; padding: 0px;" width="100%" >
              <tr valign="top">
                <td width="10%">
                  Pmt. No.<br/>
                </td>
                <td width="10%">
                  Payment Date<br/>
                </td>
                <td width="10%">
                  Beginning Balance<br/>
                </td>
                <td width="10%">
                  Scheduled Payment<br/>
                </td>
                <td width="10%">
                  Admin Payment<br/>
                </td>
                <td width="10%">
                  Total Payment<br/>
                </td>
                <td width="10%">
                  Principal<br/>
                </td>
                <td width="10%">
                  Interest<br/>
                </td>
                <td width="10%">
                  Ending Balance<br/>
                </td>
                <td width="10%">
                  Cumulative Interest<br/>
                </td>
              </tr>
              <br/>
              <tr>
                <xsl:for-each select="../FOOTER/INSTALTABLE">
                  <tr>
                    <td>
                      <xsl:value-of select="PAYMENTNUM"/>
                    </td>
                    <td>
                      <xsl:value-of select="INSTALDATE"/>
                    </td>
                    <td>
                      <xsl:value-of select="OPENINGBAL"/>
                    </td>
                    <td>
                      <xsl:value-of select="INSTALAMT"/>
                    </td>
                    <td>
                      <xsl:value-of select="MONTHLYADMINAMT"/>
                    </td>
                    <td>
                      <xsl:value-of select="TOTALAMT"/>
                    </td>
                    <td>
                      <xsl:value-of select="PRINCIPAL"/>
                    </td>
                    <td>
                      <xsl:value-of select="INTEREST"/>
                    </td>
                    <td>
                      <xsl:value-of select="CLOSINGBAL"/>
                    </td>
                    <td>
                      <xsl:value-of select="CUMINT"/>
                    </td>
                  </tr>
                </xsl:for-each>
              </tr>
            </table>
          </td>
        </tr>
        <br/>
        <tr>
          <td>
            I hereby agree to make all payments in full as set in the above schedule until the principal, application fee  (if applicable) and all interest accrued thereon is paid in full. In the event that a payment is overdue, late fees and interest will be assessed. In the event of  failure to pay as scheduled, I agree to pay any costs incurred in the collection of my debt, including but not limited to legal fees, bailiff fees and other collection expenses.
          </td>
        </tr>
        <br/>
        <br/>
        <tr>
          <td>
            I have read and understood and agree to abide by the terms and conditions on this Promissory Note.
          </td>
        </tr>
        <br/>
        <br/>
        <tr>
          <td>
            <table width="100%" style="margin: 0px; padding: 0px;">
              <tr>
                <td width="50%">_______________________________</td>
                <td width="50%">_______________________________</td>
              </tr>
              <tr>
                <td>Borrower's Signature</td>
                <td>Date</td>
              </tr>
            </table>
          </td>
        </tr>
        <br/>
        <tr>
          <td>
            <table width="100%" style="margin: 0px; padding: 0px;">
              <tr>
                <td width="50%">_______________________________</td>
                <td width="50%">_______________________________</td>
              </tr>
              <tr>
                <td>Witness (on behalf of UNICOMER ( Belize ) Limited)</td>
                <td>Date</td>
              </tr>
            </table>
          </td>
        </tr>

        <tr>
          <td>
            <table width="100%" style="font-size = 0.75em; margin: 0px; padding: 0px;" >
              <tr>
                <td align="center">
                  <b>CONSUMER CREDIT DISCLOSURE and PROMISSORY NOTE</b>
                </td>
              </tr>
              <br/>
              <tr>
                <td>
                  <b>1.  Promise to Pay:</b>  On demand for value received, I promise to pay the Total Payments specified in this agreement to the Lender. All payments will be made at the address of the company or such other place as the Lender may specify beginning <xsl:value-of select="../FOOTER/INSTALTABLE/INSTALDATE"/> until the entire credit and related charges are paid as shown in the Payment Schedule.
                </td>
              </tr>
              <tr>
                <td>
                  <b>2.  Late Charge:</b>   If a payment that is due is unpaid after the due date a late charge will be attached which will amount to a percentage of the scheduled payment.
                </td>
              </tr>
              <tr>
                <td>
                  <b>3.  Dishonored Cheques:</b>   If a cheque is returned a fee for the returned cheque will be added to the amount owed and will be collected with the regular monthly payment or it may be collected separately.
                </td>
              </tr>
              <tr>
                <td>
                  <b>4.  Deferrals:</b>  If due to circumstances the customer requests an extension of time to make any of the scheduled payments and this extension is agreed in writing by the company, additional interest shall be charged for the extension.
                </td>
              </tr>
              <tr>
                <td>
                  <b>5.  Prepayment:</b>  There shall be no rebate of interest in the event that the payments are made early and the next payment shall be due as scheduled. If the entire loan including interest is paid before the final payment is due, a portion of the Finance Charge will be calculated and reimbursed to the Borrower and there will be no penalty attached for early payment.
                </td>
              </tr>
              <tr>
                <td>
                  <b>6.  Default:</b>  If the payments are not made on or before the due dates for payment and the loan falls in default the Borrower may be required to pay the unpaid principal balance and any accrued interest at once. Notice to pay all of the unpaid balance and interest shall be forwarded to the Borrower’s last known address within ten days of the default.
                </td>
              </tr>
              <tr>
                <td>
                  <b>7.  Collection Expense:</b>  If the loan and interest is referred to an attorney for collection, the Borrower shall pay all attorney fees as set by the court plus court costs. If the loan and interest is referred to a collection agency the Borrower shall pay any fees due to such agent.
                </td>
              </tr>
              <tr>
                <td>
                  <b>8.  Statement of Truthful Information:</b>  The Borrower warrants that all information given to the Lender is true.
                </td>
              </tr>
              <tr>
                <td>
                  <b>9.  Joint Liability:</b>  If there is more than one Borrower each Borrower agrees to keep all of the promises in the loan documents. The Lender is not obligated to seek payment from any of the other Borrower/s without first seeking repayment from the principal Borrower.
                </td>
              </tr>
              <tr>
                <td>
                  <b>10.  Savings Clause:</b>  If at any time any of the provisions of this contract and Promissory Note is or becomes illegal, invalid or unenforceable in any respect under any law or regulation of any jurisdiction, neither the legality, validity and enforceability of the remaining provisions of this contract and Promissory Note nor the legality, validity or enforceability of such provision under the law of any other jurisdiction shall be in any way affected or impaired as a result.
                </td>
              </tr>
              <tr>
                <td>
                  <b>11.  Illegality:</b>  If at any time it is unlawful or contrary to any request from or requirement of any central bank or other fiscal monetary or governmental or regulatory authority, for the Lender to make, fund or allow to remain outstanding all or any part of the loan on the existing terms and conditions in the Contract and Promissory Note, then the Lender will promptly deliver to the Borrower a certificate to that effect  and (i) the Lender shall not thereafter be obliged to advance any further monies under the contract or Promissory Note or be obliged to allow to remain outstanding all or any part of the loan on the existing terms and conditions and (ii) if the Lender so require, the Borrower shall on such date as the Lender specifies, not being a date earlier than thirty (30) days, repay the Loan together with accrued interest on it and any other amounts due to it hereunder.
                </td>
              </tr>
              <tr>
                <td>
                  <b>12.  Assignment/Transfer:</b>   The Borrower may not assign or transfer any of its rights or obligations under this contract and Promissory Note. The Lender may at any time assign, sell, novate, securitise, sell participations or otherwise transfer all or any of its rights and obligations under this contract and Promissory Note or other related document and the Borrower shall enter into all documents specified by the Lender to be necessary to give effect to any such assignment or transfer. The Lender may upon giving written notice to the Borrower change its lending office at any time.
                </td>
              </tr>
              <tr>
                <td>
                  <b>13.  Final Agreement and Modifications in Writing:</b>   This written loan agreement is the final agreement between the Borrower (and any joint Borrower) and the Lender and replaces any and all prior agreements, negotiations and arrangements between the Borrower and the Lender.  Any change and/or modifications including deletions to this Promissory Note must be made in writing upon agreement by both parties. Amendments and/or alterations to this Promissory Note that become necessary due to a change in governing legislation, court order or other orders and/or directions by any regulatory or governmental authority shall be immediately binding upon both Parties and such amendments shall be made to the Promissory Note by the Lender and the amended Promissory Note signed by both Parties.
                </td>
              </tr>
              <tr>
                <td>
                  <b>14.  Mailing Notice to Borrower:</b>   Notice shall be mailed by registered post to the last known address on record of the Borrower.
                </td>
              </tr>
              <tr>
                <td>
                  <b>15. Third Party Disclosure:</b>   The Borrower (and any joint Borrower) authorizes and consents to the Lender receiving and exchanging any financial and other information which it may have in its possession about the Borrower (and any joint Borrower) with third party credit institutions with whom the Borrower (and any joint Borrower) may have or purpose to have financial dealings from time to time.  The Borrower (and any joint Borrower) indemnifies the Lender against any loss, claims, damages, liabilities, actions and proceedings, legal or other expense which may be directly and reasonably incurred as a consequence of the disclosure of the financial and other information.
                </td>
              </tr>
            </table>
          </td>
        </tr>
      </table>

      <!-- end new page template -->
    </div>
    <xsl:variable name="lastPage" select="LAST" />
    <xsl:variable name="lastAgreement" select="../LAST" />
    <xsl:if test="$lastPage = 'False' or $lastAgreement = 'FALSE' ">
      <br class="pageBreak" />
      <!-- if it's not the last page -->
    </xsl:if>

  </xsl:template>
</xsl:stylesheet>


