using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MinigameSystem.Minigames.FindYourCredentials
{
    /// <summary>
    /// Infinite horizontal carousel for UI elements in a Canvas.
    /// Always shows 3 elements on screen: the center element is enlarged and side elements are smaller.
    /// Supports moving left and right with smooth interpolation and automatically handles draw order.
    /// </summary>
    public class InfiniteCarousel : MonoBehaviour
    {
        [Header("Carousel Elements")]
        [Tooltip("List of images to display in the carousel. They should be arranged horizontally.")]
        [SerializeField] private List<RectTransform> items = new List<RectTransform>();

        [Header("Carousel Settings")]
        [Tooltip("Duration of the movement interpolation in seconds.")]
        [SerializeField] private float moveDuration = 0.5f;

        [Tooltip("Scale of the centered element.")]
        [SerializeField] private float centerScale = 1.2f;

        [Tooltip("Scale of side elements.")]
        [SerializeField] private float sideScale = 0.8f;

        [Tooltip("Spacing between elements in pixels.")]
        [SerializeField] private float spacing = 200f;

        private int centerIndex = 0; // current center element
        private bool isMoving = false;

        public int CurrentIndex => centerIndex;

        private void Start()
        {
            // Initialize positions and scales
            UpdateCarouselImmediate();
            UpdateDrawOrder();
        }

        /// <summary>
        /// Move the carousel to the left (next element becomes center)
        /// </summary>
        public void MoveLeft()
        {
            if (isMoving) return;
            centerIndex = (centerIndex + 1) % items.Count;
            StartCoroutine(AnimateCarousel());
        }

        /// <summary>
        /// Move the carousel to the right (previous element becomes center)
        /// </summary>
        public void MoveRight()
        {
            if (isMoving) return;
            centerIndex = (centerIndex - 1 + items.Count) % items.Count;
            StartCoroutine(AnimateCarousel());
        }

        /// <summary>
        /// Immediately sets the carousel positions and scales without interpolation
        /// </summary>
        private void UpdateCarouselImmediate()
        {
            for (int i = 0; i < items.Count; i++)
            {
                int relativeIndex = (i - centerIndex + items.Count) % items.Count;

                float targetX = 0f;
                float targetScale = sideScale;

                if (relativeIndex == 0)
                {
                    targetX = 0f;
                    targetScale = centerScale;
                }
                else if (relativeIndex == 1 || (relativeIndex == -items.Count + 1))
                {
                    targetX = spacing;
                }
                else if (relativeIndex == items.Count - 1 || relativeIndex == -1)
                {
                    targetX = -spacing;
                }
                else
                {
                    // hide extra items offscreen
                    targetX = 1000f;
                }

                items[i].anchoredPosition = new Vector2(targetX, items[i].anchoredPosition.y);
                items[i].localScale = Vector3.one * targetScale;
            }

            UpdateDrawOrder();
        }

        /// <summary>
        /// Coroutine to smoothly interpolate positions and scales of carousel elements
        /// </summary>
        private IEnumerator AnimateCarousel()
        {
            isMoving = true;

            // Store starting positions and scales
            Vector2[] startPositions = new Vector2[items.Count];
            Vector3[] startScales = new Vector3[items.Count];

            Vector2[] targetPositions = new Vector2[items.Count];
            Vector3[] targetScales = new Vector3[items.Count];

            for (int i = 0; i < items.Count; i++)
            {
                startPositions[i] = items[i].anchoredPosition;
                startScales[i] = items[i].localScale;

                int relativeIndex = (i - centerIndex + items.Count) % items.Count;

                float x = 0f;
                float scale = sideScale;

                if (relativeIndex == 0)
                {
                    x = 0f;
                    scale = centerScale;
                }
                else if (relativeIndex == 1 || relativeIndex == -items.Count + 1)
                {
                    x = spacing;
                }
                else if (relativeIndex == items.Count - 1 || relativeIndex == -1)
                {
                    x = -spacing;
                }
                else
                {
                    x = 1000f; // move offscreen
                }

                targetPositions[i] = new Vector2(x, items[i].anchoredPosition.y);
                targetScales[i] = Vector3.one * scale;
            }

            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);
                t = Mathf.SmoothStep(0f, 1f, t);

                for (int i = 0; i < items.Count; i++)
                {
                    items[i].anchoredPosition = Vector2.Lerp(startPositions[i], targetPositions[i], t);
                    items[i].localScale = Vector3.Lerp(startScales[i], targetScales[i], t);
                }

                yield return null;
            }

            // Ensure final positions/scales
            for (int i = 0; i < items.Count; i++)
            {
                items[i].anchoredPosition = targetPositions[i];
                items[i].localScale = targetScales[i];
            }

            UpdateDrawOrder();
            isMoving = false;
        }

        /// <summary>
        /// Updates the draw order so the center element appears in front and side elements behind
        /// </summary>
        private void UpdateDrawOrder()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (i == centerIndex)
                    items[i].SetAsLastSibling(); // center on top
                else
                    items[i].SetSiblingIndex(0); // sides behind
            }
        }
    }
}
