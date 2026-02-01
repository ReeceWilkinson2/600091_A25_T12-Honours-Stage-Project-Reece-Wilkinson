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
        }

        private async Task SyncProfileAsync()
        {
            var profile = await _canvas.GetMyProfileAsync();
            Console.WriteLine($"Syncing profile: {profile.Id}, {profile.Name}, {profile.Primary_Email}");

            var student = await _db.Students.FindAsync(profile.Id);
            if (student == null)
            {
                _db.Students.Add(new Student
                {
                    Id = profile.Id,
                    Name = profile.Name,
                    Email = profile.Primary_Email
                });
            }
            else
            {
                student.Name = profile.Name;
                student.Email = profile.Primary_Email;
            }

            await _db.SaveChangesAsync();
        }

        private async Task SyncCoursesAsync()
        {
            var courses = await _canvas.GetMyCoursesAsync();
            Console.WriteLine($"Syncing {courses.Count} courses...");

            foreach (var c in courses)
            {
                var course = await _db.Courses.FindAsync(c.Id);
                if (course == null)
                {
                    course = new Course
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Code = c.Course_Code,
                        Syllabus = c.Syllabus_Body
                    };
                    _db.Courses.Add(course);
                }
                else
                {
                    course.Name = c.Name;
                    course.Code = c.Course_Code;
                    course.Syllabus = c.Syllabus_Body;
                }

                // Get assignments for this course
                var assignments = await _canvas.GetCourseAssignmentsAsync(c.Id);
                Console.WriteLine($"Syncing {assignments.Count} assignments for course {course.Name}...");

                foreach (var a in assignments)
                {
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

                    //    // Get submissions for this assignment
                    //    var submissions = await _canvas.GetAssignmentSubmissionsAsync(course.Id, a.Id);
                    //    foreach (var s in submissions)
                    //    {
                    //        var submission = await _db.Submissions.FindAsync(s.Id);
                    //        if (submission == null)
                    //        {
                    //            submission = new Submission
                    //            {
                    //                Id = s.Id,
                    //                AssignmentId = a.Id,
                    //                Score = s.Score,
                    //                WorkflowState = s.Workflow_State,
                    //                SubmittedAt = s.Submitted_At,
                    //                Comments = string.Join("\n", s.Comments?.Select(cmt => cmt.Comment) ?? Array.Empty<string>())
                    //            };
                    //            _db.Submissions.Add(submission);
                    //        }
                    //        else
                    //        {
                    //            submission.Score = s.Score;
                    //            submission.WorkflowState = s.Workflow_State;
                    //            submission.SubmittedAt = s.Submitted_At;
                    //            submission.Comments = string.Join("\n", s.Comments?.Select(cmt => cmt.Comment) ?? Array.Empty<string>());
                    //        }
                    //    }
                    //}

                    // Save everything for this course in one batch
                    await _db.SaveChangesAsync();
                }
            }
        }
    }
}
