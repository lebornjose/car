using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int initialHealth = 1;
    public float currentHealth;
    public int currentScore;
    public bool ifGameStart = true;
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void InitializeGame()
    {
        currentHealth = initialHealth;
        currentScore = 0;
    }
    public void UpdateHealth(float amount)
    {
        currentHealth += amount;
        UIManager.Instance.UpdateUICount();
        if(currentHealth<=0)
            GameOver();

    }
    public void AddRandomItem(int ItemNum)//随机获得道具
    {
        AudioManager.Instance.PlayAudio_GetItem();
        int randomIndex = Random.Range(0, 4);
        if(ItemNum!=-1)
            randomIndex = ItemNum;
        ItemType randomItemType = (ItemType)randomIndex;
        ItemManager.Instance.AddItem(randomItemType,1);
        UIManager.Instance.UpdateUICount();
    }
    public void UpdateScore(int score)
    {
        currentScore += score;
        if(currentScore>=20)
            ObjectGenerator.Instance.MaxCarType = 3;
        if(currentScore>=40)
            ObjectGenerator.Instance.MaxCarType = 4;
        if(currentScore>=60)
            ObjectGenerator.Instance.MaxCarType = 5;
        if(currentScore%2==0)
        {
            int ranNum = Random.Range(0, 4);
            if(ranNum==2||ranNum==0)
                AddRandomItem(ranNum);
        }
        else if(currentScore%5==0)
        {
            AddRandomItem(-1);
        }
        if(currentScore%20==0)
        {
            if(Random.Range(0,3)==0)
                UpdateHealth(1);
        }
        UIManager.Instance.UpdateUICount();
    }
    public void StartGame()
    {
        ItemManager.Instance.itemCounts[ItemType.RemoveCollision]  = 0;
        ItemManager.Instance.itemCounts[ItemType.RoadBlock] = 0;
        ItemManager.Instance.itemCounts[ItemType.SpeedUp] = 0;
        ItemManager.Instance.itemCounts[ItemType.PoliceCar] = 0;
        currentHealth = initialHealth;
        currentScore = 0;
        UIManager.Instance.UpdateUICount();
        ifGameStart = true;
        ObjectGenerator.Instance.StartSpwawn();
    }
    public void GameOver()
    {
        saveManager.SaveHighScore(currentScore);
        RankManager.Instance.sendScore(saveManager.LoadHighScore());
        UIManager.Instance.panelGameOver.SetActive(true);
        ifGameStart = false;
        ObjectGenerator.Instance.ifStopSpawn = true; // 停止生成
        ObjectGenerator.Instance.StopAllCoroutines(); // 停止所有协程
        MyObjectPool.Instance.ClearAll();
        var allObject = FindObjectsOfType<BaseMovement>();
        foreach (var obj in allObject)
        {
            MyObjectPool.ReleaseGameObject(obj.gameObject);
        }
    }
}