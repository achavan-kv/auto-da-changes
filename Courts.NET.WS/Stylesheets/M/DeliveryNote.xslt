<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <HTML>
      <head>
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"></meta>
        <style type="text/css" media="all">
          @import url(<xsl:value-of select="DELIVERYNOTES/@CSSPATH"/>   );
        </style>
      </head>
      <BODY>
        <xsl:apply-templates select="DELIVERYNOTES" />
      </BODY>
    </HTML>
  </xsl:template>

  <xsl:template match="DELIVERYNOTES">
    <xsl:apply-templates select="DELIVERYNOTE" />
  </xsl:template>

  <xsl:template match="DELIVERYNOTE">
    <div style="position:relative">
      <xsl:apply-templates select="HEADER" />
      <P></P>
      <xsl:apply-templates select="LINEITEMS" />
      <P></P>
      <xsl:apply-templates select="FOOTER" />
      <P></P>
      <xsl:variable name="last" select="LAST" />
      <xsl:if test="$last != 'TRUE'">
        <br class="pageBreak" />
      </xsl:if>
    </div>
  </xsl:template>

  <xsl:template match="HEADER">
    <TABLE class="normal" id="Address" height="7" width="600" border="0">
      <TR>
        <xsl:apply-templates select="DELTEXT" />
        <TD width="35%" align="left">
          <xsl:apply-templates select="BRANCH" />
          <xsl:apply-templates select="BUFFNO" />
        </TD>
      </TR>
    </TABLE>
    <TABLE class="normal" height="7" width="600" border="0">
      <TR>
        <TD width="10%" align="left" colspan="2">
          Stock Location: <xsl:apply-templates select="LOCATION" />
        </TD>
      </TR>
    </TABLE>
    <TABLE class="normal" height="7" width="600" border="0">
      <TR>
        <TD align="left">
          AT:
          <xsl:apply-templates select="PRINTED" />
          <xsl:apply-templates select="ACCTNO" />
        </TD>
      </TR>
    </TABLE>
    <TABLE class="normal" height="7" width="600" border="0">
      <TR>
        <TD align="left" colspan="2">
          DUE: <xsl:value-of select="DELDATE" />
        </TD>
      </TR>
    </TABLE>
    <TABLE class="normal" height="7" width="600" border="0">
      <TR>
        <TD width="65%" align="left">
          PRINTED BY: <xsl:apply-templates select="PRINTEDBY" />
        </TD>
        <TD width="35%" align="left">
        </TD>
      </TR>
    </TABLE>
    <TABLE class="normal" height="7" width="600" border="0">
      <TR>
        <TD width="65%">
          <STRONG>
            <xsl:apply-templates select="PRINTTEXT" />
          </STRONG>
        </TD>
        <TD width="35%"></TD>
      </TR>
      <TR>
        <TD width="65%">
          <STRONG>
            <xsl:apply-templates select="CANCELTEXT" />
          </STRONG>
        </TD>
        <TD width="35%" id="name" align="left">
          <xsl:apply-templates select="CUSTOMERNAME" />
        </TD>
      </TR>
      <TR>
        <TD width="65%"></TD>
        <xsl:apply-templates select="ALIAS" />
      </TR>
      <TR>
        <TD width="65%"></TD>
        <xsl:apply-templates select="ADDRESS1" />
      </TR>
      <TR>
        <TD width="65%"></TD>
        <xsl:apply-templates select="ADDRESS2" />
      </TR>
      <TR>
        <TD width="65%"></TD>
        <xsl:apply-templates select="ADDRESS3" />
      </TR>
      <TR>
        <TD width="65%"></TD>
        <xsl:apply-templates select="POSTCODE" />
      </TR>
      <TR>
        <TD width="65%"></TD>
        <xsl:apply-templates select="HOMETEL" />
      </TR>
      <TR>
        <TD width="65%"></TD>
        <xsl:apply-templates select="WORKTEL" />
      </TR>
      <TR>
        <TD width="65%"></TD>
        <xsl:apply-templates select="MOBILE" />
      </TR>
      <TR>
        <TD width="65%"></TD>
        <!-- LW 69370 delivery Address Telephone Number to be included-->
        <xsl:apply-templates select="DELTEL" />
      </TR>
    </TABLE>
  </xsl:template>

  <xsl:template match="LINEITEMS">
    <xsl:apply-templates select="LINEITEM" />
  </xsl:template>

  <xsl:template match="LINEITEM">
    <TABLE class="normal" id="Items" width="650" border="0">
      <TR>
        <xsl:apply-templates select="QUANTITY" />
        <xsl:apply-templates select="ITEMNO" />
        <xsl:apply-templates select="DESC1" />
        <xsl:apply-templates select="PRICE" />
      </TR>
      <TR>
        <xsl:apply-templates select="RETDETAILS" />
        <td width="10%"></td>
        <xsl:apply-templates select="DESC2" />
        <td width="25%"></td>
      </TR>
      <xsl:apply-templates select="NOTES" />
    </TABLE>
    <xsl:apply-templates select="PB" />
  </xsl:template>

  <xsl:template match="FOOTER">
    <!-- Extra bit added for COD delivery notes 21/08/2003 -->
    <xsl:if test="COD='Y'">
      <div style="position:absolute; top=19cm; left=4.5cm; width=100%" align="left">
        <B>
          <U>
            <I>C.O.D.</I>
          </U>
        </B>
      </div>
      <div style="position:absolute; top=19cm; width=100%" align="center">
        <table class="normal" height="7" width="650" border="0">
          <tr>
            <td width="80%" align="right"></td>
            <td width="20%" align="right">
              <xsl:value-of select="format-number(sum(../LINEITEMS/LINEITEM/PRICE), '#,##0.00')" />
            </td>
          </tr>
          <tr>
            <td width="80%" align="right">Additional Charges/Allowances:</td>
            <td width="20%" align="right">
              <xsl:value-of select="format-number(ADDCHARGES, '#,##0.00')" />
            </td>
          </tr>
          <tr>
            <td width="80%" align="right">Amount Payable:</td>
            <td width="20%" align="right">
              <xsl:value-of select="format-number(PAYABLE, '#,##0.00')" />
            </td>
          </tr>
        </table>
      </div>
    </xsl:if>

    <xsl:if test="COD='N'">
      <div style="position:absolute; top=19cm; width=100%" align="center">
        <TABLE class="normal" height="7" width="650" border="0">
          <TR>
            <TD width="80%" align="right"></TD>
            <TD width="20%" align="right">
              <xsl:value-of select="format-number(sum(../LINEITEMS/LINEITEM/PRICE), '#,##0.00')" />
            </TD>
          </TR>
        </TABLE>
      </div>
    </xsl:if>

    <div id="SalesPerson" style="position:absolute; top=22cm; width=100%" align="center">
      sp: <xsl:value-of select="USERNAME" />
      (<xsl:value-of select="USER"/>)
    </div>
    <div style="position:absolute; top=23cm; width=100%" align="center">
      <center>
        <TABLE width="100%" align="center" border="0">
          <TR>
            <TD>
              <font size="0.2em">
                <xsl:value-of select="CUSTNOTES" />
              </font>
            </TD>
          </TR>
        </TABLE>
      </center>
    </div>
  </xsl:template>

  <xsl:template match="QUANTITY">
    <TD id="quantity" align="left" width="10%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="ITEMNO">
    <TD id="itemno" align="left" width="10%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="DESC1">
    <TD id="desc1" align="left" width="55%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="PRICE">
    <TD id="price" align="right" width="25%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="DESC2">
    <td id="descr2" align="left" width="55%">
      <xsl:apply-templates />
    </td>
  </xsl:template>

  <xsl:template match="NOTES">
    <TR>
      <td width="10%"></td>
      <td width="10%"></td>
      <td id="notes" align="left" width="55%">
        <xsl:apply-templates />
      </td>
      <td width="25%"></td>
    </TR>
  </xsl:template>

  <xsl:template match="DELTEXT">
    <!-- 68930 22/11/07 rdb make text larger for Collection: -->
    <TD class="CashierHeader" width="65%" id="delText" align="left" style="font-size:20">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="BRANCH">
    <xsl:apply-templates />/
  </xsl:template>

  <xsl:template match="BUFFNO">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="ACCTNO">
    <TD width="35%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="FIRSTNAME">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="LASTNAME">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="CUSTOMERNAME">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="ALIAS">
    <TD width="35%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="ADDRESS1">
    <TD width="35%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="ADDRESS2">
    <TD width="35%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="ADDRESS3">
    <TD width="35%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="POSTCODE">
    <TD width="35%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="HOMETEL">
    <TD width="35%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="WORKTEL">
    <TD width="35%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="MOBILE">
    <TD width="35%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="DELTEL">
    <TD width="35%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="PRINTED">
    <TD width="62%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>

  <xsl:template match="LOCATION">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="PRINTEDBY">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="PRINTTEXT">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="CANCELTEXT">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="RETDETAILS">
    <TD align="left" width="10%">
      <xsl:apply-templates />
    </TD>
  </xsl:template>


  <xsl:template match="PB">
    <br class="pageBreak" />
    <xsl:apply-templates select="../../../HEADER" />
    <P></P>
  </xsl:template>

</xsl:stylesheet>

