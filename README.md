# DNT Console Template

<p align="left">
  <a href="https://github.com/VahidN/DntConsole">
     <img alt="GitHub Actions status" src="https://github.com/VahidN/DntConsole/workflows/.NET%20Core%20Build/badge.svg">
  </a>
</p>

This a .NET 7x console app template with the following configured features:

- Central package management
- Roslyn based analyzers
- Dependency injection
- Config files using the IOptions pattern
- Logging system
- SQLite DB and scoped EF-Core access
- Command line parser
- Typed HttpClient
- Integration tests using an in-memory SQLite DB

To install it as a new project template, first clone this repository and then run the `_register_template.cmd` file. 
Finally create a new folder and run the `dotnet new dntconsole` command in it to create a new dnt-console app!