namespace Yafers.Web.Utils
{
    public static class StringExtensions
    {
        public static List<string> SplitInPartsByWords(this string text, int maxLength = 3000)
        {
            var parts = new List<string>();

            if (string.IsNullOrEmpty(text))
                return parts;

            int index = 0;
            while (index < text.Length)
            {
                int length = Math.Min(maxLength, text.Length - index);

                if (index + length >= text.Length)
                {
                    parts.Add(text.Substring(index));
                    break;
                }

                int lastSpace = text.LastIndexOf(' ', index + length, length);

                if (lastSpace > index)
                {
                    parts.Add(text.Substring(index, lastSpace - index));
                    index = lastSpace + 1; // пропускаем пробел
                }
                else
                {
                    parts.Add(text.Substring(index, length));
                    index += length;
                }
            }

            return parts;
        }
    }
}
