using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certitrack.Models
{
    public partial class Channel
    {
        public Channel()
        {
            CertificateLink = new HashSet<CertificateLink>();
        }

        public int Id { get; set; }
        [Display(Name = "Channel")]
        public string ChannelName { get; set; }

        public ICollection<CertificateLink> CertificateLink { get; set; }
    }
}
