DELETE FROM dbo.CountryMaintenance
WHERE CodeName IN ('allowdnaddedtoload','onepicklistpertruck','reqdeldatefilter',
'printpicklist','CancelDelNoteIfFailed','ServiceLocation','ServiceMatrix',
'ServiceResolutionFault','ServiceActionRequired','SRRepairEstimate',
'SRClsdTabBeforeResTab','SRAwaitCustPayment','SRAcctName',
'MonthsSinceSRResolved','DaysSinceSRLastUpdated')
