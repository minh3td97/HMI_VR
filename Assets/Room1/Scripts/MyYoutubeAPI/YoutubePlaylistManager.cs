using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;


public class YoutubePlaylistManager : YoutubeAPIManager
{
    private YoutubeData data;
    
              
    private YoutubePlaylistItems[] playslistItems;
    

    //REMEMBER TO CHANGE HERE IF YOU NEED TO POINT TO YOUR GOOGLE APP. 
    /* 
     * TO CREATE YOUR GOOGLE APP AND USE YOUR API GO TO:
     * https://code.google.com/apis/console
     * -Create a project.
     * -Go to APIs -> YouTube APIs -> YouTube Data API and enable that.
     * -then go to credentials create a new key for Public API access
     * - now you have the API key just copy and change the variable APIKey with your new API Key to monitor the use of youtube api calls.
     * Any question? mail me: kelvinparkour@gmail.com
     * 
     * */

    private const string APIKey = "AIzaSyAUA3aDR2ZHrr6tQ3Jwmr_Pc24bysC32vY";

    public void GetPlaylistItems(string playlistId,int maxResults, Action<YoutubePlaylistItems[]> callback)
    {
        StartCoroutine(YoutubeCallPlaylist(playlistId, maxResults, callback));
    }
 

    IEnumerator YoutubeCallPlaylist(string playlistId,int maxResults, Action<YoutubePlaylistItems[]> callback)
    {
        WWW call = new WWW("https://www.googleapis.com/youtube/v3/playlistItems/?playlistId="+ playlistId + "&maxResults="+maxResults+"&part=snippet%2CcontentDetails&key="+APIKey);
        yield return call;
        Debug.Log(call.url);
        JSONNode result = JSON.Parse(call.text);
        int totalResults = result["pageInfo"]["totalResults"];
        string nextPageToken = result["nextPageToken"];

        playslistItems = new YoutubePlaylistItems[totalResults];
        for (int itemsCounter = 0; itemsCounter < totalResults; itemsCounter++)
        {
            playslistItems[itemsCounter] = new YoutubePlaylistItems();
            playslistItems[itemsCounter].videoId = result["items"][itemsCounter % maxResults]["snippet"]["resourceId"]["videoId"];
            SetSnippet(result["items"][itemsCounter % maxResults]["snippet"], out playslistItems[itemsCounter].snippet);
            int checkZero = (itemsCounter + 1) % maxResults;
            if(checkZero == 0 )
            {
                call = new WWW("https://www.googleapis.com/youtube/v3/playlistItems/?playlistId="+ playlistId + "&maxResults="+maxResults+"&pageToken="+nextPageToken+"&part=snippet%2CcontentDetails&key="+APIKey);
                yield return call;
                Debug.Log(call.url);
                result = JSON.Parse(call.text);
                nextPageToken = result["nextPageToken"];   
            }                                                   
        }
        callback.Invoke(playslistItems);
    }

    private void SetSnippet(JSONNode resultSnippet, out YoutubeSnippet data)
    {
        data = new YoutubeSnippet();
        data.publishedAt = resultSnippet["publishedAt"];
        data.channelId = resultSnippet["channelId"];
        data.title = resultSnippet["title"];
        data.description = resultSnippet["description"];
        //Thumbnails
        data.thumbnails = new YoutubeTumbnails();
        data.thumbnails.defaultThumbnail = new YoutubeThumbnailData();
        data.thumbnails.defaultThumbnail.url = resultSnippet["thumbnails"]["default"]["url"];
        data.thumbnails.defaultThumbnail.width = resultSnippet["thumbnails"]["default"]["width"];
        data.thumbnails.defaultThumbnail.height = resultSnippet["thumbnails"]["default"]["height"];
        data.thumbnails.mediumThumbnail = new YoutubeThumbnailData();
        data.thumbnails.mediumThumbnail.url = resultSnippet["thumbnails"]["medium"]["url"];
        data.thumbnails.mediumThumbnail.width = resultSnippet["thumbnails"]["medium"]["width"];
        data.thumbnails.mediumThumbnail.height = resultSnippet["thumbnails"]["medium"]["height"];
        data.thumbnails.highThumbnail = new YoutubeThumbnailData();
        data.thumbnails.highThumbnail.url = resultSnippet["thumbnails"]["high"]["url"];
        data.thumbnails.highThumbnail.width = resultSnippet["thumbnails"]["high"]["width"];
        data.thumbnails.highThumbnail.height = resultSnippet["thumbnails"]["high"]["height"];
        data.thumbnails.standardThumbnail = new YoutubeThumbnailData();
        data.thumbnails.standardThumbnail.url = resultSnippet["thumbnails"]["standard"]["url"];
        data.thumbnails.standardThumbnail.width = resultSnippet["thumbnails"]["standard"]["width"];
        data.thumbnails.standardThumbnail.height = resultSnippet["thumbnails"]["standard"]["height"];
        data.channelTitle = resultSnippet["channelTitle"];
        //TAGS
        data.tags = new string[resultSnippet["tags"].Count];
        for (int index = 0; index < data.tags.Length; index++)
        {
            data.tags[index] = resultSnippet["tags"][index];
        }
        data.categoryId = resultSnippet["categoryId"];
    }
    private void SetStatistics(JSONNode resultStatistics, out YoutubeStatistics data)
    {
        data = new YoutubeStatistics();
        data.viewCount = resultStatistics["viewCount"];
        data.likeCount = resultStatistics["likeCount"];
        data.dislikeCount = resultStatistics["dislikeCount"];
        data.favoriteCount = resultStatistics["favoriteCount"];
        data.commentCount = resultStatistics["commentCount"];
    }
    private void SetContentDetails(JSONNode resultContentDetails, out YoutubeContentDetails data)
    {
        data = new YoutubeContentDetails();
        data.duration = resultContentDetails["duration"];
        data.dimension = resultContentDetails["dimension"];
        data.definition = resultContentDetails["definition"];
        data.caption = resultContentDetails["caption"];
        data.licensedContent = resultContentDetails["licensedContent"];
        data.projection = resultContentDetails["projection"];

        if(resultContentDetails["contentRating"] != null)
        {
            Debug.Log("Age restrict found!");
            if (resultContentDetails["contentRating"]["ytRating"] == "ytAgeRestricted")
                data.ageRestrict = true;
            else
                data.ageRestrict = false;
        }
        else
            data.ageRestrict = false;

    }
}