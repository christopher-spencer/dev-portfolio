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
    type VARCHAR(50) NOT NULL,
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
    volunteer_id INTEGER,
    achievement_id INTEGER,
    PRIMARY KEY (volunteer_id, achievement_id),
    FOREIGN KEY (volunteer_id) REFERENCES volunteer_works(id),
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
    volunteer_id INTEGER,
    skill_id INTEGER,
    PRIMARY KEY (volunteer_id, skill_id),
    FOREIGN KEY (volunteer_id) REFERENCES volunteer_works(id),
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
    volunteer_id INTEGER,
    PRIMARY KEY (portfolio_id, volunteer_id),
    FOREIGN KEY (portfolio_id) REFERENCES portfolios(id),
    FOREIGN KEY (volunteer_id) REFERENCES volunteer_works(id)
);

INSERT INTO users (username, password_hash, salt, user_role) 
VALUES ('testUser', 'Jg45HuwT7PZkfuKTz6IB90CtWY4=', 'LHxP4Xh7bN0=', 'test_user');
INSERT INTO users (username, password_hash, salt, user_role) 
VALUES ('testAdmin', 'YhyGVQ+Ch69n4JMBncM4lNF/i9s=', 'Ar/aB2thQTI=', 'test_admin');

--Creating a test portfolio
INSERT INTO portfolios (name, location, professional_summary, email) 
VALUES ('Test Portfolio', 'Test Location', 'Test Professional Summary', 'email1@test.com')
RETURNING id INTO test_portfolio_id;

--Creating a test portfolio main image
INSERT INTO images (name, url, type) VALUES ('Portfolio Main Image', "MainImage.jpeg", 'main image') 
RETURNING id INTO test_main_image_id;

INSERT INTO portfolio_images (portfolio_id, image_id) 
VALUES (test_portfolio_id, test_main_image_id);

UPDATE portfolios SET main_image_id = test_main_image_id 
WHERE id = test_portfolio_id;

--Creating test portfolio additional images 1 and 2
INSERT INTO images (name, url, type) 
VALUES ('Portfolio Additional Image 1', "AdditionalImage1.jpeg", 'additional image')
RETURNING id INTO test_additional_image_1_id;

INSERT INTO portfolio_images (portfolio_id, image_id)
VALUES (test_portfolio_id, test_additional_image_1_id);

INSERT INTO images (name, url, type) 
VALUES ('Portfolio Additional Image 2', "AdditionalImage2.jpeg", 'additional image')
RETURNING id INTO test_additional_image_2_id;

INSERT INTO portfolio_images (portfolio_id, image_id)
VALUES (test_portfolio_id, test_additional_image_2_id);

--Creating test portfolio websites
INSERT INTO websites (name, url, type)
VALUES ('Test Portfolio GitHub', 'https://www.github.com/portfolio-test', 'github')
RETURNING id INTO test_portfolio_github_id;

INSERT INTO portfolio_websites (portfolio_id, website_id)
VALUES (test_portfolio_id, test_portfolio_github_id);

INSERT INTO websites (name, url, type)
VALUES ('Test Portfolio LinkedIn', 'https://www.linkedin.com/portfolio-test', 'linkedin')
RETURNING id INTO test_portfolio_linkedin_id;

--Creating test portfolio website images
INSERT INTO images (name, url)
VALUES ('Portfolio GitHub Icon', 'GitHubIcon.jpeg')
RETURNING id INTO test_portfolio_github_image_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_portfolio_github_id, test_portfolio_github_image_id);

INSERT INTO images (name, url)
VALUES ('Portfolio LinkedIn Icon', 'LinkedInIcon.jpeg')
RETURNING id INTO test_portfolio_linkedin_image_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_portfolio_linkedin_id, test_portfolio_linkedin_image_id);

--Creating test portfolio hobbies
INSERT INTO hobbies (description)
VALUES ('Test Hobby Description 1')
RETURNING id INTO test_hobby_1_id;

INSERT INTO portfolio_hobbies (portfolio_id, hobby_id)
VALUES (test_portfolio_id, test_hobby_1_id);

INSERT INTO hobbies (description)
VALUES ('Test Hobby Description 2')
RETURNING id INTO test_hobby_2_id;

INSERT INTO portfolio_hobbies (portfolio_id, hobby_id)
VALUES (test_portfolio_id, test_hobby_2_id);

--Creating test portfolio hobby images
INSERT INTO images (name, url)
VALUES ('Hobby Image 1', 'HobbyImage1.jpeg')
RETURNING id INTO test_hobby_image_1_id;

INSERT INTO hobby_images (hobby_id, image_id)
VALUES (test_hobby_1_id, test_hobby_image_1_id);

