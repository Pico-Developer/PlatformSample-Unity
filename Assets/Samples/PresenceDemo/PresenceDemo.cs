using System.Linq;
using System.Text;
using LitJson;
using Newtonsoft.Json;
using Pico.Platform.Models;

namespace Pico.Platform.Samples
{
    public class PresenceDemo : CommandFramework
    {
        private UserList cacheUserList;
        private ApplicationInviteList cacheApplicationInviteList;
        private UserRoomList cacheUserRoomList;
        private DestinationList cacheDestinationList;
        private User currentUser;

        public override Fun[] GetFunList()
        {
            return new[]
            {
                new Fun("a", "a : GetLaunchDetail", GetLaunchDetail),
                new Fun("b", "b <packageName> [<deeplink>]: Launch other app", LaunchOtherApp),
                new Fun("c", "c <appId> [<deeplink>]: Launch other app by appId", LaunchOtherAppByAppId),
                new Fun("d", "d : Launch store", LaunchStore),
                new Fun("dd", "dd : Get version", GetVersion),
                new Fun("e", "e <userId1> <userId2> ...:GetInvitableUsers with suggested user list", Presence_GetInvitableUsers),
                new Fun("f", "f : GetUserArrayNextPage", GetUserArrayNextPage),
                new Fun("g", "g <userId> : Send invites to user", Presence_SendInvites),
                new Fun("gg", "gg : LaunchInvitePanel", Presence_LaunchInvitePanel),
                new Fun("h", "h: Get sent invites", Presence_GetSentInvites),
                new Fun("hh", "hh: Get next page of sent invites", Presence_GetSentInvitesNextPage),
                new Fun("i", "i <destination>:Set destination of presence", Presence_SetDestination),
                new Fun("ii", "ii <joinable=0|1> :Set joinable of presence", Presence_SetJoinable),
                new Fun("iii", "iii <lobbySession> :Set LobbySession of presence", Presence_SetLobbySession),
                new Fun("iiii", "iiii <matchSession> :Set MatchSession of presence", Presence_SetMatchSession),
                new Fun("iiiii", "iiiii <extra> :Set Extra of presence", Presence_SetExtra),
                new Fun("j", "j :Clear presence", Presence_Clear),
                new Fun("k", "k <videoPath> <videoThumbPath>: Share video", Presence_ShareVideo),
                new Fun("kk", "kk <image1> <image2> ... :Share video by images", Presence_ShareImages),
                new Fun("l", "l: Get destinations", Presence_GetDestinations),
                new Fun("m", "m: Get next page destinations", Presence_GetNextPageDestinations),
                new Fun("n", "n: Get current user presence info", Presence_GetUser),
            };
        }

        public override void OnInit()
        {
            PresenceService.SetJoinIntentReceivedNotificationCallback(OnJoinIntentChanged);
            ApplicationService.SetLaunchIntentChangedCallback(OnLaunchIntentChanged);
        }

        private void OnLaunchIntentChanged(Message<string> message)
        {
            Log($"LaunchIntentChanged:{JsonConvert.SerializeObject(message.Data)}");
            var launchDetails = ApplicationService.GetLaunchDetails();
            Log($"LaunchDetails:{JsonConvert.SerializeObject(launchDetails)}");
        }

        private void OnJoinIntentChanged(Message<PresenceJoinIntent> message)
        {
            Log($"PresenceJoinIntentChanged: {JsonConvert.SerializeObject(message.Data)}");
        }

        void Presence_GetUser(string[] args)
        {
            Log("call UserService.Get(currentUser.ID) to retrieve presence info");
            if (currentUser == null)
            {
                UserService.GetLoggedInUser().OnComplete(m =>
                {
                    if (m.IsError)
                    {
                        Log($"GetLoggedInUser failed : error={JsonMapper.ToJson(m.Error)}");
                        return;
                    }

                    Log("GetLoggedInUser successfully");
                    currentUser = m.Data;
                    Presence_GetUser(args);
                });
                return;
            }

            UserService.Get(currentUser.ID).OnComplete(m =>
            {
                if (m.IsError)
                {
                    Log($"User.Get failed error={JsonMapper.ToJson(m.Error)}");
                    return;
                }

                Log($"User info:{JsonMapper.ToJson(m.Data)}");
            });
        }

        private void LaunchStore(string[] args)
        {
            ApplicationService.LaunchStore().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"LaunchStore failed :error={JsonMapper.ToJson(msg.Error)}");
                    return;
                }

