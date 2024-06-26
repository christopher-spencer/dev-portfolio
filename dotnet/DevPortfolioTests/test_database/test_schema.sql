BEGIN TRANSACTION;

DROP TABLE IF EXISTS portfolio_images;
DROP TABLE IF EXISTS portfolio_websites;
DROP TABLE IF EXISTS portfolio_skills;
DROP TABLE IF EXISTS portfolio_work_experiences;
DROP TABLE IF EXISTS portfolio_educations;
DROP TABLE IF EXISTS portfolio_credentials;
DROP TABLE IF EXISTS portfolio_open_source_contributions;
DROP TABLE IF EXISTS portfolio_volunteer_works;
DROP TABLE IF EXISTS portfolio_hobbies;
DROP TABLE IF EXISTS portfolio_sideprojects;

DROP TABLE IF EXISTS sideproject_websites;
DROP TABLE IF EXISTS sideproject_dependencies_and_libraries;
DROP TABLE IF EXISTS sideproject_apis_and_services;
DROP TABLE IF EXISTS sideproject_contributors;
DROP TABLE IF EXISTS sideproject_goals;
DROP TABLE IF EXISTS sideproject_skills;

DROP TABLE IF EXISTS work_experience_skills;
DROP TABLE IF EXISTS credential_skills;
DROP TABLE IF EXISTS open_source_contribution_skills;
DROP TABLE IF EXISTS volunteer_work_skills;

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

DROP TABLE IF EXISTS volunteer_work_images;
DROP TABLE IF EXISTS open_source_contribution_images;
DROP TABLE IF EXISTS credential_images;
DROP TABLE IF EXISTS education_images;
DROP TABLE IF EXISTS work_experience_images;

DROP TABLE IF EXISTS contributor_websites;
DROP TABLE IF EXISTS api_service_websites;
DROP TABLE IF EXISTS dependency_library_websites;

DROP TABLE IF EXISTS volunteer_work_websites;
DROP TABLE IF EXISTS open_source_contribution_websites;
DROP TABLE IF EXISTS credential_websites;
DROP TABLE IF EXISTS education_websites;
DROP TABLE IF EXISTS work_experience_websites;

DROP TABLE IF EXISTS education_achievements;
DROP TABLE IF EXISTS work_experience_achievements;
DROP TABLE IF EXISTS open_source_contribution_achievements;
DROP TABLE IF EXISTS volunteer_work_achievements;

DROP TABLE IF EXISTS portfolios;
DROP TABLE IF EXISTS sideprojects;
DROP TABLE IF EXISTS blogposts;
DROP TABLE IF EXISTS users;

DROP TABLE IF EXISTS work_experiences;
DROP TABLE IF EXISTS educations;
DROP TABLE IF EXISTS credentials;
DROP TABLE IF EXISTS open_source_contributions;
DROP TABLE IF EXISTS volunteer_works;

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
    name VARCHAR(100) NOT NULL,
    url VARCHAR(2000) NOT NULL,
    type VARCHAR(50)
);

CREATE TABLE websites (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    url VARCHAR(2000) NOT NULL,
    type VARCHAR(50),
    logo_id INTEGER,
    FOREIGN KEY (logo_id) REFERENCES images(id)
);

CREATE TABLE skills (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    icon_id INTEGER,
    FOREIGN KEY (icon_id) REFERENCES images(id)
);

CREATE TABLE achievements (
    id SERIAL PRIMARY KEY,
    description VARCHAR(1000) NOT NULL,
    icon_id INTEGER,
    FOREIGN KEY (icon_id) REFERENCES images(id)
);

CREATE TABLE hobbies (
    id SERIAL PRIMARY KEY,
    description VARCHAR(1000) NOT NULL,
    icon_id INTEGER,
    FOREIGN KEY (icon_id) REFERENCES images(id)
);

CREATE TABLE dependencies_and_libraries (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(1000),
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
    email VARCHAR(150),
    bio VARCHAR(3000),
    contribution_details VARCHAR(3000),
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
    description VARCHAR(2000),
    website_id INTEGER,
    logo_id INTEGER,
    FOREIGN KEY (website_id) REFERENCES websites(id),
    FOREIGN KEY (logo_id) REFERENCES images(id)
);

CREATE TABLE goals (
    id SERIAL PRIMARY KEY,
    description VARCHAR(1000) NOT NULL,
    icon_id INTEGER,
    FOREIGN KEY (icon_id) REFERENCES images(id)
);

