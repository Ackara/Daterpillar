﻿CREATE TABLE [card_type]
(
	[Id] INT NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

ALTER TABLE [card_type] ADD PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [card_type_Name] ON [card_type] ([Name] ASC);

CREATE TABLE [attribute]
(
	[Id] TINYINT NOT NULL,
	[Name] VARCHAR(16) NOT NULL
);

ALTER TABLE [attribute] ADD PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [attribute_Name] ON [attribute] ([Name] ASC);

CREATE TABLE [card_icon]
(
	[Id] TINYINT NOT NULL,
	[Name] VARCHAR(16) NOT NULL
);

ALTER TABLE [card_icon] ADD PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [card_icon_Name] ON [card_icon] ([Name] ASC);

CREATE TABLE [monster_type]
(
	[Id] TINYINT NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

ALTER TABLE [monster_type] ADD PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [monster_type_Name] ON [monster_type] ([Name] ASC);

CREATE TABLE [ability]
(
	[Id] INT NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

ALTER TABLE [ability] ADD PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [ability_Name] ON [ability] ([Name] ASC);

CREATE TABLE [card]
(
	[Id] INT NOT NULL,
	[Name] VARCHAR(128) NOT NULL,
	[Text] TEXT NOT NULL,
	[Pendulum_Effect] TEXT NOT NULL,
	[Level] TINYINT NOT NULL DEFAULT '0',
	[Atk] INT NOT NULL,
	[Def] INT NOT NULL,
	[Card_Type_Id] INT NOT NULL,
	[Attribute_Id] TINYINT NOT NULL,
	[Card_Icon_Id] TINYINT NOT NULL,
	[Monster_Type_Id] TINYINT NOT NULL,
	[Ability_Id] INT NOT NULL,
	CONSTRAINT [card_Attribute_Id_TO_attribute_Id] FOREIGN KEY ([Attribute_Id]) REFERENCES [attribute]([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT [card_Card_Icon_Id_TO_card_icon_Id] FOREIGN KEY ([Card_Icon_Id]) REFERENCES [card_icon]([Id]) ON UPDATE CASCADE,
	CONSTRAINT [card_Monster_Type_Id_TO_monster_type_Id] FOREIGN KEY ([Monster_Type_Id]) REFERENCES [monster_type]([Id]) ON UPDATE CASCADE ON DELETE CASCADE
);

ALTER TABLE [card] ADD PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [card_Name] ON [card] ([Name] ASC);

CREATE TABLE [card_extras]
(
	[Card_Id] INT NOT NULL,
	[Rulings] TEXT,
	[Tips] TEXT,
	[Trivia] TEXT,
	[Passcode] INT NOT NULL,
	CONSTRAINT [card_extras_Card_Id_TO_card_Id] FOREIGN KEY ([Card_Id]) REFERENCES [card]([Id])
);

ALTER TABLE [card_extras] ADD PRIMARY KEY ([Card_Id] ASC);

CREATE TABLE [pack]
(
	[Id] INT NOT NULL,
	[Acronym] VARCHAR(32) NOT NULL,
	[Name] VARCHAR(128) NOT NULL,
	[Description] TEXT NOT NULL,
	[Release_Date] DATE NOT NULL,
	[Size] INT NOT NULL,
	[Konami_Id] INT NOT NULL
);

ALTER TABLE [pack] ADD PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [pack_Name] ON [pack] ([Name] ASC);

CREATE TABLE [rarity]
(
	[Id] TINYINT NOT NULL,
	[Name] VARCHAR(32) NOT NULL,
	[Code] VARCHAR(4) NOT NULL,
	[Value] INT NOT NULL
);

ALTER TABLE [rarity] ADD PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [rarity_Name] ON [rarity] ([Name] ASC);

CREATE TABLE [card_number]
(
	[Pack_Acronym] VARCHAR(32) NOT NULL,
	[Number] SMALLINT NOT NULL,
	[Rarity_Id] TINYINT NOT NULL,
	[Art_Index] TINYINT NOT NULL,
	[Card_Id] INT NOT NULL,
	[Pack_Id] INT NOT NULL,
	CONSTRAINT [key_with_custom_name] FOREIGN KEY ([Rarity_Id]) REFERENCES [rarity]([Id]),
	CONSTRAINT [card_number_Card_Id_TO_card_Id] FOREIGN KEY ([Card_Id]) REFERENCES [card]([Id]),
	CONSTRAINT [card_number_Pack_Id_TO_pack_Id] FOREIGN KEY ([Pack_Id]) REFERENCES [pack]([Id])
);

ALTER TABLE [card_number] ADD PRIMARY KEY ([Pack_Acronym] ASC, [Number] ASC, [Rarity_Id] ASC, [Art_Index] ASC);

CREATE TABLE [effect]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[Name] VARCHAR(32) NOT NULL
);

CREATE UNIQUE INDEX [effect_Name] ON [effect] ([Name] ASC);

