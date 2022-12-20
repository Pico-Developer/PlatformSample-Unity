/*******************************************************************************
Copyright © 2015-2022 Pico Technology Co., Ltd.All rights reserved.

NOTICE：All information contained herein is, and remains the property of
Pico Technology Co., Ltd. The intellectual and technical concepts
contained herein are proprietary to Pico Technology Co., Ltd. and may be
covered by patents, patents in process, and are protected by trade secret or
copyright law. Dissemination of this information or reproduction of this
material is strictly forbidden unless prior written permission is obtained from
Pico Technology Co., Ltd.
*******************************************************************************/

using System;

namespace Pico.Platform.Models
{
    /**
     * \ingroup Models
     */
    /// <summary>App launch details.</summary>
    public class LaunchDetails
    {
        /** @brief Deeplink message. You can pass a deeplink when you call  \ref ApplicationService.LaunchApp, and the other app will receive the deeplink. */
        public readonly string DeeplinkMessage;

        /** @brief Destination API name configured on the PICO Developer Platform. */
        public readonly string DestinationApiName;


        /** @brief The lobby session ID that identifies a group or team. */
        public readonly string LobbySessionID;

        /** @brief The match session ID that identifies a competition. */
        public readonly string MatchSessionID;

        /** @brief The customized extra presence info. */
        public readonly string Extra;

        /** @brief Room ID. */
        public readonly UInt64 RoomID;
        
        /** @brief Challenge ID. */
        public readonly UInt64 ChallengeID;

        /** @brief Tracking ID. */
        public readonly string TrackingID;

        /** @brief User list. */
        public readonly UserList Users;

        /** @brief How the app was launched. */
        public readonly LaunchType LaunchType;
        /** @brief The source where the app is launched. */
        public readonly string LaunchSource;

        public LaunchDetails(IntPtr o)
        {
            DeeplinkMessage = CLIB.ppf_LaunchDetails_GetDeeplinkMessage(o);
            DestinationApiName = CLIB.ppf_LaunchDetails_GetDestinationApiName(o);
            LaunchSource = CLIB.ppf_LaunchDetails_GetLaunchSource(o);
            LobbySessionID = CLIB.ppf_LaunchDetails_GetLobbySessionID(o);
            MatchSessionID = CLIB.ppf_LaunchDetails_GetMatchSessionID(o);
            Extra = CLIB.ppf_LaunchDetails_GetExtra(o);
            RoomID = CLIB.ppf_LaunchDetails_GetRoomID(o);
            ChallengeID = CLIB.ppf_LaunchDetails_GetChallengeID(o);
            TrackingID = CLIB.ppf_LaunchDetails_GetTrackingID(o);
            Users = new UserList(CLIB.ppf_LaunchDetails_GetUsers(o));
            LaunchType = CLIB.ppf_LaunchDetails_GetLaunchType(o);
        }
    }


    public class ApplicationVersion
    {
        public readonly long CurrentCode;
        public readonly string CurrentName;
        public readonly long LatestCode;
        public readonly string LatestName;

        public ApplicationVersion(IntPtr o)
        {
            CurrentCode = CLIB.ppf_ApplicationVersion_GetCurrentCode(o);
            CurrentName = CLIB.ppf_ApplicationVersion_GetCurrentName(o);
            LatestCode = CLIB.ppf_ApplicationVersion_GetLatestCode(o);
            LatestName = CLIB.ppf_ApplicationVersion_GetLatestName(o);
        }
    }
}