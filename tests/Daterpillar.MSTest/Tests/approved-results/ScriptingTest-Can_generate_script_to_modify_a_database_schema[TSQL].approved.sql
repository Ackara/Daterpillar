SYNTAX: TSQL


EXEC sp_RENAME 'placeholder', 'publisher'
GO

EXEC sp_RENAME 'service.Zombie_fk', 'ActiveUsers', 'COLUMN'
GO

-- Modifying [service].[Subscribers]

ALTER TABLE [service] ALTER COLUMN [Subscribers] INT NOT NULL;

ALTER TABLE [service] ADD DEFAULT 0 FOR [Subscribers];

-- End --

