namespace Backend_poulina_future_jobs.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public AppUser User { get; set; }
    }
}
