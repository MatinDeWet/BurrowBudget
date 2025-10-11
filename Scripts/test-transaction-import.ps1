# Get File Information for Transaction Import
# Usage: .\test-transaction-import.ps1 -FilePath "C:\path\to\your\file.csv"

param(
    [Parameter(Mandatory=$true)]
    [string]$FilePath
)

# Verify file exists
if (-not (Test-Path $FilePath)) {
    Write-Error "File not found: $FilePath"
    exit 1
}

# Get file information
$file = Get-Item $FilePath
$fileName = $file.Name
$fileSize = $file.Length

# Determine content type based on extension
$contentTypes = @{
    ".csv"  = "text/csv"
    ".txt"  = "text/plain"
    ".json" = "application/json"
    ".xml"  = "application/xml"
    ".xlsx" = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    ".xls"  = "application/vnd.ms-excel"
    ".ofx"  = "application/x-ofx"
    ".qfx"  = "application/x-qfx"
}

$extension = $file.Extension.ToLower()
$contentType = $contentTypes[$extension]
if (-not $contentType) {
    $contentType = "application/octet-stream"
}

# Calculate SHA256 hash
Write-Host "Calculating SHA256 hash..." -ForegroundColor Yellow
$sha256 = [System.Security.Cryptography.SHA256]::Create()
$fileStream = [System.IO.File]::OpenRead($FilePath)
$hashBytes = $sha256.ComputeHash($fileStream)
$fileStream.Close()
$sha256Hash = [System.BitConverter]::ToString($hashBytes).Replace("-", "").ToLower()

# Display information
Write-Host ""
Write-Host "=== File Information ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "File Path:       $FilePath"
Write-Host "File Name:       $fileName"
Write-Host "File Size:       $fileSize bytes" -NoNewline
Write-Host " ($([math]::Round($fileSize/1KB, 2)) KB)" -ForegroundColor Gray
Write-Host "Content Type:    $contentType"
Write-Host "SHA256 Hash:     $sha256Hash"
Write-Host ""

# Generate JSON for API request
Write-Host "=== JSON for PrepareTransactionImportBatchUpload ===" -ForegroundColor Cyan
$prepareJson = @{
    fileName = $fileName
    contentType = $contentType
    fileSize = $fileSize
    accountId = "00000000-0000-0000-0000-000000000000"  # Replace with your Account ID
} | ConvertTo-Json -Depth 10

Write-Host $prepareJson
Write-Host ""

Write-Host "=== JSON for ConfirmTransactionImportBatchUpload ===" -ForegroundColor Cyan
$confirmJson = @{
    importBatchId = "00000000-0000-0000-0000-000000000000"  # Will be returned from Prepare endpoint
    sha256Hash = $sha256Hash
} | ConvertTo-Json -Depth 10

Write-Host $confirmJson
Write-Host ""

Write-Host "Note: Replace the placeholder GUIDs with your actual Account ID and Import Batch ID" -ForegroundColor Yellow
