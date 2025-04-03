<?xml version="1.0" encoding="UTF-8" ?>
<!--// CR 1024 (NM 30/04/2009) - The following fields are commented out - (Deposit Taken, Balance To Collect, Customer To Pay & Parts allocated)-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <HTML>
      <head>
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"></meta>
        <style type="text/css" media="all">
          @import url(<xsl:value-of select="BATCHPRINTS/@CSSPATH"/>	);
        </style>
      </head>
      <BODY>
        <xsl:apply-templates select="BATCHPRINTS" />
      </BODY>
    </HTML>
  </xsl:template>

  <xsl:template match="BATCHPRINTS">
    <xsl:apply-templates select="BATCHPRINT" />
  </xsl:template>
  
  <xsl:template match="BATCHPRINT">
    <div style="position:relative">
      <xsl:apply-templates select="HEADER" />
      <P></P>
      <xsl:apply-templates select="PARTS" />
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
      <TABLE border="0" class="normal" width ="100%" CELLPADDING="10" >
      <TR>
        <TD width ="50%">
          <B>Name: </B> <xsl:apply-templates select="NAME" />
          <BR>
            <B>Account Number: </B>
            <xsl:apply-templates select="ACCTNO" />
          </BR>
          <BR>
            <B>Printing Location: </B>
            <xsl:apply-templates select="PRINTLOCN" />
          </BR>
          <BR>
            <B>Action Required: </B>
            <xsl:apply-templates select="ACTION" />
          </BR>
        </TD>
        <TD>
          <B>
            Ref:</B>
          <xsl:apply-templates select="SERVICEREQUESTNO" />
          </TD>
      </TR>


    </TABLE>
    <TABLE border="0" class="normal" width ="100%" CELLPADDING="10">
      <TR>
        <TD width ="50%">
          <B>Home: </B>
          <xsl:apply-templates select="HOMETEL" />
          <BR>
            <B>
              Office:
            </B>
            <xsl:apply-templates select="WORKTEL" />
            </BR>
          <BR>
            <B>Cell: </B>
            <xsl:apply-templates select="MOBILETEL" />
          </BR>
        </TD>
        <TD width="10%" valign="top">
          <B>Address:</B>
        </TD>
        <TD>
          <xsl:apply-templates select="ADDRESS1" />
          <BR>
            <xsl:apply-templates select="ADDRESS2" />
          </BR>
          <BR>
            <xsl:apply-templates select="ADDRESS3" />
          </BR>
          <BR>
            <xsl:apply-templates select="ADDRESSPC" />
          </BR>
        </TD>
      </TR>
    </TABLE>
    <!--<TABLE border="0" class="normal" width ="100%" CELLPADDING="10">
      <TR>
        <TD width ="100%">
          <B>Directions:</B>
          <xsl:apply-templates select="DIRECTIONS" />
        </TD>
      </TR>
    </TABLE>-->
	  <!--IP 05/08/08 UAT5.1 UAT(516) Display Technician Special Instructions-->
	  <TABLE border="0" class="normal" width ="100%" CELLPADDING="10">
		  <TR>
			  <TD width ="100%">
				  <B>Directions:</B>
				  <xsl:apply-templates select="DIRECTIONS" />
				  <BR>
					  <B>Special Instructions:</B>
					  <xsl:apply-templates select="INSTRUCTIONS"/>
				  </BR>
			  </TD>
		  </TR>
	  </TABLE>
    <TABLE class="normal" width ="100%" CELLPADDING="10">
      <TR>
        <TD width="100%">
          <B>Product Code and description:</B>
        </TD>
      </TR>
    </TABLE>
    <TABLE class="normal" width ="100%" CELLPADDING="10">
      <TR>
        <TD width="30%">
          <xsl:apply-templates select="PRODCODE" />
        </TD>
        <TD>
          <xsl:apply-templates select="COMMENTS" />
        </TD>
      </TR>
    </TABLE>
    <TABLE class="normal" width ="100%" CELLPADDING="10">
      <TR>
        <TD width ="20%">
          <B>
            Appointment:</B>
          <BR>
              <xsl:apply-templates select="SLOTDATE" />
            </BR>
            <BR>
              <xsl:apply-templates select="SLOT" />
            </BR>
          </TD>
        <TD width ="20%">
          <B>Call Reported:</B>
          <BR>
            <xsl:apply-templates select="DATELOGGED" />
          </BR>
        </TD>
        <TD width="20%">
          <B>
            Delivery Date:</B>
            <BR>
              <xsl:apply-templates select="PURCHASEDATE" />
            </BR>
          </TD>
      </TR>
    </TABLE>

    <TABLE border="0" class="normal" CELLPADDING="10">
      <TR>
        <!--<TD width ="30%">
          <B>Deposit Taken:</B> $
          <xsl:apply-templates select="DEPOSIT" />
          <BR>
            <B>Balance To Collect:</B> $
            <xsl:apply-templates select="BALANCE" />
          </BR>
        </TD>-->
        <TD>
          <B>EW contract:</B>
          <xsl:apply-templates select="EW" />
          <BR>
            <B>MAN contract:</B>
            <xsl:apply-templates select="FYW" /></BR>
          <BR>
            <!--<B>Customer To Pay:</B>-->
            <!--<xsl:apply-templates select="CHARGETOCUSTOMER" />-->
          </BR>          
        </TD>
        <TD>
          <B> SerialNo:</B>
          <xsl:apply-templates select="SERIALNO" />
        </TD>
        <TD>
          <B> Location:</B>
          <xsl:apply-templates select="SERVICEBRANCHNO" />
        </TD>
      </TR>
    </TABLE>
    <!--<TABLE class="normal" width ="100%" CELLPADDING="5">
      <TR>
        <TD>
          <B>Parts allocated:</B>
        </TD>
      </TR>
    </TABLE>-->
    
    
  </xsl:template>

  <!--<xsl:template match="PARTS">
    <xsl:apply-templates select="PART" />
  </xsl:template>-->

  <!--<xsl:template match="PART">
    <TABLE border="0" class="normal" width ="100%" CELLPADDING="10">
      <TR>
        <TD width ="50%">
          
         <xsl:apply-templates select="PARTNO" />
          
          <BR>
            <xsl:apply-templates select="TYPE" />
          </BR>
        </TD>
      </TR>

    </TABLE>
  </xsl:template>-->

  <xsl:template match="FOOTER">
  <div style="position:absolute; left: 0px; top: 800px; bottom:0px" align="left">

    <B>Signed for by Technician as tested and operating correctly:</B>
    <br />
    <br />
    <br />
    <br />
    <B>Signed for by Customer as fully repaired:</B>
  </div>
  <div style="position:absolute; left: 230px; top: 950px; bottom:0px" align="center" runat="server" id="warrantable" visible="false">
    <b>
      <u>
        <font size= "5">
          <xsl:apply-templates select="WARRANTABLE" />
        </font>
      </u>
    </b>
  </div>
  </xsl:template>
</xsl:stylesheet>