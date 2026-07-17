using FBSC.ODMS.Web.Areas.Admin.Queries.Roles;
using FBSC.ODMS.Web.Areas.Admin.Queries.Users;
using MediatR;

namespace FBSC.ODMS.Web.Helper
{
    public class UserHelper(IMediator mediator)
    {
        public async Task<string> GetUserName(string userId)
        {
            string userName = "[User Not Found]";
            _ = (await (mediator.Send(new GetUserByIdQuery(userId)))).Select(l => userName = l.Name!);            
            return userName;
        }
        public async Task<string?> GetUserRole(string? userId)
        {             
            return await mediator.Send(new GetRoleByUserIdQuery(userId));
        }
    }
}
