namespace Cirilla.Models
{
    public class StringKeyValuePair
    {
        public string Key { get; }
        public string Value { get; }

        public StringKeyValuePair(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
