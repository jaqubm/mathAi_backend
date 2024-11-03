CREATE DATABASE [mathAi-DB] GO;

CREATE SCHEMA mathAi;


CREATE TABLE mathAi.[User]
(
    Email NVARCHAR(255) NOT NULL PRIMARY KEY,  -- Unique identifier for the user (Email)
    Name NVARCHAR(255) NOT NULL,               -- Name of the user
    IsTeacher BIT NOT NULL DEFAULT 0,          -- Boolean flag indicating whether the user is a teacher
    FirstTimeSignIn BIT NOT NULL DEFAULT 1     -- Boolean flag to check if it's the user's first sign-in
);

CREATE TABLE mathAi.[ExerciseSet]
(
    Id NVARCHAR(50) NOT NULL PRIMARY KEY,     -- Unique identifier for the exercise set
    Name NVARCHAR(255) NOT NULL,               -- Name of the exercise set
    SchoolType NVARCHAR(50) NOT NULL,         -- Type of school (e.g., "Primary", "High School")
    Grade INT NOT NULL,                        -- Grade level associated with the exercise set
    Subject NVARCHAR(255) NOT NULL,            -- Subject of the exercise set (e.g., "Math")
    UserId NVARCHAR(255),                      -- Reference to the teacher (User) who created the set

    CONSTRAINT FK_User_ExerciseSet FOREIGN KEY (UserId)
        REFERENCES mathAi.[User](Email)
        ON DELETE SET NULL
);

CREATE TABLE mathAi.[Exercise]
(
    Id NVARCHAR(50) NOT NULL PRIMARY KEY,     -- Unique identifier for the exercise
    Content NVARCHAR(MAX) NOT NULL,            -- The content or description of the exercise
    FirstHint NVARCHAR(MAX) NOT NULL,          -- First hint for the exercise
    SecondHint NVARCHAR(MAX) NOT NULL,         -- Second hint for the exercise
    ThirdHint NVARCHAR(MAX) NOT NULL,          -- Third hint for the exercise
    Solution NVARCHAR(MAX) NOT NULL,           -- Solution to the exercise
    ExerciseSetId NVARCHAR(50) NOT NULL,      -- Foreign key linking the exercise to an exercise set

    CONSTRAINT FK_ExerciseSet_Exercise FOREIGN KEY (ExerciseSetId)
        REFERENCES mathAi.[ExerciseSet](Id)
        ON DELETE CASCADE
);

CREATE TABLE mathAi.[Class]
(
    Id NVARCHAR(50) NOT NULL PRIMARY KEY,     -- Unique identifier for the class
    Name NVARCHAR(255) NOT NULL,               -- Name of the class
    OwnerId NVARCHAR(255) NOT NULL,            -- Foreign key linking the class to the teacher (User)

    CONSTRAINT FK_User_Class_Owner FOREIGN KEY (OwnerId)
        REFERENCES mathAi.[User](Email)
        ON DELETE CASCADE
);

CREATE TABLE mathAi.[ClassStudent]
(
    Id INT IDENTITY(1,1) PRIMARY KEY,          -- Surrogate key as primary key
    ClassId NVARCHAR(50) NOT NULL,            -- Foreign key linking the class
    StudentId NVARCHAR(255) NOT NULL,          -- Foreign key linking the student to a user (User)

    CONSTRAINT FK_Class_ClassStudent FOREIGN KEY (ClassId)
        REFERENCES mathAi.[Class](Id)
        ON DELETE CASCADE,                     -- This remains if you want cascading deletes on Class

    CONSTRAINT FK_User_ClassStudent FOREIGN KEY (StudentId)
        REFERENCES mathAi.[User](Email)
        ON DELETE NO ACTION,                   -- Use NO ACTION to prevent multiple cascade paths

    UNIQUE (ClassId, StudentId)                -- Ensures unique ClassId + StudentId combination
);


CREATE TABLE mathAi.[Assignment]
(
    Id NVARCHAR(50) NOT NULL PRIMARY KEY,     -- Unique identifier for the assignment
    Name NVARCHAR(255) NOT NULL,               -- Name of the assignment
    StartDate DATE NOT NULL,                   -- Start date of the assignment
    DueDate DATE NOT NULL,                     -- Due date of the assignment
    ClassId NVARCHAR(50) NOT NULL,            -- Foreign key linking the assignment to a class
    ExerciseSetId NVARCHAR(50) NOT NULL,      -- Foreign key linking the assignment to an exercise set

    CONSTRAINT FK_Class_Assignment FOREIGN KEY (ClassId)
        REFERENCES mathAi.[Class](Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_ExerciseSet_Assignment FOREIGN KEY (ExerciseSetId)
        REFERENCES mathAi.[ExerciseSet](Id)
        ON DELETE CASCADE
);


CREATE TABLE mathAi.[AssignmentSubmission]
(
    Id NVARCHAR(50) NOT NULL PRIMARY KEY,         -- Unique identifier for each submission
    AssignmentId NVARCHAR(50) NOT NULL,           -- Foreign key linking to the Assignment table
    StudentId NVARCHAR(255) NOT NULL,             -- Foreign key linking to the User table (for students only)
    SubmissionDate DATETIME,                      -- Timestamp for when the assignment was submitted
    Completed BIT NOT NULL DEFAULT 0,             -- Status flag to check if the assignment is completed

    CONSTRAINT FK_Assignment_Submission FOREIGN KEY (AssignmentId)
        REFERENCES mathAi.[Assignment](Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_User_Submission FOREIGN KEY (StudentId)
        REFERENCES mathAi.[User](Email)
        ON DELETE NO ACTION  -- Prevents cascading delete to avoid conflict
);


CREATE TABLE mathAi.[ExerciseAnswers]
(
    Id NVARCHAR(50) NOT NULL PRIMARY KEY,          -- Unique identifier for each answer entry
    AssignmentSubmissionId NVARCHAR(50) NOT NULL,  -- Foreign key linking to the AssignmentSubmissions table
    ExerciseId NVARCHAR(50) NOT NULL,              -- Foreign key linking to the Exercise table
    StudentAnswer NVARCHAR(MAX),                   -- The student's answer to the exercise
    Grade INT,                                     -- Grade given to the student's answer for the exercise
    Feedback NVARCHAR(MAX),                        -- Optional feedback from the teacher
    AnsweredDate DATETIME,                         -- Timestamp of when the student submitted their answer

    CONSTRAINT FK_Submission_ExerciseAnswer FOREIGN KEY (AssignmentSubmissionId)
        REFERENCES mathAi.[AssignmentSubmission](Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_Exercise_ExerciseAnswer FOREIGN KEY (ExerciseId)
        REFERENCES mathAi.[Exercise](Id)
        ON DELETE NO ACTION  -- Prevents cascading delete to avoid conflict
);


DROP TABLE mathAi.[ExerciseAnswers];
DROP TABLE mathAi.[AssignmentSubmission];
DROP TABLE mathAi.[Assignment];
DROP TABLE mathAi.[ClassStudent];
DROP TABLE mathAi.[Class];
DROP TABLE mathAi.[Exercise];
DROP TABLE mathAi.[ExerciseSet];
DROP TABLE mathAi.[User];
