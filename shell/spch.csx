#!/usr/bin/env dotnet-script
#load "IShell.csx"

using System;
using System.Diagnostics;

/// SuperLocal ConCHa (Shell in portuguese)
public class Spch: IShell
{
    #region Fields
    private System.Diagnostics.Process process;

    private readonly string _fileName;
    #endregion

    #region Constructor 
    public Spch(string fileName) 
    {
        _fileName = fileName;
        process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _fileName,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
    }
    
    #endregion
    
    #region Public methods

    public string Execute(string cmd) 
    {
        process.StartInfo.Arguments = $"-c \"{EscapeArgs(cmd)}\"";
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return result;        
    }

    public string Execute(string description, string cmd) 
    {
        process.StartInfo.Arguments = $"-c \"{EscapeArgs(cmd)}\"";
        DateTime startTime = DateTime.Now;      

        process.Start();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("----- STARTED [{0} - {1}] : {2} -----", startTime.ToShortDateString(), startTime.ToShortTimeString(), description);        
        Console.ForegroundColor = ConsoleColor.White;

        string result = process.StandardOutput.ReadToEnd();
        do 
        {
            process.Refresh();
            //ToDo - Clear line before writing
            Console.Write("Duration: {0} seconds", DateTime.Now.Subtract(startTime).TotalSeconds);
        } while (!process.HasExited);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("----- FINISHED [{0} - {1}] : {2} -----", DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortTimeString(), description);
        Console.ForegroundColor = ConsoleColor.White;        
        
        return result;        
    }
    
    public string ls(params string[] args) => Execute("ls " + string.Join(" ", args));

    public string dir(params )
    #endregion

    #region Private methods
    
    protected string EscapeArgs(string cmd) => cmd.Replace("\"", "\\\"");

    public
    
    #endregion
}
