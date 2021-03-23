$environment = $env:ASPNETCORE_ENVIRONMENT
if (-not($environment)) {
    $environment = "Development"
}

docker run `
    --rm `
    -itd `
    -p 7000:80 `
    -e ASPNETCORE_ENVIRONMENT="$environment" `
    -e ASPNETCORE_URLS='http://+' `
    -e ESZOP_LOGS_DIR="$env:ESZOP_LOGS_DIR" `
    -v "$pwd\..\logs:/logs" `
    --network eszop-network `
    --name eszop-carts-api `
    eszop-carts-api