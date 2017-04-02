using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetWorker.Utilities;

namespace Game_Core.Utilities
{
    static class ExtensionMethods
    {
        public static void PutVector3(this RawMessage message, string key, Vector3 value)
        {
            float[] floats = new float[]{value.x,value.y,value.z};
            
            var byteArray = new byte[floats.Length * 4];
            Buffer.BlockCopy(floats, 0, byteArray, 0, byteArray.Length);
            
            message.putByteArray(key, byteArray);
        }

        public static Vector3 GetVector3(this RawMessage message, string key)
        {
            byte[] bytes = message.getByteArray(key);

            float[] floats = new float[bytes.Length/4];
            Buffer.BlockCopy(bytes,0,floats,0,bytes.Length);

            return new Vector3(floats[0], floats[1], floats[2]);
        }

        public static void PutVector3Array(this RawMessage message, string key, Vector3[] values)
        {
            float[] floats = new float[values.Length*3];
            for(int i=0; i<values.Length; i++)
            {
                floats[i*3] = values[i].x;
                floats[i*3+1] = values[i].y;
                floats[i*3+2] = values[i].z;
            }

            var byteArray = new byte[floats.Length * 4];
            Buffer.BlockCopy(floats, 0, byteArray, 0, byteArray.Length);

            message.putByteArray(key, byteArray);
        }

        public static Vector3[] GetVector3Array(this RawMessage message, string key)
        {
            byte[] bytes = message.getByteArray(key);

            float[] floats = new float[bytes.Length / 4];
            Buffer.BlockCopy(bytes, 0, floats, 0, bytes.Length);

            Vector3[] result = new Vector3[floats.Length/3];
            for (int i = 0; i < result.Length; i++)
                result[i] = new Vector3(floats[i * 3], floats[i * 3 + 1], floats[i * 3+2]);
            return result;
        }
    }
}
