namespace Blue.Cosacs.Merchandising.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Models;

    public interface IUserRepository
    {
        IEnumerable<UserViewModel> Get();

        UserViewModel Get(int userId);

        IEnumerable<UserPermissionsViewModel> Permissions(int userId);
    }

    public class UserRepository : IUserRepository
    {
        public IEnumerable<UserViewModel> Get()
        {
            using (var scope = Context.Read())
            {
                const int CanReceiveId = (int)MerchandisingPermissionEnum.UserCanReceiveGoods;
                var perms = scope.Context.UserPermissionsView.Where(x => x.PermissionId == CanReceiveId);
                return Mapper.Map<IEnumerable<UserViewModel>>(perms);
            }
        }

        public UserViewModel Get(int userId)
        {
            using (var scope = Context.Read())
            {
                var perms = scope.Context.UserPermissionsView.Where(x => x.UserId == userId);
                var user = Mapper.Map<UserViewModel>(Enumerable.First(perms));
                user.PermissionIds = perms.Select(p => p.PermissionId).ToList();
                return user;
            }
        }

        public IEnumerable<UserPermissionsViewModel> Permissions(int userId)
        {
            using (var scope = Context.Read())
            {
                return Mapper.Map<IEnumerable<UserPermissionsViewModel>>(scope.Context.UserPermissionsView.Where(x => x.UserId == userId));
            }
        }
    }
}