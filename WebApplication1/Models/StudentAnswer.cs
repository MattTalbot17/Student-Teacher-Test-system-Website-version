//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class StudentAnswer
    {
        public int StudentAnswerID { get; set; }
        public int StudentNumber { get; set; }
        public string QuestionID { get; set; }
        public string Answer { get; set; }
    
        public virtual Question_Answer Question_Answer { get; set; }
        public virtual Student Student { get; set; }
    }
}