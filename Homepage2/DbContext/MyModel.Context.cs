using Homepage2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Homepage2
{
    public partial class DefaultConnection : ApplicationDbContext
    {
        public DefaultConnection()
            //: base("name=DefaultConnection")
        {
        }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Links> Links { get; set; }
        
        
    }
}