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
            await SyncOutcomesAndResultsAsync(student);
        }

        private async Task<User> SyncProfileAsync()
        {
            var profile = await _canvas.GetMyProfileAsync();
            //Console.WriteLine($"Syncing profile: {profile.Id}, {profile.Name}");
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
            //Console.WriteLine($"Syncing {courses.Count} courses...");
            foreach (var c in courses)
            {
                var course = await _db.Courses.Include(x => x.Students).FirstOrDefaultAsync(x => x.Id == c.Id);

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

                if (!course.Students.Any(s => s.StudentId == student.StudentId))
                {
                    course.Students.Add(student);
                }

                //await _db.SaveChangesAsync();

                var assignments = await _canvas.GetCourseAssignmentsAsync(c.Id);
                //Console.WriteLine($"Syncing {assignments.Count} assignments for {course.Name}");
                foreach (var a in assignments)
                {
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
                        //await _db.SaveChangesAsync();
                    }
                    else
                    {
                        assignment.Name = a.Name;
                        assignment.Description = a.Description;
                        assignment.DueAt = a.Due_At;
                        //await _db.SaveChangesAsync();
                    }

                    var s = a.Submission;

                    if (s == null || s.Score == null || s.Score < 0)
                        continue;

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
                        //await _db.SaveChangesAsync();
                    }
                    else
                    {
                        submission.Score = s.Score;
                        submission.SubmittedAt = s.SubmittedAt;
                        //await _db.SaveChangesAsync();
                    }

                    if (s.Comments != null)
                    {
                        foreach (var cmt in s.Comments)
                        {
                            var existingComment = await _db.SubmissionComments.FirstOrDefaultAsync(x => x.Id == cmt.Id);

                            if (existingComment == null)
                            {
                                var comment = new SubmissionComment
                                {
                                    Id = cmt.Id,
                                    SubmissionId = submission.Id,
                                    Comment = cmt.Comment,
                                    AuthorName = cmt.AuthorName,
                                    CreatedAt = cmt.CreatedAt
                                };
                                _db.SubmissionComments.Add(comment);
                            }
                            else
                            {
                                existingComment.Comment = cmt.Comment;
                                existingComment.AuthorName = cmt.AuthorName;
                                existingComment.CreatedAt = cmt.CreatedAt;
                            }
                        }
                        await _db.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task SyncOutcomesAndResultsAsync(User student)
        {
            var localCourses = await _db.Courses.ToListAsync();
            foreach (var course in localCourses)
            {
                //Console.WriteLine($"Syncing outcomes for course {course.Id}");
                var wrapper = await _canvas.GetOutcomeResultsWrapperAsync(course.Id);

                if (wrapper == null)
                    continue;

                if (wrapper.Linked?.Outcomes != null)
                {
                    foreach (var co in wrapper.Linked.Outcomes)
                    {
                        var outcome = await _db.Outcomes.FindAsync(co.Id);

                        if (outcome == null)
                        {
                            outcome = new Outcome
                            {
                                Id = co.Id,
                                CourseId = course.Id,
                                Title = co.Title,
                                Description = co.Description,
                                CalculationMethod = co.CalculationMethod,
                                MasteryPoints = co.MasteryPoints
                            };
                            _db.Outcomes.Add(outcome);
                        }
                        else
                        {
                            outcome.Title = co.Title;
                            outcome.Description = co.Description;
                            outcome.CalculationMethod = co.CalculationMethod;
                            outcome.MasteryPoints = co.MasteryPoints;
                        }
                    }
                }

                if (wrapper.OutcomeResults != null && wrapper.OutcomeResults.Any())
                {
                    foreach (var res in wrapper.OutcomeResults)
                    {
                        if (res.Outcome == null)
                            continue;

                        var existing = await _db.OutcomeResults.FirstOrDefaultAsync(r => r.Id == res.Id);
                        long? assignmentId = null;

                        if (long.TryParse(res.Links?.Assignment, out var parsedId))
                        {
                            assignmentId = parsedId;
                        }

                        if (existing == null)
                        {
                            var newResult = new OutcomeResult
                            {
                                Id = res.Id,
                                StudentId = student.StudentId,
                                OutcomeId = res.Outcome.Id,
                                AssignmentId = assignmentId,
                                Score = res.Score,
                                Mastery = res.Mastery
                            };
                            _db.OutcomeResults.Add(newResult);
                        }
                        else
                        {
                            existing.Score = res.Score;
                            existing.Mastery = res.Mastery;
                            existing.AssignmentId = assignmentId;
                        }
                    }
                }
                else
                {
                    //Console.WriteLine($"No outcome results yet for course {course.Id}");
                }
                await _db.SaveChangesAsync();
            }
        }
    }
}