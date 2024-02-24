BEGIN TRANSACTION;

DROP TABLE IF EXISTS sideproject_websites;
DROP TABLE IF EXISTS sideproject_dependencies_and_libraries;
DROP TABLE IF EXISTS sideproject_apis_and_services;
DROP TABLE IF EXISTS sideproject_contributors;
DROP TABLE IF EXISTS sideproject_skills;
DROP TABLE IF EXISTS sideproject_goals;
DROP TABLE IF EXISTS sideproject_images;
DROP TABLE IF EXISTS goals;
DROP TABLE IF EXISTS dependencies_and_libraries;
DROP TABLE IF EXISTS contributors;
DROP TABLE IF EXISTS apis_and_services;
DROP TABLE IF EXISTS websites;
DROP TABLE IF EXISTS skills;
DROP TABLE IF EXISTS images;
DROP TABLE IF EXISTS sideprojects;
DROP TABLE IF EXISTS blogposts;
DROP TABLE IF EXISTS users;

CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password_hash VARCHAR(200) NOT NULL,
    salt VARCHAR(200) NOT NULL,
    user_role VARCHAR(50) NOT NULL
);

CREATE TABLE blogposts (
	blogpost_id SERIAL PRIMARY KEY,
	blogpost_name VARCHAR(50) NOT NULL UNIQUE,
	blogpost_author VARCHAR(50) NOT NULL,
	blogpost_description VARCHAR(300) NOT NULL,
	blogpost_content VARCHAR(5000) NOT NULL,
	main_image_id INTEGER,
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (main_image_id) REFERENCES images(id)
);

CREATE TABLE sideprojects (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE,
    main_image_id INTEGER,
    description VARCHAR(500) NOT NULL,
    video_walkthrough_url VARCHAR(2000),
    website_id INTEGER,
    github_repo_link_id INTEGER,
    project_status VARCHAR(40),
    start_date TIMESTAMP,
    finish_date TIMESTAMP,
    FOREIGN KEY (main_image_id) REFERENCES images(id),
    FOREIGN KEY (website_id) REFERENCES websites(id),
    FOREIGN KEY (github_repo_link_id) REFERENCES websites(id)
);

CREATE TABLE images (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    url VARCHAR(2000) NOT NULL
);

CREATE TABLE skills (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    icon_id INTEGER,
    FOREIGN KEY (icon_id) REFERENCES images(id)
);

CREATE TABLE websites (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    url VARCHAR(2000),
    logo_id INTEGER,
    FOREIGN KEY (logo_id) REFERENCES images(id)
);

CREATE TABLE apis_and_services (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(400),
    website_id INTEGER,
    logo_id INTEGER,
    FOREIGN KEY (website_id) REFERENCES websites(id),
    FOREIGN KEY (logo_id) REFERENCES images(id)
);

CREATE TABLE contributors (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(60) NOT NULL,
    last_name VARCHAR(60) NOT NULL,
    contributor_image_id INTEGER,
    email VARCHAR(75),
    bio VARCHAR(500),
    contribution_details VARCHAR(500),
    linkedin_id INTEGER,
    github_id INTEGER,
    portfolio_id INTEGER,
    FOREIGN KEY (contributor_image_id) REFERENCES images(id),
    FOREIGN KEY (linkedin_id) REFERENCES websites(id),
    FOREIGN KEY (github_id) REFERENCES websites(id),
    FOREIGN KEY (portfolio_id) REFERENCES websites(id)
);

CREATE TABLE dependencies_and_libraries (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(400),
    website_id INTEGER,
    logo_id INTEGER,
    FOREIGN KEY (website_id) REFERENCES websites(id),
    FOREIGN KEY (logo_id) REFERENCES images(id)
);

CREATE TABLE goals (
    id SERIAL PRIMARY KEY,
    description VARCHAR(300) NOT NULL,
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
