using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EZBM.Core.Tools;

/// <summary>
/// Provides enhanced logging capabilities with contextual information and stack tracing.
/// <br/><br/>
/// Author: OpenAI ChatGPT, Google Gemini, DefinitelyRus
/// </summary>
public class Log
{
    public enum Mode
    {
        Message,
        Warning,
        Error
    }

    #region Static Logging

    /// <summary>
    /// Logs a trace message with contextual information to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="printAs">Specifies the severity level of the message.</param>
    /// <param name="frameDepth">The number of stack frames to skip.</param>
    /// <param name="printTrace">Whether to print the full trace.</param>
    /// <param name="filePath">Provided by the compiler.</param>
    /// <param name="line">Provided by the compiler.</param>
    public static void Message(
        string message, 
        Mode printAs, 
        int frameDepth, 
        bool printTrace, 
        [CallerFilePath] string filePath = "", 
        [CallerLineNumber] int line = 0)
    {
        StackTrace trace = new(frameDepth, true);
        StackFrame[]? frames = trace.GetFrames();
        
        bool noFrames = frames == null || frames.Length == 0;
        if (noFrames)
        {
            string err = "No stack frames available for trace logging.";
            throw new InvalidOperationException(err);
        }

        StackFrame[] relevantFrames = FilterFrames(frames!);
        int depth = 0;
        
        foreach (StackFrame frame in relevantFrames)
        {
            MethodBase? method = frame.GetMethod();
            string indent = new(' ', depth);
            string prefix;

            if (method != null)
            {
                string className = method.DeclaringType?.Name ?? "UNKNOWN_CLASS";
                string methodName = method.Name;
                
                bool isTopLevelMain = methodName == "<Main>$";
                if (isTopLevelMain)
                {
                    methodName = "MAIN";
                }
                
                int frameLine = frame.GetFileLineNumber();
                
                string rootIndent = depth == 0 ? "\n" : "";
                indent = rootIndent + indent;
                
                string locationInfo = frameLine > 0 
                    ? $"{className}.{methodName}:{frameLine}" 
                    : $"{className}.{methodName}:?";
                    
                prefix = $"{indent}[{locationInfo}]";
            }
            else
            {
                string fileName = Path.GetFileName(filePath);
                prefix = $"{indent}[{fileName} @ line {line}]";
            }

            bool isLastFrame = depth == relevantFrames.Length - 1;
            if (isLastFrame)
            {
                string insert = printAs switch
                {
                    Mode.Warning => "WARN: ",
                    Mode.Error => "ERROR: ",
                    _ => string.Empty
                };

                Console.WriteLine($"{prefix} {insert}{message}");
            }
            else if (printTrace)
            {
                Console.WriteLine(prefix);
            }

            depth++;
        }
    }

    /// <summary>
    /// Logs the message without tracing where the method was called from.
    /// </summary>
    public static void Me(string? message, bool enabled = true, bool printTrace = false)
    {
        if (!enabled) return;
        string safeMsg = message ?? string.Empty;
        Message(safeMsg, Mode.Message, 2, printTrace);
    }

    /// <summary>
    /// Logs the message without tracing where the method was called from.
    /// </summary>
    public static void Me(Func<string> messageFactory, bool enabled = true, bool printTrace = false)
    {
        if (!enabled) return;
        string msg = messageFactory();
        Message(msg, Mode.Message, 2, printTrace);
    }

    /// <summary>
    /// Logs the warning message and traces where the method was called from.
    /// </summary>
    public static void Warn(string? message, bool enabled = true, bool printTrace = false)
    {
        if (!enabled) return;
        string safeMsg = message ?? string.Empty;
        Message(safeMsg, Mode.Warning, 2, printTrace);
    }

    /// <summary>
    /// Logs the warning message and traces where the method was called from.
    /// </summary>
    public static void Warn(Func<string> messageFactory, bool enabled = true, bool printTrace = false)
    {
        if (!enabled) return;
        string msg = messageFactory();
        Message(msg, Mode.Warning, 2, printTrace);
    }

    /// <summary>
    /// Logs the error message and traces where the method was called from.
    /// </summary>
    public static void Err(string? message, bool enabled = true, bool printTrace = false)
    {
        if (!enabled) return;
        string safeMsg = message ?? string.Empty;
        Message(safeMsg, Mode.Error, 2, printTrace);
    }

    /// <summary>
    /// Logs the error message and traces where the method was called from.
    /// </summary>
    public static void Err(Func<string> messageFactory, bool enabled = true, bool printTrace = false)
    {
        if (!enabled) return;
        string msg = messageFactory();
        Message(msg, Mode.Error, 2, printTrace);
    }

    /// <summary>
    /// Filters out frames from system and third-party namespaces.
    /// </summary>
    private static StackFrame[] FilterFrames(StackFrame[] frames)
    {
        IEnumerable<StackFrame> filtered = frames.Where(f => 
        {
            MethodBase? method = f.GetMethod();
            Type? type = method?.DeclaringType;
            if (type == null) return false;

            string? ns = type.Namespace;
            if (string.IsNullOrEmpty(ns)) return true;

            bool isSystem = ns.StartsWith("System");
            bool isMicrosoft = ns.StartsWith("Microsoft");
            bool isLogger = type.Name == "Log";
            
            return !isSystem && !isMicrosoft && !isLogger;
        });
        
        return filtered.Reverse().ToArray();
    }

    #endregion
}