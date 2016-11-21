# Daterpillar

[![license](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000?style=flat-square)](https://github.com/Ackara/Daterpillar/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/Gigobyte.Daterpillar.Core.svg?maxAge=2592000?style=flat-square)](https://www.nuget.org/packages/Gigobyte.Daterpillar.Core)

|            |**master**|**development**|
|------------|----------|---------------|
|**build status:**|![master](https://acklann.visualstudio.com/_apis/public/build/definitions/3f4e6949-e21e-4b02-a69d-067a400f0377/9/badge)|![development](https://acklann.visualstudio.com/_apis/public/build/definitions/3f4e6949-e21e-4b02-a69d-067a400f0377/3/badge)|

----------

Daterpillar allows you to write your database schema in XML then transform that document into MySQL, SQLite or even C# classes.

## How It Works
Let say you have the following XML document

```xml
<schema xmlns="http://api.gigobyte.com/schema/v1/xddl.xsd">
  <table name="employee">
	<column name="Id" autoIncrement="true">
	  <dataType>int</dataType>
	  <modifier>PRIMARY KEY</modifier>
	</column>

	<column name="Name">
	  <dataType scale="64">varchar</dataType>
	</column>
  </table>
</schema>
```

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


