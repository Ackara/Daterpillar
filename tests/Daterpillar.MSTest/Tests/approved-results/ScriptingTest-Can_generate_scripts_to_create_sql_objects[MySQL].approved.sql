SYNTAX: MySQL


CREATE TABLE `genre` (
	`Id`  INT NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT '',
	`Name`  VARCHAR(255) NOT NULL COMMENT ''
)
COMMENT 'Represents a music genre.';

CREATE TABLE `song` (
	`Id`  INT NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT 'The unique identifier.',
	`Name`  VARCHAR(15) NOT NULL COMMENT '',
	`GenreId`  INT NOT NULL COMMENT '',
	`Track`  TINYINT NOT NULL DEFAULT 1 COMMENT '',
	`Lyrics`  VARCHAR(255) COMMENT '',
	CONSTRAINT `song_GenreId_TO_genre_Id__fk` FOREIGN KEY (`GenreId`) REFERENCES `genre`(`Id`) ON UPDATE CASCADE ON DELETE RESTRICT
)
COMMENT '';

CREATE INDEX `song__GenreId_index` ON `song` (`GenreId` ASC);

CREATE TABLE `album` (
	`SongId`  INT NOT NULL COMMENT '',
	`ArtistId`  INT NOT NULL COMMENT '',
	`Name`  VARCHAR(255) NOT NULL COMMENT '',
	`Year`  SMALLINT NOT NULL COMMENT '',
	`Price`  DECIMAL(8, 2) NOT NULL COMMENT ''
)
COMMENT '';

-- If you are reading this, I don't discriminate.

INSERT INTO genre (Name) VAlUES ('Hip Hop'), ('R&B'), ('Country');

INSERT INTO song (Name, GenreId, Track) VALUES ('Survival', '1', '1');

INSERT INTO album (SongId, ArtistId, Name, Year, Price) VALUES ('1', '1', 'Scorpion', '2018', '14.99');

ALTER TABLE `song` ADD COLUMN `ReleaseDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '';

ALTER TABLE `album` ADD CONSTRAINT `album_SongId_TO_song_Id__fk` FOREIGN KEY (`SongId`) REFERENCES `song`(`Id`)  ON UPDATE CASCADE ON DELETE RESTRICT;

CREATE UNIQUE INDEX `song__Name_index` ON `song` (`Name` DESC);

CREATE INDEX `album__SongId_and_ArtistId_index` ON `album` (`SongId` ASC, `ArtistId` ASC);

