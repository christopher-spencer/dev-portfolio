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
	blogpost_name VARCHAR(50) NOT NULL UNIQUE,
	blogpost_author VARCHAR(50),
	blogpost_description VARCHAR(300),
	blogpost_content VARCHAR(5000),
	image_name VARCHAR(50),
	image_url VARCHAR(512),
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	CONSTRAINT PK_blogpost PRIMARY KEY (blogpost_id)
);
// TODO text vs varchar
CREATE TABLE sideprojects (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE,
    main_image_id INTEGER,
    description TEXT,
    video_walkthrough_url TEXT,
    website_id INTEGER,
    github_repo_link_id INTEGER,
    project_status VARCHAR(100),
    start_date TIMESTAMP,
    finish_date TIMESTAMP,
    FOREIGN KEY (main_image_id) REFERENCES images(id),
    FOREIGN KEY (website_id) REFERENCES websites(id),
    FOREIGN KEY (github_repo_link_id) REFERENCES websites(id)
);

CREATE TABLE images (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    url TEXT
);

CREATE TABLE skills (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    icon_id INTEGER,
    FOREIGN KEY (icon_id) REFERENCES images(id)
);

CREATE TABLE websites (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    url TEXT,
    logo_id INTEGER,
    FOREIGN KEY (logo_id) REFERENCES images(id)
);

CREATE TABLE apis_and_services (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    website_id INTEGER,
    logo_id INTEGER,
    FOREIGN KEY (website_id) REFERENCES websites(id),
    FOREIGN KEY (logo_id) REFERENCES images(id)
);

CREATE TABLE contributors (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    contributor_image_id INTEGER,
    email VARCHAR(255),
    bio TEXT,
    contribution_details TEXT,
    linkedin_link_id INTEGER,
    github_link_id INTEGER,
    portfolio_link_id INTEGER,
    FOREIGN KEY (contributor_image_id) REFERENCES images(id),
    FOREIGN KEY (linkedin_link_id) REFERENCES websites(id),
    FOREIGN KEY (github_link_id) REFERENCES websites(id),
    FOREIGN KEY (portfolio_link_id) REFERENCES websites(id)
);

CREATE TABLE dependencies_and_libraries (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    website_id INTEGER,
    logo_id INTEGER,
    FOREIGN KEY (website_id) REFERENCES websites(id),
    FOREIGN KEY (logo_id) REFERENCES images(id)
);

CREATE TABLE goals (
    id SERIAL PRIMARY KEY,
    description TEXT,
    icon_id INTEGER,
    FOREIGN KEY (icon_id) REFERENCES images(id)
);

-- Join table for SideProject and Image
CREATE TABLE sideproject_images (
    sideproject_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (sideproject_id, image_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

-- Join table for SideProject and Goal
CREATE TABLE sideproject_goals (
    sideproject_id INTEGER,
    goal_id INTEGER,
    PRIMARY KEY (sideproject_id, goal_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (goal_id) REFERENCES goals(id)
);

-- Join table for SideProject and Skill
CREATE TABLE sideproject_skills (
    sideproject_id INTEGER,
    skill_id INTEGER,
    PRIMARY KEY (sideproject_id, skill_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (skill_id) REFERENCES skills(id)
);

-- Join table for SideProject and Contributor
CREATE TABLE sideproject_contributors (
    sideproject_id INTEGER,
    contributor_id INTEGER,
    PRIMARY KEY (sideproject_id, contributor_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (contributor_id) REFERENCES contributors(id)
);

-- Join table for SideProject and ApiService
CREATE TABLE sideproject_apis_and_services (
    sideproject_id INTEGER,
    apiservice_id INTEGER,
    PRIMARY KEY (sideproject_id, apiservice_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (apiservice_id) REFERENCES apis_and_services(id)
);

-- Join table for SideProject and DependencyLibrary
CREATE TABLE sideproject_dependencies_and_libraries (
    sideproject_id INTEGER,
    dependencylibrary_id INTEGER,
    PRIMARY KEY (sideproject_id, dependencylibrary_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (dependencylibrary_id) REFERENCES dependencies_and_libraries(id)
);

-- Join table for SideProject and Website
CREATE TABLE sideproject_websites (
    sideproject_id INTEGER,
    website_id INTEGER,
    PRIMARY KEY (sideproject_id, website_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (website_id) REFERENCES websites(id)
);


INSERT INTO users (username, password_hash, salt, user_role) VALUES ('user', 'Jg45HuwT7PZkfuKTz6IB90CtWY4=', 'LHxP4Xh7bN0=', 'user');
INSERT INTO users (username, password_hash, salt, user_role) VALUES ('admin', 'YhyGVQ+Ch69n4JMBncM4lNF/i9s=', 'Ar/aB2thQTI=', 'admin');

COMMIT TRANSACTION;
