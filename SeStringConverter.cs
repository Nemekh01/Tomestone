using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lumina.Text;
using Lumina.Text.Payloads;

namespace LuminaAPI
{
    public class SeStringConverter : JsonConverter< SeString >
    {
        private readonly Lumina.GameData _lumina;

        public SeStringConverter(Lumina.GameData lumina) : base()
        {
            _lumina = lumina;
        }
        public override SeString Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options ) => null;

        public override void Write(
            Utf8JsonWriter writer,
            SeString seString,
            JsonSerializerOptions options ) =>
            //writer.WriteStringValue("[VGS] Shazbot!");
            writer.WriteStringValue(CreateHtml(seString));
            //writer.WriteStringValue( seString.RawString );

        private string CreateHtml(SeString input)
        {
            //var toReturn = "[VGS] Shazbot!";
            var toReturn = "";
            var rawData = input.Payloads;
            foreach (var x in rawData)
            {
                if (x is TextPayload)
                {
                    toReturn += x;
                }
                else
                {
                    toReturn += ParseHtmlFromPayload(x.Data);
                }
            }
            return toReturn;
        }
        
        private string ParseHtmlFromPayload(ReadOnlySpan<byte> input)
        {
            string toReturn = "";
            var br = new BinaryReader(new MemoryStream(input.ToArray()));
            // Move past 0x2 start marker
            var d = br.ReadByte();
            
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                d = br.ReadByte();
                switch(d)
                {
                    case 0x48:
                        toReturn += ParseTextColor(br);
                        break;
                    case 0x49:
                        toReturn += ParseTextGlow(br);
                        break;
                    case 0x10:
                        toReturn += "<br>";
                        break;

                    default:
                        break;
                }
            }
            return toReturn;
        }

        private string ParseTextColor(BinaryReader br)
        {
            // Skip unknown intro bytes
            br.BaseStream.Position += 2;
            //uint color = _lumina.GetExcelSheet<Lumina.Excel.GeneratedSheets.UIColor>().GetEnumerator();
            return "";
        }

        private string ParseTextGlow(BinaryReader br)
        {
            return "";
        }
    }
}