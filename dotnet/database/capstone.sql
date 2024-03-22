// TODO AUTOCONNECT DB UPDATES****

BEGIN TRANSACTION;

DROP TABLE IF EXISTS sideproject_websites;

DROP TABLE IF EXISTS sideproject_dependencies_and_libraries;
DROP TABLE IF EXISTS sideproject_apis_and_services;
DROP TABLE IF EXISTS sideproject_contributors;
DROP TABLE IF EXISTS sideproject_skills;
DROP TABLE IF EXISTS sideproject_goals;

DROP TABLE IF EXISTS portfolio_images
DROP TABLE IF EXISTS sideproject_images;
DROP TABLE IF EXISTS blogpost_images;

DROP TABLE IF EXISTS website_images;
DROP TABLE IF EXISTS skill_images;
DROP TABLE IF EXISTS goal_images;

DROP TABLE IF EXISTS achievement_images;
DROP TABLE IF EXISTS hobby_images;

DROP TABLE IF EXISTS contributor_images;
DROP TABLE IF EXISTS api_service_images;
DROP TABLE IF EXISTS dependency_library_images;

DROP TABLE IF EXISTS experience_images;

DROP TABLE IF EXISTS contributor_websites;
DROP TABLE IF EXISTS api_service_websites;
DROP TABLE IF EXISTS dependency_library_websites;

DROP TABLE IF EXISTS portfolios;
DROP TABLE IF EXISTS sideprojects;
DROP TABLE IF EXISTS blogposts;
DROP TABLE IF EXISTS users;

DROP TABLE IF EXISTS experiences;

DROP TABLE IF EXISTS goals;
DROP TABLE IF EXISTS dependencies_and_libraries;
DROP TABLE IF EXISTS contributors;
DROP TABLE IF EXISTS apis_and_services;

DROP TABLE IF EXISTS hobbies;
DROP TABLE IF EXISTS achievements;
DROP TABLE IF EXISTS websites;
DROP TABLE IF EXISTS skills;
DROP TABLE IF EXISTS images;

CREATE TABLE images (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    url VARCHAR(2000) NOT NULL,
    type VARCHAR(30)
);

CREATE TABLE websites (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    url VARCHAR(2000) NOT NULL,
    type VARCHAR(30) NOT NULL,
    logo_id INTEGER,
    FOREIGN KEY (logo_id) REFERENCES images(id)
);

CREATE TABLE skills (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    icon_id INTEGER,
    FOREIGN KEY (icon_id) REFERENCES images(id)
);

CREATE TABLE achievements (
    id SERIAL PRIMARY KEY,
    description VARCHAR(350) NOT NULL,
    icon_id INTEGER,
    FOREIGN KEY (icon_id) REFERENCES images(id)
);

