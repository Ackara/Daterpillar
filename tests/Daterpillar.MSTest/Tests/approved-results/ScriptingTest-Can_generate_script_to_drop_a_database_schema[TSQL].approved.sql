SYNTAX: TSQL


DROP TABLE [zombie];

ALTER TABLE [service] DROP CONSTRAINT [service_Zombie_fk_TO_placeholder_Id__fk];

ALTER TABLE [service] DROP COLUMN [Zombie];

DROP INDEX [service].[service__Subscribers_index];

