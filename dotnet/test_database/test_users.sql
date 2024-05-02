CREATE USER test_dev_portfolio_owner
WITH PASSWORD 'test_password';

GRANT ALL
ON ALL TABLES IN SCHEMA public
TO test_dev_portfolio_owner;

GRANT ALL
ON ALL SEQUENCES IN SCHEMA public
TO test_dev_portfolio_owner;

CREATE USER test_dev_portfolio_appuser
WITH PASSWORD 'test_password';

GRANT SELECT, INSERT, UPDATE, DELETE
ON ALL TABLES IN SCHEMA public
TO test_dev_portfolio_appuser;

GRANT USAGE, SELECT
ON ALL SEQUENCES IN SCHEMA public
TO test_dev_portfolio_appuser;