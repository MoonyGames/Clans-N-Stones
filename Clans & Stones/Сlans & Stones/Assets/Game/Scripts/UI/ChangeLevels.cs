using UnityEngine;

public class ChangeLevels : MonoBehaviour
{
    public static ChangeLevels Instance { get; private set; }

    private int Level { get; set; }

    public int[] WavesCountForLevels;
    [SerializeField] private int[] _wavesSpawnTimeForLevels;

    public delegate void LevelChanged();
    public static LevelChanged OnLevelChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        Level += PlayerPrefs.GetInt("Level", 0);

        StartCoroutine(EnemyGenerator.Instance.SpawnOnTimer(WavesCountForLevels[Level], _wavesSpawnTimeForLevels[Level]));
    }

    public void ChangeLevel() => PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 0) + 1);
}
