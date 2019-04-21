using System.Collections;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using System;
using TMPro;

namespace Photon.Pun.Mine
{
    public class SpaceShip : MonoBehaviour, IPunObservable
    {
        [SerializeField]
        public int Health = 1000;
        [SerializeField]
        public float FixPercentage = 0;
        public PhotonView photonView;

        private Transform textObject;
        private TextMeshProUGUI textbox; 
        private Transform cam;

        void Start()
        {
            textObject = this.transform.Find("ontop").transform;
            textbox = this.transform.Find("ontop/Text").transform.GetComponent<TextMeshProUGUI>();
            cam = Camera.main.gameObject.transform;
        }
   
        void Update()
        {
            textbox.SetText(Health + " / %"+ FixPercentage);
        }
        void LateUpdate()
        {
            textObject.transform.LookAt(cam);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(Health);
                stream.SendNext(FixPercentage);

            }
            else
            {
                Health = (int)stream.ReceiveNext();
                FixPercentage = (float)stream.ReceiveNext();
            }
        }

        public IEnumerator CFixRuntime()
        {            
            yield return new WaitForSeconds(0.25f);
            while(true){                          
                yield return new WaitForSeconds(0.25f);
                FixPercentage += 0.05f;
            }
        }           

    }
}