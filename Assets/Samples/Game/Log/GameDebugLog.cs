using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Text;
using Pico.Platform.Models;

namespace Pico.Platform.Samples.Game
{
    public class GameDebugLog
    {
        public static string GetRoomLogData(Models.Room room)
        {
            string str = "Room[\nDataStore:\n";
            foreach (var item in room.DataStore)
            {
                str += $"DataStore key: {item.Key}, value: {item.Value}\n";
            }

            str += $"Description: {room.Description}, ID: {room.RoomId}, IsMembershipLocked: {room.IsMembershipLocked}, JoinPolicy: {room.RoomJoinPolicy}, " +
                   $"Joinability: {room.RoomJoinability}, MaxUsers: {room.MaxUsers}, Type: {room.RoomType}, Name: {room.Name}\n";
            if (room.OwnerOptional == null)
            {
                str += "OwnerOptional is null\n";
            }
            else
            {
                str += $"{GetUserLogData(room.OwnerOptional)}\n";
            }

            if (room.UsersOptional == null)
            {
                str += "UsersOptional is null\n";
            }
            else
            {
                str += $"{GetUserListLogData(room.UsersOptional)}\n";
            }

            return str + "\n]Room";
        }

        public static string GetMatchmakingAdminSnapshotLogData(MatchmakingAdminSnapshot obj)
        {
            return $"MatchmakingAdminSnapshot[\nCandidates: {GetMatchmakingAdminSnapshotCandidateListLogData(obj.CandidateList)}\nMyCurrentThreshold: {obj.MyCurrentThreshold}\n]MatchmakingAdminSnapshot";
        }

        public static string GetMatchmakingAdminSnapshotCandidateLogData(MatchmakingAdminSnapshotCandidate obj)
        {
            return $"MatchmakingAdminSnapshotCandidate[\nCanMatch: {obj.CanMatch}, MyTotalScore: {obj.MyTotalScore}, TheirCurrentThreshold: {obj.TheirCurrentThreshold}\n]MatchmakingAdminSnapshotCandidate";
        }

        public static string GetMatchmakingAdminSnapshotCandidateListLogData(MatchmakingAdminSnapshotCandidateList obj)
        {
            string log = $"MatchmakingAdminSnapshotCandidateList[\nCapacity: {obj.Capacity}, TotalCount: {obj.TotalCount}";
            var list = obj.GetEnumerator();
            while (list.MoveNext())
            {
                var item = list.Current;
                log += $"{GetMatchmakingAdminSnapshotCandidateLogData(item)}\n";
            }

            return log + "]MatchmakingAdminSnapshotCandidateList";
        }

        public static string GetMatchmakingEnqueueResultLogData(MatchmakingEnqueueResult obj)
        {
            string log = "MatchmakingEnqueueResult[\n";
            if (obj.AdminSnapshotOptional == null)
            {
                log += "AdminSnapshotOptional: null\n";
            }
            else
            {
                log += $"AdminSnapshotOptional: {GetMatchmakingAdminSnapshotLogData(obj.AdminSnapshotOptional)}\n";
            }

            log += $"AverageWait: {obj.AverageWait}, MatchesInLastHourCount: {obj.MatchesInLastHourCount}, MaxExpectedWait: {obj.MaxExpectedWait}, " +
                   $"Pool: {obj.Pool}, RecentMatchPercentage: {obj.RecentMatchPercentage}";
            return log + "\n]MatchmakingEnqueueResult";
        }

        public static string GetMatchmakingEnqueueResultAndRoomLogData(MatchmakingEnqueueResultAndRoom obj)
        {
            return $"MatchmakingEnqueueResultAndRoom[\nMatchmakingEnqueueResult: {GetMatchmakingEnqueueResultLogData(obj.MatchmakingEnqueueResult)}\nRoom: {GetRoomLogData(obj.Room)}\n]MatchmakingEnqueueResultAndRoom";
        }

        public static string GetMatchmakingRoomLogData(MatchmakingRoom obj)
        {
            return $"MatchmakingRoom[\nRoom: {GetRoomLogData(obj.Room)}\nPingTime: {obj.PingTime}\nHasPingTime: {obj.HasPingTime}\n]MatchmakingRoom";
        }

