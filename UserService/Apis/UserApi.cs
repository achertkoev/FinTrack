using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Contracts;
using UserService.Domain.Entities;
using UserService.Services;

namespace UserService.Apis;

public static class UserApi
{
    public static IEndpointRouteBuilder MapUserApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/users");
        api.MapPost("create", CreateAsync);

        var auth = api.MapGroup("auth");
        auth.MapPost("", LoginAsync);
            
        return api;
    }
    
    private static async Task<Results<Created, BadRequest<ProblemHttpResult>>> CreateAsync(
        [AsParameters] ServiceManager serviceManager, UserDto user)
    {
        var existUser = await serviceManager.Context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
        if(existUser != null)
            return TypedResults.BadRequest(TypedResults.Problem(title: "Электронная почта уже существует",
                statusCode: 400));

        var passwordHasher = new PasswordHasher<UserDto>();
        var newUser = new User
        {
            Name = user.Name,
            Email = user.Email,
            PasswordHash = passwordHasher.HashPassword(user, user.Password)
        };

        await serviceManager.Context.Users.AddAsync(newUser);
        await serviceManager.Context.SaveChangesAsync();
        return TypedResults.Created();
    }

    private static async Task<Results<Ok<string>, BadRequest<ProblemHttpResult>>> LoginAsync(
        [AsParameters] ServiceManager serviceManager, UserDto userDto)
    {
        var user = await serviceManager.Context.Users
            .FirstOrDefaultAsync(x => x.Email == userDto.Email);

        if (user is null)
        {
            return TypedResults.BadRequest(TypedResults.Problem(title: "Пользователь не найден"));
        }
        
        var passwordHasher = new PasswordHasher<UserDto>();
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(userDto, user.PasswordHash, userDto.Password);
        
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return TypedResults.BadRequest(TypedResults.Problem(title: "Неверный пароль"));
        }  
        
        var token = serviceManager.TokenProvider.Create(user);
        
        return TypedResults.Ok(token);
    }
}