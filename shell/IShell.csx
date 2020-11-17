#!/usr/bin/env dotnet-script

using System;

public interface IShell 
{
    string Execute(string cmd);

    string Execute(string description, string cmd);
    
    string ls(params string[] args);
}
