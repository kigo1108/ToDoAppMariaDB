namespace b1.Models
{
    public class ErrorVM
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public override string ToString()=>System.Text.Json.JsonSerializer.Serialize(this);
    }
}
