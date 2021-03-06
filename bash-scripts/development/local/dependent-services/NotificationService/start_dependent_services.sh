#!/bin/bash

docker run \
    --rm \
    -itd \
    -e ACCEPT_EULA=s \
    -e MSSQL_PID=Developer \
    -e MSSQL_SA_PASSWORD=Test1234 \
    -p 9100:1433 \
    -v eszop-notification-sqlserver:/var/opt/mssql \
    --name eszop-notification-sqlserver \
    --network eszop-network \
    mcr.microsoft.com/mssql/server:2019-latest