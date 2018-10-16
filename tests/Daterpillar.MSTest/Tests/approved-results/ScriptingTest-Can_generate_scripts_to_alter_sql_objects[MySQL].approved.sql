SYNTAX: MySQL


ALTER TABLE `placeholder` RENAME TO `publisher`;

ALTER TABLE `service` CHANGE COLUMN `Zombie_fk` `Zombie_fk` INT NOT NULL COMMENT '';

ALTER TABLE `service` CHANGE COLUMN `Subscribers` `Subscribers` INT NOT NULL DEFAULT 0 COMMENT 'Get or set the number of customers.';

