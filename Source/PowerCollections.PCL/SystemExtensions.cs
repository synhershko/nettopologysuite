namespace System
{
    internal class Console
    {
        public static void WriteLine() { }

        public static void WriteLine(string s, params object[] arr) { }

        public static void Write(string s, params object[] arr) { }
    }

    public delegate TOutput Converter<in TInput, out TOutput>(TInput input);
}
