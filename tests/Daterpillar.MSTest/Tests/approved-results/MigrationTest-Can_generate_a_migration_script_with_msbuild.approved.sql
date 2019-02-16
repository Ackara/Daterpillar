-- Creating the genre table

CREATE TABLE `genre` (
	`Id` INT NOT NULL COMMENT '',
	`Name` VARCHAR(255) NOT NULL COMMENT '',
	PRIMARY KEY (`Id` ASC)
)
COMMENT '';

CREATE UNIQUE INDEX `genre__Name_index` ON `genre` (`Name` ASC);

-- End --

-- Creating the song table

CREATE TABLE `song` (
	`Id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT '',
	`Name` VARCHAR(255) NOT NULL COMMENT '',
	`Length` INT NOT NULL COMMENT 'Get or set the song\'s length in seconds.',
	`Genre` INT NOT NULL COMMENT '',
	`Disc` INT NOT NULL DEFAULT 1 COMMENT '',
	`Track` SMALLINT NOT NULL COMMENT '',
	`Artist` VARCHAR(255) NOT NULL COMMENT '',
	`Album` VARCHAR(255) NOT NULL COMMENT '',
	CONSTRAINT `song_Genre_TO_genre_Id__fk` FOREIGN KEY (`Genre`) REFERENCES `genre`(`Id`) ON UPDATE CASCADE ON DELETE RESTRICT
)
COMMENT '';

CREATE INDEX `song__Name_index` ON `song` (`Name` ASC);

CREATE INDEX `song__Genre_index` ON `song` (`Genre` ASC);

-- End --

CREATE TABLE `artist` (
	`Name` VARCHAR(32) NOT NULL COMMENT '',
	`Bio` TEXT NOT NULL COMMENT '',
	`Id` INT NOT NULL COMMENT '',
	`DOB` DATETIME NOT NULL COMMENT '',
	PRIMARY KEY (`Id` ASC)
)
COMMENT '';


-- Seed-Data
-- -----------------------------------------------
INSERT INTO genre (Id, Name)
VALUES (1, 'Hip Hop'), (2, 'Rock'), (3, 'Pop'), (4, 'R&B');


-- If you're reading this it means multiple scrips can be used.