        public static string GetMatchmakingRoomListLogData(MatchmakingRoomList obj)
        {
            string log = $"MatchmakingRoomList[\nCapacity: {obj.Capacity}, TotalCount: {obj.TotalCount}";
            var list = obj.GetEnumerator();
            while (list.MoveNext())
            {
                var item = list.Current;
                log += $"{GetMatchmakingRoomLogData(item)}\n";
            }

            log += "]MatchmakingRoomList";
            return log;
        }

        public static string GetMatchmakingStatsLogData(MatchmakingStats obj)
        {
            return $"MatchmakingStats[\nDrawCount: {obj.DrawCount}, LossCount: {obj.LossCount}, SkillLevel: {obj.SkillLevel}, SkillMean: {obj.SkillMean}, " +
                   $"SkillStandardDeviation: {obj.SkillStandardDeviation}, WinCount: {obj.WinCount}\n]MatchmakingStats";
        }

        public static string GetRoomListLogData(RoomList obj)
        {
            string log = $"RoomList[\nTotalCount: {obj.TotalCount}, CurIndex: {obj.CurIndex}, PageSize: {obj.PageSize}, Capacity: {obj.Capacity}, NextPageParam: {obj.NextPageParam}, HasNextPage: {obj.HasNextPage}\n";
            var list = obj.GetEnumerator();
            while (list.MoveNext())
            {
                var item = list.Current;
                log += $"{GetRoomLogData(item)}\n";
            }

            return log + "\n]RoomList";
        }

        public static string GetUserLogData(Models.User obj, bool printSingleLog = false)
        {
            string log =
                $"User[DisplayName: {obj.DisplayName}, ID: {obj.ID}, InviteToken: {obj.InviteToken}, ImageURL: {obj.ImageUrl}, " +
                $"PresenceStatus: {obj.PresenceStatus}, Gender: {obj.Gender}]User";
            if (printSingleLog)
            {
                Debug.Log("GetUserLogData:" + log);
            }
            return log;
        }

        public static string GetUserListLogData(UserList obj, bool printSingleLog = false)
        {
            if (obj == null)
                return "UserList[null]";
            string log = $"UserList[Count: {obj.Count}\n";
            var list = obj.GetEnumerator();
            while (list.MoveNext())
            {
                var item = list.Current;
                log += $"{GetUserLogData(item, printSingleLog)}\n";
            }

            return log + "]UserList";
        }

        public static string GetLeaderboardLogData(Leaderboard leaderboard)
        {
            string str = $"Leaderboard[ApiName: {leaderboard.ApiName}, ID: {leaderboard.ID}\n";
            if (leaderboard.DestinationOptional == null)
            {
                str += "DestinationOptional is null\n";
            }
            else
            {
                str += $"DestinationOptional: {GetDestinationLogData(leaderboard.DestinationOptional)}\n";
            }

            return str + "\n]Leaderboard";
        }

        public static string GetDestinationLogData(Destination destination)
        {
            string str = $"Destination[ApiName: {destination.ApiName}, ID: {destination.DeeplinkMessage}, DisplayName: {destination.DisplayName}";
            return str + "\n]Destination";
        }

        public static string GetLeaderboardEntryLogData(LeaderboardEntry leaderboardEntry)
        {
            string str = $"LeaderboardEntry[DisplayScore: {leaderboardEntry.DisplayScore}, ID: {leaderboardEntry.ID}" +
                         $", Rank: {leaderboardEntry.Rank}, Score: {leaderboardEntry.Score}, ExtraData: {Encoding.UTF8.GetString(leaderboardEntry.ExtraData)}" +
                         $", Timestamp: {leaderboardEntry.Timestamp}, User: [{GetUserLogData(leaderboardEntry.User)}]\n";
            if (leaderboardEntry.SupplementaryMetricOptional == null)
            {
                str += "SupplementaryMetricOptional is null\n";
            }
            else
            {
                str += $"SupplementaryMetricOptional: {GetSupplementaryMetricLogData(leaderboardEntry.SupplementaryMetricOptional)}\n";
            }

            return str + "\n]LeaderboardEntry";
        }

        public static string GetSupplementaryMetricLogData(SupplementaryMetric supplementaryMetric)
        {
            string str = $"[Metric: {supplementaryMetric.Metric}, ID: {supplementaryMetric.ID}\n";
            return str + "\n]";
        }

