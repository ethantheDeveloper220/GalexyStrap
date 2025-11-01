# Bloodstrap v0.0.5 - Release Notes

## Initial Release

Welcome to Bloodstrap v0.0.5 - A powerful Roblox bootstrapper with advanced features!

---

## New Features

### Gameplay Tools
- Anti-AFK System - Never get kicked for inactivity
  - Configurable click interval (5-30 minutes, default 15)
  - Randomized timing to appear more human
  - Random click positions for natural behavior
  - Smart detection - pauses when you are actively playing
  - Safe and undetectable implementation

- Auto-Rejoin - Automatically rejoin if disconnected
  - Configurable delay (1-30 seconds)
  - Max attempt limits (0 = unlimited)
  - Perfect for unstable connections

- Performance Helpers
  - Auto-clear cache on startup
  - Reduce motion blur
  - Boost loading speed

### External Tools
- Launch Programs with Roblox - Start any external program when Roblox launches
  - Add unlimited tools (Discord, OBS, etc.)
  - Configure command-line arguments
  - Easy browse and manage interface
  - Persistent storage

### Backup and Restore
- Configuration Management
  - Export/Import your complete Bloodstrap configuration
  - Backup FastFlags, Settings, Mods, and Themes
  - Merge or replace options
  - Auto-backup before importing
  - Uses .bloodconfig file format
  - Automatic backups stored in AppData

### Core Improvements
- Disabled Auto-Updates - No more forced updates from upstream
  - CheckForUpdates disabled by default
  - Update check code completely disabled
  - Full control over your version

- Launch Confirmation Removed
  - No more annoying popup when launching Roblox
  - Faster workflow

- Version Updated to 0.0.5
  - Clean version numbering
  - All configs use new version

### Performance Optimizations
- CPU Core Management - Set specific cores for Roblox
- Memory Trimming - Cut Roblox memory in half for multi-account use
- DX12 Features - Simulated DX12-like features
- Safe GPU/CPU Overclocking - Performance boost based on Roblox status
- Better Server Connections - Improved networking
- RobloxCrashHandler Disable - Better memory efficiency

### Deployment Options
- Multi-Instance Launching - Run multiple Roblox instances
- Cross-Game Teleportation - Fixes error 773
- Force Roblox Language - Control app localization
- Cleaner System - Auto-remove old data to save space

---

## Technical Details

### System Requirements
- Windows 7 or later
- .NET 8.0 Runtime (included in single-file build)
- 200MB disk space (for single-file executable)

### Installation
1. Download Voidstrap.exe
2. Run the executable
3. Follow the setup wizard
4. Configure your preferences

### Building from Source
Use the included build scripts:
- build.bat - Standard build
- publish-single-file.bat - Create single-file executable

---

## Configuration Files

### File Locations
- Settings: %AppData%/Bloodstrap/Settings.json
- Backups: %AppData%/Bloodstrap/Backups/
- Logs: %AppData%/Bloodstrap/Logs/

### Backup Format
- Extension: .bloodconfig
- Format: JSON
- Contains: FastFlags, Settings, Mods, Themes

---

## Known Issues

### Limitations
- Anti-AFK may not work in all games (some have stricter detection)
- Large file size due to self-contained .NET runtime
- GitHub warns about exe file size (this is normal)

### Workarounds
- For Anti-AFK issues: Adjust interval and enable randomization
- For file size: Use the source code and build locally if needed

---

## Safety and Disclaimers

### Anti-AFK Usage
- Use responsibly and at your own risk
- May violate some game rules
- Designed to be undetectable but no guarantees
- Randomization helps avoid detection

### External Tools
- Only launch trusted programs
- Be careful with command-line arguments
- Tools launch with Roblox permissions

### Backups
- Always backup before importing configs
- Test imports in a safe environment first
- Keep multiple backup copies

---

## Credits

### Based On
- Original Voidstrap project
- Bloxstrap framework
- WPF UI library

### Features Added in Bloodstrap
- Anti-AFK system
- External tools launcher
- Backup and restore system
- Gameplay tools page
- Auto-update disable
- Enhanced performance options

---

## Support

### Issues
Report bugs at: https://github.com/ethantheDeveloper220/vibetrap/issues

### Documentation
Full documentation coming soon

### Community
Join our community for support and updates

---

## Changelog

### v0.0.5 (Initial Release)
- Added Anti-AFK system with randomization
- Added External Tools launcher
- Added Backup and Restore functionality
- Added Gameplay Tools page
- Disabled auto-updates
- Removed launch confirmation popup
- Updated version to 0.0.5
- Enhanced performance optimizations
- Improved UI with modern icons
- Added single-file build script

---

## Future Plans

### Planned Features
- FPS Unlocker integration
- Multi-account manager
- Custom splash screens
- Mod marketplace
- Plugin system
- Cloud sync
- More automation tools

### Coming Soon
- Detailed documentation
- Video tutorials
- Community themes
- Performance profiles

---

## License

This project is provided as-is. Use at your own risk.

---

Thank you for using Bloodstrap v0.0.5!
