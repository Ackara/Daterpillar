/* DROP ([card_extras].[Trivia]) SCRIPT */
PRAGMA foreign_keys = 0;
CREATE TABLE [card_extras_temp_table] AS SELECT * FROM [card_extras];
DROP TABLE [card_extras];
CREATE TABLE IF NOT EXISTS [card_extras]
(
	[Card_Id] INTEGER NOT NULL,
	[Rulings] TEXT,
	[Tips] TEXT,
	[Passcode] INTEGER NOT NULL,
	CONSTRAINT [card_extras_Card_Id_TO_card_Id] FOREIGN KEY ([Card_Id]) REFERENCES [card]([Id])
);

CREATE INDEX IF NOT EXISTS [card_extras_Card_Id] ON [card_extras] ([Card_Id] ASC);

INSERT INTO [card_extras] ([Card_Id], [Rulings], [Tips], [Passcode]) SELECT [Card_Id], [Rulings], [Tips], [Passcode] FROM [card_extras_temp_table];
DROP TABLE [card_extras_temp_table];
PRAGMA foreign_keys = 1;
/* END SCRIPT */
