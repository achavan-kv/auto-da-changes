<?xml version="1.0" encoding="UTF-8" ?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;" >
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<HEAD>
				<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"></meta>
				<style type="text/css" media="all">
          @import url(<xsl:value-of select="ACTIONSHEETS/@CSSPATH"/>	);
        </style>
			</HEAD>
			<BODY>	
				<xsl:apply-templates select="ACTIONSHEETS" />
			</BODY>
		</HTML>
	</xsl:template>
	
	<xsl:template match="ACTIONSHEETS">
		<xsl:apply-templates select="ACTIONSHEET" />		
	</xsl:template>

				
	<xsl:template match="ACTIONSHEET">
		<xsl:apply-templates select="HEADER" />		
		<P></P>
		<xsl:apply-templates select="DETAILS" />		
		<P></P>
		<xsl:apply-templates select="LINEITEMS" />		
		<P></P>
		<xsl:apply-templates select="CUSTOMERS" />		
		<P></P>
		<xsl:apply-templates select="SERVICEREQUESTS" />
		<P></P>
		<xsl:variable name="last" select="LAST" />
		<xsl:if test="$last != 'TRUE'">
			<br class="pageBreak" />
		</xsl:if>
	</xsl:template>

	<xsl:template match="HEADER">
		<P>
			<TABLE id="Table1" cellSpacing="1" cellPadding="1" width="600" align="center" border="0">
				<TR>
					<TD class="RFHead1" align="middle">DEBT COLLECTORS ACTION SHEET 1</TD>
				</TR>
        <TR>
          <TD class="RFHead1" align="middle">
            <xsl:value-of select="STORETYPE" /> Store</TD>
				</TR>
			</TABLE>
		</P>
		<P align="left">
			<TABLE class="RFBody" id="Table2"  cellSpacing="1" cellPadding="1" width="100%" align="center" border="0">
				<TR>
					<TD width="35%" ><STRONG>Account No </STRONG><xsl:value-of select="ACCTNO" /></TD>
					<TD width="35%" ><STRONG>Name </STRONG><xsl:value-of select="TITLE" /> <xsl:value-of select="FIRSTNAME" /> <xsl:value-of select="LASTNAME" /></TD>
          <TD width ="15%" align="center" >
            <xsl:variable name="photo" select="PHOTO" />
            <xsl:if test="$photo != ''">
              <IMG height ="100" width="120"  src="{$photo}"></IMG>
            </xsl:if>          
          </TD>
            <TD width ="15%" valign="middle" >
              <xsl:variable name="signature" select="SIGNATURE" />
              <xsl:if test="$signature != ''">
                <STRONG>Signature </STRONG>
                <IMG height ="30" width="100"  src="{$signature}"></IMG>
              </xsl:if>
          </TD>
        </TR>
				<TR>
					<TD class="RFBodyBold" width="33%" colspan="2"><xsl:value-of select="ALIAS" /></TD>
					<TD width="67%"></TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="33%" colspan="2"><xsl:value-of select="RFCUSTOMER" /></TD>
					<TD width="67%"></TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="33%" colspan="2"><xsl:value-of select="PRIVILEGECUSTOMER" /></TD>
					<TD width="67%"></TD>
				</TR>
			</TABLE>
		</P>
		<P>
			<TABLE class ="ThaiLineItem" id="Table3" style="WIDTH: 704px; HEIGHT: 94px" cellSpacing="1" cellPadding="1" width="704" align="center" border="0">
				<TR>
					<TD class="RFBodyBold" width="33%">Home Address</TD>
					<TD class="RFBodyBold" width="33%">Delivery Address</TD>
					<TD class="RFBodyBold" width="33%">Employers Address</TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="ADDR1" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="DELIVERYADDR1" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="NAME" />
					</TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="ADDR2" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="DELIVERYADDR2" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="WORKADDR1" />
					</TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="ADDR3" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="DELIVERYADDR3" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="WORKADDR2" />
					</TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="POSTCODE" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="DELIVERYPCODE" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="WORKADDR3" />
					</TD>
				</TR>
				<TR>
					<TD width="33%"></TD>
					<TD width="33%"></TD>
					<TD width="33%">
						<xsl:value-of select="WORKPCODE" />
					</TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="HOMETEL" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="DELIVERYTEL" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="WORKTEL" />
					</TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="MOBILE" />
					</TD>
					<TD width="33%"></TD>
					<TD width="33%"></TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="MOBILE2" />
					</TD>
					<TD width="33%"></TD>
					<TD width="33%"></TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="MOBILE3" />
					</TD>
					<TD width="33%"></TD>
					<TD width="33%"></TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="MOBILE4" />
					</TD>
					<TD width="33%"></TD>
					<TD width="33%"></TD>
				</TR>
			</TABLE>
		</P>
		
		<P></P>
		<P>
			<TABLE class ="ThaiLineItem" id="Table5"  cellSpacing="1" cellPadding="1" width="100%" align="center" border="0">
				<TR>
					<TD class="RFBodyBold">Location Details</TD>
				</TR>
				<TR>
					<TD>
						<xsl:value-of select="NOTES" />
					</TD>
				</TR>
			</TABLE>
		</P>
		<P>
			<TABLE class ="ThaiLineItem"  cellSpacing="1" cellPadding="1" width="100%" align="center" border="0">
				<xsl:if test="INSTRUCTIONS != 'NONE'">						
					<TR>
						<TD class="RFBodyBold">Instructions</TD>
					</TR>
					<TR>
						<TD><xsl:value-of select="INSTRUCTIONS" /></TD>
					</TR>
				</xsl:if>
			</TABLE>
		</P>
	</xsl:template>
	
	<xsl:template match="DETAILS">
		<P>
			<TABLE class ="ThaiLineItem" id="Table4" style="WIDTH: 705px; HEIGHT: 219px" cellSpacing="1" cellPadding="1" width="705" align="center" border="0">
				<TR>
					<TD class="RFBodyBold" width="25%">Name Of Debt Collector</TD>
					<TD width="25%">
						<xsl:value-of select="COLLECTOR" />
					</TD>
					<TD class="RFBodyBold" width="25%">Date Of Delivery</TD>
					<TD width="25%">
						<xsl:value-of select="DELIVERYDATE" />
					</TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="25%">Print Date</TD>
					<TD width="25%">
						<xsl:value-of select="DATE" />
					</TD>
					<TD class="RFBodyBold" width="25%">Date Of Last Payment</TD>
					<TD width="25%">
						<xsl:value-of select="DATELASTPAID" />
					</TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="25%">Arrears</TD>
					<TD width="25%">
						<xsl:value-of select="ARREARS" />
					</TD>
					<TD class="RFBodyBold" width="25%">Arrears Level</TD>
					<TD width="25%">
						<xsl:value-of select="ARREARSLEVEL" />
					</TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="25%">Balance Outstanding</TD>
					<TD width="25%">
						<xsl:value-of select="BALANCE" />
					</TD>
					<xsl:variable name="print" select="PRINTCHARGES" />
					<xsl:if test="$print != 'True'">
						<TD width="25%"></TD>
						<TD width="25%"></TD>
					</xsl:if>
					<xsl:if test="$print = 'True'">
						<TD class="RFBodyBold" width="25%">Interest\Admin Charges</TD>
						<TD width="25%">
							<xsl:value-of select="INTEREST" />
						</TD>
					</xsl:if>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="25%">Collection Costs</TD>
					<TD width="25%">
						<xsl:value-of select="COSTS" />
					</TD>
					<TD width="25%"></TD>
					<TD width="25%"></TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="25%">Total To Collect</TD>
					<TD width="25%">
						<xsl:value-of select="TOTAL" />
					</TD>
					<TD width="25%"></TD>
					<TD width="25%"></TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="25%">Monthly Instalment</TD>
					<TD width="25%">
						<xsl:value-of select="INSTALMENT" />
					</TD>
					<TD width="25%"></TD>
					<TD width="25%"></TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="25%">Due Date</TD>
					<TD width="25%">
						<xsl:value-of select="DUEDATE" />
					</TD>
					<TD width="25%"></TD>
					<TD width="25%"></TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="25%">Return Date</TD>
					<TD width="25%">
						<xsl:value-of select="RETURNDATE" />
					</TD>
					<TD width="25%"></TD>
					<TD width="25%"></TD>
				</TR>
        <TR>
          <TD class="RFBodyBold" width="25%">Deadline Date</TD>
          <TD width="25%">
            <xsl:value-of select="DEADLINEDATE" />
          </TD>
          <TD width="25%"></TD>
          <TD width="25%"></TD>
        </TR>
			</TABLE>
		</P>
	</xsl:template>
	
	<xsl:template match="LINEITEMS">
		<P>
			<TABLE style="WIDTH: 708px; HEIGHT: 30px" cellSpacing="1" cellPadding="1" width="708" align="center" border="0">
				<xsl:apply-templates select="LINEITEM" />
			</TABLE>
		</P>	
		<P>
			<TABLE class ="ThaiLineItem" style="WIDTH: 708px; HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="708" align="center" border="0">
				<TR>
					<TD class="RFBodyBold" width="80%">Total Value</TD>
					<TD width="20%">
						<xsl:value-of select="TOTALVALUE" />
					</TD>
				</TR>
			</TABLE>
		</P>
		<P></P>
		<P></P>
		<P></P>
		<P>Collector Remarks:</P>

		<xsl:if test="../CUSTOMERS/CUSTOMER/NAME!='' or ../SERVICEREQUESTS/SERVICEREQUEST/SERVICEREQUESTNO!=''">
			
				<br class="pageBreak" />
				
				<TABLE cellSpacing="1" cellPadding="1" width="600" align="center" border="0">
					<TR>
						<TD class="RFHead1" align="middle">CUSTOMER ADDITIONAL DETAILS</TD>
					</TR>
				</TABLE>
				<xsl:apply-templates select="../HEADER" />
				<P></P>
			
		</xsl:if>
	</xsl:template>

	<xsl:template match="LINEITEM">
		<TR>
			<TD class="ThaiLineItem" width = "5%">
				<xsl:value-of select="QUANTITY" />
			</TD>
			<TD class="ThaiLineItem" width = "75%">
				<xsl:value-of select="DESC1" />
			</TD>
			<TD class="ThaiLineItem" width = "20%">
				<xsl:value-of select="PRICE" />
			</TD>	
		</TR>
		<TR>
		    <TD class="ThaiLineItem" width = "5%">
		    </TD>
			<TD class="ThaiLineItem" width = "75%">
				<xsl:value-of select="DESC2" />
			</TD>	
		</TR>
	</xsl:template>

	<xsl:template match="CUSTOMERS">
		<xsl:if test="CUSTOMER/NAME!=''">
			<TABLE style="WIDTH: 704px; HEIGHT: 94px" cellSpacing="1" cellPadding="1" width="704" align="center" border="0">
				<xsl:apply-templates select="CUSTOMER" />
			</TABLE>
		</xsl:if>
	</xsl:template>

	<xsl:template match="CUSTOMER">
		<TR><TD class="ThaiLineItem"><xsl:value-of select="NAME" /></TD></TR>
		<TR><TD class="ThaiLineItem"><xsl:value-of select="ADDR1" /></TD></TR>
		<TR><TD class="ThaiLineItem"><xsl:value-of select="ADDR2" /></TD></TR>
		<TR><TD class="ThaiLineItem"><xsl:value-of select="ADDR3" /></TD></TR>
		<TR><TD class="ThaiLineItem"><xsl:value-of select="POSTCODE" /></TD></TR>
		<TR><TD class="ThaiLineItem"><xsl:value-of select="TELNO" /></TD></TR>
		<TR><TD class="ThaiLineItem"><xsl:value-of select="WORKNO" /></TD></TR>
		<TR><TD class="ThaiLineItem"><xsl:value-of select="MOBILENO" /></TD></TR>
		<TR></TR>
		<TR><TD class="ThaiLineItem"><xsl:value-of select="COMMENT" /></TD></TR>
		<TR></TR>
		<TR><TD class="ThaiLineItem"><xsl:value-of select="DIRECTIONS" /></TD></TR>
		<TR></TR>
		<TR></TR>
		<TR></TR>
	</xsl:template>

	<xsl:template match="SERVICEREQUESTS">
		<xsl:if test="SERVICEREQUEST/SERVICEREQUESTNO!=''">
			<SPAN class="RFBodyBold">Service Requests</SPAN>
			<TABLE style="WIDTH: 704px; HEIGHT: 94px" cellSpacing="1" cellPadding="1" width="704" align="center" border="0">
				<TR>
					<TD class="RFBodyBold">
						SR No
					</TD>

					<TD class="RFBodyBold">
						Date Logged
					</TD>

					<TD class="RFBodyBold">
						Prod Code
					</TD>
					<TD class="RFBodyBold">
						Date Closed
					</TD>

					<TD class="RFBodyBold">
						Rpmt Stat
					</TD>

					<TD class="RFBodyBold">
						Dlvr Name
					</TD>
				</TR>
				<xsl:apply-templates select="SERVICEREQUEST" />
			</TABLE>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="SERVICEREQUEST">
		<TR>
			<TD class="ThaiLineItem" colspan="6">
				<span class="RFBodyBold">&nbsp;&nbsp;&nbsp;Product Description:&nbsp;&nbsp;</span>
				<span class="ThaiLineItem"><xsl:value-of select="PRODUCTDESCRIPTION" /></span>
			</TD>
		</TR>
		<TR>
			<TD class="ThaiLineItem">
				<xsl:value-of select="SERVICEREQUESTNO" />
			</TD>
			<TD class="ThaiLineItem">
				<xsl:value-of select="DATELOGGED" />
			</TD>
			<TD class="ThaiLineItem">
				<xsl:value-of select="PRODUCTCODE" />
			</TD>
			<TD class="ThaiLineItem">
				<xsl:value-of select="DATECLOSED" />
			</TD>
			<TD class="ThaiLineItem">
				<xsl:value-of select="REPLACEMENTSTATUS" />
			</TD>
			<TD class="ThaiLineItem">
				<xsl:value-of select="DELIVERERNAME" />
			</TD>
		</TR>
	</xsl:template>


</xsl:stylesheet>
