using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPun
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerPositionText;

    // Start is called before the first frame update
    void Start()
    {
        /*
        //Conditionally activating movement controls for only local player and not remote player spawwned through network
        if(photonView.IsMine)
        {
            //If this player is local player
            transform.GetComponent<MovementController>().enabled = true;
            transform.GetComponent<MovementController>().joystick.gameObject.SetActive(true);
        }
        else
        {
            //If this player is remote player
            transform.GetComponent<MovementController>().enabled = false;
            transform.GetComponent<MovementController>().joystick.gameObject.SetActive(false);
        }
        */
        SetPlayerName();
    }

    private void Update()
    {
        playerPositionText.text = gameObject.transform.position.ToString();
    }

    void SetPlayerName()
    {
        if(playerNameText != null)
        {
            if(photonView.IsMine)
            {
                playerNameText.text = "YOU";
                playerNameText.color = Color.red;
            }
            else
            {
                playerNameText.text = photonView.Owner.NickName;
            }
        }
    }
}
