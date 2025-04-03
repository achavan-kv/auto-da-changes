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
                <xsl:apply-templates select="FOODLOSS" />
            </BODY>
        </HTML>
    </xsl:template>

    <xsl:template match="FOODLOSS">
        <xsl:apply-templates select="HEADER" />
        <xsl:apply-templates select="ITEMS" />
        <xsl:apply-templates select="FOOTER" />
    </xsl:template>

    <xsl:template match="HEADER">

        <div class="DateBold" style="LEFT: 250px; WIDTH: 150px; POSITION: absolute; TOP: 93px; HEIGHT: 30px">
            <span class="HPHeader">
                FOOD LOSS
            </span>
        </div>

        <div class="smallSS" style="LEFT: 450px; WIDTH: 200px; POSITION: absolute; TOP: 93px; HEIGHT: 30px">
            Date Logged: <xsl:value-of select="DATELOGGED" />
        </div>

        <div class="smallSS" style="LEFT: 30px; WIDTH: 159px; POSITION: absolute; TOP: 150px; HEIGHT: 31px">
            <b>
                ID/IC No.         <br/>
                AccountNo.        <br/>
                ServiceRequestNo. <br/>
            </b>
        </div>

        <div class="smallSS" style="LEFT: 200px; WIDTH: 159px; POSITION: absolute; TOP: 150px; HEIGHT: 31px">
            <xsl:value-of select="CUSTID" />
            <br/>
            <xsl:value-of select ="ACCTNO"/>
            <br/>
            <xsl:value-of select ="SERVICEREQUESTNO"/>
        </div>

        <div class="smallSS" style="LEFT: 30px; WIDTH: 159px; POSITION: absolute; TOP: 215px; HEIGHT: 31px">
            <b>
                Title, First Name, Surname
                <br/>Address
                <br/>
            </b>
        </div>

        <div class="smallSS" style="POSITION: absolute; LEFT: 200px; TOP: 215px; WIDTH: 159px; ">
            <xsl:value-of select="NAME" />
            <br />
            <xsl:value-of select="ADDR1" />
            <br/>
            <xsl:value-of select="ADDR2" />
            <br/>
            <xsl:value-of select="ADDR3" />
            <br/>
            <xsl:value-of select="POSTCODE" />
        </div>
    </xsl:template>

    <xsl:template match="ITEMS">
        <div class="smallSS" style="POSITION: absolute; LEFT: 20px; TOP: 325px;">
            <TABLE id="Items" width="90%" border="0">
                <TR>
                    <TD align="left" width="5%">                        
                    </TD>
                    <TD align="left" width="80%">
                        Item Description
                    </TD>
                    <TD align="right" width="20%">
                        Item Value
                    </TD>
                </TR>
                <xsl:apply-templates select="ITEM" />
                <TR>
                   <TD align="left">                        
                    </TD>
                    <TD align="left">
                        Total
                    </TD>
                    <TD align="right">
                        <xsl:value-of select="ITEMTOTAL" />
                    </TD>
                </TR>
            </TABLE>
        </div>
    </xsl:template>

    <xsl:template match="ITEM">
        <TR>
            <xsl:apply-templates select="BULLETEDNUMBER" />
            <xsl:apply-templates select="DESC" />
            <xsl:apply-templates select="VALUE" />
        </TR>
    </xsl:template>

     <xsl:template match="BULLETEDNUMBER">
        <TD align="left">
            <xsl:apply-templates />
        </TD>
    </xsl:template>
    
    <xsl:template match="DESC">
        <TD align="left">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="VALUE">
        <TD align="right">
            <xsl:apply-templates />
        </TD>
    </xsl:template>

    <xsl:template match="FOOTER">

    </xsl:template>

</xsl:stylesheet>

