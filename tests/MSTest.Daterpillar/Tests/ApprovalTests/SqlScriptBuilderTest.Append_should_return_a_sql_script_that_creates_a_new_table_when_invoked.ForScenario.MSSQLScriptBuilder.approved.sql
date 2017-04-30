CREATE TABLE [archetype]
(
	[Id] INT NOT NULL,
	[Name] VARCHAR(256) NOT NULL
);

ALTER TABLE [archetype] ADD PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [archetype_Name] ON [archetype] ([Name] DESC);

