Layered Architecture Sample Application - JANUARY 2014 Release (updated for .NET 4.8)
by Serena Yeoh (serena_yeoh@yahoo.com)

LAYERED ARCHITECTURE SAMPLE
https://github.com/serenagrl/layer-samples

Last updated: October 2020

INTRODUCTION
============

Layered Architecture Sample is designed to demonstrate how to apply the 
Layered Architecture Pattern with various Microsoft .NET Technologies 
that are available in .NET Framework. It is aimed at illustrating how code of 
similar responsibilities can be factored into multiple logical layers which are 
applicable in most enterprise applications.

The primary objective of the sample is to focus on layering and therefore, certain 
cross-cutting functionalities have been omitted to maintain its simplicity.

1. PRE-REQUISITES
=================

1.1 CONFIGURING THE WORKFLOW INSTANCE STORE

    This sample requires the Workflow Instance Store to be created and 
    properly configured in ONE database named WorkflowInstanceStore.

    If you have chosen different database names, configure this sample
    to use them in the config files.

    If you have not configured your Workflow Instance Store, please follow
    these steps:

    1. Open SQL Server Management Studio and create a new database. 
       i.e. WorkflowInstanceStore

    2. Once the database is created, go to File->Open->File
    
    3. Navigate to the folder 
       %WINDIR%\Microsoft.NET\Framework\v4.xxx\SQL\EN

    4. Open the following scripts:
       a. SqlWorkflowInstanceStoreSchema.sql
       b. SqlWorkflowInstanceStoreLogic.sql
       c. SqlWorkflowInstanceStoreSchemaUpgrade.sql

    5. Ensure that the database i.e. WorkflowInstanceStore, is selected in the 
       database drop down.
 
    6. Execute the scripts in the order above.


2. SETUP INSTRUCTIONS
=====================

    1) Run the script provided in the Scripts folder to create the sample 
       database. Verify you have the LeaveSample database created after 
       execution. 
	   
	   Note: You can also deploy the database from the Database project.

    2) Configure the credentials to your database in the web.config file of the
       LeaveSample.Host.Web project.

    3) Right-click on the Solution, click on Enable Nuget Package Restore.
	
    4) Compile and run the solution.

    5) If for some reasons, the startup projects were messed up, configure
       the solution to Multiple Startup Projects and set Start to the following:

       LeaveSample.Hosts.Web
       LeaveSample.UI.Windows


3. MORE INFORMATION ON THE APPLICATION ARCHITECTURE
===================================================

For more information on Layered Architecture, please visit
http://serena-yeoh.blogspot.com/2013/06/layered-architecture-for-net.html