using HunterVsDino.Players;
using UnityEngine;

namespace HunterVsDino.CameraRig
{
    /// <summary>
    /// Smoothly follows a target. With no target set it follows the local player
    /// (index 0 for now; in online co-op each client sets its own hunter).
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [Tooltip("Time to catch up with the target. 0 = locked to the target.")]
        [SerializeField] private float smoothTime = 0.15f;

        private Vector3 velocity;

        public Transform Target
        {
            get => target;
            set => target = value;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                if (PlayerRegistry.All.Count == 0) return;
                target = PlayerRegistry.All[0].transform;
                SnapToTarget();
            }

            Vector3 goal = new(target.position.x, target.position.y, transform.position.z);
            transform.position = smoothTime > 0f
                ? Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime)
                : goal;
        }

        public void SnapToTarget()
        {
            if (target == null) return;
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
            velocity = Vector3.zero;
        }
    }
}
