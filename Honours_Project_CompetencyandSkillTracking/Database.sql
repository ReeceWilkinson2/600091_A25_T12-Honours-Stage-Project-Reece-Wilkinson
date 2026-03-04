-- Students table
CREATE TABLE IF NOT EXISTS Students (
    StudentID TEXT NOT NULL PRIMARY KEY,
    UserName TEXT NOT NULL,
    Password TEXT NOT NULL,
    StEmail TEXT NOT NULL,
    Surname TEXT,
    Forenames TEXT,
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

-- Employer table
CREATE TABLE IF NOT EXISTS Employers (
    EmployerID TEXT NOT NULL PRIMARY KEY,
    Username TEXT NOT NULL,
    Password TEXT NOT NULL,
    EmployerEmail TEXT NOT NULL,
    Surname TEXT,
    Forenames TEXT NOT NULL DEFAULT '',
    Role TEXT NOT NULL
);

-- Departments table
CREATE TABLE IF NOT EXISTS Departments (
    Department TEXT NOT NULL PRIMARY KEY
);

-- Modules table (Canvas Version)
CREATE TABLE IF NOT EXISTS Courses (
    CanvasCourseID INTEGER NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL,
    Syllabus TEXT,
    Term TEXT NOT NULL
);

-- Modules table (CSV Version)
CREATE TABLE IF NOT EXISTS Modules (
    DatabaseID INTEGER PRIMARY KEY AUTOINCREMENT,
    ModCode TEXT NOT NULL,
    Course TEXT NOT NULL,
    Programme TEXT NOT NULL,
    ModuleName TEXT NOT NULL,
    Route TEXT NOT NULL,
    Award TEXT NOT NULL,
    CBOYear TEXT NOT NULL,
    Trimester TEXT NOT NULL,
    SelectionStatus TEXT NOT NULL,
    Level TEXT NOT NULL,
    Credits TEXT NOT NULL,
    UNIQUE(ModCode)
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

-- CompetencyData table
CREATE TABLE IF NOT EXISTS CompetencyData (
    CompetencyDbID INTEGER PRIMARY KEY AUTOINCREMENT,
    CompetencyName TEXT NOT NULL,
    CompetencyID TEXT,
    AdditionalNotes TEXT
);

-- CompetencyLevels table
CREATE TABLE IF NOT EXISTS CompetencyLevels (
    LevelDbID INTEGER PRIMARY KEY AUTOINCREMENT,
    LevelNumber INTEGER NOT NULL,
    Description TEXT,
    ModCode TEXT NOT NULL,
    CompetencyDbID INTEGER NOT NULL,
    FOREIGN KEY (CompetencyDbID) 
        REFERENCES CompetencyData(CompetencyDbID)
        ON DELETE CASCADE,
    FOREIGN KEY (ModCode)
        REFERENCES Modules(ModCode)
);

-- Outcomes table
CREATE TABLE IF NOT EXISTS StudentCompetencies (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentId TEXT NOT NULL,
    CompetencyId INTEGER NOT NULL,
    Score REAL,
    Mastery INTEGER, -- 0 or 1
    FOREIGN KEY (StudentId) REFERENCES Students(StudentID),
    FOREIGN KEY (CompetencyId) REFERENCES Competencies(CompetencyAssignmentId)
);

-- AssignmentSubmission table
CREATE TABLE IF NOT EXISTS Submissions (
    Id INTEGER NOT NULL PRIMARY KEY,
    AssignmentId INTEGER NOT NULL,
    Score REAL,
    WorkflowState TEXT,
    SubmittedAt TEXT,
    Comments TEXT,
    FOREIGN KEY (AssignmentId) REFERENCES Assignments(CanvasAssignmentId)
);