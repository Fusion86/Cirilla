namespace Cirilla.MVVM.Common
{
    // Sometimes we can't KeyValuePair<string, String>, for example with CsvReader.GetRecords<T>().
    public class StringKeyValuePair
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public StringKeyValuePair(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
