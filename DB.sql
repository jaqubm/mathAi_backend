CREATE SCHEMA mathAi;

CREATE TABLE mathAi.[User]
(
    Email NVARCHAR(255) NOT NULL PRIMARY KEY ,  -- Corresponds to the 'Email' property
    Name NVARCHAR(255) NOT NULL,                -- Corresponds to the 'Name' property
    IsTeacher BIT NOT NULL DEFAULT 0,           -- Corresponds to the 'IsTeacher' property
);

DROP TABLE mathAi.[User];
