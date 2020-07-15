# Certitrack
[![Build status](https://dev.azure.com/NemoCodes/certitrack/_apis/build/status/certitrack-demo%20-%20CI)](https://dev.azure.com/NemoCodes/certitrack/_build/latest?definitionId=4) ![.NET Core](https://github.com/ThatChocolateGuy/Certitrack/workflows/.NET%20Core/badge.svg) ![Docker](https://github.com/ThatChocolateGuy/Certitrack/workflows/Docker/badge.svg?branch=develop)

ASP.NET Core Gift Certificate Management System

## Overview
Certitrack is a fairly simple, yet relationally complex Gift Certificate Management System (GCMS).
Issue, track, edit, delete, and redeem certificates. Generate PDF reports. Easily manage staff, clients and much more. Suitable for any organization or team.

Built with Microsoft’s .NET Core stack, Certitrack leverages the power of a robust and secure relational SQL database.
This server-rendered app uses the Bootstrap-based AdminLTE frontend framework, along with AJAX requests where possible, yielding a fast and highly responsive user experience (UX).

Certitrack is Dockerized and deployed with Azure Pipleines (CI/CD).

## DEMO: <a href="https://certitrack.nem.codes/" target="_blank">Certitrack-Demo-App</a>
#### U: `admin@certitrack.com`  P: `admin123`
#### U: `nonadmin@certitrack.com`  P: `nonadmin`

## USAGE:
1. Clone the repo
2. Launch the `.csproj` with VS or VSCode
3. Run with IIS or Docker `(ctrl + F5 or F5 to debug)`
4. Explore the app with the above login credentials
<sub>
	Note: Due to limitations within the jsreport nuget packages, PDF reports will not generate on an IIS-based Azure App Service.
	Instead, build and publish the project's Docker image to a container on any linux environment (Azure Web App for Containers recommended).
	Please install and get a basic understanding of Docker before undertaking any of this. I guarantee you will confuse yourself if you don't!
</sub>

## BUGS:
- Please create an issue or pull request if you find any

## FEATURES TO ADD:	
- edit customer name freely for redeemed certificate
- rework frontend in Vue.js
