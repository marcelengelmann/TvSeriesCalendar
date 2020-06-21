Set WshShell = CreateObject("WScript.Shell")
Dim objWMIService, colProcessList
Set objWMIService = GetObject("winmgmts:" & "{impersonationLevel=impersonate}!\\" & "." & "\root\cimv2")
if WScript.Arguments.Count <> 2 then
    WScript.Echo "Something Went Wrong"
	WScript.Quit 1
end if
Set colProcessList = objWMIService.ExecQuery("SELECT * FROM Win32_Process WHERE ProcessId = '" & WScript.Arguments(0) & "'")
Do While colProcessList.Count = 1
	WScript.Sleep 100
	Set colProcessList = objWMIService.ExecQuery("SELECT * FROM Win32_Process WHERE ProcessId = '" & WScript.Arguments(0) & "'")
Loop

Set objFSO = CreateObject("Scripting.FileSystemObject")
Set objFolder = objFSO.GetFolder(WScript.Arguments(1) & "update").Files

For Each objFile In objFolder
    If objFile.Name <> "update.vbs" Then
		WScript.Echo objFile.Path
        objFSO.CopyFile objFile.Path, WScript.Arguments(1)
    End If
Next
WshShell.Run(chr(34) & WScript.Arguments(1) & "TvSeriesCalendar.exe" & chr(34))
