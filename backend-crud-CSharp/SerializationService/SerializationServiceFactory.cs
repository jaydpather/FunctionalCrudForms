namespace RebelSoftware.Serialization
{
    public interface ISerializationService
    {
        //todo: these function names should not contain "Json" b/c this interface could be used for other formats like XML or CSV
        T DeserializeFromJson<T>(string jsonString);
        string SerializeToJson<T>(T objToSerialize);
    }

    public static class SerializationServiceFactory
    {
        public static ISerializationService CreateJsonSerializationService()
        {
            return new JsonSerializationService();
        }
    }
}