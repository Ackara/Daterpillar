file: V1.1__alter_schema.sql


-- Before table works.

-- Creating the publisher table

CREATE TABLE [publisher] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[logo] BLOB
)
;

INSERT INTO publisher (Name) VALUES ('OVO'), ('UMG');

-- End --

ALTER TABLE [song] RENAME TO [track];

-- Modifying the [track] table.

PRAGMA foreign_keys=off;
BEGIN TRANSACTION;
CREATE TABLE [_track_old] AS SELECT * FROM [track];
DROP TABLE [track];
CREATE TABLE [track] (
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(255) NOT NULL,
	[Duration] INTEGER NOT NULL,
	[Disc] INTEGER NOT NULL DEFAULT 1,
	[Track] INTEGER NOT NULL,
	[Artist] VARCHAR(255) NOT NULL,
	[Album] VARCHAR(255) NOT NULL,
	[PublisherId] INTEGER NOT NULL DEFAULT 1,
	FOREIGN KEY ([PublisherId]) REFERENCES [publisher]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
)
;

CREATE INDEX IF NOT EXISTS [track__Name_index] ON [track] ([Name] ASC);

CREATE INDEX IF NOT EXISTS [track__PublisherId_index] ON [track] ([PublisherId] ASC);

INSERT INTO [track] ([Id], [Name], [Duration], [Disc], [Track], [Artist], [Album]) SELECT [Id], [Name], [Length], [Disc], [Track], [Artist], [Album] FROM [_track_old];
DROP TABLE [_track_old];
COMMIT;
PRAGMA foreign_keys=on;

-- End --

-- Modifying the [artist] table.

PRAGMA foreign_keys=off;
BEGIN TRANSACTION;
CREATE TABLE [_artist_old] AS SELECT * FROM [artist];
DROP TABLE [artist];
CREATE TABLE [artist] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(32) NOT NULL,
	[Bio] VARCHAR(512) DEFAULT '',
	[DOB] DATETIME NOT NULL
)
;

INSERT INTO [artist] ([Id], [Name], [Bio], [DOB]) SELECT [Id], [Name], [Bio], [DOB] FROM [_artist_old];
DROP TABLE [_artist_old];
COMMIT;
PRAGMA foreign_keys=on;

-- End --

DROP TABLE [genre];

CREATE TABLE [album] (
	[Name] VARCHAR(255) NOT NULL,
	[Year] INTEGER NOT NULL,
	[ArtistId] INTEGER NOT NULL,
	[SongId] INTEGER NOT NULL,
	PRIMARY KEY ([SongId] ASC, [ArtistId] ASC),
	FOREIGN KEY ([SongId]) REFERENCES [track]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT,
	FOREIGN KEY ([ArtistId]) REFERENCES [artist]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
)
;


    -- CHECKS:
    -- * Create [Ablum] table.
    -- * Create [song].[PublisherId] column, index and foreign-key.
    -- * Drop [Genre] table.
    -- * Drop [song].[Genre] column, index and foreign-key.
    -- * Rename [song] to track.
    -- * Rename [song].[Length] column.
    -- * Alter [artist].[Bio] column.
    -- * Syntax specific script was added.
    -- * Toggle [artist] and [song] auto-increment.
    

-- If you're reading this, the syntax is SQLite.

