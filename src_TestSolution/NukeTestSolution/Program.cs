// See https://aka.ms/new-console-template for more information
using ConsumedModule;
using System.Reflection;

Console.WriteLine("Hello, Fusion Test Solution");




BusinessLogicClass businessLogic = new();
string output = businessLogic.DoBusinessLogic("First String", "Second String");

Version? v = Assembly.GetExecutingAssembly().GetName().Version;

Console.WriteLine($"Version: {v}");

Console.WriteLine(output);