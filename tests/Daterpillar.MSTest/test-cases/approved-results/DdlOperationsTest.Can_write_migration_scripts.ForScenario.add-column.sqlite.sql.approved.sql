-- Modifying the [car] table.

PRAGMA foreign_keys=off;
BEGIN TRANSACTION;
CREATE TABLE [_car_old] AS SELECT * FROM [car];
DROP TABLE [car];
CREATE TABLE [car] (
	[id] VARCHAR(255) NOT NULL,
	[make] VARCHAR(255) NOT NULL,
	[model] VARCHAR(255) NOT NULL,
	[year] INTEGER NOT NULL,
	PRIMARY KEY ([id] ASC)
)
;

INSERT INTO [car] ([id], [make], [model]) SELECT [id], [make], [model] FROM [_car_old];
DROP TABLE [_car_old];
COMMIT;
PRAGMA foreign_keys=on;

-- End --

