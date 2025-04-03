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
                <xsl:apply-templates select="TAXINVOICES" />
            </BODY>
        </HTML>
    </xsl:template>

    <xsl:template match="TAXINVOICES">
        <xsl:apply-templates select="TAXINVOICE" />
        <P></P>
    </xsl:template>

    <xsl:template match="TAXINVOICE">
        <div style="position:relative" >
            <!-- resets the position context -->
            <xsl:apply-templates select="HEADER" />
            <xsl:apply-templates select="LINEITEMS" />
            <!-- Separate Invoice Total -->
            <hr/>
            <!-- jec 03/01/08 -->
            <xsl:apply-templates select="FOOTER" />
            <xsl:variable name="last" select="LAST" />
            <xsl:choose>
                <xsl:when test="$last != 'TRUE'">
                    <br class="pageBreak" />
                </xsl:when>
                <xsl:otherwise>
                    <HR />
                    <DIV class="smallPrint" style="text-align:center;">
                        <span class="bold">
                            No Cash Refund:
                        </span>
                        <xsl:text disable-output-escaping="yes"><![CDATA[All Claims &/or returns must be accompanied with the original invoice. The general terms of sale as filed with the Office of the Court of Curacao shall apply to all transactions between Omni B.V. & the other party. Deviations from these terms & conditions can only be agreed upon in writing. All goods shall remain Omni's property until they have been fully paid by the other party.]]></xsl:text>
                    </DIV>
                    <div class="bold smallPrint"  style="text-align:center;text-decoration: underline;padding:7px;">
                        Not Covered Under Warranty:
                    </div>
                    <DIV class="smallPrint" style="text-align:center;">
                        <xsl:text disable-output-escaping="yes"><![CDATA[
Products with removed or altered serial number. Products with damages: Caused by use contrary to the manual; by installation, repair or reinstallation from non-authorized service centers; power / voltage fluctuations; vandalism; water sand; insects; altered or modified by unauthorized roots / play stores.
Batteries, plastic / cosmetic parts and accessories. Low brightness on screen due to normal aging in the screen tube, lamp or burnt images
]]></xsl:text>
                    </DIV>
                </xsl:otherwise>
            </xsl:choose>
        </div>
    </xsl:template>

    <xsl:template match="FOOTER">
        <!--<div style="position:absolute; top=60%; left=0%; width=100%" align="right">	 jec 02/01/08 overprinting-->
        <TABLE class="normal" height="7" width="600" border="0">
            <TR>
                <TD width="80%" align="right" class="bold">Sale subtotal: </TD>
                <TD width="20%" align="right">
                    <xsl:value-of select="EXTOTAL" />
                </TD>
            </TR>
            <TR>
                <TD width="80%" align="right" class="bold">Tax: </TD>
                <TD width="20%" align="right">
                    <xsl:value-of select="TAXTOTAL" />
                </TD>
            </TR>
            <TR>
                <TD width="80%" align="right" class="bold">Total: </TD>
                <TD width="20%" align="right">
                    <xsl:value-of select="INCTOTAL" />
                </TD>
            </TR>
        </TABLE>
        <!--</div> overprinting-->
    </xsl:template>

    <xsl:template match="FOOTER_OLD">
        <!--<div style="position:absolute; top=60%; left=0%; width=100%" align="right">	 jec 02/01/08 overprinting-->
        <TABLE class="normal" height="7" width="600" border="0">
            <TR>
                <TD width="25%" align="right" class="bold">Invoice Total2: </TD>
                <TD width="25%" align="right">
                    <xsl:value-of select="EXTOTAL" />
                </TD>
                <TD width="25%" align="right">
                    <xsl:value-of select="TAXTOTAL" />
                </TD>
                <TD width="25%" align="right">
                    <xsl:value-of select="INCTOTAL" />
                </TD>
            </TR>
        </TABLE>
        <!--</div> overprinting-->
    </xsl:template>

    <xsl:template match="HEADER">
        <TABLE class="normal" height="7" width="600" border="0">
            <TR>
                <td width="60%">
                    DATE: <xsl:value-of select="NOW" />
                </td>
                <td width="40%">
                    PURCHASE DATE: <xsl:value-of select="DATE" />
                </td>
            </TR>
            <TR>
                <xsl:apply-templates select="BRANCHNAME" />
                <xsl:apply-templates select="ACCTNO" />
            </TR>
            <TR>
                <xsl:apply-templates select="BRANCHADDR1" />
                <!--xsl:apply-templates select="BUFFNO" /-->
            </TR>
            <TR>
                <xsl:apply-templates select="BRANCHADDR2" />
                <TD width="30%"></TD>
            </TR>
            <TR>
                <xsl:apply-templates select="BRANCHADDR3" />
                <TD width="30%"></TD>
            </TR>
            <TR></TR>
            <TR>
                <TD width="70%"></TD>
                <TD width="30%">
                    <xsl:apply-templates select="FIRSTNAME" />
                    <xsl:apply-templates select="LASTNAME" />
                </TD>
            </TR>
            <TR>
                <TD width="70%"></TD>
                <xsl:apply-templates select="ADDR1" />
            </TR>
            <TR>
                <TD width="70%"></TD>
                <xsl:apply-templates select="ADDR2" />
            </TR>
            <TR>
                <TD width="70%"></TD>
                <xsl:apply-templates select="ADDR3" />
            </TR>
            <TR>
                <TD width="70%"></TD>
                <xsl:apply-templates select="POSTCODE" />
            </TR>
        </TABLE>
        <P></P>
        <TABLE class="normal" width="600" border="0">
            <TR>
                <TD>
                    <xsl:apply-templates select="SALETEXT" />
                </TD>
            </TR>
            <TR>
                <TD class="heading" width="10%">Qty</TD>
                <TD class="heading" width="20%" align="left">Tax Rate</TD>
                <TD class="heading" width="40%" align="right">Value</TD>
                <!--<xsl:apply-templates select="TAXNAME" />-->
                <TD class="heading" width="30%" align="right">Incl</TD>
            </TR>
        </TABLE>
    </xsl:template>

    <xsl:template match="LINEITEMS">
        <xsl:apply-templates select="LINEITEM">
            <xsl:sort select="INDEX" />
            <!-- sort the remaining items on the index node -->
        </xsl:apply-templates>
    </xsl:template>

    <xsl:template match="LINEITEM">
        <xsl:choose>
            <xsl:when test="TYPE!='KitDiscount' and TYPE!='Component'" >
                <P></P>
                <TABLE class="normal" width="700" border="0">
                    <TR class="bold">
                        <xsl:apply-templates select="QUANTITY" />
                        <td width="15%" align="left">
                            <xsl:value-of select="ITEMNO" />
                        </td>
                        <TD width="75%" align="left">
                            <xsl:value-of select="DESC" /> -
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="DESC2" />
                            <xsl:value-of select="MODEL"/>
                        </TD>
                    </TR>
                    <xsl:if test="CONTRACTNO != ''">
                        <tr>
                            <td></td>
                            <td></td>
                            <td>
                                Contract No - <xsl:value-of select="CONTRACTNO" />
                            </td>
                        </tr>
                    </xsl:if>
                </TABLE>
                <TABLE class="normal" width="600" border="0">
                    <TR>
                        <TD width="10%"></TD>
                        <td width="20%" align="left">
                            <xsl:value-of select="TAXRATE" />%
                        </td>
                        <td width="40%" align="right">
                            <xsl:value-of select="ORDERVALUEEXTAX" />
                        </td>
                        <!--<td width="20%" align="right">
              <xsl:value-of select="TAXAMOUNT" />
            </td>-->
                        <td width="30%" align="right">
                            <xsl:value-of select="ORDERVALUE" />
                        </td>
                    </TR>
                </TABLE>
                <xsl:if test="(TYPE='Warranty' or TYPE='KitWarranty') and TERMSTYPE='WC'">
                    <TABLE class="normal" width="600" border="0">
                        <TR>
                            <TD width="20%"></TD>
                            <td width="80%" align="left">
                                Warranty to Pay
                            </td>
                        </TR>
                    </TABLE>
                </xsl:if>
                <xsl:apply-templates select="SERIALNOS" />
                <xsl:apply-templates select="PB" />
                <xsl:apply-templates select="RELATED" />
            </xsl:when>
            <xsl:otherwise>
                <TABLE class="smaller" width="600" border="0">
                    <TR>
                        <xsl:apply-templates select="QUANTITY" />
                        <td width="15%" align="left">
                            <xsl:value-of select="ITEMNO" />
                        </td>
                        <TD width="75%" align="left">
                            <xsl:value-of select="DESC" />
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="DESC2" />
                        </TD>
                    </TR>
                </TABLE>

                <xsl:apply-templates select="RELATED" />
            </xsl:otherwise>
        </xsl:choose>

    </xsl:template>

    <xsl:template match="RELATED">
        <xsl:apply-templates select="LINEITEM" />
    </xsl:template>

    <!--<xsl:template match="DATE">
    <TD width="70%">
      DATE: <xsl:apply-templates select="NOW"/>
    </TD>
    <TD width="30%">
      PURCHASE DATE: <xsl:apply-templates select="DATE"/>
    </TD>
  </xsl:template>-->

    <xsl:template match="BRANCHNAME">
        <TD width="70%">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="ACCTNO">
        <TD width="30%">
            INVOICE NO: <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="BRANCHADDR1">
        <TD width="70%">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="BUFFNO">
        <TD width="30%">
            Serial No: <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="BRANCHADDR2">
        <TD width="70%">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="BRANCHADDR3">
        <TD width="70%">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="FIRSTNAME">
        <xsl:apply-templates />
    </xsl:template>

    <xsl:template match="LASTNAME">
        <xsl:apply-templates />
    </xsl:template>

    <xsl:template match="ADDR1">
        <TD width="30%">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="ADDR2">
        <TD width="30%">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="ADDR3">
        <TD width="30%">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="POSTCODE">
        <TD width="30%">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="QUANTITY">
        <TD width="10%" align="left">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="DESC">
        <TD width="40%" align="left">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="DESC2">
        <TD width="40%" align="left">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="TAXRATE">
        <TD width="20%" align="left">
            <xsl:apply-templates />%
        </TD>
    </xsl:template>

    <xsl:template match="PRICE">
        <TD width="20%" align="right">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="TAXAMOUNT">
        <TD width="20%" align="right">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="TAXNAME">
        <TD class="heading" width="20%" align="right">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="INCLUSIVE">
        <TD width="20%" align="right">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="EXTOTAL">
        <TD width="25%" align="right">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="INCTOTAL">
        <TD width="25%" align="right">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="TAXTOTAL">
        <TD width="25%" align="right">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="SALETEXT">
        <xsl:apply-templates />
    </xsl:template>

    <xsl:template match="PB">
        <br class="pageBreak" />
        <xsl:apply-templates select="../../../HEADER" />
    </xsl:template>


</xsl:stylesheet>