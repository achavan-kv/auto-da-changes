<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<HEAD>
				<TITLE></TITLE>
				<META content="Microsoft Visual Studio 7.0" name="GENERATOR"></META>
				<META content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"></META>
				<style type="text/css" media="all"> @import url(styles.css); 
				</style>
			</HEAD>
			<BODY>
				<TABLE id="Table1" cellSpacing="0" cellPadding="5" width="600" border="1">
					<TR>
						<TD style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none"><IMG alt="" src="smallLogo.jpg" /></TD>
					</TR>
					<TR>
						<TD class="RFHead1" style="BORDER-TOP: gray 1pt solid; BORDER-BOTTOM: gray 1pt solid; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none">UNICOMER 
							READY FINANCE CUSTOMER DETAILS</TD>
					</TR>
					<TR>
						<TD class="RFHead1" style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none">
							<TABLE class="RFHead2" id="Table2" style="HEIGHT: 81px" cellSpacing="1" cellPadding="1" width="100%" border="0">
								<TR>
									<TD><xsl:value-of select="RFSUMMARY/CUSTDETAILS/TITLE" />
										<xsl:value-of select="RFSUMMARY/CUSTDETAILS/FIRSTNAME" />
										<xsl:value-of select="RFSUMMARY/CUSTDETAILS/LASTNAME" />
									</TD>
								</TR>
								<TR>
									<TD><xsl:value-of select="RFSUMMARY/CUSTDETAILS/CURRENTADDRESS/ADDRESS1" /></TD>
								</TR>
								<TR>
									<TD><xsl:value-of select="RFSUMMARY/CUSTDETAILS/CURRENTADDRESS/ADDRESS2" /></TD>
								</TR>
								<TR>
									<TD><xsl:value-of select="RFSUMMARY/CUSTDETAILS/CURRENTADDRESS/ADDRESS3" /></TD>
								</TR>
								<TR>
									<TD><xsl:value-of select="RFSUMMARY/CUSTDETAILS/CURRENTADDRESS/POSTCODE" /></TD>
								</TR>
								<TR>
									<TD> </TD>
								</TR>
							</TABLE>
						</TD>
					</TR>
					<TR>
						<TD class="RFHead2" style="BORDER-TOP: gray 1pt solid; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none">PERSONAL 
							CIRCUMSTANCES</TD>
					</TR>
				</TABLE>
				<P>
					<TABLE class="RFBody" id="Table3" cellSpacing="0" cellPadding="5" width="600" border="1">
						<TR>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-LEFT: gray 1pt; BORDER-TOP-STYLE: none; BORDER-BOTTOM: gray 1pt solid" vAlign="top" width="20%">
								<TABLE id="Table4" style="HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="100%" border="0">
									<TR>
										<TD class="RFBody">Date of Birth</TD>
									</TR>
									<TR>
										<TD class="RFBody">Home Tel</TD>
									</TR>
									<TR>
										<TD class="RFBody">Work Tel</TD>
									</TR>
									<TR>
										<TD class="RFBody">Mobile Tel</TD>
									</TR>
									<TR>
										<TD class="RFBody">Id Number</TD>
									</TR>
								</TABLE>
							</TD>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-BOTTOM: gray 1pt solid; BORDER-LEFT-STYLE: none" vAlign="top" width="30%">
								<TABLE id="Table5" style="HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="100%" border="0">
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/DOB" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/HOMETEL" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/WORKTEL" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/MOBILETEL" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/CUSTID" /></TD>
									</TR>
								</TABLE>
							</TD>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-BOTTOM: gray 1pt solid; BORDER-LEFT-STYLE: none" vAlign="top" width="20%">
								<TABLE id="Table6" style="HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="100%" align="center" border="0">
									<TR>
										<TD class="RFBody">Property Type</TD>
									</TR>
									<TR>
										<TD class="RFBody">Time at current Address</TD>
									</TR>
									<TR>
										<TD class="RFBody">Previous Address</TD>
									</TR>
									<TR>
										<TD class="RFBody"> </TD>
									</TR>
									<TR>
										<TD class="RFBody"> </TD>
									</TR>
									<TR>
										<TD class="RFBody"> </TD>
									</TR>
								</TABLE>
							</TD>
							<TD style="BORDER-TOP-STYLE: none; BORDER-BOTTOM: gray 1pt solid; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" vAlign="top" width="30%">
								<TABLE id="Table7" style="HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="100%" border="0">
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/PROPTYPE" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/TIMEINCURRADDRESS" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/PREVADDRESS/ADDRESS1" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/PREVADDRESS/ADDRESS2" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/PREVADDRESS/ADDRESS3" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/PREVADDRESS/POSTCODE" /></TD>
									</TR>
								</TABLE>
							</TD>
						</TR>
						<TR>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-BOTTOM: gray 1pt solid; BORDER-LEFT-STYLE: none" vAlign="top" width="20%">
								<TABLE id="Table8" style="HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="100%" border="0">
									<TR>
										<TD class="RFBody">Occupation</TD>
									</TR>
									<TR>
										<TD class="RFBody">Employer Address</TD>
									</TR>
									<TR>
										<TD class="RFBody">Employment Status</TD>
									</TR>
									<TR>
										<TD class="RFBody">Tel</TD>
									</TR>
									<TR>
										<TD class="RFBody">Time in current Employment</TD>
									</TR>
								</TABLE>
							</TD>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-BOTTOM: gray 1pt solid; BORDER-LEFT-STYLE: none" vAlign="top" width="30%">
								<TABLE id="Table9" style="HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="100%" border="0">
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/OCCUPATION" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/EMPLOYER" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/EMPLOYMENTSTAT" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/EMPLOYERTEL" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/TIMECURREMPLOYMENT" /></TD>
									</TR>
								</TABLE>
							</TD>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-BOTTOM: gray 1pt solid; BORDER-LEFT-STYLE: none" vAlign="top" width="20%">
								<TABLE id="Table10" style="HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="100%" border="0">
									<TR>
										<TD class="RFBody">Marital status</TD>
									</TR>
									<TR>
										<TD class="RFBody">Spouse's Employment status</TD>
									</TR>
									<TR>
										<TD class="RFBody">Spouse's Occupation</TD>
									</TR>
									<TR>
										<TD class="RFBody">No. of dependants</TD>
									</TR>
									<TR>
										<TD class="RFBody"></TD>
									</TR>
								</TABLE>
							</TD>
							<TD style="BORDER-TOP-STYLE: none; BORDER-BOTTOM: gray 1pt solid; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" vAlign="top" width="30%">
								<TABLE id="Table11" style="HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="100%" border="0">
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/MARITALSTAT" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/SPOUSE/EMPLOYMENTSTAT" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/SPOUSE/OCCUPATION" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/DEPENDENTS" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"></TD>
									</TR>
								</TABLE>
							</TD>
						</TR>
						<TR>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" width="20%">
								<TABLE id="Table12" style="HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="100%" border="0">
									<TR>
										<TD class="RFBody">Bank Name</TD>
									</TR>
									<TR>
										<TD class="RFBody">Time with bank</TD>
									</TR>
									<TR>
										<TD class="RFBody">Account number</TD>
									</TR>
									<TR>
										<TD class="RFBody"> </TD>
									</TR>
									<TR>
										<TD class="RFBody"> </TD>
									</TR>
								</TABLE>
							</TD>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" width="30%">
								<TABLE id="Table13" style="HEIGHT: 75px" cellSpacing="1" cellPadding="1" width="100%" border="0">
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/BANKNAME" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/TIMEATBANK" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/BANKACCTNO" /></TD>
									</TR>
									<TR>
										<TD class="RFBody"></TD>
									</TR>
									<TR>
										<TD class="RFBody"></TD>
									</TR>
								</TABLE>
							</TD>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" width="20%">
								<TABLE id="Table14" style="HEIGHT: 19px" cellSpacing="1" cellPadding="1" width="100%" border="0">
									<TR>
										<TD class="RFBody">Monthly Income</TD>
									</TR>
								</TABLE>
							</TD>
							<TD style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" width="30%">
								<TABLE id="Table15" style="HEIGHT: 19px" cellSpacing="1" cellPadding="1" width="100%" border="0">
									<TR>
										<TD class="RFBody"><xsl:value-of select="RFSUMMARY/CUSTDETAILS/MONTHLYINCOME" /></TD>
									</TR>
								</TABLE>
							</TD>
						</TR>
					</TABLE>
				</P>
				<P>
					<TABLE id="Table16" cellSpacing="0" cellPadding="5" width="600" border="1">
						<TR>
							<TD class="RFBody" style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none">I 
								hereby confirm that the details about me shown above are correct at this time. 
								I understand that a Ready Finance card and spending limit has been given based 
								on my declared circumstances and that a failure to declare information 
								correctly or the declaration of false information may result in the card being 
								withdrawn by UNICOMER. <br/><br/>
								This schedule is supplemental to any Agreement dated hereafter within 12 months of this statement
								(the 'Main Agreement') and made between the Customer and the Owner (as such terms are
								defined in the Main Agreement) and the Customer hereby acknowledges that the terms of
								the Main Agreement shall be deemed to be and are hereby made a part hereof and shall
								apply to all transactions entered into between the Customer and the Owner persuant to the
								terms hereof.</TD>
						</TR>
						<TR>
							<TD class="RFBody" style="BORDER-TOP: gray 1pt solid; BORDER-BOTTOM: gray 1pt solid; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none">Print 
								Name:	<xsl:value-of select="RFSUMMARY/CUSTDETAILS/FIRSTNAME" />
										<xsl:value-of select="RFSUMMARY/CUSTDETAILS/LASTNAME" />
							</TD>
						</TR>
						<TR>
							<TD class="RFBody" style="BORDER-TOP-STYLE: none; BORDER-BOTTOM: gray 1pt solid; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none">Signature:
							</TD>
						</TR>
						<TR>
							<TD class="RFBody" style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none">Date: <xsl:value-of select="RFSUMMARY/CURRENTDATE" />
							</TD>
						</TR>
					</TABLE>
				</P>
				<br class="pageBreak" />			
				<p>
					<TABLE id="Table17" cellSpacing="0" cellPadding="5" width="600" border="1">
						<TR>
							<TD style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none"><IMG alt="" src="smallLogo.jpg" /></TD>
						</TR>
						<TR>
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt solid; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none">READY 
								FINANCE ACCOUNT SUMMARY</TD>
						</TR>
						<TR>
							<TABLE class="RFHead2" style="BORDER-LEFT: gray 1pt solid; BORDER-RIGHT: gray 1pt solid; BORDER-BOTTOM: gray 1pt solid; HEIGHT: 81px" cellSpacing="1" cellPadding="1" width="600" border="0">
								<TR>
									<TD><xsl:value-of select="RFSUMMARY/CUSTDETAILS/TITLE" />
										<xsl:value-of select="RFSUMMARY/CUSTDETAILS/FIRSTNAME" />
										<xsl:value-of select="RFSUMMARY/CUSTDETAILS/LASTNAME" />
									</TD>
								</TR>
								<TR>
									<TD><xsl:value-of select="RFSUMMARY/CUSTDETAILS/CURRENTADDRESS/ADDRESS1" /></TD>
								</TR>
								<TR>
									<TD><xsl:value-of select="RFSUMMARY/CUSTDETAILS/CURRENTADDRESS/ADDRESS2" /></TD>
								</TR>
								<TR>
									<TD><xsl:value-of select="RFSUMMARY/CUSTDETAILS/CURRENTADDRESS/ADDRESS3" /></TD>
								</TR>
								<TR>
									<TD><xsl:value-of select="RFSUMMARY/CUSTDETAILS/CURRENTADDRESS/POSTCODE" /></TD>
								</TR>
								<TR>
									<TD> </TD>
								</TR>
							</TABLE>
						</TR>
					</TABLE>
				</p>
				<p>					
					<TABLE class="RFBody" id="Table18" style="BORDER-BOTTOM-STYLE: none" cellSpacing="0" cellPadding="5" width="600" border="1">
						<TR>
							<TD class="RFHead2" style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: gray 1pt" width="50%">RF 
								Account Number</TD>
							<TD class="RFHead2" style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: gray 1pt" width="50%">
								<xsl:value-of select="RFSUMMARY/ACCTDETAILS/RFACCTNO" />
							</TD>
						</TR>
						<TR height="15" >
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt solid; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" width="50%"></TD>
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt solid; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" width="50%"></TD>
						</TR>
						<TR>
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" align="right" width="50%">SPENDING 
								LIMIT:</TD>
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" width="50%">
								<xsl:value-of select="RFSUMMARY/ACCTDETAILS/SPENDINGLIMIT" />
							</TD>
						</TR>
						<TR>
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" align="right" width="50%">AVAILABLE 
								SPEND:</TD>
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" width="50%">
								<xsl:value-of select="RFSUMMARY/ACCTDETAILS/AVAILABLESPEND" />
							</TD>
						</TR>
						<TR>
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" align="right" width="50%">LIMIT 
								VALID FROM:</TD>
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" width="50%">
								<xsl:value-of select="RFSUMMARY/ACCTDETAILS/VALIDFROM" />
							</TD>
						</TR>
						<TR>
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" align="right" width="50%">LIMIT 
								VALID UNTIL:</TD>
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" width="50%">
								<xsl:value-of select="RFSUMMARY/ACCTDETAILS/VALIDTO" />
							</TD>
						</TR>
						<TR height="15" >
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" width="50%"></TD>
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" width="50%"></TD>
						</TR>
						<TR>
							<TD class="RFHead2" style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" width="50%">ACCOUNT 
								ACTIVITY</TD>
							<TD class="RFHead1" style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" width="50%"></TD>
						</TR>
					</TABLE>
					<TABLE id="Table19" cellSpacing="0" cellPadding="5" width="600" border="1">
						<TR class="RFHead2">
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="middle" width="12%">RF 
								ref. number</TD>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="middle" width="12%">Date 
								of purchase</TD>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="middle" width="12%">Monthly 
								instalment</TD>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="middle" width="12%">No. 
								of instalments</TD>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="middle" width="12%">Date 
								of final instalment</TD>
							<TD style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="left" width="40%">Description 
								of items</TD>
						</TR>
						<!-- this is where the accounts template should be applied -->
						<xsl:apply-templates select="RFSUMMARY/ACCTDETAILS/ACCOUNTS" />
					</TABLE>
					<TABLE class="RFHead2" id="Table20" style="BORDER-TOP-STYLE: none" cellSpacing="0" cellPadding="5" width="600" border="1">
						<TR>
							<TD style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP: gray 1pt; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="right" width="75%" class="RFHead2"> TOTAL 
								MONTHLY INSTALMENT</TD>
							<TD style="BORDER-TOP: gray 1pt; BORDER-BOTTOM: gray 1pt solid; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none" vAlign="top" align="left" width="25%"> 
								<xsl:value-of select="RFSUMMARY/ACCTDETAILS/TOTALMONTHLYINSTALMENT" />
							</TD>
						</TR>
						<TR>
							<TD class="RFHead2" style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP: gray 1pt; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="right" width="75%">MONTHLY 
								PAYMENT DATE</TD>
							<TD style="BORDER-TOP: gray 1pt solid; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="left" width="25%">
								<xsl:value-of select="RFSUMMARY/ACCTDETAILS/MONTHLYDUEDATE" />
							</TD>
						</TR>
					</TABLE>
				</p>
			</BODY>
		</HTML>
	</xsl:template>
	
	<xsl:template match="ACCOUNTS">
		<xsl:apply-templates select="ACCOUNT" />
	</xsl:template>
	
	<xsl:template match="ACCOUNT">
		<TR>
			<TD class="RFBody" style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP: gray 1pt solid; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="middle" width="12%">
				<xsl:value-of select="ACCTNO" />
			</TD>
			<TD class="RFBody" style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP: gray 1pt solid; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="middle" width="12%">
				<xsl:value-of select="DATEOPEN" />
			</TD>
			<TD class="RFBody" style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP: gray 1pt solid; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="middle" width="12%">
				<xsl:value-of select="MONTHLYINSTALMENT" />
			</TD>
			<TD class="RFBody" style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP: gray 1pt solid; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="middle" width="12%">
				<xsl:value-of select="NOOFINSTALMENTS" />
			</TD>
			<TD class="RFBody" style="BORDER-RIGHT: gray 1pt solid; BORDER-TOP: gray 1pt solid; BORDER-LEFT: gray 1pt; BORDER-BOTTOM: gray 1pt" vAlign="top" align="middle" width="12%">
				<xsl:value-of select="DATEOFFINALINSTAL" />
			</TD>
			<TD class="RFBody" style="BORDER-TOP: gray 1pt solid; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" vAlign="top" align="left" width="40%">
				<xsl:apply-templates select="ITEMS" />
			</TD>
		</TR>
	</xsl:template>
	
	<xsl:template match="ITEMS">
		<xsl:apply-templates select="ITEM" />
	</xsl:template>
	
	<xsl:template match="ITEM">
		<xsl:value-of select="DESCRIPTION" /><BR />
	</xsl:template>
	
</xsl:stylesheet>

  