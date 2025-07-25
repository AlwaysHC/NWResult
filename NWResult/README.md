Your function's result type will contain the "ok" and the "error" types and, thanks to the generic type inference, the C# compiler will do the work for you.

Usage:
```csharp
    private static Result<string, int> FuncOK()
        => "OK";

    private static Result<string, int> FuncErr()
        => -1;

    private Result<string, int> FuncEx()
        => throw new Exception("Exception");
```

The library permits a functional or a procedural usage of the results.


- Functional without Exception management:

```csharp
string R = FuncOK().OnlyIfValue(v => v + v, "Default"); // "OKOK"
string R = FuncErr().OnlyIfValue(v => v + v, "Default")); // "Default"
string R = FuncEx().OnlyIfValue(v => v + v, "Default")); // Exception

string R = FuncOK().IfValue(v => v + v).ElseError(e => e.ToString())); // "OKOK"
string R = FuncErr().IfValue(v => v + v).ElseError(e => e.ToString())); // "-1"
string R = FuncEx().IfValue(v => v + v).ElseError(e => e.ToString())); // Exception
```

- Functional with Exception management:
- 
```csharp
string R = Result.WithException(FuncOK).Exception(ex => ex.Message).OnlyIfValue(v => v + v, "Default"); // "OKOK"
string R = Result.WithException(FuncErr).Exception(ex => ex.Message).OnlyIfValue(v => v + v, "Default")); // "Default"
string R = Result.WithException(FuncEx).Exception(ex => ex.Message).OnlyIfValue(v => v + v, "Default")); // "Exception"

string R = Result.WithException(FuncOK).Exception(ex => ex.Message).IfValue(v => v + v).ElseError(e => e.ToString())); // "OKOK"
string R = Result.WithException(FuncErr).Exception(ex => ex.Message).IfValue(v => v + v).ElseError(e => e.ToString())); // "-1"
string R = Result.WithException(FuncEx).Exception(ex => ex.Message).IfValue(v => v + v).ElseError(e => e.ToString())); // "Exception"
```

- Procedural without Exception management:
```csharp
string Test = "";

FuncOK().OnlyIfValue(v => Test = v + v, "Default");
FuncErr().OnlyIfValue(v => Test = v + v, "Default");
FuncEx().OnlyIfValue(v => Test = v + v, "Default");

FuncOK().IfValue(v => Test = v + v).ElseError(e => Test = e.ToString();
FuncErr().IfValue(v => Test = v + v).ElseError(e => Test = e.ToString();
FuncEx().IfValue(v => Test = v + v).ElseError(e => Test = e.ToString());
```

- Procedural with Exception management:
```csharp
string Test = "";

Result.WithException(FuncOK).Exception(ex => Test = ex.Message).OnlyIfValue(v => Test = v + v);
Result.WithException(FuncErr).Exception(ex => Test = ex.Message).OnlyIfValue(v => Test = v + v);
Result.WithException(FuncEx).Exception(ex => Test = ex.Message).OnlyIfValue(v => Test = v + v);

Result.WithException(FuncOK).Exception(ex => Test = ex.Message).IfValue(v => Test = v + v).ElseError(e => Test = e.ToString());
Result.WithException(FuncErr).Exception(ex => Test = ex.Message).IfValue(v => Test = v + v).ElseError(e => Test = e.ToString());
Result.WithException(FuncEx).Exception(ex => Test = ex.Message).IfValue(v => Test = v + v).ElseError(e => Test = e.ToString());
```

- "Only exception" management:
```csharp
string Test = "";

Exception? ex = Result.OnlyException(FuncOK); // null
Exception? ex = Result.OnlyException(FuncErr); // null
Exception? ex = Result.OnlyException(FuncEx); // Exception

Result.WithException(FuncOK).OnlyException(ex => ex.Message, "Default"); // "Default"
Result.WithException(FuncErr).OnlyException(ex => ex.Message, "Default"); // "Default"
Result.WithException(FuncEx).OnlyException(ex => ex.Message, "Default"); // "Exception"

Result.OnlyException(FuncOK, ex => Test = ex.Message);
Result.OnlyException(FuncErr, ex => Test = ex.Message);
Result.OnlyException(FuncEx, ex => Test = ex.Message);

Result.WithException(FuncOK).OnlyException(ex => Test = ex.Message);
Result.WithException(FuncErr).OnlyException(ex => Test = ex.Message);
Result.WithException(FuncEx).OnlyException(ex => Test = ex.Message);
```
