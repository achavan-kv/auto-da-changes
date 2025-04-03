SIMPLESAMPLE

PURPOSE: 
This app demonstrates the most basic use of the AppUpdater component.  The major difference between this sample & IntroSample, is that this sample looks directly looks at the files on the server to detect new updates instead of using a server manifest.
  
SERVER SETUP:

* Create a folder on the web server. 
* Using the IIS manager, enable Directory Browsing for the new folder.

BUILD ADVANCEDSAMPLE
 
*  Open the AdvancedSample project.   You may need to re-add the AppUpdater reference.
*  Set the UpdateUrl property on the AppUpdater to point at the folder you created on your server.
*  Re-Build

CLIENT SETUP:

*  Copy SimpleSample_ClientSetup directory to your client computer.
*  Copy the Simpleample.exe & AppUpdater.dll you just built to the 1.0.0.0 directory on the client

PERFORM UPDATE:

* Launch the app with AppStart.exe 
* Update the app.  You do NOT need to update the version number in the assemblyinfo file.  Only last modified date/time is used.  
* Copy the new update into the folder you created on your web server.    
* The app will auto-update within 30 seconds.

