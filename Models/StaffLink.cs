namespace Certitrack.Models
{
    public partial class StaffLink
    {
        public int StaffId { get; set; }
        public int RoleId { get; set; }
        public int StaffTypeId { get; set; }

        public Role Role { get; set; }
        public Staff Staff { get; set; }
        public StaffType StaffType { get; set; }
    }
}
