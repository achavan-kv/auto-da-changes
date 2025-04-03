<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<head>
				<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"></meta>
				<style type="text/css" media="all">
					.cdiv_Report
					{
					BORDER-RIGHT: #848284 1px solid;
					BORDER-TOP: #848284 1px solid;
					;
					LEFT: expression(div_userBar.offsetWidth + 12);
					OVERFLOW: auto;
					BORDER-LEFT: #848284 1px solid;
					;
					WIDTH: expression((document.body.offsetWidth - div_userBar.offsetWidth) - 17);
					BORDER-BOTTOM: white 1px solid;
					POSITION: absolute;
					;
					TOP: expression(div_titleBar.offsetTop + div_titleBar.offsetHeight + 3);
					;
					HEIGHT: expression(document.body.offsetHeight - div_input.offsetTop - 5);
					BACKGROUND-COLOR: #e0edff
					}
					.cStatistics
					{
					FONT-SIZE: 8pt;
					COLOR: #000000;
					FONT-FAMILY: MS Sans Serif,Arial, Helvetica, sans-serif
					}
					.cStatHead
					{
					PADDING-RIGHT: 1px;
					PADDING-LEFT: 1px;
					FONT-WEIGHT: bold;
					FONT-SIZE: 10pt;
					PADDING-BOTTOM: 1px;
					COLOR: #ffffff;
					PADDING-TOP: 1px;
					FONT-FAMILY: MS Sans Serif,Arial, Helvetica, sans-serif;
					BACKGROUND-COLOR: #4169ab
					}
					.cStatHead2
					{
					PADDING-RIGHT: 1px;
					PADDING-LEFT: 1px;
					FONT-WEIGHT: bold;
					FONT-SIZE: 8pt;
					PADDING-BOTTOM: 1px;
					COLOR: #000000;
					PADDING-TOP: 1px;
					FONT-FAMILY: MS Sans Serif,Arial, Helvetica, sans-serif;
					BACKGROUND-COLOR: #99c7f0
					}
					.cStatDetail
					{
					PADDING-RIGHT: 1px;
					PADDING-LEFT: 1px;
					FONT-SIZE: 8pt;
					PADDING-BOTTOM: 1px;
					COLOR: #000000;
					PADDING-TOP: 1px;
					FONT-FAMILY: MS Sans Serif,Arial, Helvetica, sans-serif
					}
					.cStatDetail TD
					{
					BORDER-BOTTOM: #b5c7de 1px solid;
					BACKGROUND-COLOR: #e0edff
					}
					.cStatDetailB
					{
					PADDING-RIGHT: 1px;
					PADDING-LEFT: 1px;
					FONT-WEIGHT: bold;
					FONT-SIZE: 8pt;
					PADDING-BOTTOM: 1px;
					COLOR: #000000;
					PADDING-TOP: 1px;
					FONT-FAMILY: MS Sans Serif,Arial, Helvetica, sans-serif
					}
					.cReportMastHead
					{
					BACKGROUND-COLOR: #ffffff
					}
				</style>
			</head>
			<BODY  bgcolor="#e0edff">
				<xsl:apply-templates select="RSP_ENQ" />
			</BODY>
		</HTML>
	</xsl:template>

	<xsl:template match="RSP_ENQ">
		<div align="center">
			<table cellspacing="0" cellpadding="0" width="100%">
				<xsl:for-each select="RSP">

					<!-- STL
					<xsl:if test="CHILD_NO[. $gt$ 1]">
						<tr><td style="page-break-after:always"></td></tr>
					</xsl:if>
					-->

					<tr>
						<td>
							<table cellspacing="0" cellpadding="0" width="100%">
								<!-- STL we don't have the gif 
								<tr>
									<td colspan="4" height="40" valign="middle" width="100%" class="cReportMastHead">
										<img src="/cgi-crb/images/bureaustream_logo.gif" />
									</td>
								</tr>
								-->
								<xsl:for-each select="Header">
									<tr>
										<td width="28%" class="cStatHead" colspan="1">
											Enquiry Number
											<xsl:value-of select="@OrdItm" />
										</td>
										<td width="44%" class="cStatHead" colspan="2">
											<!--STL Client Reference:<xsl:entity-ref name="nbsp"/>-->
											Client Reference:
											<xsl:value-of select="ReqCr/@Ref"/>
										</td>
										<td width="28%" class="cStatHead" colspan="1">
											Enquiry Date
											<xsl:value-of select="ReqCr/@Dt" />
										</td>
									</tr>
									<!-- Not available for dpgroup
									<tr>
										<td class="cStatHead2">Enquiry Type</td>
										<td class="cStatHead2">Product Type</td>
										<td class="cStatHead2">Amount</td>
										<td class="cStatHead2">Guarantor</td>
									</tr>
									<tr>
										<td class="cStatDetail">
											<xsl:value-of select="//ENQ_TYPE" />
										</td>
										<td class="cStatDetail">
											<xsl:value-of select="//PRD_TYPE" />
										</td>
										<td class="cStatDetail">
											<xsl:value-of select="//AMOUNT" />
										</td>
										<td class="cStatDetail">
											<xsl:value-of select="CONSUMER_ENQ/GUARANTOR" />
										</td>
									</tr>
									-->
								</xsl:for-each>
							</table>
						</td>
					</tr>

					<tr>
						<td style="padding-top:10px;padding-bottom:5px;" align="center">
							<table cellspacing="0" cellpadding="0" class="cStatistics" width="98%">
								<tr>
									<td style="border-right: 1 solid #B5C7DE;" valign="top" align="left" width="50%">
										<table cellspacing="0" cellpadding="0" width="98%">
											<tr>
												<td style="padding-bottom: 10px;">
													<table cellspacing="0" cellpadding="0" width="100%">
														<tr>
															<td class="cStatHead">Details</td>
														</tr>

														<tr>
															<td>
																<table cellspacing="0" cellpadding="0" width="100%" class="cStatDetail">
																	<xsl:for-each select="RspDtl/PsnData">
																	<tr>
																		<td>ID</td>
																		<td align="right">
																			<xsl:value-of select="@ID" />
																		</td>
																	</tr>
																	<tr>
																		<td>Name</td>
																		<td align="right">
																			<xsl:value-of select="@Name" />
																		</td>
																	</tr>																	
																	<tr>
																		<td>Gender</td>
																		<td align="right">
																			<xsl:value-of select="@Gender" />
																		</td>
																	</tr>
																	<tr>
																		<td>Year of Birth</td>
																		<td align="right">
																			<xsl:value-of select="@DOY" />
																		</td>
																	</tr>
																	<tr>
																		<td>Marital Status</td>
																		<td align="right">
																			<xsl:value-of select="@Marital" />
																		</td>
																	</tr>
																	<tr>
																		<td>Nationality</td>
																		<td align="right">
																			<xsl:value-of select="@Nationality" />
																		</td>
																	</tr>
																	<tr>
																		<td>Ethnic Group</td>
																		<td align="right">
																			<xsl:value-of select="@Race" />
																		</td>
																	</tr>
																	</xsl:for-each>
																	<xsl:for-each select="RspDtl/EmplDet">
																		<xsl:if test="@EmplName!=''">
																			<tr>
																				<td>Employer Name</td>
																				<td align="right">
																					<xsl:value-of select="@EmplName" />
																				</td>
																			</tr>
																		</xsl:if>
																		<xsl:if test="@POS!=''">
																			<tr>
																				<td>Employment Position</td>
																				<td align="right" >
																					<xsl:value-of select="@POS" />
																				</td>
																			</tr>
																		</xsl:if>
																	</xsl:for-each>
																</table>
															</td>
														</tr>
													</table>
												</td>
											</tr>
										</table>
									</td>

									<td valign="top" style="padding-left:5px;">
										<table cellspacing="0" cellpadding="0" width="99%">
											<tr>
												<td style="padding-bottom: 10px;">
													<table cellspacing="0" cellpadding="0" width="100%">

														<!-- STL 
										<xsl:if test="CHILD_NO[. $gt$ 1]">
											<tr>
												<td class="cStatHead">
												</td>
											</tr>
										</xsl:if>
										-->

														<tr>
															<td class="cStatHead">Summary</td>
														</tr>

														<tr>
															<td>
																<table cellspacing="0" cellpadding="0" width="100%" class="cStatDetail">
																	<xsl:for-each select="RspDtl/SrhSum">
																		<xsl:if test="@Purpose='RV'">
																			<tr>
																				<td>Previous Account Review Enquiries</td>

																				<td align="right">
																					<xsl:value-of select="@Curr + @Last" />
																				</td>
																			</tr>
																		</xsl:if>
																		<xsl:if test="@Purpose='NA'">
																		<tr>
																			<td>New Application Enquiries</td>

																			<td align="right">
																				<xsl:value-of select="@Curr + @Last" />
																			</td>
																		</tr>
																		</xsl:if>
																		<xsl:if test="@Purpose='DC'">
																			<tr>
																				<td>Previous Debt Collection Enquiries</td>

																				<td align="right">
																					<xsl:value-of select="@Curr + @Last" />
																				</td>
																			</tr>
																		</xsl:if>

																	</xsl:for-each>
																	<xsl:for-each select="RspDtl/LitSum">
																		<tr>
																			<td>Bankruptcies</td>

																			<td align="right">
																				<xsl:value-of select="@Bankrupt" />
																			</td>
																		</tr>
																		<tr>
																			<td>Total Litigation</td>

																			<td align="right">
																				<xsl:value-of select="@LitPast + @LitPast2" />
																			</td>
																		</tr>
																	</xsl:for-each>
																</table>
															</td>
														</tr>
													</table>
												</td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</td>
					</tr>

					<tr>
						<td style="padding-top:10px;padding-bottom:5px;" align="center">
							<table cellspacing="0" cellpadding="0" width="98%">
								<xsl:for-each select="ADDITIONAL_IDS">
									<tr>
										<td style="padding-bottom: 10px;">
											<table cellspacing="0" cellpadding="0" width="100%">
												<tr>
													<td class="cStatHead" colspan="3">Additional Identification</td>
												</tr>

												<tr>
													<td class="cStatHead2" width="14%">Date Loaded</td>

													<td class="cStatHead2" width="20%">ID Type</td>

													<td class="cStatHead2">Id Code</td>
												</tr>

												<xsl:for-each select="ADDITIONAL_ID">
													<tr>
														<td class="cStatDetail">
															<xsl:value-of select="ADD_ID_LOAD_DATE" />
														</td>

														<td class="cStatDetail">
															<xsl:value-of select="ADD_ID_TYPE" />
														</td>

														<td class="cStatDetail">
															<xsl:value-of select="ADD_ID_CODE" />
														</td>
													</tr>
												</xsl:for-each>
											</table>
										</td>
									</tr>
								</xsl:for-each>
							</table>
						</td>
					</tr>

					<tr>
						<td style="padding-top:10px;padding-bottom:5px;" align="center">
							<table cellspacing="0" cellpadding="0" width="98%">
								<xsl:for-each select="ADDRESSES">
									<tr>
										<td style="padding-bottom: 10px;">
											<table cellspacing="0" cellpadding="0" width="100%">
												<tr>
													<td class="cStatHead" colspan="2">Addresses</td>
												</tr>

												<tr>
													<td class="cStatHead2" width="14%">Date Loaded</td>

													<td class="cStatHead2">Address</td>
												</tr>

												<xsl:for-each select="ADDRESS">
													<tr>
														<td class="cStatDetail">
															<xsl:value-of select="ADR_LOAD_DATE" />
														</td>

														<td class="cStatDetail">
															<xsl:value-of select="ADR_ADDRESS" />
														</td>
													</tr>
												</xsl:for-each>
											</table>
										</td>
									</tr>
								</xsl:for-each>
							</table>
						</td>
					</tr>

					<xsl:for-each select="OCCUPATIONS">
						<tr>
							<td style="padding-top:10px;padding-bottom:5px;" align="center">
								<table cellspacing="0" cellpadding="0" width="98%">
									<tr>
										<td style="padding-bottom: 10px;">
											<table cellspacing="0" cellpadding="0" width="100%">
												<tr>
													<td class="cStatHead" colspan="3">Employment</td>
												</tr>

												<tr>
													<td class="cStatHead2" width="14%">Date Loaded</td>

													<td class="cStatHead2" width="43%">Occupation</td>

													<td class="cStatHead2" width="43%">Employer</td>
												</tr>

												<xsl:for-each select="OCCUPATION">
													<tr>
														<td class="cStatDetail">
															<xsl:value-of select="OCC_LOAD_DATE" />
														</td>

														<td class="cStatDetail">
															<xsl:value-of select="OCC_OCCUPATION" />
														</td>

														<td class="cStatDetail">
															<xsl:value-of select="OCC_EMPLOYER" />
														</td>

														<td class="cStatDetail" align="right">
															<xsl:value-of select="INQUIRY_AMOUNT" />
														</td>
													</tr>
												</xsl:for-each>
											</table>
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</xsl:for-each>

					<xsl:for-each select="ADDITIONAL_NAMES">
						<tr>
							<td style="padding-top:10px;padding-bottom:5px;" align="center">
								<table cellspacing="0" cellpadding="0" width="98%">
									<tr>
										<td style="padding-bottom: 10px;">
											<table cellspacing="0" cellpadding="0" width="100%">
												<tr>
													<td class="cStatHead" colspan="2">Additional Names</td>
												</tr>

												<tr>
													<td class="cStatHead2" width="14%">Date Loaded</td>

													<td class="cStatHead2">Name</td>
												</tr>

												<xsl:for-each select="ADDITIONAL_NAME">
													<tr>
														<td class="cStatDetail">
															<xsl:value-of select="INN_LOAD_DATE" />
														</td>

														<td class="cStatDetail">
															<xsl:value-of select="INN_NAME" />
														</td>
													</tr>
												</xsl:for-each>
											</table>
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</xsl:for-each>

					<xsl:for-each select="CNT_NUMBERS">
						<tr>
							<td style="padding-top:10px;padding-bottom:5px;" align="center">
								<table cellspacing="0" cellpadding="0" width="98%">
									<td style="padding-bottom: 10px;">
										<table cellspacing="0" cellpadding="0" width="100%">
											<tr>
												<td class="cStatHead" colspan="6">Contact Numbers</td>
											</tr>

											<tr>
												<td class="cStatHead2" width="20%">Date Loaded</td>

												<td class="cStatHead2" width="40%">Type</td>

												<td class="cStatHead2" width="40%">Number</td>

											</tr>

											<xsl:for-each select="CNT_NUMBER">
												<tr>
													<td class="cStatDetail">
														<xsl:value-of select="CNT_LOAD_DATE" />
													</td>

													<td class="cStatDetail">
														<xsl:value-of select="CNT_TYPE" />
													</td>

													<td class="cStatDetail">
														<xsl:value-of select="CNT_PHONE" />
													</td>

												</tr>
											</xsl:for-each>
										</table>
									</td>
								</table>
							</td>
						</tr>
					</xsl:for-each>

					<tr>
						<td style="padding-top:10px;padding-bottom:5px;" align="center">
							<table cellspacing="0" cellpadding="0" width="98%">
								<xsl:for-each select="PREVIOUS_INQUIRIES">
									<tr>
										<td style="padding-bottom: 10px;">
											<table cellspacing="0" cellpadding="0" width="100%">
												<tr>
													<td class="cStatHead" colspan="4">Previous Enquiries</td>
												</tr>

												<tr>
													<td class="cStatHead2" width="14%">Date Loaded</td>

													<td class="cStatHead2" width="58%">Enquirer</td>

													<td class="cStatHead2" width="14%">Product Type</td>

													<td class="cStatHead2" width="14%" align="right">Amount</td>
												</tr>

												<xsl:for-each select="PREVIOUS_INQUIRY">
													<tr>
														<td class="cStatDetail">
															<xsl:value-of select="IPI_LOAD_DATE" />
														</td>

														<td class="cStatDetail">
															<xsl:value-of select="IPI_NAME" />
														</td>

														<td class="cStatDetail">
															<xsl:value-of select="IPI_PRODUCT_TYPE" />
														</td>

														<td class="cStatDetail" align="right">
															<xsl:value-of select="IPI_AMOUNT" />
														</td>
													</tr>
												</xsl:for-each>
											</table>
										</td>
									</tr>
								</xsl:for-each>

								<xsl:for-each select="PAYMENT_DEFAULTS">
									<tr>
										<td style="padding-bottom: 10px;">
											<table cellspacing="0" cellpadding="0" width="100%">
												<tr>
													<td class="cStatHead" colspan="6">Payment Defaults</td>
												</tr>

												<tr>
													<td class="cStatHead2" width="14%">Date Loaded</td>

													<td class="cStatHead2" width="38%">Creditor</td>

													<td class="cStatHead2" width="12%">Status</td>

													<td class="cStatHead2" align="right" width="12%">Amount</td>

													<td class="cStatHead2" align="right" width="12%">Balance</td>

													<td class="cStatHead2" align="right" width="12%">Status Date</td>
												</tr>

												<xsl:for-each select="PAYMENT_DEFAULT">
													<tr>
														<td class="cStatDetail">
															<xsl:value-of select="IPD_LOAD_DATE" />
														</td>

														<td class="cStatDetail">
															<xsl:value-of select="IPD_NAME" />
														</td>

														<td class="cStatDetail">
															<xsl:value-of select="IPD_STATUS" />
														</td>

														<td class="cStatDetail" align="right">
															<xsl:value-of select="IPD_AMOUNT" />
														</td>

														<td class="cStatDetail" align="right">
															<xsl:value-of select="IPD_BALANCE" />
														</td>

														<td class="cStatDetail" align="right">
															<xsl:value-of select="IPD_STATUS_DATE" />
														</td>
													</tr>
												</xsl:for-each>
											</table>
										</td>
									</tr>
								</xsl:for-each>

								<xsl:for-each select="DEBTSTREAM_DETAILS">
									<tr>
										<td style="padding-bottom: 10px;">
											<table cellspacing="0" cellpadding="0" width="100%">
												<tr>
													<td class="cStatHead" colspan="8">Collections</td>
												</tr>

												<tr>
													<!-- <td class="cStatHead2" width="10%">Date Case Loaded</td> -->
													<!-- <td class="cStatHead2" width="30%">Creditor</td>-->
													<!-- <td class="cStatHead2" width="7%">Case Number</td>-->
													<!-- <td class="cStatHead2" align="right" width="12%">Original Value</td>-->
													<!-- <td class="cStatHead2" align="right" width="12%">Current Balance</td>-->
													<!-- <td class="cStatHead2" align="right" width="12%">Status</td>-->
													<!--<td class="cStatHead2" align="right" width="12%">Status Date</td>-->
													<td class="cStatHead2" width="10%">Date Case Loaded</td>
													<td class="cStatHead2" width="24%">Creditor</td>
													<td class="cStatHead2" width="24%">Debtor</td>
													<td class="cStatHead2" width="7%">Case Number</td>
													<td class="cStatHead2" align="right" width="10%">Original Value</td>
													<td class="cStatHead2" align="right" width="10%">Current Balance</td>
													<td class="cStatHead2" align="right" width="6%">Status</td>
													<td class="cStatHead2" align="right" width="7%">Status Date</td>
												</tr>

												<xsl:for-each select="DEBTSTREAM_DETAIL">
													<tr>
														<td class="cStatDetail">
															<xsl:value-of select="DTI_LOAD_DATE" />
														</td>
														<td class="cStatDetail">
															<xsl:value-of select="DTI_CREDITOR" />
														</td>
														<!--ADDED-->
														<td class="cStatDetail">
															<xsl:value-of select="DTI_DEBTOR" />
														</td>
														<td class="cStatDetail">
															<xsl:value-of select="DTI_CLAIM" />
														</td>
														<td class="cStatDetail" align="right">
															<xsl:value-of select="DTI_VALUE" />
														</td>
														<td class="cStatDetail" align="right">
															<xsl:value-of select="DTI_BALANCE" />
														</td>
														<!--<td class="cStatDetail" align="right"><xsl:value-of select="DTI_STATUS" /></td> -->
														<td class="cStatDetail" align="center">
															<xsl:value-of select="DTI_STATUS" />
														</td>
														<td class="cStatDetail" align="right">
															<xsl:value-of select="DTI_STATUS_DATE" />
														</td>
													</tr>
												</xsl:for-each>
											</table>
										</td>
									</tr>
								</xsl:for-each>

								<xsl:for-each select="DEBTSTREAM_MSG">
									<tr>
										<td style="padding-bottom: 10px;">
											<table cellspacing="0" cellpadding="0" width="100%">
												<tr>
													<td class="cStatHead" colspan="8">Collections</td>
												</tr>
												<tr>
													<td class="cStatDetail">
														<xsl:value-of select="DEBTSTREAM_MSG_TXT" />
													</td>
												</tr>
											</table>
										</td>
									</tr>
								</xsl:for-each>

								<xsl:for-each select="BANKRUPTCY_DETAILS">
									<tr>
										<td style="padding-bottom: 10px;">
											<table cellspacing="0" cellpadding="0" width="100%">
												<tr>
													<td class="cStatHead" colspan="8">Bankruptcy Proceedings</td>
												</tr>

												<tr>
													<td class="cStatHead2" width="19%">Bankruptcy Number</td>
													<td class="cStatHead2" width="19%">Order Date</td>
													<td class="cStatHead2" width="19%">Petition Date</td>
													<td class="cStatHead2" width="19%">Original Order Date</td>
													<td class="cStatHead2" width="19%">Gazette Date</td>
												</tr>

												<xsl:for-each select="BANKRUPTCY_DETAIL">
													<tr>
														<td class="cStatDetail">
															<xsl:value-of select="BKT_NUMBER" />
														</td>
														<td class="cStatDetail">
															<xsl:value-of select="BKT_ORDER_DATE" />
														</td>
														<td class="cStatDetail">
															<xsl:value-of select="BKT_PETITON_DATE" />
														</td>
														<td class="cStatDetail">
															<xsl:value-of select="BKT_ORG_ORD_DATE" />
														</td>
														<td class="cStatDetail">
															<xsl:value-of select="BKT_GAZETTE_DATE" />
														</td>
													</tr>

													<tr>
														<td class="cStatDetail" width="95%" colspan="5">
															<xsl:value-of select="BKT_NATURE_OF_ORDER" />
															<br />
															<br />
														</td>
													</tr>
												</xsl:for-each>


											</table>
										</td>
									</tr>
								</xsl:for-each>

								<xsl:for-each select="PUBLIC_NOTICES">
									<tr>
										<td style="padding-bottom: 10px;">
											<table cellspacing="0" cellpadding="0" width="100%">
												<tr>
													<td class="cStatHead" colspan="3">Public Notices</td>
												</tr>

												<tr>
													<td class="cStatHead2" width="14%">Date Loaded</td>

													<td class="cStatHead2" width="14%">Type</td>

													<td class="cStatHead2">Publication</td>
												</tr>

												<xsl:for-each select="PUBLIC_NOTICE">
													<tr>
														<td class="cStatDetail">
															<xsl:value-of select="IPN_LOAD_DATE" />
														</td>
														<td class="cStatDetail">
															<xsl:value-of select="IPN_TYPE" />
														</td>
														<td class="cStatDetail">
															<xsl:value-of select="IPN_PUBLICATION" />
														</td>
													</tr>
													<xsl:for-each select="IPN_TEXT">
														<tr>
															<td class="cStatDetail" colspan="3">
																<!--STL <pre />-->
																<xsl:value-of select="IPT_LINE" />
															</td>
														</tr>
													</xsl:for-each>
													<tr>
														<td class="cStatDetail">
														</td>
													</tr>
												</xsl:for-each>
											</table>
										</td>
									</tr>
								</xsl:for-each>

								<xsl:for-each select="NARRATIVES">
									<tr>
										<td style="padding-bottom: 10px;">
											<table cellspacing="0" cellpadding="0" width="100%">
												<tr>
													<td class="cStatHead" colspan="2">Narrative</td>
												</tr>

												<tr>
													<td class="cStatHead2">Date Loaded</td>
												</tr>

												<xsl:for-each select="NARRATIVE">
													<tr>
														<td class="cStatDetail">
															<xsl:value-of select="NAR_LOAD_DATE" />
														</td>
													</tr>

													<xsl:for-each select="NAR_TEXT">
														<tr>
															<td class="cStatDetail">
																<xsl:value-of select="NAR_LINE" />
															</td>
														</tr>
													</xsl:for-each>

													<tr>
														<td class="cStatDetail">
														</td>
													</tr>
												</xsl:for-each>
											</table>
										</td>
									</tr>
								</xsl:for-each>

								<xsl:for-each select="NO_ADVERSE">
									<tr>
										<td style="padding-bottom: 10px;">
											<table cellspacing="0" cellpadding="0" width="100%">
												<tr>
													<td class="cStatDetailB" colspan="2">
														<xsl:value-of select="NO_ADVERSE_TEXT" />
													</td>
												</tr>
											</table>
										</td>
									</tr>
								</xsl:for-each>

								
							</table>
						</td>
					</tr>
				</xsl:for-each>
			</table>
		</div>
	</xsl:template>
</xsl:stylesheet>

