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

        public void Awake()
        {
            // Find the layers in the project settings
            _targetsLayer = LayerMask.GetMask("Targets");
            _obstaclesLayer = LayerMask.GetMask("Obstacles");
            gameObject.tag = friendTag;

            GameObject[] safeZone = GameObject.FindGameObjectsWithTag("SafeZone");
            safeZoneRef = safeZone[0];
            targetPosition = new Vector3(maxX, transform.position.y, maxZ);
        }

        private void Update()
        {
            if (!_isActive) return;
            if (startDone == true)
            {
                DecisionMaking();
            }
            else
            {
                Perception();
            }
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
            if (goLeft == true)
            {
                if (transform.position.x == minX)
                {
                    readyTurn = true;
                    goLeft = false;
                    newRotation = Quaternion.Euler(0f, 180f, 0f);
                    targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - moveZperTurn);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, _maxSpeed * Time.deltaTime);
                }
            }
            else if (goRight == true)
            {
                if (transform.position.x == maxX)
                {
                    readyTurn = true;
                    goRight = false;
                    newRotation = Quaternion.Euler(0f, 180f, 0f);
                    targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - moveZperTurn);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, _maxSpeed * Time.deltaTime);
                }
            }
            else if (readyTurn == true)
            {
                if (transform.position == targetPosition)
                {
                    if (transform.position.y > 0f)
                    {
                        goRight = true;
                        readyTurn = false;
                        newRotation = Quaternion.Euler(0f, 90f, 0f);
                        targetPosition = new Vector3(transform.position.x + maxX * 2, transform.position.y, transform.position.z);
                    }
                    else
                    {
                        readyTurn = false;
                        goLeft = true;
                        newRotation = Quaternion.Euler(0f, -90f, 0f);
                        targetPosition = new Vector3(transform.position.x + minX * 2, transform.position.y, transform.position.z);
                    }
                }
                else
                {
                    if (targetPosition.z <= -24)
                    {
                        transform.position = Vector3.Lerp(transform.position, safeZoneRef.transform.position, _maxSpeed * Time.deltaTime);
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, targetPosition, _maxSpeed * Time.deltaTime);
                    }
                    
                }             
            }
        }

        /// <summary>
        /// Make sure to use FixedUpdate for movement with physics based Rigidbody
        /// You can optionally use FixedDeltaTime for movement calculations, but it is not required since fixedupdate is called at a fixed rate
        /// </summary>
        private void FixedUpdate()
        {
            if (!_isActive) return;
            
        }
    }
}
