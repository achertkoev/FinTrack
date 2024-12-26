using UserService.Infrastructure.Persistent;

namespace UserService.Services;

public class ServiceManager(UserDbContext context, TokenProvider tokenProvider)
{
    public UserDbContext Context { get; } = context;
    public TokenProvider TokenProvider { get; } = tokenProvider;
}