namespace DimmedAPI.DTOs
{
    public class UserNotificationsCreateDTO
    {
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public bool NotificationCons { get; set; }
    }
} 