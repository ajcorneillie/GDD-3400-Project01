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

        float maxX = 24;
        float minX = -24;
        float minZ = -24;
        float maxZ = 24;

        GameObject safeZoneRef;
        float stateChangeZLevel = -20;
        float moveZperTurn = 3;

        bool goLeft = false;
        bool goRight = false;
        bool readyTurn = true;
        bool startDone = false;
        Quaternion newRotation;
        Vector3 targetPosition;
        private Transform sheepTarget;
        bool hitSheep = true;

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

        void Update()
        {
            DecisionMaking();
        }

        private void Perception()
        {
            if (transform.position == targetPosition)
            {
                startDone = true;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, _maxSpeed * Time.deltaTime);
                readyTurn = true;
            }     
        }

        private void DecisionMaking()
        {
            transform.position = Vector3.MoveTowards(transform.position,targetPosition,_maxSpeed * Time.deltaTime);
            Vector3 direction = (new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position);
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            if (hitSheep == false)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, _sightRadius);
                foreach (Collider hit in hits)
                {
                    if (hit.CompareTag("Friend") && hit.gameObject != gameObject && hit.gameObject.transform.position.y == 0)
                    {
                        sheepTarget = hit.transform;
                    }
                }
            }


            if (sheepTarget != null && hitSheep == false)
            {
                targetPosition = sheepTarget.position;
            }
            else if(hitSheep == true && Vector3.Distance(transform.position, safeZoneRef.transform.position) > 1f)
            {
                targetPosition = safeZoneRef.transform.position;
                _maxSpeed = 2f;
            }
            else if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
            {
                PickRandomPos();
                sheepTarget = null;
                hitSheep = false;
            }
        }

        private void PickRandomPos()
        {
            float randX = Random.Range(minX, maxX);
            float randZ = Random.Range(minZ, maxZ);

            targetPosition = new Vector3(randX, transform.position.y, randZ);
            _maxSpeed = 5f;
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Friend")
            {
                hitSheep = true;
            }
        }
    }
}
