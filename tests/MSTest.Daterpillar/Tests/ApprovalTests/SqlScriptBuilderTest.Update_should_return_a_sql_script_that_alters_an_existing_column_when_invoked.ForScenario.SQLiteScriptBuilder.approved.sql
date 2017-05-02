PRAGMA foreign_keys = 0;
CREATE TABLE [rarity_temp_table] AS SELECT * FROM [rarity];
DROP TABLE [rarity];
CREATE TABLE IF NOT EXISTS [rarity]
(
	[Id] INTEGER NOT NULL,
	[name_of_entity] VARCHAR(256) NOT NULL,
	[Code] VARCHAR(4) NOT NULL,
	[Value] INTEGER NOT NULL
);

CREATE INDEX IF NOT EXISTS [rarity_Id] ON [rarity] ([Id] ASC);
CREATE UNIQUE INDEX IF NOT EXISTS [rarity_name_of_entity] ON [rarity] ([name_of_entity] ASC);

INSERT INTO [rarity] ([Id], [name_of_entity], [Code], [Value]) SELECT [Id], [Name], [Code], [Value] FROM [rarity_temp_table];
DROP TABLE [rarity_temp_table];
PRAGMA foreign_keys = 1;
