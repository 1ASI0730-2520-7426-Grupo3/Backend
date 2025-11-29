using coolgym_webapi.Contexts.Security.Domain.Commands;
using coolgym_webapi.Contexts.Security.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.Security.Domain.Services;

public interface IUserCommandService
{
    Task<User> Handle(CreateUserCommand command);
    Task<(User User, string AccessToken)> Handle(LoginUserCommand command);
}
