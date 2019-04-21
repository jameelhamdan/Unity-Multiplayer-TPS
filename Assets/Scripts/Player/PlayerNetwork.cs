using System.Collections;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using TMPro;
using System;

namespace Photon.Pun.Mine
{
    //All RPC Calls will happen here    
    public class PlayerNetwork : MonoBehaviour, IPunObservable
    {
        public float SmoothingDelay = 5;
        public PhotonView photonView;
        Transform cam;
        Transform textbox; 

        //Learping
        private Vector3 correctPlayerPos = Vector3.zero; 
        private Quaternion correctPlayerRot = Quaternion.identity;
        private Inventory myInventory;

        //Shoot Stuff
        public Transform ShootPoint;
        public GameObject BulletPrefab;
        public float BulletSpeed;


        //Animator
        public Animator anim;        
        public void Awake()
        {
            photonView = GetComponent<PhotonView>();                      
            bool observed = false;
            foreach (Component observedComponent in this.photonView.ObservedComponents)
            {
                if (observedComponent == this)
                {
                    observed = true;
                    break;
                }
            }
            if (!observed)
            {
                Debug.LogWarning(this + " is not observed by this object's photonView! OnPhotonSerializeView() in this class won't be used.");
            }

            //Change NameField
            this.transform.Find("ontopPlayer/nameText").GetComponent<TextMeshProUGUI>().SetText(photonView.Owner.NickName);

            //Assign anim values
			anim = gameObject.GetComponentInChildren<Animator>();

            if (photonView.IsMine)
            {
                this.GetComponent<PlayerManager>().enabled = true;
                this.GetComponent<Inventory>().enabled = true;
                this.GetComponent<PlayerMotor>().enabled = true;
                this.GetComponent<PlayerShoot>().enabled = true;
                myInventory = this.GetComponent<Inventory>();

            } else {
                this.GetComponent<PlayerManager>().enabled = false;
                this.GetComponent<Inventory>().enabled = false;
                this.GetComponent<PlayerMotor>().enabled = false;
                this.GetComponent<PlayerShoot>().enabled = false;
                
            }           
        }
        
        void Start(){
            textbox = this.transform.Find("ontopPlayer").transform;
            cam = Camera.main.gameObject.transform;
            ShootPoint = transform.Find("Gun/ShootPoint").transform;
            BulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
        }


        public void Update()
        {
            if (!photonView.IsMine)
            {
                //Update remote player
                // transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
                //transform.rotation = Quaternion.Slerp(transform.rotation, correctPlayerRot, Time.deltaTime * this.SmoothingDelay);
                //transform.Translate(correctPlayerPos,Space.Self); 
            }
        }  
        
        void LateUpdate()
        {            
            textbox.LookAt(cam);           
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // if (stream.IsWriting)
            // {
            //     //We own this player: send the others our data
            //     stream.SendNext(transform.position);
            //     stream.SendNext(transform.rotation);
            // }
            // else
            // {
            //     //Network player, receive data
            //     correctPlayerPos = (Vector3)stream.ReceiveNext();
            //     correctPlayerRot = (Quaternion)stream.ReceiveNext();
            // }
        }     

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other)
        {
            if(photonView.IsMine && other.CompareTag("Custom/ItemPickup")){                
                var newid = other.transform.GetComponent<PickupID>().ID;
                Debug.Log("Trying to Add " + newid);     
                //Do pickup validation here
                var result = myInventory.AddItem(newid);
                if(result){

                    PhotonNetwork.Destroy(other.transform.gameObject);                

                    Debug.Log("Adding Succeeded " + newid);
                } else {
                    Debug.Log("Adding Failed For " + newid);
                }

            }
        }

        //Called from all clients
        [PunRPC]
        public void Shoot(Vector3 SpawnPos,Vector3 velocity, Quaternion rotation, PhotonMessageInfo info)
        {
            float lag = (float) (PhotonNetwork.Time - info.SentServerTime);
            //network insantiate        
            //this method is called "Local Bullets"
            var newBullet = Instantiate(BulletPrefab,SpawnPos, rotation);  
            newBullet.GetComponent<Photon.Pun.Mine.PlayerBullet>().InitializeBullet(photonView.Owner,velocity,rotation,BulletSpeed,Mathf.Abs(lag));
        }
        public void ShootClient(Vector3 SpawnPos,Vector3 velocity, Quaternion rotation)
        {
            //network insantiate        
            //this method is called "Local Bullets"
            var newBullet = Instantiate(BulletPrefab,SpawnPos, rotation);  
            newBullet.GetComponent<Photon.Pun.Mine.PlayerBullet>().InitializeBullet(photonView.Owner,velocity,rotation,BulletSpeed,Mathf.Abs(0));
        }
    }
}
