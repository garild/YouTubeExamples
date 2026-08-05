$ErrorActionPreference = "Stop"

function Test-TcpPort {
    param(
        [string]$Name,
        [string]$HostName = "localhost",
        [int]$Port
    )

    $client = New-Object System.Net.Sockets.TcpClient
    try {
        $client.Connect($HostName, $Port)
        Write-Host "[OK] $Name reachable on ${HostName}:$Port" -ForegroundColor Green
    }
    finally {
        $client.Close()
    }
}

Write-Host "Checking local infrastructure services..." -ForegroundColor Cyan

Test-TcpPort -Name "PostgreSQL" -Port 5432
Test-TcpPort -Name "Redis" -Port 6379
Test-TcpPort -Name "RabbitMQ (AMQP)" -Port 5672
Test-TcpPort -Name "RabbitMQ (Management UI)" -Port 15672
Test-TcpPort -Name "Keycloak" -Port 8080

try {
    $response = Invoke-WebRequest -Uri "http://localhost:9000/health/ready" -UseBasicParsing -TimeoutSec 10
    if ($response.StatusCode -eq 200) {
        Write-Host "[OK] Keycloak health endpoint is ready (port 9000)" -ForegroundColor Green
    }
    else {
        throw "Unexpected status code: $($response.StatusCode)"
    }
}
catch {
    Write-Host "[WARN] Keycloak management health is not ready yet: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "All core ports are reachable on localhost." -ForegroundColor Green
