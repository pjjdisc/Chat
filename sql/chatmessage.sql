CREATE TABLE `test`.`chatmessage` (
  `Mid` VARCHAR(50) NOT NULL,
  `UserId` VARCHAR(50) NOT NULL,
  `Message` NVARCHAR(1000) NOT NULL,
  `CreateDate` DATETIME NOT NULL DEFAULT now(),
  PRIMARY KEY (`Mid`));