INSERT INTO images (name, url)
VALUES ('Hobby Image 2', 'HobbyImage2.jpeg')
RETURNING id INTO test_hobby_image_2_id;

INSERT INTO hobby_images (hobby_id, image_id)
VALUES (test_hobby_2_id, test_hobby_image_2_id);

--Creating test portfolio tech skills
INSERT INTO skills (name)
VALUES ('Test Skill 1')
RETURNING id INTO test_skill_1_id;

INSERT INTO portfolio_skills (portfolio_id, skill_id)
VALUES (test_portfolio_id, test_skill_1_id);

INSERT INTO skills (name)
VALUES ('Test Skill 2')
RETURNING id INTO test_skill_2_id;

INSERT INTO portfolio_skills (portfolio_id, skill_id)
VALUES (test_portfolio_id, test_skill_2_id);

--Creating test portfolio tech skill icons
INSERT INTO images (name, url)
VALUES ('Skill Icon 1', 'SkillIcon1.jpeg')
RETURNING id INTO test_skill_icon_1_id;

INSERT INTO skill_images (skill_id, image_id)
VALUES (test_skill_1_id, test_skill_icon_1_id);

INSERT INTO images (name, url)
VALUES ('Skill Icon 2', 'SkillIcon2.jpeg')
RETURNING id INTO test_skill_icon_2_id;

INSERT INTO skill_images (skill_id, image_id)
VALUES (test_skill_2_id, test_skill_icon_2_id);

--Creating test portfolio sideprojects
INSERT INTO sideprojects (name, description, video_walkthrough_url, project_status, 
    start_date, finish_date) 
VALUES ('Test Sideproject', 'Test Description', 'Test Video Walkthrough URL', 
    'Test Project Status', '2021-01-01', '2021-01-02')
RETURNING id INTO test_sideproject_1_id;

INSERT INTO sideprojects (name, description, video_walkthrough_url, project_status, 
    start_date, finish_date)
VALUES ('Test Sideproject 2', 'Test Description 2', 'Test Video Walkthrough URL 2', 
    'Test Project Status 2', '2021-01-01', '2021-01-02')
RETURNING id INTO test_sideproject_2_id;

--Creating test portfolio sideproject 1 main image
INSERT INTO images (name, url, type)
VALUES ('Sideproject 1 Main Image', 'Sideproject1MainImage.jpeg', 'main image')
RETURNING id INTO test_sideproject_1_main_image_id;

INSERT INTO sideproject_images (sideproject_id, image_id)
VALUES (test_sideproject_1_id, test_sideproject_1_main_image_id);

UPDATE sideprojects SET main_image_id = test_sideproject_1_main_image_id
WHERE id = test_sideproject_1_id;

--Creating test portfolio sideproject 1 additional images 1 and 2
INSERT INTO images (name, url, type)
VALUES ('Sideproject 1 Additional Image 1', 'Sideproject1AdditionalImage1.jpeg', 
    'additional image')
RETURNING id INTO test_sideproject_1_additional_image_1_id;

INSERT INTO sideproject_images (sideproject_id, image_id)
VALUES (test_sideproject_1_id, test_sideproject_1_additional_image_1_id);

INSERT INTO images (name, url, type)
VALUES ('Sideproject 1 Additional Image 2', 'Sideproject1AdditionalImage2.jpeg', 
    'additional image')
RETURNING id INTO test_sideproject_1_additional_image_2_id;

INSERT INTO sideproject_images (sideproject_id, image_id)
VALUES (test_sideproject_1_id, test_sideproject_1_additional_image_2_id);

--Creating test portfolio sideproject 1 websites
INSERT INTO websites (name, url, type)
VALUES ('Sideproject 1 GitHub', 'https://www.github.com/sideproject1', 'github')
RETURNING id INTO test_sideproject_1_github_id;

INSERT INTO sideproject_websites (sideproject_id, website_id)
VALUES (test_sideproject_1_id, test_sideproject_1_github_id);

INSERT INTO websites (name, url, type)
VALUES ('Sideproject 1 Main Website', 'https://www.main-website.com/sideproject1', 
    'main website')
RETURNING id INTO test_sideproject_1_main_website_id;

INSERT INTO sideproject_websites (sideproject_id, website_id)
VALUES (test_sideproject_1_id, test_sideproject_1_main_website_id);

--Creating test portfolio sideproject 1 goals
INSERT INTO goals (description)
VALUES ('Test Goal 1')
RETURNING id INTO test_goal_1_id;

INSERT INTO sideproject_goals (sideproject_id, goal_id)
VALUES (test_sideproject_1_id, test_goal_1_id);

INSERT INTO goals (description)
VALUES ('Test Goal 2')
RETURNING id INTO test_goal_2_id;

