using Microsoft.EntityFrameworkCore;
using Honours_Project_CompetencyandSkillTracking.Data;
using System.Linq;
using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;

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
            var student = await SyncProfileAsync();
            await SyncCoursesAsync(student);
            //await SyncOutcomesAndResultsAsync(student);
            await GetOutcomesForJohnWTestCourseAsync();
        }

        private async Task<User> SyncProfileAsync()
        {
            var profile = await _canvas.GetMyProfileAsync();
            Console.WriteLine($"Syncing profile: {profile.Id}, {profile.Name}");

            string studentId = profile.Id.ToString();

            var student = await _db.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                student = new User
                {
                    StudentId = studentId,
                    UserName = profile.Name,
                    StEmail = profile.Primary_Email,
                    Password = "password",
                    Role = "Student"
                };

                _db.Students.Add(student);
                await _db.SaveChangesAsync();
            }
            else
            {
                student.UserName = profile.Name;
                student.StEmail = profile.Primary_Email;
                await _db.SaveChangesAsync();
            }

            return student;
        }

        private async Task SyncCoursesAsync(User student)
        {
            var courses = await _canvas.GetMyCoursesAsync();
            Console.WriteLine($"Syncing {courses.Count} courses...");

            foreach (var c in courses)
            {
                var course = await _db.Courses.Include(c => c.Students).FirstOrDefaultAsync(x => x.Id == c.Id);

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

                // 🔹 Ensure student is linked to course
                if (!course.Students.Any(s => s.StudentId == student.StudentId))
                {
                    course.Students.Add(student);
                }

                await _db.SaveChangesAsync();

                var assignments = await _canvas.GetCourseAssignmentsAsync(c.Id);
                Console.WriteLine($"Syncing {assignments.Count} assignments for {course.Name}");

                foreach (var a in assignments)
                {
                    // Skip unsupported submission types
                    if (a.SubmissionTypes == null ||
                        a.SubmissionTypes.Contains("none") ||
                        a.SubmissionTypes.Contains("on_paper") ||
                        a.SubmissionTypes.Contains("external_tool") ||
                        a.SubmissionTypes.Contains("quiz"))
                    {
                        continue;
                    }

                    var assignment = await _db.Assignments.FirstOrDefaultAsync(x => x.Id == a.Id);

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
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        assignment.Name = a.Name;
                        assignment.Description = a.Description;
                        assignment.DueAt = a.Due_At;
                        await _db.SaveChangesAsync();
                    }

                    var submissions = await _canvas.GetAssignmentSubmissionsAsync(course.Id, a.Id);

                    foreach (var s in submissions)
                    {
                        var submission = await _db.Submissions.FirstOrDefaultAsync(x => x.Id == s.Id);

                        if (submission == null)
                        {
                            submission = new Submission
                            {
                                Id = s.Id,
                                AssignmentId = assignment.Id,
                                StudentId = student.StudentId,
                                Score = s.Score,
                                SubmittedAt = s.SubmittedAt
                            };

                            _db.Submissions.Add(submission);
                        }
                        else
                        {
                            submission.Score = s.Score;
                            submission.SubmittedAt = s.SubmittedAt;
                        }
                    }

                    await _db.SaveChangesAsync();
                }
            }
        }

        private async Task SyncOutcomesAndResultsAsync(User student)
        {
            var localCourses = await _db.Courses.ToListAsync();

            foreach (var course in localCourses)
            {
                Console.WriteLine($"Syncing outcomes for {course.Name}");

                // Sync Outcome Definitions
                var canvasOutcomes = await _canvas.GetCourseOutcomesAsync(course.Id);

                foreach (var co in canvasOutcomes)
                {
                    var outcome = await _db.Outcomes.FindAsync(co.Id);

                    if (outcome == null)
                    {
                        co.CourseId = course.Id;
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

                await _db.SaveChangesAsync();

                // Sync Outcome Results (student scoped by token)
                var results = await _canvas.GetOutcomeResultsAsync(course.Id);

                foreach (var res in results)
                {
                    var existing = await _db.OutcomeResults.FirstOrDefaultAsync(r => r.Id == res.Id);

                    if (existing == null)
                    {
                        res.StudentId = student.StudentId; // ensure ownership
                        _db.OutcomeResults.Add(res);
                    }
                    else
                    {
                        existing.Score = res.Score;
                        existing.Mastery = res.Mastery;
                        existing.AssignmentId = res.AssignmentId;
                    }
                }

                await _db.SaveChangesAsync();
            }
        }
        public async Task GetOutcomesForJohnWTestCourseAsync()
        {
            long courseId = 77966;

            try
            {
                var outcomes = await _canvas.GetCourseOutcomesAsync(courseId);

                if (outcomes.Any())
                {
                    Console.WriteLine(
                        $"Found {outcomes.Count} outcomes for course {courseId}.");

                    foreach (var outcome in outcomes)
                    {
                        Console.WriteLine(
                            $"Outcome: {outcome.Title} | " +
                            $"Mastery: {outcome.MasteryPoints} | " +
                            $"Method: {outcome.CalculationMethod}");
                    }
                }
                else
                {
                    Console.WriteLine(
                        $"No outcomes found for course {courseId}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> ERROR fetching JohnWTest outcomes: {ex.Message}");
            }
        }
    }
}