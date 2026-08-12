using myh;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlantState
{
    Disable,
    Enable
}

public enum PlantType
{
    Sunflower,
    PeaShooter,
    DoublePeaShooter,
    SnowPeaShooter,
    PotatoMine,
    WallNut,
    CherryBomb,
}

public class Plant : MonoBehaviour, IPointerClickHandler
{
    protected PlantState mPlantState = PlantState.Disable;

    public PlantType plantType;

    public int row;

    public int column;

    /// <summary>
    /// 血量
    /// </summary>
    [SerializeField] protected int mHealth;

    protected int OriginHealth;

    /// <summary>
    /// 动画
    /// </summary>
    protected Animator mAnimator;

    private void Start()
    {
        mAnimator = GetComponent<Animator>();
        TurnToDisable();
        OriginHealth = mHealth;
    }

    protected virtual void Update()
    {
        switch (mPlantState)
        {
            case PlantState.Disable:
                DisableUpdate();
                break;
            case PlantState.Enable:
                EnableUpdate();
                break;
        }
    }

    private void DisableUpdate()
    {
    }

    protected virtual void EnableUpdate()
    {
    }

    protected void TurnToDisable()
    {
        mPlantState = PlantState.Disable;
        mAnimator.speed = 0;
    }

    public void TurnToEnable()
    {
        DelayCall.After(0.5f,(() =>
        {
            mPlantState = PlantState.Enable;
        }));
        mAnimator.speed = 1;
        GetComponent<SpriteRenderer>().sortingLayerName = "Game";
    }

    public virtual void DamageHealth(int damage)
    {
        mHealth -= damage;
        if (mHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"row:{row},column:{column},{gameObject.name}");
        if (MainGameManager.GetInstance().IsTakeShovel() && mPlantState != PlantState.Disable)
        {
            EffectAudioManager.Instance.PlayEffect("Audio/RemovePlant");
            MainGameManager.GetInstance().ChangeShovelState(false);
            UIManager.Hide("ShovelStateUi");
            Destroy(gameObject);
        }
        
    }
}