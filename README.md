VisualStudio.Data.Sqlite
========================

![build status](https://img.shields.io/github/actions/workflow/status/bricelam/VS.Data.Sqlite/dotnet.yml?main)

A Data Designer Extensibility ([DDEX](https://docs.microsoft.com/previous-versions/visualstudio/visual-studio-2013/bb165128(v=vs.120))) provider for [Microsoft.Data.Sqlite](https://docs.microsoft.com/dotnet/standard/data/sqlite/).

CI builds are available on the [Open VSIX Gallery](https://www.vsixgallery.com/extension/0b471821-68a4-49dd-b175-e6daf4e5cebf). Install [this extension](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.VSIXGallery-nightlybuilds) to get automatic updates.

What it's for
-------------

This extension enables other Visual Studio extensions (like the [EF Core Power Tools](https://github.com/ErikEJ/EFCorePowerTools)) that use the DDEX APIs to do things like the following.

- Create connection strings
- Create design-time connections
- Read database schemas

It also enables connecting to SQLite databases in **Server Explorer** to see their schema and data. This read-only view can be useful when debugging your app.

What it's not for
-----------------

There are plenty of great tools out there to help you design SQLite databases and queries. This is not one of them. The following features are specifically outside the scope of this project.

- Creating databases
- Modifying databases (both schemas and data)
- Designing databases
- Designing queries
  - Reviewing query plans
- Comparing schemas
- Importing and exporting data
- Migrating databases
- Generating code

If you're looking for a great tool that does all of that inside Visual Studio, I recommend the [SQLite Toolbox](https://marketplace.visualstudio.com/items?itemName=ErikEJ.SQLServerCompactSQLiteToolbox).

Screenshots
-----------

![Server Explorer, table data, and Properties toolbox windows](.github/Screenshot4.png)
![Data Source integration](.github/Screenshot1.png)
![Connection dialog](.github/Screenshot2.png)
![Retrieve Data menu item on tables](.github/Screenshot3.png)