CREATE TABLE work_experiences (
    id SERIAL PRIMARY KEY,
    position_title VARCHAR(150) NOT NULL,
    company_name VARCHAR(150) NOT NULL,
    company_logo_id INTEGER,
    company_website_id INTEGER,
    location VARCHAR(300) NOT NULL,
    description VARCHAR(3000),
    start_date TIMESTAMP NOT NULL,
    end_date TIMESTAMP,
    main_image_id INTEGER,
    FOREIGN KEY (company_logo_id) REFERENCES images(id),
    FOREIGN KEY (company_website_id) REFERENCES websites(id),
    FOREIGN KEY (main_image_id) REFERENCES images(id)
);

CREATE TABLE educations (
    id SERIAL PRIMARY KEY,
    institution_name VARCHAR(150) NOT NULL,
    institution_logo_id INTEGER,
    institution_website_id INTEGER,
    location VARCHAR(300) NOT NULL,
    description VARCHAR(2000),
    field_of_study VARCHAR(300),
    major VARCHAR(150),
    minor VARCHAR(150),
    degree_obtained VARCHAR(200),
    gpa_overall NUMERIC(3,2),
    gpa_in_major NUMERIC(3,2),
    start_date TIMESTAMP NOT NULL,
    graduation_date TIMESTAMP,
    main_image_id INTEGER,
    FOREIGN KEY (institution_logo_id) REFERENCES images(id),
    FOREIGN KEY (institution_website_id) REFERENCES websites(id),
    FOREIGN KEY (main_image_id) REFERENCES images(id)
);

CREATE TABLE credentials (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    issuing_organization VARCHAR(250) NOT NULL,
    description VARCHAR(2000),
    organization_logo_id INTEGER,
    organization_website_id INTEGER,
    issue_date TIMESTAMP,
    expiration_date TIMESTAMP,
    credential_id_number INTEGER,
    credential_website_id INTEGER,
    main_image_id INTEGER,
    FOREIGN KEY (organization_logo_id) REFERENCES images(id),
    FOREIGN KEY (organization_website_id) REFERENCES websites(id),
    FOREIGN KEY (credential_website_id) REFERENCES websites(id),
    FOREIGN KEY (main_image_id) REFERENCES images(id)
);

CREATE TABLE open_source_contributions (
    id SERIAL PRIMARY KEY,
    project_name VARCHAR(200) NOT NULL,
    organization_name VARCHAR(200) NOT NULL,
    organization_logo_id INTEGER,
    start_date TIMESTAMP NOT NULL,
    end_date TIMESTAMP,
    project_description VARCHAR(2000),
    contribution_details VARCHAR(3000) NOT NULL,
    organization_website_id INTEGER,
    organization_github_id INTEGER,
    main_image_id INTEGER,
    FOREIGN KEY (organization_logo_id) REFERENCES images(id),
    FOREIGN KEY (organization_website_id) REFERENCES websites(id),
    FOREIGN KEY (organization_github_id) REFERENCES websites(id),
    FOREIGN KEY (main_image_id) REFERENCES images(id)
);

CREATE TABLE volunteer_works (
    id SERIAL PRIMARY KEY,
    organization_name VARCHAR(200) NOT NULL,
    organization_logo_id INTEGER,
    location VARCHAR(300),
    organization_description VARCHAR(2000),
    organization_website_id INTEGER,
    position_title VARCHAR(200) NOT NULL,
    start_date TIMESTAMP NOT NULL,
    end_date TIMESTAMP,
    main_image_id INTEGER,
    FOREIGN KEY (organization_logo_id) REFERENCES images(id),
    FOREIGN KEY (organization_website_id) REFERENCES websites(id),
    FOREIGN KEY (main_image_id) REFERENCES images(id)
);

CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(150) NOT NULL UNIQUE,
    password_hash VARCHAR(300) NOT NULL,
    salt VARCHAR(300) NOT NULL,
    user_role VARCHAR(100) NOT NULL
);

CREATE TABLE blogposts (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL UNIQUE,
    author VARCHAR(150) NOT NULL,
    description VARCHAR(3000) NOT NULL,
    content VARCHAR(15000) NOT NULL,
    main_image_id INTEGER,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (main_image_id) REFERENCES images(id)
);

CREATE TABLE sideprojects (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE,
    main_image_id INTEGER,
    description VARCHAR(5000) NOT NULL,
    video_walkthrough_url VARCHAR(2000),
    website_id INTEGER,
    github_repo_link_id INTEGER,
    project_status VARCHAR(75),
    start_date TIMESTAMP,
    finish_date TIMESTAMP,
    FOREIGN KEY (main_image_id) REFERENCES images(id),
    FOREIGN KEY (website_id) REFERENCES websites(id),
    FOREIGN KEY (github_repo_link_id) REFERENCES websites(id)
);

