file: V1.1__alter_schema.sql


-- Before table works.

-- Creating the publisher table

CREATE TABLE `publisher` (
	`Id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT '',
	`Name` VARCHAR(255) NOT NULL COMMENT '',
	`logo` BLOB COMMENT ''
)
COMMENT '';

INSERT INTO publisher (Name) VALUES ('OVO'), ('UMG');

-- End --

ALTER TABLE `song` RENAME TO `track`;

-- Modifying the track table

ALTER TABLE `track` ADD COLUMN `PublisherId` INT NOT NULL DEFAULT 1 COMMENT '';

ALTER TABLE `track` CHANGE COLUMN `Id` `Id` INT NOT NULL COMMENT '';

ALTER TABLE `track` CHANGE COLUMN `Length` `Duration` INT NOT NULL COMMENT 'Get or set the song\'s length in seconds.';

ALTER TABLE `track` DROP FOREIGN KEY `song_Genre_TO_genre_Id__fk`;

ALTER TABLE `track` ADD CONSTRAINT `track_PublisherId_TO_publisher_Id__fk` FOREIGN KEY (`PublisherId`) REFERENCES `publisher`(`Id`)  ON UPDATE CASCADE ON DELETE RESTRICT;

DROP INDEX `song__Name_index` ON `track`;

DROP INDEX `song__Genre_index` ON `track`;

ALTER TABLE `track` DROP COLUMN `Genre`;

CREATE INDEX `track__Name_index` ON `track` (`Name` ASC);

CREATE INDEX `track__PublisherId_index` ON `track` (`PublisherId` ASC);

-- End --

-- Modifying the artist table

ALTER TABLE `artist` CHANGE COLUMN `Bio` `Bio` VARCHAR(512) DEFAULT '' COMMENT '';

ALTER TABLE `artist` CHANGE COLUMN `Id` `Id` INT NOT NULL AUTO_INCREMENT COMMENT '';

-- End --

DROP TABLE `genre`;

CREATE TABLE `album` (
	`Name` VARCHAR(255) NOT NULL COMMENT '',
	`Year` INT NOT NULL COMMENT '',
	`ArtistId` INT NOT NULL COMMENT '',
	`SongId` INT NOT NULL COMMENT '',
	PRIMARY KEY (`SongId` ASC, `ArtistId` ASC),
	CONSTRAINT `album_SongId_TO_track_Id__fk` FOREIGN KEY (`SongId`) REFERENCES `track`(`Id`) ON UPDATE CASCADE ON DELETE RESTRICT,
	CONSTRAINT `album_ArtistId_TO_artist_Id__fk` FOREIGN KEY (`ArtistId`) REFERENCES `artist`(`Id`) ON UPDATE CASCADE ON DELETE RESTRICT
)
COMMENT '';


    -- CHECKS:
    -- * Create [Ablum] table.
    -- * Create [song].[PublisherId] column, index and foreign-key.
    -- * Drop [Genre] table.
    -- * Drop [song].[Genre] column, index and foreign-key.
    -- * Rename [song] to track.
    -- * Rename [song].[Length] column.
    -- * Alter [artist].[Bio] column.
    -- * Syntax specific script was added.
    -- * Toggle [artist] and [song] auto-increment.
    

-- If you're reading this, the syntax is MySQL.

