namespace DimmedAPI.DTOs
{
    public class UserNotificationsResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public bool NotificationCons { get; set; }
        public string Name { get; set; }
        public string BranchName { get; set; }
    }
} 