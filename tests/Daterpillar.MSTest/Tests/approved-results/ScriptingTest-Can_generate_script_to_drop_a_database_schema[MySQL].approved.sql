SYNTAX: MySQL


DROP TABLE `zombie`;

ALTER TABLE `service` DROP FOREIGN KEY `service_Zombie_fk_TO_placeholder_Id__fk`;

ALTER TABLE `service` DROP COLUMN `Zombie`;

DROP INDEX `service__Subscribers_index` ON `service`;

