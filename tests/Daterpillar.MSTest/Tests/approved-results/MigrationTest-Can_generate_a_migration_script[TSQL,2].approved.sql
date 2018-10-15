file: V1.1__Update.tsql.sql


-- Creating the label table

CREATE TABLE [label] (
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] VARCHAR(255) NOT NULL,
	[TotalArtist] TINYINT NOT NULL  DEFAULT 0
);

-- End --

-- Modifying the song table

EXEC sp_RENAME 'song', 'track'
GO

ALTER TABLE [track] ADD [Synced] BIT NOT NULL  DEFAULT 0;

ALTER TABLE [track] DROP CONSTRAINT [song_Genre_TO_genre_Id__fk];

DROP INDEX [track].[song__Name_index];

DROP INDEX [track].[song__Genre_index];

EXEC sp_RENAME 'track.Length', 'Duration', 'COLUMN'
GO

-- Modifying [track].[Duration]

ALTER TABLE [track] ALTER COLUMN [Duration] INT NOT NULL;

ALTER TABLE [track] ADD DEFAULT 0 FOR [Duration];

-- End --

CREATE INDEX [track__Name_index] ON [track] ([Name] ASC);

-- End --

-- Modifying the album table

ALTER TABLE [album] DROP CONSTRAINT [album_SongId_TO_song_Id__fk];

ALTER TABLE [album] ADD CONSTRAINT [album_SongId_TO_track_Id__fk] FOREIGN KEY ([SongId]) REFERENCES [track]([Id])  ON UPDATE CASCADE ON DELETE NO ACTION;

-- End --

DROP TABLE [genre];


-- Seed-Data
-- -----------------------------------------------
INSERT INTO label (Name)
VALUES ('Independent'), ('YMCMB'), ('OVO'), ('UMG'), ('Atlantic Records');
;
    

