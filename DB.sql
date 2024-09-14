CREATE SCHEMA mathAi;


CREATE TABLE mathAi.[User]
(
    Email NVARCHAR(255) NOT NULL PRIMARY KEY,   -- Corresponds to 'Email' property
    Name NVARCHAR(255) NOT NULL,                -- Corresponds to 'Name' property
    IsTeacher BIT NOT NULL DEFAULT 0,           -- Corresponds to 'IsTeacher' property
    FirstTimeSignIn BIT NOT NULL DEFAULT 1      -- Corresponds to 'FirstTimeSignIn' property
);

CREATE TABLE mathAi.[ExerciseSet]
(
    Id NVARCHAR(255) NOT NULL PRIMARY KEY,      -- Corresponds to 'Id' property
    Name NVARCHAR(255) NOT NULL,                -- Corresponds to 'Name' property
    SchoolType NVARCHAR(255) NOT NULL,          -- Corresponds to 'SchoolType' property
    Grade INT NOT NULL,                         -- Corresponds to 'Grade' property
    Subject NVARCHAR(255) NOT NULL,             -- Corresponds to 'Subject' property
    UserId NVARCHAR(255),                       -- Foreign key to link the ExerciseSet to a User

    CONSTRAINT FK_User_ExerciseSet FOREIGN KEY (UserId)
        REFERENCES mathAi.[User](Email)
        ON DELETE SET NULL
);

CREATE TABLE mathAi.[Exercise]
(
    Id NVARCHAR(255) NOT NULL PRIMARY KEY,      -- Corresponds to 'Id' property
    Content NVARCHAR(MAX) NOT NULL,             -- Corresponds to 'Content' property
    FirstHint NVARCHAR(MAX) NOT NULL,           -- Corresponds to 'FirstHint' property
    SecondHint NVARCHAR(MAX) NOT NULL,          -- Corresponds to 'SecondHint' property
    ThirdHint NVARCHAR(MAX) NOT NULL,           -- Corresponds to 'ThirdHint' property
    Solution NVARCHAR(MAX) NOT NULL,            -- Corresponds to 'Solution' property
    ExerciseSetId NVARCHAR(255) NOT NULL,       -- Foreign key to link the Exercise to an ExerciseSet

    CONSTRAINT FK_ExerciseSet_Exercise FOREIGN KEY (ExerciseSetId)
        REFERENCES mathAi.[ExerciseSet](Id)
        ON DELETE CASCADE
);


DROP TABLE mathAi.[Exercise];
DROP TABLE mathAi.[ExerciseSet];
DROP TABLE mathAi.[User];
