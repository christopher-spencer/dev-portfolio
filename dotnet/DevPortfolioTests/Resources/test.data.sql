BEGIN TRANSACTION;

DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS blogposts;

CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password_hash VARCHAR(200) NOT NULL,
    salt VARCHAR(200) NOT NULL,
    user_role VARCHAR(50) NOT NULL
);

CREATE TABLE blogposts (
	blogpost_id SERIAL,
	blogpost_name varchar(50) NOT NULL UNIQUE,
	blogpost_author varchar (50),
	blogpost_description varchar(300),
	blogpost_content varchar(5000),
	image_name varchar(50),
	image_url varchar(512),
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	CONSTRAINT PK_blogpost PRIMARY KEY (blogpost_id)
);

INSERT INTO users (username, password_hash, salt, user_role) VALUES ('user', 'Jg45HuwT7PZkfuKTz6IB90CtWY4=', 'LHxP4Xh7bN0=', 'user');
INSERT INTO users (username, password_hash, salt, user_role) VALUES ('admin', 'YhyGVQ+Ch69n4JMBncM4lNF/i9s=', 'Ar/aB2thQTI=', 'admin');

COMMIT TRANSACTION;