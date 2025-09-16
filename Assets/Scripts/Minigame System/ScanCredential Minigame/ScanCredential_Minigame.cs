using UnityEngine;
using System.Collections;

namespace MinigameSystem.Minigames
{
    /// <summary>
    /// Credential verification minigame.
    /// Detects when a GrabAndDrop object is placed near the target point.
    /// If the object remains for 1 second, starts the verification animation.
    /// When the animation ends, ConfirmVerification is called (via Animator Event).
    /// If the object name matches the expected idName -> Complete, else -> Reject and retry.
    /// </summary>
    public class ScanCredential_Minigame : MinigameHandler
    {
        [Header("Target Settings")]
        [Tooltip("The RectTransform that represents the target point.")]
        [SerializeField] private RectTransform targetPoint;

        [Tooltip("List of draggable objects that can be checked.")]
        [SerializeField] private GrabAndDrop[] objects;

        [Tooltip("Maximum distance allowed to consider the object near the target.")]
        [SerializeField] private float detectionRadius = 50f;

        [Tooltip("Expected object ID name.")]
        [SerializeField] private string idName;

        [Header("Animation")]
        [Tooltip("Animator that will play the verification animation.")]
        [SerializeField] private Animator animator;

        private GrabAndDrop currentObject;
        private Coroutine checkCoroutine;
        private bool isVerifying;

        // Object currently being verified
        private GameObject verifyingObject;

        protected override void OnStartGame()
        {
            ResetState();
        }

        protected override void UpdateGame()
        {
            if (isVerifying) return;

            foreach (var obj in objects)
            {
                if (obj == null) continue;

                float distance = Vector2.Distance(
                    obj.transform.position,
                    targetPoint.position
                );

                if (distance <= detectionRadius)
                {
                    // If not already checking something, start verification
                    if (checkCoroutine == null)
                    {
                        currentObject = obj;
                        checkCoroutine = StartCoroutine(StartVerification(obj));
                    }
                }
                else
                {
                    // Cancel if the current object moves away
                    if (obj == currentObject)
                    {
                        CancelVerification();
                    }
                }
            }
        }

        /// <summary>
        /// Coroutine that waits before starting the animation.
        /// </summary>
        private IEnumerator StartVerification(GrabAndDrop obj)
        {
            yield return new WaitForSeconds(1f);

            if (obj == null) yield break;

            // Begin animation
            isVerifying = true;
            verifyingObject = obj.gameObject;
            animator.SetTrigger("Verify");
        }

        /// <summary>
        /// Cancels verification if the object moves away.
        /// </summary>
        private void CancelVerification()
        {
            if (checkCoroutine != null)
            {
                StopCoroutine(checkCoroutine);
                checkCoroutine = null;
            }

            isVerifying = false;
            currentObject = null;
            verifyingObject = null;
        }

        /// <summary>
        /// Called by the Animator event when the verification animation finishes.
        /// </summary>
        public void ConfirmVerification()
        {
            if (verifyingObject == null)
            {
                ResetState();
                return;
            }

            if (verifyingObject.name == idName)
            {
                Complete();
            }
            else
            {
                Reject();
            }
        }

        /// <summary>
        /// Success: completes the game.
        /// </summary>
        private void Complete()
        {
            CompleteGame("Scan Credential", "No log is aviable", 100);
            ResetState();
        }

        /// <summary>
        /// Failure: rejects and allows retry.
        /// </summary>
        private void Reject()
        {
            // Could play a fail sound or feedback here
            ResetState();
        }

        /// <summary>
        /// Resets verification state so the game can continue.
        /// </summary>
        private void ResetState()
        {
            if (checkCoroutine != null)
            {
                StopCoroutine(checkCoroutine);
                checkCoroutine = null;
            }

            isVerifying = false;
            currentObject = null;
            verifyingObject = null;
        }
    }
}
