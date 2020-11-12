using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MySynchronization : MonoBehaviour, IPunObservable
{
    public bool synchronizeVelocity = true;
    public bool synchronizeAngularVelocity = true;
    public bool isTeleportationEnabled = true;
    public float teleportationIfDistanceGreaterThan = 1.0f;

    private Rigidbody rb;
    private PhotonView photonView;
    private Vector3 networkedPosition;
    private Quaternion networkedRotation;
    private GameObject battleArenaGameobject;
    private GameObject cam;

    private float distance;
    private float angle;

    void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();

        networkedPosition = new Vector3();
        networkedRotation = new Quaternion();

        battleArenaGameobject = GameObject.FindGameObjectWithTag("RoomPlatform");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //gameObject.transform.SetParent(camera.transform);
            gameObject.transform.position = cam.transform.position;
        }
    }

    void FixedUpdate()
    {
        if(!photonView.IsMine)
        {
            //moving from current position to position received over network in fixed intervals(10 time per seconds)
            rb.position = Vector3.MoveTowards(rb.position, networkedPosition, distance * (1.0f / PhotonNetwork.SerializationRate));

            //rotating towards current rotation to rotation received over network in fixed intervals(10 time per seconds)
            rb.rotation = Quaternion.RotateTowards(rb.rotation, networkedRotation, angle * (1.0f / PhotonNetwork.SerializationRate));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            //photon view belongs to this user(Local ME) and this user controls this playerObject.
            //should send position, rotation, velocity, etc. data to other remote playerObjects.
            stream.SendNext(rb.position - battleArenaGameobject.transform.position);                        //BattleArena's position added as offset
            stream.SendNext(rb.rotation);

            if(synchronizeVelocity)
            {
                stream.SendNext(rb.velocity);
            }

            if(synchronizeAngularVelocity)
            {
                stream.SendNext(rb.angularVelocity);
            }
        }
        else if(stream.IsReading)
        {
            //This section will be called on playerObject(Remote ME) on remote user's client-side
            //as for those playerObjects this playerObject will be a remote playerObject.
            networkedPosition = (Vector3)stream.ReceiveNext() + battleArenaGameobject.transform.position;   //BattleArena's position added as offset
            networkedRotation = (Quaternion)stream.ReceiveNext();
            
            if(isTeleportationEnabled)
            {
                if (Vector3.Distance(rb.position, networkedPosition) > teleportationIfDistanceGreaterThan)
                {
                    rb.position = networkedPosition;
                }
            }

            if(synchronizeVelocity || synchronizeAngularVelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                
                if(synchronizeVelocity)
                {
                    rb.velocity = (Vector3)stream.ReceiveNext();

                    networkedPosition += rb.velocity * lag;

                    distance = Vector3.Distance(rb.position, networkedPosition);
                }

                if(synchronizeAngularVelocity)
                {
                    rb.angularVelocity = (Vector3)stream.ReceiveNext();

                    networkedRotation = Quaternion.Euler(rb.angularVelocity * lag) * networkedRotation;

                    angle = Quaternion.Angle(rb.rotation, networkedRotation);
                }
            }
        }    
    }
}
