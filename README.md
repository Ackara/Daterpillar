# Daterpillar 
![Build Status](https://acklann.visualstudio.com/_apis/public/build/definitions/3f4e6949-e21e-4b02-a69d-067a400f0377/18/badge)

[![license](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000?style=flat-square)](https://github.com/Ackara/Daterpillar/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/Acklann.Daterpillar.svg?maxAge=2592000?style=flat-square)](https://www.nuget.org/packages/Acklann.Daterpillar)
----------

Daterpillar is a micro-orm and DevOps toolset that helps you manage schemas and generate scripts. Currently it supports C#, MSSQL, MySQL and SQLite.

## What can you do?
### Generate scripts.

Let say you have the following class.
```c#
[Table]
public class Employee
{
	[Column(AutoIncrement = true)]
	pulbic int Id { get; set; }
	
	[Column]
	public string Name { get; set; }
}
```
Granted you have installed the package using [NuGet](https://www.nuget.org/packages/Acklann.Daterpillar), a MSBuild target will be included so when you build your project a ***.schema.xml** file will be generated along side your project's .dll file in the bin directory. The file will look something like the following.

```xml
<schema xmlns="http://static.acklann.com/schema/v2/daterpillar.xsd">
  <table name="employee">
	<column name="Id" autoIncrement="true">
	  <dataType>int</dataType>
	</column>

	<column name="Name">
	  <dataType scale="64">varchar</dataType>
	</column>
  </table>
</schema>
```
What you can then do with the file is generate a MySQL script using the powershell module that comes with the package. You can also install from the powerhshell gallery. 

``PS> Install-Module -Name Daterpillar.Automation``

Once imported can use the following to generate a MySQL script.




you will get **MySQL** 

```sql
CREATE TABLE employee
(
	Id INT PRIMARY KEY AUTO_INCREMENT,
	Name VARCHAR(64)
);
```

in **C#**

```csharp
public class Employee
{
	public int Id { get; set; }
	public int Name { get; set; }
}
```

Creating your own template or customizing an existing one is also easy.

## Getting Started
The easiest way to get started is by installing the [NuGet](https://www.nuget.org/packages/Gigobyte.Daterpillar.Core) package. `` Install-Package Gigobyte.Daterpillar.Core ``

## License
Daterpillar is Copyright © 2016 Ackara and other contributors under the [MIT License](https://github.com/Ackara/Daterpillar/blob/master/LICENSE).


