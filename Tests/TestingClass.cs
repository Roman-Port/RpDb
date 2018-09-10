using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RpDb;

namespace Tests
{
    class TestingClass
    {
        [ClassAttrib(1)]
        public string testingInt { get; set; }

        public TestingClass()
        {
            testingInt = "23";
        }
    }
}
