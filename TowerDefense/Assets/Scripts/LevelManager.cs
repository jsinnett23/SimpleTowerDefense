using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    public Transform startPoint;
    public Transform[] path;
    private bool isPlacingTurret = false;





    public int currentCash; // Current available cash
    [SerializeField] private int startingCash = 100; // Starting cash amount
    [SerializeField] private int cashPerEnemy = 10;
    [SerializeField] private int baseHealth = 20; // Base health
    [SerializeField] private int turretCost = 50; // Cost to buy a turret



    [SerializeField] private TextMeshProUGUI cashText; // Reference to the TextMeshPro component for cash
    [SerializeField] private TextMeshProUGUI livesText; // Reference to the TextMeshPro component for lives
    [SerializeField] private TextMeshProUGUI waveText; // Referen
    [SerializeField] private GameObject turretPrefab; // Turret prefab to instantiate




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
        waveText.text = "Wave: " + EnemySpanwer.currentWave; // Ensure EnemySpanwer has a public static currentWave variable
    }
    void Update()
    {
        if (isPlacingTurret && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            if (hit.collider != null && hit.collider.CompareTag("TurretPlacementZone"))
            {
                Vector3 placementPosition = hit.collider.transform.position;
                Instantiate(turretPrefab, placementPosition, Quaternion.identity);
                isPlacingTurret = false;
            }
            else
            {
                Debug.Log("Invalid placement area");
            }
        }
    }

    public void EnemyReachedBase()
    {
        baseHealth--;
        UpdateUI(); // Update UI when a life is lost

        Debug.Log("Base Health: " + baseHealth);

        if (baseHealth <= 0)
        {
            GameOver();
        }
    }
    public void EnemyDestroyed()
    {
        currentCash += cashPerEnemy;
        UpdateUI(); // Update UI when a life is lost

        Debug.Log("Cash earned! Current Cash: " + currentCash);
        // Here you can also update the UI to reflect the new cash amount
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        // Here you can add anything you want to happen when the game is over,
        // like displaying a game over screen or stopping the game.
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
    private void EnableTurretPlacement()
    {
        isPlacingTurret = true;
    }
}
