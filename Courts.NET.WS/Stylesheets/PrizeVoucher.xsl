<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <HTML>
            <HEAD>
                <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"></meta>
                <style type="text/css" media="all">
                    @import url(styles.css);
                </style>
            </HEAD>
            <BODY>
             <!--
                This is an XSLT template file. Fill in this area with the
                XSL elements which will transform your XML to XHTML.
            -->
                <xsl:apply-templates select="PRIZEVOUCHERS" />
            </BODY>
        </HTML>
    </xsl:template>

    <xsl:template match="PRIZEVOUCHERS">
        <xsl:apply-templates select="VOUCHERS" />
    </xsl:template>

    <xsl:template match="VOUCHERS">
        <TABLE class ="ThaiLineItem" width="600">
            <TR>
                <TD style="position: relative; top: 2.5cm; left: 3.7cm; width=50%">
                    <xsl:value-of select="VOUCHER1/ACCOUNTNO" />
                </TD>
                <TD style="position: relative; top: 2.5cm; left: 4cm; width=50%">
                    <xsl:value-of select="VOUCHER2/ACCOUNTNO" />
                </TD>
            </TR>
            <TR>
                <TD style="position: relative; top: 2.5cm; left: 3.7cm; width=50%">
                    <xsl:value-of select="VOUCHER1/VOUCHERNO" />
                </TD>
                <TD style="position: relative; top: 2.5cm; left: 4cm; width=50%">
                    <xsl:value-of select="VOUCHER2/VOUCHERNO" />
                </TD>
            </TR>
            <TR>
                <TD style="position: relative; top: 4.5cm; left: 3.7cm; width=50%">
                    <xsl:value-of select="VOUCHER3/ACCOUNTNO" />
                </TD>
                <TD style="position: relative; top: 4.5cm; left: 4cm; width=50%">
                    <xsl:value-of select="VOUCHER4/ACCOUNTNO" />
                </TD>
            </TR>
            <TR>
                <TD style="position: relative; top: 4.5cm; left: 3.7cm; width=50%">
                    <xsl:value-of select="VOUCHER3/VOUCHERNO" />
                </TD>
                <TD style="position: relative; top: 4.5cm; left: 4cm; width=50%">
                    <xsl:value-of select="VOUCHER4/VOUCHERNO" />
                </TD>
            </TR>
            <TR>
                <TD style="position: relative; top: 6.5cm; left: 3.7cm; width=50%">
                    <xsl:value-of select="VOUCHER5/ACCOUNTNO" />
                </TD>
                <TD style="position: relative; top: 6.5cm; left: 4cm; width=50%">
                    <xsl:value-of select="VOUCHER6/ACCOUNTNO" />
                </TD>
            </TR>
            <TR>
                <TD style="position: relative; top: 6.5cm; left: 3.7cm; width=50%">
                    <xsl:value-of select="VOUCHER5/VOUCHERNO" />
                </TD>
                <TD style="position: relative; top: 6.5cm; left: 4cm; width=50%">
                    <xsl:value-of select="VOUCHER6/VOUCHERNO" />
                </TD>
            </TR>
            <TR>
                <TD style="position: relative; top: 8.5cm; left: 3.7cm; width=50%">
                    <xsl:value-of select="VOUCHER7/ACCOUNTNO" />
                </TD>
                <TD style="position: relative; top: 8.5cm; left: 4cm; width=50%">
                    <xsl:value-of select="VOUCHER8/ACCOUNTNO" />
                </TD>
            </TR>
            <TR>
                <TD style="position: relative; top: 8.5cm; left: 3.7cm; width=50%">
                    <xsl:value-of select="VOUCHER7/VOUCHERNO" />
                </TD>
                <TD style="position: relative; top: 8.5cm; left: 4cm; width=50%">
                    <xsl:value-of select="VOUCHER8/VOUCHERNO" />
                </TD>
            </TR>
            <TR>
                <TD style="position: relative; top: 10.5cm; left: 3.7cm; width=50%">
                    <xsl:value-of select="VOUCHER9/ACCOUNTNO" />
                </TD>    
                <TD style="position: relative; top: 10.5cm; left: 4cm; width=50%">
                    <xsl:value-of select="VOUCHER10/ACCOUNTNO" />
                </TD>
                </TR>
            <TR>
                <TD style="position: relative; top: 10.5cm; left: 3.7cm; width=50%">
                    <xsl:value-of select="VOUCHER9/VOUCHERNO" />
                </TD>
                <TD style="position: relative; top: 10.5cm; left: 4cm; width=50%">
                    <xsl:value-of select="VOUCHER10/VOUCHERNO" />
                </TD>
            </TR>
        </TABLE>

        <xsl:apply-templates select="PB" />
    </xsl:template>

    <xsl:template match="PB">
        <br class="pageBreak" />
    </xsl:template>

</xsl:stylesheet>


