//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BAPT.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Markup
    {
        public int Id { get; set; }
        public string MarkupType { get; set; }
        public int MarkupTerm { get; set; }
        public double MarkupPercent { get; set; }
        public string Segement { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<System.DateTime> UpdatedDateTime { get; set; }
    }
}
