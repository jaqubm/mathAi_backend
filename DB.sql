CREATE SCHEMA mathAi;

CREATE TABLE mathAi.[User]
(
    Email NVARCHAR(255) NOT NULL PRIMARY KEY ,  -- Corresponds to the 'Email' property
    Name NVARCHAR(255) NOT NULL,                -- Corresponds to the 'Name' property
    
    IsTeacher BIT NOT NULL DEFAULT 0,           -- Corresponds to the 'IsTeacher' property
    
    FirstTimeSignIn BIT NOT NULL DEFAULT 1,     -- Corresponds to the 'FirstTimeSignIn' property
);

CREATE TABLE mathAi.[ExerciseSet]
(
    ExerciseSetId NVARCHAR(255) NOT NULL PRIMARY KEY  -- Corresponds to ExerciseSetId
);

CREATE TABLE mathAi.[Exercise]
(
    ExerciseId NVARCHAR(255) NOT NULL PRIMARY KEY,    -- Corresponds to ExerciseId
    
    ExerciseContent NVARCHAR(MAX) NOT NULL,           -- Corresponds to ExerciseContent
    
    FirstExerciseHint NVARCHAR(MAX) NOT NULL,         -- Corresponds to FirstExerciseHint
    SecondExerciseHint NVARCHAR(MAX) NOT NULL,        -- Corresponds to SecondExerciseHint
    ThirdExerciseHint NVARCHAR(MAX) NOT NULL,         -- Corresponds to ThirdExerciseHint
    
    ExerciseSolution NVARCHAR(MAX) NOT NULL,          -- Corresponds to ExerciseSolution
    
    ExerciseSetId NVARCHAR(255) NOT NULL,             -- Foreign key to link the Exercise to an ExerciseSet

    CONSTRAINT FK_ExerciseSet_Exercise FOREIGN KEY (ExerciseSetId)
        REFERENCES mathAi.[ExerciseSet](ExerciseSetId)
        ON DELETE CASCADE
);

DROP TABLE mathAi.[User];
DROP TABLE mathAi.[ExerciseSet];
DROP TABLE mathAi.[Exercise];
