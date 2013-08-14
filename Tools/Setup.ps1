function Test-Administrator  
{  
    $user = [Security.Principal.WindowsIdentity]::GetCurrent();
    (New-Object Security.Principal.WindowsPrincipal $user).IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)  
}

$dir = Get-Location
$cwsFiles = (
    "CubeWorldMITM.exe",
    "Ionic.Zip.dll",
    "CWSProtocol.dll",
    "ServerService.dll",
    "System.Data.SQLite",
    "SQLite.Interop.dll",
    "CWSRestart.exe",
    "Interop.NATUPNPLib.dll",
    "CWSWeb.exe",
    "Nancy.Authentication.Forms.dll",
    "Nancy.dll",
    "Nancy.Hosting.Self.dll",
    "Nancy.ViewEngines.Razor",
    "System.Web.Razor.Unofficial.dll",
    "Utilities.dll")
$cwsWebName = "CWSWeb.exe"

"Current directory: " + $dir

$selection = "";

while("y", "n" -notcontains $selection.ToLowerInvariant())
{
    $selection = Read-Host "Would you like to unblock our files in the current directory (y/n)?"
}

if($selection.ToLowerInvariant() -eq "y")
{
    "Unblocking files"
    foreach($f in gci $dir -Recurse -File | ForEach-Object -Process {$_.FullName})
    {
        foreach($n in $cwsFiles)
        {
            if($f.Contains($n))
            {
                "Unblocking file " + $f
                Unblock-File $f
            }
        }
    }
}

if(Test-Administrator)
{
    $selection = "";

    while("y", "n" -notcontains $selection.ToLowerInvariant())
    {
        $selection = Read-Host "Would you like to add a Windows Firewall Rule for Port 8181 (y/n)?"
    }

    if($selection.ToLowerInvariant() -eq "y")
    {
        if(Test-Path($dir.FullName + $cwsWebName))
        {
            $programPath = $dir.FullName + $cwsWebName

            New-NetFirewallRule -DisplayName "CWSWeb 8181 Inbound TCP" -Program $programPath -Direction Inbound -RemotePort 8181 -LocalPort 8181 -Profile Any -Action Allow -Protocol TCP
            New-NetFirewallRule -DisplayName "CWSWeb 8181 Outbound TCP" -Program $programPath -Direction Outbound -RemotePort 8181 -LocalPort 8181 -Profile Any -Action Allow -Protocol TCP
            New-NetFirewallRule -DisplayName "CWSWeb 8181 Inbound UDP" -Program $programPath -Direction Inbound -RemotePort 8181 -LocalPort 8181 -Profile Any -Action Allow -Protocol UDP
            New-NetFirewallRule -DisplayName "CWSWeb 8181 Outbound UDP" -Program $programPath -Direction Outbound -RemotePort 8181 -LocalPort 8181 -Profile Any -Action Allow -Protocol UDP
        }
        else
        {
            Write-Warning "CWSWeb was not found in the current directory"
        }
    }
}
else
{
    "Run this script as administrator, to add Firewall rules"
}