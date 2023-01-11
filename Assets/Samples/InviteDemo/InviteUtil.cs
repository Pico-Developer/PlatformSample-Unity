using PICO.Platform.Samples.Invite.Models;

namespace PICO.Platform.Samples.Invite
{
    public static class InviteUtil
    {
        public static User ToManagerUser(Pico.Platform.Models.User user)
        {
            User u = new User();
            u.Avatar = user.ImageUrl;
            u.NickName = user.DisplayName;
            u.Id = user.ID;
            return u;
        }

        public static User ToMyUser(Pico.Platform.Models.User u)
        {
            var it = new User();
            it.Avatar = u.ImageUrl;
            it.NickName = u.DisplayName;
            it.Id = u.ID;
            return it;
        }
    }
}