using System;
using System.Collections.Generic;

namespace Certitrack.Models
{
    public partial class Channel
    {
        public Channel()
        {
            CertificateLink = new HashSet<CertificateLink>();
        }

        public int Id { get; set; }
        public string ChannelName { get; set; }

        public ICollection<CertificateLink> CertificateLink { get; set; }
    }
}
