using Microsoft.EntityFrameworkCore;
using Honours_Project_CompetencyandSkillTracking.Data;
using System.Linq;

namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class CanvasSyncService
    {
        private readonly CanvasService _canvas;
        private readonly AppDbContext _db;

        public CanvasSyncService(CanvasService canvas, AppDbContext db)
        {
            _canvas = canvas;
            _db = db;
        }

        public async Task SyncAllAsync()
        {
            await SyncProfileAsync();
            await SyncCoursesAsync();
            //await SyncOutcomesAndResultsAsync();
        }

        private async Task SyncProfileAsync()
        {
            var profile = await _canvas.GetMyProfileAsync();
            Console.WriteLine($"Syncing profile: {profile.Id}, {profile.Name}, {profile.Primary_Email}");

            // Store Canvas Id as string to match Student PK
            string profileId = profile.Id.ToString();

            var student = await _db.Students.FindAsync(profileId);
            if (student == null)
            {
                student = new User
                {
                    StudentId = profileId,
                    UserName = profile.Name,
                    StEmail = profile.Primary_Email,
                    Password = "password",
                    Role = "Student"
                };
                _db.Students.Add(student);
            }
            else
            {
                student.UserName = profile.Name;
                student.StEmail = profile.Primary_Email;
            }

            await _db.SaveChangesAsync();
        }

        private async Task SyncCoursesAsync()
        {
            var courses = await _canvas.GetMyCoursesAsync();
            Console.WriteLine($"Syncing {courses.Count} courses...");

            foreach (var c in courses)
            {
                // Find by Canvas course Id (long)
                var course = await _db.Courses.FindAsync(c.Id);
                if (course == null)
                {
                    course = new Course
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Code = c.Course_Code,
                        Syllabus = c.Syllabus_Body,
                        Term = c.Term?.Name ?? ""
                    };
                    _db.Courses.Add(course);
                }
                else
                {
                    course.Name = c.Name;
                    course.Code = c.Course_Code;
                    course.Syllabus = c.Syllabus_Body;
                    course.Term = c.Term?.Name ?? "";
                }

                // Sync assignments
                var assignments = await _canvas.GetCourseAssignmentsAsync(c.Id);
                Console.WriteLine($"Syncing {assignments.Count} assignments for course {course.Name}...");

                foreach (var a in assignments)
                {
                    // Skip unsupported submission types for student token
                    if (a.SubmissionTypes == null || a.SubmissionTypes.Contains("none") ||
                        a.SubmissionTypes.Contains("on_paper") ||
                        a.SubmissionTypes.Contains("external_tool") ||
                        a.SubmissionTypes.Contains("quiz"))
                    {
                        Console.WriteLine($">>> Skipping assignment {a.Id} (unsupported for student token)");
                        continue;
                    }

                    var assignment = await _db.Assignments.FindAsync(a.Id);
                    if (assignment == null)
                    {
                        assignment = new Assignment
                        {
                            Id = a.Id,
                            CourseId = course.Id,
                            Name = a.Name,
                            Description = a.Description,
                            DueAt = a.Due_At
                        };
                        _db.Assignments.Add(assignment);
                    }
                    else
                    {
                        assignment.Name = a.Name;
                        assignment.Description = a.Description;
                        assignment.DueAt = a.Due_At;
                    }

                    // Get submissions for this assignment
                    try
                    {
                        var submissions = await _canvas.GetAssignmentSubmissionsAsync(course.Id, a.Id);
                        Console.WriteLine($">>> Got {submissions.Count} submissions for assignment {assignment.Name}");

                        foreach (var s in submissions)
                        {
                            var submission = await _db.Submissions.FindAsync(s.Id);
                            if (submission == null)
                            {
                                submission = new Submission
                                {
                                    Id = s.Id,
                                    AssignmentId = a.Id,
                                    Score = s.Score,
                                    //WorkflowState = s.WorkflowState,
                                    SubmittedAt = s.SubmittedAt
                                };
                                _db.Submissions.Add(submission);
                            }
                            else
                            {
                                submission.Score = s.Score;
                                //submission.WorkflowState = s.WorkflowState;
                                submission.SubmittedAt = s.SubmittedAt;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($">>> ERROR fetching submissions for assignment {a.Id}: {ex.Message}");
                    }
                }

                // Save batch per course
                await _db.SaveChangesAsync();
            }
        }
        private async Task SyncOutcomesAndResultsAsync()
        {
            // We sync based on the courses already in our database
            var localCourses = await _db.Courses.ToListAsync();

            foreach (var course in localCourses)
            {
                Console.WriteLine($"Syncing Outcomes for course: {course.Name}...");

                // 1. Sync Outcomes (The Definitions)
                var canvasOutcomes = await _canvas.GetCourseOutcomesAsync(course.Id);
                foreach (var co in canvasOutcomes)
                {
                    var outcome = await _db.Outcomes.FindAsync(co.Id);
                    if (outcome == null)
                    {
                        co.CourseId = course.Id; // Ensure FK is set
                        _db.Outcomes.Add(co);
                    }
                    else
                    {
                        outcome.Title = co.Title;
                        outcome.Description = co.Description;
                        outcome.CalculationMethod = co.CalculationMethod;
                        outcome.MasteryPoints = co.MasteryPoints;
                    }
                }
                // Save outcomes first so results can find them via FK
                await _db.SaveChangesAsync();

                // 2. Sync Outcome Results (The Student Performance)
                var results = await _canvas.GetOutcomeResultsAsync(course.Id);
                Console.WriteLine($"Found {results.Count} outcome results for {course.Name}");

                foreach (var res in results)
                {
                    // Canvas OutcomeResults ID can be large, ensure your DB uses long
                    var existingResult = await _db.OutcomeResults.FindAsync(res.Id);

                    if (existingResult == null)
                    {
                        // Ensure the student exists in our DB before linking
                        var studentExists = await _db.Students.AnyAsync(s => s.StudentId == res.StudentId);
                        if (!studentExists) continue;

                        _db.OutcomeResults.Add(res);
                    }
                    else
                    {
                        existingResult.Score = res.Score;
                        existingResult.Mastery = res.Mastery;
                        existingResult.AssignmentId = res.AssignmentId;
                    }
                }
                await _db.SaveChangesAsync();
            }
        }
    }
}