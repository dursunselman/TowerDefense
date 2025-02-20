using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;
    
    private GameObject towerObj;
    public Turret turret;
    public TurretSlowmo turret2;
    private Color startColor;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    private void OnMouseDown()
    {
        if (UIManager.main.IsHoveringUI()) return;
        
        if (towerObj != null)
        {
            if (towerObj.GetComponent<Turret>() != null)
            {
                turret.OpenUpgradeUI(); // Turret1'in upgrade iþlemleri
                return;
            }
            else if (towerObj.GetComponent<TurretSlowmo>() != null)
            {
                turret2.OpenUpgradeUI(); // Turret2'nin upgrade iþlemleri
                return;
            }
        }
        

        Tower towerToBuild = BuildManager.main.GetSelectedTower();

        if(towerToBuild.cost > levelManager.main.currency)
        {
            Debug.Log("You can't afford this tower");
            return;
        }

        levelManager.main.SpendCurrency(towerToBuild.cost);

        towerObj = Instantiate(towerToBuild.prefab,transform.position,Quaternion.identity);
        turret = towerObj.GetComponent<Turret>();
        turret2 = towerObj.GetComponent<TurretSlowmo>();
    }
}
