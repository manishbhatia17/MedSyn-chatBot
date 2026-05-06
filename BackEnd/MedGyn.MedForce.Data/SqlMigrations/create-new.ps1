
function New-Migration-Script {
    param(
        [string] $name = "./"
    )

    $filename = "$(Get-Script-Filename $name)"

    if(Test-Path $filename) {
        Write-Host "Opened existing schema file $($filename)."
    }
    else {
        New-Item $filename -type file

        Add-Content $filename "`n`n"
        Add-Content $filename "/* ======= This is $($filename) === */"
        Add-Content $filename "/* ======= Your $($path) SQL code goes here =================================*/"
        Add-Content $filename "/* ==========================================================================*/"
        Add-Content $filename "`n`n`n`n`n"
        Add-Content $filename "/* ==========================================================================*/"
        Add-Content $filename "/* ======== Your SQL code was above here ====================================*/"
        Add-Content $filename "`n`n"

        Write-Host "Created schema file $($filename)."
    }
    return $filename
}

function Get-Script-Filename {
    param(
        [string] $name
    )

    $filename =  $name.replace(" ", "-")
    $id = Get-Date -UFormat "%Y%m%d%H%M"

    return "$($id)_$($filename).sql"
}


$createdFiles = @()
$name = Read-Host -Prompt 'What goal are you accomplishing with this script (will be used as the script name)?'
Write-Host "Using file $(Get-Script-Filename $name)."

$createdFiles += New-Migration-Script $name

ForEach($file in $createdFiles) {
    Invoke-Item $file
}