CREATE TABLE portfolios (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    main_image_id INTEGER,
    location VARCHAR(250),
    professional_summary VARCHAR(4000) NOT NULL,
    email VARCHAR(250),
    github_repo_link_id INTEGER,
    linkedin_id INTEGER,
    FOREIGN KEY (main_image_id) REFERENCES images(id),
    FOREIGN KEY (github_repo_link_id) REFERENCES websites(id),
    FOREIGN KEY (linkedin_id) REFERENCES websites(id)
);

CREATE TABLE education_achievements (
    education_id INTEGER,
    achievement_id INTEGER,
    PRIMARY KEY (education_id, achievement_id),
    FOREIGN KEY (education_id) REFERENCES educations(id),
    FOREIGN KEY (achievement_id) REFERENCES achievements(id)
);

CREATE TABLE work_experience_achievements (
    experience_id INTEGER,
    achievement_id INTEGER,
    PRIMARY KEY (experience_id, achievement_id),
    FOREIGN KEY (experience_id) REFERENCES work_experiences(id),
    FOREIGN KEY (achievement_id) REFERENCES achievements(id)
);

CREATE TABLE open_source_contribution_achievements (
    contribution_id INTEGER,
    achievement_id INTEGER,
    PRIMARY KEY (contribution_id, achievement_id),
    FOREIGN KEY (contribution_id) REFERENCES open_source_contributions(id),
    FOREIGN KEY (achievement_id) REFERENCES achievements(id)
);

