/*
 * SCHEMA:		example inc.
 * VERSION:		1.0.0.0
 * AUTHOR:		john@example.com
 * GENERATED:	Dec 24, 2015
*/

-- -----------------------------------
-- ARTIST TABLE
-- -----------------------------------
CREATE TABLE IF NOT EXISTS [artist]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(64) NOT NULL,
	[Bio] TEXT 
);


-- -----------------------------------
-- GENRE TABLE
-- -----------------------------------
CREATE TABLE IF NOT EXISTS [genre]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(64) 
);


-- -----------------------------------
-- ALBUM TABLE
-- -----------------------------------
CREATE TABLE IF NOT EXISTS [album]
(
	[Artist_Id] INTEGER NOT NULL,
	[Name] VARCHAR(64) NOT NULL,
	PRIMARY KEY ([Artist_Id] ASC, [Name] ASC),
	FOREIGN KEY ([Artist_Id]) REFERENCES [artist] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE
);


-- -----------------------------------
-- SONG TABLE
-- -----------------------------------
CREATE TABLE IF NOT EXISTS [song]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(64) NOT NULL,
	[Length] DECIMAL(4, 2) ,
	[Price] DECIMAL(12, 2) NOT NULL,
	[Album_Id] INTEGER NOT NULL,
	[Artist_Id] INTEGER NOT NULL,
	[Genre_Id] INTEGER NOT NULL,
	FOREIGN KEY ([Genre_Id]) REFERENCES [Genre] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY ([Artist_Id]) REFERENCES [artist] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE
);

INSERT INTO [genre] ([Name]) VALUES ('Rap'), ('Pop'), ('R&B'), ('Rock'), ('Jazz');