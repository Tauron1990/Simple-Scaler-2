using System;
using System.IO;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Simple_Scaler_2.Processing
{
    [PublicAPI, Serializable]
    public sealed class TransformSettings
    {
        private static readonly XmlSerializer TransformSerializer = new XmlSerializer(typeof(TransformSettings));

        [XmlIgnore]
        public string FilePath { get; set; }

        public bool Checker { get; set; }
        public int  Rand1X  { get; set; } = 10;
        public int  Rand1Y  { get; set; } = 10;
        public int  Rand2X  { get; set; } = -1;
        public int  Rand2Y  { get; set; } = -1;
        public int  Kor1X   { get; set; }
        public int  Kor1Y   { get; set; }
        public int  Kor2X   { get; set; }
        public int  Kor2Y   { get; set; }
        public int  Kor3X   { get; set; }
        public int  Kor3Y   { get; set; }
        public int  Kor4X   { get; set; }
        public int  Kor4Y   { get; set; }

        public static TransformSettings FromSettings(string path)
        {
            using (var reader = new StreamReader(path))
            {
                var set      = (TransformSettings) TransformSerializer.Deserialize(reader);
                set.FilePath = path;
                return set;
            }
        }

        public void Save(string path)
        {
            using (var writer = new StreamWriter(path)) TransformSerializer.Serialize(writer, this);
        }

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(FilePath)) return;
            Save(FilePath);
        }
    }
}