if (!(Test-Path "./Bonsai.exe")) {
    & dotnet build ../src/Aeon.sln --configuration Release
    Invoke-WebRequest "https://github.com/bonsai-rx/bonsai/releases/download/2.7-rc1/Bonsai.zip" -OutFile "temp.zip"
    Move-Item -Path "NuGet.config" "temp.config"
    Expand-Archive "temp.zip" -DestinationPath "." -Force
    Move-Item -Path "temp.config" "NuGet.config" -Force
    Remove-Item -Path "temp.zip"
    Remove-Item -Path "Bonsai32.exe"
}
& .\Bonsai.exe --no-editor