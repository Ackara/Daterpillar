SYNTAX: SQLite


ALTER TABLE [placeholder] RENAME TO [publisher];

-- RENAME: service.Zombie_fk to ActiveUsers

PRAGMA foreign_keys=off;
BEGIN TRANSACTION;
CREATE TABLE [_service_old] AS SELECT * FROM [service];
DROP TABLE [service];
CREATE TABLE [service] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[Subscribers] INTEGER NOT NULL,
	[Zombie] VARCHAR(255) NOT NULL,
	[ActiveUsers] INTEGER NOT NULL,
	FOREIGN KEY ([ActiveUsers]) REFERENCES [placeholder]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
);

CREATE INDEX [service__Subscribers_index] ON [service] ([Subscribers] ASC);

INSERT INTO [service] SELECT * FROM [_service_old];
DROP TABLE [_service_old];
COMMIT;
PRAGMA foreign_keys=on;

-- END RENAME

-- MODIFY: service.Subscribers

PRAGMA foreign_keys=off;
BEGIN TRANSACTION;
CREATE TABLE [_service_old] AS SELECT * FROM [service];
DROP TABLE [service];
CREATE TABLE [service] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(255) NOT NULL,
	[Subscribers] INTEGER NOT NULL  DEFAULT 0,
	[Zombie] VARCHAR(255) NOT NULL,
	[ActiveUsers] INTEGER NOT NULL,
	FOREIGN KEY ([ActiveUsers]) REFERENCES [placeholder]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT
);

CREATE INDEX [service__Subscribers_index] ON [service] ([Subscribers] ASC);

INSERT INTO [service] SELECT * FROM [_service_old];
DROP TABLE [_service_old];
COMMIT;
PRAGMA foreign_keys=on;

-- END MODIFY

