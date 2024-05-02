-- **************************************************************
-- This script destroys the database and associated users
-- **************************************************************

-- The following line terminates any active connections to the database so that it can be destroyed
SELECT pg_terminate_backend(pid)
FROM pg_stat_activity
WHERE datname = 'test_dev_portfolio';

DROP DATABASE test_dev_portfolio;

DROP USER test_dev_portfolio_owner;
DROP USER test_dev_portfolio_appuser;