using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.CSharp;

public class TestClass { }

public interface Foo
{
    void test();
}

namespace ML.ExtTest
{
    public class CompilerTest
    {
        static void CompileAndGo(string code)
        {
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.ReferencedAssemblies.Add("PokerBot.exe");

            CodeDomProvider provider = new CSharpCodeProvider();

            CompilerResults compilerResults = provider.CompileAssemblyFromSource(cp, code);

            if (compilerResults.Errors.HasErrors)
            {
                foreach (CompilerError error in compilerResults.Errors)
                {
                    Console.WriteLine("COMPILER ERROR: " + error.ErrorText);
                }
            }
            else
            {
                Type testClassType = compilerResults.CompiledAssembly.GetType("TestClass");
                Foo foo = Activator.CreateInstance(testClassType) as Foo;
                foo.test();



                testClassType.InvokeMember(String.Empty, BindingFlags.CreateInstance, null, null, null);
            }
        }

        public static void Main()
        {
            string code =
              "using System;" +
              "using ML.ExtTest;" +
              "public class TestClass : Foo" +
              "  {" +
              "  public TestClass()" +
              "    {" +
              "    string[] message = { \"Success!\" };" +
              "" +
              "    foreach( string str in message )" +
              "      {" +
              "      Console.WriteLine( str );" +
              "      }" +
              "    }" +
              " public void test() { Console.WriteLine(\"test!!!\"); }" +
              "  }";

            Console.WriteLine("This one works:");
            CompileAndGo(code);

            Console.ReadKey();
        }
    }
}

