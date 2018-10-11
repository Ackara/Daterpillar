SYNTAX: SQLite


DROP TABLE [zombie];

PRAGMA foreign_keys=off;
BEGIN TRANSACTION;
CREATE TABLE [_service_old] AS SELECT * FROM [service];
DROP TABLE [service];
CREATE TABLE [service] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[Subscribers] INTEGER NOT NULL,
	[Zombie] VARCHAR(255) NOT NULL,
	[Zombie_fk] INTEGER NOT NULL
);

CREATE INDEX [service_Subscribers_index] ON [service] ([Subscribers] ASC);

INSERT INTO [service] SELECT * FROM [_service_old];
DROP TABLE [_service_old];
COMMIT;
PRAGMA foreign_keys=on;

PRAGMA foreign_keys=off;
BEGIN TRANSACTION;
CREATE TABLE [_service_new] AS SELECT [Id], [Name], [Subscribers], [Zombie_fk] FROM [service];
DROP TABLE [service];
CREATE TABLE [service] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[Subscribers] INTEGER NOT NULL,
	[Zombie_fk] INTEGER NOT NULL
);

CREATE INDEX [service_Subscribers_index] ON [service] ([Subscribers] ASC);

INSERT INTO [service] SELECT * FROM [_service_new];
DROP TABLE [_service_new];
COMMIT;
PRAGMA foreign_keys=on;

DROP INDEX [service_Subscribers_index];

