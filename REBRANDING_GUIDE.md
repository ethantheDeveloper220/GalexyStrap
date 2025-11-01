# 🎨 Rebranding Guide

This guide explains how to rebrand the application to a different name.

## Quick Rebrand (3 Steps)

### 1. Update Project Configuration

Edit `ProjectInfo.cs` and change these 3 values:

```csharp
public const string PROJECT_NAME = "YourAppName";      // Change this
public const string PROJECT_OWNER = "YourGitHubName";  // Change this
public const string PROJECT_REPO = "your-repo-name";   // Change this
```

### 2. Update Batch Scripts

Edit `project-config.bat`:

```batch
SET PROJECT_NAME=YourAppName
SET OLD_PROJECT_NAME=Bloodstrap
```

### 3. Run Rebrand Script

```batch
final-rebrand.bat
```

This will:
- Update all XAML window titles
- Update all visible text
- Rebuild the application
- Create `YourAppName.exe`

---

## What Gets Updated Automatically

When you change `PROJECT_NAME` in `ProjectInfo.cs`, these are automatically updated:

### Application Level
- ✅ Window titles (via code-behind)
- ✅ Registry keys (`HKCU\Software\YourAppName`)
- ✅ Uninstall registry entry
- ✅ AppData folder path (`%LocalAppData%\YourAppName`)
- ✅ Download/Help/Support links

### XAML Files
The `final-rebrand.bat` script updates:
- ✅ All `Title="..."` attributes
- ✅ All `Text="..."` attributes
- ✅ Installer window titles

### Build Output
- ✅ Executable name: `YourAppName.exe`
- ✅ DLL name: `YourAppName.dll`

---

## Manual Updates Required

Some things need manual updates:

### 1. Icon File
Replace `Bloxstrap\Bloodstrap.ico` with your custom icon

### 2. Resource Strings
Edit `Bloxstrap\Resources\Strings.resx` to find/replace any remaining brand references

### 3. Project File
Update `Voidstrap.csproj` if you want to rename the project itself:
```xml
<AssemblyName>YourAppName</AssemblyName>
```

---

## File Structure

```
📁 Project Root
├── 📄 ProjectInfo.cs          ← Main branding config (C#)
├── 📄 project-config.bat      ← Batch script config
├── 📄 final-rebrand.bat       ← Rebrand + build script
├── 📄 reset-bloodstrap-data.bat ← Data reset tool
└── 📁 Bloxstrap
    ├── 📄 App.xaml.cs         ← Uses ProjectInfo
    ├── 📄 Bloodstrap.ico      ← Replace with your icon
    └── 📁 Resources
        └── 📄 Strings.resx    ← Resource strings
```

---

## Testing Your Rebrand

After rebranding:

1. ✅ Run the application
2. ✅ Check all window titles
3. ✅ Verify registry keys: `regedit` → `HKCU\Software\YourAppName`
4. ✅ Check AppData folder: `%LocalAppData%\YourAppName`
5. ✅ Test uninstaller
6. ✅ Verify About page shows correct name

---

## Reverting Changes

To revert to Bloodstrap:

1. Edit `ProjectInfo.cs`:
   ```csharp
   public const string PROJECT_NAME = "Bloodstrap";
   ```

2. Run `final-rebrand.bat`

3. Done!

---

## Advanced: Full Project Rename

To rename the entire Visual Studio project:

1. Rename `Voidstrap.csproj` to `YourAppName.csproj`
2. Update namespace in all `.cs` files from `Voidstrap` to `YourAppName`
3. Update `xmlns` references in all `.xaml` files
4. Update solution file references

**Note:** This is complex and not recommended unless necessary.

---

## Support

If you encounter issues:
- Check that `ProjectInfo.cs` values don't contain special characters
- Ensure `project-config.bat` matches `ProjectInfo.cs`
- Run `reset-bloodstrap-data.bat` to clean old data
- Rebuild from scratch: `dotnet clean` then `final-rebrand.bat`

---

**Happy Rebranding!** 🎨
