using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;
public class OpenDataMessage
{
    // type 用于表明事件类型
    public string type;
    public int score;
    public string rankType;//排行榜类型 type1~6
}
public class RankManager : MonoBehaviour
{
    public static RankManager Instance { get; private set; }
    public Canvas MyCanvas;
    public RawImage RankBody;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ShowRank()
    {
        RectTransform rectTransform = RankBody.GetComponent<RectTransform>();
        CanvasScaler scaler = MyCanvas.GetComponent<CanvasScaler>();
        var referenceResolution = scaler.referenceResolution;
        float width = RankBody.rectTransform.rect.width*(Screen.height / referenceResolution.y);
        float height = RankBody.rectTransform.rect.height*(Screen.height / referenceResolution.y);
        float posx = rectTransform.anchoredPosition.x;
        float posy = rectTransform.anchoredPosition.y;
        posx = Screen.width/2 - (width/2-Mathf.Abs(posx));
        posy = Screen.height/2-(height/2-Mathf.Abs(posy));
        WX.ShowOpenData(RankBody.texture, (int)posx, (int)posy, (int)width, (int)height);
        OpenDataMessage msgData = new OpenDataMessage();
        msgData.type = "showFriendsRank";
        msgData.rankType = "gameScore";
        string msg = JsonUtility.ToJson(msgData);
        WX.GetOpenDataContext().PostMessage(msg);
    }
    public void HideRank()
    {
        WX.HideOpenData();
    }
    public void sendScore(int score)
    {
        OpenDataMessage msgData = new OpenDataMessage();
        msgData.type = "setUserRecord";
        msgData.score = score;
        msgData.rankType = "gameScore";
        string msg = JsonUtility.ToJson(msgData);
        WX.GetOpenDataContext().PostMessage(msg);
    }
}
