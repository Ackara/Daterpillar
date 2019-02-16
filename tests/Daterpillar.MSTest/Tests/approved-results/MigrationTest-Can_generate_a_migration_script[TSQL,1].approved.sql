file: V1.0__init.TSQL.sql


-- Creating the genre table

CREATE TABLE [genre] (
	[Id] INT NOT NULL,
	[Name] VARCHAR(255) NOT NULL,
	PRIMARY KEY ([Id] ASC)
)
;

CREATE UNIQUE INDEX [genre__Name_index] ON [genre] ([Name] ASC);

-- End --

-- Creating the song table

CREATE TABLE [song] (
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] VARCHAR(255) NOT NULL,
	[Length] INT NOT NULL,
	[Genre] INT NOT NULL,
	[Disc] INT NOT NULL DEFAULT 1,
	[Track] SMALLINT NOT NULL,
	[Artist] VARCHAR(255) NOT NULL,
	[Album] VARCHAR(255) NOT NULL,
	CONSTRAINT [song_Genre_TO_genre_Id__fk] FOREIGN KEY ([Genre]) REFERENCES [genre]([Id]) ON UPDATE CASCADE ON DELETE NO ACTION
)
;

CREATE INDEX [song__Name_index] ON [song] ([Name] ASC);

CREATE INDEX [song__Genre_index] ON [song] ([Genre] ASC);

-- End --

CREATE TABLE [artist] (
	[Name] VARCHAR(32) NOT NULL,
	[Bio] TEXT NOT NULL,
	[Id] INT NOT NULL,
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
    

