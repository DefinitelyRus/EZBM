# Update Documentation

## Update `Changelog.md`

- Read through the Git changes and create new changelog entries.
- Follow the instruction provided in the file itself.
- Create an entry for each of the days where changes were made since the latest entry.

## Update in-code documentation for C# files

Add/update in-code documentation to this file following the existing documentation style (including spacing and line breaks) from other source code files of the same type. Keep it short and use plain English. Use/add a "Documented by" field and add "Google Gemini" to it. Add usage examples if it's not already intuitive or straight-forward. Do this for the class itself and all its members.

- Add or update in-code documentation to all `.cs` files following the documentation style and spacing examples below.
- Use other existing in-code documentation as reference as the examples are just guidelines, not rules.
- Keep the descriptions short and use plain English.
- Add "Author(s)", "Editor(s)", and "Documented by" fields.
  - Add "DefinitelyRus" to the authors.
  - Add "Google Gemini" to the "documented by".

Example:

```csharp
namespace SomeNamespace;

/// <summary>
/// Manages file handling, including reads, writes, creation, deletion, etc.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editor(s): None<br/>
/// Documented by: Google Gemini</i>
/// </summary>
public class FileManager
{
    /// <summary>
    /// The path to the user's documents folder.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    public string UserDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    /// <summary>
    /// The default file name to use in place of a missing or invalid file name.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string DefaultFileName = "save_file.json";


    /// <summary>
    /// Writes string content to a specified file in the user's documents folder.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="filename">The filename including the file extension.</param>
    /// <param name="content">The text content to write to the file.</param>
    /// <param name="overwriteExisting">Whether to overwrite the file if it already exists.</param>
    public void WriteFile(string? filename, string? content, bool overwriteExisting = false)
    {
        // ...
    }


    /// <summary>
    /// Reads the string content from a specified file in the user's documents folder.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="filename">The filename including the file extension.</param>
    public void WriteFile(string? filename)
    {
        // ...
    }
}
```
