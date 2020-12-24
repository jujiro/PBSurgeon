Grab the port and the PBIX file name.

Go to this location:
%LocalAppData%\Microsoft\Power BI Desktop\AnalysisServicesWorkspaces

Folders (One per each opened PBIX file): 
AnalysisServicesWorkspace_b6c011cf-6636-4e05-9923-260ea79f2446
AnalysisServicesWorkspace_6c56ff53-7b30-45b2-b9df-2d78ca54feff

Look for:
AnalysisServicesWorkspace_b6c011cf-6636-4e05-9923-260ea79f2446/data/msmdsrv.port.txt (This will contain the port for the analytic services.)

Look for:
AnalysisServicesWorkspace_b6c011cf-6636-4e05-9923-260ea79f2446/data/FlightRecorderBack.trc

and look for the following fragment to get the underlying PBIX filename.
<ddl700_700:PackagePath>Y:\Devlp\Ashish-Projects\github-jujiro\PBSurgeon\docs\AdventureWorksDW2017.pbix</ddl700_700:PackagePath>
