namespace Alexander.Models
{
    public class AccountResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public EconomyProfile Profile { get; set; }

    public static AccountResult Ok(string message, EconomyProfile profile = null)
    {
        return new AccountResult
        {
            Success = true,
            Message = message,
            Profile = profile
        };
    }

    public static AccountResult Failure(string message)
    {
        return new AccountResult
        {
            Success = false,
            Message = message
        };
    }
}
}
