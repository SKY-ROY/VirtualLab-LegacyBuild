using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
public class PlayerSelectionManager : MonoBehaviour
{
    public Transform playerSwitcherTransform;
    public int playerSelectionNumber;
    public GameObject[] spinnerTopModels;

    [Header("UI")]
    public TextMeshProUGUI playerModelType_Text;
    public GameObject UI_Selection;
    public GameObject UI_AfterSelection;
    public Button next_Button;
    public Button previous_Button;

    [Header("AR Functionality")]
    public bool isAR;

    [Header("Scene Names")]
    public string AREnabledScene = "Scene_Gameplay_AR";
    public string ARDisabledScene = "Scene_Gameplay_Normal";

    #region UNITY Methods

    // Start is called before the first frame update
    void Start()
    {
        UI_Selection.SetActive(true);
        UI_AfterSelection.SetActive(false);
        
        playerSelectionNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region UI Callback Methods
    public void NextPlayer()
    {
        playerSelectionNumber++;
        playerSelectionNumber %= 4;

        Debug.Log("Player Index: " + playerSelectionNumber);

        //buttons are disabled to avoid consecutive clicks
        next_Button.enabled = false;
        previous_Button.enabled = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, 90, 1.0f));
        
        //Change Top-type text
        if(playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            playerModelType_Text.text = "Attack";
        }    
        else
        {
            playerModelType_Text.text = "Defence";
        }
    }

    public void PreviousPlayer()
    {
        playerSelectionNumber--;
        if(playerSelectionNumber < 0)
        {
            playerSelectionNumber = spinnerTopModels.Length - 1;
        }

        Debug.Log("Player Index: " + playerSelectionNumber);

        //buttons are disabled to avoid consecutive clicks
        next_Button.enabled = false;
        previous_Button.enabled = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, -90, 1.0f));

        //Change Top-type text
        if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            playerModelType_Text.text = "Attack";
        }
        else
        {
            playerModelType_Text.text = "Defence";
        }
    }

    public void OnSelectButtonClicked()
    {
        UI_Selection.SetActive(false);
        UI_AfterSelection.SetActive(true);

        //Creating a new custom property through hashtable with string and int as key and value pairs.
        var playerSelectionProp = new ExitGames.Client.Photon.Hashtable { { MultiplayerARSpinTopGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);

    }

    public void OnReselectButtonClicked()
    {
        UI_Selection.SetActive(true);
        UI_AfterSelection.SetActive(false);
    }

    public void OnBattleButtonClicked()
    {
        if(isAR)
        {
            SceneLoader.Instance.LoadScene(AREnabledScene);
        }
        else
        {
            SceneLoader.Instance.LoadScene(ARDisabledScene);
        }
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }

    #endregion

    #region Private Methods

    IEnumerator Rotate(Vector3 axis, Transform transformToRotate, float angle, float duration = 1.0f)
    {
        Quaternion originalRotation = transformToRotate.rotation;

        //rotate a vector by another vector
        Quaternion finalRotation = transformToRotate.rotation * Quaternion.Euler(axis * angle);

        float elapsedTime = 0.0f;
        
        while(elapsedTime < duration)
        {
            //This will rotate gameObject from initial value to final value by an amount set
            transformToRotate.rotation = Quaternion.Slerp(originalRotation, finalRotation, elapsedTime / duration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        /*Since the slerp will never reach the final roatation value but 
         * it will be very close to the final value after duration therefore 
         * on termination of loop we put the assign the final value to the 
         * rotation of the playerSwitcherTransform.
        */
        transformToRotate.rotation = finalRotation;

        //On completion of roatation vuttons are enabled again
        next_Button.enabled = true;
        previous_Button.enabled = true;
    }

    #endregion

}
