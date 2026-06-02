# Update Documentation

## Update `Task List.md` and `Feature Checklist.md`

- Whenever significant progress is made or a feature is completed, review `Task List.md` and `Feature Checklist.md`.
- Mark completed items with `[x]` in `Feature Checklist.md`.
- Update the `Status` column in `Task List.md` from `Todo` to `Done` (or `In Progress` if applicable).
- Ensure that the documented progress aligns with the actual state of the codebase.

## Update `Changelog.md`

- Read through the Git changes and create new changelog entries.
- Follow the instruction provided in the file itself.
- Create an entry for each of the days where changes were made since the latest entry.

## Update in-code documentation for C# files

- Add or update in-code documentation to all `.cs` files following the documentation style example below.
- Follow the spacing and line breaks from the example below. (Not rules, just examples)
- Keep descriptions short and use plain English.
- Add "Author(s)", "Editor(s)", and "Documented by" fields.
  - Add "DefinitelyRus" to the "Author(s)" field.
  - Add your name to the "Documented by" field. (e.g., "Google Gemini", "OpenAI ChatGPT", etc.)

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
