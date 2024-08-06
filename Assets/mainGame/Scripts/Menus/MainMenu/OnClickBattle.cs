using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class OnClickBattle : MonoBehaviour
{
    public void OnClickHandler()
    {
        if(gameObject.name.Contains("Battle"))
            CombatMode.isSoloqEnabled = true;
        if(gameObject.name.Contains("Cup"))
            CombatMode.isSoloqEnabled = false;

        IGoToCombat();
    }

    private IEnumerator GoToCombat()
    {
        StartCoroutine(SceneManagerScript.instance.FadeOut());
        yield return new WaitForSeconds(GeneralUtils.GetRealOrSimulationTime(SceneFlag.FADE_DURATION));
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.Combat.ToString());
    }

    private void IGoToCombat()
    {
        Debug.Log("Start Battle!");
        StartCoroutine(GoToCombat());
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
}
