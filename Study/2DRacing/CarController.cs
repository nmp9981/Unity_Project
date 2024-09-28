using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Racing2D
{
    public class CarController : MonoBehaviour
    {
        public Rigidbody2D carRigidbody;
        public Rigidbody2D backTire;
        public Rigidbody2D frontTire;

        public float speed = 1500;
        public float carTorque = 30;
        private float movement;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            movement = Input.GetAxis("Horizontal");
        }
        private void FixedUpdate()
        {
            backTire.AddTorque(-movement * speed * Time.fixedDeltaTime * 1000f);
            frontTire.AddTorque(-movement * speed * Time.fixedDeltaTime * 1000f);
            carRigidbody.AddTorque(-movement * carTorque * Time.fixedDeltaTime * 1000f);
        }
    }

}
