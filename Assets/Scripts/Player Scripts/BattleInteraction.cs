using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class BattleInteraction : MonoBehaviourPun
{
    public Spinner spinnerScript;
    public Image spinSpeedBar_Image;
    public TextMeshProUGUI spinSpeedRatio_Text;
    public GameObject UI_3D_Gameobject;
    public GameObject deathPanel_UI_Prefab;
    public float commonDamageCoefficient = 0.04f;

    [Header("Visual Effects")]
    public List<GameObject> pooledObjects;
    public int amountToPool = 8;
    public GameObject CollisionEffectPrefab;

    [Header("Player Type")]
    public bool isAttacker;
    public bool isDefender;


    [Header("Player Type Damage Coefficients")]
    public float doDamage_Coefficient_Attacker = 10f;   //do more damage than Defender -ADVANTAGE
    public float getDamaged_Coefficient_Attacker = 1.2f;//gets more damage than Defender -DISADVANTAGE
    public float doDamage_Coefficient_Defender = 0.75f; //do less damage than attacker -DISADVANTAGE
    public float getDamaged_Coefficient_Defender = 0.2f;//gets less damage than attacker -ADVANTAGE

    private Rigidbody rb;
    private GameObject deathPanel_UI_Gameobject;
    private float startSpinSpeed;
    private float currentSpinSpeed;
    private bool isDead = false;

    
    void Awake()
    {
        startSpinSpeed = spinnerScript.spinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;

        spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed; 
    }

    void Start()
    {
        CheckPlayerType();

        rb = GetComponent<Rigidbody>();


        if (photonView.IsMine)
        {
            pooledObjects = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(CollisionEffectPrefab, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                Vector3 effectPosition = (gameObject.transform.position + collision.transform.position) / 2 + new Vector3(0, 0.05f, 0);

                //Instantiate Collision Effect ParticleSystem
                GameObject collisionEffectGameobject = GetPooledObject();
                if (collisionEffectGameobject != null)
                {
                    collisionEffectGameobject.transform.position = effectPosition;
                    collisionEffectGameobject.SetActive(true);
                    collisionEffectGameobject.GetComponentInChildren<ParticleSystem>().Play();

                    //De-activate Collision Effect Particle System after some seconds.
                    StartCoroutine(DeactivateAfterSeconds(collisionEffectGameobject, 0.5f));

                }
            }

            //Comparing the lateral and longitudinal movement speeds of the SpinnerTops
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            Debug.Log("Local player speed: " + mySpeed + " Remote player speed: " + otherPlayerSpeed);

            if(mySpeed > otherPlayerSpeed)
            {
                Debug.Log("Damage inflicted to other player.");

                float defaulDamageAmount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600 * commonDamageCoefficient;

                if (isAttacker)
                {
                    defaulDamageAmount *= doDamage_Coefficient_Attacker;
                }
                else if (isDefender)
                {
                    defaulDamageAmount *= doDamage_Coefficient_Defender;
                }

                //This section will be specifically executed when a remote playerobject hits(attacks) a local playerObject
                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    //Apply Damage to the slower player through RPC so it is called in all remote client intances of this playerObject in the room
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, defaulDamageAmount);
                }

            }
        }
    }

    private void CheckPlayerType()
    {
        if(gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;
        }
        else if(gameObject.name.Contains("Defender"))
        {
            isDefender = true;
            isAttacker = false;

            //extra health to defender types
            spinnerScript.spinSpeed = 4400;

            startSpinSpeed = spinnerScript.spinSpeed;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;
        }
    }

    [PunRPC]
    public void DoDamage(float _damageAmount)
    {
        if (!isDead)
        {
            if (isAttacker)
            {
                _damageAmount *= getDamaged_Coefficient_Attacker;

                if (_damageAmount > 1000)
                {
                    _damageAmount = 400f;
                }
            }
            else if (isDefender)
            {
                _damageAmount *= getDamaged_Coefficient_Defender;
            }

            spinnerScript.spinSpeed -= _damageAmount;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
            spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;

            if (currentSpinSpeed < 100)
            {
                //Die
                Die();
            }
        }
    }

    void Die()
    {
        isDead = true;

        GetComponent<MovementController>().enabled = false;

        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        spinnerScript.spinSpeed = 0f;

        UI_3D_Gameobject.SetActive(false);

        if(photonView.IsMine)
        {
            //countdown for respawn
            StartCoroutine(ReSpawn());
        }
    }

    IEnumerator ReSpawn()
    {
        GameObject canvasGameObject = GameObject.Find("Canvas");
        if(deathPanel_UI_Gameobject == null)
        {
            deathPanel_UI_Gameobject = Instantiate(deathPanel_UI_Prefab, canvasGameObject.transform);
        }
        else
        {
            deathPanel_UI_Gameobject.SetActive(true);
        }

        Text respawnTimeText = deathPanel_UI_Gameobject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;

        respawnTimeText.text = respawnTime.ToString(".00");

        while(respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
        
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(".00");

            GetComponent<MovementController>().enabled = false;
        }

        deathPanel_UI_Gameobject.SetActive(false);

        GetComponent<MovementController>().enabled = true;

        photonView.RPC("ReBorn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void ReBorn()
    {
        spinnerScript.spinSpeed = startSpinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;

        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        UI_3D_Gameobject.SetActive(true);

        isDead = false;
    }


    public GameObject GetPooledObject()
    {

        for (int i = 0; i < pooledObjects.Count; i++)
        {

            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);

    }
}
