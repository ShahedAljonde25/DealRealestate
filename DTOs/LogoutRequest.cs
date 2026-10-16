namespace DealRealState.DTOs
{
    public class LogoutRequestDTO
    {
        public int UserID { get; set; }

        public string RefreshToken { get; set; }
    }
}