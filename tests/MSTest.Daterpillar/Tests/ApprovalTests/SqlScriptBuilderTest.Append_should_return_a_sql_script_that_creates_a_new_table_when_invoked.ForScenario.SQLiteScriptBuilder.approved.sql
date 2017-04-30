CREATE TABLE IF NOT EXISTS [archetype]
(
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(256) NOT NULL
);

CREATE INDEX IF NOT EXISTS [archetype_Id] ON [archetype] ([Id] ASC);
CREATE UNIQUE INDEX IF NOT EXISTS [archetype_Name] ON [archetype] ([Name] DESC);

