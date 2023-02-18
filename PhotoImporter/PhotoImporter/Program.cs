using PhotoImporter._Dependencies;

namespace PhotoImporter {
    public class Program {
        static IConsoleWriter _consoleWriter = new ConsoleWriter();

        public static void SetConsoleWriter(IConsoleWriter consoleWriter) {
            _consoleWriter = consoleWriter;
        }

        public static void Main(string[] args) {

        }
    }
}