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

CREATE UNIQUE INDEX IF NOT EXISTS [song__Name_index] ON [song] ([Name] DESC);

CREATE INDEX IF NOT EXISTS [album__SongId_and_ArtistId_index] ON [album] ([SongId] ASC, [ArtistId] ASC);

