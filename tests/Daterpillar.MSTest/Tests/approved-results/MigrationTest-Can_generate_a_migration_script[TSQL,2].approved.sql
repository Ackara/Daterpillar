file: V1.1__Update.tsql.sql


-- Before table works.

-- Creating the publisher table

CREATE TABLE [publisher] (
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] VARCHAR(255) NOT NULL,
	[logo] VARBINARY(255)
)
;

INSERT INTO publisher (Name) VALUES ('OVO'), ('UMG');

-- End --

EXEC sp_RENAME 'song', 'track'
GO

-- Modifying the track table

ALTER TABLE [track] ADD [PublisherId] INT NOT NULL DEFAULT 1;

--      Modifying [track].[Id]

ALTER TABLE [track] ALTER COLUMN [Id] INT NOT NULL;

--      End      --

EXEC sp_RENAME 'track.Length', 'Duration', 'COLUMN'
GO

ALTER TABLE [track] DROP CONSTRAINT [song_Genre_TO_genre_Id__fk];

ALTER TABLE [track] ADD CONSTRAINT [track_PublisherId_TO_publisher_Id__fk] FOREIGN KEY ([PublisherId]) REFERENCES [publisher]([Id])  ON UPDATE CASCADE ON DELETE NO ACTION;

DROP INDEX [track].[song__Name_index];

DROP INDEX [track].[song__Genre_index];

ALTER TABLE [track] DROP COLUMN [Genre];

CREATE INDEX [track__Name_index] ON [track] ([Name] ASC);

CREATE INDEX [track__PublisherId_index] ON [track] ([PublisherId] ASC);

-- End --

-- Modifying the artist table

--      Modifying [artist].[Bio]

ALTER TABLE [artist] ALTER COLUMN [Bio] VARCHAR(512);

ALTER TABLE [artist] ADD DEFAULT '' FOR [Bio];

--      End      --

ALTER TABLE [artist] ALTER COLUMN [Id] INT NOT NULL;


-- End --

DROP TABLE [genre];

CREATE TABLE [album] (
	[Name] VARCHAR(255) NOT NULL,
	[Year] INT NOT NULL,
	[ArtistId] INT NOT NULL,
	[SongId] INT NOT NULL,
	PRIMARY KEY ([SongId] ASC, [ArtistId] ASC),
	CONSTRAINT [album_SongId_TO_track_Id__fk] FOREIGN KEY ([SongId]) REFERENCES [track]([Id]) ON UPDATE CASCADE ON DELETE NO ACTION,
	CONSTRAINT [album_ArtistId_TO_artist_Id__fk] FOREIGN KEY ([ArtistId]) REFERENCES [artist]([Id]) ON UPDATE CASCADE ON DELETE NO ACTION
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
    

-- If you're reading this, the syntax is TSQL.

