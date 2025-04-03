<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">



    <html>
      <head>
        <style type="text/css" media="all">
          @import url(styles.css);
          body,table{font-family:Helvetica; font-size:10}
        </style>
      </head>
      <body>
        <xsl:apply-templates select="AGREEMENTS" />
      </body>
    </html>


  </xsl:template>

  <xsl:template match="AGREEMENTS">
    <xsl:apply-templates select="AGREEMENT" />
    <P></P>
  </xsl:template>

  <!-- UAT 179 Agreement details to be printed in Bhasa as well-->
  <xsl:template match="AGREEMENT">
    <xsl:apply-templates select="PAGE" />
    <!--<xsl:variable name="last" select="LAST" />
    <xsl:if test="$last != 'TRUE'">
      <br class="pageBreak" />
    </xsl:if>-->

    <xsl:apply-templates select="BHASAPAGE" />

    <P></P>
  </xsl:template>

  <xsl:template match="PAGE">
    <div style="position:relative">

      <!-- start new page template -->


      <table>
        <tr>
          <td>
            <table>
              <tr>
                <td rowspan="2">
                  <img src="CourtsMalaysiaAgreementLogo.JPG" border ="0"/>
                </td>
                <td align="center" width="90%">
                  <h5>CHATTEL AGREEMENT, COURTS (M) SDN BHD (154820-D)</h5>
                </td>
              </tr>
              <tr>
                <td align="center" colspan="2">
                  <h5>HEAD OFFICE No. 36, Jalan Genting Kelang, Setapak, 53300 Kuala Lumpur, MALAYSIA</h5>
                </td>
              </tr>
              <tr>
                <td colspan="2">
                  I/We, the Customer and Co-Purchaser identified in the <b>First Schedule, PART I</b> below, hereby jointly and severally, irrevocably agree and confirm that
                  I/we make the Offer and Request as defined in and subject to the Standard Terms &amp; Conditions Governing the Chattel Agreement (attached hereto)
                  and this <b>First Schedule</b>. In that regard, I/we confirm as follows:
                </td>
              </tr>
              <tr>
                <td colspan="2">
                  <hr />
                </td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td>
            <table width="100%">
              <tr>
                <td colspan="3" align="center">
                  <h5>FIRST SCHEDULE</h5>
                </td>
              </tr>
              <tr>
                <td></td>
                <td>
                  <b>Title/Names (In block capitals) as per I.C of Customer</b>
                </td>
                <td>
                  <b>Account Number</b>
                </td>
              </tr>
              <tr>
                <td rowspan="2">
                  <h5>PART I COPY</h5>
                </td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/NAME" />
                </td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/ACCTNO" />
                </td>
              </tr>
              <tr>

                <td style="border-bottom:1px solid black;" colspan="2">
                  <!-- only display spouse here -->
                  <xsl:variable name="isSpouse" select="HEADER/RELATIONSHIP" />
                  <xsl:if test="$isSpouse = 'S'">
                    <!--IP - 19/05/08 - UAT(175) Do not display the Spouse-->
                    <!--xsl:value-of select="HEADER/JOINTNAME" />&#xA0;-->
                  </xsl:if>
                </td>

              </tr>
              <tr>
                <td>Full Postal Address :</td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/ADDR1" />
                </td>
                <td></td>
              </tr>
              <tr>
                <td>(in capital letters)</td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/ADDR2" />
                </td>
                <td></td>
              </tr>
              <tr>
                <td>State / Postcode :</td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/ADDR3" />
                  &#xA0;&#xA0;&#xA0;&#xA0;&#xA0;
                  <xsl:value-of select="HEADER/POSTCODE" />
                </td>
                <td></td>
              </tr>
              <tr>
                <td>New IC No. / Tel :</td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/CUSTID" />
                  &#xA0;&#xA0;&#xA0;&#xA0;&#xA0;
                  <xsl:value-of select="HEADER/HOMETEL" />
                </td>
                <td></td>
              </tr>
              <tr>
                <td>Co-Purchaser and New IC No. :</td>
                <td style="border-bottom:1px solid black;" colspan="2">
                  <!-- only display spouse here -->
                  <xsl:variable name="isJoint" select="HEADER/RELATIONSHIP" />
                  <xsl:if test="$isJoint = 'J'">
                    <xsl:value-of select="HEADER/JOINTNAME" />&#xA0;&#xA0;<xsl:value-of select="HEADER/JOINTID" />
                  </xsl:if>
                </td>
              </tr>
              <tr>
                <td>" Location " for delivery of Goods </td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/DELADDR1" />
                </td>
                <td></td>
              </tr>
              <tr>
                <td>(if address is not the same as above)</td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/DELADDR2" />
                </td>
                <td></td>
              </tr>
              <tr>
                <td>State / Postcode :</td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/DELADDR3" />
                  &#xA0;&#xA0;&#xA0;&#xA0;&#xA0;
                  <xsl:value-of select="HEADER/DELPOSTCODE" />
                </td>
                <td></td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td>
            <table width="100%">
              <tr>
                <td colspan="7" align="center" style="border-bottom:1px solid black;">
                  <h5>PART II</h5>
                </td>
              </tr>
              <tr>
                <td style="border-bottom:1px solid black;">
                  <b>Qty.</b>
                </td>
                <td style="border-bottom:1px solid black;">
                  <b>Product Code</b>
                </td>
                <!--UAT 179 Sales Invoice No, Brand, Model removed from stylesheet-->
                <td style="border-bottom:1px solid black;">
                  <b>
                    <!--Sales Invoice No.-->
                  </b>
                </td>
                <td style="border-bottom:1px solid black;">
                  <b>Particulars of Goods</b>
                </td>
                <td style="border-bottom:1px solid black;">
                  <b>
                    <!--Brand-->
                  </b>
                </td>
                <td style="border-bottom:1px solid black;">
                  <b>
                    <!--Model-->
                  </b>
                </td>
                <td style="border-bottom:1px solid black;">
                  <b>Cash Price</b>
                </td>
              </tr>
              <xsl:apply-templates select="LINEITEMS" />

              <tr>
                <td colspan="7" align="center" style="border-bottom:1px solid black;">&#xA0;</td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td>
            <table width="100%">
              <tr>
                <td rowspan="8">
                  <h5>PART III</h5>
                </td>
                <td>TOTAL GOODS CASH PRICE :</td>
                <td></td>
                <td align="right">
                  <b>
                    <xsl:value-of select="../FOOTER/GOODSVAL" />
                  </b>
                </td>
              </tr>
              <tr>
                <td>(Where applicable) Add Consolidated Amount from Account No/s :</td>
                <td></td>
                <td align="right">
                  <b></b>
                </td>
              </tr>
              <tr>
                <td>TOTAL CASH PRICE :</td>
                <td></td>
                <td align="right">
                  <b>

                  </b>
                </td>
              </tr>
              <tr>
                <td>
                  Add Charges for Deferred Payments Calculated @ <xsl:value-of select="../FOOTER/INTERESTRATE" /> per month (less Home Club Voucher discount, if applicable)
                </td>
                <td></td>
                <td align="right">
                  <b>
                    <xsl:value-of select="../FOOTER/DT" />
                    <!--UAT 164 DT added to stylesheet-->
                  </b>
                </td>
              </tr>
              <tr>
                <td>Total Amount Payable</td>
                <td>
                  <b></b>
                </td>
                <td align="right">
                  <b>
                    <xsl:value-of select="../FOOTER/TOTAL" />
                  </b>
                </td>
              </tr>
              <tr>
                <td>Deposit</td>
                <td></td>
                <td align="right">
                  <b>
                    <xsl:value-of select="../FOOTER/DEPOSIT" />
                  </b>
                </td>
              </tr>
              <!--UAT 164 Charge for Credit text to be removed-->
              <!--<tr>
                <td>Charge for Credit</td>    
              <td>
                  <b></b>
                </td>
                <td align="right">
                  <b>
                    <xsl:value-of select="../FOOTER/DT" />
                  </b>
                </td>
              </tr>-->
              <tr>
                <td>Balance Payable</td>
                <td>
                  <b></b>
                </td>
                <td align="right">
                  <b>
                    <xsl:value-of select="../FOOTER/BALANCE" />
                  </b>
                </td>
              </tr>
              <tr>
                <td colspan="4" align="center" style="border-bottom:1px solid black;">&#xA0;</td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td>
            <b>PART IV -</b> The Balance Payable shall be paid by <xsl:value-of select="../FOOTER/INSTALNO" /> monthly installments of <xsl:value-of select="../FOOTER/FIRSTINST" /> each and a final installment of <xsl:value-of select="../FOOTER/FINALINST" />, with the installment falling due and payable (i) on or before [&#xA0;&#xA0;][&#xA0;&#xA0;]/[&#xA0;&#xA0;][&#xA0;&#xA0;]/[&#xA0;&#xA0;][&#xA0;&#xA0;][&#xA0;&#xA0;][&#xA0;&#xA0;] (date/month/year)
            or (ii) if (i) is left blank, one calendar month after the delivery and/or provision of the Goods or any part thereof (valued at their respective Cash price plus Charges for
            Deferred Payment) which together with the Consolidated Amount (if applicable) total not less than 75% of the Total Amount Payable. For the avoidance of doubt, the date
            of delivery and/or provision of the Goods or part thereof (as the case may be) as recorded in Courts' delivery document shall be conclusive evidence binding against me,
            regardless of whether I accept delivery or not. The expression "Agreed Payment Date" shall be the day when the first installment becomes due and payable.
          </td>
        </tr>
        <tr>
          <td>
            <b>PART V -</b> I/We have read and understand the Standard Terms and Conditions Governing the Chattel Agreement (attached hereto) and
            the Standard Terms and Conditions Governing Courts Max Silver/Gold (if applicable,attached hereto) and this <b>
              First Schedule.
              IMPORTANT NOTE: I/We represent and declare that the Goods are purchased solely for my/our personal use and I/we shall not howsoever
              use the Goods as security in any manner whatsoever and in breach thereof, I/we acknowledged that the Courts is inter alia, entitled to
              demand for immediate payment of the Balance Payable and all Other Money payable to Courts and take legal action against me/us.
            </b>
          </td>
        </tr>
        <tr>
          <td>
            <table cellspacing="4" width="100%">
              <tr>
                <td>Dated:</td>
                <td>Signature/s of Customer/Co-Purchaser/s</td>
                <td>In the presence of:</td>
              </tr>
              <tr>
                <p></p>
              </tr>
              <tr>
                <p></p>
              </tr>
              <tr>
                <p></p>
              </tr>
              <tr>
                <td style="border-bottom:1px solid black;width:90px;">
                  <xsl:value-of select="HEADER/DATE" />&#xA0;
                </td>
                <td style="border-bottom:1px solid black">&#xA0;</td>
                <td style="border-bottom:1px solid black">&#xA0;</td>
              </tr>
              <tr>
                <td></td>
                <td></td>
                <td>Signature and name Courts (M) Sdn Bhd (154820-D) Staff</td>
              </tr>
            </table>
          </td>
        </tr>
      </table>


      <!-- end new page template -->


      <xsl:variable name="lastPage" select="LAST" />
      <xsl:variable name="lastAgreement" select="../LAST" />
      <xsl:if test="$lastPage = 'False' or $lastAgreement = 'FALSE' ">
        <br class="pageBreak" />
        <!-- if it's not the last page -->
      </xsl:if>
    </div>
  </xsl:template>

  <xsl:template match="BHASAPAGE">
    <div style="position:relative">

      <!-- start new page template -->


      <table>
        <tr>
          <td>
            <table>
              <tr>
                <td rowspan="2">
                  <img src="CourtsMalaysiaAgreementLogo.JPG" border ="0"/>
                </td>
                <td align="center" width="90%">
                  <h5>PERJANJIAN CATEL KEPADA,   COURTS (M) SDN BHD (154820-D)</h5>
                </td>
              </tr>
              <tr>
                <td align="center" colspan="2">
                  <h5>PEJABAT UTAMA No. 36, Jalan Genting Kelang, Setapak, 53300 Kuala Lumpur, MALAYSIA</h5>
                </td>
              </tr>
              <tr>
                <td colspan="2">
                  Saya/Kami, Pelanggan dan Pembeli Bersama yang dikenali di dalam <b>Jadual Pertama, BAHAGIAN 1</b> di bawah, dengan bersesama dan berasingan,
                  secara tidak boleh dibatalkan bersetuju dan mengesahkan bahawa
                  Saya/Kami membuat Tawaran dan Permohonan seperti yang di definasikan di dalam dan tertakluk kepada Terma &amp; Syarat Biasa Yang Menguasai Perjanjian Catel (yang dilampirkan di sini)
                  dan <b>Jadual Pertama</b>. ini. Dalam hal itu, Saya/Kami mengesahkan seperti berikut:
                </td>
              </tr>
              <tr>
                <td colspan="2">
                  <hr />
                </td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td>
            <table width="100%">
              <tr>
                <td colspan="3" align="center">
                  <h5>JADUAL PERTAMA</h5>
                </td>
              </tr>
              <tr>
                <td></td>
                <td>
                  <b>Gelaran/Nama (dalam huruf besar) seperti di dalam Kad Pengenalan Pelanggan</b>
                </td>
                <td>
                  <b>Nombor Akaun</b>
                </td>
              </tr>
              <tr>
                <td rowspan="2">
                  <h5>BAHAGIAN I SALINAN</h5>
                </td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/NAME" />
                </td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/ACCTNO" />
                </td>
              </tr>
              <tr>

                <td style="border-bottom:1px solid black;" colspan="2">
                  <!-- only display spouse here -->
                  <xsl:variable name="isSpouse" select="HEADER/RELATIONSHIP" />
                  <xsl:if test="$isSpouse = 'S'">
                    <!--IP - 19/05/08 - UAT(175) Do not display the Spouse-->
                    <!--xsl:value-of select="HEADER/JOINTNAME" />&#xA0;-->
                  </xsl:if>
                </td>

              </tr>
              <tr>
                <td>Alamat Pos Penuh :</td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/ADDR1" />
                </td>
                <td></td>
              </tr>
              <tr>
                <td>(dalam huruf besar)</td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/ADDR2" />
                </td>
                <td></td>
              </tr>
              <tr>
                <td>Negeri / Poskod :</td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/ADDR3" />
                  &#xA0;&#xA0;&#xA0;&#xA0;&#xA0;
                  <xsl:value-of select="HEADER/POSTCODE" />
                </td>
                <td></td>
              </tr>
              <tr>
                <td>No. K/P Baru / Tel :</td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/CUSTID" />
                  &#xA0;&#xA0;&#xA0;&#xA0;&#xA0;
                  <xsl:value-of select="HEADER/HOMETEL" />
                </td>
                <td></td>
              </tr>
              <tr>
                <td>No. K/P Baru Pembeli Bersama :</td>
                <td style="border-bottom:1px solid black;" colspan="2">
                  <!-- only display spouse here -->
                  <xsl:variable name="isJoint" select="HEADER/RELATIONSHIP" />
                  <xsl:if test="$isJoint = 'J'">
                    <xsl:value-of select="HEADER/JOINTNAME" />&#xA0;&#xA0;<xsl:value-of select="HEADER/JOINTID" />
                  </xsl:if>
                </td>
              </tr>
              <tr>
                <td>
                  "Lokasi" unutk penyerahan
                </td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/DELADDR1" />
                </td>
                <td></td>
              </tr>
              <tr>
                <td>(if address is not the same as above)</td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/DELADDR2" />
                </td>
                <td></td>
              </tr>
              <tr>
                <td>Negeri / Poskod :</td>
                <td style="border-bottom:1px solid black;">
                  <xsl:value-of select="HEADER/DELADDR3" />
                  &#xA0;&#xA0;&#xA0;&#xA0;&#xA0;
                  <xsl:value-of select="HEADER/DELPOSTCODE" />
                </td>
                <td></td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td>
            <table width="100%">
              <tr>
                <td colspan="7" align="center" style="border-bottom:1px solid black;">
                  <h5>BAHAGIAN II</h5>
                </td>
              </tr>
              <tr>
                <td style="border-bottom:1px solid black;">
                  <b>Bil.</b>
                </td>
                <td style="border-bottom:1px solid black;">
                  <b>Kod Produk</b>
                </td>
                <!--UAT 179 Sales Invoice No, Brand, Model removed from stylesheet-->
                <td style="border-bottom:1px solid black;">
                  <b>
                    <!--Sales Invoice No.-->
                  </b>
                </td>
                <td style="border-bottom:1px solid black;">
                  <b>Butir Barang</b>
                </td>
                <td style="border-bottom:1px solid black;">
                  <b>
                    <!--Brand-->
                  </b>
                </td>
                <td style="border-bottom:1px solid black;">
                  <b>
                    <!--Model-->
                  </b>
                </td>
                <td style="border-bottom:1px solid black;">
                  <b>Harga Tunai</b>
                </td>
              </tr>
              <xsl:apply-templates select="LINEITEMS" />

              <tr>
                <td colspan="7" align="center" style="border-bottom:1px solid black;">&#xA0;</td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td>
            <table width="100%">
              <tr>
                <td rowspan="8">
                  <h5>BAHAGIAN III</h5>
                </td>
                <td>JUMLAH HARGA TUNAI BARANG-BARANG :</td>
                <td></td>
                <td align="right">
                  <b>
                    <xsl:value-of select="../FOOTER/GOODSVAL" />
                  </b>
                </td>
              </tr>
              <tr>
                <td>(Di mana terpakai) Tambah Amaun Yang Disatukan Dari Akaun No. :</td>
                <td></td>
                <td align="right">
                  <b></b>
                </td>
              </tr>
              <tr>
                <td>JUMLAH HARGA TUNAI :</td>
                <td></td>
                <td align="right">
                  <b>

                  </b>
                </td>
              </tr>
              <tr>
                <td>
                  Tambah Caj-Caj Pembayaran Tertunda Dikira @ <xsl:value-of select="../FOOTER/INTERESTRATE" /> sebulan (tolak diskaun baucer Home Club, jika terpakai)
                </td>
                <td></td>
                <td align="right">
                  <b>
                    <xsl:value-of select="../FOOTER/DT" />
                  </b>
                </td>
              </tr>
              <tr>
                <td>Jumlah Amaun Perlu Dibayar</td>
                <td>
                  <b></b>
                </td>
                <td align="right">
                  <b>
                    <xsl:value-of select="../FOOTER/TOTAL" />
                  </b>
                </td>
              </tr>
              <tr>
                <td>Tolak : Pembayaran Cengkeram Telah Dibayar</td>
                <td></td>
                <td align="right">
                  <b>
                    <xsl:value-of select="../FOOTER/DEPOSIT" />
                  </b>
                </td>
              </tr>
              <!--<tr>
                <td>Charge for Credit</td>
                <td>
                  <b></b>
                </td>
                <td align="right">
                  <b>
                    <xsl:value-of select="../FOOTER/DT" />
                  </b>
                </td>
              </tr>-->
              <tr>
                <td>Baki Perlu Dibayar</td>
                <td>
                  <b></b>
                </td>
                <td align="right">
                  <b>
                    <xsl:value-of select="../FOOTER/BALANCE" />
                  </b>
                </td>
              </tr>
              <tr>
                <td colspan="4" align="center" style="border-bottom:1px solid black;">&#xA0;</td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td>
            <b>BAHAGIAN IV -</b> Baki Perlu Dibayar mestilah dibayar secara <xsl:value-of select="../FOOTER/INSTALNO" /> ansuran-ansuran bulanan dalam jumlah <xsl:value-of select="../FOOTER/FIRSTINST" /> setiap satu dan ansuran terakhir dalam jumlah <xsl:value-of select="../FOOTER/FINALINST" />, dengan ansuran menjadi tertunggak dan perlu dibayar (i) pada atau sebelum [&#xA0;&#xA0;][&#xA0;&#xA0;]/[&#xA0;&#xA0;][&#xA0;&#xA0;]/[&#xA0;&#xA0;][&#xA0;&#xA0;][&#xA0;&#xA0;][&#xA0;&#xA0;] (tarikh/bulan/tahun)
            atau (ii) sekiranya (i) dikosongkan, satu bulan kalendar selepas penyerahan dan/atau pembekalan Barang-Barang atau mana-mana bahagian daripada
            mereka (dinilaikan pada Harga Tunai tambah Caj-Caj Pembayaran Tertunda) di mana bersama-sama dengan Amaun Yang Disatukan (sekiranya terpakai) berjumlah tidak
            kurang daripada 75% Jumlah Amaun Perlu Dibayar. Bagi mengelakkan keraguan, tarikh penyerahan dan/atau pembekalan Barang-Barang atau mana-mana bahagian
            daripada mereka (seperti mana yang terpakai) seperti yang direkodkan di dalam dokumen penyerahan Courts adalah bukti konklusif yang terikat terhadap saya, tidak kira
            samada saya terima penyerahan ataupun tidak. Ungkapan "Tarikh Pembayaran Yang Dipersetujui" adalah hari apabila ansuran pertama menjadi tertunggak dan perlu dibayar.
          </td>
        </tr>
        <tr>
          <td>
            <b>BAHAGIAN V -</b> Saya/Kami telah membaca and memahami Terma &amp; Syarat Biasa Yang Menguasai Perjanjian Catel (yang dilampirkan di sini), dan
            Terma Dan Syarat Biasa Yang Menguasai Max Perak/Emas Courts (Sekiranya terpakai,dilampirkan di sini) dan <b>Jadual Pertama</b> ini.
            NOTA PENTING: Saya/Kami memberi representasi dan membuat pengakuan bahawa Barang-Barang adalah dibeli hanya untuk kegunaan peribadi saya/kami dan
            saya/kami tidak akan walau sebagaimana pun menggunakan barang-Barang sebagai sekurti dalam apa jua cara dan dalam kemungkirannya, saya/kami bersetuju
            bahawa Courts adalah inter alia, berhak untuk menuntut pembayaran serta merta Baki Perlu Dibayar serta segala Wang lain yang perlu dibayar kepada Courts dan
            untuk mengambil tindakan undang-undang terhadap saya/kami.

          </td>
        </tr>
        <tr>
          <td>
            <table cellspacing="4" width="100%">
              <tr>
                <td>Bertarikh pada:</td>
                <td>Tandatangan Pelanggan/Pembeli Bersama</td>
                <td>Dengan kehadiran:</td>
              </tr>
              <tr>
                <p></p>
              </tr>
              <tr>
                <p></p>
              </tr>
              <tr>
                <p></p>
              </tr>
              <tr>
                <td style="border-bottom:1px solid black;width:90px;">
                  <xsl:value-of select="HEADER/DATE" />&#xA0;
                </td>
                <td style="border-bottom:1px solid black">&#xA0;</td>
                <td style="border-bottom:1px solid black">&#xA0;</td>
              </tr>
              <tr>
                <td></td>
                <td></td>
                <td>Tandatangan dan nama Kakitangan Courts (M) Sdn Bhd (154820-D)</td>
              </tr>
            </table>
          </td>
        </tr>
      </table>


      <!-- end new page template -->


      <xsl:variable name="lastPage" select="LAST" />
      <xsl:variable name="lastAgreement" select="../LAST" />
      <xsl:if test="$lastPage = 'False' or $lastAgreement = 'FALSE' ">
        <br class="pageBreak" />
        <!--if it's not the last page-->
      </xsl:if>
    </div>
  </xsl:template>


  <xsl:template match="LINEITEMS">
    <xsl:apply-templates select="LINEITEM" />
  </xsl:template>

  <xsl:template match="LINEITEM">

    <xsl:variable name="addTo" select="ADDTO" />

    <xsl:if test="$addTo = 'True'">
      <tr>
        <td>
          <xsl:value-of select="QUANTITY"/>
        </td>
        <td>
          <xsl:value-of select="ITEMNO"/>
        </td>
        <td></td>
        <td>
          <xsl:value-of select="DESC" />
          (<xsl:value-of select="ACCTNO" />)
        </td>
        <td></td>
        <td></td>
        <td>
          <xsl:value-of select="VALUE"/>
        </td>
      </tr>
    </xsl:if>


    <xsl:if test="$addTo != 'True'">
      <tr>
        <td>
          <xsl:value-of select="QUANTITY"/>
        </td>
        <td>
          <xsl:value-of select="ITEMNO"/>
        </td>
        <td></td>
        <td>
          <xsl:value-of select="DESC"/>
          <xsl:if test="(TRIM != '' and TRIM != ' ') and (DESC2 != '' and DESC2 != ' ')">
            , <xsl:value-of select="DESC2"/>
          </xsl:if>
        </td>
        <td></td>
        <td></td>
        <td>
          <xsl:value-of select="VALUE"/>
        </td>
      </tr>
      <xsl:if test="TRIM != '' and TRIM != ' ' ">
        <tr>
          <td colspan="3"></td>
          <td>
            <!--xsl:value-of select="TRIM" /-->
          </td>
          <td colspan="3">
          </td>
        </tr>
      </xsl:if>

      <xsl:if test="(DESC2 != '' and DESC2 != ' ') and (TRIM = '' or TRIM = ' ')">
        <tr>
          <td colspan="3"></td>
          <td>
            <xsl:value-of select="DESC2" />
          </td>
          <td colspan="3">
          </td>
        </tr>
      </xsl:if>

    </xsl:if>
    <tr>
      <td>
      </td>
    </tr>
  </xsl:template>



</xsl:stylesheet>