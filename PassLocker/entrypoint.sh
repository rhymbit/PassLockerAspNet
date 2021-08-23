#!/bin/bash

until dotnet ef migrations add IntialCreate; do
>&2 echo "Creating database migrations"
sleep 1
done

>&2 echo "Database Migrations Created"

until dotnet ef database update; do
>&2 echo "SQL Server is starting up"
sleep 1
done

>&2 echo "SQL Server is up - executing command"