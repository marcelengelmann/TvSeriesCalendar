@echo off
set newVer=%1
set config=%2
set directory=%3
set /p currentVersion=<"F:\Programs\Workspaces\Visual Studio\Projects\TvSeriesCalendar\Releases\newestVersion.txt"
if %newVer% LEQ %currentVersion% (
 echo Increase Version!
 Exit /B 1
)
mkdir "F:\Programs\Workspaces\Visual Studio\Projects\TvSeriesCalendar-Public\TvSeriesCalendar\Releases\Updates\%newVer%"
tar -c -f "F:\Programs\Workspaces\Visual Studio\Projects\TvSeriesCalendar-Public\TvSeriesCalendar\Releases\Updates\%newVer%\%newVer%_%config%.zip" --directory="%directory%" *
REM echo %newVer%>"F:\Programs\Workspaces\Visual Studio\Projects\TvSeriesCalendar\Releases\newestVersion.txt"