# Bloodstrap Icon Creation Instructions

## Design Specifications

### Icon Design:
- **Base**: Red Roblox logo (instead of white/gray)
- **Accent**: Blood drop overlay
- **Size**: 256x256 pixels (will be scaled down)
- **Format**: ICO file

### Color Palette:
- **Primary Red**: #CC0000 or #B30000 (dark blood red)
- **Blood Drop**: #8B0000 (darker red for contrast)
- **Highlight**: #FF3333 (lighter red for shine effect)

### Design Elements:
1. **Roblox Square Logo** - Make it red (#CC0000)
2. **Blood Drop** - Add a dripping blood drop from top-right corner
   - Position: Top-right, slightly overlapping the logo
   - Style: Glossy/shiny blood drop with highlight
   - Size: About 30% of logo height

### Tools You Can Use:

#### Option 1: Online (Easiest)
1. Go to https://www.photopea.com/ (free Photoshop alternative)
2. Create new 256x256 image
3. Import Roblox logo
4. Change color to red (#CC0000)
5. Draw blood drop shape with pen tool
6. Add gradient for shine effect
7. Export as PNG, then convert to ICO at https://convertio.co/png-ico/

#### Option 2: GIMP (Free Software)
1. Download GIMP from https://www.gimp.org/
2. Create 256x256 canvas
3. Import/draw red Roblox square
4. Use ellipse tool for blood drop
5. Add layer effects for shine
6. Export as ICO

#### Option 3: AI Generation
Use an AI image generator with this prompt:
```
"Red Roblox logo icon with a glossy blood drop dripping from the top right corner, 
dark red color scheme, gaming icon style, clean and modern, 256x256 pixels"
```

### File Locations to Replace:
After creating the icon, replace these files:
1. `Bloxstrap/Voidstrap.ico` - Main application icon
2. `Bloxstrap/Resources/IconVoidstrap.ico` - Resource icon
3. `Bloxstrap/Voidstrap.png` - PNG version

### Quick Color Reference:
```
Background: Transparent
Logo Red: #CC0000
Blood Drop: #8B0000
Highlight: #FF3333 (add white overlay at 30% opacity for shine)
Shadow: #660000 (for depth)
```

### Blood Drop Shape:
```
Top: Rounded (like water droplet top)
Middle: Slightly wider
Bottom: Pointed (teardrop shape)
Shine: Small white ellipse on upper-left of drop (20% size)
```

## After Creating Icon:

1. Save as `Bloodstrap.ico`
2. Copy to:
   - `Bloxstrap/Bloodstrap.ico`
   - `Bloxstrap/Resources/IconBloodstrap.ico`
3. Update `.csproj` file to reference new icon
4. Rebuild project

---

**Note**: The icon should be simple, recognizable, and work well at small sizes (16x16, 32x32, 48x48).
