/*
 * SCHEMA:		example inc.
 * VERSION:		1.0.0.0
 * AUTHOR:		john@example.com
 * GENERATED:	January 02, 2016
*/

-- -----------------------------------
-- TABLEA TABLE
-- -----------------------------------
DROP TABLE IF EXISTS [tableA];
CREATE TABLE IF NOT EXISTS [tableA]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(64) 
);


-- -----------------------------------
-- TABLEB TABLE
-- -----------------------------------
DROP TABLE IF EXISTS [tableB];
CREATE TABLE IF NOT EXISTS [tableB]
(
	[Id] INTEGER ,
	[Date] DATE ,
	[TableA_Id] INTEGER ,
	[Age] INTEGER NOT NULL,
	PRIMARY KEY ([Id] ASC),
	FOREIGN KEY ([TableA_Id]) REFERENCES [tableA] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS age_idx ON [tableB] ([Age] ASC);
