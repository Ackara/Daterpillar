SYNTAX: TSQL


CREATE TABLE [genre] (
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] VARCHAR(255) NOT NULL
)
;

CREATE TABLE [song] (
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] VARCHAR(15) NOT NULL,
	[GenreId] INT NOT NULL,
	[Track] TINYINT NOT NULL DEFAULT 1,
	[Lyrics] VARCHAR(255),
	CONSTRAINT [song_GenreId_TO_genre_Id__fk] FOREIGN KEY ([GenreId]) REFERENCES [genre]([Id]) ON UPDATE CASCADE ON DELETE NO ACTION
)
;

CREATE INDEX [song__GenreId_index] ON [song] ([GenreId] ASC);

CREATE TABLE [album] (
	[SongId] INT NOT NULL,
	[ArtistId] INT NOT NULL,
	[Name] VARCHAR(255) NOT NULL,
	[Year] SMALLINT NOT NULL,
	[Price] DECIMAL(8, 2) NOT NULL
)
;

-- If you are reading this, I don't discriminate.

INSERT INTO genre (Name) VAlUES ('Hip Hop'), ('R&B'), ('Country');

INSERT INTO song (Name, GenreId, Track) VALUES ('Survival', '1', '1');

INSERT INTO album (SongId, ArtistId, Name, Year, Price) VALUES ('1', '1', 'Scorpion', '2018', '14.99');

ALTER TABLE [song] ADD [ReleaseDate] DATETIME NOT NULL DEFAULT GETDATE();

ALTER TABLE [album] ADD CONSTRAINT [album_SongId_TO_song_Id__fk] FOREIGN KEY ([SongId]) REFERENCES [song]([Id])  ON UPDATE CASCADE ON DELETE NO ACTION;

CREATE UNIQUE INDEX [song__Name_index] ON [song] ([Name] DESC);

CREATE INDEX [album__SongId_and_ArtistId_index] ON [album] ([SongId] ASC, [ArtistId] ASC);

