#!/usr/bin/env dotnet-script
#load "spch.csx"

using System;
using System.Diagnostics;

public class Bash: Spch, IShell
{

    #region Constructor 
    public Bash() : base("/bin/bash")
    {
        
    }
    
    #endregion    
}