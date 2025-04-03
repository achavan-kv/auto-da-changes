[System.Reflection.Assembly]::LoadWithPartialName("System.Web.Extensions")

$manifest = "{
		DbServer:'mssql.cosacs.net',
		DbName:'Cosacs_Test_100',
		DbUsername:'sa',
		DbPassword:'',
		ReportServer:'mssql.cosacs.net',
		ReportDbName:'Cosacs_Test_100',
		ReportUsername:'sa',
		ReportPassword:'',
		AnalysisServicesServerName:'',
		AnalysisServicesDbName:'',
		IisServer:'localhost',
		SitePort:'10010',
		CronPort:'9001',
		InstallPath:'C:\\CosacsGridTests\\Ver10',
		SeleniumHubPath: 'C:\\Selenium',
		BuildVersion:'10.0',
		Modules : [
				'SalesManagement',
				'Sales',
				'NonStocks',
				'Payments',
                'Financial',
				'Customer',
'Communication'
		]
	}"
	
	# kill process
	# $softwarelist = 'redis-server|Blue.Cron.Service|Blue.Hub.Service|java'
	
	# get-process |
    # Where-Object {$_.ProcessName -match $softwarelist} |
    # Stop-Process -force

    $endPoint = "http://grid.cosacs.net:8088/go"
    $wr = [System.Net.HttpWebRequest]::Create($endPoint)
    $wr.Method= 'POST';
    $wr.ContentType="application/json";
    $Body = [byte[]][char[]]$manifest;
    $wr.Timeout = 900000;

    $Stream = $wr.GetRequestStream();

    $Stream.Write($Body, 0, $Body.Length);

    $Stream.Flush();
    $Stream.Close();

    $resp = $wr.GetResponse().GetResponseStream();

    $sr = New-Object System.IO.StreamReader($resp);

     Write-Host $sr.ReadToEnd();
	 
	# $seleniumStartHub = "C:\Selenium-Grid\StartHub.cmd";
	# $seleniumStartNode = "C:\Selenium-Grid\StartNode.cmd";
	
    # Start-Process $seleniumStartHub;
	# Start-Process $seleniumStartNode;