        public static string GetLeaderboardListLogData(LeaderboardList obj)
        {
            string log = $"LeaderboardList[\nTotalCount: {obj.TotalCount}, Capacity: {obj.Capacity}, NextPageParam: {obj.NextPageParam}, HasNextPage: {obj.HasNextPage}\n";
            var list = obj.GetEnumerator();
            int i = 0;
            string itemsLog = "";
            while (list.MoveNext())
            {
                i++;
                var item = list.Current;
                itemsLog += $"{GetLeaderboardLogData(item)}\n";
                if (i % 10 == 0)
                {
                    // too many log
                    Debug.Log($"LeaderboardList too many logs[{i - 9}-{i}]:{itemsLog}");
                    itemsLog = "";
                }
            }

            return $"{log}{itemsLog}\n]LeaderboardList";
        }

        public static string GetLeaderboardEntryListLogData(LeaderboardEntryList obj)
        {
            string log = $"LeaderboardEntryList[\nCapacity: {obj.Capacity}, TotalCount: {obj.TotalCount}, NextPageParam: {obj.NextPageParam}, HasNextPage: {obj.HasNextPage}, PreviousPageParam: " +
                         $"{obj.PreviousPageParam}, HasPreviousPage: {obj.HasPreviousPage}\n";
            var list = obj.GetEnumerator();
            int i = 0;
            string itemsLog = "";
            while (list.MoveNext())
            {
                i++;
                var item = list.Current;
                itemsLog += $"{GetLeaderboardEntryLogData(item)}\n";
                if (i % 10 == 0)
                {
                    // too many log
                    Debug.Log($"LeaderboardList too many logs[{i - 9}-{i}]:{itemsLog}");
                    itemsLog = "";
                }
            }

            return $"{log}{itemsLog}\n]LeaderboardEntryList";
        }

        // achievement
        public static string GetAchievementUpdateLogData(AchievementUpdate data)
        {
            string str = $"AchievementUpdate[Name: {data.Name}, JustUnlocked: {data.JustUnlocked}]AchievementUpdate";
            return str;
        }

        public static string GetAchievementDefinitionListLogData(AchievementDefinitionList obj)
        {
            string log = $"AchievementDefinitionList[\nTotalSize: {obj.TotalSize}, Capacity: {obj.Capacity}, NextPageParam: {obj.NextPageParam}, HasNextPage: {obj.HasNextPage}\n";
            var list = obj.GetEnumerator();
            int i = 0;
            string itemsLog = "";
            while (list.MoveNext())
            {
                i++;
                var item = list.Current;
                itemsLog += $"{GetAchievementDefinitionLogData(item)}\n";
                if (i % 10 == 0)
                {
                    // too many log
                    Debug.Log($"AchievementDefinitionList too many logs[{i - 9}-{i}]:{itemsLog}");
                    itemsLog = "";
                }
            }

            return $"{log}{itemsLog}\n]AchievementDefinitionList";
        }

        public static string GetAchievementDefinitionLogData(AchievementDefinition obj)
        {
            string str = $"AchievementDefinition[Name: {obj.Name}, Target: {obj.Target}, Type: {obj.Type}, BitfieldLength: {obj.BitfieldLength}, " +
                         $"Description: {obj.Description}, Title: {obj.Title}, IsArchived: {obj.IsArchived}, " +
                         $"IsSecret: {obj.IsSecret}, ID: {obj.ID}, UnlockedDescription: {obj.UnlockedDescription}, " +
                         $"WritePolicy: {obj.WritePolicy}, LockedImageURL: {obj.LockedImageURL}, UnlockedImageURL: {obj.UnlockedImageURL}]AchievementDefinition";
            return str;
        }

        public static string GetAchievementProgressListLogData(AchievementProgressList obj)
        {
            string log = $"AchievementProgressList[\nTotalSize: {obj.TotalSize}, Capacity: {obj.Capacity}, NextPageParam: {obj.NextPageParam}, HasNextPage: {obj.HasNextPage}\n";
            var list = obj.GetEnumerator();
            int i = 0;
            string itemsLog = "";
            while (list.MoveNext())
            {
                i++;
                var item = list.Current;
                itemsLog += $"{GetAchievementProgressLogData(item)}\n";
                if (i % 10 == 0)
                {
                    // too many log
                    Debug.Log($"AchievementProgressList too many logs[{i - 9}-{i}]:{itemsLog}");
                    itemsLog = "";
                }
            }

            return $"{log}{itemsLog}\n]AchievementProgressList";
        }

