//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WitsClasses
{
    using System;
    using System.Collections.Generic;
    
    public partial class Players
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Players()
        {
            this.BlockedPlayers = new HashSet<BlockedPlayers>();
            this.BlockedPlayers1 = new HashSet<BlockedPlayers>();
            this.Friends = new HashSet<Friends>();
            this.Friends1 = new HashSet<Friends>();
            this.Notifications = new HashSet<Notifications>();
            this.Notifications1 = new HashSet<Notifications>();
        }
    
        public string username { get; set; }
        public string email { get; set; }
        public string userPassword { get; set; }
        public Nullable<int> highestScore { get; set; }
        public Nullable<int> profilePictureId { get; set; }
        public Nullable<int> celebrationId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlockedPlayers> BlockedPlayers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlockedPlayers> BlockedPlayers1 { get; set; }
        public virtual Celebrations Celebrations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Friends> Friends { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Friends> Friends1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notifications> Notifications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notifications> Notifications1 { get; set; }
        public virtual ProfilePictures ProfilePictures { get; set; }
    }
}
