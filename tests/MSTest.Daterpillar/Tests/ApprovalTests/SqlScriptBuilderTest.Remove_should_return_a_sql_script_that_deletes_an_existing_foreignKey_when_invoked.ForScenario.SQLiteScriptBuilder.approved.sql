/* DROP ([card_number].[key_with_custom_name]) SCRIPT */
PRAGMA foreign_keys = 0;
CREATE TABLE [card_number_temp_table] AS SELECT * FROM [card_number];
DROP TABLE [card_number];
CREATE TABLE IF NOT EXISTS [card_number]
(
	[Pack_Acronym] VARCHAR(32) NOT NULL,
	[Number] INTEGER NOT NULL,
	[Rarity_Id] INTEGER NOT NULL,
	[Art_Index] INTEGER NOT NULL,
	[Card_Id] INTEGER NOT NULL,
	[Pack_Id] INTEGER NOT NULL,
	CONSTRAINT [card_number_Card_Id_TO_card_Id] FOREIGN KEY ([Card_Id]) REFERENCES [card]([Id]),
	CONSTRAINT [card_number_Pack_Id_TO_pack_Id] FOREIGN KEY ([Pack_Id]) REFERENCES [pack]([Id])
);

CREATE INDEX IF NOT EXISTS [card_number_Pack_Acronym_and_Number_and_Rarity_Id_and_Art_Index] ON [card_number] ([Pack_Acronym] ASC, [Number] ASC, [Rarity_Id] ASC, [Art_Index] ASC);

INSERT INTO [card_number] ([Pack_Acronym], [Number], [Rarity_Id], [Art_Index], [Card_Id], [Pack_Id]) SELECT [Pack_Acronym], [Number], [Rarity_Id], [Art_Index], [Card_Id], [Pack_Id] FROM [card_number_temp_table];
DROP TABLE [card_number_temp_table];
PRAGMA foreign_keys = 1;
/* END SCRIPT */
