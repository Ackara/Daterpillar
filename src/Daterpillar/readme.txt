Daterpillar is a miro-orm that currently supports SQL Server, MySQL and SQLite. The library helps you to do the following.
* Generate a database schema from classes marked with specific attributes.
* Construct SQL queries.
* Construct SQL commands.

*******************
*      USAGE      *
*******************

1- Generating schemas.
The package ships with a msbuild target that runs before the build target. When executed the target will generate a xml file (<asseblyName>.schema.xml) in the project output directory. You can specify which classes are included in the xml file decorating them will attributes.

example:
[Table]
public class User
{
	
	[Column] public int Id { get; set; }
	[Column] public string Name { get; set; }
}


2- Constructing SQL queries
The Query class contain methods to help create SQL queries. There is also a Query<T> class that uses relfection to build the queries where as the Query class uses plain old string concatenation.

example:
var query = new Query<User>(Syntax.MySQL)
	.SelectAll()
	.Where(x=> x.Name == "john");
or
var query = new Query(Syntax.MySQL)
	.SelectAll()
	.Where("name='john'");
	
output: SELECT * FROM User Where Name='john';

3- Constructing SQL commands
The library contain several extension methods to generate commands from objects.

example:
var user = new User() { Id = 12, Name = "john" };
var insertCmd = user.ToInsertCommand();

output: INSERT INTO User (Id, Name) VALUES ('12', 'john');

var users = new User[] { ... };
var insertCmd = users.ToInsertCommand();

output: output: INSERT INTO User (Id, Name) VALUES ('12', 'john'), ...