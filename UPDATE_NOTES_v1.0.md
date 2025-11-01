# 🩸 Bloodstrap v1.0 - Update Notes

**Release Date:** November 1, 2025  
**Version:** 1.0.0  
**Build:** Release

---

## 🎉 What's New in This Update

### 🩸 **Complete Rebranding**
- **Application Name:** Voidstrap → **Bloodstrap**
- **New Icon:** Custom blood-themed icon with transparent background
- **All UI Elements:** Every window, dialog, and menu now displays "Bloodstrap"
- **Executable Name:** `Bloodstrap.exe`

### 🎮 **New Features**

#### Player Status & Inventory System
- **Player Status Dashboard** - View your Roblox account info at a glance
- **Inventory Viewer** - Browse your Roblox inventory with category filters
  - Hats, Gear, Faces, Clothing, Accessories
  - Real-time inventory loading
- **Badges Viewer** - Display all earned badges in a grid layout
- **Gamepasses Console** - View and manage owned gamepasses
- **Quick Actions** - Direct links to Profile, Friends, Groups, Transactions

#### Web Login Integration
- **Login via Web** - Secure web-based Roblox authentication
- **Cookie Management** - Automatic `.ROBLOSECURITY` cookie detection
- **Persistent Sessions** - Stay logged in across launches
- **Cookie Storage:** `%LocalAppData%\Roblox\LocalStorage\RobloxCookies.dat`

### 🛠️ **Technical Improvements**

#### Centralized Configuration
- **ProjectInfo.cs** - Single source of truth for project name and branding
- **Easy Rebranding** - Change 3 values to rebrand entire application
- **Automated Scripts:**
  - `complete-auto-rebrand.bat` - Full rebrand automation
  - `reset-bloodstrap-data.bat` - Complete data reset tool
  - `fix-strings.ps1` - Resource string updater

#### Code Quality
- Updated all resource strings (100+ instances)
- Fixed icon references across all XAML files
- Improved project structure with centralized constants
- Better maintainability for future updates

---

## 🔧 **Bug Fixes**

### Icon Issues
- ✅ Fixed icon loading errors
- ✅ Resolved "Cannot locate resource 'voidstrap.ico'" errors
- ✅ Updated all icon references to use new Bloodstrap icon
- ✅ Removed background from custom blood icon

### UI/UX Fixes
- ✅ Fixed window titles showing old branding
- ✅ Updated navigation menu items
- ✅ Fixed snackbar notifications
- ✅ Corrected all dialog titles

### Build Issues
- ✅ Resolved compilation errors from rebranding
- ✅ Fixed resource reference issues
- ✅ Updated project configuration files

---

## 📝 **Changed Files**

### Core Files
- `Bloxstrap/ProjectInfo.cs` - **NEW** - Centralized project configuration
- `Bloxstrap/App.xaml.cs` - Updated to use ProjectInfo
- `Bloxstrap/Voidstrap.csproj` - Updated assembly name and icon references

### UI Files (Updated)
- `UI/Elements/Settings/MainWindow.xaml` - Navigation and titles
- `UI/Elements/Dialogs/LaunchMenuDialog.xaml` - Launch menu title
- `UI/Elements/Dialogs/UninstallerDialog.xaml` - Uninstaller title
- `UI/Elements/Installer/MainWindow.xaml` - Installer title
- **+50 other XAML files** - All titles and text updated

### Resources
- `Resources/Strings.resx` - **100+ strings updated**
- `Resources/IconBloodstrap.ico` - New custom icon
- `Bloodstrap.ico` - Application icon

### Scripts (NEW)
- `complete-auto-rebrand.bat` - Automated rebrand tool
- `reset-bloodstrap-data.bat` - Data reset utility
- `fix-strings.ps1` - Resource string updater
- `project-config.bat` - Configuration variables
- `final-rebrand.bat` - Quick rebrand script

---

## 🚀 **Installation & Upgrade**

### Fresh Install
1. Download `Bloodstrap.exe`
2. Run the installer
3. Follow on-screen instructions
4. Configure your preferences

### Upgrading from Voidstrap
1. Run `reset-bloodstrap-data.bat` to clean old data (optional)
2. Install Bloodstrap v1.0
3. Your settings will be preserved if you didn't reset

---

## ⚠️ **Breaking Changes**

### Registry Keys
- Old: `HKCU\Software\Voidstrap`
- New: `HKCU\Software\Bloodstrap`

### AppData Folder
- Old: `%LocalAppData%\Voidstrap`
- New: `%LocalAppData%\Bloodstrap`

### Executable Name
- Old: `Voidstrap.exe`
- New: `Bloodstrap.exe`

**Note:** The reset script will clean up old Voidstrap data automatically.

---

## 📊 **Statistics**

- **Files Modified:** 150+
- **Lines Changed:** 500+
- **Strings Updated:** 100+
- **XAML Files Updated:** 50+
- **New Scripts Created:** 5
- **Build Time:** ~30 seconds
- **Final Size:** ~200MB (single-file executable)

---

## 🎯 **Known Issues**

None at this time. Please report any issues on GitHub.

---

## 🔜 **Coming Soon**

- Enhanced music player features
- More customization options
- Additional gameplay tools
- Performance improvements
- Bug fixes and optimizations

---

## 📞 **Support**

- **GitHub Issues:** https://github.com/ethantheDeveloper220/vibetrap/issues
- **Discord:** https://discord.gg/bsR6EbZmvX
- **Wiki:** https://github.com/ethantheDeveloper220/vibetrap/wiki

---

## 🙏 **Credits**

- **Original Bloxstrap** - Foundation and inspiration
- **Community** - Bug reports and feedback
- **You** - For using Bloodstrap!

---

**Enjoy Bloodstrap v1.0!** 🩸🎮

*Built with ❤️ for the Roblox community*
