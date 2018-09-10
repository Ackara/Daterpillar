

-- ----------------------------------------
-- TABLE [ability]
-- ----------------------------------------
CREATE TABLE [ability]
(
	[Id] INT NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

ALTER TABLE [ability] ADD CONSTRAINT [ability_Id] PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [ability_Name] ON [ability] ([Name] ASC);

-- ----------------------------------------
-- TABLE [archetype]
-- ----------------------------------------
CREATE TABLE [archetype]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[Name] VARCHAR(128) NOT NULL,
	[Description] TEXT NOT NULL,
	[Accepted] BIT NOT NULL
);

CREATE UNIQUE INDEX [archetype_Name] ON [archetype] ([Name] ASC);

-- ----------------------------------------
-- TABLE [attribute]
-- ----------------------------------------
CREATE TABLE [attribute]
(
	[Id] INT NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

ALTER TABLE [attribute] ADD CONSTRAINT [attribute_Id] PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [attribute_Name] ON [attribute] ([Name] ASC);

-- ----------------------------------------
-- TABLE [legality]
-- ----------------------------------------
CREATE TABLE [legality]
(
	[Id] INT NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

ALTER TABLE [legality] ADD CONSTRAINT [legality_Id] PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [legality_Name] ON [legality] ([Name] ASC);

-- ----------------------------------------
-- TABLE [card]
-- ----------------------------------------
CREATE TABLE [card]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[Name] VARCHAR(128) NOT NULL,
	[LocalizedName] VARCHAR(128) NOT NULL,
	[Archetypes] VARCHAR(128) NOT NULL,
	[Text] TEXT NOT NULL,
	[PendulumEffect] TEXT NOT NULL,
	[Card_Type_Id] INT NOT NULL,
	[Ability] INT NOT NULL,
	[Attribute_Id] INT NOT NULL,
	[Icon_Id] INT NOT NULL,
	[Monster_Type_Id] INT NOT NULL,
	[Status] INT NOT NULL,
	[Level] INT NOT NULL,
	[Scale] INT NOT NULL,
	[Link] INT NOT NULL,
	[Atk] INT NOT NULL,
	[Def] INT NOT NULL,
	[KonamiId] INT NOT NULL,
	CONSTRAINT [card_Status_TO_legality_Id] FOREIGN KEY ([Status]) REFERENCES [legality]([Id]) ON UPDATE CASCADE ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [card_Name] ON [card] ([Name] ASC);
CREATE INDEX [card_Card_Type_Id] ON [card] ([Card_Type_Id] ASC);
CREATE INDEX [card_Ability] ON [card] ([Ability] ASC);
CREATE INDEX [card_Attribute_Id] ON [card] ([Attribute_Id] ASC);
CREATE INDEX [card_Icon_Id] ON [card] ([Icon_Id] ASC);
CREATE INDEX [card_Monster_Type_Id] ON [card] ([Monster_Type_Id] ASC);
CREATE INDEX [card_Status] ON [card] ([Status] ASC);

-- ----------------------------------------
-- TABLE [rarity]
-- ----------------------------------------
CREATE TABLE [rarity]
(
	[Id] INT NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

ALTER TABLE [rarity] ADD CONSTRAINT [rarity_Id] PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [rarity_Name] ON [rarity] ([Name] ASC);

-- ----------------------------------------
-- TABLE [product_type]
-- ----------------------------------------
CREATE TABLE [product_type]
(
	[Id] INT NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

ALTER TABLE [product_type] ADD CONSTRAINT [product_type_Id] PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [product_type_Name] ON [product_type] ([Name] ASC);

-- ----------------------------------------
-- TABLE [pack]
-- ----------------------------------------
CREATE TABLE [pack]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[Acronym] VARCHAR(8) NOT NULL,
	[Name] VARCHAR(128) NOT NULL,
	[Product_Type_Id] INT NOT NULL,
	[Description] TEXT NOT NULL,
	[Size] INT NOT NULL,
	[ReleaseDate] DATETIME NOT NULL,
	[KonamiId] INT NOT NULL,
	CONSTRAINT [pack_Product_Type_Id_TO_product_type_Id] FOREIGN KEY ([Product_Type_Id]) REFERENCES [product_type]([Id]) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE INDEX [pack_Acronym] ON [pack] ([Acronym] ASC);
CREATE UNIQUE INDEX [pack_Name] ON [pack] ([Name] ASC);
CREATE INDEX [pack_Product_Type_Id] ON [pack] ([Product_Type_Id] ASC);

-- ----------------------------------------
-- TABLE [card_number]
-- ----------------------------------------
CREATE TABLE [card_number]
(
	[Pack_Acronym] VARCHAR(64) NOT NULL,
	[Language] VARCHAR(64) NOT NULL,
	[Edition] VARCHAR(64) NOT NULL,
	[Number] INT NOT NULL,
	[Rarity_Id] INT NOT NULL,
	[Artwork] INT NOT NULL,
	[Card_Id] INT NOT NULL,
	[Pack_Id] INT NOT NULL,
	CONSTRAINT [card_number_Rarity_Id_TO_rarity_Id] FOREIGN KEY ([Rarity_Id]) REFERENCES [rarity]([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT [card_number_Card_Id_TO_card_Id] FOREIGN KEY ([Card_Id]) REFERENCES [card]([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT [card_number_Pack_Id_TO_pack_Id] FOREIGN KEY ([Pack_Id]) REFERENCES [pack]([Id]) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE INDEX [card_number_Card_Id] ON [card_number] ([Card_Id] ASC);
CREATE INDEX [card_number_Pack_Id] ON [card_number] ([Pack_Id] ASC);
ALTER TABLE [card_number] ADD CONSTRAINT [card_number_Pack_Acronym_and_Language_and_Edition_and_Number_and_Rarity_Id_and_Artwork] PRIMARY KEY ([Pack_Acronym] ASC, [Language] ASC, [Edition] ASC, [Number] ASC, [Rarity_Id] ASC, [Artwork] ASC);

-- ----------------------------------------
-- TABLE [card_type]
-- ----------------------------------------
CREATE TABLE [card_type]
(
	[Id] INT NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

ALTER TABLE [card_type] ADD CONSTRAINT [card_type_Id] PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [card_type_Name] ON [card_type] ([Name] ASC);

-- ----------------------------------------
-- TABLE [icon]
-- ----------------------------------------
CREATE TABLE [icon]
(
	[Id] INT NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

ALTER TABLE [icon] ADD CONSTRAINT [icon_Id] PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [icon_Name] ON [icon] ([Name] ASC);

-- ----------------------------------------
-- TABLE [monster_type]
-- ----------------------------------------
CREATE TABLE [monster_type]
(
	[Id] INT NOT NULL,
	[Name] VARCHAR(32) NOT NULL
);

ALTER TABLE [monster_type] ADD CONSTRAINT [monster_type_Id] PRIMARY KEY ([Id] ASC);
CREATE UNIQUE INDEX [monster_type_Name] ON [monster_type] ([Name] ASC);


-- ----------------------------------------
-- TABLE [event]
-- ----------------------------------------
CREATE TABLE [event]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[Date] DATETIME NOT NULL,
	[Level] VARCHAR(8) NOT NULL,
	[Logger] VARCHAR(128) NOT NULL,
	[Message] TEXT NOT NULL,
	[Exception] TEXT NOT NULL,
	[Branch] VARCHAR(32),
	[Machine] VARCHAR(32)
);

CREATE INDEX [event_Level] ON [event] ([Level] ASC);


-- ----------------------------------------
-- SCRIPT: ability seed data
-- ----------------------------------------
INSERT INTO ability (Id, Name) VALUES ('1', 'Effect'), ('2', 'Flip'), ('4', 'Tuner'), ('8', 'Gemini'), ('16', 'Union'), ('32', 'Spirit'), ('64', 'Toon');
-- ----------------------------------------
-- SCRIPT: attribute seed data
-- ----------------------------------------
INSERT INTO attribute (Id, Name) VALUES ('1', 'Dark'), ('2', 'Light'), ('3', 'Earth'), ('4', 'Water'), ('5', 'Fire'), ('6', 'Wind'), ('7', 'Divine');
-- ----------------------------------------
-- SCRIPT: card_type seed data
-- ----------------------------------------
INSERT INTO card_type (Id, Name) VALUES ('1', 'Spell'), ('2', 'Trap'), ('4', 'Normal'), ('8', 'Effect'), ('16', 'Ritual'), ('32', 'Fusion'), ('64', 'Synchro'), ('128', 'Xyz'), ('256', 'Pendulum'), ('512', 'Link');
-- ----------------------------------------
-- SCRIPT: icon seed data
-- ----------------------------------------
INSERT INTO icon (Id, Name) VALUES ('1', 'Normal'), ('2', 'Equip'), ('3', 'Field'), ('4', 'Quick-Play'), ('5', 'Ritual'), ('6', 'Continuous'), ('7', 'Counter');
-- ----------------------------------------
-- SCRIPT: legality seed data
-- ----------------------------------------
INSERT INTO legality (Id, Name) VALUES ('1', 'Unlimited'), ('2', 'Forbidden'), ('3', 'Limited'), ('4', 'Semi-Limited');
-- ----------------------------------------
-- SCRIPT: monster_type seed data
-- ----------------------------------------
INSERT INTO monster_type (Id, Name) VALUES ('1', 'Spellcaster'), ('2', 'Dragon'), ('3', 'Zombie'), ('4', 'Warrior'), ('5', 'Beast-Warrior'), ('6', 'Winged Beast'), ('7', 'Fiend'), ('8', 'Fairy'), ('9', 'Insect'), ('10', 'Dinosaur'), ('11', 'Reptile'), ('12', 'Fish'), ('13', 'Sea Serpent'), ('14', 'Aqua'), ('15', 'Pyro'), ('16', 'Thunder'), ('17', 'Rock'), ('18', 'Plant'), ('19', 'Machine'), ('20', 'Psychic'), ('21', 'Divine-Beast'), ('22', 'Wyrm'), ('23', 'Cyberse'), ('24', 'Beast');
-- ----------------------------------------
-- SCRIPT: product_type seed data
-- ----------------------------------------
INSERT INTO product_type (Id, Name) VALUES ('1', 'BoosterPack'), ('2', 'SpecialEditionBox'), ('3', 'StarterDeck'), ('4', 'StructureDeck'), ('5', 'Tin'), ('6', 'DuelistPack'), ('7', 'DuelTerminal'), ('8', 'Manga'), ('9', 'Tournament'), ('10', 'Promo'), ('11', 'VideoGame'), ('12', 'Others');
-- ----------------------------------------
-- SCRIPT: rarity seed data
-- ----------------------------------------
INSERT INTO rarity (Id, Name) VALUES ('1', 'Common'), ('2', 'Rare'), ('3', 'GoldSecret'), ('4', 'GoldRare'), ('5', 'SuperRare'), ('6', 'Holographic'), ('7', 'SecretRare'), ('8', 'UltraRare'), ('9', 'UltimateRare'), ('10', 'GhostRare'), ('11', 'Hobby'), ('12', 'Shattefoil'), ('13', 'Starfoil'), ('14', 'PlatinumSecretRare');

GO
CREATE VIEW card_archetype AS
SELECT 
  a.Name AS 'Name',
  c.Id AS 'Card_Id', 
  a.Id AS 'Archetype_Id'
FROM card c
  JOIN archetype a on c.Name LIKE concat('%', a.Name, '%')
WHERE a.Accepted = 1;

