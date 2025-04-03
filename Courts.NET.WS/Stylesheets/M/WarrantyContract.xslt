<?xml version="1.0" encoding="UTF-8" ?>
<!DOCTYPE xsl:stylesheet [
    <!ENTITY nbsp " ">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <HTML>
            <HEAD>
                <TITLE></TITLE>
                <META NAME="GENERATOR" Content="Microsoft Visual Studio 7.0"></META>
                <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"></meta>
                <style type="text/css" media="all">
                    @import url(styles.css);
                </style>
            </HEAD>
            <BODY>
                <xsl:apply-templates select="CONTRACTS" />
            </BODY>
        </HTML>
    </xsl:template>

    <xsl:template match="CONTRACTS">
        <xsl:apply-templates select="CONTRACT" />
    </xsl:template>

    <xsl:template match="CONTRACT">
        <div style="position:relative">
            <div class="smaller" style="LEFT: 15.25cm; WIDTH: 4cm; POSITION: absolute; TOP: 0.2cm; HEIGHT: 0.5cm">
                <xsl:value-of select="COPY" />
            </div>
            <div class="RFHead1" style="LEFT: 16cm; WIDTH: 2cm; POSITION: absolute; TOP: 1.5cm; HEIGHT: 0.5cm">
                <xsl:value-of select="CONTRACTNO" />
            </div>
            <div class="RFHead2" style="LEFT: 15.5cm; WIDTH: 3cm; POSITION: absolute; TOP: 6.85cm; HEIGHT: 0.5cm">
                <xsl:value-of select="SOLDBY" />
            </div>
            <div class="RFHead2" style="LEFT: 15.5cm; WIDTH: 3cm; POSITION: absolute; TOP: 8.2cm; HEIGHT: 0.5cm">
                <xsl:value-of select="BRANCHNAME" />
            </div>
            <div class="RFHead2" style="LEFT: 15.5cm; WIDTH: 3cm; POSITION: absolute; TOP: 9.6cm; HEIGHT: 0.5cm">
                <xsl:value-of select="STORENO" />
            </div>
            <div class="RFHead2" style="LEFT: 15.5cm; WIDTH: 3cm; POSITION: absolute; TOP: 11cm; HEIGHT: 0.5cm">
                <xsl:value-of select="TODAY" />
            </div>
            <div class="RFHead2" style="LEFT: 15.5cm; WIDTH: 3cm; POSITION: absolute; TOP: 12.4cm; HEIGHT: 0.5cm">
                <xsl:value-of select="SOLDBYNAME" />
            </div>
            <div class="WarrantyHeader" 
                 style="LEFT: 2.2cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.3cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
                <b>
                    <xsl:value-of select="ACCTNO1" />
                </b>
            </div>
            <div class="WarrantyHeader" 
                 style="LEFT: 2.95cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.3cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
                <b>
                    <xsl:value-of select="ACCTNO2" />
                </b>
            </div>
            <div class="WarrantyHeader" 
                 style="LEFT: 3.7cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.3cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
                <b>
                    <xsl:value-of select="ACCTNO3" />
                </b>
            </div>
            <div class="WarrantyHeader" 
                 style="LEFT: 4.45cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.3cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
                <b>
                    <xsl:value-of select="ACCTNO4" />
                </b>
            </div>
            <div class="WarrantyHeader" 
                 style="LEFT: 5.2cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.3cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
                <b>
                    <xsl:value-of select="ACCTNO5" />
                </b>
            </div>
            <div class="WarrantyHeader" 
                 style="LEFT: 5.95cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.3cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
                <b>
                    <xsl:value-of select="ACCTNO6" />
                </b>
            </div>
            <div class="WarrantyHeader" 
                 style="LEFT: 6.7cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.3cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
                <b>
                    <xsl:value-of select="ACCTNO7" />
                </b>
            </div>
            <div class="WarrantyHeader" 
                 style="LEFT: 7.45cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.3cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
                <b>
                    <xsl:value-of select="ACCTNO8" />
                </b>
            </div>
            <div class="WarrantyHeader" 
                 style="LEFT: 8.2cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.3cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
                <b>
                    <xsl:value-of select="ACCTNO9" />
                </b>
            </div>
            <div class="WarrantyHeader" 
                 style="LEFT: 8.95cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.3cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
                <b>
                    <xsl:value-of select="ACCTNO10" />
                </b>
            </div>
            <div class="WarrantyHeader" 
                 style="LEFT: 9.7cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.3cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
                <b>
                    <xsl:value-of select="ACCTNO11" />
                </b>
            </div>
            <div class="WarrantyHeader" 
                 style="LEFT: 10.45cm;
						WIDTH: 0.8cm;
						POSITION: absolute;
						TOP: 12.3cm;
						HEIGHT: 0.5cm;
						TEXT-ALIGN: center">
                <b>
                    <xsl:value-of select="ACCTNO12" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 2.3cm; WIDTH: 5cm; POSITION: absolute; TOP: 6.85cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="FIRSTNAME" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 6.9cm; WIDTH: 5cm; POSITION: absolute; TOP: 6.85cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="LASTNAME" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 2.3cm; WIDTH: 10cm; POSITION: absolute; TOP: 7.85cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="ADDRESS1" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 2.3cm; WIDTH: 10cm; POSITION: absolute; TOP: 8.35cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="ADDRESS2" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 2.3cm; WIDTH: 10cm; POSITION: absolute; TOP: 8.85cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="ADDRESS3" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 2.3cm; WIDTH: 10cm; POSITION: absolute; TOP: 9.35cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="POSTCODE" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 2.3cm; WIDTH: 5cm; POSITION: absolute; TOP: 10.6cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="WORKTEL" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 6.9cm; WIDTH: 5cm; POSITION: absolute; TOP: 10.6cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="HOMETEL" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 1cm; WIDTH: 5cm; POSITION: absolute; TOP: 15.25cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="DATEOFPURCHASE" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 7.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 15.25cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="ITEMNO" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 1cm; WIDTH: 16cm; POSITION: absolute; TOP: 16.4cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="ITEMDESC1" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 1cm; WIDTH: 16cm; POSITION: absolute; TOP: 16.9cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="ITEMDESC2" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 4cm; WIDTH: 16cm; POSITION: absolute; TOP: 17.8cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="ITEMPRICE" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 1cm; WIDTH: 5cm; POSITION: absolute; TOP: 18.9cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="WARRANTYNO" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 7.5cm; WIDTH: 5cm; POSITION: absolute; TOP: 18.9cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="WARRANTYPRICE" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 1cm; WIDTH: 16cm; POSITION: absolute; TOP: 19.9cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="WARRANTYDESC1" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 1cm; WIDTH: 16cm; POSITION: absolute; TOP: 20.3cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="WARRANTYDESC2" />
                </b>
            </div>
            <xsl:if test="TERMSTYPE='WC'">
                <DIV class="smallPrint" style="Z-INDEX: 106; LEFT: 0.5cm; WIDTH: 10cm; POSITION: absolute; TOP: 20.7cm; HEIGHT: 0.317cm">
                    Warranty purchased on credit. Customer has <xsl:value-of select="WARRANTYCREDIT" /> days after purchase of stock item to pay for warranty otherwise warranty will expire.
                </DIV>
            </xsl:if>
            <div class="WarrantyHeader" style="LEFT: 1cm; WIDTH: 5cm; POSITION: absolute; TOP: 22cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="PLANNEDDELIVERY" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 10cm; WIDTH: 5cm; POSITION: absolute; TOP: 22cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="EXPIRYOFWARRANTY" />
                </b>
            </div>
            <div class="WarrantyHeader" style="LEFT: 16.25cm; WIDTH: 5cm; POSITION: absolute; TOP: 25.5cm; HEIGHT: 0.5cm">
                <b>
                    <xsl:value-of select="TODAY" />
                </b>
            </div>
            <xsl:variable name="last" select="LAST" />
            <xsl:if test="$last != 'TRUE'">
                <br class="pageBreak" />
            </xsl:if>
        </div>
    </xsl:template>

</xsl:stylesheet>

