#This file generates an iplist with the specified number of IPs
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

$number = 10000;

$file = $scriptPath + "\iplist.txt";

$content = "";

$a = 128;
$b = 0;
$c = 0;
$d = 0;

for($i = 0; $i -le $number; $i++)
{
    $ip = "{0}.{1}.{2}.{3}" -f $a, $b, $c, $d; 

    $d = $d + 1;

    if($d -gt 255)
    {
        $d = 0;
        $c = $c + 1;
    }

    if($c -gt 255)
    {
        $c = 0;
        $b = $b + 1;
    }

    if($b -gt 255)
    {
        $b = 0;
        $a = $a + 1;
    }

    $content += "{0}`r`n" -f $ip;

    $percentage = ($i / $number) * 100;

    Write-Progress -Activity "Generating IPs" -Status "Percent generated:" -PercentComplete $percentage;
}
Write-Host "Creating file";

$content | Out-File $file -Force;

Write-Host "Done";