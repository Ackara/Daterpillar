SYNTAX: SQLite


ALTER TABLE [placeholder] RENAME TO [publisher];

-- Modifying the [service] table.

PRAGMA foreign_keys=off;
BEGIN TRANSACTION;
CREATE TABLE [_service_old] AS SELECT * FROM [service];
DROP TABLE [service];
CREATE TABLE [service] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[Subscribers] INTEGER NOT NULL DEFAULT 0,
	[Zombie] VARCHAR(255) NOT NULL,
	[ActiveUsers] INTEGER NOT NULL,
	FOREIGN KEY ([ActiveUsers]) REFERENCES [placeholder]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
)
;

CREATE INDEX IF NOT EXISTS [service__Subscribers_index] ON [service] ([Subscribers] ASC);

INSERT INTO [service] ([Id], [Name], [Subscribers], [Zombie]) SELECT [Id], [Name], [Subscribers], [Zombie] FROM [_service_old];
DROP TABLE [_service_old];
COMMIT;
PRAGMA foreign_keys=on;

-- End --