                Log($"LaunchStore successfully data={msg.Data}");
            });
        }

        private void GetVersion(string[] args)
        {
            ApplicationService.GetVersion().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"Call Application GetVersion error:{JsonConvert.SerializeObject(msg.Error)}");
                    return;
                }

                Log($"ApplicationService.GetVersion result={JsonConvert.SerializeObject(msg.Data)}");
            });
        }

        void Presence_GetDestinations(string[] args)
        {
            Log("GetDestinations");
            PresenceService.GetDestinations().OnComplete(m =>
            {
                if (m.IsError)
                {
                    Log($"PresenceService.GetDestinations failed : {JsonConvert.SerializeObject(m.Error)}");
                    return;
                }

                cacheDestinationList = m.Data;
                Log($"DestinationList:{DestinationList2String(m.Data)}");
            });
        }

        void Presence_GetNextPageDestinations(string[] args)
        {
            if (cacheDestinationList == null)
            {
                Log("Please get the first page.");
                return;
            }

            if (!cacheDestinationList.HasNextPage)
            {
                Log("DestinationList has no next page.");
                return;
            }

            PresenceService.GetNextDestinationListPage(cacheDestinationList).OnComplete(m =>
            {
                if (m.IsError)
                {
                    Log($"GetNextDestinationListPage error : {JsonConvert.SerializeObject(m.Error)}");
                    return;
                }

                cacheDestinationList = m.Data;
                Log($"DestinationList : {DestinationList2String(m.Data)}");
            });
        }

        string DestinationList2String(DestinationList destinationList)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var i in destinationList)
            {
                builder.Append($"ApiName={i.ApiName} DisplayName={i.DisplayName} deeplinkMessage={i.DeeplinkMessage}");
            }

            return builder.ToString();
        }

        void Presence_ShareVideo(string[] args)
        {
            string videoPath = "", videoThumbPath = "";
            if (args.Length == 2)
            {
                videoPath = args[1];
                videoThumbPath = "";
            }
            else if (args.Length == 3)
            {
                videoPath = args[1];
                videoThumbPath = args[2];
            }
            else
            {
                Log($"ShareVideo needs two args");
                return;
            }

            Log($"Share video videoPath={videoPath} videoThumbPath={videoThumbPath}");
            PresenceService.ShareVideo(videoPath, videoThumbPath).OnComplete(msg => { Log($"ShareVideo result :{JsonConvert.SerializeObject(msg.Error)}"); });
        }

        void Presence_ShareImages(string[] args)
        {
            var images = args.Skip(1).ToList();
            if (images.Count == 0)
            {
                Log($"ShareImages need one param");
                return;
            }

            Log($"ShareVideoByImages {string.Join(",", images)}");
            PresenceService.ShareVideoByImages(images).OnComplete(msg => { Log($"ShareVideoByImages result:{JsonConvert.SerializeObject(msg.Error)}"); });
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

        void Presence_GetSentInvitesNextPage(string[] args)
        {
            if (cacheApplicationInviteList == null)
            {
                Log($"Please get ApplicationInviteList first");
                return;
            }

            if (!cacheApplicationInviteList.HasNextPage)
            {
                Log("ApplicationInviteList has no next page");
                return;
            }

            PresenceService.GetNextApplicationInviteListPage(cacheApplicationInviteList).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"Get next page failed：code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log(ApplicationInviteList2String(msg.Data));
                cacheApplicationInviteList = msg.Data;
            });
        }

        void Presence_LaunchInvitePanel(string[] args)
        {
            PresenceService.LaunchInvitePanel().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"Send invites failed:{JsonConvert.SerializeObject(msg.Error)}");
                    return;
                }

                Log($"Launch invite panel successfully !");
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
                    Log($"Send invites failed:{JsonConvert.SerializeObject(msg.Error)}");
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
                    Log($"GetInvitableUsers failed:{JsonConvert.SerializeObject(msg.Error)}");
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
                cacheApplicationInviteList = msg.Data;
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

            var packageName = args[1];
            var option = new ApplicationOptions();
            if (args.Length == 3)
            {
                var deeplink = args[2];
                option.SetDeeplinkMessage(deeplink);
            }

            Log($"LaunchApp:{packageName}");
            ApplicationService.LaunchApp(packageName, option).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"LaunchApp error:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log($"LaunchApp {msg.Data}");
            });
        }

        void LaunchOtherAppByAppId(string[] args)
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
            ApplicationService.LaunchAppByAppId(appId, option).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"LaunchApp error:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                Log($"LaunchApp {msg.Data}");
            });
        }

        void GetLaunchDetail(string[] args)
        {
            var detail = ApplicationService.GetLaunchDetails();
            Log($"{JsonConvert.SerializeObject(detail)}");
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
                    Log($"Get next page failed：{JsonConvert.SerializeObject(msg.Error)}");
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