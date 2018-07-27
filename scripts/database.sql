CREATE TABLE Users(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    Username nvarchar(4) NOT NULL,
    Password nvarchar(200) NOT NULL,
    Salt nvarchar(200) NOT NULL,
    Role nvarchar(12) NOT NULL,
)

CREATE TABLE Orders(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    Number integer NOT NULL,
    ClientCountry nvarchar(2) NOT NULL,
    ClientIdentificationNumber nvarchar(20) NOT NULL,
    BrokerCountry nvarchar(2) NOT NULL,
    BrokerIdentificationNumber nvarchar(20) NOT NULL,
    Owner1Name nvarchar(4) NOT NULL,
    CreationDate datetime NOT NULL,
    InvoiceNumber integer,
    CMRId UNIQUEIDENTIFIER,
    IsCMRRequired bit NOT NULL,
    FVKId UNIQUEIDENTIFIER,
    IsFVKRequired bit NOT NULL,
    FVPId UNIQUEIDENTIFIER,
    IsFVPRequired bit NOT NULL,
    NIPId UNIQUEIDENTIFIER,
    IsNIPRequired bit NOT NULL,
    NotaId UNIQUEIDENTIFIER,
    IsNotaRequired bit NOT NULL,
    PPId UNIQUEIDENTIFIER,
    IsPPRequired bit NOT NULL,
    RKId UNIQUEIDENTIFIER,
    IsRKRequired bit NOT NULL,
    ZKId UNIQUEIDENTIFIER,
    IsZKRequired bit NOT NULL,
    ZPId UNIQUEIDENTIFIER,
    IsZPRequired bit NOT NULL
)

CREATE TABLE Permissions(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    CMR nvarchar(12) NOT NULL,
    FVK nvarchar(12) NOT NULL,
    FVP nvarchar(12) NOT NULL,
    NIP nvarchar(12) NOT NULL,
    Nota nvarchar(12) NOT NULL,
    PP nvarchar(12) NOT NULL,
    RK nvarchar(12) NOT NULL,
    ZK nvarchar(12) NOT NULL,
    ZP nvarchar(12) NOT NULL
)

CREATE TABLE Logs(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    Event nvarchar(50) NOT NULL,
    Date datetime NOT NULL,
    OrderId UNIQUEIDENTIFIER NOT NULL,
    Number integer NOT NULL,
    ClientCountry nvarchar(2) NOT NULL,
    ClientIdentificationNumber nvarchar(20) NOT NULL,
    BrokerCountry nvarchar(2) NOT NULL,
    BrokerIdentificationNumber nvarchar(20) NOT NULL,
    Owner1Name nvarchar(4) NOT NULL,
    Username nvarchar(4) NOT NULL,
    InvoiceNumber integer
)

CREATE TABLE Files(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    Path nvarchar(100) NOT NULL,
    OrderId UNIQUEIDENTIFIER,
    FileType nvarchar(4)
)

SELECT * FROM Users
SELECT * FROM Permissions
SELECT * FROM Logs
SELECT * FROM Orders

DROP TABLE Users
DROP TABLE Orders
DROP TABLE Logs

SET ANSI_WARNINGS  ON