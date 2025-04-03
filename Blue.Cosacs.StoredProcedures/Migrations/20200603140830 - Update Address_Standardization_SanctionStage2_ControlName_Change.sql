

--Script for Address Standardization CR2019 - 025

UPDATE MandatoryFields Set Control ='gbData.tcApplicants.tpApp1.tcApp1.tpEmployer.grpEmployer.tcEmpAddress.cmbVillage'
WHERE CONTROL = 'gbData.tcApplicants.tpApp1.tcApp1.tpEmployer.grpEmployer.tcEmpAddress.txtAddress2'

UPDATE MandatoryFields Set Control ='gbData.tcApplicants.tpApp1.tcApp1.tpEmployer.grpEmployer.tcEmpAddress.cmbRegion'
WHERE CONTROL = 'gbData.tcApplicants.tpApp1.tcApp1.tpEmployer.grpEmployer.tcEmpAddress.txtAddress3'

UPDATE MandatoryFields Set Control ='gbData.tcApplicants.tpApp1.tcApp1.tpPreviousAddress.grpPrevAddress.tcPrevAddress.cmbVillage'
WHERE CONTROL = 'gbData.tcApplicants.tpApp1.tcApp1.tpPreviousAddress.grpPrevAddress.tcPrevAddress.txtAddress2'

UPDATE MandatoryFields Set Control ='gbData.tcApplicants.tpApp1.tcApp1.tpPreviousAddress.grpPrevAddress.tcPrevAddress.cmbRegion'
WHERE CONTROL = 'gbData.tcApplicants.tpApp1.tcApp1.tpPreviousAddress.grpPrevAddress.tcPrevAddress.txtAddress3'