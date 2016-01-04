﻿/*
 * SCHEMA:		example inc.
 * VERSION:		1.0.0.0
 * AUTHOR:		john@example.com
 * GENERATED:	Jan 02, 2016
*/

-- -----------------------------------
-- GENRE TABLE
-- -----------------------------------
DROP TABLE IF EXISTS [genre];
CREATE TABLE IF NOT EXISTS [genre]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(64) 
);


-- -----------------------------------
-- ARTIST TABLE
-- -----------------------------------
DROP TABLE IF EXISTS [artist];
CREATE TABLE IF NOT EXISTS [artist]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Name] VARCHAR(64) NOT NULL,
	[Bio] TEXT 
);


-- -----------------------------------
-- ALBUM TABLE
-- -----------------------------------
DROP TABLE IF EXISTS [album];
CREATE TABLE IF NOT EXISTS [album]
(
	[Artist_Id] INTEGER NOT NULL,
	[Name] VARCHAR(64) NOT NULL,
	[Release_Date] DATE ,
	PRIMARY KEY ([Artist_Id] ASC, [Name] ASC),
	FOREIGN KEY ([Artist_Id]) REFERENCES [artist] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE
);


-- -----------------------------------
-- SONG TABLE
-- -----------------------------------
DROP TABLE IF EXISTS [song];
CREATE TABLE IF NOT EXISTS [song]
(
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Album_Id] INTEGER NOT NULL,
	[Artist_Id] INTEGER NOT NULL,
	[Genre_Id] INTEGER NOT NULL,
	[Name] VARCHAR(64) NOT NULL,
	[Length] DECIMAL(4, 2) ,
	[Price] DECIMAL(12, 2) NOT NULL,
	[On_Device] BOOLEAN NOT NULL DEFAULT '0',
	FOREIGN KEY ([Genre_Id]) REFERENCES [genre] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY ([Artist_Id]) REFERENCES [artist] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE
);



    /* *** SEED *** */

    INSERT INTO genre (Id, Name) VALUES
    ('1', 'Rap'),
    ('2', 'Pop'),
    ('3', 'Rock');

    /* *** Arist Data *** */
    insert into artist (Name, Bio) values ('John Stone', 'Duis aliquam convallis nunc. Proin at turpis a pede posuere nonummy. Integer non velit.');
    insert into artist (Name, Bio) values ('Thomas Perkins', 'Suspendisse potenti. In eleifend quam a odio. In hac habitasse platea dictumst.');
    insert into artist (Name, Bio) values ('Christopher Green', 'Curabitur gravida nisi at nibh. In hac habitasse platea dictumst. Aliquam augue quam, sollicitudin vitae, consectetuer eget, rutrum at, lorem.');
    insert into artist (Name, Bio) values ('John Morales', 'Curabitur at ipsum ac tellus semper interdum. Mauris ullamcorper purus sit amet nulla. Quisque arcu libero, rutrum ac, lobortis vel, dapibus at, diam.');
    insert into artist (Name, Bio) values ('Doris Robertson', 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Proin risus. Praesent lectus.

    Vestibulum quam sapien, varius ut, blandit non, interdum in, ante. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Duis faucibus accumsan odio. Curabitur convallis.');
    insert into artist (Name, Bio) values ('Doris Palmer', 'Maecenas ut massa quis augue luctus tincidunt. Nulla mollis molestie lorem. Quisque ut erat.');
    insert into artist (Name, Bio) values ('Elizabeth Freeman', 'Vestibulum ac est lacinia nisi venenatis tristique. Fusce congue, diam id ornare imperdiet, sapien urna pretium nisl, ut volutpat sapien arcu sed augue. Aliquam erat volutpat.');
    insert into artist (Name, Bio) values ('Rachel Bailey', 'Nulla ut erat id mauris vulputate elementum. Nullam varius. Nulla facilisi.');
    insert into artist (Name, Bio) values ('Jason Collins', 'Maecenas tristique, est et tempus semper, est quam pharetra magna, ac consequat metus sapien ut nunc. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Mauris viverra diam vitae quam. Suspendisse potenti.

    Nullam porttitor lacus at turpis. Donec posuere metus vitae ipsum. Aliquam non mauris.');
    insert into artist (Name, Bio) values ('Joan Burke', 'Aliquam quis turpis eget elit sodales scelerisque. Mauris sit amet eros. Suspendisse accumsan tortor quis turpis.');

    /* *** Album Data *** */
    insert into album (Artist_Id, Name, Release_Date) values (8, 'tempus', '2015-09-12');
    insert into album (Artist_Id, Name, Release_Date) values (9, 'id', '2015-11-18');
    insert into album (Artist_Id, Name, Release_Date) values (2, 'mauris', '2015-10-14');
    insert into album (Artist_Id, Name, Release_Date) values (4, 'quam', '2015-09-24');
    insert into album (Artist_Id, Name, Release_Date) values (7, 'nunc nisl', '2015-02-11');
    insert into album (Artist_Id, Name, Release_Date) values (4, 'suspendisse potenti', '2015-08-24');
    insert into album (Artist_Id, Name, Release_Date) values (5, 'morbi', '2015-02-07');
    insert into album (Artist_Id, Name, Release_Date) values (6, 'vel nisl', '2015-02-11');
    insert into album (Artist_Id, Name, Release_Date) values (1, 'elementum eu', '2015-02-24');
    insert into album (Artist_Id, Name, Release_Date) values (3, 'amet lobortis', '2015-11-10');
    insert into album (Artist_Id, Name, Release_Date) values (10, 'sollicitudin ut', '2015-08-23');
    insert into album (Artist_Id, Name, Release_Date) values (6, 'lorem', '2015-01-20');
    insert into album (Artist_Id, Name, Release_Date) values (7, 'integer', '2015-11-04');
    insert into album (Artist_Id, Name, Release_Date) values (1, 'sit', '2015-11-06');
    insert into album (Artist_Id, Name, Release_Date) values (6, 'volutpat dui', '2015-02-22');

    /* *** Song Data *** */
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 1, 3, 'enim leo', 1.31, 37, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 1, 1, 'duis', 1.98, 68, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 5, 2, 'vivamus', 1.04, 86, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 10, 1, 'eu', 1.53, 30, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 6, 2, 'justo', 1.18, 99, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 6, 3, 'non', 1.38, 55, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 9, 2, 'ante', 2.0, 85, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 3, 2, 'eget', 1.94, 27, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 1, 2, 'sed', 1.41, 80, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 4, 1, 'felis ut', 1.21, 13, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 1, 2, 'sed magna', 1.66, 89, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 2, 1, 'semper interdum', 1.77, 76, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 10, 3, 'turpis', 1.49, 83, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 9, 2, 'augue a', 1.64, 97, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 2, 3, 'fusce', 1.56, 93, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 4, 2, 'in', 1.56, 36, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 7, 1, 'felis', 1.85, 42, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 2, 3, 'sed', 1.41, 50, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 6, 1, 'consectetuer', 1.03, 88, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 8, 3, 'volutpat quam', 1.14, 42, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 6, 2, 'in', 1.77, 39, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 5, 2, 'egestas metus', 1.27, 99, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 1, 3, 'fermentum donec', 1.19, 56, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 3, 1, 'metus', 1.06, 44, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 2, 3, 'nibh ligula', 1.76, 97, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 2, 2, 'vehicula condimentum', 1.22, 90, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 5, 2, 'in consequat', 1.62, 20, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 4, 1, 'neque', 1.84, 80, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 4, 3, 'in lacus', 1.02, 93, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 9, 1, 'pulvinar', 1.31, 8, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 1, 3, 'ut erat', 1.39, 78, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 4, 2, 'integer ac', 1.47, 25, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 1, 2, 'et ultrices', 1.57, 27, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 10, 2, 'varius', 1.55, 37, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 10, 2, 'nunc', 1.6, 98, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 10, 3, 'vitae consectetuer', 1.11, 64, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 6, 2, 'vel', 1.52, 99, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 1, 2, 'quis', 1.86, 30, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 5, 2, 'est et', 1.18, 96, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 7, 1, 'donec', 1.9, 8, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 8, 1, 'dapibus dolor', 1.2, 3, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 7, 3, 'erat', 1.61, 10, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 7, 2, 'orci', 1.21, 36, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 6, 1, 'arcu libero', 1.46, 42, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 6, 1, 'adipiscing', 1.78, 51, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 2, 3, 'est', 1.16, 85, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 10, 3, 'odio', 1.56, 23, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 9, 3, 'aliquet pulvinar', 1.81, 60, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 5, 3, 'mi pede', 1.72, 63, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 6, 3, 'suspendisse accumsan', 1.38, 79, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 4, 1, 'elementum', 1.97, 43, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 7, 3, 'dolor', 1.34, 52, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 5, 2, 'montes nascetur', 1.18, 28, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 3, 2, 'neque duis', 1.54, 91, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 9, 3, 'nulla', 1.18, 26, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 10, 1, 'eu massa', 1.44, 95, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 3, 2, 'sit amet', 1.17, 54, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 4, 2, 'platea', 1.18, 47, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 9, 3, 'nullam', 1.68, 13, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 2, 2, 'proin', 1.37, 43, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 8, 2, 'eu felis', 1.73, 13, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 10, 2, 'ut mauris', 1.85, 96, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 8, 3, 'lacus curabitur', 1.54, 25, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 8, 1, 'est', 1.98, 35, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 10, 2, 'luctus', 1.72, 19, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 4, 3, 'sociis natoque', 1.08, 81, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 7, 2, 'nam dui', 1.93, 17, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 8, 3, 'at ipsum', 1.79, 23, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 9, 1, 'ultrices posuere', 1.58, 26, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 8, 1, 'donec', 1.17, 75, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 10, 1, 'suspendisse', 1.74, 43, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 6, 2, 'ac', 1.35, 41, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 10, 2, 'ultrices libero', 1.1, 61, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 9, 1, 'risus', 1.43, 6, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 10, 1, 'vestibulum', 1.17, 32, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 7, 2, 'nullam porttitor', 1.7, 78, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 10, 3, 'ligula vehicula', 1.07, 94, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 4, 1, 'nunc viverra', 1.39, 78, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 1, 1, 'tortor sollicitudin', 1.83, 95, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 2, 1, 'in', 1.75, 1, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 9, 3, 'parturient', 1.51, 42, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 8, 2, 'magna', 1.01, 49, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 5, 2, 'ut nunc', 1.69, 20, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 8, 2, 'donec', 1.75, 61, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 10, 3, 'suspendisse potenti', 1.77, 49, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 6, 1, 'ipsum', 1.98, 57, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 3, 2, 'scelerisque', 1.69, 98, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 3, 2, 'nunc nisl', 1.67, 58, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 9, 3, 'porttitor lacus', 1.53, 72, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 1, 2, 'nisi eu', 1.11, 71, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 10, 2, 'lobortis est', 1.04, 14, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 9, 1, 'varius nulla', 1.5, 2, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 2, 2, 'elit', 1.5, 17, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 8, 3, 'adipiscing elit', 1.48, 43, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 8, 2, 'odio', 1.74, 7, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 3, 3, 'vulputate ut', 1.76, 46, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 7, 1, 'orci luctus', 1.9, 87, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 6, 2, 'neque', 1.8, 2, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 6, 1, 'tellus nisi', 1.73, 91, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 4, 2, 'suspendisse', 1.53, 57, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 10, 1, 'platea dictumst', 1.81, 33, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 4, 3, 'in', 1.49, 6, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 3, 2, 'vivamus', 1.42, 30, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 6, 1, 'at dolor', 1.13, 17, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 8, 1, 'rhoncus', 1.3, 43, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 1, 2, 'turpis', 1.34, 13, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 9, 1, 'in sagittis', 1.91, 24, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 5, 1, 'ultricies', 1.86, 8, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 2, 1, 'cras', 1.03, 37, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 10, 2, 'congue', 1.22, 4, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 1, 1, 'curae', 1.18, 74, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 8, 2, 'id pretium', 1.06, 48, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 5, 1, 'justo', 1.26, 73, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 1, 2, 'donec dapibus', 1.42, 41, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 1, 1, 'mauris morbi', 1.13, 45, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 3, 2, 'ante vel', 1.83, 85, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 4, 1, 'eget', 1.78, 40, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 1, 3, 'ipsum', 1.86, 30, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 10, 2, 'dapibus', 1.77, 78, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 8, 3, 'nulla', 2.0, 51, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 8, 3, 'mauris', 1.52, 17, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 10, 3, 'volutpat', 1.08, 97, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 3, 2, 'nulla', 1.11, 15, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 9, 1, 'hac', 1.2, 91, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 2, 3, 'quam', 1.28, 12, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 8, 3, 'augue', 1.43, 42, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 9, 1, 'donec diam', 1.28, 21, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 9, 3, 'praesent blandit', 1.39, 9, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 6, 1, 'aenean', 1.12, 88, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 1, 1, 'ac est', 1.08, 74, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 1, 1, 'nam', 1.2, 81, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 8, 2, 'ullamcorper', 1.88, 60, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 8, 1, 'sed augue', 1.97, 82, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 7, 2, 'nibh in', 1.91, 45, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 3, 1, 'duis faucibus', 1.69, 88, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 4, 3, 'suspendisse ornare', 1.17, 54, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 10, 1, 'sapien', 1.42, 41, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 8, 2, 'phasellus in', 1.45, 45, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 3, 2, 'lacus', 1.22, 77, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 10, 1, 'pellentesque volutpat', 1.34, 22, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 9, 1, 'euismod scelerisque', 1.21, 15, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 8, 1, 'eget congue', 1.4, 72, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 3, 2, 'donec ut', 1.26, 70, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 2, 2, 'vestibulum', 1.31, 55, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 6, 3, 'condimentum', 1.16, 89, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 6, 3, 'nam ultrices', 1.63, 98, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 8, 3, 'quam', 1.52, 58, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 9, 2, 'vestibulum proin', 1.66, 4, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 9, 3, 'nisi', 1.09, 24, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 3, 3, 'aliquet', 1.32, 24, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 1, 2, 'augue', 1.32, 75, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 10, 1, 'duis consequat', 1.79, 91, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 8, 2, 'imperdiet', 1.73, 65, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 5, 1, 'nisi', 1.68, 1, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 1, 3, 'tincidunt ante', 1.34, 16, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 9, 1, 'mattis pulvinar', 1.24, 69, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 5, 3, 'luctus tincidunt', 1.33, 7, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 9, 2, 'praesent', 1.82, 8, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 10, 3, 'turpis nec', 1.81, 60, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 4, 1, 'lobortis', 1.6, 80, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 3, 1, 'pretium iaculis', 1.37, 60, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 3, 2, 'sapien a', 1.58, 34, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 4, 1, 'congue etiam', 1.28, 95, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 9, 1, 'vitae ipsum', 1.53, 69, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 1, 2, 'vulputate nonummy', 1.43, 30, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 9, 1, 'et', 1.05, 34, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 2, 2, 'sapien', 1.39, 35, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 5, 2, 'amet', 1.84, 60, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 5, 1, 'integer', 1.33, 26, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 2, 3, 'porta volutpat', 1.46, 6, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 9, 1, 'at', 1.11, 22, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 3, 3, 'odio', 1.65, 53, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 1, 3, 'sollicitudin ut', 1.46, 91, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 7, 3, 'consequat ut', 1.09, 16, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 3, 3, 'posuere felis', 1.6, 51, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 1, 3, 'urna ut', 1.14, 34, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 7, 2, 'vestibulum ac', 1.67, 83, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 9, 1, 'congue diam', 1.03, 13, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 7, 3, 'integer', 1.61, 19, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 2, 3, 'erat vestibulum', 1.63, 54, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 4, 3, 'nulla sed', 1.61, 32, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 8, 1, 'sed vel', 1.12, 61, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 1, 3, 'volutpat', 1.01, 35, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 8, 1, 'luctus', 1.83, 90, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 7, 3, 'in', 1.21, 34, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 4, 1, 'ipsum dolor', 1.49, 42, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 2, 1, 'accumsan', 1.66, 43, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 3, 1, 'rutrum', 1.4, 39, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 5, 1, 'ligula', 1.24, 1, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 4, 3, 'curae nulla', 1.26, 95, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 9, 3, 'nulla suscipit', 1.02, 55, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 6, 2, 'blandit', 1.71, 8, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 6, 1, 'ullamcorper', 1.57, 2, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 4, 1, 'eu interdum', 1.24, 37, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 2, 2, 'ut', 1.37, 52, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 10, 3, 'mauris eget', 1.33, 85, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 5, 1, 'a', 1.9, 12, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 7, 2, 'cum', 1.73, 35, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 6, 3, 'at', 1.13, 41, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 6, 1, 'erat quisque', 1.68, 78, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 2, 3, 'arcu adipiscing', 1.16, 15, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 4, 3, 'ultrices', 1.26, 27, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 1, 2, 'maecenas ut', 1.85, 30, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 8, 3, 'eu', 1.45, 62, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 4, 2, 'luctus et', 1.74, 35, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 7, 3, 'nullam', 1.49, 39, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 9, 2, 'ut', 1.38, 53, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 4, 2, 'donec ut', 1.32, 10, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 6, 2, 'ipsum', 1.7, 38, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 10, 1, 'commodo', 1.5, 97, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 3, 3, 'orci nullam', 1.95, 33, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 7, 1, 'etiam faucibus', 1.89, 41, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 5, 1, 'justo', 1.7, 2, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 3, 3, 'nunc', 1.0, 67, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 6, 3, 'diam neque', 1.15, 81, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 6, 2, 'massa', 1.12, 76, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 6, 3, 'consequat varius', 1.76, 79, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 7, 2, 'enim', 1.18, 88, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 7, 1, 'vel', 1.54, 70, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (3, 3, 3, 'gravida', 1.06, 58, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 3, 1, 'semper', 1.86, 9, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 7, 3, 'mattis', 1.79, 89, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 3, 1, 'odio donec', 1.32, 23, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 8, 2, 'at', 1.43, 72, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 2, 2, 'eu interdum', 1.6, 40, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 9, 1, 'ac', 1.23, 3, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 2, 3, 'blandit non', 1.63, 47, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 7, 3, 'aliquam erat', 1.4, 41, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (7, 4, 1, 'vivamus', 1.42, 35, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 5, 2, 'aliquam lacus', 1.97, 84, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 5, 2, 'viverra pede', 1.72, 25, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 8, 2, 'sit', 1.75, 42, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 8, 2, 'libero', 1.97, 1, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 10, 3, 'eros elementum', 1.7, 55, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 6, 3, 'hac', 1.39, 51, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 8, 1, 'id turpis', 1.75, 81, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 3, 1, 'integer', 1.93, 68, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (2, 7, 2, 'amet nulla', 1.9, 14, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 3, 1, 'erat eros', 1.05, 8, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (5, 1, 3, 'amet', 1.14, 67, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 10, 2, 'pellentesque', 1.45, 28, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 2, 2, 'dui', 1.56, 89, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (8, 9, 3, 'curabitur gravida', 1.03, 32, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (6, 9, 3, 'quis', 1.43, 16, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (10, 9, 3, 'ridiculus mus', 1.3, 41, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 8, 2, 'scelerisque', 1.48, 58, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 10, 2, 'turpis', 1.12, 89, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (9, 8, 1, 'ultrices', 1.22, 99, 0);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (1, 8, 2, 'nibh in', 1.42, 94, 1);
    insert into song (Album_Id, Artist_Id, Genre_Id, Name, Length, Price, On_Device) values (4, 6, 3, 'ipsum', 1.06, 38, 1);
