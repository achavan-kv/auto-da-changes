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
                <xsl:apply-templates select="BOOKINGS" />
            </BODY>
        </HTML>
    </xsl:template>

    <xsl:template match="BOOKINGS">
        <xsl:apply-templates select="BOOKING" />
    </xsl:template>

    <xsl:template match="BOOKING">
        <div class="HPHeader">
            <p>Installation Booking</p>
        </div>
        <div style="position:relative">
            <xsl:apply-templates select="HEADER" />
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
        <TABLE border="0" class="normal" width ="100%" CELLPADDING="1" >
            <TR>
                <TD width="20%" valign="top">
                    <B>Customer Name:</B>
                </TD>
                <TD width="38%" valign="top">
                    <xsl:apply-templates select="CUSTNAME" />
                </TD>
                <TD width="3%">
                </TD>
                <TD width="20%" valign="top">
                    <B>Branch No:</B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="BRANCHNO" />
                </TD>
            </TR>
            <TR>
                <TD valign="top">
                    <B>Account No:</B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="ACCTNO" />
                </TD>
                <TD>
                </TD>
                <TD valign="top">
                    <B>Delivery Date:</B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="DELDATE" />
                </TD>
            </TR>
            <TR>
                <TD valign="top">
                    <B>Agreement No:</B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="AGREEMENTNO" /> 
                </TD>
                <TD>
                </TD>
                <TD valign="top">
                    <B>Purchase Date:</B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="PURCHASEDATE" />
                </TD>
            </TR>
        </TABLE>
        <BR></BR>
        <BR></BR>
        <TABLE border="0" class="normal" width ="100%" CELLPADDING="1">
            <TR>
                <TD colspan="5" class="normalBold">
                    <U>Item Detail</U>
                </TD>
            </TR>
            <TR>
                <TD width="20%" valign="top">
                    <B>Product Code: </B> <!--IP - 21/03/11 - #3325 - Change heading from Item No to Product Code-->
                </TD>
                <TD width="38%" valign="top">
                    <xsl:apply-templates select="ITEMNO" />
                </TD>
                <TD width="3%">
                </TD>
                <TD width="20%" valign="top">
                    <B>Manufacturer: </B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="MANUFACTURER" />
                </TD>
            </TR>
            <TR>
                <TD valign="top">
                    <B>Model No: </B>
                </TD>
                <TD valign="top">____________________</TD>
                <TD>
                </TD>
                <TD valign="top">
                    <B>Serial No: </B>
                </TD>
                <TD valign="top">____________________</TD>
            </TR>
            <TR>
                <TD valign="top">
                    <B>Description: </B>
                </TD>
                <TD valign="top" colspan="4">
                    <xsl:apply-templates select="ITEMDESC" />
                </TD>
            </TR>
            <TR>
                <TD valign="top">
                    <B>Has Warranty: </B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="HASWARRANTY" />
                </TD>
                <TD colspan="3">
                </TD>
            </TR>              
        </TABLE>
        <BR></BR>
        <BR></BR>
        <TABLE border="0" class="normal" width ="100%" CELLPADDING="1">
            <TR>
                <TD colspan="5" class="normalBold">
                    <U>Customer Contact Detail</U>
                </TD>
            </TR>
            <TR>
                <TD width="16%" valign="top">
                    <B>Home Tel: </B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="HOMETEL" />
                </TD>               
            </TR>
            <TR>
                <TD valign="top">
                    <B>Work Tel: </B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="WORKTEL" />
                </TD>               
            </TR>
            <TR>
                <TD valign="top">
                    <B>Address:</B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="ADDRESS1" /><BR></BR>
                    <xsl:apply-templates select="ADDRESS2" /><BR></BR>
                    <xsl:apply-templates select="ADDRESS3" /><BR></BR>
                    <xsl:apply-templates select="POSTCODE" /> 
                </TD>               
            </TR>
            <TR>
                <TD valign="top">
                    <B>Directions: </B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="DIRECTION" />
                </TD>               
            </TR>
        </TABLE>
        <BR></BR>
        <BR></BR>
        <TABLE border="0" class="normal" width ="100%" CELLPADDING="1">
            <TR>
                <TD colspan="5" class="normalBold">
                    <U>Installation Detail</U>
                </TD>
            </TR>
            <TR>
                <TD width="28%" valign="top">
                    <B>Installation No:</B>
                </TD>
                <TD width="25%" valign="top">
                    <xsl:apply-templates select="INSTNO" />
                </TD>
                <TD width="3%">
                </TD>
                <TD width="20%" valign="top">                    
                    <B>Technician: </B>
                </TD>
                <TD valign="top">                    
                    <xsl:apply-templates select="TECHNICIAN" />
                </TD>
            </TR>
            <TR>
                <TD valign="top">
                    <B>Installation Date:</B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="INSTDATE" />
                </TD>
                <TD>
                </TD>
                <TD valign="top">
                    <B>Slots: </B>
                </TD>
                <TD valign="top">
                    <xsl:apply-templates select="SLOTS" />
                </TD>
            </TR>            
        </TABLE>
    </xsl:template>
    <xsl:template match="FOOTER">
        <div style="position:absolute; left: 0px; top: 700px; bottom:0px" align="left">
            <B>Signed for by Technician as tested and operating correctly:</B>
            <br />
            <br />
            <br />
            <br />
            <B>Signed for by Customer as installed:</B>
        </div>
    </xsl:template>
</xsl:stylesheet>