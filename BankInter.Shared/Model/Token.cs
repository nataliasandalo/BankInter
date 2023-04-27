using System;

public class Token
{
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; }

    private DateTime _expirationTime;

    public bool IsExpired()
    {
        return DateTime.UtcNow >= _expirationTime;
    }

    public void SetExpirationTime()
    {
        _expirationTime = DateTime.UtcNow.AddSeconds(ExpiresIn);
    }
}