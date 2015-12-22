/*
 * SCHEMA:		example inc.
 * VERSION:		1.0.0.0
 * AUTHOR:		john@example.com
 * GENERATED:	Dec 22, 2015
*/

-- -----------------------------------
-- EMPLOYEE TABLE
-- -----------------------------------
CREATE TABLE IF NOT EXISTS [employee]
(
	[Id] INTEGER not null,
	[birth_date] DATE not null,
	[first_name] VARCHAR(14) not null,
	[last_name] VARCHAR(16) not null,
	[gender] INTEGER not null,
	[hire_date] DATE not null,
	PRIMARY KEY ([Id] ASC)
);


-- -----------------------------------
-- SALARY TABLE
-- -----------------------------------
CREATE TABLE IF NOT EXISTS [salary]
(
	[Employee_Id] INTEGER not null,
	[amount] DECIMAL(12, 2) not null default 0,
	[from_date] DATE not null,
	[to_date] DATE not null,
	PRIMARY KEY ([from_date] ASC, [Employee_Id] ASC),
	FOREIGN KEY ([Employee_Id]) REFERENCES [employee] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE
);
