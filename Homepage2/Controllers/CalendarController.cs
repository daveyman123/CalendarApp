using Hangfire;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using SendGrid;
using System.Net;
using System.Text.RegularExpressions;

namespace Homepage2.Controllers
{
    
    public class CalendarController : Controller
    {
        public void SendMail(string email, Event e)
        {
            try
            {
                //Message properties, add your values below 
                string fromProperty = "noreply@example.com";
                //string recipientsProperty = "@'John Smith <" + email + ">'";
  //              List<String> recipientsProperty = new List<String>
//{
  //  @"John Smith <djgraff1@cougars.ccis.edu>",
    
//};
                string subjectProperty = "Calendar Reminder";
                string HTMLContentProperty = e.Subject + " is set to start at: " + e.Start.ToString()
                    + " go to http://homepage220170930121718.azurewebsites.net/Calendar to see the event";
                //SendGrid credentials 

                //The email object 
                var message = new SendGridMessage();
                message.From = new MailAddress(fromProperty);
                message.AddTo(email);
                message.Subject = subjectProperty;
                //Add the HTML and Text bodies 
                message.Html = HTMLContentProperty;
                var credentials = new NetworkCredential(
                       ConfigurationManager.AppSettings["mailAccount"],
                       ConfigurationManager.AppSettings["mailPassword"]
                       );
                var transportWeb = new Web(credentials);
                transportWeb.DeliverAsync(message).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        // GET: Home
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        private DefaultConnection _context;

        public CalendarController()
        {
            _context = new DefaultConnection();
        }

        public JsonResult GetEvents()
        {
            using (DefaultConnection dc = new DefaultConnection())
            {
                var userID = User.Identity.GetUserId();
              
                
                var events = (from e in _context.Events
                              
                              where e.userID == userID
                              select e).ToList();
                


                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetEmails()
        {
            using (DefaultConnection dc = new DefaultConnection())
            {
                var emails = (from emailz in dc.Users

                              select emailz.Email).ToList();

                return new JsonResult { Data = emails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult SaveEvent(Event e)
        {
           
            var RecIdz = Guid.NewGuid().ToString();
            e.userID = User.Identity.GetUserId();
            var status = false;
            string jobID = "0";
            using (DefaultConnection dc = new DefaultConnection())
            {
                if (e.EventID == 0 && e.Reminder > 0)
                {
                    
                    var email = (from emailz in dc.Users
                                 where emailz.Id == e.userID
                                 select emailz.Email).FirstOrDefault();
                    if (e.Reminder == 1)
                    {
                        jobID = BackgroundJob.Schedule(() => SendMail(email, e), e.Start);
                    }
                    if (e.Reminder == 2)
                    {
                        jobID = BackgroundJob.Schedule(() => SendMail(email, e), e.Start.AddMinutes(-15));
                    }
                    if (e.Reminder == 3)
                    {
                        jobID = BackgroundJob.Schedule(() => SendMail(email, e), e.Start.AddHours(-1));
                    }
                    if (e.Reminder == 4)
                    {
                        jobID = BackgroundJob.Schedule(() => SendMail(email, e), e.Start.AddDays(-7));
                    }
                    if (e.Reminder == 5)
                    {
                        jobID = BackgroundJob.Schedule(() => SendMail(email, e), e.Start.AddDays(-14));
                    }
                    e.JobID = jobID;

                }
                if (e.EventID == 0 && e.Freq > 0)
                {
                    DateTime stDate = e.Start;
                    DateTime EnDate = e.End ?? e.Start;
                    int i = 0;
                    do
                    {
                        Event _e = new Event();
                        if (e.EventID == 0 && e.Freq == 1)
                        {
                            stDate = stDate.AddDays(7);
                            EnDate = EnDate.AddDays(7);
                            _e.Start = stDate;
                            _e.End = EnDate;
                        }
                        if (e.Freq == 2)
                        {
                            stDate = stDate.AddMonths(1);
                            EnDate = EnDate.AddMonths(1);
                            _e.Start = stDate;
                            _e.End = EnDate;
                        }
                        if (e.Freq == 3)
                        {
                            stDate = stDate.AddYears(1);
                            EnDate = EnDate.AddYears(1);
                            _e.Start = stDate;
                            _e.End = EnDate;
                        }
                        e.RecId = RecIdz;
                        _e.Subject = e.Subject;
                        _e.Description = e.Description;
                        _e.IsFullDay = e.IsFullDay;
                        _e.ThemeColor = e.ThemeColor;
                        _e.Freq = e.Freq;
                        _e.userID = User.Identity.GetUserId();
                        _e.RecId = RecIdz;
                        _e.JobID = jobID;
                        dc.Events.Add(_e);
                        
                        i++;
                    } while (i < 25);
                }

                if (e.EventID > 0)
                {
                    //Update the event
                    var v = dc.Events.Where(a => a.EventID == e.EventID).FirstOrDefault();
                    if (v != null)
                    {
                        v.Subject = e.Subject;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                        v.Freq = e.Freq;
                        v.userID = e.userID;
                        v.RecId = e.RecId;
                        v.JobID = e.JobID;
                    }
                }
                else
                {
                    if (e.RecId == null)
                    {
                        e.RecId = RecIdz;
                    }
                    dc.Events.Add(e);
                }

                dc.SaveChanges();
                status = true;

            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            using (DefaultConnection dc = new DefaultConnection())
            {
                var v = dc.Events.Where(a => a.EventID == eventID).FirstOrDefault();
                if (v != null)
                {   if (v.JobID != null)
                    {
                        BackgroundJob.Delete(v.JobID);
                    }
                    //using (var connection = JobStorage.Current.GetConnection())
                    dc.Events.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult ShareEvent(string emails, string UserID, string subject,
                                        DateTime start, DateTime end, string RecID)
        {
            var emailz = emails.Split('|').ToList();
            string emailzz = "";
            var status = false;

            using (DefaultConnection dc = new DefaultConnection())
            {
                
                int i = 0;
                do
                {
                    Event events = new Event();
                    emailzz = Regex.Replace(emailz[i], "[|]", "");
                    
                    if (emailzz != "")
                    {
                        
                        //events = slectedEvent;
                        var foo = dc.Users.Where(h => h.Email == emailzz).FirstOrDefault();
                        events.userID = foo.Id;
                        events.End = end;
                        events.Start = start;
                        events.Subject = subject;
                        events.RecId = RecID;
                        //share.ShareEmail = foo.UserName;
                        
                        
                        var jobId = BackgroundJob.Enqueue(() => SendMail(emailzz, events));
                    }
                    
                    
                    dc.Events.Add(events);
                    i++;
                } while (i < emailz.Count()-1);
                
                dc.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };

        }
        [HttpPost]
        public JsonResult ClearAll(string RecId, DateTime Start, bool all)
        {
            var status = false;
            if (all == true)
            {


                using (DefaultConnection dc = new DefaultConnection())
                {
                    var events = (from e in _context.Events
                                  where e.RecId == RecId
                                  select e).ToList();
                    foreach (Event i in events)
                    {
                        dc.Events.Attach(i);

                        dc.Events.Remove(i);
                    }
                    dc.SaveChanges();
                    status = true;
                }
            }
            
            using (DefaultConnection dc = new DefaultConnection())
            {
                //var userID = User.Identity.GetUserId();
                var events = (from e in _context.Events
                              where e.RecId == RecId && e.Start > Start
                              select e).ToList();
                foreach (Event i in events)
                {
                    dc.Events.Attach(i);
                   
                    dc.Events.Remove(i);
                }
                dc.SaveChanges();
                status = true;
            }
        
                return new JsonResult { Data = new { status = status } };
        }


    }
}