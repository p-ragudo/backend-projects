namespace ExpenseTrackerApi.Services.Auth;

public record RegisterRequest(
    string Email, 
    string Username, 
    string Password
);

public record LoginRequest(
    string Email, 
    string Password
);

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string Email
);

public record RefreshTokenRequest(
    string RefreshToken
);