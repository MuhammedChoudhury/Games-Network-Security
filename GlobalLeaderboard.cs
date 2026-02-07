using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class GlobalLeaderboard : MonoBehaviour
{
    int maxResults = 5;
    public LeaderboardPopup leaderboardPopup;

    public void SubmitScore(int playerScore)
    {
        

        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate()
                {
                    StatisticName = "Most Kills",
                    Value = playerScore,
                }
            }

        };
         PlayFabClientAPI.UpdatePlayerStatistics(request, PlayFabUpdateStatsResult, PlayFabUpdateStatusError); 
    }

    // public class UpdatePlayerStatisticsRequest : PlayFabRequestCommon
    // {
    //     // / <summary>
    //     // / The optional custom tags associated with the request (e.g. build number, )
    //     // / </summary>
    //     public Dictionary<string,string> CustomTags;
    //     // / <summary>
    //     // / Statistics to be updated with the provided values
    //     // / </summary>
    //     public List<StatisticUpdate> Statistics;
    // }

    // public class StatisticUpdate : PlayFabBaseModel
    // {
    //     // / <summary>
    //     // / unique name of the statistic
    //     // / </summary>
    //     public string StatisticName;
    //     // / <summary>
    //     // / statistic value for the player
    //     // / </summary>
    //     public int Value;
    //     // / <summary>
    //     // / for updates to an existing statistic value for a player, the version of the statistic when it was loaded. Null when
    //     // / setting the statistic value for the first time.
    //     // / </summary>
    //     public uint? Version;

    // }

    void PlayFabUpdateStatsResult(UpdatePlayerStatisticsResult updatePlayerStatisticsResult)
    {
        Debug.Log("PlayFab - Score submitted.");
    }

    void PlayFabUpdateStatusError(PlayFabError updatePlayerStatisticsError)
    {
        Debug.Log("PlayFab - Error occured while submitting score: " + updatePlayerStatisticsError.ErrorMessage);
    } 

    public void GetLeaderboard()
    {
        GetLeaderboardRequest request = new GetLeaderboardRequest()
        {
            MaxResultsCount = maxResults,
            StatisticName = "Most Kills",
        };
        PlayFabClientAPI.GetLeaderboard(request, PlayFabGetLeaderboardResult, PlayFabGetLeaderBoardError);
    }

    // void GetLeaderboard()
    // {
    //     GetLeaderboardRequest request = new GetLeaderboardRequest()
    //     {

    //     };
        // /PlayFabClientAPI.GetLeaderboard();
    // }

    // public class GetLeaderboardRequest : PlayFabRequestCommon
    // {
    //     // / <summary>
    //     // / The optional custom tags associated with the request (e.g. build)
    //     // / </summary>
    //     public Dictionary<string, string> CustomTags;
    //     // / <summary>
    //     // / Maximumn number of entries to retrieve. Default 10, maximum 100.
    //     // / </summary>
    //     public int? MaxResultsCount;
    //     // / <summary>
    //     // / If non-null, this determines which properties of the resulting pl
    //     // / only allowed client profile properties for the title may be r
    //     // / the Game Manager "Client Profile Options" tab in the "Settings" s
    //     // / </summary>
    //     public PlayerProfileViewConstraints ProfileConstraints;
    //     // / <summary>
    //     // / Position in the leaderboard to start this listing (defaults to th)
    //     // / </summary>
    //     public int StartPosition;
    //     // / <summary>
    //     // / Statistic used to rank player for this leaderboard
    //     // / </summary>
    //     public string StatisticName;
    //     // / <summary>
    //     // / The version of the leaderboard to get.
    //     // / </summary>
    //     public int? Version;
    // }

    void PlayFabGetLeaderboardResult(GetLeaderboardResult getLeaderboardResult)
    {
        Debug.Log("PlayFab - Get Leaderboard completed.");
        leaderboardPopup.UpdateUI(getLeaderboardResult.Leaderboard);

    }

    void PlayFabGetLeaderBoardError(PlayFabError getLeaderboardError)
    {
        Debug.Log("PlayFab - Error occured while getting Leaderboard: " + getLeaderboardError.ErrorMessage);
    }

    
 

}
