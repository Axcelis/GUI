param(
	[switch]$Run
)

Write-Host "Building BasicGUI..."
dotnet build .\BasicGUI

Write-Host "Building TestPublisherApp..."
dotnet build .\TestPublisherApp

Write-Host "Building TestSubscriberApp..."
dotnet build .\TestSubscriberApp

Write-Host "Build complete."

if ($Run) {
	Write-Host "Starting BasicGUI..."
	Start-Process "dotnet" "run --project .\BasicGUI"
	Start-Sleep -Seconds 1

	Write-Host "Starting TestPublisherApp..."
	Start-Process "dotnet" "run --project .\TestPublisherApp"

	Write-Host "Starting TestSubscriberApp..."
	Start-Process "dotnet" "run --project .\TestSubscriberApp"
}
