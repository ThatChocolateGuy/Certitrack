# Certitrack [![Build status](https://dev.azure.com/NemoCodes/certitrack/_apis/build/status/certitrack-demo%20-%20CI)](https://dev.azure.com/NemoCodes/certitrack/_build/latest?definitionId=4) ![.NET Core](https://github.com/ThatChocolateGuy/Certitrack/workflows/.NET%20Core/badge.svg)
ASP.NET Core Gift Certificate Management System

## Overview
Certitrack is a fairly simple, yet relationally complex Gift Certificate Management System (GCMS).
Issue, track, edit, delete, and redeem certificates. Generate PDF reports. Easily manage staff, clients and much more. Suitable for any organization or team.

Built with Microsoft’s .NET Core stack, Certitrack leverages the power of a robust and secure relational SQL database.
This server-rendered app uses the Bootstrap-based AdminLTE frontend framework, along with AJAX requests where possible, yielding a fast and highly responsive user experience (UX).

Certitrack is currently being reworked into separate backend (.NET Core Web API) and frontend (Vue.js) services to provide an even smoother UX.

## DEMO: <a href="https://certitrack-gcms-demo.azurewebsites.net/" target="_blank">Certitrack-Demo-App</a>
#### U: ```admin@certitrack.com```  P: ```admin123```
#### U: ```nonadmin@certitrack.com```  P: ```nonadmin```
<sub>
	Note: Please be patient as app may take some time to cold boot.
	Certitrack is hosted on a serverless Azure instance with auto-pausing db to conserve resources and save on overhead costs. This stuff ain’t cheap!.
</sub>

## INSTALLATION:
1. Clone the repo
2. Launch the .csproj with VS or VSCode
3. Run with IIS or Docker '''(ctrl + F5 or F5 to debug)'''
4. Explore the app with the above login credentials
<sub>
	Note: Due to limitations within the jsreport nuget packages, PDF reports will not generate on an IIS-based Azure App Service.
	Instead, build and publish the project's Docker image to a container on any linux environment (Azure Web App for Containers recommended).
	Please install and get a basic understanding of Docker before undertaking any of this. I guarantee you will confuse yourself if you don't!
</sub>

## BUGS:
- Please create an issue or pull request for this README if you find any

## NEW FEATURES TO ADD:	
- edit customer name freely for redeemed certificate
- dashboard to show aggregates
-- filter by time periods
