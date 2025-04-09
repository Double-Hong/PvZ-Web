using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{

    public GameObject lostHead;

    public GameObject bg;

    public Button backToMainBtn;

    private float mChangeTime = 1f;

    private float timer = 0f;

    private Image bgImage;
    
    // Start is called before the first frame update
    void Start()
    {
        backToMainBtn.onClick.AddListener(OnBackMainBtnClick);
        bgImage = bg.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (lostHead.transform.localScale.x <= 1.5f)
        {
            lostHead.transform.localScale = new Vector3((timer / mChangeTime), timer / mChangeTime, timer / mChangeTime);
            bgImage.color = new Color(0, 0, 0, (timer / mChangeTime) );
        }
        else
        {
            backToMainBtn.gameObject.SetActive(true);
        }
        
    }
    
    private void OnBackMainBtnClick()
    {
        MainGameManager.GetInstance().BackToMain();
        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/UserPanel");
        Instantiate(prefab, MainGameManager.GetInstance().GameCanvas.transform);
        Destroy(gameObject);
    }
}
