using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoImporter._Dependencies;
using Moq;
using Moq.Language.Flow;

namespace PhotoImporter.Tests;

[TestClass]
public class MainTests {
    Mock<IConsoleWriter> _consoleWriter;

    [TestInitialize]
    public void Setup() {
        _consoleWriter = new Mock<IConsoleWriter>();

        Program.SetConsoleWriter(_consoleWriter.Object);
    }

    [TestMethod]
    public void Main_NoArgs_HelpPrinted() {
        main();
    }

    void main(params string[] args)
        => Program.Main(args);
}