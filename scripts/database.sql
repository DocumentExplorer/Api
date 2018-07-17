CREATE TABLE Users(
    
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    Username nvarchar(4) NOT NULL,
    Password nvarchar(200) NOT NULL,
    Salt nvarchar(200) NOT NULL,
    Role nvarchar(5) NOT NULL,
)

SELECT * FROM Users