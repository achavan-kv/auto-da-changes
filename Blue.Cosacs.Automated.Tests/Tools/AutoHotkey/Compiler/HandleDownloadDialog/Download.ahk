;[AUTOSTART]_______________________________________________________________________________________
;// set up some global defaults:	
	#singleInstance force	
	#persistent	
	#keyHistory 0
	#winActivateForce
	setBatchLines -1
	setTitleMatchMode 2
	setTitleMatchMode slow
	detectHiddenWindows on
	setWinDelay 0
	SetKeyDelay 10
	autotrim on
	stringcaseSense off
	
doDownload:
	;// FF Win7
	#ifWinExist Opening ahk_class MozillaDialogClass
	
		strWindowTitle := "Opening ahk_class MozillaDialogClass"
		winActivate % strWindowTitle
		sleep 500
		send {Down}{enter}
		sleep 500

	#ifWinExist
	
exitApp