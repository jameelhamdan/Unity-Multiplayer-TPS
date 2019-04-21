using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Photon.Pun.Mine
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class PlayerMotor : MonoBehaviour
    {
        public float Speed = 5f;
        public float JumpHeight = 2f;
        public float GroundDistance = 0.2f;
        public LayerMask Ground;
        private Rigidbody _body;
        private Vector3 _inputs = Vector3.zero;
        private bool _isGrounded = true;
        private Transform _groundChecker;
        private GameObject _mainCamera;

        private Animator anim;
        //Initilize
        void Start()
        {
            _mainCamera = Camera.main.gameObject;
            _body = GetComponent<Rigidbody>();
            _groundChecker = transform.GetChild(0);
        }

        //Call Functions
        void Update()
        {
            CheckGround();
            Move();
            animate();
        }

        void FixedUpdate()
        {
            //Move player
            _body.MovePosition(_body.position + _inputs.normalized * Speed * Time.fixedDeltaTime);
            _mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z - 10);
        }

        private void CheckGround()
        {
            //Check For Ground
            _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        }

        private void Move()
        {
            //Get Input Values
            _inputs = Vector3.zero;
            _inputs.x = Input.GetAxis("Horizontal");
            _inputs.z = Input.GetAxis("Vertical");

            //Player Forward is direction of movement 
            if (_inputs != Vector3.zero)
            {
                transform.forward = _inputs;
            }
        }
        
        private void animate()
        {
            if (anim == null)
            {
                try
                {
                    anim = this.gameObject.GetComponent<PlayerNetwork>().anim;
                }
                catch
                {

                }
            } else {
                var _inputs = Vector3.zero;
                _inputs.x = Input.GetAxis("Horizontal");
                _inputs.z = Input.GetAxis("Vertical");
                
                //Player Forward is direction of movement 
                if (_inputs != Vector3.zero)
                {
                    anim.SetInteger ("AnimationPar", 1);
                } else {
                    anim.SetInteger ("AnimationPar", 0);
                }
            }            
        }
    }
}