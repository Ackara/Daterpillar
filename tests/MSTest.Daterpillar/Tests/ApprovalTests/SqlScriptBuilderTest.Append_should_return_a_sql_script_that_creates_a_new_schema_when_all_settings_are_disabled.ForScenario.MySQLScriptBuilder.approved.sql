CREATE TABLE IF NOT EXISTS `card_type`
(
	`Id` INT NOT NULL,
	`Name` VARCHAR(32) NOT NULL
);

ALTER TABLE `card_type` ADD PRIMARY KEY (`Id` ASC);
CREATE UNIQUE INDEX `card_type_Name` ON `card_type` (`Name` ASC);

CREATE TABLE IF NOT EXISTS `attribute`
(
	`Id` TINYINT NOT NULL,
	`Name` VARCHAR(16) NOT NULL
);

ALTER TABLE `attribute` ADD PRIMARY KEY (`Id` ASC);
CREATE UNIQUE INDEX `attribute_Name` ON `attribute` (`Name` ASC);

CREATE TABLE IF NOT EXISTS `card_icon`
(
	`Id` TINYINT NOT NULL,
	`Name` VARCHAR(16) NOT NULL
);

ALTER TABLE `card_icon` ADD PRIMARY KEY (`Id` ASC);
CREATE UNIQUE INDEX `card_icon_Name` ON `card_icon` (`Name` ASC);

CREATE TABLE IF NOT EXISTS `monster_type`
(
	`Id` TINYINT NOT NULL,
	`Name` VARCHAR(32) NOT NULL
);

ALTER TABLE `monster_type` ADD PRIMARY KEY (`Id` ASC);
CREATE UNIQUE INDEX `monster_type_Name` ON `monster_type` (`Name` ASC);

CREATE TABLE IF NOT EXISTS `ability`
(
	`Id` INT NOT NULL COMMENT 'Get or set the Id. This value is a flag/it must be a power of 2.',
	`Name` VARCHAR(32) NOT NULL
);

ALTER TABLE `ability` ADD PRIMARY KEY (`Id` ASC);
CREATE UNIQUE INDEX `ability_Name` ON `ability` (`Name` ASC);

CREATE TABLE IF NOT EXISTS `card`
(
	`Id` INT NOT NULL,
	`Name` VARCHAR(128) NOT NULL,
	`Text` TEXT NOT NULL,
	`Pendulum_Effect` TEXT NOT NULL,
	`Level` TINYINT NOT NULL DEFAULT '0',
	`Atk` INT NOT NULL,
	`Def` INT NOT NULL,
	`Card_Type_Id` INT NOT NULL,
	`Attribute_Id` TINYINT NOT NULL,
	`Card_Icon_Id` TINYINT NOT NULL,
	`Monster_Type_Id` TINYINT NOT NULL,
	`Ability_Id` INT NOT NULL,
	CONSTRAINT `card_Attribute_Id_TO_attribute_Id` FOREIGN KEY (`Attribute_Id`) REFERENCES `attribute`(`Id`) ON UPDATE CASCADE ON DELETE RESTRICT,
	CONSTRAINT `card_Card_Icon_Id_TO_card_icon_Id` FOREIGN KEY (`Card_Icon_Id`) REFERENCES `card_icon`(`Id`) ON UPDATE CASCADE ON DELETE NO ACTION,
	CONSTRAINT `card_Monster_Type_Id_TO_monster_type_Id` FOREIGN KEY (`Monster_Type_Id`) REFERENCES `monster_type`(`Id`) ON UPDATE CASCADE ON DELETE RESTRICT
);

ALTER TABLE `card` ADD PRIMARY KEY (`Id` ASC);
CREATE UNIQUE INDEX `card_Name` ON `card` (`Name` ASC);
CREATE INDEX `card_Attribute_Id` ON `card` (`Attribute_Id` ASC);
CREATE INDEX `card_Card_Icon_Id` ON `card` (`Card_Icon_Id` ASC);
CREATE INDEX `card_Monster_Type_Id` ON `card` (`Monster_Type_Id` ASC);

CREATE TABLE IF NOT EXISTS `card_extras`
(
	`Card_Id` INT NOT NULL,
	`Rulings` TEXT,
	`Tips` TEXT,
	`Trivia` TEXT,
	`Passcode` INT NOT NULL,
	CONSTRAINT `card_extras_Card_Id_TO_card_Id` FOREIGN KEY (`Card_Id`) REFERENCES `card`(`Id`) ON UPDATE NO ACTION ON DELETE NO ACTION
);

ALTER TABLE `card_extras` ADD PRIMARY KEY (`Card_Id` ASC);

CREATE TABLE IF NOT EXISTS `pack`
(
	`Id` INT NOT NULL,
	`Acronym` VARCHAR(32) NOT NULL,
	`Name` VARCHAR(128) NOT NULL,
	`Description` TEXT NOT NULL,
	`Release_Date` DATE NOT NULL,
	`Size` INT NOT NULL COMMENT 'The number of unique cards in the pack.',
	`Konami_Id` INT NOT NULL
);

ALTER TABLE `pack` ADD PRIMARY KEY (`Id` ASC);
CREATE UNIQUE INDEX `pack_Name` ON `pack` (`Name` ASC);

CREATE TABLE IF NOT EXISTS `rarity`
(
	`Id` TINYINT NOT NULL,
	`Name` VARCHAR(32) NOT NULL,
	`Code` VARCHAR(4) NOT NULL,
	`Value` INT NOT NULL
);

ALTER TABLE `rarity` ADD PRIMARY KEY (`Id` ASC);
CREATE UNIQUE INDEX `rarity_Name` ON `rarity` (`Name` ASC);

CREATE TABLE IF NOT EXISTS `card_number`
(
	`Pack_Acronym` VARCHAR(32) NOT NULL,
	`Number` SMALLINT NOT NULL,
	`Rarity_Id` TINYINT NOT NULL,
	`Art_Index` TINYINT NOT NULL,
	`Card_Id` INT NOT NULL,
	`Pack_Id` INT NOT NULL,
	CONSTRAINT `card_number_Rarity_Id_TO_rarity_Id` FOREIGN KEY (`Rarity_Id`) REFERENCES `rarity`(`Id`) ON UPDATE NO ACTION ON DELETE NO ACTION,
	CONSTRAINT `card_number_Card_Id_TO_card_Id` FOREIGN KEY (`Card_Id`) REFERENCES `card`(`Id`) ON UPDATE NO ACTION ON DELETE NO ACTION,
	CONSTRAINT `card_number_Pack_Id_TO_pack_Id` FOREIGN KEY (`Pack_Id`) REFERENCES `pack`(`Id`) ON UPDATE NO ACTION ON DELETE NO ACTION
);

CREATE INDEX `card_number_Rarity_Id` ON `card_number` (`Rarity_Id` ASC);
CREATE INDEX `card_number_Card_Id` ON `card_number` (`Card_Id` ASC);
CREATE INDEX `card_number_Pack_Id` ON `card_number` (`Pack_Id` ASC);
ALTER TABLE `card_number` ADD PRIMARY KEY (`Pack_Acronym` ASC, `Number` ASC, `Rarity_Id` ASC, `Art_Index` ASC);

CREATE TABLE IF NOT EXISTS `effect`
(
	`Id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'A custom column comment.',
	`Name` VARCHAR(32) NOT NULL
);

CREATE UNIQUE INDEX `effect_Name` ON `effect` (`Name` ASC);


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
    
