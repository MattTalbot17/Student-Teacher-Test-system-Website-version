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
    
    public partial class Student_Test
    {
        public string StudentTestID { get; set; }
        public string TestName { get; set; }
        public int StudentNumber { get; set; }
        public decimal TestMark { get; set; }
    
        public virtual Student Student { get; set; }
    }
}