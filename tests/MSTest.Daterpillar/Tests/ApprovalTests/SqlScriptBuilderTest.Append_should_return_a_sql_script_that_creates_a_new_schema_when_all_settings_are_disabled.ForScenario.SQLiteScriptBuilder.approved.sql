CREATE TABLE IF NOT EXISTS [card_type]
(
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

CREATE INDEX IF NOT EXISTS [card_type_Id] ON [card_type] ([Id] ASC);
CREATE UNIQUE INDEX IF NOT EXISTS [card_type_Name] ON [card_type] ([Name] ASC);

CREATE TABLE IF NOT EXISTS [attribute]
(
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(16) NOT NULL
);

CREATE INDEX IF NOT EXISTS [attribute_Id] ON [attribute] ([Id] ASC);
CREATE UNIQUE INDEX IF NOT EXISTS [attribute_Name] ON [attribute] ([Name] ASC);

CREATE TABLE IF NOT EXISTS [card_icon]
(
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(16) NOT NULL
);

CREATE INDEX IF NOT EXISTS [card_icon_Id] ON [card_icon] ([Id] ASC);
CREATE UNIQUE INDEX IF NOT EXISTS [card_icon_Name] ON [card_icon] ([Name] ASC);

CREATE TABLE IF NOT EXISTS [monster_type]
(
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

CREATE INDEX IF NOT EXISTS [monster_type_Id] ON [monster_type] ([Id] ASC);
CREATE UNIQUE INDEX IF NOT EXISTS [monster_type_Name] ON [monster_type] ([Name] ASC);

CREATE TABLE IF NOT EXISTS [ability]
(
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

CREATE INDEX IF NOT EXISTS [ability_Id] ON [ability] ([Id] ASC);
CREATE UNIQUE INDEX IF NOT EXISTS [ability_Name] ON [ability] ([Name] ASC);

CREATE TABLE IF NOT EXISTS [card]
(
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(128) NOT NULL,
	[Text] TEXT NOT NULL,
	[Pendulum_Effect] TEXT NOT NULL,
	[Level] INTEGER NOT NULL DEFAULT '0',
	[Atk] INTEGER NOT NULL,
	[Def] INTEGER NOT NULL,
	[Card_Type_Id] INTEGER NOT NULL,
	[Attribute_Id] INTEGER NOT NULL,
	[Card_Icon_Id] INTEGER NOT NULL,
	[Monster_Type_Id] INTEGER NOT NULL,
	[Ability_Id] INTEGER NOT NULL,
	CONSTRAINT [card_Attribute_Id_TO_attribute_Id] FOREIGN KEY ([Attribute_Id]) REFERENCES [attribute]([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT [card_Card_Icon_Id_TO_card_icon_Id] FOREIGN KEY ([Card_Icon_Id]) REFERENCES [card_icon]([Id]) ON UPDATE CASCADE,
	CONSTRAINT [card_Monster_Type_Id_TO_monster_type_Id] FOREIGN KEY ([Monster_Type_Id]) REFERENCES [monster_type]([Id]) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS [card_Id] ON [card] ([Id] ASC);
CREATE UNIQUE INDEX IF NOT EXISTS [card_Name] ON [card] ([Name] ASC);

CREATE TABLE IF NOT EXISTS [card_extras]
(
	[Card_Id] INTEGER NOT NULL,
	[Rulings] TEXT,
	[Tips] TEXT,
	[Trivia] TEXT,
	[Passcode] INTEGER NOT NULL,
	CONSTRAINT [card_extras_Card_Id_TO_card_Id] FOREIGN KEY ([Card_Id]) REFERENCES [card]([Id])
);

CREATE INDEX IF NOT EXISTS [card_extras_Card_Id] ON [card_extras] ([Card_Id] ASC);

CREATE TABLE IF NOT EXISTS [pack]
(
	[Id] INTEGER NOT NULL,
	[Acronym] VARCHAR(32) NOT NULL,
	[Name] VARCHAR(128) NOT NULL,
	[Description] TEXT NOT NULL,
	[Release_Date] DATE NOT NULL,
	[Size] INTEGER NOT NULL,
	[Konami_Id] INTEGER NOT NULL
);

CREATE INDEX IF NOT EXISTS [pack_Id] ON [pack] ([Id] ASC);
CREATE UNIQUE INDEX IF NOT EXISTS [pack_Name] ON [pack] ([Name] ASC);

CREATE TABLE IF NOT EXISTS [rarity]
(
	[Id] INTEGER NOT NULL,
	[Name] VARCHAR(32) NOT NULL,
	[Code] VARCHAR(4) NOT NULL,
	[Value] INTEGER NOT NULL
);

CREATE INDEX IF NOT EXISTS [rarity_Id] ON [rarity] ([Id] ASC);
CREATE UNIQUE INDEX IF NOT EXISTS [rarity_Name] ON [rarity] ([Name] ASC);

CREATE TABLE IF NOT EXISTS [card_number]
(
	[Pack_Acronym] VARCHAR(32) NOT NULL,
	[Number] INTEGER NOT NULL,
	[Rarity_Id] INTEGER NOT NULL,
	[Art_Index] INTEGER NOT NULL,
	[Card_Id] INTEGER NOT NULL,
	[Pack_Id] INTEGER NOT NULL,
	CONSTRAINT [key_with_custom_name] FOREIGN KEY ([Rarity_Id]) REFERENCES [rarity]([Id]),
	CONSTRAINT [card_number_Card_Id_TO_card_Id] FOREIGN KEY ([Card_Id]) REFERENCES [card]([Id]),
	CONSTRAINT [card_number_Pack_Id_TO_pack_Id] FOREIGN KEY ([Pack_Id]) REFERENCES [pack]([Id])
);

CREATE INDEX IF NOT EXISTS [card_number_Pack_Acronym_and_Number_and_Rarity_Id_and_Art_Index] ON [card_number] ([Pack_Acronym] ASC, [Number] ASC, [Rarity_Id] ASC, [Art_Index] ASC);

CREATE TABLE IF NOT EXISTS [effect]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(32) NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS [effect_Name] ON [effect] ([Name] ASC);


      /* This is a System.Flag */
      INSERT INTO card_type VALUES
      (1, 'Spell'),
      (2, 'Trap'),
      (4, 'Normal'),
      (8, 'Effect'),
      (16, 'Fusion'),
      (32, 'Ritual'),
      (64, 'Synchro'),
      (128, 'Xyz'),
      (256, 'Pendulum');

      /* This is a System.Flag */
      INSERT INTO ability VALUES
      (01, 'Effect'),
      (02, 'Flip'),
      (04, 'Gemini'),
      (08, 'Spirit'),
      (16, 'Tuner'),
      (32, 'Union');

      INSERT INTO attribute VALUES
      (0, 'None'),
      (1, 'Dark'),
      (2, 'Divine'),
      (3, 'Earth'),
      (4, 'Fire'),
      (5, 'Light'),
      (6, 'Water'),
      (7, 'Wind');

      INSERT INTO card_icon VALUES
      (1, 'Continuous'),
      (2, 'Counter'),
      (3, 'Equip'),
      (4, 'Field'),
      (5, 'Normal'),
      (6, 'Quick-Play'),
      (7, 'Ritual');

      INSERT INTO monster_type VALUES
      (00, 'None'),
      (01, 'Aqua'),
      (02, 'Beast'),
      (03, 'Beast-Warrior'),
      (04, 'Dinosaur'),
      (05, 'Divine-Beast'),
      (06, 'Dragon'),
      (07, 'Fairy'),
      (08, 'Fiend'),
      (09, 'Fish'),
      (10, 'Insect'),
      (11, 'Machine'),
      (12, 'Plant'),
      (13, 'Psychic'),
      (14, 'Pyro'),
      (15, 'Reptile'),
      (16, 'Rock'),
      (17, 'Sea Serpent'),
      (18, 'Spellcaster'),
      (19, 'Thunder'),
      (20, 'Warrior'),
      (21, 'Winged Beast'),
      (22, 'Wyrm'),
      (23, 'Zombie');

      INSERT INTO rarity VALUES
      (01, 'Common', 'C', '1'),
      (02, 'Rare', 'R', '2'),
      (03, 'Gold Secret', 'GS', '2'),
      (04, 'Gold Rare', 'GR', '2'),
      (05, 'Super Rare', 'SR', '3'),
      (06, 'Holographic', 'HR', '3'),
      (07, 'Secret Rare', 'SE', '5'),
      (08, 'Ultra Rare', 'UR', '6'),
      (09, 'Ultimate Rare', 'UL', '7'),
      (10, 'Ghost Rare', 'GH', '8'),
      (11, 'Hobby', 'H', '5'),
      (12, 'Shattefoil', 'SH', '6'),
      (13, 'Starfoil', 'ST', '6'),
      (14, 'Platinum Secret Rare', 'PS', '8');
    
