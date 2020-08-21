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
    public class LecturersController : Controller
    {
        private MCQDatabaseEntities db = new MCQDatabaseEntities();
        //Encryption DLL is used to encrypt the password provided by user in order for it to be compared to the password store in the database
        Class1 EncryptionDll = new Class1();

        // GET: Lecturers/Edit/
        //lecturers are able to edit their password
        public ActionResult Edit(string id)
        {
            if (Session["LecturerID"] == null)
            {
                Session["ErrorMessage"] = "Please Login first";
                return RedirectToAction("LecturerLogin", "Lecturers");
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Lecturer lecturer = db.Lecturers.Find(id);
                if (lecturer == null)
                {
                    return HttpNotFound();
                }
                return View(lecturer);
            }
        }

        // POST: Lecturers/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LecturerID,Firstname,LastName")] Lecturer lecturer, string password)
        {
            if (ModelState.IsValid)
            {
                lecturer.L_Password = EncryptionDll.EncryptData(password, "ffhhgfgh");
                db.Entry(lecturer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewTest");
            }
            return View(lecturer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Log in view for lecturers to sign in
        public ActionResult LecturerLogin()
        {
            if (Session["ErrorMessage"] != null)
            {
                ViewBag.Error = Session["ErrorMessage"] as string;
            }

            return View();
        }

        //the username and encrypted password are compared to that in the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LecturerLogin(Lecturer lecturerObj)
        {
            if (ModelState.IsValid)
            {
                bool found = false;
                if (lecturerObj.LecturerID != null && lecturerObj.L_Password != null)
                {
                    using (MCQDatabaseEntities db = new MCQDatabaseEntities())
                    {
                        string password = EncryptionDll.EncryptData(lecturerObj.L_Password, "ffhhgfgh");
                        //checks if any username and password are a match

                        var obj = db.Lecturers.Where(a => a.LecturerID.Equals(lecturerObj.LecturerID) && a.L_Password.Equals(password)).FirstOrDefault();
                        if (obj != null)
                        {
                            Session["LecturerID"] = lecturerObj.LecturerID.ToString();
                            Session["StudentID"] = null;
                            return RedirectToAction("ViewTest");
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
            return View(lecturerObj);
        }

        //view for Lecturers to view the Tests that students have completed
        public ActionResult ViewTest()
        {
            IEnumerable<Student_Test> StudentTestList = new List<Student_Test>();
            if (Session["LecturerID"] == null)
            {
                Session["ErrorMessage"] = "Please Login first";
                return RedirectToAction("LecturerLogin", "Lecturers");
            }
            else
            {
                string id = Session["LecturerID"].ToString();
                IEnumerable<Test> TestTableList = new List<Test>();
                TestTableList = db.Tests.ToList().Where(a => a.LecturerID == id);

                foreach (var item in TestTableList)
                {
                    StudentTestList = db.Student_Test.ToList();
                }
            }
            return View(StudentTestList.ToList());
        }

        // GET: Lecturers/CreateTest
        //View for Lecturers to create a test
        public ActionResult CreateTest()
        {
            if (Session["LecturerID"] == null)
            {
                Session["ErrorMessage"] = "Please Login first";
                return RedirectToAction("LecturerLogin", "Lecturers");
            }
            else
            {
                return View();
            }
        }
        //POST Create Test
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTest([Bind(Include = "TestName, Publish")] Test test)
        {
            if (ModelState.IsValid)
            {
                //checks if the fields are not empty - data validation
                if (test.TestName != null && test.Publish != null)
                {
                    //iterates through the database to see if the test name by Lecturer matchs one in the database
                    bool notFound = false;
                    using (MCQDatabaseEntities db = new MCQDatabaseEntities())
                    {
                        var obj = db.Tests.Where(a => a.TestName.Equals(test.TestName)).FirstOrDefault();
                        if (obj == null)
                        {
                            notFound = true;
                            test.LecturerID = Session["LecturerID"].ToString();
                            int count = 1;
                            List<Test> list = db.Tests.ToList();
                            for (int i = 0; i < list.Count; i++)
                            {
                                Test item = list[i];
                                if (count != Int32.Parse(item.TestID.Substring(2, 3)))
                                {
                                    if (count < 10)
                                    {
                                        test.TestID = "TE00" + count;
                                    }
                                    else if (count < 100 && count > 10)
                                    {
                                        test.TestID = "TE0" + count;
                                    }
                                    else
                                    {
                                        test.TestID = "TE" + count;
                                    }
                                }
                                else
                                {
                                    count++;
                                    if (count > list.Count)
                                    {
                                        if (count < 10)
                                        {
                                            test.TestID = "TE00" + count;
                                        }
                                        else if (count < 100 && count >= 10)
                                        {
                                            test.TestID = "TE0" + count;
                                        }
                                        else
                                        {
                                            test.TestID = "TE" + count;
                                        }
                                    }
                                }
                            }
                            db.Tests.Add(test);
                            db.SaveChanges();
                            Session["TestID"] = test.TestID;
                            return RedirectToAction("CreateQuestionsView");
                        }
                    }

                    if (notFound == false)
                    {
                        ViewBag.Error = "This test already exists";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Please fill in all fields!";
                    return View();
                }

            }
            return View(test);
        }
        // GET: Lecturers/CreateTest
        //view for lecturers to craete questions for the test
        public ActionResult CreateQuestionsView()
        {
            if (Session["LecturerID"] == null)
            {
                Session["ErrorMessage"] = "Please Login first";
                return RedirectToAction("LecturerLogin", "Lecturers");
            }
            else
            {
                return View();
            }
        }
        //POST Create Question

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuestionsView([Bind(Include = "Question, Answer1, Answer2, Answer3, Answer4, CorrectAnswer, Publish")] Question_Answer qa)
        {
            if (ModelState.IsValid)
            {
                //checks if all the fields have values in them
                if (qa.Question != null && qa.Answer1 != null && qa.Answer3 != null && qa.Answer4 != null && qa.CorrectAnswer != null)
                {
                    //check to see that one of the possible answers matches the correct answer
                    if (qa.Answer1 == qa.CorrectAnswer || qa.Answer2 == qa.CorrectAnswer || qa.Answer3 == qa.CorrectAnswer || qa.Answer4 == qa.CorrectAnswer)
                    {
                        qa.TestID = Session["TestID"].ToString();
                        int count = 1;
                        List<Question_Answer> list = db.Question_Answer.ToList();
                        //piece of code that finds the last known ID and increments it by 1
                        for (int i = 0; i < list.Count; i++)
                        {
                            Question_Answer item = list[i];
                            if (count != Int32.Parse(item.QuestionID.Substring(2, 3)))
                            {
                                if (count < 10)
                                {
                                    qa.QuestionID = "Q000" + count;
                                }
                                else if (count < 100 && count > 10)
                                {
                                    qa.QuestionID = "Q00" + count;
                                }
                                else
                                {
                                    qa.QuestionID = "Q0" + count;
                                }
                            }
                            else
                            {
                                count++;
                                if (count > list.Count)
                                {
                                    if (count < 10)
                                    {
                                        qa.QuestionID = "Q000" + count;
                                    }
                                    else if (count < 100 && count >= 10)
                                    {
                                        qa.QuestionID = "Q00" + count;
                                    }
                                    else
                                    {
                                        qa.QuestionID = "Q0" + count;
                                    }
                                }
                            }
                        }
                        //loads data into table
                        db.Question_Answer.Add(qa);
                        db.SaveChanges();
                        return RedirectToAction("CreateQuestionsView");
                    }
                    else
                    {
                        ViewBag.Error = "Correct Answer does not match any possible answers";
                        return View();
                    }
                }
                {
                    ViewBag.Error = "Correct Answer does not match any possible answers";
                    return View();
                }

            }
            return View(qa);
        }
        //View for Lecturers to view all their tests created
        public ActionResult TestList()
        {
            if (Session["LecturerID"] == null)
            {
                Session["ErrorMessage"] = "Please Login first";
                return RedirectToAction("LecturerLogin", "Lecturers");
            }
            else
            {
                return View(db.Tests.ToList().Where(a => a.LecturerID == Session["LecturerID"].ToString()));
            }
        }
        //view for Lecturers to pulish/unpublish a test
        public ActionResult EditTest(string id)
        {
            if (Session["LecturerID"] == null)
            {
                Session["ErrorMessage"] = "Please Login first";
                return RedirectToAction("LecturerLogin", "Lecturers");
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Test test = db.Tests.Find(id);
                if (test == null)
                {
                    return HttpNotFound();
                }
                return View(test);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTest([Bind(Include = "Publish")] Test test, string id, string name)
        {
            if (ModelState.IsValid)
            {
                test.TestName = name;
                test.TestID = id;
                test.LecturerID = Session["LecturerID"].ToString();
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("TestList");
            }
            return View(test);
        }
    }
}
