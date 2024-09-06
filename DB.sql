CREATE SCHEMA mathAi;

CREATE TABLE mathAi.[User]
(
    UserId NVARCHAR(255) NOT NULL PRIMARY KEY,  -- Corresponds to the 'userId' property
    Name NVARCHAR(255) NOT NULL,                -- Corresponds to the 'name' property
    Email NVARCHAR(255) NOT NULL,               -- Corresponds to the 'email' property
    Picture NVARCHAR(MAX) NULL                  -- Corresponds to the 'picture' property, allowing NULL values
);
