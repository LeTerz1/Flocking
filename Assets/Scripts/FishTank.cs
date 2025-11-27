using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class FishTank : MonoBehaviour
{
    [SerializeField]
    private Vector2 Size = new Vector2(16f, 9f);

    [SerializeField]
    [FormerlySerializedAs("FishPrefab")]
    private GameObject fish = null;

    //[Range(1, 100)]
    [SerializeField]
    private int SpawningCount;


    private Fish[] fishes = null;
    private List<Fish> fishList = new List<Fish>();

    private void Start()
    {
        fishes = new Fish[SpawningCount];
        for (int i = 0; i < SpawningCount; i++)
        {
            CreateFish(Vector3.zero);
        }
    }

    private void CreateFish(Vector3 worldposition)
    {
        GameObject fishInstance = Instantiate(fish, transform);
        fishInstance.gameObject.name = $"Fish {System.Guid.NewGuid()}";
       fishList.Add(fishInstance.GetComponent<Fish>());
    }
    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            CreateFish(transform.InverseTransformPoint(mousePosition));

        }
        // Loop around out of bound fishes.
        int fishesCount = fishes.Length;
        for (int i = 0; i < fishesCount; i++)
        {
            Fish fish = fishes[i];
            Vector3 position = fish.transform.localPosition;

            // Left border?
            if (position.x < -Size.x * 0.5f)
            {
                position.x += Size.x;
            }
            // Right border?
            else if (position.x > Size.x * 0.5f)
            {
                position.x -= Size.x;
            }

            // Top border?
            if (position.y > Size.y * 0.5f)
            {
                position.y -= Size.y;
            }
            // Bottom border?
            if (position.y <  -Size.y * 0.5f)
            {
                position.y += Size.y;
            }

            fish.transform.localPosition = position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Size);
    }
}