INSERT INTO sideproject_goals (sideproject_id, goal_id)
VALUES (test_sideproject_1_id, test_goal_2_id);

--Creating test portfolio sideproject goal icons
INSERT INTO images (name, url)
VALUES ('Goal Icon 1', 'GoalIcon1.jpeg')
RETURNING id INTO test_goal_icon_1_id;

INSERT INTO goal_images (goal_id, image_id)
VALUES (test_goal_1_id, test_goal_icon_1_id);

INSERT INTO images (name, url)
VALUES ('Goal Icon 2', 'GoalIcon2.jpeg')
RETURNING id INTO test_goal_icon_2_id;

INSERT INTO goal_images (goal_id, image_id)
VALUES (test_goal_2_id, test_goal_icon_2_id);

--Creating test portfolio sideproject 1 tools used
INSERT INTO skills (name)
VALUES ('Test Tool 1')
RETURNING id INTO test_tool_1_id;

INSERT INTO sideproject_skills (sideproject_id, skill_id)
VALUES (test_sideproject_1_id, test_tool_1_id);

INSERT INTO skills (name)
VALUES ('Test Tool 2')
RETURNING id INTO test_tool_2_id;

INSERT INTO sideproject_skills (sideproject_id, skill_id)
VALUES (test_sideproject_1_id, test_tool_2_id);

--Creating test portfolio sideproject 1 tools used icons
INSERT INTO images (name, url)
VALUES ('Tool 1 Icon', 'Tool1Icon.jpeg')
RETURNING id INTO test_tool_1_icon_id;

INSERT INTO skill_images (skill_id, image_id)
VALUES (test_tool_1_id, test_tool_1_icon_id);

INSERT INTO images (name, url)
VALUES ('Tool 2 Icon', 'Tool2Icon.jpeg')
RETURNING id INTO test_tool_2_icon_id;

INSERT INTO skill_images (skill_id, image_id)
VALUES (test_tool_2_id, test_tool_2_icon_id);

--Creating test portfolio sideproject 1 contributors
INSERT INTO contributors (first_name, last_name, email, bio, contribution_details)
VALUES ('Johnny', 'Testman', 'johnnytestman@test.com', 'Test Bio', 'Test Contribution Details')
RETURNING id INTO test_contributor_1_id;

INSERT INTO sideproject_contributors (sideproject_id, contributor_id)
VALUES (test_sideproject_1_id, test_contributor_1_id);

INSERT INTO contributors (first_name, last_name, email, bio, contribution_details)
VALUES ('Jane', 'Testwoman', 'janetestwoman@test.com', 'Test Bio 2', 
    'Test Contribution Details 2')
RETURNING id INTO test_contributor_2_id;

INSERT INTO sideproject_contributors (sideproject_id, contributor_id)
VALUES (test_sideproject_1_id, test_contributor_2_id);

--Creating test portfolio sideproject 1 contributor images
INSERT INTO images (name, url)
VALUES ('Johnny Testman Image', 'JohnnyTestmanImage.jpeg')
RETURNING id INTO test_contributor_1_image_id;

INSERT INTO contributor_images (contributor_id, image_id)
VALUES (test_contributor_1_id, test_contributor_1_image_id);

--Creating test portfolio sideproject 1 contributor websites
INSERT INTO websites (name, url, type)
VALUES ('Johnny Testman GitHub', 'https://www.github.com/johnny-testman', 'github')
RETURNING id INTO test_contributor_1_github_id;

INSERT INTO contributor_websites (contributor_id, website_id)
VALUES (test_contributor_1_id, test_contributor_1_github_id);

INSERT INTO websites (name, url, type)
VALUES ('Johnny Testman LinkedIn', 'https://www.linkedin.com/johnny-testman', 'linkedin')
RETURNING id INTO test_contributor_1_linkedin_id;

INSERT INTO contributor_websites (contributor_id, website_id)
VALUES (test_contributor_1_id, test_contributor_1_linkedin_id);

INSERT INTO websites (name, url, type)
VALUES ('Johnny Testman Portfolio Link', 'https://www.portfolio.com/johnny-testman', 
    'portfolio link')
RETURNING id INTO test_contributor_1_portfolio_id;

INSERT INTO contributor_websites (contributor_id, website_id)
VALUES (test_contributor_1_id, test_contributor_1_portfolio_id);

--Creating test portfolio sideproject 1 APIs and services used
INSERT INTO apis_and_services (name, description) 
VALUES ('Test API/Service 1', 'Test API/Service Description 1')
RETURNING id INTO test_api_service_1_id;

INSERT INTO sideproject_apis_and_services (sideproject_id, apiservice_id)
VALUES (test_sideproject_1_id, test_api_service_1_id);

