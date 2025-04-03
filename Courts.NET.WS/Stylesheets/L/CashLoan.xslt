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
          <table width="100%">
            <tr>
              <td align="left">
                <b>PROMISSORY NOTE</b>
              </td>
              <td align="right">
                <b>
                  ACCOUNT NUMBER <xsl:value-of select="HEADER/ACCTNO" />
                </b>
              </td>
            </tr>
          </table>
          <br/>
        </tr>
        <tr>
          <td>To: Courts ( St. Lucia ) Limited (hereinafter referred to as the Lender)</td>
        </tr>
        <br/>
        <br/>
        <tr>
          <td>
            From: <xsl:value-of select="HEADER/NAME" /> of <xsl:value-of select="HEADER/ADDR1" /> <xsl:value-of select="HEADER/ADDR2" />(hereinafter referred to as the Borrower)
          </td>
        </tr>
        <br/>
        <br/>
        <tr>
          <td>
            For value received the undersigned jointly and severally (if more than one), promise to pay Courts ( St. Lucia ) Limited, (the Company) the sum of <xsl:value-of select="../FOOTER/TOTAL" /> dollars, including interests payable by installments as follows:
          </td>
        </tr>
        <br/>
        <br/>
        <tr>
          <td>
            <table border="1" style="border-collapse:collapse;" width="100%">
              <tr valign="top">
                <td width="50%">
                  Installment Amount<br/>
                </td>
                <td width="50%">
                  Date<br/>
                </td>
              </tr>
              <br/>
              <tr>
                <xsl:for-each select="../FOOTER/INSTALTABLE">
                  <tr>
                    <td>
                      <xsl:value-of select="INSTALMENTAMOUNT"/>
                    </td>
                    <td>
                      <xsl:value-of select="INSTALMENTDATE"/>
                    </td>
                  </tr>
                </xsl:for-each>
              </tr>
            </table>
          </td>
        </tr>
        <br/>
        <br/>
        <tr>
          <td>
            I hereby agree to make all payments in full as set in the above schedule until the principal, application fee  (if applicable) and all interest accrued thereon is paid in full.
          </td>
        </tr>
        <br/>
        <br/>
        <tr>
          <td>
            In the event that a payment is overdue, late fees will be assessed. In the event of  failure to pay as scheduled, I agree to pay any costs incurred in the collection of my debt, including but not limited to legal fees, bailiff fees and other collection expenses.
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
            <table width="100%">
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
            <table width="100%">
              <tr>
                <td width="50%">_______________________________</td>
                <td width="50%">_______________________________</td>
              </tr>
              <tr>
                <td>Witness (on behalf of Courts ( St. Lucia ) Limited)</td>
                <td>Date</td>
              </tr>
            </table>
          </td>
        </tr>
        <br class="pageBreak" />

        <tr>
          <td>
            <table>
              <tr>
                <td align="center">
                  <b>CONSUMER CREDIT DISCLOSURE and PROMISSORY NOTE</b>
                </td>
              </tr>
              <br/>
              <br/>
              <br/>
              <tr>
                <td>
                  <b>1.  Promise to Pay.</b>  On demand for value received, I promise to pay the Total Payments specified in this agreement to you the Lender. All payments will be made at the address of the company or such other place as the Lender may specify beginning <xsl:value-of select="../FOOTER/INSTALTABLE/INSTALMENTDATE"/> until the entire credit and related charges are paid as shown in the Payment Schedule.
                </td>
              </tr>
              <tr>
                <td>
                  <b>2.  Late Charge.</b>   If a payment that is due is unpaid after the due date a late charge will be attached which will amount to a percentage of the scheduled payment.
                </td>
              </tr>
              <tr>
                <td>
                  <b>3.  Dishonored Cheques.</b>   If a cheque is returned a fee for the returned cheque will be added to the amount owed and will be collected with the regular monthly payment or it may be collected separately.
                </td>
              </tr>
              <tr>
                <td>
                  <b>4.  Deferrals.</b>  If due to circumstances the customer requests an extension of time to make any of the scheduled payments and this extension is agreed in writing by the company, additional interest shall be charged for the extension.
                </td>
              </tr>
              <tr>
                <td>
                  <b>5.  Prepayment.</b>  There will be no rebate of interest in the event that the payments are made early and the next payment shall be due as scheduled. If the entire loan including interest is paid before the final payment is due, a portion of the Finance Charge will be calculated and reimbursed to the Borrower and there will be no penalty attached for early payment.
                </td>
              </tr>
              <tr>
                <td>
                  <b>6.  Default.</b>  If the payments are not made on or before the due dates for payment and the loan falls in default the Borrower may be required to pay the unpaid principal balance and any accrued interest at once. Notice to pay all of the unpaid balance and interest shall be forwarded to the Borrower’s last known address within ten days of the default.
                </td>
              </tr>
              <tr>
                <td>
                  <b>7.  Collection Expense.</b>  If the loan and interest is referred to an attorney for collection, the Borrower shall pay all attorney fees as set by the court plus court costs. If the loan and interest is referred to a collection agency the Borrower shall pay any fees due to such agent.
                </td>
              </tr>
              <tr>
                <td>
                  <b>8.  Statement of Truthful Information.</b>  The Borrower warrants that all information given to the Lender is true.
                </td>
              </tr>
              <tr>
                <td>
                  <b>9.  Joint Liability.</b>  If there is more than one Borrower each Borrower agrees to keep all of the promises in the loan documents. The Lender is not obligated to seek payment from any of the other Borrower/s without first seeking repayment from the principal Borrower.
                </td>
              </tr>
              <tr>
                <td>
                  <b>10.  Savings Clause.</b>  If any part of this contract is declared invalid, the rest of the contract remains valid.
                </td>
              </tr>
              <tr>
                <td>
                  <b>11.  Final Agreement and Modifications in Writing.</b>  This written loan agreement is the final agreement between the Borrower (and any joint Borrower) and the Lender and cannot be changed by prior, current, or future Notes.  Any change to this Promissory Note must be made in writing by an authorized signatory of the Lender. Any amendments must be signed to by both Parties.
                </td>
              </tr>
              <tr>
                <td>
                  <b>12.  Mailing Notice to Borrower.</b>   Notice shall be mailed by registered post to the last known address on record of the Borrower.
                </td>
              </tr>
            </table>
          </td>
        </tr>
        <br/>
        <br/>
        <tr>
          <td>
            I also authorize and instruct <xsl:value-of select="HEADER/EMPLOYER" /> &#160; <xsl:value-of select="HEADER/EMPYADD1" /> &#160; <xsl:value-of select="HEADER/EMPYADD2" /> &#160; <xsl:value-of select="HEADER/EMPYADD3" /> &#160; <xsl:value-of select="HEADER/EMPYPOCODE" /> and the Employer hereby agrees that on the receipt of the signed  Note by <xsl:value-of select="HEADER/NAME" /> (the Employee) to deduct the sum of <xsl:value-of select="../FOOTER/TOTAL" /> from the salary of <xsl:value-of select="HEADER/NAME" /> (the Employee) for remittance to Courts ( St. Lucia ) LIMITED in <xsl:value-of select="../FOOTER/INSTALNO"/> payments COMMENCING <xsl:value-of select="../FOOTER/INSTALTABLE/INSTALMENTDATE"/>, until the entire Note and related charges( if any) are paid off to Courts ( St. Lucia ) LIMITED
          </td>
        </tr>
        <br/>
        <br/>
        <tr>
          <td>
            <table width="100%">
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
            <table width="100%">
              <tr>
                <td width="50%">_______________________________</td>
                <td width="50%">_______________________________</td>
              </tr>
              <tr>
                <td>Witness (on behalf of Courts ( St. Lucia ) Limited)</td>
                <td>Date</td>
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
</xsl:stylesheet>


