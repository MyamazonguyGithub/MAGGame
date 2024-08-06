using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cheatCode : MonoBehaviour
{
    public static cheatCode instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void AddGems()
    {
        User.Instance.gems += 500000000;
    }

    public void AddGold()
    {
        User.Instance.gold += 500000000;
    }

    public void AddEnergy()
    {
        User.Instance.energy += 500000000;
    }
}
