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
			<tr><td align="center"><b>CONSUMER CREDIT DISCLOSURE and PROMISSORY NOTE</b></td></tr>
			<tr>
				<td>
					<table border="1" style="border-collapse:collapse;" width="100%">
						<tr>
							<td width="48%">ACCOUNT/CONTRACT NO. <xsl:value-of select="HEADER/ACCTNO" /></td>
							<td width="4%"></td>
							<td width="48%">DATE OF NOTE</td>
						</tr>
						<tr>
							<td>CREDITOR / LENDER</td>
							<td></td>
							<td>BORROWER <xsl:value-of select="HEADER/NAME" /></td>
						</tr>
						<tr>
							<td>
                ADDRESS <xsl:value-of select="HEADER/ADDR1" />
              </td>
							<td></td>
							<td>
                ADDRESS <xsl:value-of select="HEADER/ADDR2" />
              </td>
						</tr>
						<tr>
							<td></td>
							<td></td>
							<td></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr><td>"I" and "me" and similar words mean each person who signs as a Borrower. You and your and similar words mean the Lender.</td></tr>
			<tr>
				<td>
					<table border="1" style="border-collapse:collapse;" width="100%">
						<tr valign="top">
							<td width="33.33%">
								FINANCE CHARGE<br/>
								The dollar amount the credit will cost me<br/>
								<xsl:value-of select="../FOOTER/PRETAXDT" />
							</td>
							<td width="33.33%">
								Amount Financed<br/>
								The amount of credit provided to me or on my behalf.<br/>
								<xsl:value-of select="../FOOTER/PRETAXGOODSVAL" />
							</td>
							<td width="33.33%">
								
								Total of Payments<br/>  
								The amount I will have paid after I have made all payments as scheduled.<br/>
								<xsl:value-of select="../FOOTER/TOTAL" />
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td>
					My Payment Schedule will be: 
					<table border="1" style="border-collapse:collapse;" width="100%">
						<tr>
							<td>Number of payments</td>
							<td>Amount of payments</td>
							<td>When Payments Are Due</td>
						</tr>
						<tr>
							<td><xsl:value-of select="../FOOTER/INSTALNO" /></td>
							<td><xsl:value-of select="../FOOTER/FIRSTINST" /></td>
							<td></td>
						</tr>	
						<tr>
							<td>1</td>
							<td><xsl:value-of select="../FOOTER/FINALINST" /></td>
							<td></td>
						</tr>				
					</table>
				</td>
			</tr>
      <tr>
        <td>
          <table>
            <tr>
              <td align="center">
                <b>PROMISSORY NOTE</b>
              </td>
            </tr>
            <tr>
              <td>
                <b>1.Promise to Pay.</b>  I promise to pay the Total of Payments and any Prepaid Finance Charge to the order of you, the Lender.  I will make the payments at your address above. I will make the payments on the dates and in the amounts shown in the Payment Schedule.
              </td>
            </tr>
						<tr><td><b>2.  Late Charge.</b>   If I don't pay all of a payment within 10 days after it is due, you can charge me a late charge.  The late charge will be 5% of the scheduled payment.</td></tr>
						<tr><td><b>3.  Dishonored Checks.</b>   I agree to pay you a fee of up to $30 for a returned check.  You can add the fee to the amount I owe or collect it separately.</td></tr>
						<tr><td><b>4.  Deferrals.</b>  If I ask for more time to make any payment and you agree, I will pay more interest to extend the payment.  The extra interest will be figured under the Finance       Commission rules.</td></tr>
						<tr><td><b>5.  Prepayment.</b>  I can make a whole payment early.  Unless you agree otherwise in writing, I may not skip payments.  If I make a payment early, my next payment will still   be due as scheduled.   If I prepay my loan in full before the final payment is due, I may save a portion of the Finance Charge and I will not have to pay a penalty.</td></tr>
						<tr><td><b>6.  Security Agreement.</b>  If collateral is given for this loan, I will see the separate security agreement for more information and agreements.</td></tr>
						<tr><td><b>7.  Default.</b>  I will be in default if:  I do not timely make a payment; I break any promise I made in this agreement; I allow a judgment to be entered against me or the     collateral; I sell, lease, or dispose of the collateral; I use the collateral for an illegal purpose; or you believe in good faith that I am not going to keep any of my promises.</td></tr>
						<tr><td><b>8.  Waiver of Notice of Intent to Accelerate/Waiver of Notice of Acceleration.</b>  If I am in default, you may require me to repay the entire unpaid principal balance, and any accrued interest at once.  You don't have to give me notice that you are demanding or intend to demand immediate payment of all that I owe.  If you don't enforce your rights every time, you can still enforce them later.  If this debt is referred to an attorney for collection, I will pay any attorney fees set by the court plus court costs.</td></tr>
						<tr><td><b>9.  Collection Expense.</b>  If this debt is referred to an attorney for collection, I will pay any attorney fees set by the court plus court costs.</td></tr>
						<tr><td><b>10.  Statement of Truthful Information.</b>  I promise that all information I gave you is true.</td></tr>
						<tr><td><b>11.  Joint Liability.</b>  If there is more than one Borrower, each Borrower agrees to keep all of the promises in the loan documents. I understand that you may seek payment    from only me without first looking to any other Borrower.</td></tr>
						<tr><td><b>12.  Usury Savings Clause.</b>  I don't have to pay interest or other amounts that are more than the law allows.</td></tr>
						<tr><td><b>13.  Savings Clause.</b>  If any part of this contract is declared invalid, the rest of the contract remains valid.</td></tr>
						<tr><td><b>14.  Final Agreement and Modifications in Writing.</b>  This written loan agreement is the final agreement between you and me and may not be changed by prior, current, or future agreements or statements between you and me.  There are no oral agreements between us relating to this loan agreement.  Any change to this agreement has to be in writing.  Both you and I have to sign it</td></tr>
						<tr><td><b>15.  Mailing Notice to Borrower.</b>  You can mail any notice to me at my last address in your records.  Your duty to give me notice will be satisfied when you mail it.</td></tr>
						<tr><td><b>16.  No Waiver of Lender's Rights.</b>  If you don't enforce your rights every time, you can still enforce them later.</td></tr>
						
					</table>
				</td>
			</tr>
			<tr>
				<td>
					This lender is licensed and examined by the State of Texas Office of Consumer Credit Commissioner.  Call the Consumer Credit Hotline or write for credit information or assistance with credit problems:  Office of Consumer Credit Commissioner, 2601 North Lamar Boulevard, Austin, Texas 78750-4207, www.occc.state.tx.us, (512) 936-7600  or (800) 538-1579
				</td>
			</tr>
			<tr>
				<td>
					<b>NOTICE TO BORROWER - DO NOT SIGN THIS AGREEMENT BEFORE READING IT OR IF IT CONTAINS BLANK SPACES.  BORROWER IS ENTITLED TO A COPY OF THE AGREEMENT HE SIGNS.  BORROWER SHOULD KEEP THIS AGREEMENT TO PROTECT HIS RIGHTS.</b>
				</td>
			</tr>
			<tr>
				<td>
					<b>I ACKNOWLEDGE THAT ON ____________________________, I READ, SIGNED, AND RECEIVED A COMPLETED COPY OF THIS PROMISSORY NOTE WITH ALL BLANKS COMPLETED.</b>
				</td>
			</tr>
      <tr>
        <td>
          <table border="1" style="border-collapse:collapse;" width="100%">
            <tr>
              <td width="50%" align="center">
                <table>
                  <tr>
                    <td>X_______________________________</td>
                  </tr>
                  <tr>
                    <td align="center">Borrower</td>
                  </tr>
                  <tr>
                    <td>X_______________________________</td>
                  </tr>
                  <tr>
                    <td align="center">Borrower</td>
                  </tr>
                </table>
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
</xsl:stylesheet>


	