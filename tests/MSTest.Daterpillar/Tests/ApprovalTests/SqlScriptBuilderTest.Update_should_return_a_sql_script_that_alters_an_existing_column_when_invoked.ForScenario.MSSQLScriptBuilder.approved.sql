ALTER TABLE [card] ALTER COLUMN [Name] VARCHAR(256) NOT NULL;
EXEC sp_rename 'card.Name', 'name_of_card', 'COLUMN';
