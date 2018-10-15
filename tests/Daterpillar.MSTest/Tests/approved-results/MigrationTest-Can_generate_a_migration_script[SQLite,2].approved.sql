file: V1.1__Update.sqlite.sql


-- Creating the label table.
-- --------------------------------------------------

CREATE TABLE [label] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[TotalArtist] INTEGER NOT NULL  DEFAULT 0
);

-- Modifying the song table.
-- --------------------------------------------------

ALTER TABLE [song] RENAME TO [track];

ALTER TABLE [track] ADD COLUMN [Synced] BOOLEAN NOT NULL  DEFAULT 0;

-- Removing [track_Genre_TO_genre_Id__fk]

BEGIN TRANSACTION;
CREATE TABLE [_track_old] AS SELECT * FROM [track];
DROP TABLE [track];
CREATE TABLE [track] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[Length] INTEGER NOT NULL,
	[Genre] INTEGER NOT NULL,
	[Disc] INTEGER NOT NULL  DEFAULT 1,
	[Track] INTEGER NOT NULL,
	[Artist] VARCHAR(255) NOT NULL,
	[Album] VARCHAR(255) NOT NULL,
	[Synced] BOOLEAN NOT NULL  DEFAULT 0
);

CREATE INDEX [track__Name_index] ON [track] ([Name] ASC);

CREATE INDEX [track__Genre_index] ON [track] ([Genre] ASC);

INSERT INTO [track] SELECT * FROM [_track_old];
DROP TABLE [_track_old];
COMMIT;

-- End --

DROP INDEX [track__Name_index];

DROP INDEX [track__Genre_index];

-- Renaming [track].[Length] to Duration

BEGIN TRANSACTION;
CREATE TABLE [_track_old] AS SELECT * FROM [track];
DROP TABLE [track];
CREATE TABLE [track] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[Duration] INTEGER NOT NULL,
	[Genre] INTEGER NOT NULL,
	[Disc] INTEGER NOT NULL  DEFAULT 1,
	[Track] INTEGER NOT NULL,
	[Artist] VARCHAR(255) NOT NULL,
	[Album] VARCHAR(255) NOT NULL,
	[Synced] BOOLEAN NOT NULL  DEFAULT 0,
	FOREIGN KEY ([Genre]) REFERENCES [genre]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
);

CREATE INDEX [track__Name_index] ON [track] ([Name] ASC);

CREATE INDEX [track__Genre_index] ON [track] ([Genre] ASC);

INSERT INTO [track] SELECT * FROM [_track_old];
DROP TABLE [_track_old];
COMMIT;

-- End --

-- Modifying [track].[Duration]

BEGIN TRANSACTION;
CREATE TABLE [_track_old] AS SELECT * FROM [track];
DROP TABLE [track];
CREATE TABLE [track] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[Duration] INTEGER NOT NULL  DEFAULT 0,
	[Genre] INTEGER NOT NULL,
	[Track] INTEGER NOT NULL,
	[Synced] BOOLEAN NOT NULL  DEFAULT 0,
	[Artist] VARCHAR(255) NOT NULL,
	[Album] VARCHAR(255) NOT NULL,
	[Disc] INTEGER NOT NULL  DEFAULT 1
);

INSERT INTO [track] SELECT * FROM [_track_old];
DROP TABLE [_track_old];
COMMIT;

-- End --

CREATE INDEX [track__Name_index] ON [track] ([Name] ASC);

-- Modifying the album table.
-- --------------------------------------------------

-- Removing [album_SongId_TO_track_Id__fk]

BEGIN TRANSACTION;
CREATE TABLE [_album_old] AS SELECT * FROM [album];
DROP TABLE [album];
CREATE TABLE [album] (
	[Name] VARCHAR(255) NOT NULL,
	[Year] INTEGER NOT NULL,
	[ArtistId] INTEGER NOT NULL,
	[SongId] INTEGER NOT NULL,
	PRIMARY KEY ([SongId] ASC, [ArtistId] ASC),
	FOREIGN KEY ([ArtistId]) REFERENCES [artist]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
);

INSERT INTO [album] SELECT * FROM [_album_old];
DROP TABLE [_album_old];
COMMIT;

-- End --

-- Creating album_SongId_TO_track_Id__fk

BEGIN TRANSACTION;
CREATE TABLE [_album_old] AS SELECT * FROM [album];
DROP TABLE [album];
CREATE TABLE [album] (
	[Name] VARCHAR(255) NOT NULL,
	[Year] INTEGER NOT NULL,
	[ArtistId] INTEGER NOT NULL,
	[SongId] INTEGER NOT NULL,
	PRIMARY KEY ([SongId] ASC, [ArtistId] ASC),
	FOREIGN KEY ([ArtistId]) REFERENCES [artist]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT,
	FOREIGN KEY ([SongId]) REFERENCES [track]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
);

INSERT INTO [album] SELECT * FROM [_album_old];
DROP TABLE [_album_old];
COMMIT;

-- End --

DROP TABLE [genre];


-- Seed-Data
-- -----------------------------------------------
INSERT INTO label (Id, Name)
VALUES ('0', 'Independent'), ('1', 'YMCMB'), ('2', 'OVO'), ('3', 'UMG'), ('4', 'Atlantic Records');
;
    

