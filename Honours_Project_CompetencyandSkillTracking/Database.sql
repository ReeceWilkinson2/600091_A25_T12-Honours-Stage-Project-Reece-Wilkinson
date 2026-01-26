-- Students table
CREATE TABLE IF NOT EXISTS Students (
    StudentID TEXT NOT NULL PRIMARY KEY,
    Username TEXT NOT NULL,
    Password TEXT NOT NULL,
    StEmail TEXT NOT NULL,
    Surname TEXT,
    Forenames TEXT NOT NULL DEFAULT '',
    Role TEXT NOT NULL
);

-- Staff table
CREATE TABLE IF NOT EXISTS Staff (
    StaffID TEXT NOT NULL PRIMARY KEY,
    Username TEXT NOT NULL,
    Password TEXT NOT NULL,
    Email TEXT NOT NULL,
    Surname TEXT,
    Forenames TEXT NOT NULL DEFAULT '',
    Role TEXT NOT NULL
);

-- Departments table
CREATE TABLE IF NOT EXISTS Departments (
    Department TEXT NOT NULL PRIMARY KEY
);

-- Courses table
CREATE TABLE IF NOT EXISTS Courses (
    CanvasCourseID INTEGER NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL,
    Syllabus TEXT,
    Term TEXT NOT NULL
);

-- Assignments table
CREATE TABLE IF NOT EXISTS Assignments (
    CanvasAssignmentId INTEGER NOT NULL PRIMARY KEY,
    CourseId INTEGER NOT NULL,
    Title TEXT,
    Description TEXT,
    DueDate TEXT,
    FOREIGN KEY (CourseId) REFERENCES Courses(CanvasCourseID)
);

-- Competency table
CREATE TABLE IF NOT EXISTS Competencies (
    CompetencyAssignmentId INTEGER NOT NULL PRIMARY KEY,
    CourseId INTEGER NOT NULL,
    Title TEXT,
    Levels INTEGER,
    Description TEXT,
    FOREIGN KEY (CourseId) REFERENCES Courses(CanvasCourseID)
);