using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadPlaylistScript : MonoBehaviour {

    YoutubePlaylistManager youtubeapi;
    //YoutubeAPIManager youtubeapi;
    public Text searchField;
    public Text HeaderText;
    public Text DescriptionField;
    public YoutubeVideoUi[] videoListUI;
    public GameObject videoUIResult;
    public GameObject mainUI;
    [SerializeField] Transform menuPanel;
    [SerializeField] GameObject buttonPrefab;
    private bool loaded = false;

    private YoutubePlaylistItems[] myPlaylist;

    void Start()
    {
        //Get the api component
        youtubeapi = GameObject.FindObjectOfType<YoutubePlaylistManager>();
        if (youtubeapi == null)
        {
            youtubeapi = gameObject.AddComponent<YoutubePlaylistManager>();
        }
   
        GetPlaylist();
    }

    public void GetPlaylist()
    {
        youtubeapi.GetPlaylistItems(searchField.text, 50, OnGetPlaylistDone);
      
    }

    void OnGetPlaylistDone(YoutubePlaylistItems[] results)
    {

        myPlaylist = results;
    }

    public void on2DBtnClick()
    {
        if (!loaded)
        {
            LoadVideosOnUI(myPlaylist);
        } else
        {
            LoadVideoOnUI();
        }
         
    }

    void LoadVideosOnUI(YoutubePlaylistItems[] videoList)
    {
        for (int i = 0; i < videoList.Length; i++)
        {
            GameObject button = (GameObject)Instantiate(buttonPrefab);
            button.transform.SetParent(menuPanel, false);
            button.GetComponent<MyYoutubeVideoUI>().videoName.text = videoList[i].snippet.title;
            button.GetComponent<MyYoutubeVideoUI>().videoId = videoList[i].videoId;
            button.GetComponent<MyYoutubeVideoUI>().description = videoList[i].snippet.description;
            button.GetComponent<MyYoutubeVideoUI>().thumbUrl = videoList[i].snippet.thumbnails.defaultThumbnail.url;
            button.GetComponent<MyYoutubeVideoUI>().LoadThumbnail();
        }
        loaded = true;
    }

    void LoadVideoOnUI()
    {
        videoUIResult.SetActive(true);
    } 
}
