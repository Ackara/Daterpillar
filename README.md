[![build](https://gigobyte.visualstudio.com/DefaultCollection/_apis/public/build/definitions/c1607574-4e07-41e8-954f-f983147fe67d/5/badge)](https://gigobyte.visualstudio.com/DefaultCollection/_apis/public/build/definitions/c1607574-4e07-41e8-954f-f983147fe67d/5/badge)
[![version](https://img.shields.io/nuget/v/Gigobyte.Daterpillar.Core.svg?style=flat-square)](https://www.nuget.org/packages?q=Gigobyte.Daterpillar.Core)
[![downloads](https://img.shields.io/nuget/dt/Gigobyte.Daterpillar.Core.svg)](https://img.shields.io/nuget/dt/Gigobyte.Daterpillar.Core.svg)
# Daterpillar
Daterpillar allows you to write your database schema in XML then transform that document into MySQL, SQLite or even C# classes.

## How It Works
Let say you have the following XML document

```xml
<schema xmlns="http://schema.gigobyte.com/v1/xsml.xsd">
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
The easiest way to get started is by installing the NuGet package.
[Install-Package Gigobyte.Daterpillar.Core](https://www.nuget.org/packages/Gigobyte.Daterpillar.Core).

## License
[MIT License](https://github.com/Ackara/Daterpillar/blob/master/LICENSE)