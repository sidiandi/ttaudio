$productName = "ttaudio"
$msi = dir '.\out\Release-Any CPU\*.msi'

"Will test installation of $msi"

function uninstall($appName)
{
    "Looking for already installed $appName"

    $app = Get-WmiObject -Class Win32_Product -Filter "Name = 'ttaudio'" | Where-Object { $_.Name -match $appName }

    if ($app)
    {
        "Found already installed $appName"
        $app.Uninstall()
        "$appName was uninstalled"
    }
    else
    {
        "$appName is not installed."
    }
}

function install($file)
{
    $DataStamp = get-date -Format yyyyMMddTHHmmss
    $logFile = '{0}-{1}.log' -f $file.fullname,$DataStamp
    $MSIArguments = @(
        "/norestart"
        "/lv"
        """$logFile"""
        "/quiet"
        "/i"
        ('"{0}"' -f $file.fullname)
    )
    $MSIArguments
    Start-Process "msiexec.exe" -ArgumentList $MSIArguments -Wait -NoNewWindow 
    "$file was installed. Log in $logFile"
}

uninstall($productName)
install($msi)


