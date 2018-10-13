file: V1.0__init.SQLite.sql


-- Creating the genre table.
--------------------------------------------------

CREATE TABLE [genre] (
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(255) NOT NULL
);

CREATE UNIQUE INDEX [genre__Name_index] ON [genre] ([Name] ASC);

-- Creating the song table.
--------------------------------------------------

CREATE TABLE [song] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[Length] INTEGER NOT NULL,
	[Genre] INTEGER NOT NULL,
	[Disc] INTEGER NOT NULL,
	[Track] INTEGER NOT NULL,
	[Artist] VARCHAR(255) NOT NULL,
	[Album] VARCHAR(255) NOT NULL,
	FOREIGN KEY ([Genre]) REFERENCES [genre]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
);

CREATE INDEX [song__Name_index] ON [song] ([Name] ASC);

CREATE INDEX [song__Genre_index] ON [song] ([Genre] ASC);

-- Creating the artist table.
--------------------------------------------------

CREATE TABLE [artist] (
	[Name] VARCHAR(32) NOT NULL,
	[Bio] TEXT NOT NULL  DEFAULT '',
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[DOB] DATETIME NOT NULL
);

-- Creating the album table.
--------------------------------------------------

CREATE TABLE [album] (
	[Name] VARCHAR(255) NOT NULL,
	[Year] INTEGER NOT NULL,
	[ArtistId] INTEGER NOT NULL,
	[SongId] INTEGER NOT NULL,
	PRIMARY KEY ([SongId] ASC, [ArtistId] ASC),
	FOREIGN KEY ([SongId]) REFERENCES [song]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT,
	FOREIGN KEY ([ArtistId]) REFERENCES [artist]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
);

