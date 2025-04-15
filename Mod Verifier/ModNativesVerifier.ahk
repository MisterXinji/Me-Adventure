#NoEnv
#SingleInstance Force
SetBatchLines, -1
SetWorkingDir, %A_ScriptDir%

; Monster Hunter Wilds configuration
targetGame := "Monster Hunter Wilds"
overlayWidth := 350  ; Slightly wider for better text display
refreshRate := 30    ; Updates per second (lower = better game performance)

; Create transparent overlay
Gui, Overlay: +LastFound +AlwaysOnTop +ToolWindow -Caption +E0x20
Gui, Overlay: Margin, 10, 5
Gui, Overlay: Color, 0x121212
WinSet, TransColor, 0x121212 210

; Modern-looking UI elements
Gui, Overlay: Font, c0xFFFFFF s10 q5, Segoe UI
Gui, Overlay: Add, Text, vStatusText x10 y5 w330, Initializing mod check...
Gui, Overlay: Add, Progress, vProgressBar x10 y25 w330 h6 c0x00FF80 Background0x333333, 0
Gui, Overlay: Add, Text, vFileText x10 y35 w330, 
Gui, Overlay: Add, Text, vStatsText x10 y50 w330, 

; Position in top-right with 10px offset from edge
SysGet, monitor, MonitorWorkArea
Gui, Overlay: Show, % "x" monitorRight-overlayWidth-10 " y10 w" overlayWidth " h70 NoActivate", MHWModOverlay

; Get all mod files
modFolders := ["Mod Natives Performance", "nativePC"]  ; Common MH mod folders
files := []
totalSize := 0

for i, folder in modFolders
{
    if InStr(FileExist(folder), "D")
    {
        Loop, Files, %folder%\*.*, R
        {
            files.Push({path: A_LoopFileFullPath, name: A_LoopFileName, size: A_LoopFileSize})
            totalSize += A_LoopFileSize
        }
    }
}

if (files.Length() = 0)
{
    ShowTemporaryMessage("No mod files found!", 2000)
    ExitApp
}

; Performance-optimized checking loop
checkStart := A_TickCount
processedSize := 0
lastUpdate := 0
cycleCount := 0

for index, file in files
{
    ; Throttle UI updates to maintain game FPS
    if (A_TickCount - lastUpdate > 1000/refreshRate)
    {
        percent := (index/files.Length())*100
        elapsed := (A_TickCount - checkStart)/1000
        speed := processedSize/elapsed
        remaining := (totalSize - processedSize)/(speed > 0 ? speed : 1)
        
        ; Update overlay
        GuiControl, Overlay:, ProgressBar, %percent%
        GuiControl, Overlay:, StatusText, % "Checking mods (" Round(percent) "%)"
        GuiControl, Overlay:, FileText, % ">" (StrLen(file.name) > 28 ? SubStr(file.name, 1, 25) "..." : file.name)
        GuiControl, Overlay:, StatsText, % FormatStats(processedSize, totalSize, elapsed, remaining)
        
        lastUpdate := A_TickCount
        cycleCount++
    }
    
    ; Actual validation (customize this)
    ValidateModFile(file.path)
    
    processedSize += file.size
    
    ; Keep overlay on top without stealing focus
    if (cycleCount >= refreshRate) && WinExist(targetGame)
    {
        WinSet, AlwaysOnTop, On, MHWModOverlay
        cycleCount := 0
    }
    
    Sleep, 1  ; Minimal delay for game responsiveness
}

; Completion
ShowTemporaryMessage("Checked " files.Length() " mod files", 3000)
ExitApp

; Functions
ValidateModFile(filePath) {
    ; Add your actual mod validation logic here
    ; Example: Check file signatures, hashes, or structure
    
    ; Simulate work (remove in production)
    critical
    Sleep, % 5 + Random(1, 15)
}

FormatStats(processed, total, elapsed, remaining) {
    return Format("{1}/{2} | {3}s | ETA: {4}s"
        , FormatBytes(processed)
        , FormatBytes(total)
        , Round(elapsed,1)
        , Round(remaining,1))
}

FormatBytes(bytes) {
    static units := ["B","KB","MB","GB"]
    loop {
        if (bytes < 1024 || A_Index = units.Length())
            return Round(bytes, (bytes < 10 ? 2 : 1)) " " units[A_Index]
        bytes /= 1024
    }
}

ShowTemporaryMessage(msg, duration) {
    GuiControl, Overlay:, StatusText, %msg%
    GuiControl, Overlay:, FileText, 
    GuiControl, Overlay:, StatsText, 
    Sleep, %duration%
}

Random(min, max) {
    Random, r, min, max
    return r
}

; Hotkeys
#IfWinExist MHWModOverlay
^!h::Gui, Overlay: Hide  ; Ctrl+Alt+H to hide
^!o::Gui, Overlay: Show  ; Ctrl+Alt+O to show
^!r::Reload              ; Ctrl+Alt+R to reload
#If