#!/bin/bash
export PGPASSWORD='postgres1'
BASEDIR=$(dirname $0)
DATABASE=test_dev_portfolio
psql -U postgres -f "$BASEDIR/test_dropdb.sql" &&
createdb -U postgres $DATABASE &&
psql -U postgres -d $DATABASE -f "$BASEDIR/test_schema.sql" &&
psql -U postgres -d $DATABASE -f "$BASEDIR/test_data.sql" &&
psql -U postgres -d $DATABASE -f "$BASEDIR/test_users.sql"