using System.Collections;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using TMPro;
using System.Linq;

namespace Photon.Pun.Mine
{     
    [RequireComponent(typeof(PhotonView))]
    class EnemyController : MonoBehaviour
    {
        [SerializeField]
        public int Health;
        public float speed;
        private Rigidbody _body;

        public TextMeshProUGUI text;
        public GameObject cam;
        private IEnumerator LoseHCoroutine;
        private IEnumerator GetPlayerCoroutine;
        private PhotonView photonView;
        public GameObject PlayerToFollow;

        public void Awake()
        {
            photonView = GetComponent<PhotonView>();
            _body = GetComponent<Rigidbody>();

            text = this.gameObject.transform.Find("Canvas/Text").GetComponent<TextMeshProUGUI>();
            cam = Camera.main.gameObject;
            //LoseHCourntine = LoseHealth();
            GetPlayerCoroutine = GetPlayer();
            //if(!photonView.iserv)
            StartCoroutine(GetPlayerCoroutine);
        }
        
        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Custom/Bullet")){
                Debug.Log("GOT HIT");
                if(other.transform.GetComponent<PlayerBullet>().Owner.IsLocal){
                    //Call lose health RPC on all clients
                    photonView.RPC("loseHealthCall", RpcTarget.AllBufferedViaServer,10);
                }
            }

            //For Testing
            // if (other.CompareTag("Player") && other.transform.GetComponent<PhotonView>().IsMine)
            // {
            //     if(LoseHCourntine != null) StopCoroutine(LoseHCourntine);
            //     StartCoroutine(LoseHCourntine);
            // }
        }
        void OnTriggerExit(Collider other)
        {
            // if (other.CompareTag("Player") && other.transform.GetComponent<PhotonView>().IsMine)
            // {
            //     StopCoroutine(LoseHCourntine);
            // }
        }

        public void FixedUpdate(){
            MoveTowardsPlayer();
        }

        public void LateUpdate()
        {
            text.transform.parent.LookAt(cam.transform);
            text.SetText("" + Health);

            if(Health <= 0 ){
                Death();
            }
        }

        private void MoveTowardsPlayer(){

            if(PlayerToFollow == null) return;
            var shortestdist = Vector3.Distance(this.gameObject.transform.position,PlayerToFollow.transform.position);

            //Do actual movement
            if(shortestdist < 10){
                var moveVector = (PlayerToFollow.transform.position - this.gameObject.transform.position).normalized;  
                transform.forward = moveVector;
                _body.MovePosition(_body.position + moveVector.normalized * speed * Time.fixedDeltaTime);
                Debug.DrawRay(this.gameObject.transform.position,moveVector,Color.blue,0.1f);
            }
        }

        // IEnumerator LoseHealth()
        // {
        //     while (true && Health > 1)
        //     {
        //         photonView.RPC("loseHealthCall", RpcTarget.AllBufferedViaServer,1);
        //         yield return new WaitForSeconds(0.175f);
        //     }
        // }


        [PunRPC]
        public void loseHealthCall(int amount){//,PhotonMessageInfo info){
            Health -= amount;
            if(Health <= 0 ){
                //DIE STUFF AND ADD SCORE

                Death();                                
                return;
            }
        }


        private void Death(){
            PhotonNetwork.Destroy(gameObject);
        }

        private IEnumerator GetPlayer(){
            while(true){                
                //Update player list
                yield return new WaitForSeconds(0.3f);
                GameObject ClosestP = null;
                var playerlist = GameObject.FindGameObjectsWithTag("Player").ToList();
                float shortestdist = 99999;
                if(playerlist.Count == 0){
                    continue;
                }
                
                foreach(var player in playerlist){
                    var tempdist = Vector3.Distance(this.gameObject.transform.position,player.transform.position);
                    if(tempdist < shortestdist){
                        ClosestP = player;
                        shortestdist = tempdist;
                    }
                }

                if(ClosestP == null) continue;
                //Assign to Global Variable 
                PlayerToFollow = ClosestP;
            }
        }

    }
}