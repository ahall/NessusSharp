using System;
using DotNessus;

namespace Tests
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Connection conn = new Connection("127.0.0.1", "ahall", "temp123", true);
        }
    }
}

