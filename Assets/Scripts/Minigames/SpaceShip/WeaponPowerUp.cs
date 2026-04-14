using UnityEngine;

namespace SpaceShip
{
    public class WeaponPowerUp : MonoBehaviour
    {
        [SerializeField] private float fallSpeed = 3f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public int WeaponIndex { get; private set; }

        private void Update()
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

            if (transform.position.y < -8f)
            {
                Destroy(gameObject);
            }
        }

        public void Configure(int weaponIndex, Sprite[] weaponSprites)
        {
            WeaponIndex = weaponIndex;

            if (spriteRenderer != null &&
                weaponSprites != null &&
                weaponIndex >= 0 &&
                weaponIndex < weaponSprites.Length)
            {
                //spriteRenderer.sprite = weaponSprites[weaponIndex];
            }
        }
    }
}