
ALTER TABLE Service.Request ADD
	Evaluation int NULL,
	EvaluationLocation int NULL,
	EvaluationAction int NULL,
	EstimateReceived smalldatetime NULL,
	EstimateLabourCost money NULL,
	EstimateAdditionalLabourCost money NULL,
	EstimateTransportCost money NULL,
	AllocationItemReceivedOn smalldatetime NULL,
	AllocationPartExpectOn smalldatetime NULL,
	AllocationZone int NULL,
	AllocationTechnician nchar(10) NULL,
	AllocationServiceScheduledOn datetime NULL,
	AllocationInstructions varchar(4000) NULL,
	Resolution int NULL,
	ResolutionDate smalldatetime NULL,
	ResolutionSupplierToCharge int NULL,
	ResolutionCategory int NULL,
	ResolutionReport varchar(4000) NULL,
	FinalizedFailure int NULL,
	FinializeReturnDate smalldatetime NULL,
	Comments varchar(4000) NULL
GO
