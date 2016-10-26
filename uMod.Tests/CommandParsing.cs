using Microsoft.VisualStudio.TestTools.UnitTesting;

using uMod.Agent;

namespace uMod.Tests
{
    /// <summary>
    /// Contains parsing tests
    /// </summary>
    [TestClass]
    public class CommandParsing
    {
        /// <summary>
        /// Tests basic args in commands
        /// </summary>
        [TestMethod]
        public void CommandParsing_BasicArgs()
        {
            var cmd = new Command("testcmd thing other_thing \"escaped thing\" 10 \"escaped \\\"quoted\\\" string\"");

            Assert.AreEqual("testcmd", cmd.Verb);
            Assert.AreEqual(5, cmd.SimpleArgs.Length);
            Assert.AreEqual("thing", cmd.SimpleArgs[0]);
            Assert.AreEqual("other_thing", cmd.SimpleArgs[1]);
            Assert.AreEqual("escaped thing", cmd.SimpleArgs[2]);
            Assert.AreEqual("10", cmd.SimpleArgs[3]);
            Assert.AreEqual("escaped \"quoted\" string", cmd.SimpleArgs[4]);
        }

        /// <summary>
        /// Tests named args in commands
        /// </summary>
        [TestMethod]
        public void CommandParsing_NamedArgs()
        {
            var cmd = new Command("testcmd key1:value key2:\"escaped value\" key3:stuff:things");

            Assert.AreEqual("testcmd", cmd.Verb);
            Assert.AreEqual(3, cmd.NamedArgCount);
            Assert.AreEqual("value", cmd.GetNamedArg("key1"));
            Assert.AreEqual("escaped value", cmd.GetNamedArg("key2"));
            Assert.AreEqual("stuff:things", cmd.GetNamedArg("key3"));
        }

        /// <summary>
        /// Tests case insensitivity in commands
        /// </summary>
        [TestMethod]
        public void CommandParsing_CaseInsensitivity()
        {
            var cmd = new Command("TestCMD Key1:value key1:realValue THING:OTHERTHING");

            Assert.AreEqual("testcmd", cmd.Verb);
            Assert.AreEqual(2, cmd.NamedArgCount);
            Assert.AreEqual("realValue", cmd.GetNamedArg("Key1"));
            Assert.AreEqual("realValue", cmd.GetNamedArg("key1"));
            Assert.AreEqual("OTHERTHING", cmd.GetNamedArg("thing"));
            Assert.AreEqual("OTHERTHING", cmd.GetNamedArg("THING"));
        }

        [TestMethod]
        public void CommandParsing_ColonInEscapedNamedArg()
        {
            var cmd = new Command(". run:\"scan select:true;patch;launch\"");

            Assert.AreEqual(".", cmd.Verb);
            Assert.AreEqual(1, cmd.NamedArgCount);
            Assert.AreEqual("scan select:true;patch;launch", cmd.GetNamedArg("run"));
        }
    }
}
