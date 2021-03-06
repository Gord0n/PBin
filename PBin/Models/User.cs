//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PBin.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.Post = new HashSet<Post>();
            this.SharedPost = new HashSet<SharedPost>();
            this.UserRole = new HashSet<UserRole>();
            this.WatchWord = new HashSet<WatchWord>();
        }
    
        public System.Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<bool> Active { get; set; }
        public string Salt { get; set; }
    
        public virtual ICollection<Post> Post { get; set; }
        public virtual ICollection<SharedPost> SharedPost { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
        public virtual ICollection<WatchWord> WatchWord { get; set; }
    }
}
