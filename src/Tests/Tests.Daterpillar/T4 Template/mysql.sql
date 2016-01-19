/*
 * SCHEMA:		example inc.
 * VERSION:		1.0.0.0
 * AUTHOR:		john@example.com
 * GENERATED:	Dec 24, 2015
*/

CREATE DATABASE IF NOT EXISTS `zune`;
USE `zune`;

-- ------------------------------
-- EMPLOYEE TABLE
-- ------------------------------
CREATE TABLE IF NOT EXISTS `employee`
(
	`Id` INT not null COMMENT '',
	`birth_date` DATE not null COMMENT '',
	`first_name` VARCHAR(14) not null COMMENT '',
	`last_name` VARCHAR(16) not null COMMENT '',
	`gender` INT not null COMMENT '',
	`hire_date` DATE not null COMMENT '',
	PRIMARY KEY (`Id` ASC)
);

-- ------------------------------
-- SALARY TABLE
-- ------------------------------
CREATE TABLE IF NOT EXISTS `salary`
(
	`Employee_Id` INT not null COMMENT '',
	`salary` DECIMAL(12, 2) not null default 0 COMMENT '',
	`from_date` DATE not null COMMENT '',
	`to_date` DATE not null COMMENT '',
	PRIMARY KEY (`from_date` ASC, `Employee_Id` ASC),
	FOREIGN KEY (`Employee_Id`) REFERENCES `employee`(`Id`) ON UPDATE CASCADE ON DELETE CASCADE
);

-- ------------------------------
-- ARTIST TABLE
-- ------------------------------
CREATE TABLE IF NOT EXISTS `artist`
(
	`Id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT '',
	`Name` VARCHAR(64) NOT NULL COMMENT '',
	`Bio` TEXT  COMMENT ''
);

-- ------------------------------
-- GENRE TABLE
-- ------------------------------
CREATE TABLE IF NOT EXISTS `genre`
(
	`Id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT '',
	`Name` VARCHAR(64)  COMMENT ''
);

-- ------------------------------
-- ALBUM TABLE
-- ------------------------------
CREATE TABLE IF NOT EXISTS `album`
(
	`Artist_Id` INT NOT NULL COMMENT '',
	`Name` VARCHAR(64) NOT NULL COMMENT '',
	PRIMARY KEY (`Artist_Id` ASC, `Name` ASC),
	FOREIGN KEY (`Artist_Id`) REFERENCES `artist`(`Id`) ON UPDATE  ON DELETE 
);

-- ------------------------------
-- SONG TABLE
-- ------------------------------
CREATE TABLE IF NOT EXISTS `song`
(
	`Id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT COMMENT '',
	`Name` VARCHAR(64) NOT NULL COMMENT '',
	`Length` DECIMAL(4, 2)  COMMENT '',
	`Price` DECIMAL(12, 2) NOT NULL COMMENT '',
	`Album_Id` INT NOT NULL COMMENT '',
	`Artist_Id` INT NOT NULL COMMENT '',
	`Genre_Id` INT NOT NULL COMMENT '',
	FOREIGN KEY (`Genre_Id`) REFERENCES `Genre`(`Id`) ON UPDATE  ON DELETE ,
	FOREIGN KEY (`Album_Id`) REFERENCES `album`(`Id`) ON UPDATE  ON DELETE ,
	FOREIGN KEY (`Artist_Id`) REFERENCES `artist`(`Id`) ON UPDATE  ON DELETE 
);
