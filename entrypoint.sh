#!/usr/bin/env bash

set -e
run_cmd="dotnet Tayko.co.dll"

until dotnet ef database update; do
>&2 echo "PostgreSQL Server is starting up"
sleep 1
done

>&2 echo "PostgreSQL Server is up - starting app"
exec "$run_cmd"
