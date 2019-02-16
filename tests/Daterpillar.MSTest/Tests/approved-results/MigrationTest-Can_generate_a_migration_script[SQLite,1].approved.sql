file: V1.0__init.SQLite.sql


-- Creating the genre table

CREATE TABLE [genre] (
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(255) NOT NULL,
	PRIMARY KEY ([Id] ASC)
)
;

CREATE UNIQUE INDEX IF NOT EXISTS [genre__Name_index] ON [genre] ([Name] ASC);

-- End --

-- Creating the song table

CREATE TABLE [song] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[Length] INTEGER NOT NULL,
	[Genre] INTEGER NOT NULL,
	[Disc] INTEGER NOT NULL DEFAULT 1,
	[Track] INTEGER NOT NULL,
	[Artist] VARCHAR(255) NOT NULL,
	[Album] VARCHAR(255) NOT NULL,
	FOREIGN KEY ([Genre]) REFERENCES [genre]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
)
;

CREATE INDEX IF NOT EXISTS [song__Name_index] ON [song] ([Name] ASC);

CREATE INDEX IF NOT EXISTS [song__Genre_index] ON [song] ([Genre] ASC);

-- End --

CREATE TABLE [artist] (
	[Name] VARCHAR(32) NOT NULL,
	[Bio] TEXT NOT NULL,
	[Id] INTEGER NOT NULL,
	[DOB] DATETIME NOT NULL,
	PRIMARY KEY ([Id] ASC)
)
;


-- Seed-Data
-- -----------------------------------------------
INSERT INTO genre (Id, Name)
VALUES (1, 'Hip Hop'), (2, 'Rock'), (3, 'Pop'), (4, 'R&B');


-- If you're reading this it means multiple scrips can be used.


-- Seed-Data
-- -----------------------------------------------

INSERT INTO song (Name, Artist, Album, Genre, Track, Length)
VALUES
('Survival', 'Drake', 'Scorpion', '1', '1', '136')
;

INSERT INTO artist (Id, Name, Bio, DOB)
VALUES
(6, 'Drake', 'canadian-jewish-british-jamican-afro-latina rapper from the 6.', '1987-10-04')
;
    

