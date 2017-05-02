ALTER TABLE [rarity] ALTER COLUMN [Name] VARCHAR(256) NOT NULL;
EXEC sp_rename 'rarity.Name', 'name_of_entity', 'COLUMN';
