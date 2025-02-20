using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class TurretSlowmo : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button upgradeButton;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float aps = 4f; //attacks per second
    [SerializeField] private float freezeTime = 1f;
    [SerializeField] private int baseUpgradeCost = 150;

    private float timeUntilFire;
    private float apsBase;
    private float targetingRangeBase;
    private float freezeTimeBase;

    private int level = 1;

    private void Start()
    {
        apsBase = aps;
        targetingRangeBase = targetingRange;
        freezeTimeBase = freezeTime;

        upgradeButton.onClick.AddListener(Upgrade);
    }

    private void Update()
    {
        timeUntilFire += Time.deltaTime;

        if (timeUntilFire >= 1f / aps)
        {
            FreezeEnemies();
            timeUntilFire = 0f;
        }

    }

    private void FreezeEnemies()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)
            transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            for(int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];

                EnemyMovement em = hit.transform.GetComponent<EnemyMovement>();
                em.UpdateSpeed(0.25f);

                StartCoroutine(ResetEnemySpeed(em));
            }
        }
    }

    private IEnumerator ResetEnemySpeed(EnemyMovement em)
    {
        yield return new WaitForSeconds(freezeTime);

        em.ResetSpeed();
    }

    public void OpenUpgradeUI()
    {
        upgradeUI.SetActive(true);
    }

    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        UIManager.main.SetHoveringState(false);
    }

    public void Upgrade()
    {
        if (CalculateCost() > levelManager.main.currency) return;

        levelManager.main.SpendCurrency(CalculateCost());

        level++;

        aps = CalculateAPS();
        targetingRange = CalculateRange();
        freezeTime = CalculateFreezeTime();

        CloseUpgradeUI();
        Debug.Log("New BPS: " + aps);
        Debug.Log("New Targeting Range: " + targetingRange);
        Debug.Log("New Freeze Time: " + freezeTime);
        Debug.Log("New Cost: " + CalculateCost());

    }

    private int CalculateCost()
    {
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 0.8f));
    }

    private float CalculateAPS()
    {
        return apsBase * Mathf.Pow(level, 0.6f);
    }

    private float CalculateRange()
    {
        return targetingRangeBase * Mathf.Pow(level, 0.4f);
    }

    private float CalculateFreezeTime()
    {
        return freezeTimeBase * Mathf.Pow(level, 0.3f);
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
}
