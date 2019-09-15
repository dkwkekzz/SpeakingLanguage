using System;

class Test
{
    static Test() { }

    public static string x = EchoAndReturn("In type initializer");

    public static string EchoAndReturn(string s)
    {
        Console.WriteLine(s);
        return s;
    }
}

class Test2
{
    public static Test2 instance = new Test2();

    private Test2()
    {
        Console.WriteLine("created test2.");
    }

    public void work()
    {
        Console.WriteLine("working test2.");
    }
}

class Driver
{
    private static void submain()
    {
        Test2.instance.work();
    }

    public static void Main()
    {
        Console.WriteLine("Starting Main");
        // Invoke a static method on Test
        Test.EchoAndReturn("Echo!");
        Console.WriteLine("After echo");
        // Reference a static field in Test
        string y = Test.x;
        // Use the value just to avoid compiler cleverness
        if (y != null)
        {
            Console.WriteLine("After field access");
        }

        submain();

        Console.ReadLine();
    }
}