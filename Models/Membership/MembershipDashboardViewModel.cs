using Domain.Enums;

namespace UIStoreMVC.Models.Membership
{
    public class MembershipDashboardViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public MembershipType MembershipType { get; set; }
        public bool IsVipActive { get; set; }

        public bool HasAgencyMembership { get; set; }

        public bool HasAgencyApplication { get; set; }
        public AgencyApplicationStatus? AgencyApplicationStatus { get; set; }
        public string? AgencyApplicationRejectionReason { get; set; }
    }
}