        public static string GetAchievementProgressLogData(AchievementProgress obj)
        {
            string unlockTime = obj.IsUnlocked ? $"UnlockTime: {obj.UnlockTime}, " : "";
            string str = $"AchievementProgress[ID: {obj.ID}, Name: {obj.Name}, Bitfield: {obj.Bitfield}, Count: {obj.Count}, " +
                         $"IsUnlocked: {obj.IsUnlocked}, {unlockTime}ExtraData: {Encoding.UTF8.GetString(obj.ExtraData)}]AchievementProgress";
            return str;
        }
        public static string GetRoomInviteNotificationListLogData(RoomInviteNotificationList obj)
        {
            string log = $"RoomInviteNotificationList[TotalCount: {obj.TotalCount}, Capacity: {obj.Capacity}, NextPageParam: {obj.NextPageParam}, HasNextPage: {obj.HasNextPage}\n";
            var list = obj.GetEnumerator();
            while (list.MoveNext())
            {
                var item = list.Current;
                log += $"{GetRoomInviteNotificationLogData(item)}\n";
            }

            return log + "]RoomInviteNotificationList";
        }

        public static string GetRoomInviteNotificationLogData(RoomInviteNotification obj)
        {
            string log =$"RoomInviteNotification[RoomID: {obj.RoomID}, ID: {obj.ID}, SenderID: {obj.SenderID}, SentTime: {obj.SentTime}]RoomInviteNotification";
            return log;
        }
        
        // challenge
        public static string GetLogData(Challenge obj)
        {
            string str =
                $"Challenge[CreationType: {obj.CreationType}, ID: {obj.ID}, Leaderboard: {GetLeaderboardLogData(obj.Leaderboard)}, " +
                $"Title: {obj.Title}, InvitedUsersOptional: {GetUserListLogData(obj.InvitedUsersOptional)}, " +
                $"Visibility: {obj.Visibility}, ParticipantsOptional: {GetUserListLogData(obj.ParticipantsOptional)}, " +
                $"StartDate: {GetDateTime(obj.StartDate)}, EndDate: {GetDateTime(obj.EndDate)}]Challenge";
            return str;
        }

        public static string GetDateTime(DateTime dateTime)
        {
            if (TimeUtil.DateTimeToSeconds(dateTime) == 0)
            {
                return "0";
            }
            else
            {
                return $"{dateTime}";
            }
        }
        public static string GetLogData(ChallengeEntry obj)
        {
            string str = $"ChallengeEntry[DisplayScore: {obj.DisplayScore}, ID: {obj.ID}, Rank: {obj.Rank}, " +
                         $"Score: {obj.Score}, ExtraData: {Encoding.UTF8.GetString(obj.ExtraData)}, " +
                         $"User: {GetUserLogData(obj.User)}, " +
                         $"Timestamp: {GetDateTime(obj.Timestamp)}]ChallengeEntry";
            return str;
        }
        

        public static string GetLogData(object o)
        {
            if (o is Challenge)
            {
                return GetLogData(o as Challenge);
            }
            
            if (o as ChallengeList != null)
            {
                return GetListLogData<Challenge>(o as ChallengeList, "ChallengeList");
            }
            
            if (o as ChallengeEntry != null)
            {
                return GetLogData(o as ChallengeEntry);
            }
            
            if (o as ChallengeEntryList != null)
            {
                return GetListLogData<ChallengeEntry>(o as ChallengeEntryList, "ChallengeEntryList");
            }

            return "cannot recognize input object";
        }
        public static string GetListLogData<T>(object o, string typeName)
        {
            var obj = o as MessageArray<T>;
            string log = $"{typeName}[Capacity: {obj.Capacity}\n";
            var list = obj.GetEnumerator();
            while (list.MoveNext())
            {
                var item = list.Current;
                log += $"{GetLogData(item)}\n";
            }

            return log + $"]{typeName}";
        }
    }
}