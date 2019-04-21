using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine;

using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.Mine
{
    public class PlayerBullet : MonoBehaviour
    {
        public Player Owner { get; private set; }

        public void Start()
        {
            StartCoroutine(DestroyBullet());
            Destroy(gameObject, 3.0f);
        }

        IEnumerator DestroyBullet(){
            yield return new WaitForSeconds(3f);
            //network destroy
            Destroy(this.gameObject);  
        }

        public void OnCollisionEnter(Collision collision)
        {
            Destroy(this.gameObject);  
        }
        
        public void InitializeBullet(Player owner, Vector3 forward,Quaternion rot,float BulletSpeed,float lag)
        {

            Owner = owner;
            var VelocityOutput = new Vector3(forward.x - Random.Range(-0.03f, 0.03f), forward.y - Random.Range(-0.03f, 0.03f), forward.z - Random.Range(-0.03f, 0.03f)).normalized * BulletSpeed;        
            transform.GetComponent<Rigidbody>().velocity = VelocityOutput;
            //transform.GetComponent<Rigidbody>().position += transform.GetComponent<Rigidbody>().velocity * lag;
            transform.rotation = rot;
        }
    }
}