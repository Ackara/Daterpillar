/* ADD ([card].[card_Card_Type_Id_TO_card_type_Id]) SCRIPT */
PRAGMA foreign_keys = 0;
CREATE TABLE [card_temp_table] AS SELECT * FROM [card];
DROP TABLE [card];

CREATE TABLE IF NOT EXISTS [card]
(
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(128) NOT NULL,
	[Text] TEXT NOT NULL,
	[Pendulum_Effect] TEXT NOT NULL,
	[Level] INTEGER NOT NULL DEFAULT 0,
	[Atk] INTEGER NOT NULL,
	[Def] INTEGER NOT NULL,
	[Card_Type_Id] INTEGER NOT NULL,
	[Attribute_Id] INTEGER NOT NULL,
	[Card_Icon_Id] INTEGER NOT NULL,
	[Monster_Type_Id] INTEGER NOT NULL,
	[Ability_Id] INTEGER NOT NULL,
	CONSTRAINT [card_Attribute_Id_TO_attribute_Id] FOREIGN KEY ([Attribute_Id]) REFERENCES [attribute]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT,
	CONSTRAINT [card_Card_Icon_Id_TO_card_icon_Id] FOREIGN KEY ([Card_Icon_Id]) REFERENCES [card_icon]([Id]) ON UPDATE CASCADE,
	CONSTRAINT [card_Monster_Type_Id_TO_monster_type_Id] FOREIGN KEY ([Monster_Type_Id]) REFERENCES [monster_type]([Id]) ON UPDATE CASCADE ON DELETE RESTRICT,
	CONSTRAINT [card_Card_Type_Id_TO_card_type_Id] FOREIGN KEY ([Card_Type_Id]) REFERENCES [card_type]([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
	PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX IF NOT EXISTS [card_Name] ON [card] ([Name] ASC);
CREATE INDEX IF NOT EXISTS [card_Attribute_Id] ON [card] ([Attribute_Id] ASC);
CREATE INDEX IF NOT EXISTS [card_Card_Icon_Id] ON [card] ([Card_Icon_Id] ASC);
CREATE INDEX IF NOT EXISTS [card_Monster_Type_Id] ON [card] ([Monster_Type_Id] ASC);
INSERT INTO [card] ([Id], [Name], [Text], [Pendulum_Effect], [Level], [Atk], [Def], [Card_Type_Id], [Attribute_Id], [Card_Icon_Id], [Monster_Type_Id], [Ability_Id]) SELECT [Id], [Name], [Text], [Pendulum_Effect], [Level], [Atk], [Def], [Card_Type_Id], [Attribute_Id], [Card_Icon_Id], [Monster_Type_Id], [Ability_Id] FROM [card_temp_table];
DROP TABLE [card_temp_table];
PRAGMA foreign_keys = 1;
/* END SCRIPT */
