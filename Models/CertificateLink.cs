namespace Certitrack.Models
{
    public partial class CertificateLink
    {
        public int CertificateId { get; set; }
        public int? StaffId { get; set; }
        public int? CustomerId { get; set; }
        public int? PromotionId { get; set; }
        public int? ChannelId { get; set; }

        public Certificate Certificate { get; set; }
        public Channel Channel { get; set; }
        public Customer Customer { get; set; }
        public Promotion Promotion { get; set; }
        public Staff Staff { get; set; }
    }
}
