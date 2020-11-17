#!/usr/bin/env dotnet-script
#load "IShell.csx"
#load "spch.csx"

using System;
using System.Diagnostics;

public class Fish: Spch, IShell
{

    #region Constructor 

    private readonly string _configPath;
    private readonly string _functionsPath;

    public Fish(string configPath = "~/.config/fish/") : base("usr/bin/fish")
    {
        _configPath = configPath;
        _functionsPath = Path.Combine(configPath, "functions");
    }
    
    public bool CreateFunction(string name, string description, string[] function)  
    {
        if (!System.IO.Directory.Exists(_functionsPath))
            System.IO.Directory.CreateDirectory(_functionsPath);

        System.IO.File.WriteAllLines(Path.Combine(_functionsPath, string.Concat(name, ".fish")), function);        
    }
    #endregion    
}
