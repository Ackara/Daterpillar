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
The easiest way to get started is by installing the NuGet package with the following [Install-Package Gigobyte.Daterpillar.Core](https://www.nuget.org/packages/Gigobyte.Daterpillar.Core). 



Lately I have ran into the scenario whereby I need to maintain two SQL schemata which are structurally identical, but maintained on two different SQL platforms (MySQL & SQLite). Why would you use two? Well if you're paying for a relational database that lives in the cloud like me, you might want to cache the results on the client locally if possible to save some cash, and that is where the SQLite comes in.

Think of daterpillar as a cross-platform database framework, sort of like Phone Gap and Xamarin, you write once deploy everywhere. With that said you create your schema using XML then daterpillar will transform it into MySQL, SQLite or even C#. Yes 


Daterpillar is a library coupled with some handy tools that helps you with the following:
* Generate C# s