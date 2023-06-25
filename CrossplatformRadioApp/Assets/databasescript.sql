CREATE SCHEMA IF NOT EXISTS DB_NAME DEFAULT CHARACTER SET utf8 ;
USE DB_NAME ;

CREATE TABLE IF NOT EXISTS DB_NAME.`SavedFiles` (
        `Id` INT NOT NULL AUTO_INCREMENT,
        `FileName` VARCHAR(100) NOT NULL,
        `Format` VARCHAR(45) NOT NULL,
        `ByteCode` LONGBLOB NOT NULL,
        `DateOfSaving` DATETIME NOT NULL,
        PRIMARY KEY (`Id`))
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS DB_NAME.`Records` (
        `Id` INT NOT NULL AUTO_INCREMENT,
        `FileName` VARCHAR(45) NOT NULL,
        `DateOfRecord` DATETIME NOT NULL,
        PRIMARY KEY (`Id`))
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS DB_NAME.`RecordedIqData` (
        `Id` INT NOT NULL AUTO_INCREMENT,
        `RecordId` INT NOT NULL,
        `I` INT NOT NULL,
        `Q` INT NOT NULL,
        `DatetimeOfRecord` DATETIME(6) NOT NULL,
        PRIMARY KEY (`Id`),
        INDEX `Record_idx` (`RecordId` ASC) VISIBLE,
        CONSTRAINT `Record`
           FOREIGN KEY (`RecordId`)
               REFERENCES DB_NAME.`Records` (`Id`)
               ON DELETE NO ACTION
               ON UPDATE NO ACTION)
ENGINE = InnoDB;