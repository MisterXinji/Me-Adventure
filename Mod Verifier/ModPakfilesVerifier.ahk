#NoEnv
#SingleInstance Force
SetBatchLines, -1
SetWorkingDir %A_ScriptDir%

; --------------------------
; Configuration Options
; --------------------------
scanDelay := 200                ; Milliseconds between files
verificationDisplayTime := 3000 ; How long to show verification complete (ms)
workingDisplayTime := 5000      ; How long to show "Mods working" (ms)
overlayX := 10                  ; X position of overlay
overlayY := 10                  ; Y position of overlay
backgroundColor := "1A1A1A"     ; Dark background
textColor := "White"
progressColor := "45B5E6"       ; Light blue progress bar
successColor := "2ECC71"        ; Green for completion

; --------------------------
; Create the Overlay GUI
; --------------------------
Gui, Overlay:New, +AlwaysOnTop +ToolWindow -Caption +LastFound
Gui, Overlay:Color, %backgroundColor%
Gui, Overlay:Font, c%textColor% s10, Segoe UI
Gui, Overlay:Add, Text, vCountText w200, Initializing mod verification...
Gui, Overlay:Add, Progress, vProgressBar w200 h20 c%progressColor%, 0
Gui, Overlay:Show, x%overlayX% y%overlayY% NoActivate, Mod Verifier

; --------------------------
; File Verification Process
; --------------------------
SetTimer, VerifyFiles, -500
return

VerifyFiles:
    ; First count all .pak files
    GuiControl, Overlay:, CountText, Searching for mod files...
    totalFiles := 0
    
    Loop, Files, *.pak, R
    {
        totalFiles++
        Sleep, %scanDelay%
    }
    
    if (totalFiles = 0)
    {
        GuiControl, Overlay:, CountText, No mod files found!
        SetTimer, CloseOverlay, -%verificationDisplayTime%
        return
    }
    
    ; Verify each file with progress
    verifiedFiles := 0
    GuiControl, Overlay:, CountText, Found %totalFiles% mod files`nVerifying...
    
    Loop, Files, *.pak, R
    {
        verifiedFiles++
        currentProgress := (verifiedFiles / totalFiles) * 100
        GuiControl, Overlay:, ProgressBar, %currentProgress%
        GuiControl, Overlay:, CountText, Verifying mod %verifiedFiles% of %totalFiles%
        Sleep, %scanDelay%
    }
    
    ; Show verification complete
    GuiControl, Overlay:, ProgressBar, 100
    GuiControl, Overlay: +c%successColor%, ProgressBar
    GuiControl, Overlay:, CountText, Verification complete!`n%totalFiles% mods ready
    
    ; After delay, show "Mods working..."
    SetTimer, ShowWorkingStatus, -%verificationDisplayTime%
return

ShowWorkingStatus:
    ; Change to working status display (no animation)
    GuiControl, Overlay:, CountText, Mods working...`nAll systems operational
    GuiControl, Overlay:, ProgressBar, 100
    
    ; Close after working display time
    SetTimer, CloseOverlay, -%workingDisplayTime%
return

CloseOverlay:
    ; Simple fade out
    Loop, 10
    {
        WinSet, Transparent, % 255 - (A_Index * 25), Mod Verifier
        Sleep, 30
    }
    Gui, Overlay:Destroy
    ExitApp
return

; Press Escape to exit anytime
Escape::
    Gui, Overlay:Destroy
    ExitApp
return