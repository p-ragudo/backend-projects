namespace ExpenseTrackerApi.Services.Auth;

public class TokenResult
{
    public string? RefreshToken { get; }
    public string? AccessToken { get; }
    public Guid? UserId { get; }
    public string? Email { get; }
    public bool IsSuccess { get; }
    public TokenErrorType Error { get; }


    private TokenResult(
        string? refreshToken,
        string? accessToken,
        Guid? userId,
        string? email, 
        bool isSuccess, 
        TokenErrorType error)
    {
        RefreshToken = refreshToken;
        AccessToken = accessToken;
        UserId = userId;
        Email = email;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static TokenResult Success(
        string refreshToken, 
        string accessToken,
        Guid? userId,
        string? email)
    {
        return new(
            refreshToken, 
            accessToken,
            userId,
            email,
            true, 
            TokenErrorType.None
        );
    }
    
    public static TokenResult Failure(TokenErrorType error)
        => new(null, null, null, null, false, error);
}

public enum TokenErrorType
{
    None,
    InvalidToken,
    InvalidCredentials,
    CompromisedToken,
    UserNotFound,
    UserAlreadyExists
}