using EncryptionDLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class StudentsController : Controller
    {
        private MCQDatabaseEntities db = new MCQDatabaseEntities();
        //encryption class is used to encrypt users password 
        Class1 EncryptionDll = new Class1();

        // GET: Students/Edit/5
        //students are able to edit their password
        public ActionResult Edit(int? id)
        {
            if (Session["StudentID"] == null)
            {
                Session["ErrorMessage"] = "Please Login first";
                return RedirectToAction("StudentLogin", "Students");
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Student student = db.Students.Find(id);
                if (student == null)
                {
                    return HttpNotFound();
                }
                return View(student);
            }
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentNumber,Age,Firstname,LastName")] Student student, string password)
        {
            if (ModelState.IsValid)
            {
                student.S_Password = EncryptionDll.EncryptData(password, "ffhhgfgh");
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("StudentViewMarks");
            }
            return View(student);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //view for students to log in
       
        public ActionResult StudentLogin()
        {
            if (Session["ErrorMessage"] != null)
            {
                ViewBag.Error = Session["ErrorMessage"] as string;
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentLogin(Student studentObj)
        {
            if (ModelState.IsValid)
            {
                bool found = false;
                if(studentObj.StudentNumber.ToString() != null && studentObj.S_Password != null)
                {
                    using (MCQDatabaseEntities db = new MCQDatabaseEntities())
                    {
                        string password = EncryptionDll.EncryptData(studentObj.S_Password, "ffhhgfgh");
                        var obj = db.Students.Where(a => a.StudentNumber.Equals(studentObj.StudentNumber) && a.S_Password.Equals(password)).FirstOrDefault();
                        if (obj != null)
                        {
                            found = true;
                            Session["StudentID"] = studentObj.StudentNumber.ToString();
                            Session["LecturerID"] = null;
                            return RedirectToAction("StudentViewMarks");
                        }
                    }

                    if (!found)
                    {
                        ViewBag.Error = "Incorrect Details - please try again!";
                    }
                }
                else
                {
                    ViewBag.Error = "Please fill in all fields";
                }
                return View();

            }
            return View(studentObj);
        }
        //GET Student View Marks
        //view students marks for the tests they have completed
        public ActionResult StudentViewMarks()
        {
            if (Session["StudentID"] == null)
            {
                Session["ErrorMessage"] = "Please Login first";
                return RedirectToAction("StudentLogin", "Students");
            }
            else
            {
                int id = Convert.ToInt32(Session["StudentID"].ToString());
                return View(db.Student_Test.ToList().Where(a => a.StudentNumber == id));
            }

        }
        //GET View Memo
        //This displays the memo for a specific test that has been selected
        public ActionResult ViewMemo(string selectedTestName)
        {
            if (Session["StudentID"] == null)
            {
                Session["ErrorMessage"] = "Please Login first";
                return RedirectToAction("StudentLogin", "Students");
            }
            else
            {
                if (selectedTestName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string testName = selectedTestName;
                IEnumerable<Test> name= new List<Test>();
                name = db.Tests.ToList().Where(a => a.TestName == testName);

                foreach (var item in name)
                {
                    var qa = db.StudentAnswers.Include(a => a.Question_Answer).Where(c => c.Question_Answer.TestID == item.TestID);
                    return View(qa.ToList());
                }

                return View();
            }
        }
    }
}
