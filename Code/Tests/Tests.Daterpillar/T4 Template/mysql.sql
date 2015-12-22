/*
 * SCHEMA:		example inc.
 * VERSION:		1.0.0.0
 * AUTHOR:		john@example.com
 * GENERATED:	Dec 22, 2015
*/

CREATE DATABASE IF NOT EXISTS `daterpillar`;
USE `daterpillar`;

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
	`amount` DECIMAL(12, 2) not null default 0 COMMENT '',
	`from_date` DATE not null COMMENT '',
	`to_date` DATE not null COMMENT '',
	PRIMARY KEY (`from_date` ASC, `Employee_Id` ASC),
	FOREIGN KEY (`Employee_Id`) REFERENCES `employee`(`Id`) ON UPDATE CASCADE ON DELETE CASCADE
);
