SYNTAX: TSQL


EXEC sp_RENAME 'placeholder', 'publisher'
GO

EXEC sp_RENAME 'service.Zombie_fk', 'ActiveUsers', 'COLUMN'
GO

ALTER TABLE [service] ALTER COLUMN [Subscribers] INT;
ALTER TABLE [service] ADD DEFAULT 0 FOR [Subscribers];
