using UnityEngine;

namespace Level
{
    public class RandomTargetsGeneration : MonoBehaviour
    {
        [Min(0)]
        public int numberOfTargets;

        public Vector3 targetSpaceBorderMin;
        public Vector3 targetSpaceBorderMax;

        public GameObject targetPrefab;
        
        private void Start()
        {
            for (var i = 0; i < numberOfTargets; ++i)
            {
                var position = RandomCoordinates();
                var instance = Instantiate(targetPrefab, position, Quaternion.identity);
            }
        }

        private Vector3 RandomCoordinates()
        {
            var x = Random.Range(targetSpaceBorderMin.x, targetSpaceBorderMax.x);
            var y = Random.Range(targetSpaceBorderMin.y, targetSpaceBorderMax.y);
            var z = Random.Range(targetSpaceBorderMin.z, targetSpaceBorderMax.z);
            return new Vector3(x, y, z);
        }
    }
}
