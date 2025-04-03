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

        <div class="DateBold" style="LEFT: 50px; WIDTH: 750px; POSITION: absolute; TOP: 93px; HEIGHT: 30px">
            <table width ="100%">
                <tr>
                    <xsl:if test="EXTWARRANTY='Y'">
                        <td width="20%">
                            <IMG  alt="" src="Supashield logo BW final.jpg" style="height:100px;width:100px;" />
                        </td>
                    </xsl:if>
                    <td>
                        <span class="HPHeader">
                            Courts
                            <xsl:if test="EXTWARRANTY='Y'">
                                Supashield Extended Warranty<BR />
                            </xsl:if>
                            Food Spoilage Declaration Claim Form
                        </span>
                    </td>
                </tr>
            </table>
        </div>
        
        <div style="LEFT: 30px; WIDTH: 750px; POSITION: absolute; TOP: 225px; HEIGHT: 50px; ">
            <span class="HPHeader">Customer Information</span>
        </div>

        <div style="LEFT: 30px; WIDTH: 500px; POSITION: absolute; TOP: 280px; HEIGHT: 30px">
            <b>
                Date:
                <br/>Name:
                <br/>Address:
                <br/>
                <Br/>
                <br/>
                <br/>Home Phone:
                <br/>Mobile Phone:
            </b>
        </div>

        <div style="POSITION: absolute; LEFT: 350px; TOP: 280px; WIDTH: 450px; ">
            <xsl:value-of select="NOW" />
            <br/>
            <xsl:value-of select="NAME" />
            <br />
            <xsl:value-of select="ADDR1" />
            <br/>
            <xsl:value-of select="ADDR2" />
            <br/>
            <xsl:value-of select="ADDR3" />
            <br/>
            <xsl:value-of select="POSTCODE" />
            <br/>
            <xsl:value-of select="HOMETEL" />
            <br/>
            <xsl:value-of select="MOBILE" />
        </div>

        <div  style="LEFT: 30px; WIDTH: 300px; POSITION: absolute; TOP: 450px; HEIGHT: 31px">
            <b>
                Model No:         <br/>
                Account No:        <br/>
                Contract No: <br/>
            </b>
        </div>

        <div style="LEFT: 350px; WIDTH: 450px; POSITION: absolute; TOP: 450px; HEIGHT: 30px">
            <xsl:value-of select="MODELNO" />
            <br/>
            <xsl:value-of select ="ACCTNO"/>
            <br/>
            <xsl:value-of select ="CONTRACTNO"/>
        </div>

        <div style="LEFT: 30px; WIDTH: 750px; POSITION: absolute; TOP: 515px; HEIGHT: 50px;">
            <span class="HPHeader">Claim Information</span>
        </div>
        <div  style="LEFT: 30px; WIDTH: 500px; POSITION: absolute; TOP: 575px; HEIGHT: 30px">
            <b>
                Date Logged: <br/>
                Service Request: <br/>
                Fault:
            </b>
        </div>


        <div  style="LEFT: 350px; WIDTH: 450px; POSITION: absolute; TOP: 575px; HEIGHT: 31px">
            <xsl:value-of select="DATELOGGED" />
            <br/>
            <xsl:value-of select="SERVICEREQUESTNO" />
            <br/>
            <xsl:value-of select="TECHNICIANNOTES" />
        </div>

        <div  style="LEFT: 30px; WIDTH: 500px; POSITION: absolute; TOP: 650px; HEIGHT: 30px">
            <b>
                Technician Name:        <br/>
                Branch: <br/>
                Total Value of Food Spoiled: <br/>
            </b>
        </div>

        <div  style="LEFT: 350px; WIDTH: 450px; POSITION: absolute; TOP: 650px; HEIGHT: 31px">
            <xsl:value-of select ="TECHNICIANNAME"/>
            <br/>
            <xsl:value-of select ="BRANCHNAME"/>
            <br/>
            Rs <xsl:value-of select ="../ITEMS/TOTAL/VALUE"/>
        </div>

        <div style="LEFT: 30px; WIDTH: 750px; POSITION: absolute; TOP: 715px; HEIGHT: 50px;">
            <span class="HPHeader">Certification</span>
        </div>
        
        <div style="POSITION: absolute; LEFT: 20px; TOP: 800px;">
            <TABLE id="certification" width="90%" border="0">
                <TR>
                    <TD align="left" width="80%">
                        ________________________________________________________
                        <br /><b> Customer's/Claimant Signature</b>
                    </TD>

                    <TD align="left" width="20%" style="Vertical-align:top;">
                        DATE:_______________________________
                    </TD>
                </TR>
                <TR>
                    <TD align="left" colspan="2">
                        I heareby certify that the statements contained herin are
                        true and that all material facts pertaining to this food
                        spoilage claim are correct.
                    </TD>
                </TR>
                <TR>
                    <TD align="left" width="80%">
                        <br/>
                        ________________________________________________________
                        <br /><b> Technician's/CSR Signature</b>
                        <br />On Behalf of Courts(Mauritius) Ltd
                        <br />Courts(Mauritius) Ltd
                    </TD>

                    <TD align="left" width="20%" style="Vertical-align:top;">
                        <br/>
                        DATE:_______________________________
                    </TD>
                </TR>
                <TR>
                    <TD align="right" colspan="2">
                        <br/>
                        ________________________________________________________
                        <br /><b> Technical Manager's Approval</b>
                    </TD>
                </TR>
                <TR>
                    <TD align="left" colspan="2">
                        <i>
                            Food Spoilage Declaration Form to be duly completed
                            and forwarded to the attention of the Technician Manager,
                            Trianon Warehouse for processing of claim for payment
                        </i>
                    </TD>
                </TR>
            </TABLE>
        </div>
    </xsl:template>

    <xsl:template match="ITEMS">
        <br class="pageBreak" />
        <div>
            <TABLE id="head" width="750px" border="0">
                <TR>
                    <TD align="left" width="200px">
                        To:
                    </TD>

                    <TD align="left" width="450px">
                        <b>Electrical Service Department</b>
                        <br/>Courts Trianon
                    </TD>
                </TR>
                <TR>
                    <TD align="left" width="200px">
                        Attention:
                    </TD>

                    <TD align="left" width="450px">
                        The Technical Manager
                    </TD>
                </TR>
                <TR>
                    <TD align="left" width="200px">
                        <br/>
                        <xsl:if test="../HEADER/EXTWARRANTY='Y'">
                            <IMG  alt="" src="Supashield logo BW final.jpg" style="height:100px;width:100px;" />
                        </xsl:if>
                    </TD>

                    <TD align="left" width="450px">
                        <br/>
                        <B>
                            <U>List of Spoiled Food</U>
                        </B>
                    </TD>
                </TR>
            </TABLE>

            <DIV style="border: 2px solid; Padding: 5px; display: table; width: 510px;">  
              <TABLE id="Items" border="0" width="500px">
                  <TR>
                      <TD align="left" width="5%">                          
                      </TD>
                      <TD align="left" width="75%">
                          Item Description
                      </TD>
                      <TD align="right" width="20%">
                          Rs
                      </TD>
                  </TR>
                  <xsl:apply-templates select="ITEM" />                
              </TABLE>
            </DIV>
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

    <xsl:template match="FOOTER"></xsl:template>

</xsl:stylesheet>

