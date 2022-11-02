using System;
using System.Collections;
using System.Collections.Generic;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Pico.Platform.Samples
{
    public class ChallengeOptionsItem : MonoBehaviour
    {
        public InputField LeaderboardName;

        public InputField Title;

        public Dropdown ViewerFilter;

        public Dropdown Visibility;

        public InputField StartDate;

        public InputField EndDate;

        public Toggle IncludeActive;

        public Toggle IncludeFuture;

        public Toggle IncludePast;

        public ChallengeOptions GetOption()
        {
            ChallengeOptions options = new ChallengeOptions();
            options.SetTitle(Title.text);
            options.SetVisibility((ChallengeVisibility)Visibility.value);
            if (!string.IsNullOrEmpty(EndDate.text))
            {
                options.SetEndDate(Util.SecondsToDateTime(Convert.ToInt64(EndDate.text)));
            }

            if (!string.IsNullOrEmpty(StartDate.text))
            {
                options.SetStartDate(Util.SecondsToDateTime(Convert.ToInt64(StartDate.text)));
            }
            
            options.SetLeaderboardName(LeaderboardName.text);
            options.SetViewerFilter((ChallengeViewerFilter)ViewerFilter.value);
            options.SetIncludeActiveChallenges(IncludeActive.isOn);
            options.SetIncludeFutureChallenges(IncludeFuture.isOn);
            options.SetIncludePastChallenges(IncludePast.isOn);
            return options;
        }
    }
}

