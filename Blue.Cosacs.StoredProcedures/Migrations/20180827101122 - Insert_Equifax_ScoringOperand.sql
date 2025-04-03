
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Age')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Age','numeric',0)	
	END
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Avg_agreement_total_1m')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Avg_agreement_total_1m','numeric',0)	
	END		
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Avg_agreement_total_1m_sq')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Avg_agreement_total_1m_sq','numeric',0)
	END
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Avg_balance_arrears_12m')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Avg_balance_arrears_12m','numeric',0)	
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Avg_balance_arrears_12m_ln')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Avg_balance_arrears_12m_ln','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Avg_balance_arrears_1m_ln')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Avg_balance_arrears_1m_ln','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Balancearrears_pound_6m')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Balancearrears_pound_6m','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Balancearrears_pound_6m_ln')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Balancearrears_pound_6m_ln','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Count_daysarrear_30more_17m')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Count_daysarrear_30more_17m','numeric',0)	
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Count_daysarrear_30more_17m_ln')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Count_daysarrear_30more_17m_ln','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Count_daysarrear_60more_17m_ln')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Count_daysarrear_60more_17m_ln','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Daysarrears_pound_6m')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Daysarrears_pound_6m','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Employmentstatus_woe')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions,DropDownName) values 
	('Employmentstatus_woe','option',20,'EmploymentStatus')
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Flag_customerstatus_his_woe')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions,DropDownName) values 
	('Flag_customerstatus_his_woe','option',25,'FlagCustomerStatus')
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Gender_woe')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions,DropDownName) values 
	('Gender_woe','option',0,'Gender')
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Maritalstatus_woe')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions,DropDownName) values 
	('Maritalstatus_woe','option',15,'MaritalStatus')
	END
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='max_perc_outs_3m')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('max_perc_outs_3m','numeric',0)	
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Max_perc_outs_3m_sq')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Max_perc_outs_3m_sq','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='max_perc_outsarrears_6m')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('max_perc_outsarrears_6m','numeric',0)	
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Max_perc_outsarrears_6m_ln')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Max_perc_outsarrears_6m_ln','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Mobilenumber_woe')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions,DropDownName) values 
	('Mobilenumber_woe','option',0,'MobileNumber')
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Newest_credit')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Newest_credit','numeric',0)	
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Newest_credit_sq')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Newest_credit_sq','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Number_account_17m')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values
	 ('Number_account_17m','numeric',0)
	 END	 
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Number_account_opened_3m')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Number_account_opened_3m','numeric',0)
	END	
	 IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Number_account_opened_3m_cr')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Number_account_opened_3m_cr','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Numberdependents')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Numberdependents','numeric',0)
	END		
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Numberdependents_cr')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Numberdependents_cr','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Numberdependents_sq')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Numberdependents_sq','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Occupation_woe')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions,DropDownName) values 
	('Occupation_woe','option',21,'Occupation')
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Oldest_credit')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Oldest_credit','numeric',0)	
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Oldest_credit_ln')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Oldest_credit_ln','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Postcode_woe')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Postcode_woe','free',0)
	END
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Ratio_ndependent_to_age')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Ratio_ndependent_to_age','numeric',0)
	END
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Ratio_tcurrentemploy_to_age')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Ratio_tcurrentemploy_to_age','numeric',0)
	END		
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Residentialstatus_woe')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions,DropDownName) values 
	('Residentialstatus_woe','option',19,'ResidentialStatus')
	END
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Timecurrentaddress')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Timecurrentaddress','numeric',0)
	END		
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Timecurrentaddress_ln')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Timecurrentaddress_ln','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Timecurrentemployment')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Timecurrentemployment','numeric',0)
	END
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Timecurrentaddress_ln')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Timecurrentaddress_ln','numeric',0)
	END	
	IF NOT EXISTS (SELECT * FROM [Equifax_ScoringOperand] WHERE [operandname]='Timecurrentemployment_sr')
	BEGIN
	Insert into Equifax_ScoringOperand (operandname,operandType,OperandOptions) values 
	('Timecurrentemployment_sr','numeric',0)	
	END