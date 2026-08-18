Get-Content .env | ForEach-Object {
    if ($_ -match '^\s*([^=]*)=(.*)$') {
        Set-Variable -Name $matches[1].Trim() -Value $matches[2].Trim()
    }
}

docker network create db-network
docker network create cache-network
docker network create messaging-network
docker network create storage-network

docker run -d `
    --name warehouse-postgres `
    --network db-network `
    -e POSTGRES_DB=$POSTGRES_DB `
    -e POSTGRES_USER=$POSTGRES_USER `
    -e POSTGRES_PASSWORD=$POSTGRES_PASSWORD `
    -v postgres-data:/var/lib/postgresql `
    postgres:18

docker run -d `
    --name warehouse-redis `
    --network cache-network `
    redis:7.4-alpine

docker run -d `
    --name warehouse-rabbitmq `
    --network messaging-network `
    -p 15672:15672 `
    -e RABBITMQ_USER=$RABBITMQ_USER `
    -e RABBITMQ_PASS=$RABBITMQ_PASSWORD `
    -v rabbitmq-data:/var/lib/rabbitmq `
    rabbitmq:3-management

docker run -d `
    --name warehouse-minio `
    --network storage-network `
    -p 9001:9001 `
    -e MINIO_ROOT_USER=$MINIO_USER `
    -e MINIO_ROOT_PASSWORD=$MINIO_PASSWORD `
    -e MINIO_BUCKET_NAME=$MINIO_BUCKET_NAME `
    -v minio-data:/data `
    minio/minio:latest server /data --console-address ":9001"

docker build -t warehouse-api -f ./WarehouseManagementApi/Dockerfile ./WarehouseManagementApi

docker run -d `
    --name warehouse-api `
    --network db-network `
    -p 8080:8080 `
    -e ASPNETCORE_ENVIRONMENT=Development `
    -e ConnectionStrings__DefaultConnection="Host=warehouse-postgres;Port=5432;Database=$POSTGRES_DB;Username=$POSTGRES_USER;Password=$POSTGRES_PASSWORD" `
    -e ConnectionStrings__Redis="warehouse-redis:6379" `
    -e RabbitMQ__Host="warehouse-rabbitmq" `
    -e RabbitMQ__Exchange="warehouse.events" `
    -e RabbitMQ__Port="5672" `
    -e RabbitMQ__Username=$RABBITMQ_USER `
    -e RabbitMQ__Password=$RABBITMQ_PASSWORD `
    -e MinIO__Endpoint="warehouse-minio:9000" `
    -e MinIO__AccessKey=$MINIO_USER `
    -e MinIO__SecretKey=$MINIO_PASSWORD `
    -e MinIO__BucketName="warehouse-assets" `
    -e MinIO__UseSsl="false" `
    -e Firebase__ProjectId=$FIREBASE_PROJECT_ID `
    -e FirebaseServiceAccountPath="/app/secrets/firebase-service-account.json" `
    -v "${FIREBASE_CREDENTIALS_HOST_PATH}:/app/secrets/firebase-service-account.json:ro" `
    warehouse-api

docker network connect cache-network warehouse-api
docker network connect messaging-network warehouse-api
docker network connect storage-network warehouse-api

docker ps

# before re-running this script:
# docker rm -f warehouse-api warehouse-postgres warehouse-redis warehouse-rabbitmq warehouse-minio
# docker network rm data-network cache-network messaging-network storage-network
