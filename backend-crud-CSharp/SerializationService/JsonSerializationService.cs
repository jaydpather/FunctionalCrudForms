using System;
using Newtonsoft.Json;

namespace RebelSoftware.Serialization
{
    public class JsonSerializationService : ISerializationService
    {
        public T DeserializeFromJson<T>(string jsonString)
        {
            var retVal = JsonConvert.DeserializeObject<T>(jsonString);
            return retVal;
        }

        public string SerializeToJson<T>(T objToSerialize)
        {
            var retVal = JsonConvert.SerializeObject(objToSerialize);
            return retVal;
        }
    }
}
