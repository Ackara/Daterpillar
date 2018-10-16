SYNTAX: SQLite


CREATE TABLE [genre] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL
)
;

CREATE TABLE [song] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(15) NOT NULL,
	[GenreId] INTEGER NOT NULL,
	[Track] INTEGER NOT NULL DEFAULT 1,
	[Lyrics] VARCHAR(255),
	FOREIGN KEY ([GenreId]) REFERENCES [genre]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
)
;

CREATE INDEX IF NOT EXISTS [song__GenreId_index] ON [song] ([GenreId] ASC);

CREATE TABLE [album] (
	[SongId] INTEGER NOT NULL,
	[ArtistId] INTEGER NOT NULL,
	[Name] VARCHAR(255) NOT NULL,
	[Year] INTEGER NOT NULL,
	[Price] DECIMAL(8,2) NOT NULL
)
;

-- If you are reading this, I don't discriminate.

INSERT INTO genre (Name) VAlUES ('Hip Hop'), ('R&B'), ('Country');

INSERT INTO song (Name, GenreId, Track) VALUES ('Survival', '1', '1');

INSERT INTO album (SongId, ArtistId, Name, Year, Price) VALUES ('1', '1', 'Scorpion', '2018', '14.99');

ALTER TABLE [song] ADD COLUMN [ReleaseDate] TEXT NOT NULL DEFAULT '';

-- Creating album_SongId_TO_song_Id__fk

BEGIN TRANSACTION;
CREATE TABLE [_album_old] AS SELECT * FROM [album];
DROP TABLE [album];
CREATE TABLE [album] (
	[SongId] INTEGER NOT NULL,
	[ArtistId] INTEGER NOT NULL,
	[Name] VARCHAR(255) NOT NULL,
	[Year] INTEGER NOT NULL,
	[Price] DECIMAL(8,2) NOT NULL,
	FOREIGN KEY ([SongId]) REFERENCES [song]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
)
;

INSERT INTO [album] SELECT * FROM [_album_old];
DROP TABLE [_album_old];
COMMIT;

-- End --

CREATE UNIQUE INDEX IF NOT EXISTS [song__Name_index] ON [song] ([Name] DESC);

-- Creating album Primary-Key

PRAGMA foreign_keys=off;
BEGIN TRANSACTION;
CREATE TABLE [_album_old] AS SELECT * FROM [album];
DROP TABLE [album];
CREATE TABLE [album] (
	[SongId] INTEGER NOT NULL,
	[ArtistId] INTEGER NOT NULL,
	[Name] VARCHAR(255) NOT NULL,
	[Year] INTEGER NOT NULL,
	[Price] DECIMAL(8,2) NOT NULL,
	PRIMARY KEY ([SongId] ASC, [ArtistId] ASC),
	FOREIGN KEY ([SongId]) REFERENCES [song]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
)
;

INSERT INTO [album] SELECT * FROM [_album_old];
DROP TABLE [_album_old];
COMMIT;
PRAGMA foreign_keys=on;

-- End --

