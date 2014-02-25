using System;
using NUnit.Framework;
using BuildCSystem.Projects;

namespace Tests
{
    [TestFixture]
    public class PathTests
    {
        [TestCase("/home/inb/file/folder/file.cs", "/home/inb", "file\\folder\\file.cs")]
        [TestCase("/home/inb/file/file.cs", "/home/inb", "file\\file.cs")]
        public void TestPaths(string file, string folder, string expected)
        {
             Assert.AreEqual(expected, PathTools.NativeToRelativeWindows(file, folder));
        }

        [TestCase("file.cs", "/home/inb")]
        [TestCase("/home/inb/file.cs", "inb")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidPaths(string file, string folder)
        {
            PathTools.NativeToRelativeWindows(file, folder);
        }
    }
}



