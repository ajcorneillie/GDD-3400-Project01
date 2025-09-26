using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

namespace GDD3400.Project01
{
    public class Dog : MonoBehaviour
    {
        
        private bool _isActive = true;
        public bool IsActive 
        {
            get => _isActive;
            set => _isActive = value;
        }

        // Required Variables (Do not edit!)
        private float _maxSpeed = 5f;
        private float _sightRadius = 7.5f;

        // Layers - Set In Project Settings
        public LayerMask _targetsLayer;
        public LayerMask _obstaclesLayer;

        // Tags - Set In Project Settings
        private string friendTag = "Friend";
        private string threatTag = "Threat";
        private string safeZoneTag = "SafeZone";

        //Random Area Selector Bounds
        float maxX = 24;
        float minX = -24;
        float minZ = -24;
        float maxZ = 24;

        //Random References
        GameObject safeZoneRef;
        Quaternion newRotation;
        Vector3 targetPosition;
        private Transform sheepTarget;
        bool hitSheep = true;

        //Awake Method
        public void Awake()
        {
            // Find the layers in the project settings
            _targetsLayer = LayerMask.GetMask("Targets");
            _obstaclesLayer = LayerMask.GetMask("Obstacles");
            gameObject.tag = friendTag;

            GameObject[] safeZone = GameObject.FindGameObjectsWithTag("SafeZone");
            safeZoneRef = safeZone[0];
            targetPosition = new Vector3(maxX, transform.position.y, maxZ);
            PickRandomPos();
        }

        //Update Method
        void Update()
        {
            DecisionMaking();
        }

        //Decides What to Do
        private void DecisionMaking()
        {
            //Defines the direction to move towards and the direction to face
            transform.position = Vector3.MoveTowards(transform.position,targetPosition,_maxSpeed * Time.deltaTime);
            Vector3 direction = (new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position);
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            //Checks if it has hit a sheep
            if (hitSheep == false)
            {
                //looks for sheep within its sight range
                Collider[] hits = Physics.OverlapSphere(transform.position, _sightRadius);
                foreach (Collider hit in hits)
                {
                    //Makes sure it is a sheep and not the dog
                    if (hit.CompareTag("Friend") && hit.gameObject != gameObject && hit.gameObject.transform.position.y == 0)
                    {
                        sheepTarget = hit.transform;
                    }
                }
            }

            //If a sheep is has not been hit and is in sight range go towards it
            if (sheepTarget != null && hitSheep == false)
            {
                targetPosition = sheepTarget.position;
            }
            //If a sheep has been hit and is not in safe zone move twards safe zone
            else if(hitSheep == true && Vector3.Distance(transform.position, safeZoneRef.transform.position) > 1f)
            {
                targetPosition = safeZoneRef.transform.position;
                _maxSpeed = 2f;
            }
            //i within certain range to random position select new random position
            else if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
            {
                PickRandomPos();
                sheepTarget = null;
                hitSheep = false;
            }
        }

        //selects a random position to go to
        private void PickRandomPos()
        {
            float randX = Random.Range(minX, maxX);
            float randZ = Random.Range(minZ, maxZ);

            targetPosition = new Vector3(randX, transform.position.y, randZ);
            _maxSpeed = 5f;
        }

        //checks for collision with a sheep
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Friend")
            {
                hitSheep = true;
            }
        }
    }
}
