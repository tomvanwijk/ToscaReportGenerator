<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
	<xsl:output method="html" indent="yes"/>
	<xsl:template match="/">
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1" />
				<link rel="stylesheet" type="text/css" href="report.css"/>
				<title>
					Tosca Rapport
				</title>
				<script >
					function openParms(tableId) {
					document.getElementById('plusTable'+tableId).style.display='none';
					document.getElementById('minusTable'+tableId).style.display='inline';
					document.getElementById('parmTable'+tableId).style.display='block';
					}
					function closeParms(tableId) {
					document.getElementById('minusTable'+tableId).style.display='none';
					document.getElementById('plusTable'+tableId).style.display='inline';
					document.getElementById('parmTable'+tableId).style.display='none';
					}
				</script>
			</head>
			<body>
				<table class="tabledef headerBox">
					<tr>
						<td class="td">
							Rapportnaam
						</td>
						<td class="tdright">
							<xsl:value-of select="ExecutionListsRoot/@Naam"  disable-output-escaping="yes" />
						</td>
						<td class="td">
							Aantal testgevallen OK
						</td>
						<td class="tdright">
							<xsl:value-of select="count(//ExecutionEntry[@Status='Passed'])"  disable-output-escaping="yes" />
						</td>
					</tr>
					<tr>
						<td class="td" >Aanmaakdatum rapport</td>
						<td class="tdright">
							<xsl:value-of select="ExecutionListsRoot/@Aanmaakdatum"  disable-output-escaping="yes" />
						</td>

						<td class="td" style="width:20%">
							Aantal testgevallen NOK
						</td>
						<td class="tdright">
							<xsl:value-of select="count(//ExecutionEntry[@Status='Failed'])"  disable-output-escaping="yes" />
						</td>
					</tr>
					<tr>
						<td class="td">
							Aantal testgevallen
						</td>
						<td class="tdright">
							<xsl:value-of select="count(//ExecutionEntry)"  disable-output-escaping="yes" />
						</td>
						<td class="td" style="width:20%">
							Aantal testgevallen To Do
						</td>
						<td class="tdright">
							<xsl:value-of select="count(//ExecutionEntry[@Status='Not Run'])"  disable-output-escaping="yes" />
						</td>
					</tr>
				</table>

				<xsl:for-each select="ExecutionListsRoot/ExecutionList">
					<xsl:sort select="@Naam"/>
					<div class=" tabledef line box">
						<div class="ExecutionListLeft">
							<xsl:variable name="n" select="@NodePath"/>
							<span id="plusTable{$n}" class="expandBox expandBox-Normal expandBox-Plus" title="Show" onclick="openParms('{$n}');">+ </span>
							<span id="minusTable{$n}" class="expandBox expandBox-Normal expandBox-Min" title="Hide" onclick="closeParms('{$n}');">- </span>
							Naam ExecutionList:
						</div>
						<div class="ExecutionListRight">
							<div class="counter">
								<xsl:value-of select="@Naam"/>
							</div>
							<div class="counters rightNotRun">
								<xsl:value-of select="count(ExecutionEntryFolder/ExecutionEntry[@Status='Not Run'])+count(ExecutionEntry[@Status='Not Run'])"/>
							</div>
							<div class="counters rightFailed">
								<xsl:value-of select="count(ExecutionEntryFolder/ExecutionEntry[@Status='Failed'])+count(ExecutionEntry[@Status='Failed'])"/>
							</div>
							<div class="counters rightPassed">
								<xsl:value-of select="count(ExecutionEntryFolder/ExecutionEntry[@Status='Passed'])+count(ExecutionEntry[@Status='Passed'])"/>
							</div>
							<div class="counters rightTotaalWhite">
								<xsl:value-of select="count(ExecutionEntryFolder/ExecutionEntry)+count(ExecutionEntry)"/>
							</div>
						</div>
					</div>
					<div id="parmTable{@NodePath}" class="line tabledef" style="display:none;" >
						<xsl:for-each select="ExecutionEntryFolder">
							<div class="line line-ExecutionFolder tabledef">
								<div class="ExecutionEntryFolderLeft">
									<xsl:variable name="n" select="@NodePath"/>
									<span id="plusTable{$n}" class="expandBox expandBox-Normal expandBox-Plus" title="Show" onclick="openParms('{$n}');">+ </span>
									<span id="minusTable{$n}" class="expandBox expandBox-Normal expandBox-Min" title="Hide" onclick="closeParms('{$n}');">- </span>
									<xsl:value-of select="@Naam" disable-output-escaping="yes" />
								</div>
								<div class="ExecutionEntryFolderRight">
									<div class="subcounters rightNotRun">
										<xsl:value-of select="count(ExecutionEntry[@Status='Not Run'])"/>
									</div>
									<div class="subcounters rightFailed">
										<xsl:value-of select="count(ExecutionEntry[@Status='Failed'])"/>
									</div>
									<div class="subcounters rightPassed">
										<xsl:value-of select="count(ExecutionEntry[@Status='Passed'])"/>
									</div>
									<div class="subcounters rightTotaal">
										<xsl:value-of select="count(ExecutionEntry)"/>
									</div>

								</div>
							</div>
							<div id="parmTable{@NodePath}" style="display:none;" class="tabledef">
								<div class="line" style="margin-top:5px;"/>
								<xsl:for-each select="ExecutionEntry">
									<div class="line line-Executionentry">
										<div class="left left-ExecutionEntry" >
											<xsl:if test="@Status != 'Not Run'">
												<xsl:variable name="n" select="@NodePath"/>
												<span id="plusTable{$n}" class="expandBox expandBox-Normal expandBox-Plus" title="Show" onclick="openParms('{$n}');">+ </span>
												<span id="minusTable{$n}" class="expandBox expandBox-Normal expandBox-Min" title="Hide" onclick="closeParms('{$n}');">- </span>
											</xsl:if>
											<xsl:if test="@Status  = 'Not Run'">                                
												<span class="expandBox expandBox-Empty">X </span>
											</xsl:if>			      
											<xsl:value-of select="@Naam" disable-output-escaping="yes" />
										</div>
										<div class="right">
											<xsl:if test="@Status='Passed'">
												<FONT color="Green">PASSED</FONT>
											</xsl:if>
											<xsl:if test="@Status='Failed'">
												<FONT color="Red">FAILED</FONT>
											</xsl:if>
											<xsl:if test="@Status='Not Run'">
												NOT RUN
											</xsl:if>
										</div>
									</div>
									<div>
										<div id="parmTable{@NodePath}" style="margin-left:20px;display:none;">
											<br/>
											<xsl:if test="RequestFile !=''">
												<div class="line">
													<div class="info left line-Info">
														RequestFile
													</div>
													<div>
														<a href="{RequestFile}" target="_blank">
															<xsl:value-of select="RequestFile"/>
														</a>
													</div>
												</div>
											</xsl:if>
											<xsl:if test="ResponseFile !=''">
												<div class="line">
													<div class="info left line-Info">
														ResponseFile
													</div>
													<div>
														<a href="{ResponseFile}" target="_blank">
															<xsl:value-of select="ResponseFile"/>
														</a>
													</div>
												</div>
											</xsl:if>
											<xsl:for-each select="Parameter">
												<div class="line">
													<div class="info left line-Info">
														<xsl:value-of select="@Key"/>
													</div>
													<div>
														<xsl:value-of select="@Value"/>
													</div>
												</div>
											</xsl:for-each>
											<xsl:for-each select="Info">
												<div class="line">
													<div class="info left line-Info">Info</div>
													<div>
														<xsl:variable name="i" select="."/>
														<xsl:choose>
															<xsl:when test="contains($i,'- Failed')">
																<FONT color="Red">
																	<xsl:value-of select="."/>
																</FONT>
															</xsl:when>
															<xsl:otherwise>
																<xsl:value-of select="$i"/>
															</xsl:otherwise>
														</xsl:choose>
													</div>
												</div>
											</xsl:for-each>
										</div>
									</div>
								</xsl:for-each>
							</div>
						</xsl:for-each>
						<div class="line" style="margin-top:5px;"/>
						<xsl:for-each select="ExecutionEntry">
							<div class="line line-Executionentry">
								<div class="left left-Executionentry">
									<xsl:if test="@Status != 'Not Run'">
										<xsl:variable name="n" select="@NodePath"/>
										<span id="plusTable{$n}" class="expandBox expandBox-Normal expandBox-Plus" title="Show" onclick="openParms('{$n}');">+ </span>
										<span id="minusTable{$n}" class="expandBox expandBox-Normal expandBox-Min" title="Hide" onclick="closeParms('{$n}');">- </span>
									</xsl:if>
									<xsl:if test="@Status = 'Not Run'">                                
										<span class="expandBox expandBox-Empty">X </span>
									</xsl:if>			      
									<xsl:value-of select="@Naam" disable-output-escaping="yes" />
								</div>
								<div class="right">
									<xsl:if test="@Status='Passed'">
										<FONT color="Green">PASSED</FONT>
									</xsl:if>
									<xsl:if test="@Status='Failed'">
										<FONT color="Red">FAILED</FONT>
									</xsl:if>
									<xsl:if test="@Status='Not Run'">
										NOT RUN
									</xsl:if>
								</div>
							</div>
							<div>
								<div id="parmTable{@NodePath}" style="margin-left:20px;display:none;">
									<xsl:if test="RequestFile !=''">
										<div class="line">
											<div class="info left line-Info">
														RequestFile
											</div>
											<div>
												<a href="{RequestFile}" target="_blank">
													<xsl:value-of select="RequestFile"/>
												</a>
											</div>
										</div>
									</xsl:if>
									<xsl:if test="ResponseFile !=''">
										<div class="line">
											<div class="info left line-Info">
														ResponseFile
											</div>
											<div>
												<a href="{ResponseFile}" target="_blank">
													<xsl:value-of select="ResponseFile"/>
												</a>
											</div>
										</div>
									</xsl:if>
									<xsl:for-each select="Parameter">
										<div class="line">
											<div class="info left line-Info">
												<xsl:value-of select="@Key"/>
											</div>
											<div>
												<xsl:value-of select="@Value"/>
											</div>
										</div>
									</xsl:for-each>
									<xsl:for-each select="Info">
										<div class="line">
											<div class="info left line-Info">Info</div>
											<div>
												<xsl:variable name="i" select="."/>
												<xsl:choose>
													<xsl:when test="contains($i,'- Failed')">
														<FONT color="Red">
															<xsl:value-of select="."/>
														</FONT>
													</xsl:when>
													<xsl:otherwise>
														<xsl:value-of select="$i"/>
													</xsl:otherwise>
												</xsl:choose>
											</div>
										</div>
									</xsl:for-each>
								</div>
							</div>
						</xsl:for-each>
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>