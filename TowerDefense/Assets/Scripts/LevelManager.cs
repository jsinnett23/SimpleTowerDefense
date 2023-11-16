using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    public Transform startPoint;
    public Transform[] path;
    private bool isPlacingTurret = false;
    private GameObject turretPreviewInstance;

    public int currentCash; // Current available cash
    [SerializeField] private int startingCash = 100; // Starting cash amount
    [SerializeField] private int cashPerEnemy = 10;
    [SerializeField] private int baseHealth = 20; // Base health
    [SerializeField] private int turretCost = 50; // Cost to buy a turret

    [SerializeField] private TextMeshProUGUI cashText; // Reference to the TextMeshPro component for cash
    [SerializeField] private TextMeshProUGUI livesText; // Reference to the TextMeshPro component for lives
    [SerializeField] private TextMeshProUGUI waveText; // Reference to the TextMeshPro component for wave
    [SerializeField] private GameObject turretPrefab; // Turret prefab to instantiate
    [SerializeField] private GameObject turretPreviewPrefab; // The semi-transparent preview prefab
    [SerializeField] private GameObject gameOverPanel; // Reference to the Game Over Panel


    private void Awake()
    {
        main = this;
        currentCash = startingCash;
        UpdateUI();

    }

    public void UpdateUI()
    {
        cashText.text = "Cash: " + currentCash;
        livesText.text = "Lives: " + baseHealth;
        waveText.text = "Wave: " + EnemySpanwer.currentWave;
    }

    void Update()
    {
        if (isPlacingTurret)
        {
            MoveTurretPreviewToMouse();
            if (Input.GetMouseButtonDown(0))
            {
                PlaceTurret();
            }
        }
    }

    private void PlaceTurret()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

        if (hit.collider != null && hit.collider.CompareTag("TurretPlacementZone"))
        {
            Vector3 placementPosition = hit.collider.transform.position;
            Instantiate(turretPrefab, placementPosition, Quaternion.identity);
            isPlacingTurret = false;
            turretPreviewInstance.SetActive(false);
        }
    }

    private void MoveTurretPreviewToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

        if (hit.collider != null)
        {
            turretPreviewInstance.transform.position = hit.collider.transform.position;
        }
    }

    public void BuyTurret()
    {
        if (currentCash >= turretCost)
        {
            currentCash -= turretCost;
            UpdateUI();
            EnableTurretPlacement();
        }
        else
        {
            Debug.Log("Not enough cash to buy a turret!");
        }
    }
    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void EnableTurretPlacement()
    {
        isPlacingTurret = true;
        if (turretPreviewInstance == null)
        {
            turretPreviewInstance = Instantiate(turretPreviewPrefab);
        }
        turretPreviewInstance.SetActive(true);
    }

    public void EnemyReachedBase()
    {
        baseHealth--;
        UpdateUI();

        if (baseHealth <= 0)
        {
            GameOver();
        }
    }

    public void EnemyDestroyed()
    {
        currentCash += cashPerEnemy;
        UpdateUI();
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        gameOverPanel.SetActive(true);

    }
}
