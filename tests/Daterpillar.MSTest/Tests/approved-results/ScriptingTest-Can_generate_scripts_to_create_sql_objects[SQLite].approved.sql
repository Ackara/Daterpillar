SYNTAX: SQLite


CREATE TABLE [genre] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL
);

CREATE TABLE [song] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(15) NOT NULL,
	[GenreId] INTEGER NOT NULL,
	[Track] INTEGER NOT NULL  DEFAULT 1,
	[Lyrics] VARCHAR(255),
	FOREIGN KEY ([GenreId]) REFERENCES [genre]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
);

CREATE TABLE [album] (
	[SongId] INTEGER NOT NULL,
	[ArtistId] INTEGER NOT NULL,
	[Name] VARCHAR(255) NOT NULL,
	[Year] INTEGER NOT NULL
);

INSERT INTO genre (Name) VAlUES ('Hip Hop'), ('R&B'), ('Country');

-- If you are reading this, I don't discriminate.

ALTER TABLE [song] ADD COLUMN [ReleaseDate] TEXT NOT NULL  DEFAULT '';

PRAGMA foreign_keys=off;
BEGIN TRANSACTION;
ALTER TABLE [album] RENAME TO [_album_old];
CREATE TABLE [album] (
	[SongId] INTEGER NOT NULL,
	[ArtistId] INTEGER NOT NULL,
	[Name] VARCHAR(255) NOT NULL,
	[Year] INTEGER NOT NULL,
	FOREIGN KEY ([SongId]) REFERENCES [song]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
);

INSERT INTO [album] SELECT * FROM [_album_old];
DROP TABLE [_album_old];
COMMIT;
PRAGMA foreign_keys=on;

CREATE INDEX [idx_song_Name] ON [song] ([Name] DESC);

PRAGMA foreign_keys=off;
BEGIN TRANSACTION;
ALTER TABLE [album] RENAME TO [_album_old];
CREATE TABLE [album] (
	[SongId] INTEGER NOT NULL,
	[ArtistId] INTEGER NOT NULL,
	[Name] VARCHAR(255) NOT NULL,
	[Year] INTEGER NOT NULL,
	PRIMARY KEY ([SongId] ASC, [ArtistId] ASC),
	FOREIGN KEY ([SongId]) REFERENCES [song]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
);

INSERT INTO [album] SELECT * FROM [_album_old];
DROP TABLE [_album_old];
COMMIT;
PRAGMA foreign_keys=on;

