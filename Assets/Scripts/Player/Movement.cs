using UnityEngine;
using System.Collections;

namespace IrishFarmSim
{
	[RequireComponent(typeof (CharacterController))] 
	[AddComponentMenu("Third Person Player/Third Person Controller")]
	public class Movement : MonoBehaviour 
	{
	    public float rotationDamping = 20f;
	    public float Speed = 0.1f;
	    public int gravity = 20;
		public GameObject currentFocus;
		public AudioClip moveSound;
		public static bool freeRoam;

	    private float moveSpeed;
		private CharacterController controller;
		private Animator animator;
		private AudioSource audioSource;

	    private CameraController cameraController;

	    void Start()
	    {
			try
			{
		        freeRoam = true;
		        controller = (CharacterController)GetComponent(typeof(CharacterController));
		        animator = GetComponent<Animator>();
				audioSource = GetComponent<AudioSource>();
		        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
			}
			catch (UnityException e)
			{
				Debug.Log("Error - " + e);
			}
	    }

		void Update()
		{
			moveSpeed = UpdateMovement();
			animator.SetFloat("Speed", moveSpeed, 0.1f, Time.deltaTime);
		}
        public float UpdateMovement()
        {
            Vector3 inputVec;

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 camForward = cameraController.transform.forward;
            Vector3 camRight = cameraController.transform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            inputVec = camForward * v + camRight * h;

            inputVec *= Speed;

            if ((inputVec.z != 0 || inputVec.x != 0))
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = moveSound;
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.Stop();
            }

            controller.Move((inputVec + Vector3.up * -gravity) * Time.deltaTime);

            if (freeRoam)
            {
                if (inputVec != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                        Quaternion.LookRotation(inputVec),
                        Time.deltaTime * rotationDamping);
            }
            else if (currentFocus != null)
            {
                Vector3 direction = (currentFocus.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationDamping / 3);
            }
            else
            {
                freeRoam = true;
            }

            return inputVec.magnitude;
        }

	    public void LookAt(GameObject target)
	    {
	        currentFocus = target;     
	        freeRoam = false;  
	    }
	}
}