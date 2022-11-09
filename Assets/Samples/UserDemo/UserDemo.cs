using System.Linq;
using System.Text;
using Pico.Platform.Models;

namespace Pico.Platform.Samples.UserDemo
{
    public class UserDemo : CommandFramework
    {
        private UserList cacheUserList;
        private UserRoomList userRoomList;

        public override Fun[] GetFunList()
        {
            return new[]
            {
                new Fun("a", "a : GetAccessToken", GetAccessToken),
                new Fun("aa", "aa : GetLoggedInUser", GetLoggedInUser),
                new Fun("b", "b <userId> : LaunchFriendRequest to <userId>", LaunchFriendRequest),
                new Fun("c", "c : GetLoggedInUserFriends", GetLoggedInUserFriends),
                new Fun("d", "d <userId> : GetUser by <userId>", GetUser),
                new Fun("e", "e : GetUserArrayNextPage", GetUserArrayNextPage),
                new Fun("ee", "ee : GetLoggedInUserFriendsAndRooms", GetLoggedInUserFriendAndRooms),
                new Fun("eee", "eee : Get next page of UserAndRoomArray", GetUserAndRoomArrayNextPage),
                new Fun("l", "l: Get authorized permissions", User_GetAuthorizedPermissions),
                new Fun("ll", "ll <permission1> <permission2>: Request permissions ", User_RequestPermissions),
            };
        }

        string PermissionResult2String(PermissionResult res)
        {
            return $"accessToken={res.AccessToken} permissions={string.Join(",", res.AuthorizedPermissions)} userId={res.UserID}";
        }

        private void User_RequestPermissions(string[] args)
        {
            Log("RequestPermissions");
            if (args.Length == 1)
            {
                Log("Please input a permission");
                return;
            }

            var permissions = args.Skip(1).ToArray();
            UserService.RequestUserPermissions(permissions).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"RequestUserPermissions failed:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log($"Request permission successfully:{PermissionResult2String(msg.Data)}");
            });
        }

        private void User_GetAuthorizedPermissions(string[] args)
        {
            Log($"Getting authorized permissions");
            UserService.GetAuthorizedPermissions().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"获取权限列表失败:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log($"Get authorized permissions successfully:{PermissionResult2String(msg.Data)}");
            });
        }

        string UserList2String(UserList userList)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"UserList:Count{userList.Count} hasNext={userList.HasNextPage} bodyParams={userList.NextPageParam}");
            foreach (var u in userList)
            {
                builder.Append(u.DisplayName).Append(",");
            }

            return builder.ToString();
        }

        string User2String(User user)
        {
            return $"name={user.DisplayName},ID={user.ID},headImage={user.ImageUrl},smallImageUrl={user.SmallImageUrl},presenceStatus={user.PresenceStatus},storeRegion={user.StoreRegion}";
        }

        string UserAndRoom2String(UserRoom userRoom)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"userId:{userRoom.User.ID}");
            if (userRoom.Room != null)
            {
                builder.Append($"roomDescription:{userRoom.Room.Description},roomId={userRoom.Room.RoomId};");
            }
            else
            {
                builder.Append("He isn't in any room");
            }

            return builder.ToString();
        }

        string UserAndRoomList2String(UserRoomList data)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"userAndRoomList Count:{data.Count}");
            foreach (var i in data)
            {
                builder.AppendLine(UserAndRoom2String(i));
            }

            return builder.ToString();
        }

        private void GetLoggedInUserFriendAndRooms(string[] args)
        {
            Log("GetLoggedInUserFriendsAndRooms...");
            UserService.GetFriendsAndRooms().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"Failed to getLoggedInUserFriendsAndRooms:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                userRoomList = msg.Data;
                Log("Got UserAndRoomList successfully");
                Log(UserAndRoomList2String(msg.Data));
            });
        }

        private void GetUserAndRoomArrayNextPage(string[] args)
        {
            if (userRoomList == null)
            {
                Log($"UserAndRoomList is empty，please get UserAndRoomList first");
                return;
            }

            if (!userRoomList.HasNextPage)
            {
                Log($"This is already the last page.");
                return;
            }

            Log("Users.GetNextUserAndRoomListPage...");
            UserService.GetNextUserAndRoomListPage(userRoomList).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"GetNextUserAndRoomListPage failed：code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                userRoomList = msg.Data;
                Log("Get next page UserAndRoomArray successfully");
                Log(UserAndRoomList2String(msg.Data));
            });
        }

        void GetAccessToken(string[] args)
        {
            Log("GetAccessToken...");
            UserService.GetAccessToken().OnComplete(delegate(Message<string> message)
            {
                if (message.IsError)
                {
                    var err = message.GetError();
                    Log($"Got access token error {err.Message} code={err.Code}");
                    return;
                }

                string accessToken = message.Data;
                Log($"Got accessToken {accessToken}");
            });
        }

        void LaunchFriendRequest(string[] args)
        {
            if (args.Length != 2)
            {
                Log("Lack <userId> argument");
                return;
            }

            UserService.LaunchFriendRequestFlow(args[1]).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    var err = msg.GetError();
                    Log($"Launch friend request error:{err.Message};code={err.Code}");
                    return;
                }

                var launchResult = msg.Data;
                Log(
                    $"Launch friend request ok:DidCancel={launchResult.DidCancel},DidSend={launchResult.DidSendRequest}");
            });
        }

        void GetLoggedInUserFriends(string[] args)
        {
            Log($"Trying to get FriendList");
            UserService.GetFriends().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    var err = msg.GetError();
                    Log($"Get Friends error {err.Message} code={err.Code}");
                    return;
                }

                var userList = msg.Data;
                cacheUserList = userList;
                Log($"Your friend list:{UserList2String(userList)}");
            });
        }

        void GetUser(string[] args)
        {
            if (args.Length != 2)
            {
                Log("Argument error");
                return;
            }

            Log($"Trying to get user {args[1]}");
            UserService.Get(args[1]).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    var err = msg.GetError();
                    Log($"Get user info failed:{err.Message} {err.Code}");
                }
                else
                {
                    var usr = msg.Data;
                    Log($"get user info by id={args[1]}：{User2String(usr)}");
                }
            });
        }

        void GetLoggedInUser(string[] args)
        {
            Log("Trying to get currently logged in user");
            UserService.GetLoggedInUser().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log("Received get user error");
                    Error error = msg.GetError();
                    Log($"Error: code={error.Code} message={error.Message}");
                    return;
                }

                Log("Received get user success");
                User user = msg.Data;
                Log($"User: {User2String(user)}");
            });
        }

        void GetUserArrayNextPage(string[] args)
        {
            if (cacheUserList == null)
            {
                Log("userList is empty,please init userList first");
                return;
            }

            if (!cacheUserList.HasNextPage)
            {
                Log("Has no next page");
                return;
            }

            Log("User.GetNextUserListPage...");
            UserService.GetNextUserListPage(cacheUserList).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    var err = msg.GetError();
                    Log($"Get next page failed：{err.Message}");
                    return;
                }

                Log($"Get next page successfully");
                var userList = msg.Data;
                cacheUserList = userList;
                Log($"next page data is ：{UserList2String(userList)}");
            });
        }
    }
}