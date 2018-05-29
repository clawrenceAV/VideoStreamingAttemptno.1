using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using SimpleJSON;
using System.Text;
using System;

public class YoutubeLivestreamSystem_EMT : MonoBehaviour
{
    /*PRIVATE INFO DO NOT CHANGE THESE URLS OR VALUES*/
    private const string serverURI = "https://unity-dev-youtube.herokuapp.com/api/info?url=https://www.youtube.com/watch?v=";
    private const string formatURI = "&format=best&flatten=true";
    /*END OF PRIVATE INFO*/
    [Header("Easy movie texture MediaPlayerCtrl componet")]
    public MediaPlayerCtrl easyPlayer;

    [SerializeField]
    public YoutubeLive liveResults;

    public VideoQuality videoQuality;

    public string videoId = "RtU_mdL2vBM";
    private string videoUrl;
    //start playing the video
    public bool playOnStart = false;

    public void Start()
    {
        if (playOnStart)
        {
            PlayYoutubeVideo(videoId);
        }
    }

    public void PlayYoutubeVideo(string _videoId)
    {
        videoId = _videoId;
        StartCoroutine(LiveRequest(videoId));
    }

    IEnumerator LiveRequest(string videoID)
    {
        WWW request = new WWW(serverURI + "" + videoID + "" + formatURI);
        Debug.Log("FO");
        yield return request;
        var requestData = JSON.Parse(request.text);
        var videos = requestData["videos"][0]["formats"];

        for (int counter = 0; counter < videos.Count; counter++)
        {
            if (videos[counter]["format_id"] == "93")
            {
                liveResults.lowQuality = videos[counter]["manifest_url"];    //360
            }
            else if (videos[counter]["format_id"] == "95" || videos[counter]["format_id"] == "300")//300
            {
                liveResults.mediumQuality = videos[counter]["manifest_url"];  //720p
            }
            else if (videos[counter]["format_id"] == "96" || videos[counter]["format_id"] == "301")//301
            {
                liveResults.fullHdQuality = videos[counter]["manifest_url"];  //1080p
            }
            else if (videos[counter]["format_id"] == "97")
            {
                liveResults.ultraHdQuality = videos[counter]["manifest_url"];  //@2160p 4k
            }

        }

        StartCoroutine(LivePlay());
    }


    IEnumerator LivePlay() //The prepare not respond so, i forced to play after some seconds
    {
        yield return new WaitForSeconds(0.5f);
        string uri = "";
        switch (videoQuality)
        {
            case VideoQuality.LowQuality:
                uri = liveResults.lowQuality;
                break;
            case VideoQuality.Hd720:
                uri = liveResults.mediumQuality;
                break;
            case VideoQuality.Hd1080:
                uri = liveResults.fullHdQuality;
                break;
            case VideoQuality.Hd2160:
                uri = liveResults.ultraHdQuality;
                break;
        }
        easyPlayer.m_strFileName = uri;
        StartCoroutine(Play());


    }

    public IEnumerator Play()
    {
        yield return new WaitForSeconds(4);
        easyPlayer.Play();
    }

    public enum VideoQuality
    {
        LowQuality,
        Hd720,
        Hd1080,
        Hd2160
    }


    public int GetMaxQualitySupportedByDevice()
    {
        if (Screen.orientation == ScreenOrientation.Landscape)
        {
            //use the height
            return Screen.currentResolution.height;
        }
        else if (Screen.orientation == ScreenOrientation.Portrait)
        {
            //use the width
            return Screen.currentResolution.width;
        }
        else
        {
            return Screen.currentResolution.height;
        }
    }
}

[System.Serializable]
public class YoutubeLive
{
    public string lowQuality;
    public string mediumQuality;
    public string fullHdQuality;
    public string ultraHdQuality;
}
