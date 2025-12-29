
param(
    [string]$path
)

Add-Type -AssemblyName System.Drawing

$img = [System.Drawing.Bitmap]::FromFile($path)
$bmp = New-Object System.Drawing.Bitmap($img.Width, $img.Height)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.DrawImage($img, 0, 0, $img.Width, $img.Height)

$targetColor = $bmp.GetPixel(0, 0)
# Tolerance for "white-ish" background
$isTarget = { param($c) 
    return ($c.R -gt 240 -and $c.G -gt 240 -and $c.B -gt 240)
}

if (& $isTarget $targetColor) {
    $queue = new-object System.Collections.Generic.Queue[System.Drawing.Point]
    $queue.Enqueue((New-Object System.Drawing.Point(0, 0)))
    
    # Track visited to avoid infinite loops, though changing color helps.
    # For simplicity and performance on small images, we just check pixel color.
    # But if we change to transparent, we can just check if it is NOT transparent.
    
    while ($queue.Count -gt 0) {
        $pt = $queue.Dequeue()
        
        if ($pt.X -lt 0 -or $pt.X -ge $bmp.Width -or $pt.Y -lt 0 -or $pt.Y -ge $bmp.Height) { continue }
        
        $currentPixel = $bmp.GetPixel($pt.X, $pt.Y)
        
        # If it's transparent already, skip (visited)
        if ($currentPixel.A -eq 0) { continue }
        
        # If it matches background color
        if (& $isTarget $currentPixel) {
            $bmp.SetPixel($pt.X, $pt.Y, [System.Drawing.Color]::Transparent)
            
            $nx = [int]$pt.X
            $ny = [int]$pt.Y
            $queue.Enqueue((New-Object System.Drawing.Point($nx + 1, $ny)))
            $queue.Enqueue((New-Object System.Drawing.Point($nx - 1, $ny)))
            $queue.Enqueue((New-Object System.Drawing.Point($nx, $ny + 1)))
            $queue.Enqueue((New-Object System.Drawing.Point($nx, $ny - 1)))
        }
    }
}

$img.Dispose()
$g.Dispose()

$bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()

Write-Host "Processed $path"
