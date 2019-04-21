using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Mine
{
    public class PlayerShoot : MonoBehaviour
    {
        public int POWER;
        public GameObject Ob;
        public Transform ShootPoint;
        public PhotonView photonView;
        public PlayerNetwork player;
        void Start()
        {
            Ob = this.gameObject;
            ShootPoint = Ob.transform.Find("Gun/ShootPoint").transform;
            
            photonView = Ob.GetComponent<PhotonView>();
            if(photonView.IsMine){
                player = transform.GetComponent<PlayerNetwork>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && photonView.IsMine)
            {
                var forward = ShootPoint.forward.normalized;                
                var newRotation = ShootPoint.rotation;
                //shoot on server
                player.photonView.RPC("Shoot", RpcTarget.Others,ShootPoint.transform.position, forward, newRotation);
                //shoot on client
                player.ShootClient(ShootPoint.transform.position, forward, newRotation);
            }
        }


    }
}