<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <HTML>
      <head>
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"></meta>
        <style type="text/css" media="all">
            @import url(<xsl:value-of select="AGREEMENTS/@CSSPATH"/>	);
            body,table	{font-family:Times New Roman;font-size:12;}
            h4			{font-family:arial;}
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
    <xsl:variable name="last" select="LAST" />
    <xsl:if test="$last != 'TRUE'">
      <br class="pageBreak" />
    </xsl:if>
    <!-- only add a 2nd pagebeak if we have an odd number of pages (to provide us with a blank side) -->
    <xsl:variable name="InsertBlankPage" select="INSERTBLANKPAGE" />
    <xsl:if test=" $InsertBlankPage = 'TRUE' ">
      <br class="pageBreak" />
    </xsl:if>
    <P></P>
  </xsl:template>

  <xsl:template match="PAGE">
    <div style="position:relative">
      <!-- resets the position context -->
      <xsl:apply-templates select="HEADER" />
      <P></P>
      <xsl:apply-templates select="LINEITEMS" />
      <xsl:apply-templates select="../FOOTER" />
      <xsl:variable name="last" select="LAST" />
      <xsl:if test="$last != 'True'">
        <br class="pageBreak" />
      </xsl:if>
      <xsl:if test="$last = 'True'">
        <!--<div style="POSITION: absolute; TOP: 15cm">-->
        <xsl:call-template name="conditions" />
        <!--</div>-->
      </xsl:if>
    </div>
  </xsl:template>

  <xsl:template match="HEADER">
    <table width="700" border="0" cellspacing="10">
      <tr>
        <td>
          <img src="{//AGREEMENTS/@IMAGEPATH}radioshack.png"  height="50px" />
        </td>
      </tr>
      <tr>
        <td>
          <table width="100%">
            <tr>
              <td width="25%">
                <h4>ST. LUCIA</h4>
              </td>

              <td width="50%" align="center">
                <h4 style="color:red;">HIRE-PURCHASE AGREEMENT</h4>
              </td>
              <td width="25%">

              </td>
            </tr>
          </table>
        </td>
      </tr>
      <tr>
        <td >
          <table style="width:100%;">
            <tr>
              <td>
                ID:&#160;<xsl:value-of select="CUSTID"/>
              </td>
              <td align="right">
                Account Number: <xsl:value-of select="ACCTNO" />
                &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
                &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;
              </td>
            </tr>
          </table>


        </td>
      </tr>
      <tr>
        <td>
          On the <xsl:value-of select="DATE" /> <b> Unicomer Saint Lucia Limited, registered office, Chaussee Road, Castries St. Lucia</b>  ("the Owner") agrees to supply the goods described below
        </td>
      </tr>
      <tr>
        <td>
          on Hire Purchase to <xsl:value-of select="NAME" />, <xsl:value-of select="ADDR1" />, <xsl:value-of select="ADDR2" />, <xsl:value-of select="ADDR3" />, <xsl:value-of select="POSTCODE" /> ("the Hirer").
        </td>
      </tr>
      <xsl:variable name="jointName" select="JOINTNAME" />
      <xsl:if test="$jointName != ''" >
        <tr>
          <td>
            Joint Holder: <xsl:value-of select="JOINTNAME" />
          </td>
        </tr>
      </xsl:if>
    </table>
  </xsl:template>




  <xsl:template match="LINEITEMS">
    <table cellpadding="0" cellspacing="0" style="width:100%;">
      <tr>
        <td>

          <table cellpadding="2" width="100%" style="border:1px solid black;border-collapse:collapse;">
            <tr>
              <td align="center" style="border-right:1px solid black;border-bottom:1px solid black;">
                <b>Quantity</b>
              </td>
              <td align="center" style="border-right:1px solid black;border-bottom:1px solid black;">
                <b>Description of Goods</b>
              </td>
              <td align="center" style="border-bottom:1px solid black;width:100px;">
                <b>Price</b>
              </td>
            </tr>
            <xsl:apply-templates select="LINEITEM" />
          </table>
        </td>
      </tr>
    </table>

  </xsl:template>

  <xsl:template match="LINEITEM">
    <xsl:variable name="itemNo" select="ITEMNO" />
    <xsl:if test="$itemNo != '511700' and $itemNo != 'TRADE'" >

      <xsl:variable name="addTo" select="ADDTO" />
      <xsl:if test="$addTo = 'True'">
        <tr>
          <td style="border-right:1px solid black;">
            <xsl:value-of select="QUANTITY" />
          </td>
          <td style="border-right:1px solid black;padding-left:4px;">
            <xsl:value-of select="DESC" />
            (<xsl:value-of select="ACCTNO" />)
          </td>
          <td></td>
        </tr>

      </xsl:if>
      <xsl:if test="$addTo != 'True'">
        <tr>
          <td style="border-right:1px solid black;" align="center">
            <xsl:value-of select="QUANTITY" />
          </td>
          <td style="border-right:1px solid black;padding-left:4px;">
            <xsl:value-of select="DESC" />
          </td>
          <td style="padding-right:4px;" align="right">
            <xsl:value-of select="VALUE" />
          </td>
        </tr>
        <tr>
          <td style="border-right:1px solid black;"></td>
          <td style="border-right:1px solid black;padding-left:4px;">
            <xsl:value-of select="TRIM" />
          </td>
          <td></td>
        </tr>
        <tr>
          <td style="border-right:1px solid black;"></td>
          <td style="border-right:1px solid black;padding-left:4px;">
            <xsl:value-of select="DESC2" />
          </td>
          <td></td>
        </tr>
      </xsl:if>

    </xsl:if>
  </xsl:template>


  <xsl:template match="FOOTER">
    <table cellpadding="0" cellspacing="0" style="width:100%;">
      <tr>
        <td>
          <table cellpadding="0" width="100%" style="border-left:1px solid black;border-right:1px solid black;border-bottom:1px solid black;border-collapse:collapse;">
            <tr>
              <td valign="top" style="border-right:1px solid black;width:50%;">
                <table>
                  <tr>
                    <td>
                      <b>Other Costs:</b>
                    </td>
                    <td></td>
                  </tr>
                  <tr>
                    <td>Total Purchase Price:</td>
                    <td>
                      <xsl:value-of select="TOTAL" />
                    </td>
                  </tr>
                  <tr>
                    <td>Rate Of Interest:</td>
                    <td>
                      <xsl:value-of select="INTERESTRATE" />
                      <xsl:variable name="servicePrint" select="SERVICEPRINT" />
                      <xsl:if test="$servicePrint = 'A'"> per annum.</xsl:if>
                      <xsl:if test="$servicePrint = 'M'"> per month.</xsl:if>
                      <xsl:if test="$servicePrint = 'L'"> per month.</xsl:if>
                    </td>
                  </tr>
                  <tr>
                    <td>Number of instalments:</td>
                    <td>
                      <xsl:value-of select="INSTALNO + 1" />
                    </td>
                  </tr>
                  <tr>
                    <td>Date for payment of instalment:</td>
                  </tr>
                  <!-- 
							goods value:	<xsl:value-of select="GOODSVAL" /> 
							deposit:	<xsl:value-of select="DEPOSIT" />
						-->
                </table>
              </td>
              <td style="width:50%;">
                <table style="border-collapse:collapse;width:100%;">
                  <tr>
                    <td style="border-right:1px solid black;">(1) Less Deposit</td>
                    <td style="width:100px;"></td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      &#160;&#160;&#160;&#160;(a) Cash
                    </td>
                    <td>
                      <xsl:value-of select="DEPOSIT"/>
                    </td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      &#160;&#160;&#160;&#160;(b) Trade-in-Allowance
                    </td>
                    <td>
                      <xsl:value-of select="TRADE" />
                    </td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Total
                    </td>
                    <td>
                      <xsl:value-of select="DEPTRADETTL"/>
                    </td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      &#160;
                    </td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">(2) Add</td>
                    <td></td>
                  </tr>

                  <tr>
                    <td style="border-right:1px solid black;">
                      &#160;&#160;&#160;&#160;(a) Insurance (<xsl:value-of select="INSPCENT"/> Per annum)
                    </td>
                    <td>
                      <xsl:value-of select="INSURANCE"/>
                    </td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      &#160;&#160;&#160;&#160;(b) Installation
                    </td>
                    <td>
                      <xsl:value-of select="../PAGE/LINEITEMS/LINEITEM[ITEMNO='511700']/VALUE" />
                    </td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      &#160;&#160;&#160;&#160;(c) Other Charges (specify)
                    </td>
                    <td></td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      &#160;
                    </td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">(3) Balance/Credit Extended</td>
                    <td>

                    </td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Total
                    </td>
                    <td>
                      <xsl:value-of select="TOTALSTLUCIA" />
                    </td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      &#160;
                    </td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      Charge for Credit(<xsl:value-of select="CHRGCREDITINTRATE"/>) per month
                    </td>
                    <td>
                      <xsl:apply-templates select="CHRGCREDITEXT" />
                    </td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Total
                    </td>
                    <td></td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">Total interest &amp; Balance payable</td>
                    <td>
                      <xsl:apply-templates select="BALANCE" />
                    </td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">Payment Terms</td>
                    <td></td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      Length of agreement<xsl:text>     </xsl:text><xsl:value-of select="INSTALNO + 1" />
                    </td>
                    <td></td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">
                      Monthly installments<xsl:text>     </xsl:text><xsl:value-of select="FIRSTINST"/>, final <xsl:value-of select="FINALINST"/>
                    </td>
                    <td></td>
                  </tr>
                  <tr>
                    <td style="border-right:1px solid black;">Total Purchase price</td>
                    <td>
                      <xsl:value-of select="TOTAL" />
                    </td>
                  </tr>
                </table>
              </td>

            </tr>
          </table>
        </td>
      </tr>
    </table>
  </xsl:template>


  <xsl:template name="conditions">
    <br/>
    <br/>
    <table>
      <tr>
        <td align="center">
          <b>Radio Shack</b>
        </td>
      </tr>
      <tr>
        <td align="center">
          <b>TERMS AND CONDITIONS</b>
        </td>
      </tr>
      <tr>
        <td>
          <table>
            <tr valign="top">
              <td>
                <p>
                  <b>WHEREBY the Owner lets and the Hirer takes on hire the Goods and effects specified in the schedule hereto subject to and upon the following terms and conditions</b>
                </p>
                <p>(1) The Hirer shall pay to the Owner (a) on the signing hereto the deposit or the first monthly installment in consideration of the option to purchase granted and (b) thereafter punctually and without previous demand the monthly installment each month commencing one month after delivery or collection of the goods until the final payment or the Hirer shall pay to the Owner punctually and without previous demand the monthly installment each month commencing ........................... after delivery of the goods until the final payment (cross out whatever is not applicable). Each payment to be made to </p>
              </td>
              <td>
                <p>the Owner at their office on Chaussee Road, Castries or at such other address as they may direct (c) on demand any expenses incurred by the Owner in ascertaining the whereabouts of the Hirer or the goods from the Hirer or from any other person (including any payment made by the Owner in discharge or satisfaction of any lien or alleged lien on the goods) or in applying for or enforcing payment of any installment or other sums payable hereunder including legal charges as between Attorney at Law and client incurred by the Owner in respect of any of the foregoing matters whether legal proceedings shall have been instituted or not.</p>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
    <br class="pageBreak" />

    <table>
      <tr>
        <td style="width:50%;" valign="top">
          <p>(2) When the total amount paid under clause 1 (a) and (b) hereof shall equal the total amount of the hire purchase price shown on the front page hereto, the said goods shall become the sole and absolute property of the Hirer. The Hirer may at any time during the continuance of the hiring purchase the said goods by payment to the Owner of an amount which together with all previous payments for hire and the amount paid for the option of purchase equals the agreed value of the whole of the said goods. But unless and until such payment has been made in full, the said chattels shall remain the property of the Owner, and the hirer shall be fully responsible for maintaining the same in good order and condition and for all loss thereof or damage thereto however occasioned (fair wear and tear only accepted)</p>
          <p>
            (3) (a) The Hirer shall keep the said goods in the Hirer's own possession and control at the above named address from which premises they shall not be removed without the previous written consent of the Owner who may there inspect them at any time on demand.<br />
            (b) The Hirer shall not purport to sell any goods comprised in the agreement or remove or attempt to remove those goods or do any other act in relation to those goods for the purpose of selling the goods.<br />
            (c) The Hirer shall inform the owner in writing of any change of address of the Hirer.
          </p>
          <p>(4) The Hirer will punctually and regularly pay or cause to be paid (and at any time on demand produce to the Owner all receipts for) the rent, rates and taxes of the premises whereon the goods may be and shall keep the said goods free and exempt from and not suffer them or any of them to be taken in any distress for rent, execution or other legal process and may keep the goods properly insured against loss or damage of fire.</p>
          <p>(5) The Hirer shall pay to the Owner interest and late payment fees on all arrears, overdue or late payment of monthly installments at the rate not exceeding 25% of the value of the arrears.</p>
          <p>(6) The Hirer shall at any time before the final payment under the Hire Purchase Agreement falls due, be entitled to terminate the agreement by giving notice of termination in writing to the Owner and at the same time or prior to termination of the hire-purchase agreement shall deliver the goods to the owner during ordinary business hours only at the places specified below; (1) Bargain Basement, Marisule Complex, Marisule Gros-Islet, (2) Radio Shack outlet Vieux Fort, (3) Radio Shack outlet Soufriere. The Hirer shall remain liable for any arrears of hire installments with interest thereon as provided by this agreement and for any damage of the goods. </p>
          <p>
            (7) (a) If the Hirer shall make default in payment of any of the sums payable hereunder or shall fail to observe or perform any of the other terms and conditions of this agreement whether express or implied the Owner may exercise his or her right to repossess (without prejudice to any pre-existing liability of the Hirer to the Owner), by written notice served personally on the Hirer or sent by registered letter to, or left at the Hirer's usual or last known address or business specified in the hire-purchase agreement.<br />
            (b) Upon termination of the 21 day notice to repossess, the Owner may take repossession of the goods without further notice.<br />
            (c) The Owner shall have a right to repossess the goods without further notice as stipulated in Clause (7) (a), if the Owner has reasonable grounds to believe that the goods shall be removed or concealed.<br />
            (d) The Owner shall be entitled to send debt collectors or such authorized persons to repossess goods or collect monies during the hours of 7am and 7pm.
          </p>
        </td>
        <td style="width:50%;" valign="top">
          <p>
            (8) (1) The Hirer shall be entitled, at any time after the owner has taken possession of the goods but before the Owner sells or agrees to sell the goods, to settle his or her obligations under the hire-purchase agreement by paying to the Owner the amount required to settle the hire-purchase agreement in the following manner:<br />
            (a) The net balance due;<br />
            (b) the reasonable costs and expenses of the Owner  and  incidental to his or her taking possession of, holding, storing, repairing, maintaining, valuing and preparing for the sale of the goods and of his or her returning them to the order of the Hirer; and<br />
            (c) the costs reasonably and actually incurred by the Owner in doing any act, matter or thing necessary to remedy any breach of hire-purchase agreement by the Hirer.<br />
            (2) The Owner shall after the expiration of 14 days  sell the goods if the Hirer does not exercise his or her right subject to Clause (8) (1).
          </p>
          <p>(9) No relaxation, indulgence or waiver shown to the Hirer shall prejudice the Owners strict rights hereunder and no waiver of any such breach thereafter committed or suffered.</p>
          <p>(10) This agreement is conditional upon the order being approved by the Owner, and upon such approval the agreement shall become binding and of full effect and otherwise be void and of no effect. Any cash deposited on the signing hereof is accepted on this condition.</p>
          <p>
            (11) ''Depending on whether the purchase has been made under the terms of Radio Shack Options Bronze, Silver, Gold or Ultimate credit plan, included in the monthly service charge is an amount of either 0.05%, 0.1% , 0.2% or 0.25% respectively of the total agreement value relating to the purchase of credit protection insurance on an installment basis by the customer from Canterbury Insurance Co. Limited under Master Policy No. CAN-Z-SLI. In the event of the customer dying, or additionally under the 
            Options Silver, Gold and Ultimate credit Plans of the goods being destroyed by ALL risk or the customer becoming disabled, or additionally under Radio Shack Options Gold and Ultimate credit plans the goods being lost through burglary, the outstanding balance due (excluding any arrears) will be paid by Radio Shack subject to the terms and conditions of the Policy.  Also under Options Gold and Options Ultimate credit plans, if the customer is hospitalized as a result of an accident, Radio Shack will pay a months installment for each day of hospitalization up to a maximum of three monthly installments. Also under Radio Shack Options Gold and Ultimate credit plans, if the customer is made redundant or retrenched, no earlier than six months after the commencement of the agreement,  a maximum of 12 installments will be paid by the insurers over the entire term of the policy. The Policy excludes:
            -Payment for death in the first six months of the credit agreement's inception unless death is ''sudden or accidental''<br />
            - Claims for disablement or death arising from any sexually transmitted disease or HIV/AIDS related illness or from any pre-existing medical condition <br />
            - Claims for total losses, disablement or death arising from terrorist act <br />
            Additionally under the Radio Shack Options Ultimate Protection plan, in the event of the goods being totally destroyed by fire flood or storm, during the first 24 months following delivery of the goods to the Hirer's home and provided the account is up to date; the customer will receive a Radio Shack voucher to the value of 25% of the original price of the goods subject to the terms and conditions of the Policy.
          </p>
          <p>A copy of the policy wording is available on request. Payment of any policy benefit shall be made to Radio Shack as Trustee for the customer to be credited to the appropriate account of the customer and shall be in complete discharge of the liability with respect to the claim to which such benefits relate'' </p>
        </td>
      </tr>
    </table>

    <br class="pageBreak" />

    <table>
      <tr>
        <td colspan="2" align="center">
          <b>NOTICE</b>
        </td>
      </tr>
      <tr>
        <td colspan="2" align="center">
          <b>(Pursuant to the Consumer Credit Act No. 29 of 2006)</b>
        </td>
      </tr>
      <tr>
        <td style="width:50%;" valign="top">
          <p>
            <b>RIGHT OF HIRER TO TERMINATE AGREEMENT</b><br />
            l . The Hirer has the right to complete the agreement at any time and if you do you will be entitled to a rebate of the following charges payable under the agreement namely:-
          </p>
          <p>
            a)	The hire purchases charges<br />
            b)	For insurance, if any: and<br />
            c)	Maintenance or repairs, if any. (See section 23 of the Consumer Credit Act No. 29 of 2006)
          </p>
          <p>2. The Hirer may, at any time before the final payment falls due, terminate this agreement by giving notice, in writing, to the owner, of the intention to so terminate this agreement.</p>
          <p>3. The Hirer must then pay any installments which are due and in arrears at the time when he gives notice.</p>
          <p>4. If the Hirer does not deliver the goods to the owner at or prior to the time mentioned in paragraph 2, the notice of termination will be ineffective and the agreement will remain in force.</p>
          <p>5. If the goods have been damaged owing to the Hirer having failed to take reasonable care of them, the Owner may sue him for the amount of the damage unless that amount can be agreed between the Hirer and the Owner.</p>
          <p>6. The Hirer should see whether this agreement contains provisions allowing him or her to put an end to the Agreement on terms more favorable to him or her than those just mentioned. If it does he or she may put an end to the Agreement on those terms.   </p>

        </td>
        <td style="width:50%;" valign="top">
          <p>
            <b>RESTRICTION OF OWNERS RIGHT TO RECOVER GOODS WHERE SEVENTY PER CENTUM OF THE HIRE PURCHASE HAS BEEN PAID</b>
          </p>

          <p>
            1. After <xsl:value-of select="../FOOTER/SEVENTYPCTOTAL" /> has been paid, then unless the Hirer has himself put an end to the agreement the Owners of the goods cannot take them back from the Hirer without the Hirer's consent unless the owners obtain an order from the Court.
          </p>

          <p>2. If the owners apply to the Court for such an Order the Court may if the Court thinks it just to do so allow the Hirer to keep either-</p>

          <p>
            (a) the whole of the goods on condition that the Hirer pay the balance of the price in the manner ordered by the Court, or <br />
            (b) a fair proportion of the goods having regard to what the Hirer has already paid.
          </p>

          <p>
            <b>RESTRICTION OF OWNERS RIGHT TO RECOVER GOODS WHERE LESS THAN SEVENTY PER CENTUM OF THE HIRE PURCHASE PRICE HAS BEEN PAID</b>
          </p>

          <p>
            1. Where less than <xsl:value-of select="../FOOTER/SEVENTYPCTOTAL" /> has been paid and the Hirer has failed to pay any installments of the Hire Purchase price, the owner of the goods cannot take them back from the Hirer without the Hirer's consent unless the Owners have given to the Hirer twenty one clear days written notice of its intentions do so.
          </p>

          <p>2. If within the set period of twenty one days the Hirer pays to the Owner all installments of the Hire Purchase price due at the date of the issue of such notice the Owner will not be entitled to repossess the goods.</p>
        </td>
      </tr>
      <tr>
        <td colspan="2">
          <p>
            <b>ENTITLEMENT TO REBATES</b>
          </p>
          <p>Early completion of this agreement will entitle the Hirer to statutory rebates in accordance with Section 23 of the Consumer Credit Act No. 29 of 2006.</p>
          <p>IN WITNESS WHEREOF the parties hereto have set their hands on the date set out above.</p>
          <p>_____________________________</p>
          <p>Signature of the Owner</p>
          <p>The Hirer shall not make any purchases under this Agreement on behalf of a Third Party. However, the goods may be assigned to a Third Party with the written consent of the Owner.</p>
          <p>I have been informed of the cash price of the goods before entering into this Agreement and agree to the terms and conditions set out in this Agreement.</p>
          <p>_____________________________</p>
          <p>Signature of the Hirer </p>
          <p>Witness to Hirers signature</p>
          <p>Name: _____________________________</p>
          <p>Adress:</p>
          <p>__________________________________________________________</p>
          <p>__________________________________________________________</p>
          <p>__________________________________________________________</p>

        </td>
      </tr>
    </table>

  </xsl:template>




</xsl:stylesheet>
