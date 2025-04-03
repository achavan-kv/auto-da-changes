<?xml version="1.0" encoding="UTF-8" ?>
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
				<TABLE id="Table1" cellSpacing="1" cellPadding="1" width="600" align="center" border="0">
					<TR>
						<TD class="RFHead1" align="middle">DEBT COLLECTORS ACTION SHEET</TD>
					</TR>
				</TABLE>
				<xsl:apply-templates select="ACTIONSHEET" />
			</BODY>
		</HTML>
	</xsl:template>
				
	<xsl:template match="ACTIONSHEET">
		<xsl:apply-templates select="HEADER" />		
		<P></P>
		<xsl:apply-templates select="DETAILS" />		
		<P></P>
		<xsl:apply-templates select="LINEITEMS" />		
		<P></P>
		<xsl:apply-templates select="EMPLOYER" />		
		<P></P>
	</xsl:template>

	<xsl:template match="HEADER">
		<P align="left">
			<TABLE class="RFBody" id="Table2" style="WIDTH: 701px; HEIGHT: 27px" cellSpacing="1" cellPadding="1" width="701" align="center" border="0">
				<TR>
					<TD class="RFBodyBold" width="10%">Account No</TD>
					<TD align="left" width="20%">
						<xsl:value-of select="ACCTNO" />
					</TD>
					
					<TD class="RFBodyBold" width="6%">Name</TD>
					<TD align="left" width="64%">
						<xsl:value-of select="TITLE" /> <xsl:value-of select="FIRSTNAME" /> <xsl:value-of select="LASTNAME" />
					</TD>
				</TR>
			</TABLE>
		</P>
		<P>
			<TABLE id="Table3" style="WIDTH: 704px; HEIGHT: 94px" cellSpacing="1" cellPadding="1" width="704" align="center" border="0">
				<TR>
					<TD class="RFBodyBold" width="33%">Home Address</TD>
					<TD class="RFBodyBold" width="33%">Postal Address</TD>
					<TD class="RFBodyBold" width="33%">Telephone No(s)</TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="ADDR1" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="POSTALADDR1" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="HOMETEL" />
					</TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="ADDR2" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="POSTALADDR2" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="POSTTEL" />
					</TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="ADDR3" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="POSTALADDR3" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="WORKTEL" />
					</TD>
				</TR>
				<TR>
					<TD width="33%">
						<xsl:value-of select="POSTCODE" />
					</TD>
					<TD width="33%">
						<xsl:value-of select="POSTALCODE" />
					</TD>
					<TD width="33%"></TD>
				</TR>
			</TABLE>
		</P>
		
		<P></P>
		<P>
			<TABLE id="Table5" style="WIDTH: 705px; HEIGHT: 47px" cellSpacing="1" cellPadding="1" width="705" align="center" border="0">
				<TR>
					<TD class="RFBodyBold">Location Details</TD>
				</TR>
				<TR>
					<TD>
						<xsl:value-of select="NOTES" />
					</TD>

					<!--<TD width="100%">
						<xsl:value-of select="NOTES" />
					</TD>-->
				</TR>
			</TABLE>
		</P>
	</xsl:template>
	
	<xsl:template match="DETAILS">
		<P>
			<TABLE id="Table4" style="WIDTH: 705px; HEIGHT: 219px" cellSpacing="1" cellPadding="1" width="705" align="center" border="0">
				<TR>
					<TD class="RFBodyBold" width="30%">Name Of Debt Collector</TD>
					<TD width="70%">
						<xsl:value-of select="COLLECTOR" />
					</TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="30%">Print Date</TD>
					<TD width="70%">
						<xsl:value-of select="DATE" />
					</TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="30%">Balance Outstanding</TD>
					<TD width="70%">
						<xsl:value-of select="BALANCE" />
					</TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="30%">Arrears</TD>
					<TD width="70%">
						<xsl:value-of select="ARREARS" />
					</TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="30%">Collection Costs</TD>
					<TD width="70%">
						<xsl:value-of select="COSTS" />
					</TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="30%">Total To Collect</TD>
					<TD width="70%">
						<xsl:value-of select="TOTAL" />
					</TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="30%">Monthly Instalment</TD>
					<TD width="70%">
						<xsl:value-of select="INSTALMENT" />
					</TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="30%">Due Date</TD>
					<TD width="70%">
						<xsl:value-of select="DUEDATE" />
					</TD>
				</TR>
				<TR>
					<TD class="RFBodyBold" width="30%">Return Date</TD>
					<TD width="70%">
						<xsl:value-of select="RETURNDATE" />
					</TD>
				</TR>
        <TR>
          <TD class="RFBodyBold" width="30%">Deadline Date</TD>
          <TD width="70%">
            <xsl:value-of select="DEADLINEDATE" />
          </TD>
        </TR>
			</TABLE>
		</P>
	</xsl:template>
	
	<xsl:template match="LINEITEMS">
		<P>
			<TABLE style="WIDTH: 708px; HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="708" align="center" border="0">
				<xsl:apply-templates select="LINEITEM" />
			</TABLE>
		</P>	
		<P>
			<TABLE style="WIDTH: 708px; HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="708" align="center" border="0">
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
		<br class="pageBreak" />
		<TABLE cellSpacing="1" cellPadding="1" width="600" align="center" border="0">
			<TR>
				<TD class="RFHead1" align="middle">CUSTOMER ADDITIONAL DETAILS</TD>
			</TR>
		</TABLE>
		<xsl:apply-templates select="../HEADER" />
		<P></P>
	</xsl:template>

	<xsl:template match="LINEITEM">
		<TR>
			<TD width = "5%">
				<xsl:value-of select="QUANTITY" />
			</TD>
			<TD width = "75%">
				<xsl:value-of select="DESC1" />
			</TD>	
			<TD width = "20%">
				<xsl:value-of select="PRICE" />
			</TD>	
		</TR>
	</xsl:template>

	<xsl:template match="EMPLOYER">
		<TABLE style="WIDTH: 704px; HEIGHT: 94px" cellSpacing="1" cellPadding="1" width="704" align="center" border="0">
			<TR>
				<TD class="RFBodyBold" >Employers Address</TD>
			</TR>
			<TR>
				<TD>
					<xsl:value-of select="NAME" />
				</TD>
			</TR>		
			<TR>
				<TD>
					<xsl:value-of select="ADDR1" />
				</TD>
			</TR>		
			<TR>
				<TD>
					<xsl:value-of select="ADDR2" />
				</TD>
			</TR>		
			<TR>
				<TD>
					<xsl:value-of select="ADDR3" />
				</TD>
			</TR>		
			<TR>
				<TD>
					<xsl:value-of select="POSTCODE" />
				</TD>
			</TR>		
		</TABLE>
	</xsl:template>
</xsl:stylesheet>
