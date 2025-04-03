set BUILD_NUMBER=1
set ReleaseVersion=9
set connstring=server=.;database=CoSaCS;Integrated Security=SSPI;Connection Reset=false;MultipleActiveResultSets=true;Application Name=CosacsWeb
set grid-connstring=server=.;database=CoSaCS;Integrated Security=SSPI;Connection Reset=false;MultipleActiveResultSets=true;Application Name=CosacsWeb
set grid-connstring-sa=server=.;database=CoSaCS;Integrated Security=SSPI;Connection Reset=false;MultipleActiveResultSets=true;Application Name=CosacsWeb
set grid-connstring-sa=server=.;database=CoSaCS;Integrated Security=SSPI;Connection Reset=false;MultipleActiveResultSets=true;Application Name=CosacsWeb
set Modules=sales,customers
set Modules_Guid=1,2
..\bin\nant\bin\NAnt.exe -buildfile:cosacs.build wix-web
