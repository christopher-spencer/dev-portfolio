CREATE USER dev_portfolio_owner
WITH PASSWORD 'finalcapstone';

GRANT ALL
ON ALL TABLES IN SCHEMA public
TO dev_portfolio_owner;

GRANT ALL
ON ALL SEQUENCES IN SCHEMA public
TO dev_portfolio_owner;

CREATE USER dev_portfolio_appuser
WITH PASSWORD 'finalcapstone';

GRANT SELECT, INSERT, UPDATE, DELETE
ON ALL TABLES IN SCHEMA public
TO dev_portfolio_appuser;

GRANT USAGE, SELECT
ON ALL SEQUENCES IN SCHEMA public
TO dev_portfolio_appuser;