using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using Photon.Realtime;

namespace Photon.Pun.Mine
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance = null;

        //public Text InfoText;         

        #region UNITY

        public void Awake()
        {
            Instance = this;
            StartGame();
        }

        public override void OnEnable()
        {
            base.OnEnable();            
            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
        }

        public void Start()
        {
            //InfoText.text = "Waiting for other players...";

            // Hashtable props = new Hashtable
            // {
            //     {AsteroidsGame.PLAYER_LOADED_LEVEL, true}
            // };
            // PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        #endregion

        #region COROUTINES

        private IEnumerator SpawnEnemy()
        {
            yield return new WaitForSeconds(0.5f);
            int i =0;
            while (true)
            {
                i++;
                if(i >5) break;
                yield return new WaitForSeconds(5f);

                Vector2 direction = Random.insideUnitCircle;
                Vector3 position = new Vector3(Random.Range(-8, 8), 10, Random.Range(-8, 8));
                PhotonNetwork.InstantiateSceneObject("Prefabs/Enemy", position, Quaternion.identity, 0,null);
            }
        }

        private IEnumerator EndOfGame(string winner, int score)
        {
            float timer = 5.0f;

            while (timer > 0.0f)
            {
                //InfoText.text = string.Format("Player {0} won with {1} points.\n\n\nReturning to login screen in {2} seconds.", winner, score, timer.ToString("n2"));

                yield return new WaitForEndOfFrame();

                timer -= Time.deltaTime;
            }

            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnDisconnected(DisconnectCause cause)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartCoroutine(SpawnEnemy());
                var SpaceshipScript = this.gameObject.transform.GetComponent<SpaceShip>();
                SpaceshipScript.StartCoroutine(SpaceshipScript.CFixRuntime());
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            CheckEndOfGame();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            // if (changedProps.ContainsKey(AsteroidsGame.PLAYER_LIVES))
            // {
            //     CheckEndOfGame();
            //     return;
            // }

            // if (!PhotonNetwork.IsMasterClient)
            // {
            //     return;
            // }

            // if (changedProps.ContainsKey(AsteroidsGame.PLAYER_LOADED_LEVEL))
            // {
            //     if (CheckAllPlayerLoadedLevel())
            //     {
            //         Hashtable props = new Hashtable
            //         {
            //             {CountdownTimer.CountdownStartTime, (float) PhotonNetwork.Time}
            //         };
            //         PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            //     }
            // }
        }

        #endregion

        private void StartGame()
        {

            // randomize a position
            var spawnPosition = new Vector3(Random.Range(-16, 16), 10, Random.Range(-16, 16));

            // instantiate player
            var newPlayer = PhotonNetwork.Instantiate("Prefabs/Player", spawnPosition, Quaternion.identity);
            Debug.Log(newPlayer.transform.name);
            if (PhotonNetwork.IsMasterClient)
            {
                //Start Game Functions
                StartCoroutine(SpawnEnemy());
                //Spaceship runtime
                var SpaceshipScript = this.gameObject.transform.GetComponent<SpaceShip>();
                SpaceshipScript.StartCoroutine(SpaceshipScript.CFixRuntime());
            }
                        
        }

        private bool CheckAllPlayerLoadedLevel()
        {
            // foreach (Player p in PhotonNetwork.PlayerList)
            // {
            //     object playerLoadedLevel;

            //     if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            //     {
            //         if ((bool) playerLoadedLevel)
            //         {
            //             continue;
            //         }
            //     }

            //     return false;
            // }

            return true;
        }

        private void CheckEndOfGame()
        {
            bool allDestroyed = true;

            // foreach (Player p in PhotonNetwork.PlayerList)
            // {
            //     object lives;
            //     if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_LIVES, out lives))
            //     {
            //         if ((int) lives > 0)
            //         {
            //             allDestroyed = false;
            //             break;
            //         }
            //     }
            // }

            if (allDestroyed)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StopAllCoroutines();
                }

                string winner = "";
                int score = -1;

                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if (p.GetScore() > score)
                    {
                        winner = p.NickName;
                        score = p.GetScore();
                    }
                }

                StartCoroutine(EndOfGame(winner, score));
            }
        }

        private void OnCountdownTimerIsExpired()
        {
            Debug.Log("Ended Timer");
        }
    }
}