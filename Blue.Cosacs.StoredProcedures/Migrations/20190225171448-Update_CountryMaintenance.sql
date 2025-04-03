-- Script Comment : Update CountryMaintenance
-- Script Name : Update_Equifax_CountryMaintenance.sql 
-- Description- , E - Equifax Parallel (Equifax Behavioural and Equifax Applicant).
-- Created For	: BB/BZ/TT
-- Created By	: Nilesh
-- Created On	: CR
-- Modified On	Modified By	Comment
	
	UPDATE CountryMaintenance  
	SET [Description]='This parameter sets the scoring process. A - Applicant ScoreCards, B - Behavioural Scoring (Branch Active, Select active branches in branch maintenance screen), P - Parallel (Behavioural and Applicant), S - Behavioural Scoring (Underwriters and Newsales)
	C- Equifax Applicant ScoreCards, D - Equifax Behavioural Scoring (Branch Active, Select active branches in branch maintenance screen)'
	WHERE CodeName='BehaviouralScorecard' and ParameterCategory='07'
		