CREATE TABLE volunteer_work_achievements (
    volunteer_work_id INTEGER,
    achievement_id INTEGER,
    PRIMARY KEY (volunteer_work_id, achievement_id),
    FOREIGN KEY (volunteer_work_id) REFERENCES volunteer_works(id),
    FOREIGN KEY (achievement_id) REFERENCES achievements(id)
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

CREATE TABLE volunteer_work_images (
    volunteer_work_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (volunteer_work_id, image_id),
    FOREIGN KEY (volunteer_work_id) REFERENCES volunteer_works(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE open_source_contribution_images (
    contribution_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (contribution_id, image_id),
    FOREIGN KEY (contribution_id) REFERENCES open_source_contributions(id),
    FOREIGN Key (image_id) REFERENCES images(id)
);

CREATE TABLE credential_images (
    credential_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (credential_id, image_id),
    FOREIGN KEY (credential_id) REFERENCES credentials(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE education_images (
    education_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (education_id, image_id),
    FOREIGN KEY (education_id) REFERENCES educations(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE work_experience_images (
    experience_id INTEGER,
    image_id INTEGER,
    PRIMARY KEY (experience_id, image_id),
    FOREIGN KEY (experience_id) REFERENCES work_experiences(id),
    FOREIGN KEY (image_id) REFERENCES images(id)
);

CREATE TABLE volunteer_work_websites (
    volunteer_work_id INTEGER,
    website_id INTEGER,
    PRIMARY KEY (volunteer_work_id, website_id),
    FOREIGN KEY (volunteer_work_id) REFERENCES volunteer_works(id),
    FOREIGN KEY (website_id) REFERENCES websites(id)
);

CREATE TABLE open_source_contribution_websites (
    contribution_id INTEGER,
    website_id INTEGER,
    PRIMARY KEY (contribution_id, website_id),
    FOREIGN KEY (contribution_id) REFERENCES open_source_contributions(id),
    FOREIGN KEY (website_id) REFERENCES websites(id)
);

CREATE TABLE credential_websites (
    credential_id INTEGER,
    website_id INTEGER,
    PRIMARY KEY (credential_id, website_id),
    FOREIGN KEY (credential_id) REFERENCES credentials(id),
    FOREIGN KEY (website_id) REFERENCES websites(id)
);

CREATE TABLE education_websites (
    education_id INTEGER,
    website_id INTEGER,
    PRIMARY KEY (education_id, website_id),
    FOREIGN KEY (education_id) REFERENCES educations(id),
    FOREIGN KEY (website_id) REFERENCES websites(id)
);

CREATE TABLE work_experience_websites (
    experience_id INTEGER,
    website_id INTEGER,
    PRIMARY KEY (experience_id, website_id),
    FOREIGN KEY (experience_id) REFERENCES work_experiences(id),
    FOREIGN KEY (website_id) REFERENCES websites(id)
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

CREATE TABLE work_experience_skills (
    experience_id INTEGER,
    skill_id INTEGER,
    PRIMARY KEY (experience_id, skill_id),
    FOREIGN KEY (experience_id) REFERENCES work_experiences(id),
    FOREIGN KEY (skill_id) REFERENCES skills(id)
);

CREATE TABLE credential_skills (
    credential_id INTEGER,
    skill_id INTEGER,
    PRIMARY KEY (credential_id, skill_id),
    FOREIGN KEY (credential_id) REFERENCES credentials(id),
    FOREIGN KEY (skill_id) REFERENCES skills(id)
);

CREATE TABLE open_source_contribution_skills (
    contribution_id INTEGER,
    skill_id INTEGER,
    PRIMARY KEY (contribution_id, skill_id),
    FOREIGN KEY (contribution_id) REFERENCES open_source_contributions(id),
    FOREIGN KEY (skill_id) REFERENCES skills(id)
);

CREATE TABLE volunteer_work_skills (
    volunteer_work_id INTEGER,
    skill_id INTEGER,
    PRIMARY KEY (volunteer_work_id, skill_id),
    FOREIGN KEY (volunteer_work_id) REFERENCES volunteer_works(id),
    FOREIGN KEY (skill_id) REFERENCES skills(id)
);

CREATE TABLE portfolio_skills (
    portfolio_id INTEGER,
    skill_id INTEGER,
    PRIMARY KEY (portfolio_id, skill_id),
    FOREIGN KEY (portfolio_id) REFERENCES portfolios(id),
    FOREIGN KEY (skill_id) REFERENCES skills(id)
);

CREATE TABLE portfolio_hobbies (
    portfolio_id INTEGER,
    hobby_id INTEGER,
    PRIMARY KEY (portfolio_id, hobby_id),
    FOREIGN KEY (portfolio_id) REFERENCES portfolios(id),
    FOREIGN KEY (hobby_id) REFERENCES hobbies(id)
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

CREATE TABLE portfolio_websites (
    portfolio_id INTEGER,
    website_id INTEGER,
    PRIMARY KEY (portfolio_id, website_id),
    FOREIGN KEY (portfolio_id) REFERENCES portfolios(id),
    FOREIGN KEY (website_id) REFERENCES websites(id)
);

CREATE TABLE portfolio_sideprojects (
    portfolio_id INTEGER,
    sideproject_id INTEGER,
    PRIMARY KEY (portfolio_id, sideproject_id),
    FOREIGN KEY (portfolio_id) REFERENCES portfolios(id),
    FOREIGN KEY (sideproject_id) REFERENCES sideprojects(id)
);

CREATE TABLE portfolio_educations (
    portfolio_id INTEGER,
    education_id INTEGER,
    PRIMARY KEY (portfolio_id, education_id),
    FOREIGN KEY (portfolio_id) REFERENCES portfolios(id),
    FOREIGN KEY (education_id) REFERENCES educations(id)
);

CREATE TABLE portfolio_work_experiences (
    portfolio_id INTEGER,
    experience_id INTEGER,
    PRIMARY KEY (portfolio_id, experience_id),
    FOREIGN KEY (portfolio_id) REFERENCES portfolios(id),
    FOREIGN KEY (experience_id) REFERENCES work_experiences(id)
);

CREATE TABLE portfolio_credentials (
    portfolio_id INTEGER,
    credential_id INTEGER,
    PRIMARY KEY (portfolio_id, credential_id),
    FOREIGN KEY (portfolio_id) REFERENCES portfolios(id),
    FOREIGN KEY (credential_id) REFERENCES credentials(id)
);

CREATE TABLE portfolio_open_source_contributions (
    portfolio_id INTEGER,
    contribution_id INTEGER,
    PRIMARY KEY (portfolio_id, contribution_id),
    FOREIGN KEY (portfolio_id) REFERENCES portfolios(id),
    FOREIGN KEY (contribution_id) REFERENCES open_source_contributions(id)
);

CREATE TABLE portfolio_volunteer_works (
    portfolio_id INTEGER,
    volunteer_work_id INTEGER,
    PRIMARY KEY (portfolio_id, volunteer_work_id),
    FOREIGN KEY (portfolio_id) REFERENCES portfolios(id),
    FOREIGN KEY (volunteer_work_id) REFERENCES volunteer_works(id)
);





COMMIT TRANSACTION;