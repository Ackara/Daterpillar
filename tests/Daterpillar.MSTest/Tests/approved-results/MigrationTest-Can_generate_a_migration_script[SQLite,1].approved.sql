file: V1.0__init.SQLite.sql


-- Creating the genre table.
-- --------------------------------------------------

CREATE TABLE [genre] (
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(255) NOT NULL
);

CREATE UNIQUE INDEX [genre__Name_index] ON [genre] ([Name] ASC);

-- Creating the song table.
-- --------------------------------------------------

CREATE TABLE [song] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[Length] INTEGER NOT NULL,
	[Genre] INTEGER NOT NULL,
	[Disc] INTEGER NOT NULL  DEFAULT 1,
	[Track] INTEGER NOT NULL,
	[Artist] VARCHAR(255) NOT NULL,
	[Album] VARCHAR(255) NOT NULL,
	FOREIGN KEY ([Genre]) REFERENCES [genre]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
);

CREATE INDEX [song__Name_index] ON [song] ([Name] ASC);

CREATE INDEX [song__Genre_index] ON [song] ([Genre] ASC);

-- Creating the artist table.
-- --------------------------------------------------

CREATE TABLE [artist] (
	[Name] VARCHAR(32) NOT NULL,
	[Bio] TEXT NOT NULL  DEFAULT '',
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[DOB] DATETIME NOT NULL
);

-- Creating the album table.
-- --------------------------------------------------

CREATE TABLE [album] (
	[Name] VARCHAR(255) NOT NULL,
	[Year] INTEGER NOT NULL,
	[ArtistId] INTEGER NOT NULL,
	[SongId] INTEGER NOT NULL,
	PRIMARY KEY ([SongId] ASC, [ArtistId] ASC),
	FOREIGN KEY ([SongId]) REFERENCES [song]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT,
	FOREIGN KEY ([ArtistId]) REFERENCES [artist]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
);


-- Seed-Data
-- -----------------------------------------------
INSERT INTO genre (Id, Name)
VALUES ('1', 'Hip Hop'), ('2', 'Rock'), ('3', 'Pop'), ('4', 'R&B');



-- Seed-Data
-- -----------------------------------------------

INSERT INTO song (Id, Name, Artist, Album, Genre, Track, Length)
VALUES
(101, 'Survival', 'Drake', 'Scorpion', '1', '1', '136')
;

INSERT INTO artist (Id, Name, Bio, DOB)
VALUES
('1', 'Drake', 'Canadian-black-jewish-british-jamican-afro-latina rapper from the 6.', '1987-10-04')
;
    

