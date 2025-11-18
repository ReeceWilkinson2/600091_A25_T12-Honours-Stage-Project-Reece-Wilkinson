CREATE SCHEMA IF NOT EXISTS `HonoursSQL`;
USE `HonoursSQL`;

CREATE TABLE Departments
(
	Department varchar(10) NOT NULL PRIMARY KEY,
    CONSTRAINT UniqueIDs UNIQUE (Department)
);

CREATE TABLE Staff
(
	StaffID varchar(10) NOT NULL PRIMARY KEY,
    Username varchar(20) NOT NULL,
    `Password` varchar(20) NOT NULL,
	SEmail varchar(50) NOT NULL,
    Surname varchar(40),
    Forenames varchar(50) NOT NULL DEFAULT '',
    `Role` varchar(10) NOT NULL,
    CONSTRAINT UniqueIDs UNIQUE (StaffID)
);

CREATE TABLE Students
(
	StudentID varchar(10) NOT NULL PRIMARY KEY,
    Username varchar(20) NOT NULL,
    `Password` varchar(20) NOT NULL,
	StEmail varchar(50) NOT NULL,
    Surname varchar(40),
    Forenames varchar(50) NOT NULL DEFAULT '',
    `Role` varchar(10) NOT NULL,
    CONSTRAINT UniqueIDs UNIQUE (StudentID)
);

CREATE TABLE Employers
(
	EmployerID varchar(10) NOT NULL PRIMARY KEY,
    Username varchar(20) NOT NULL,
    `Password` varchar(20) NOT NULL,
	EmEmail varchar(50) NOT NULL,
    Surname varchar(40),
    Forenames varchar(50) NOT NULL DEFAULT '',
    PlaceOfEmployment varchar(50) NOT NULL DEFAULT '',
    `Role` varchar(10) NOT NULL,
    CONSTRAINT UniqueIDs UNIQUE (StudentID)
);