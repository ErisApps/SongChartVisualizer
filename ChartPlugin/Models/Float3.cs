using UnityEngine;

namespace SongChartVisualizer.Models
{
    public class Float3
    {
        public float x;
        public float y;
        public float z;

        public Float3() { }

        public Float3(Float3 float3) : this(float3.x, float3.y, float3.z) { }

        public Float3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Converts the PluginConfig.StoreableFloatVector3 to a UnityEngine.Vector3 format
        /// </summary>
        public static Vector3 ToVector3(Float3 float3) => new Vector3()
        {
            x = float3.x,
            y = float3.y,
            z = float3.z
        };
    }
}
