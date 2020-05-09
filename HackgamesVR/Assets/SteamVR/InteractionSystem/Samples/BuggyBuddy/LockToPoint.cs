using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Valve.VR.InteractionSystem.Sample
{
    public class LockToPoint : MonoBehaviour
    {
        public Transform snapTo;
        public float snapTime = 2;

        private float dropTimer;
        private Interactable interactable;

        private void Start()
        {
            interactable = GetComponent<Interactable>();
        }

        private void FixedUpdate()
        {
            bool used = false;
            if (interactable != null)
                used = interactable.attachedToHand;

            if (used)
            {
                dropTimer = -1;
            }
            else
            {
                dropTimer += Time.deltaTime / (snapTime / 2);


                if (dropTimer > 1)
                {
                    transform.position = snapTo.position;
                    transform.rotation = snapTo.rotation;
                }
                else
                {
                    float t = Mathf.Pow(35, dropTimer);

                    transform.position = Vector3.Lerp(transform.position, snapTo.position, Time.fixedDeltaTime * t * 3);
                    transform.rotation = Quaternion.Slerp(transform.rotation, snapTo.rotation, Time.fixedDeltaTime * t * 2);
                }
            }
        }
    }
}