--Creating test portfolio sideproject 1 API and service image
INSERT INTO images (name, url)
VALUES ('API/Service 1 Image', 'API_Service1Image.jpeg')
RETURNING id INTO test_api_service_1_image_id;

INSERT INTO api_service_images (apiservice_id, image_id)
VALUES (test_api_service_1_id, test_api_service_1_image_id);

--Creating test portfolio sideproject 1 API and service website
INSERT INTO websites (name, url)
VALUES ('API/Service 1 Website', 'https://www.api-service1.com')
RETURNING id INTO test_api_service_1_website_id;

INSERT INTO api_service_websites (apiservice_id, website_id)
VALUES (test_api_service_1_id, test_api_service_1_website_id);

--Creating test portfolio sideproject 1 dependencies and libraries used
INSERT INTO dependencies_and_libraries (name, description)
VALUES ('Test Dependency/Library 1', 'Test Dependency/Library Description 1')
RETURNING id INTO test_dependency_library_1_id;

INSERT INTO sideproject_dependencies_and_libraries (sideproject_id, dependencylibrary_id)
VALUES (test_sideproject_1_id, test_dependency_library_1_id);

--Creating test portfolio sideproject 1 dependency/library image
INSERT INTO images (name, url)
VALUES ('Dependency/Library 1 Image', 'Dependency_Library1Image.jpeg')
RETURNING id INTO test_dependency_library_1_image_id;

INSERT INTO dependency_library_images (dependencylibrary_id, image_id)
VALUES (test_dependency_library_1_id, test_dependency_library_1_image_id);

--Creating test portfolio sideproject 1 dependency/library website
INSERT INTO websites (name, url)
VALUES ('Dependency/Library 1 Website', 'https://www.dependency-library1.com')
RETURNING id INTO test_dependency_library_1_website_id;

INSERT INTO dependency_library_websites (dependencylibrary_id, website_id)
VALUES (test_dependency_library_1_id, test_dependency_library_1_website_id);

--Creating test portfolio background work experiences
INSERT INTO work_experiences (position_title, company_name, location, description, 
    start_date, end_date)
VALUES ('Test Position Title 1', 'Test Company 1', 'Test Location 1', 'Test Description 1', 
    '2021-01-01', '2021-01-02')
RETURNING id INTO test_work_experience_1_id;

INSERT INTO portfolio_work_experiences (portfolio_id, experience_id)
VALUES (test_portfolio_id, test_work_experience_1_id);

INSERT INTO work_experiences (position_title, company_name, location, description, 
    start_date)
VALUES ('Test Position Title 2', 'Test Company 2', 'Test Location 2', 'Test Description 2',
    '2021-01-01')
RETURNING id INTO test_work_experience_2_id;

INSERT INTO portfolio_work_experiences (portfolio_id, experience_id)
VALUES (test_portfolio_id, test_work_experience_2_id);

--Creating test portfolio background work experience company logo image
INSERT INTO images (name, url, type)
VALUES ('Company 1 Logo', 'Company1Logo.jpeg', 'logo')
RETURNING id INTO test_company_1_logo_id;

INSERT INTO work_experience_images (experience_id, image_id)
VALUES (test_work_experience_1_id, test_company_1_logo_id);

UPDATE work_experiences SET company_logo_id = test_company_1_logo_id
WHERE id = test_work_experience_1_id;

--Creating test portfolio background work experience main image
INSERT INTO images (name, url, type)
VALUES ('Work Experience 1 Main Image', 'WorkExperience1MainImage.jpeg', 'main image')
RETURNING id INTO test_work_experience_1_main_image_id;

INSERT INTO work_experience_images (experience_id, image_id)
VALUES (test_work_experience_1_id, test_work_experience_1_main_image_id);

UPDATE work_experiences SET main_image_id = test_work_experience_1_main_image_id
WHERE id = test_work_experience_1_id;

--Creating test portfolio background work experience additional images 1 and 2
INSERT INTO images (name, url, type)
VALUES ('Work Experience 1 Additional Image 1', 'WorkExperience1AdditionalImage1.jpeg', 
    'additional image')
RETURNING id INTO test_work_experience_1_additional_image_1_id;

INSERT INTO work_experience_images (experience_id, image_id)
VALUES (test_work_experience_1_id, test_work_experience_1_additional_image_1_id);

INSERT INTO images (name, url, type)
VALUES ('Work Experience 1 Additional Image 2', 'WorkExperience1AdditionalImage2.jpeg', 
    'additional image')
RETURNING id INTO test_work_experience_1_additional_image_2_id;

INSERT INTO work_experience_images (experience_id, image_id)
VALUES (test_work_experience_1_id, test_work_experience_1_additional_image_2_id);



COMMIT TRANSACTION;