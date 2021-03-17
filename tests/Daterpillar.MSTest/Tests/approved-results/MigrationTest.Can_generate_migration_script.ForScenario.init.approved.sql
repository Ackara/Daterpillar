-- Creating the artist table

CREATE TABLE [artist] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[Bio] VARCHAR(255) NOT NULL,
	[DOB] DATETIME NOT NULL
)
;

CREATE INDEX IF NOT EXISTS [artist__Name_index] ON [artist] ([Name] ASC);

-- End --

CREATE TABLE [Album] (
	[Name] VARCHAR(255) NOT NULL,
	[ArtistId] INTEGER NOT NULL,
	[SongId] INTEGER NOT NULL,
	[Year] INTEGER NOT NULL,
	PRIMARY KEY ([ArtistId] ASC, [SongId] ASC),
	FOREIGN KEY ([ArtistId]) REFERENCES [artist]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT,
	FOREIGN KEY ([SongId]) REFERENCES [song]([Id]) ON UPDATE RESTRICT ON DELETE RESTRICT
)
;

-- Creating the Extra table

CREATE TABLE [Extra] (
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(255) NOT NULL,
	[PlanId] VARCHAR(255) NOT NULL,
	[OwnerId] VARCHAR(255) NOT NULL,
	PRIMARY KEY ([Id] ASC)
)
;

CREATE INDEX IF NOT EXISTS [Extra__PlanId_index] ON [Extra] ([PlanId] ASC);

CREATE INDEX IF NOT EXISTS [Extra__OwnerId_index] ON [Extra] ([OwnerId] ASC);

-- End --

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
	[Title] VARCHAR(255) NOT NULL,
	[Length] INTEGER NOT NULL,
	[Artists] VARCHAR(255) NOT NULL,
	[Album] VARCHAR(255) NOT NULL,
	[Track] INTEGER NOT NULL DEFAULT 1,
	[Disc] INTEGER NOT NULL DEFAULT 1,
	[Genre] INTEGER NOT NULL,
	FOREIGN KEY ([Genre]) REFERENCES [genre]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
)
;

CREATE INDEX IF NOT EXISTS [song__Genre_index] ON [song] ([Genre] ASC);

-- End --

INSERT INTO genre (Id, Name) VALUES 
('0', 'Hip Hop'),
('1', 'Pop'),
('2', 'Country'),
('3', 'Rock n Roll');

