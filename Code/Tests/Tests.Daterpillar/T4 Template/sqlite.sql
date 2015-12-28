/*
 * SCHEMA:		example inc.
 * VERSION:		1.0.0.0
 * AUTHOR:		john@example.com
 * GENERATED:	Dec 28, 2015
*/

-- -----------------------------------
-- GENRE TABLE
-- -----------------------------------
CREATE TABLE IF NOT EXISTS [genre]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(64) 
);


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
-- ALBUM TABLE
-- -----------------------------------
CREATE TABLE IF NOT EXISTS [album]
(
	[Artist_Id] INTEGER NOT NULL,
	[Name] VARCHAR(64) NOT NULL,
	[Release_Date] DATE ,
	PRIMARY KEY ([Artist_Id] ASC, [Name] ASC),
	FOREIGN KEY ([Artist_Id]) REFERENCES [artist] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE
);


-- -----------------------------------
-- SONG TABLE
-- -----------------------------------
CREATE TABLE IF NOT EXISTS [song]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Album_Id] INTEGER NOT NULL,
	[Artist_Id] INTEGER NOT NULL,
	[Genre_Id] INTEGER NOT NULL,
	[Name] VARCHAR(64) NOT NULL,
	[Length] DECIMAL(4, 2) ,
	[Price] DECIMAL(12, 2) NOT NULL,
	[On_Device] BOOLEAN NOT NULL DEFAULT '0',
	FOREIGN KEY ([Genre_Id]) REFERENCES [genre] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY ([Artist_Id]) REFERENCES [artist] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE
);



    /* *** SEED *** */

    INSERT INTO genre (Id, Name) VALUES
    ('1', 'Rap'),
    ('2', 'Pop'),
    ('3', 'Rock');

    INSERT INTO artist (Id, Name, Bio) VALUES
    ('1', 'Drake', 'A canadian rapper');

    INSERT INTO album (Artist_Id, Name, Release_Date) VALUES
    ('1', 'If You''er Reading This It''s Too Late', '2015-02-01');

    INSERT INTO song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) VALUES
    ('1', '1', '1', 'Legend', '4.01', '1.29', '0'),
    ('1', '1', '1', 'Energy', '3.01', '1.29', '1'),
    ('1', '1', '1', '10 Bands', '2.57', '1.29', '1');