CREATE TABLE hobbies (
    id SERIAL PRIMARY KEY,
    description VARCHAR(350) NOT NULL,
    icon_id INTEGER,
    FOREIGN KEY (icon_id) REFERENCES images(id)
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

CREATE TABLE apis_and_services (
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

CREATE TABLE experiences (
    id SERIAL PRIMARY KEY,
    position_title VARCHAR(50) NOT NULL,
    company_name VARCHAR(100) NOT NULL,
    company_logo_id INTEGER,
    company_website_id INTEGER,
    location VARCHAR(100),
    start_date TIMESTAMP,
    end_date TIMESTAMP,
    main_image_id INTEGER,
    FOREIGN KEY (company_logo_id) REFERENCES images(id),
    FOREIGN KEY (company_website_id) REFERENCES websites(id),
    FOREIGN KEY (main_image_id) REFERENCES images(id)
);

CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(200) NOT NULL,
    salt VARCHAR(200) NOT NULL,
    user_role VARCHAR(50) NOT NULL
);

CREATE TABLE blogposts (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    author VARCHAR(50) NOT NULL,
    description VARCHAR(300) NOT NULL,
    content VARCHAR(5000) NOT NULL,
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

CREATE TABLE portfolios (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    main_image_id INTEGER,
    location VARCHAR(60),
    professional_summary VARCHAR(500) NOT NULL,
    email VARCHAR(80),
    github_repo_link_id INTEGER,
    linkedin_id INTEGER,
    FOREIGN KEY (portfolio_image_id) REFERENCES images(id),
    FOREIGN KEY (github_repo_link_id) REFERENCES websites(id),
    FOREIGN KEY (linkedin_id) REFERENCES websites(id)

);

CREATE TABLE website_images (
    website_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (website_id, image_id),
    FOREIGN KEY (website_id) REFERENCES websites(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE skill_images (
    skill_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (skill_id, image_id),
    FOREIGN KEY (skill_id) REFERENCES skills(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE goal_images (
    goal_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (goal_id, image_id),
    FOREIGN KEY (goal_id) REFERENCES goals(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE achievement_images (
    achievement_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (achievement_id, image_id),
    FOREIGN KEY (achievement_id) REFERENCES achievements(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE hobby_images (
    hobby_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (hobby_id, image_id),
    FOREIGN KEY (hobby_id) REFERENCES hobbies(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE contributor_images (
    contributor_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (contributor_id, image_id),
    FOREIGN KEY (contributor_id) REFERENCES contributors(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE api_service_images (
    apiservice_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (apiservice_id, image_id),
    FOREIGN KEY (apiservice_id) REFERENCES apis_and_services(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE dependency_library_images (
    dependencylibrary_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (dependencylibrary_id, image_id),
    FOREIGN KEY (dependencylibrary_id) REFERENCES dependencies_and_libraries(id),
    FOREIGN KEY (image_id) REFERENCES images(id)    
);

CREATE TABLE experience_images (
    experience_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (experience_id, image_id),
    FOREIGN KEY (experience_id) REFERENCES experiences(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE contributor_websites (
    contributor_id INTEGER,
    website_id INTEGER,
    PRIMARY KEY (contributor_id, website_id),
    FOREIGN KEY (contributor_id) REFERENCES contributors(id),
    FOREIGN KEY (website_id) REFERENCES websites(id)
);

CREATE TABLE api_service_websites (
    apiservice_id INTEGER, 
    website_id INTEGER,
    PRIMARY KEY (apiservice_id, website_id),
    FOREIGN KEY (apiservice_id) REFERENCES apis_and_services(id),
    FOREIGN KEY (website_id) REFERENCES websites(id)   
);

CREATE TABLE dependency_library_websites (
    dependencylibrary_id INTEGER,
    website_id INTEGER,
    PRIMARY KEY (dependencylibrary_id, website_id),
    FOREIGN KEY (dependencylibrary_id) REFERENCES dependencies_and_libraries(id),
    FOREIGN KEY (website_id) REFERENCES websites(id)
);

CREATE TABLE blogpost_images (
    blogpost_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (blogpost_id, image_id),
    FOREIGN KEY (blogpost_id) REFERENCES blogposts(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE sideproject_images (
    sideproject_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (sideproject_id, image_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

// NOTE added DROP and portfolio_images table to modify
CREATE TABLE portfolio_images (
    portfolio_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (portfolio_id, image_id),
    FOREIGN KEY (portfolio_id) REFERENCES portfolios(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE sideproject_goals (
    sideproject_id INTEGER,
    goal_id INTEGER,
    PRIMARY KEY (sideproject_id, goal_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (goal_id) REFERENCES goals(id)
);

CREATE TABLE sideproject_skills (
    sideproject_id INTEGER,
    skill_id INTEGER,
    PRIMARY KEY (sideproject_id, skill_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (skill_id) REFERENCES skills(id)
);

CREATE TABLE sideproject_contributors (
    sideproject_id INTEGER,
    contributor_id INTEGER,
    PRIMARY KEY (sideproject_id, contributor_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (contributor_id) REFERENCES contributors(id)
);

CREATE TABLE sideproject_apis_and_services (
    sideproject_id INTEGER,
    apiservice_id INTEGER,
    PRIMARY KEY (sideproject_id, apiservice_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (apiservice_id) REFERENCES apis_and_services(id)
);

CREATE TABLE sideproject_dependencies_and_libraries (
    sideproject_id INTEGER,
    dependencylibrary_id INTEGER,
    PRIMARY KEY (sideproject_id, dependencylibrary_id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id),
    FOREIGN KEY (dependencylibrary_id) REFERENCES dependencies_and_libraries(id)
);

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