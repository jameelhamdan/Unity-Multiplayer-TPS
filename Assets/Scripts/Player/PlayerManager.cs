using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.Mine
{
    public class PlayerManager : MonoBehaviour
    {
        public float Health = 100;
        public float MaxHealth = 100;
        public float Shield = 100;
        public float MaxShield = 100;
        public float Oxygen = 500;
        public float MaxOxygen = 500;
        public bool recentlyDamaged = false;

        //standard stuff
        public Transform characterTransform;
        public GameObject character;
        public Slider HBar;
        public Slider OBar;
        public GameObject Player_Canvas;
        public GameObject Info_Canvas;
        GameObject helperPanel;
        Text helperText;
        
        void Start()
        {
            character = this.gameObject;
            characterTransform = character.transform;

            Player_Canvas = GameObject.FindGameObjectWithTag("UI/Player_Canvas");
            Info_Canvas = Player_Canvas.transform.Find("INFO").gameObject;
            Info_Canvas.SetActive(true);
            //Health Bar value
            HBar = Player_Canvas.transform.Find("INFO/HealthSlider").GetComponent<Slider>();
            HBar.value = Health;
            HBar.maxValue = MaxHealth;

            OBar = Player_Canvas.transform.Find("INFO/OxygenSlider").GetComponent<Slider>();
            OBar.value = Oxygen;
            OBar.maxValue = MaxOxygen;

            //repeating functions
            StartCoroutine(ShieldRuntime(1f));
            StartCoroutine(OxygenRuntime(1f));

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //Check for esc click
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            //Health Check
            if (Health <= 0 || Oxygen <= 0)
            {
                Death();
            }

            HBar.value = Health;
            OBar.value = Oxygen;
            //Check for low shield and health


        }

        // public void showMessage(string content = "message",float duration = 4f){        
        //     //doshow message code
        //     var temp = new Dialogue();
        //     temp.name = "Alert!";
        //     temp.sentence = content;     
        //     dg.displayDialogue(temp,duration);       
        // }

        private IEnumerator ShieldRuntime(float delay)
        {
            yield return new WaitForSeconds(1f);
            while(true){
                //increase shield by one everysecond
                //Do special delay if damaged            
                if (Shield < MaxShield && !recentlyDamaged)
                {
                    Shield++;
                }
                yield return new WaitForSeconds(delay);
            }
        }

        private IEnumerator OxygenRuntime(float delay)
        {
            yield return new WaitForSeconds(1f);
            while(true){
                this.Oxygen--;
                yield return new WaitForSeconds(delay);
            }
        }

        private void Death()
        {
            //Do gameover code        
            var comps = character.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour c in comps)
            {
                c.enabled = false;
            }

            Invoke("YouDied", 5f);


        }
        public void YouDied()
        {
            Time.timeScale = 0;
            Application.Quit();
        }

        //to be called from the pickup
        public void getDamaged(float amount, float damDelay = 5f)
        {
            if (Shield == 0)
            {
                this.Health -= amount;
            }
            else
            {
                float temp = this.Shield - amount;
                //if damage amount is more than shield also take from health
                if (temp > 0)
                {
                    //Only take from shield
                    this.Shield = temp;
                }
                else
                {
                    //temp should be negative
                    //zero shield and take change and decrease health
                    this.Shield = 0;
                    this.Health += temp;
                }
            }

            this.recentlyDamaged = true;
            Invoke("resetDamageDelay", damDelay);
            if (Health < 0)
            {
                Death();
            }
        }
        private void resetDamageDelay()
        {
            this.recentlyDamaged = false;
        }

        public void PlusHealth(float amount)
        {
            float temp = this.Health + amount;
            this.Health = Mathf.Clamp(temp, 0, this.MaxHealth);
        }

        public void PlusShield(float amount)
        {
            float temp = this.Shield + amount;
            this.Shield = Mathf.Clamp(temp, 0, this.MaxShield);
        }

        public void PlusOxygen(float amount)
        {
            float temp = this.Oxygen + amount;
            this.Oxygen = Mathf.Clamp(temp, 0, this.MaxOxygen);
        }

        void LateUpdate()
        {
            UpdatePlayerCanvas();
        }
        private TextMeshProUGUI pingtext;
        private void UpdatePlayerCanvas(){
            //Bars
            // HBar = GameObject.FindGameObjectWithTag("Bars/Health").GetComponent<Slider>();
            // HBar.maxValue = MaxHealth;
            // HBar.value = MaxHealth;

            // OBar = GameObject.FindGameObjectWithTag("Bars/Oxygen").GetComponent<Slider>();
            // OBar.maxValue = MaxOxygen;
            // OBar.value = MaxOxygen;

            //Update Ping
            if(pingtext == null){
                pingtext = Info_Canvas.transform.Find("PingText").GetComponent<TextMeshProUGUI>();
            } else {
                pingtext.SetText("" + PhotonNetwork.GetPing() + "/" + PhotonNetwork.NickName);

            }
        }


        //Ignore physics when colliding with other player
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == this.gameObject.tag)
            {
                Physics.IgnoreCollision(collision.collider, this.gameObject.GetComponent<Collider>());
            }
        }
    }
}