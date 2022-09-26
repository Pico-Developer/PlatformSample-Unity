using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Pico.Platform;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Samples.UserDemo
{
    delegate void Handler(string[] args);

    class Fun
    {
        public Handler handler;
        public string desc;
        public string key;

        public Fun(string key, string desc, Handler handler)
        {
            this.key = key;
            this.desc = desc;
            this.handler = handler;
        }
    }

    public class UserDemo : MonoBehaviour
    {
        public Text dataOutput;
        public Text commandList;
        public InputField inputField;
        public Button executeButton;
        public bool useAsyncInit = true;

        private Fun[] funList;

        private UserList cacheUserList;
        private ApplicationInviteList applicationInviteList;
        private UserRoomList userRoomList;

        void Start()
        {
            Log($"UseAsyncInit={useAsyncInit}");
            if (useAsyncInit)
            {
                try
                {
                    CoreService.AsyncInitialize().OnComplete(m =>
                    {
                        if (m.IsError)
                        {
                            Log($"Async initialize failed: code={m.GetError().Code} message={m.GetError().Message}");
                            return;
                        }

                        if (m.Data != PlatformInitializeResult.Success && m.Data != PlatformInitializeResult.AlreadyInitialized)
                        {
                            Log($"Async initialize failed: result={m.Data}");
                            return;
                        }

                        Log("AsyncInitialize Successfully");
                    });
                }
                catch (Exception e)
                {
                    Log($"Async Initialize Failed:{e}");
                    return;
                }
            }
            else
            {
                try
                {
                    CoreService.Initialize();
                }
                catch (UnityException e)
                {
                    Log($"Init Platform SDK error:{e}");
                    throw;
                }
            }

            funList = new[]
            {
                new Fun("a", "a : GetAccessToken", GetAccessToken),
                new Fun("aa", "aa : GetLoggedInUser", GetLoggedInUser),
                new Fun("b", "b <userId> : LaunchFriendRequest to <userId>", LaunchFriendRequest),
                //c : 获取我的好友列表
                new Fun("c", "c : GetLoggedInUserFriends", GetLoggedInUserFriends),
                new Fun("d", "d <userId> : GetUser by <userId>", GetUser),
                new Fun("e", "e : GetUserArrayNextPage", GetUserArrayNextPage),
                new Fun("ee", "ee : GetLoggedInUserFriendsAndRooms", GetLoggedInUserFriendAndRooms),
                new Fun("eee", "eee : Get next page of UserAndRoomArray", GetUserAndRoomArrayNextPage),
                new Fun("f", "f : GetLaunchDetail", GetLaunchDetail),
                new Fun("g", "g <appId> [<deeplink>]: Launch other app", LaunchOtherApp),
                new Fun("gg", "gg <userId1> <userId2> ...:GetInvitableUsers with suggested user list", Presence_GetInvitableUsers),
                new Fun("h", "h <userId> : Send invites to user", Presence_SendInvites),
                new Fun("hh", "hh: Get sent invites", Presence_GetSentInvites),
                new Fun("hhh", "hhh: Get next page of sent invites", Presence_GetSentInvitesNextPage),
                new Fun("i", "i <destination>:Set destination of presence", Presence_SetDestination),
                new Fun("ii", "ii <joinable=0|1> :Set joinable of presence", Presence_SetJoinable),
                new Fun("iii", "iii <lobbySession> :Set LobbySession of presence", Presence_SetLobbySession),
                new Fun("iiii", "iiii <matchSession> :Set MatchSession of presence", Presence_SetMatchSession),
                new Fun("iiiii", "iiiii <extra> :Set Extra of presence", Presence_SetExtra),
                new Fun("j", "j :Clear presence", Presence_Clear),
                new Fun("l", "l: Get authorized permissions", User_GetAuthorizedPermissions),
                new Fun("ll", "ll <permission1> <permission2>: Request permissions ", User_RequestPermissions),
                new Fun("z", "z : Change to RTC test Scene", args => { SceneManager.LoadScene("RtcDemo"); }),
            };
            StringBuilder s = new StringBuilder();
            foreach (var i in funList)
            {
                s.AppendLine(i.desc);
            }

            commandList.text = s.ToString();
            executeButton.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            string currentText = inputField.text.Trim();
            if (String.IsNullOrWhiteSpace(currentText))
            {
                return;
            }

            Log($"Got command text {currentText}");
            var args = Regex.Split(currentText, @"\s+");
            if (args.Length == 0)
            {
                Log("Please input a command");
                return;
            }

            var key = args[0];
            var handled = false;
            foreach (var cmd in funList)
            {
                if (cmd.key.Equals(key))
                {
                    try
                    {
                        cmd.handler(args);
                    }
                    catch (Exception e)
                    {
                        Log($"Handle command error:{cmd.desc} e={e}");
                    }

                    handled = true;
                    break;
                }
            }

            if (!handled)
            {
                Log($"Cannot find command for :{key}");
            }

            inputField.text = "";
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
            return $"name={user.DisplayName},ID={user.ID},headImage={user.ImageUrl},presenceStatus={user.PresenceStatus},storeRegion={user.StoreRegion}";
        }

        string ApplicationInvite2String(ApplicationInvite invite)
        {
            return $"Destination={invite.Destination},Recipient=(UserId={invite.Recipient.ID}),Id={invite.ID},isActive={invite.IsActive},LobbySessionId={invite.LobbySessionId},MatchSessionId={invite.MatchSessionId}";
        }

        string ApplicationInviteList2String(ApplicationInviteList inviteList)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"inviteList count:{inviteList.Count}");
            foreach (var invite in inviteList)
            {
                builder.AppendLine(ApplicationInvite2String(invite));
            }

            return builder.ToString();
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

        void Presence_GetSentInvitesNextPage(string[] args)
        {
            if (applicationInviteList == null)
            {
                Log($"Please get ApplicationInviteList first");
                return;
            }

            if (!applicationInviteList.HasNextPage)
            {
                Log("ApplicationInviteList has no next page");
                return;
            }

            PresenceService.GetNextApplicationInviteListPage(applicationInviteList).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"Get next page failed：code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log(ApplicationInviteList2String(msg.Data));
                applicationInviteList = msg.Data;
            });
        }

        void Presence_SendInvites(string[] args)
        {
            if (args.Length != 2)
            {
                Log("SendInvites argument error");
                return;
            }

            Log($"Presence.SendInvites:{args[1]}");
            var userIds = new[] {args[1]};
            PresenceService.SendInvites(userIds).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    var err = msg.GetError();
                    Log($"Send invites failed:code={err.Code} message={err.Message}");
                    return;
                }

                var res = msg.Data;
                if (res == null)
                {
                    Log("SendInvitesResult is empty");
                    return;
                }

                if (res.Invites == null)
                {
                    Log("SendInvitesResult.Invites is empty");
                    return;
                }

                Log(ApplicationInviteList2String(res.Invites));
            });
        }

        void Presence_GetInvitableUsers(string[] args)
        {
            Log($"Presence.GetInvitableUsers:args.length={args.Length}");
            var option = new InviteOptions();
            for (var i = 1; i < args.Length; i++)
            {
                option.AddSuggestedUser(args[i]);
            }

            PresenceService.GetInvitableUsers(option).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    var err = msg.GetError();
                    Log($"GetInvitableUsers failed:code={err.Code} message={err.Message}");
                    return;
                }

                var res = msg.Data;
                if (res == null)
                {
                    Log("UserList is empty");
                    return;
                }

                Log("InvitableUserList:");
                Log(UserList2String(res));
            });
        }

        void Presence_GetSentInvites(string[] args)
        {
            PresenceService.GetSentInvites().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"Presence.GetSentInvites error:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log(ApplicationInviteList2String(msg.Data));
                applicationInviteList = msg.Data;
            });
        }

        void Presence_SetDestination(string[] args)
        {
            if (args.Length != 2)
            {
                Log("Presence.SetDestination argument error");
                return;
            }

            string destination = args[1];
            PresenceService.SetDestination(destination).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"SetDestination result:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log("SetDestination successfully");
            });
        }

        void Presence_SetLobbySession(string[] args)
        {
            if (args.Length != 2)
            {
                Log("Presence.SetDestination argument error");
                return;
            }

            string lobbySession = args[1];
            PresenceService.SetLobbySession(lobbySession).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"SetLobbySession result:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log("SetLobbySession successfully");
            });
        }

        void Presence_SetMatchSession(string[] args)
        {
            if (args.Length != 2)
            {
                Log("Presence.SetMatchSession argument error");
                return;
            }

            string matchSession = args[1];
            PresenceService.SetMatchSession(matchSession).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"SetMatchSession result:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log("SetMatchSession successfully");
            });
        }

        void Presence_SetExtra(string[] args)
        {
            if (args.Length != 2)
            {
                Log("Presence.SetExtra argument error");
                return;
            }

            string extra = args[1];
            PresenceService.SetExtra(extra).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"SetExtra result:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log("SetExtra successfully");
            });
        }

        void Presence_SetJoinable(string[] args)
        {
            if (args.Length != 2)
            {
                Log("SetJoinable argument error");
                return;
            }

            var joinable = args[1];
            if (joinable != "0" && joinable != "1")
            {
                Log("Joinable=0|1");
                return;
            }

            var joinableBoolean = joinable == "1";
            PresenceService.SetIsJoinable(joinableBoolean).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"Set failed:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log("Set Presence.IsJoinable Successfully");
            });
        }

        void Presence_Clear(string[] args)
        {
            PresenceService.Clear().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"Clear Presence failed:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log("Clear Presence Successfully");
            });
        }

        void LaunchOtherApp(string[] args)
        {
            if (args.Length < 2)
            {
                Log("At lease two argument");
                return;
            }

            var appId = args[1];
            var option = new ApplicationOptions();
            if (args.Length == 3)
            {
                var deeplink = args[2];
                option.SetDeeplinkMessage(deeplink);
            }

            Log($"LaunchOtherApp:{appId}");
            ApplicationService.LaunchApp(appId, option).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"LaunchOtherApp error:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log($"LaunchOtherApp {msg.Data}");
            });
        }

        void GetLaunchDetail(string[] args)
        {
            var detail = ApplicationService.GetLaunchDetails();
            Log($"LaunchType:{detail.LaunchType} DeeplinkMessage:{detail.DeeplinkMessage} LaunchSource:{detail.LaunchSource} DestinationApiName:{detail.DestinationApiName} RoomId:{detail.RoomID} TrackingId:{detail.TrackingID} LobbySessionId:{detail.LobbySessionID} MatchSessionId:{detail.MatchSessionID} Users:{detail.Users} Extra:{detail.Extra}");
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

        void Log(String newLine)
        {
            Debug.Log(newLine);
            dataOutput.text = "> " + newLine + Environment.NewLine + dataOutput.text;
            if (dataOutput.text.Length > 1000)
            {
                dataOutput.text = dataOutput.text.Substring(0, 1000);
            }
        }
    }
}