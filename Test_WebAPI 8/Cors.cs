using System.Text;

namespace Test_WebAPI_8
{
    public record Cors
    {
        public string[]? Origins { get; set; }
        public string[]? Headers { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            ObjString(sb, Origins, nameof(Origins));

            ObjString(sb, Headers, nameof(Headers));

            return sb.ToString();
        }

        private void ObjString(StringBuilder sb, string[]? arr, string name)
        {
            if (arr == null) return;

            int index = 0;
            sb.Append($" {name} " + "[");
            foreach (string s in arr)
            {
                sb.Append($"\"{s}\"");
                if (arr.Length > 1 && arr.Length - 1 != index)
                {
                    sb.Append(",");
                }
                index++;
            }
            sb.Append("]");
        }
    }
}
