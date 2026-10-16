namespace DealRealState.DTOs
{
    public class RefreshRequestDTO
    {
        public int UserID { get; set; }

        public required string RefreshToken { get; set; } 
    }
}