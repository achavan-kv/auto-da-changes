ADVANCEDSAMPLE

PURPOSE: 
This app demonstrates several advanced uses of the AppUpdater component.  Specifically, this sample demonstrates:
  
*  Using a custom Web Service to check for udpates
*  Using AppStart in appdomain mode. (i.e. a single process and two appdomains)
*  Replacing the default AppUpdater UI with your own
*  Use of the AppUpdater Object Model

SERVER SETUP:

*  Copy the AdvancedSample_WebService folder to your wwwroot directory of your web server.
*  Open the properties on this folder in the IIS manager.  Click the "Create" button to create an application.
*  Using the IIS manager, enable Directory Browsing for AdvancedSample_WebService\AppBuilds folder.

BUILD ADVANCEDSAMPLE
 
*  Open the AdvancedSample project.   You may need to re-add the AppUpdater reference.
*  Set the web service URL to point at the webservice on your web server.  The default is:
     updateService.Url = "http://localhost/AdvancedSample_WebService/UpdateService.asmx";
*  Re-Build

CLIENT SETUP:

*  Copy AdvancedSample_ClientSetup directory to your client computer.
*  Copy the AdvancedSample.exe & AppUpdater.dll you just built to the 1.0.0.0 directory on the client
*  Copy the AdvancedSample.exe & AppUpdater.dll you just built to the AppBuilds directory of the web serfvice.

PERFORM UPDATE:

* Launch the app with AppStart.exe  Notice only one process starts.
* Update the app.  Update the version number in assemblyinfo.cs
* Copy the new update over the existing files in the AppBuild directory of the webservice.  
* The web service will detect a newer available version & inform the client the next time the client checks for an update.
* The app will auto-update within 30 seconds.