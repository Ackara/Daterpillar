ALTER TABLE `card` ADD FOREIGN KEY `card_Card_Type_Id_TO_card_type_Id` (`Card_Type_Id`) REFERENCES `card_type` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE;
