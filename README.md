# Daterpillar 

[![nuget](https://img.shields.io/nuget/v/Acklann.Daterpillar.svg?maxAge=2592000?style=flat-square)](https://www.nuget.org/packages/Acklann.Daterpillar)
[![powershell](https://img.shields.io/powershellgallery/v/daterpillar.svg?style=flat)](https://www.powershellgallery.com/packages/Daterpillar)

## The Problem
Your *.NET* project depends on a SQL database to store it's data, therefore you need a way to keep the tables in-sync with it's codebase counterparts/entities.

## The Solution
This library keeps your entities and tables in-sync by generating **sql-migration-scripts** from your project's `.dll` files. Let's say you have the following class in your project.

```csharp
[Table()]
public class User
{
    [Key(), Column(AutoIncrement = true)]
    public string Id { get; set; }

    [Column("full_name", scale: 32)]
    [StaticId("ec35bf5d-9a40-4e6f-8412-852b81807a09")]
    public string Name { get; set; }

    public DateTime DOB { get; set; }
}
```

As you can see the `User` class is decorated with a `Table` and `Column` attribues. Now when you build your project, the *NUGET* package ships with a *MSBuild* target called `GenerateMigrationScript` that will run after the project has been built and produce `.sql` script that should update your database schema, but first you will need to configure it.

Open your `.*proj` project-file and add the following elements inside a `<PropertyGroup>` element.

```xml
<!-- This turn on the feature -->
<GenerateMigrationScriptAfterBuild>true</GenerateMigrationScriptAfterBuild>

<!-- This is where the script will be saved. -->
<DaterpillarMigrationsDirectory>Migrations</DaterpillarMigrationsDirectory>

<!-- The script's language. -->
<DaterpillarLanguage>MySQL</<DaterpillarLanguage>

<!-- This file represents the current state/structure of your live database. (Optional defaulats to 'snapshot.schema.xml') -->
<DaterpillarSnapshot>snapshot.schema.xml</DaterpillarSnapshot>
```

Once configured, everytime the project is built a `.sql` script will be created. 

example: `V1.0.2__create_schema.mysql.sql`
```sql
CREATE TABLE `user`(
    `Id` VARCHAR(64) NOT NULL PRIMARY KEY AUTO_INCREMENT,
    `full_name` VARCHAR(32) NOT NULL,
    `DOB` DATETIME NOT NULL
);
```

Now that you have a migration-script you might need to run it. You can run the script manually, use another tool like [FlywayDB](https://flywaydb.org/), Powershell, or use the *MSBuild* target called `InvokeMigrationScripts`; however before you do, you will have to configure it.

Open your `.*proj` project-file and add the following elements inside a `<PropertyGroup>` element.

```xml
<!-- This is the relative-path to a .json file that contains the target database connection-string -->
<ConnectionStringFilePath>appsettings.json</ConnectionStringFilePath>

<!-- This the jpath to locate the connection-string within the .json file -->
<!-- The expected connection-string format is server=[your_address];user=[you_username];password=[your_password];database=[your_dbname] -->
<ConnectionStringJPath>/localdb/connectionString</ConnectionStringJPath>
```

The `InvokeMigrationScripts` target will have to be invoked manually either via the *dotnet-cli* or *MSBuild*. example:

`> dotnet msbuild -target:InvokeMigrationScripts`

If using *MSBuild* targets is not your preference or maybe you just want more control, try the [Powershell Module](https://www.powershellgallery.com/packages/Daterpillar) instead.

### Advance Usage

To generated the `.sql` migration-script mentioned above. The difference between `snapshot.schema.xml` and `[your-assembly].schema.xml` is used to generate the script. So where did this `[your-assembly].schema.xml` file came from? The *MSBuild* target `ExportDaterpillarSchema`. You can extend schema by merging multiple schemas together.

Lets say you want to add seed-data to a table; first you will have to add a `[assembly: Import("seed.schema.xml")]` attribute to your project. 

example: `./Properties/AssemblyInfo.cs`

```csharp
...
[assembly: Acklann.Daterpillar.Import("seed.schema.xml")]
// or
// [assembly: Acklann.Daterpillar.Import("*.schema.xml")]
...
```

Next, you can add the `seed.schema.xml` to your project.

```xml
<schema xmlns="https://raw.githubusercontent.com/Ackara/Daterpillar/master/src/daterpillar.xsd">
    <!-- This script will be appended -->
    <script name="seed" language="Generic">
    INSERT INTO User (full_name, DOB) VALUES ('Petra Ral', '2000-11-15');
    </script>
</schema>
```

**NOTE:** If the `language` attribute was set to 'MySQL' instead, the script would of only been included when MySQL explictly targeted.

You can also override or add columns by redefining a table.

```xml
<schema xmlns="https://raw.githubusercontent.com/Ackara/Daterpillar/master/src/daterpillar.xsd">
    <table name="User">
        <!-- This will rename the existing 'full_name' column -->
        <column id="ec35bf5d-9a40-4e6f-8412-852b81807a09" name="first_name">
            <dataType scale="64">varchar</dataType>
        </column>

        <!-- This will add a new column -->
        <column name="last_name">
            <dataType scale="64">varchar</dataType>
        </column>
    </table>
</schema>
```
