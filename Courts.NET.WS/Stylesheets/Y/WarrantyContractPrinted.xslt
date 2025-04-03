<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <HTML>
      <HEAD>
        <TITLE></TITLE>
        <meta name="vs_showGrid" content="True" />
        <META content="Microsoft Visual Studio 7.0" name="GENERATOR" />
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
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
      <DIV style="Z-INDEX: 139; LEFT: 0.1cm; WIDTH: 3.832cm; POSITION: absolute; TOP: 0.503cm; HEIGHT: 0.11cm">
        <IMG style="WIDTH: 114px; HEIGHT: 50px" height="110" alt="" src="CourtsMalaysiaLogo.jpg" width="114" />
      </DIV>
      <DIV class="RFHead2" style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 140; LEFT: 14.9cm; BORDER-LEFT: gray 1px solid; WIDTH: 4cm; PADDING-TOP: 10px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 0.1cm; HEIGHT: 1cm" align="center">
        <!--<xsl:value-of select="COPY" />-->
      </DIV>
      <DIV class="RFHead1" style="BORDER-RIGHT: gray 1px solid; PADDING-RIGHT: 5px; BORDER-TOP: gray 1px solid; PADDING-LEFT: 5px; Z-INDEX: 138; LEFT: 12cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 6.886cm; PADDING-TOP: 5px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 1.3cm; HEIGHT: 1.25cm">
        <TABLE class="RFHead1" id="Table20" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD width="50%">
              NO.JAMINAN<br/>
              WARRANTY NO:
            </TD>
            <TD width="50%">
              <xsl:value-of select="CONTRACTNO" />
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="Z-INDEX: 137; LEFT: 0.61cm; WIDTH: 18cm; POSITION: absolute; TOP: 3.518cm; HEIGHT: 1.3cm" align="center">
        <IMG style="WIDTH: 656px; HEIGHT: 51px" height="51" alt="" src="ServiceContract.jpg" width="656" />
      </DIV>
      <DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 136; LEFT: 0cm; BORDER-LEFT: gray 1px solid; WIDTH: 19cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 6.3cm; HEIGHT: 7.4cm"></DIV>
      <DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 135; LEFT: 0cm; BORDER-LEFT: gray 1px solid; WIDTH: 19cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 14.4cm; HEIGHT: 11.88cm"></DIV>
      <DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 130; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 6.5cm; HEIGHT: 0.003cm">
        <TABLE class="RFHead2" id="Table1" height="40" cellSpacing="1" cellPadding="6" width="100%">
          <TR>
            <TD align="middle" width="50%" bgColor="silver">SALES No.</TD>
            <TD width="50%">
              <xsl:value-of select="SOLDBY" />
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 131; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 7.9cm; HEIGHT: 1.15cm">
        <TABLE class="RFHead2" id="Table2" height="40" cellSpacing="1" cellPadding="6" width="100%">
          <TR>
            <TD align="middle" width="50%" bgColor="silver">BRANCH</TD>
            <TD width="50%">
              <xsl:value-of select="BRANCHNAME" />
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 132; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 9.35cm; HEIGHT: 1.15cm">
        <TABLE class="RFHead2" id="Table3" height="40" cellSpacing="1" cellPadding="6" width="100%">
          <TR>
            <TD vAlign="top" align="middle" width="50%" bgColor="silver">
              NO PASAR RAYA<br/>STORE No.
            </TD>
            <TD width="50%">
              <xsl:value-of select="STORENO" />
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 133; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 10.8cm; HEIGHT: 1.15cm">
        <TABLE class="RFHead2" id="Table4" height="40" cellSpacing="1" cellPadding="6" width="100%">
          <TR>
            <TD vAlign="top" align="middle" width="50%" bgColor="silver">
              TARIKH<br/>DATE
            </TD>
            <TD width="50%">
              <xsl:value-of select="TODAY" />
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 134; LEFT: 12.6cm; BORDER-LEFT: gray 1px solid; WIDTH: 6cm; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.2cm; HEIGHT: 1.15cm">
        <TABLE class="RFHead2" id="Table5" height="40" cellSpacing="1" cellPadding="6" width="100%">
          <TR>
            <TD vAlign="top" align="middle" width="50%" bgColor="silver">
              DIJUAL OLEH<br/>SOLD BY
            </TD>
            <TD width="50%">
              <xsl:value-of select="SOLDBYNAME" />
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 118; LEFT: 2.55cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
        <b>
          <xsl:value-of select="ACCTNO1" />
        </b>
      </DIV>
      <DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 119; LEFT: 3.3cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
        <b>
          <xsl:value-of select="ACCTNO2" />
        </b>
      </DIV>
      <DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 120; LEFT: 4.05cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
        <b>
          <xsl:value-of select="ACCTNO3" />
        </b>
      </DIV>
      <DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 121; LEFT: 4.8cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
        <b>
          <xsl:value-of select="ACCTNO4" />
        </b>
      </DIV>
      <DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 122; LEFT: 5.55cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
        <b>
          <xsl:value-of select="ACCTNO5" />
        </b>
      </DIV>
      <DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 123; LEFT: 6.3cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
        <b>
          <xsl:value-of select="ACCTNO6" />
        </b>
      </DIV>
      <DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 124; LEFT: 7.05cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
        <b>
          <xsl:value-of select="ACCTNO7" />
        </b>
      </DIV>
      <DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 125; LEFT: 7.8cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
        <b>
          <xsl:value-of select="ACCTNO8" />
        </b>
      </DIV>
      <DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 126; LEFT: 8.55cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
        <b>
          <xsl:value-of select="ACCTNO9" />
        </b>
      </DIV>
      <DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 127; LEFT: 9.3cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
        <b>
          <xsl:value-of select="ACCTNO10" />
        </b>
      </DIV>
      <DIV class="WarrantyHeader" style="BORDER-TOP: gray 1px solid; Z-INDEX: 128; LEFT: 10.05cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
        <b>
          <xsl:value-of select="ACCTNO11" />
        </b>
      </DIV>
      <DIV class="WarrantyHeader" style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; Z-INDEX: 129; LEFT: 10.8cm; PADDING-BOTTOM: 5px; BORDER-LEFT: gray 1px solid; WIDTH: 0.75cm; PADDING-TOP: 15px; BORDER-BOTTOM: gray 1px solid; POSITION: absolute; TOP: 12.05cm; HEIGHT: 1.35cm; TEXT-ALIGN: center">
        <b>
          <xsl:value-of select="ACCTNO12" />
        </b>
      </DIV>
      <DIV style="Z-INDEX: 115; LEFT: 0.3cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 6.4cm; HEIGHT: 0.317cm">
        <TABLE class="WarrantyHeader" id="Table6" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" width="20%">
              En./Mr.<br/>
              Pn./Mrs.<br/>
              Cik/Ms.
            </TD>
            <TD vAlign="top" width="50%">
              NAMA/NAME<br />
              <br />
              <b>
                <xsl:value-of select="FIRSTNAME" />
              </b> <b>
                <xsl:value-of select="LASTNAME" />
              </b>
            </TD>
            <TD vAlign="top" width="30%">
              NO.KP/I/C NO.<br />
              <br />
              <b>
                <xsl:value-of select="CUSTOMERID" />
              </b>
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="Z-INDEX: 116; LEFT: 0.3cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 8.2cm; HEIGHT: 2.1cm">
        <TABLE class="WarrantyHeader" id="Table7" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" width="35%">ALAMAT/ADDRESS</TD>
            <TD vAlign="top" width="65%">
              <b>
                <xsl:value-of select="ADDRESS1" />
                <br/>
                <xsl:value-of select="ADDRESS2" />
                <br />
                <xsl:value-of select="ADDRESS3" />
                <br />
                <xsl:value-of select="POSTCODE" />
              </b>
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="Z-INDEX: 117; LEFT: 0.3cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 10.5cm; HEIGHT: 0.317cm">
        <TABLE class="WarrantyHeader" id="Table8" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" width="30%">
              NOMBOR HUBUNGAN<br/>
              CONTACT NUMBERS
            </TD>
            <TD vAlign="top" width="35%">
              RUMAH/HOME<br/><br/>
              <b>
                <xsl:value-of select="HOMETEL" />
              </b>
            </TD>
            <TD vAlign="top" width="35%">
              PEJABAT/OFFICE<br/><br/>
              <b>
                <xsl:value-of select="WORKTEL" />
              </b>
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 100; LEFT: 0.292cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 7.991cm; HEIGHT: 0.2cm"></DIV>
      <DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 101; LEFT: 0.292cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 10.3cm; HEIGHT: 0.2cm"></DIV>
      <DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 102; LEFT: 0.292cm; WIDTH: 11.5cm; POSITION: absolute; TOP: 11.8cm; HEIGHT: 0.2cm"></DIV>
      <DIV style="Z-INDEX: 114; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 14.7cm; HEIGHT: 0.317cm">
        <TABLE class="WarrantyHeader" id="Table9" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" width="40%">
              DATE OF PRODUCT PURCHASE<br/><br/>
              <b>
                <xsl:value-of select="DATEOFPURCHASE" />
              </b>
            </TD>
            <TD vAlign="top" width="40%">
              PRODUCT CODE BARANGAN<br/><br/>
              <b>
                <xsl:value-of select="ITEMNO" />
              </b>
            </TD>
            <TD vAlign="top" width="20%">
              UNTUK KEGUNAAN PEJABAT<BR/>
              FOR OFFICE USE
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="Z-INDEX: 103; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 16.1cm; HEIGHT: 0.317cm">
        <TABLE class="WarrantyHeader" id="Table10" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" width="40%">
              JENAMA/BRAND / MODEL/MODEL<br/><br/>
              <b>
                <xsl:value-of select="ITEMDESC1" />
                <br/>
                <xsl:value-of select="ITEMDESC2" />
              </b>

            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="Z-INDEX: 104; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 18cm; HEIGHT: 0.317cm">
        <TABLE class="WarrantyHeader" id="Table11" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" width="30%">HARGA BARANGAN/PRICE OF GOODS</TD>
            <TD vAlign="top" width="70%">
              <b>
                <xsl:value-of select="ITEMPRICE" />
              </b>
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="Z-INDEX: 105; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 18.7cm; HEIGHT: 0.317cm">
        <TABLE class="WarrantyHeader" id="Table12" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" width="40%">
              PRODUCT CODE WARRANTY<br/>
              <b>
                <xsl:value-of select="WARRANTYNO" />
              </b>
            </TD>
            <TD vAlign="top" width="60%">
              BAYARAN JAMINAN/PRICE OF WARRANTY*<br/>
              <b>
                <xsl:value-of select="WARRANTYPRICE" />
              </b>
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="Z-INDEX: 106; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 19.7cm; HEIGHT: 0.317cm">
        <TABLE class="WarrantyHeader" id="Table13" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" width="40%">
              SERVICE CONTRACT DESCRIPTION<br/>
              <b>
                <xsl:value-of select="WARRANTYDESC1" />
                <br/>
                <xsl:value-of select="WARRANTYDESC2" />
              </b>
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <xsl:if test="TERMSTYPE='WC'">
        <DIV class="smallPrint" style="Z-INDEX: 106; LEFT: 0.3cm; WIDTH: 10cm; POSITION: absolute; TOP: 20.9cm; HEIGHT: 0.317cm">
          Warranty purchased on credit. Customer has <xsl:value-of select="WARRANTYCREDIT" /> days after purchase of stock item to pay for warranty otherwise warranty will expire.
        </DIV>
      </xsl:if>
      <DIV style="Z-INDEX: 107; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 21.4cm; HEIGHT: 0.317cm">
        <TABLE class="smallPrint" id="Table14" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" width="40%">*includes sales tax where applicable</TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="Z-INDEX: 108; LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 22cm; HEIGHT: 0.317cm">
        <TABLE class="WarrantyHeader" id="Table15" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" width="50%">
              TARIKH PENGHANTARAN BARANGAN/PLANNED DATE OF DELIVERY<br/>
              <b>
                <xsl:value-of select="PLANNEDDELIVERY" />
              </b>
            </TD>
            <TD vAlign="top" width="50%">
              PELUPUSAN JAMINAN LANJUTAN/EXPIRY OF SERVICE CONTRACT<br/>
              <b>
                <xsl:value-of select="EXPIRYOFWARRANTY" />
              </b>
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 109; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 15.982cm; HEIGHT: 0.2cm"></DIV>
      <DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 110; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 17.833cm; HEIGHT: 0.2cm"></DIV>
      <DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 111; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 18.6cm; HEIGHT: 0.2cm"></DIV>
      <DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 112; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 19.632cm; HEIGHT: 0.2cm"></DIV>
      <DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 113; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 21.933cm; HEIGHT: 0.2cm"></DIV>
      <DIV style="BORDER-TOP: gray 1px solid; Z-INDEX: 113; LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 22.933cm; HEIGHT: 0.2cm"></DIV>
      <DIV style="LEFT: 1.295cm; WIDTH: 16.5cm; POSITION: absolute; TOP: 23.099cm; HEIGHT: 1cm">
        <TABLE class="WarrantyFooter" id="Table16" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" align="middle" width="40%">
              IMPORTANT - PLEASE READ THE TERMS &amp;
              CONDITIONS BEFORE SIGNING
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="LEFT: 0.292cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 24.369cm; HEIGHT: 0.317cm">
        <TABLE class="smaller" id="Table17" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" width="40%">
              Saya di sini telah membaca dan telah faham dan menerima
              istilah dan syarat-syarat disebelah.<br/>
              I hereby acknowledge having read and understood and accept the terms and conditions.
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="LEFT: 0.3cm; WIDTH: 18.2cm; POSITION: absolute; TOP: 25cm; HEIGHT: 0.317cm">
        <TABLE class="WarrantyHeader" id="Table18" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="bottom" width="60%">TANDA TANGAN PELANGGAN/CUSTOMER SIGNATURE</TD>
            <TD vAlign="bottom" align="middle" width="40%">
              <b>
                <xsl:value-of select="TODAY" />
                <br/>
                <br/>
              </b>
              TARIKH/DATE
            </TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="LEFT: 1.295cm; WIDTH: 16.5cm; POSITION: absolute; TOP: 26.67cm; HEIGHT: 1cm">
        <TABLE class="WarrantyFooter" id="Table19" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD vAlign="top" align="middle">PLEASE CHECK ALL SECTIONS ARE COMPLETE</TD>
          </TR>
        </TABLE>
      </DIV>
      <DIV style="LEFT: 14.3cm; WIDTH: 4.681cm; POSITION: absolute; TOP: 14.394cm; HEIGHT: 7.564cm" ms_positioning="FlowLayout">
        <IMG style="WIDTH: 177px; HEIGHT: 286px" height="286" alt="" src="grey.jpg" width="177" />
      </DIV>
      <br class="pageBreak" />
      <div style="POSITION: relative">
        <DIV class="smallSS" style="FONT-WEIGHT: bold; Z-INDEX: 100; WIDTH: 20cm; POSITION: absolute; HEIGHT: 1cm; TEXT-ALIGN: center" ms_positioning="FlowLayout">
          COURTS
          ISTILAH DAN SYARAT-SYARAT/TERMS AND CONDITIONS
        </DIV>
        <DIV class="smallPrint" style="LIST-STYLE-POSITION: outside; Z-INDEX: 101; LEFT: 20px; WIDTH: 9.5cm; LIST-STYLE-TYPE: disc; POSITION: absolute; TOP: 0.5cm; HEIGHT: 5.279cm; TEXT-ALIGN: justify" ms_positioning="FlowLayout">
          <b>Kontrak Ini</b>
          <UL>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Kontrak ini meliputi bahagian dan tenega kerja sehubungan dengan
                barangan yang tertera dalam Resit Jaminan Jualan Courts("Barangan") untuk kerosakan mekanikal
                dan elektrikal sahaja seperti yang telah diperuntukkan oleh pengilang Barangan dan
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Melanjutkan jaminan perlindungan untuk tempoh seterusnva bermula
                pada tarikh tamatnya tempoh jaminan terdahulu pengilang dan berakhir pada Hari Tamatnya Waktu
                untuk Perlindungan Jaminan Lanjutan seperti yang dinyatakan di dalam Resit Jaminan Jualan Courts
                bagi barangan tersebut("Tempoh").  Ini bermakna bahawa termasuk tempoh jaminan pengilang, Barangan
                akan mempunyai jumlah tempoh jaminan selama lima(5) atau tiga(3) tahun dari tarikh penghantaran
                Barangan berdasarkan penjelasan anda bagi harga yang dikenakan, dan pilihan-pilihan yang ditunjukkan
                di hadapan perjanjian ini dan
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Semsa "Tempoh", barangan yang boleh dibawa harus dihantar ke cawangan
                Courts yang dilantik.  Semua hal-hal bersangkutan yang lain tertaklik kepada perkhidmatan pemanggilan
                luar.
              </DIV>
            </LI>
          </UL>
          <b>Kelayakan Barangan</b>
          <br/>
          Kontrak Perkhidmatan Jaminan Lanjutan Courts hanya melindungi Barangan yang:-
          <UL>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                baru dibeli dari Courts.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                dibuat untuk kegunaan di Malaysia.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                disertakan - pada masa pembelian - jaminan pengilang
                yang tulen, lengkap dan sah digunkan di Malaysia.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                berkenaan barangan pengguna seperti barangan elektronik
                dan peralatan-peralatan penting, penggunaannya dihadkan atau telah dihadkan kepada
                penggunaan tempatan dan peribadi.  Peralatan pejabat seperti mesin faksimili, mesin
                salinan dan komputer juga dilindungi di bawah penggunaan di rumah dan pejabat.
              </DIV>
            </LI>
          </UL>
          <b>Berkenaan Kerja Pembetulan</b>
          <UL>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                jika tempoh jaminan pengilang bagi Barangan belum berakhir,
                sila hubungi cawangan Courts yang berdekatan dengan anda atau agen perkhidmatan yang
                telah dicadangkan oleh mereka sendiri.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                jika tempoh jaminan pengilang telah berkakhir dan Barangan
                tersebut tertakluk kepada "Tempoh", Telefon 1800-18-1108.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                setiap kerja pembetulan hanya mesti dilakukan oleh agen yang
                telah dilantik oleh Courts dan tertakluk kepada pra-pemberian kuasa dari Courts attau
                Pentadbir yang ditujukan oleh Courts.
              </DIV>
            </LI>
          </UL>
          <b>
            BAGI SEMUA KERJA PEMBETULAN, RESIT JAMINAN JUALAN COURTS UNTUK BARANGAN MESTI DITUNJUKKAN BERSAMA
            KONTRAK INI.
          </b>
          <br/>
          <br/>
          <b>Pengecualian dari Jaminan Lanjutan</b>
          <UL>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Barangan yang masih dilindungi oleh Jaminan tulen
                pengilang.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Bahagian barangan yang kosmetik dan tidak beropersai,
                cat, atau sipan barangan.  Aksesori yang digunakan dalam atau bersama Barangan
                melainkan yang dilindungi oleh Kontrak Perkhidmatan Jaminan Lanjutan yang tersendiri,
                kabel, kord, kepala pencetak, katrij dan styli, peralatan lain seperti toner, riben,
                dram, tali, software atau pita, pilihan alat tambahan yang digabungkan dalam
                Barangan yang mana kontrak ini telah dibeli atau kerosakan pada bahagian-bahagian
                Barangan yang disebabkan dari kegunaan biasa yang tidak menghalang fungsi Barangan
                atau perkhidmatan biasa.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Penjagaan biasa, pembersihan, pelicinan, penyelarasan
                atau penjajaran.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Kerosakan yang disebabkan oleh kecurian, rompakan dan
                kemalangan termasuk gempa bumi, ribut dan atau taufan, kecuaian, penyalahgunaan,
                pasir, air, api, banjir, petir, pesawat, pelanggaran, korosi, pembocoran bateri,
                Tindakan Yang Maha Kuasa, potongan atau penambahan tenga, kekurangan atau kesalahan
                volta atau karan, pencerobohan atau pengumpulan binatang atau serangga.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Kos pemindahan atau pemasangan balik Barangan melainkan
                yang telah disertakan khas dalam kontrak anda.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Masalah penerimaan atau penghantaran yang berpunca dari
                gangguan luran.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Masalah atau kerosakan yang tidak dilindungi oleh Jaminan
                dasar pengilang.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Bateri, glob, di dalam atau di luar Barangan.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Kerosakan disebabkan oleh virus komputer atau
                penjajaran semula pada Barangan
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Sebang kegagalan, kerosakan, sekatan atau
                perhentian pada Barangan atau sebarang fungsi Barangan atau fungsi
                sebarang komponen berkenaan disebabkan oleh, atau berpunca dari atau
                sehubungan dengan, secara langsung atau tidak, sebarang rekabentuk
                perancangan atau kesepadanan kejuruteraan, penghapusan atau kekurangan
                sempena insiden tahun 2000.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Masalah atau kerosakan yang disebabkan oleh
                pengubahan yang tidak dibenarkan atau kegagalan mematuhi arahan-arahan
                pemasangan, penggunaan atau penjagaan dari pengilang barangan.
              </DIV>
            </LI>
          </UL>
          <DIV style="POSITION: absolute; TOP: 21.3cm;">
            <b>Batas Tanggungjawab</b>
            <br/>
            Courts Mammoth Berhad(Courts) tidak akan bertannggungjawab ke atas seberang kehilangan
            atau kerosakan kepada orang atau harta benda, yang disebabkan oleh - secara langsung
            atau sebaliknya - penggunaan atau kegagalan menggunakan Barangan sejauh mana yang dibenarkan
            oleh undang-undang.  Kontrak perkhidmatan ini tidak melindungi seberang kerosakan yang tertakluk
            oleh pemanggilan kembali pengilang atau yang dilindungi di bawah program bayaran pennggantian wang pengilang.
            <br/>
            <b>Ini adalah Kontrak Perkhidmatan</b><br/>
            Kontrak perkhidmatan ini hanya sah di Malaysia.  Sekiranya terjadi kecurian, perampasan, penipuan,
            atau jualan kepada pihak lain, kontrak ini dibatalkan secara automatik dan tiada wang akan dikembalikan.
            Jika ada ansuran yang belum dijelaskan lebih dari sebulan, Courts berhak untuk membatalkan kontrak
            perkhidmatan ini tanpa penggantian wang.  Kontrak ini akan ditamatkan dengan serta merta tanpa pengembalian
            wang apabila Barangan itu digantikan oleh Courts.  Penamatan ini akan berkuat kuasa pada tarikh
            Barangan yang dilindungi telah digantikan.
            <br/>
            <b>Untuk Perkhidmatan dan pertanyaan, Sila Telefon Talian Bebas Tol 1800-18-1108</b><br/>
            Kontrak perkhidmatan ini ditawarkan oleh Courts Mammoth Berhad.  Courts berhak untuk mengubah, menukar
            atau sebaliknya memperbaiki Istilah Syarat-syarat yang telah tertera di sini.  Dalam apa-apa hal yang
            bertentangan, versi Bahasa Inggeris akan mengatasi versi Bahasa Malaysia.
          </DIV>
        </DIV>
        <DIV class="smallPrint" style="LEFT: 10.5cm; WIDTH: 9cm; POSITION: absolute; TOP: 0.5cm; HEIGHT: 18.737cm; TEXT-ALIGN: justify" ms_positioning="FlowLayout">
          <b>This Contract</b>
          <UL>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                This contract applies to part and labour in respect of the prduct
                stated on the Courts Warranty Sales Receipt(the "Product") for mechanical and electrical defects
                only and only to the extent provided by the manufacturer of the Product and
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Extends the warranty cover for a further period commencing on the
                date of expiry of the manufacturer's initial warranty period and ending on the Date of Expiry of
                Extended Warranty stated on the Courts Warranty Sales Receipt for that Product(the "Term").  This
                means that, inclusive of the manufacturer's warranty period, the product will have a total of
                either five(5) years warranty or three(3) years warranty from the date of delivery of the product
                based on your payment of the applicable price, and selections indicated on the front of this
                agreement and
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                During the Term, portable items should be brought to the nominated Courts'
                store.  All other items will be subject to call out service
              </DIV>
            </LI>
          </UL>
          <b>Product Eligibility</b>
          <br/>
          A Courts Extended Warranty Service Contract only covers a Product which:-
          <UL>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                is purchased new from Courts.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                is manufactured for use Malaysia.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                included at the time of purchase the manufacturer's
                complete and original warranty valid in Malaysia.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                in the case of consumer products such as electronics
                and major appliances, the use is or has been limited to domestic and personal
                use.  Office products such as facsimile machines, copiers and computers are
                also covered for home and office use.
              </DIV>
            </LI>
          </UL>
          <b>For Repairs</b>
          <UL>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                if the manufacturer's warranty period for the product
                has not expired, contact your nearest Courts store or their recommended service
                agent directly.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                if the manufacturer's warranty period has expired and
                the Product is within the Term telephone toll free 1800-18-1108.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                repairs must only be carried out by a repairer nominated
                by Courts on each occasion, and is subject to pre-authorization by Courts or the
                Administrator appointed by Courts.
              </DIV>
            </LI>
          </UL>
          <b>
            FOR ALL REPAIRS, THE COURTS WARRANTY SALES RECEIPT FOR THE PRODUCT MUST BE PRESENTED WITH
            THIS CONTRACT.
          </b>
          <br/>
          <br/>
          <b>Exclusion from the Extended Warranty</b>
          <UL>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                products that are still covered by the original manufacturer's
                warranty.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Non operating and cosmetic items, paint, or product finish.
                Accessories used in or with the product unless covered under a seperate Extended Warranty
                Service Contract, cables, cords, print heads, cartidges and styli, other consumables
                such as toner, ribbons, drums, belts, tapes or software, add-on options incorporated
                in a Product for which this contract was purchased or normal wear and tear items not
                integral to the functioning of the Product, or routine service.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Routine maintenance, cleaning, lubrication, adjustments or
                alignments.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Damage caused by theft, burglary and accident including
                earthquake, storm and or tempest, neglect, abuse, misuse, theft, sand, water
                negligence, fire, flood, lightning, malicious damage, aircraft, impact, corrosion
                battery leakage, accts of God, power outages or surges, inadequate or improper voltage
                or current, animal or insect infestation or intrusion.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Cost of removal or re-installation of the Product unless
                specifically included in your contract.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Reception or transmission problems resulting from external
                causes.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Problems or defects not covered under the manufacturer's
                primary written warranty.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Batteries, globes, internal or external to the product.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Breakdowns caused by computer virus or realignments
                to Products.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Any failure, malfunction, defect, inoperability
                breakdown, disruption or stoppage of the Product or any Product functions
                or the function of any related components caused by arising from or related to
                directly or indirectly to any design, planning or engineering compatibility,
                omission or deficiency with the incidence of the Year 2000.
              </DIV>
            </LI>
            <LI>
              <DIV style="MARGIN-LEFT: -20px">
                Problems or defects caused by unauthorised
                modifications or failure to follow the manufacturer's installation, operation
                or maintenance instructions.
              </DIV>
            </LI>
          </UL>
          <DIV style="POSITION: absolute; TOP: 20.5cm;">
            <b>Limitation of Liability</b>
            <br/>
            Courts Mammoth Berhad(Courts) shall not be responsible for any loss or damage to a person
            or property, direct, consequential or identical arising from the use of, or inability
            to use the Product to the extent that such may be discaimed by law.  This service contract
            does not cover any defects which are subject to a manufacturer's recall or which are
            covered under a manufacturer's program of reinbursement.
            <br/>
            <br/>
            <b>This Is A Service Contract</b><br/>
            This service contract is only valid in Malaysia.  It is not transferable.  In the event of
            theft, reposession, fraud or sale to another party, the service contract is automatically
            cancelled with no refund.  If any instalment payment is more than 1 month overdue, Courts
            reserves the right to cancel the service agreement with no refund.  This contract will
            prematurely terminate with no refund when a product is replaced by Courts.  Termination will
            take effect on the date that the covered Product is replaced.
            <br/>
            <br/>
            <b>For service and enquiries phone 1800-18-1108 toll free</b><br/>
            This service contract is offered by Courts Mamoth Berhad.  Courts reserves the righ to vary,
            modify or otherwise amend the Terms and Conditions stated herein.  In the event of any inconsistency,
            the Englidh version shall prevail.
          </DIV>
        </DIV>
      </div>
      <xsl:variable name="last" select="LAST" />
      <xsl:if test="$last != 'TRUE'">
        <br class="pageBreak" />
      </xsl:if>
    </div>
  </xsl:template>
</xsl:stylesheet>

