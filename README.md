# boerenboodschap-api

## Planning

Directus Nextjs koppeling maken in test app met authenticatie en alles.

De kennis daarvan gebruiken om de huidige Nextjs app om te zetten om samen te werken met directus.

## Run the application

### Local Development with a mongo database

Configure the database connectionstring in the .env file.

### Locally

with docker-compose: `docker compose up`

with kubernetes:

1. `helm install bb-mongodb oci://registry-1.docker.io/bitnamicharts/mongodb`

2. Zoek in kubernetes secrets naar de credentials van de database en zet die in de connectionstring in deployment.yaml.

3. `helm install bb-api ./helm`

## status

This dotnet 7.0 API can handle basic CRUD operations on a mongoDB database that can be run with docker-compose.

Start the dev server with: `cd ./src/docker-compose && docker compose up`
