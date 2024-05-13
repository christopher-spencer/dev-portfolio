DO $$
DECLARE

--************************************************************************************
--PORTFOLIO 1 VARIABLES
--************************************************************************************

    test_portfolio_id INT;
    test_portfolio_main_image_id INT;
    test_portfolio_additional_image_1_id INT;
    test_portfolio_additional_image_2_id INT;
    test_portfolio_github_id INT;
    test_portfolio_linkedin_id INT;
    test_portfolio_github_image_id INT;
    test_portfolio_linkedin_image_id INT;
    test_portfolio_hobby_1_id INT;
    test_portfolio_hobby_2_id INT;
    test_portfolio_hobby_1_image_id INT;
    test_portfolio_hobby_2_image_id INT;
    test_portfolio_skill_1_id INT;
    test_portfolio_skill_2_id INT;
    test_portfolio_skill_1_icon_id INT;
    test_portfolio_skill_2_icon_id INT;

--************************************************************************************
--PORTFOLIO 1 SIDEPROJECTS VARIABLES
--************************************************************************************

    test_sideproject_1_id INT;
    test_sideproject_2_id INT;
    test_sideproject_1_main_image_id INT;
    test_sideproject_1_additional_image_1_id INT;
    test_sideproject_1_additional_image_2_id INT;
    test_sideproject_1_github_id INT;
    test_sideproject_1_main_website_id INT;
    test_sideproject_1_github_image_id INT;
    test_sideproject_1_main_website_image_id INT;
    test_sideproject_1_goal_1_id INT;
    test_sideproject_1_goal_2_id INT;
    test_sideproject_1_goal_1_icon_id INT;
    test_sideproject_1_goal_2_icon_id INT;
    test_sideproject_1_skill_1_id INT;
    test_sideproject_1_skill_2_id INT;
    test_sideproject_1_skill_1_icon_id INT;
    test_sideproject_1_skill_2_icon_id INT;

--************************************************************************************
--PORTFOLIO 1 SIDEPROJECT 1 CONTRIBUTORS VARIABLES 
--************************************************************************************

    test_sideproject_1_contributor_1_id INT;
    test_sideproject_1_contributor_2_id INT;
    test_sideproject_1_contributor_1_image_id INT;
    test_sideproject_1_contributor_1_github_id INT;
    test_sideproject_1_contributor_1_linkedin_id INT;
    test_sideproject_1_contributor_1_portfolio_link_id INT;
    test_sideproject_1_contributor_1_github_icon_id INT;
    test_sideproject_1_contributor_1_linkedin_icon_id INT;
    test_sideproject_1_contributor_1_portfolio_link_icon_id INT;

--************************************************************************************
--PORTFOLIO 1 SIDEPROJECT 1 APIS AND SERVICES VARIABLES
--************************************************************************************
// FIXME  update Portfolio 1 Sideproject 1 APIs & Services Variables  
    test_sideproject_1_api_service_1_id INT;
    test_api_service_1_image_id INT;
    test_api_service_1_website_id INT;

--************************************************************************************
--PORTFOLIO 1 SIDEPROJECT 1 DEPENDENCIES AND LIBRARIES VARIABLES
--************************************************************************************
// FIXME  update Portfolio 1 Sideproject 1 Dependencies & Libraries Variables
    test_dependency_library_1_id INT;
    test_dependency_library_1_image_id INT;
    test_dependency_library_1_website_id INT;

--************************************************************************************
--PORTFOLIO 1 SIDEPROJECT 1 BACKGROUND WORK EXPERIENCES VARIABLES
--************************************************************************************
// FIXME  update Portfolio 1 Background Work Experiences Variables
    test_work_experience_1_id INT;
    test_work_experience_2_id INT;
    test_company_1_logo_id INT;
    test_work_experience_1_main_image_id INT;
    test_work_experience_1_additional_image_1_id INT;
    test_work_experience_1_additional_image_2_id INT;
    test_work_experience_1_website_id INT;
    test_work_experience_1_website_image_id INT;

--************************************************************************************
--PORTFOLIO 1 SIDEPROJECT 1 EDUCATIONS VARIABLES
--************************************************************************************


BEGIN

--************************************************************************************
--CREATE USERS
--************************************************************************************

--Creating test user and test admin
INSERT INTO users (username, password_hash, salt, user_role) 
VALUES ('testUser', 'jjjjjjjjj', 'kkkkkkkkkk', 'user');
INSERT INTO users (username, password_hash, salt, user_role) 
VALUES ('testAdmin', 'jjjjjjjjj', 'kkkkkkkkkk', 'admin');

--************************************************************************************
--CREATE PORTFOLIO 1
--************************************************************************************

--Creating a test portfolio
INSERT INTO portfolios (name, location, professional_summary, email) 
VALUES ('Test Portfolio', 'Test Location', 'Test Professional Summary', 'email1@test.com')
RETURNING id INTO test_portfolio_id;

--Creating a test portfolio main image
INSERT INTO images (name, url, type) VALUES ('Portfolio Main Image', "MainImage.jpeg", 'main image') 
RETURNING id INTO test_portfolio_main_image_id;

INSERT INTO portfolio_images (portfolio_id, image_id) 
VALUES (test_portfolio_id, test_portfolio_main_image_id);

UPDATE portfolios SET main_image_id = test_portfolio_main_image_id 
WHERE id = test_portfolio_id;

--Creating test portfolio additional images 1 and 2
INSERT INTO images (name, url, type) 
VALUES ('Portfolio Additional Image 1', "AdditionalImage1.jpeg", 'additional image')
RETURNING id INTO test_portfolio_additional_image_1_id;

INSERT INTO portfolio_images (portfolio_id, image_id)
VALUES (test_portfolio_id, test_portfolio_additional_image_1_id);

INSERT INTO images (name, url, type) 
VALUES ('Portfolio Additional Image 2', "AdditionalImage2.jpeg", 'additional image')
RETURNING id INTO test_portfolio_additional_image_2_id;

INSERT INTO portfolio_images (portfolio_id, image_id)
VALUES (test_portfolio_id, test_portfolio_additional_image_2_id);

--Creating test portfolio websites
INSERT INTO websites (name, url, type)
VALUES ('Test Portfolio GitHub', 'https://www.github.com/portfolio-test', 'github')
RETURNING id INTO test_portfolio_github_id;

INSERT INTO portfolio_websites (portfolio_id, website_id)
VALUES (test_portfolio_id, test_portfolio_github_id);

UPDATE portfolios SET github_repo_link_id = test_portfolio_github_id
WHERE id = test_portfolio_id;

INSERT INTO websites (name, url, type)
VALUES ('Test Portfolio LinkedIn', 'https://www.linkedin.com/portfolio-test', 'linkedin')
RETURNING id INTO test_portfolio_linkedin_id;

INSERT INTO portfolio_websites (portfolio_id, website_id)
VALUES (test_portfolio_id, test_portfolio_linkedin_id);

UPDATE portfolios SET linkedin_id = test_portfolio_linkedin_id
WHERE id = test_portfolio_id;

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
RETURNING id INTO test_portfolio_hobby_1_id;

INSERT INTO portfolio_hobbies (portfolio_id, hobby_id)
VALUES (test_portfolio_id, test_portfolio_hobby_1_id);

INSERT INTO hobbies (description)
VALUES ('Test Hobby Description 2')
RETURNING id INTO test_portfolio_hobby_2_id;

INSERT INTO portfolio_hobbies (portfolio_id, hobby_id)
VALUES (test_portfolio_id, test_portfolio_hobby_2_id);

--Creating test portfolio hobby images
INSERT INTO images (name, url)
VALUES ('Hobby Image 1', 'HobbyImage1.jpeg')
RETURNING id INTO test_portfolio_hobby_1_image_id;

INSERT INTO hobby_images (hobby_id, image_id)
VALUES (test_portfolio_hobby_1_id, test_portfolio_hobby_1_image_id);

INSERT INTO images (name, url)
VALUES ('Hobby Image 2', 'HobbyImage2.jpeg')
RETURNING id INTO test_portfolio_hobby_2_image_id;

INSERT INTO hobby_images (hobby_id, image_id)
VALUES (test_portfolio_hobby_2_id, test_portfolio_hobby_2_image_id);

--Creating test portfolio tech skills
INSERT INTO skills (name)
VALUES ('Test Skill 1')
RETURNING id INTO test_portfolio_skill_1_id;

INSERT INTO portfolio_skills (portfolio_id, skill_id)
VALUES (test_portfolio_id, test_portfolio_skill_1_id);

INSERT INTO skills (name)
VALUES ('Test Skill 2')
RETURNING id INTO test_portfolio_skill_2_id;

INSERT INTO portfolio_skills (portfolio_id, skill_id)
VALUES (test_portfolio_id, test_portfolio_skill_2_id);

--Creating test portfolio tech skill icons
INSERT INTO images (name, url)
VALUES ('Skill Icon 1', 'SkillIcon1.jpeg')
RETURNING id INTO test_portfolio_skill_1_icon_id;

INSERT INTO skill_images (skill_id, image_id)
VALUES (test_portfolio_skill_1_id, test_portfolio_skill_1_icon_id);

INSERT INTO images (name, url)
VALUES ('Skill Icon 2', 'SkillIcon2.jpeg')
RETURNING id INTO test_portfolio_skill_2_icon_id;

INSERT INTO skill_images (skill_id, image_id)
VALUES (test_portfolio_skill_2_id, test_portfolio_skill_2_icon_id);

--************************************************************************************
--CREATE PORTFOLIO 1 SIDEPROJECTS
--************************************************************************************

--Creating test portfolio sideprojects
INSERT INTO sideprojects (name, description, video_walkthrough_url, project_status, 
    start_date, finish_date) 
VALUES ('Test Sideproject', 'Test Description', 'Test Video Walkthrough URL', 
    'Test Project Status', '2021-01-01', '2021-01-02')
RETURNING id INTO test_sideproject_1_id;

INSERT INTO portfolio_sideprojects (portfolio_id, sideproject_id)
VALUES (test_portfolio_id, test_sideproject_1_id);

INSERT INTO sideprojects (name, description, video_walkthrough_url, project_status, 
    start_date, finish_date)
VALUES ('Test Sideproject 2', 'Test Description 2', 'Test Video Walkthrough URL 2', 
    'Test Project Status 2', '2021-01-01', '2021-01-02')
RETURNING id INTO test_sideproject_2_id;

INSERT INTO portfolio_sideprojects (portfolio_id, sideproject_id)
VALUES (test_portfolio_id, test_sideproject_2_id);

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

UPDATE sideprojects SET github_repo_link_id = test_sideproject_1_github_id
WHERE id = test_sideproject_1_id;

INSERT INTO websites (name, url, type)
VALUES ('Sideproject 1 Main Website', 'https://www.main-website.com/sideproject1', 
    'main website')
RETURNING id INTO test_sideproject_1_main_website_id;

INSERT INTO sideproject_websites (sideproject_id, website_id)
VALUES (test_sideproject_1_id, test_sideproject_1_main_website_id);

UPDATE sideprojects SET website_id = test_sideproject_1_main_website_id
WHERE id = test_sideproject_1_id;

--Creating test portfolio sideproject 1 Github image
INSERT INTO images (name, url)
VALUES ('Sideproject 1 GitHub Icon', 'GitHubIcon1.jpeg')
RETURNING id INTO test_sideproject_1_github_image_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_sideproject_1_github_id, test_sideproject_1_github_image_id);

--Creating test portfolio sideproject 1 main website image
INSERT INTO images (name, url)
VALUES ('Sideproject 1 Main Website Image', 'MainWebsiteImage1.jpeg')
RETURNING id INTO test_sideproject_1_main_website_image_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_sideproject_1_main_website_id, test_sideproject_1_main_website_image_id);

--Creating test portfolio sideproject 1 goals
INSERT INTO goals (description)
VALUES ('Test Goal 1')
RETURNING id INTO test_sideproject_1_goal_1_id;

INSERT INTO sideproject_goals (sideproject_id, goal_id)
VALUES (test_sideproject_1_id, test_sideproject_1_goal_1_id);

INSERT INTO goals (description)
VALUES ('Test Goal 2')
RETURNING id INTO test_sideproject_1_goal_2_id;

INSERT INTO sideproject_goals (sideproject_id, goal_id)
VALUES (test_sideproject_1_id, test_sideproject_1_goal_2_id);

--Creating test portfolio sideproject goal icons
INSERT INTO images (name, url)
VALUES ('Goal Icon 1', 'GoalIcon1.jpeg')
RETURNING id INTO test_sideproject_1_goal_1_icon_id;

INSERT INTO goal_images (goal_id, image_id)
VALUES (test_sideproject_1_goal_1_id, test_sideproject_1_goal_1_icon_id);

INSERT INTO images (name, url)
VALUES ('Goal Icon 2', 'GoalIcon2.jpeg')
RETURNING id INTO test_sideproject_1_goal_2_icon_id;

INSERT INTO goal_images (goal_id, image_id)
VALUES (test_sideproject_1_goal_2_id, test_sideproject_1_goal_2_icon_id);

--Creating test portfolio sideproject 1 tools used
INSERT INTO skills (name)
VALUES ('Test Tool 1')
RETURNING id INTO test_sideproject_1_skill_1_id;

INSERT INTO sideproject_skills (sideproject_id, skill_id)
VALUES (test_sideproject_1_id, test_sideproject_1_skill_1_id);

INSERT INTO skills (name)
VALUES ('Test Tool 2')
RETURNING id INTO test_sideproject_1_skill_2_id;

INSERT INTO sideproject_skills (sideproject_id, skill_id)
VALUES (test_sideproject_1_id, test_sideproject_1_skill_2_id);

--Creating test portfolio sideproject 1 tools used icons
INSERT INTO images (name, url)
VALUES ('Tool 1 Icon', 'Tool1Icon.jpeg')
RETURNING id INTO test_sideproject_1_skill_1_icon_id;

INSERT INTO skill_images (skill_id, image_id)
VALUES (test_sideproject_1_skill_1_id, test_sideproject_1_skill_1_icon_id);

INSERT INTO images (name, url)
VALUES ('Tool 2 Icon', 'Tool2Icon.jpeg')
RETURNING id INTO test_sideproject_1_skill_2_icon_id;

INSERT INTO skill_images (skill_id, image_id)
VALUES (test_sideproject_1_skill_2_id, test_sideproject_1_skill_2_icon_id);

--************************************************************************************
-- CREATE PORTFOLIO 1 SIDEPROJECT 1 CONTRIBUTORS
--************************************************************************************

--Creating test portfolio sideproject 1 contributors
INSERT INTO contributors (first_name, last_name, email, bio, contribution_details)
VALUES ('Johnny', 'Testman', 'johnnytestman@test.com', 'Test Bio', 'Test Contribution Details')
RETURNING id INTO test_sideproject_1_contributor_1_id;

INSERT INTO sideproject_contributors (sideproject_id, contributor_id)
VALUES (test_sideproject_1_id, test_sideproject_1_contributor_1_id);

INSERT INTO contributors (first_name, last_name, email, bio, contribution_details)
VALUES ('Jane', 'Testwoman', 'janetestwoman@test.com', 'Test Bio 2', 
    'Test Contribution Details 2')
RETURNING id INTO test_sideproject_1_contributor_2_id;

INSERT INTO sideproject_contributors (sideproject_id, contributor_id)
VALUES (test_sideproject_1_id, test_sideproject_1_contributor_2_id);

--Creating test portfolio sideproject 1 contributor images
INSERT INTO images (name, url)
VALUES ('Johnny Testman Image', 'JohnnyTestmanImage.jpeg')
RETURNING id INTO test_sideproject_1_contributor_1_image_id;

INSERT INTO contributor_images (contributor_id, image_id)
VALUES (test_sideproject_1_contributor_1_id, test_sideproject_1_contributor_1_image_id);

--Creating test portfolio sideproject 1 contributor github
INSERT INTO websites (name, url, type)
VALUES ('Johnny Testman GitHub', 'https://www.github.com/johnny-testman', 'github')
RETURNING id INTO test_sideproject_1_contributor_1_github_id;

INSERT INTO contributor_websites (contributor_id, website_id)
VALUES (test_sideproject_1_contributor_1_id, test_sideproject_1_contributor_1_github_id);

UPDATE contributors SET github_id = test_sideproject_1_contributor_1_github_id
WHERE id = test_sideproject_1_contributor_1_id;

--Creating test portfolio sideproject 1 contributor linkedin

INSERT INTO websites (name, url, type)
VALUES ('Johnny Testman LinkedIn', 'https://www.linkedin.com/johnny-testman', 'linkedin')
RETURNING id INTO test_sideproject_1_contributor_1_linkedin_id;

INSERT INTO contributor_websites (contributor_id, website_id)
VALUES (test_sideproject_1_contributor_1_id, test_sideproject_1_contributor_1_linkedin_id);

UPDATE contributors SET linkedin_id = test_sideproject_1_contributor_1_linkedin_id
WHERE id = test_sideproject_1_contributor_1_id;

--Creating test portfolio sideproject 1 contributor portfolio link

INSERT INTO websites (name, url, type)
VALUES ('Johnny Testman Portfolio Link', 'https://www.portfolio.com/johnny-testman', 
    'portfolio link')
RETURNING id INTO test_sideproject_1_contributor_1_portfolio_link_id;

INSERT INTO contributor_websites (contributor_id, website_id)
VALUES (test_sideproject_1_contributor_1_id, test_sideproject_1_contributor_1_portfolio_link_id);

UPDATE contributors SET portfolio_id = test_sideproject_1_contributor_1_portfolio_link_id
WHERE id = test_sideproject_1_contributor_1_id;

--Creating test portfolio sideproject 1 github icon
INSERT INTO images (name, url)
VALUES ('Johnny Testman GitHub Icon', 'JohnnyTestmanGitHubIcon.jpeg')
RETURNING id INTO test_sideproject_1_contributor_1_github_icon_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_sideproject_1_contributor_1_github_id, test_sideproject_1_contributor_1_github_icon_id);

--Creating test portfolio sideproject 1 linkedin icon
INSERT INTO images (name, url)
VALUES ('Johnny Testman LinkedIn Icon', 'JohnnyTestmanLinkedInIcon.jpeg')
RETURNING id INTO test_sideproject_1_contributor_1_linkedin_icon_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_sideproject_1_contributor_1_linkedin_id, test_sideproject_1_contributor_1_linkedin_icon_id);

--Creating test portfolio sideproject 1 portfolio link icon
INSERT INTO images (name, url)
VALUES ('Johnny Testman Portfolio Link Icon', 'JohnnyTestmanPortfolioLinkIcon.jpeg')
RETURNING id INTO test_sideproject_1_contributor_1_portfolio_link_icon_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_sideproject_1_contributor_1_portfolio_link_id, 
    test_sideproject_1_contributor_1_portfolio_link_icon_id);

--************************************************************************************
--CREATE PORTFOLIO 1 SIDEPROJECT 1 APIS AND SERVICES
--************************************************************************************
// FIXME  update Portfolio 1 Sideproject 1 APIs & Services
--Creating test portfolio sideproject 1 APIs and services used
INSERT INTO apis_and_services (name, description) 
VALUES ('Test API/Service 1', 'Test API/Service Description 1')
RETURNING id INTO test_sideproject_1_api_service_1_id;

INSERT INTO sideproject_apis_and_services (sideproject_id, apiservice_id)
VALUES (test_sideproject_1_id, test_sideproject_1_api_service_1_id);

--Creating test portfolio sideproject 1 API and service image
INSERT INTO images (name, url)
VALUES ('API/Service 1 Image', 'API_Service1Image.jpeg')
RETURNING id INTO test_api_service_1_image_id;

INSERT INTO api_service_images (apiservice_id, image_id)
VALUES (test_sideproject_1_api_service_1_id, test_api_service_1_image_id);

--Creating test portfolio sideproject 1 API and service website
INSERT INTO websites (name, url)
VALUES ('API/Service 1 Website', 'https://www.api-service1.com')
RETURNING id INTO test_api_service_1_website_id;

INSERT INTO api_service_websites (apiservice_id, website_id)
VALUES (test_sideproject_1_api_service_1_id, test_api_service_1_website_id);

--************************************************************************************
--CREATE PORTFOLIO 1 SIDEPROJECT 1 DEPENDENCIES AND LIBRARIES
--************************************************************************************
// FIXME  update Portfolio 1 Sideproject 1 Dependencies & Libraries
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

--************************************************************************************
--CREATE PORTFOLIO 1 SIDEPROJECT 1 BACKGROUND WORK EXPERIENCES
--************************************************************************************
// FIXME  update Portfolio 1 Sideproject 1 Background Work Experiences
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

--Creating test portfolio background work experience 1 website
INSERT INTO websites (name, url)
VALUES ('Work Experience 1 Company Website', 'https://www.work-experience1.com')
RETURNING id INTO test_work_experience_1_website_id;

INSERT INTO work_experience_websites (experience_id, website_id)
VALUES (test_work_experience_1_id, test_work_experience_1_website_id);

--Creating test portfolio background work experience 1 website image
INSERT INTO images (name, url)
VALUES ('Work Experience 1 Website Image', 'WorkExperience1WebsiteImage.jpeg')
RETURNING id INTO test_work_experience_1_website_image_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_work_experience_1_website_id, test_work_experience_1_website_image_id);

--Creating test portfolio background work experience 1 responsibilities/achievements
INSERT INTO achievements (description)
VALUES ('Test Achievement 1')
RETURNING id INTO test_achievement_1_id;

INSERT INTO work_experience_achievements (experience_id, achievement_id)
VALUES (test_work_experience_1_id, test_achievement_1_id);

INSERT INTO achievements (description)
VALUES ('Test Achievement 2')
RETURNING id INTO test_achievement_2_id;

INSERT INTO work_experience_achievements (experience_id, achievement_id)
VALUES (test_work_experience_1_id, test_achievement_2_id);

--Creating test portfolio background work experience 1 achievement 1 image
INSERT INTO images (name, url)
VALUES ('Achievement 1 Image', 'Achievement1Image.jpeg')
RETURNING id INTO test_achievement_1_image_id;

INSERT INTO achievement_images (achievement_id, image_id)
VALUES (test_achievement_1_id, test_achievement_1_image_id);

--Creating test portfolio background work experience skills used and obtained
INSERT INTO skills (name)
VALUES ('Work Experience Test Skill 1')
RETURNING id INTO test_work_experience_skill_1_id;

INSERT INTO work_experience_skills (experience_id, skill_id)
VALUES (test_work_experience_1_id, test_work_experience_skill_1_id);

INSERT INTO skills (name)
VALUES ('Work Experience Test Skill 2')
RETURNING id INTO test_work_experience_skill_2_id;

INSERT INTO work_experience_skills (experience_id, skill_id)
VALUES (test_work_experience_1_id, test_work_experience_skill_2_id);

--Creating test portfolio background work experience skill 1 icon
INSERT INTO images (name, url)
VALUES ('Work Experience Skill 1 Icon', 'WorkExperienceSkill1Icon.jpeg')
RETURNING id INTO test_work_experience_skill_1_icon_id;

INSERT INTO skill_images (skill_id, image_id)
VALUES (test_work_experience_skill_1_id, test_work_experience_skill_1_icon_id);

--************************************************************************************
--CREATE PORTFOLIO 1 SIDEPROJECT 1 EDUCATIONS
--************************************************************************************
// TODO check Portfolio 1 Sideproject 1 Educations
--Creating test portfolio educations obtained
INSERT INTO educations (institution_name, location, description, field_of_study, major, 
    minor, degree_obtained, gpa_overall, gpa_in_major, start_date, graduation_date)
VALUES ('Test Institution 1', 'Test Location 1', 'Test Description 1', 
    'Test Field of Study 1', 'Test Major 1', 'Test Minor 1', 'Test Degree Obtained 1', 
    3.5, 3.7, '2021-01-01', '2021-01-02')
RETURNING id INTO test_education_1_id;

INSERT INTO portfolio_educations (portfolio_id, education_id)
VALUES (test_portfolio_id, test_education_1_id);

INSERT INTO educations (institution_name, location, description, field_of_study, major, 
    minor, degree_obtained, start_date)
VALUES ('Test Institution 2', 'Test Location 2', 'Test Description 2', 
    'Test Field of Study 2', 'Test Major 2', 'Test Minor 2', 'Test Degree Obtained 2',
    '2021-01-01')
RETURNING id INTO test_education_2_id;

INSERT INTO portfolio_educations (portfolio_id, education_id)
VALUES (test_portfolio_id, test_education_2_id);

--Creating test portfolio education 1 institution logo image
INSERT INTO images (name, url, type)
VALUES ('Institution 1 Logo', 'Institution1Logo.jpeg', 'logo')
RETURNING id INTO test_institution_1_logo_id;

INSERT INTO education_images (education_id, image_id)
VALUES (test_education_1_id, test_institution_1_logo_id);

UPDATE educations SET institution_logo_id = test_institution_1_logo_id
WHERE id = test_education_1_id;

--Creating test portfolio education 1 main image
INSERT INTO images (name, url, type)
VALUES ('Education 1 Main Image', 'Education1MainImage.jpeg', 'main image')
RETURNING id INTO test_education_1_main_image_id;

INSERT INTO education_images (education_id, image_id)
VALUES (test_education_1_id, test_education_1_main_image_id);

UPDATE educations SET main_image_id = test_education_1_main_image_id
WHERE id = test_education_1_id;

--Creating test portfolio education 1 additional images 1 and 2
INSERT INTO images (name, url, type)
VALUES ('Education 1 Additional Image 1', 'Education1AdditionalImage1.jpeg', 
    'additional image')
RETURNING id INTO test_education_1_additional_image_1_id;

INSERT INTO education_images (education_id, image_id)
VALUES (test_education_1_id, test_education_1_additional_image_1_id);

INSERT INTO images (name, url, type)
VALUES ('Education 1 Additional Image 2', 'Education1AdditionalImage2.jpeg', 
    'additional image')
RETURNING id INTO test_education_1_additional_image_2_id;

INSERT INTO education_images (education_id, image_id)
VALUES (test_education_1_id, test_education_1_additional_image_2_id);

--Creating test portfolio education 1 website
INSERT INTO websites (name, url)
VALUES ('Education 1 Institution Website', 'https://www.education1.com')
RETURNING id INTO test_education_1_website_id;

INSERT INTO education_websites (education_id, website_id)
VALUES (test_education_1_id, test_education_1_website_id);

--Creating test portfolio education 1 website image
INSERT INTO images (name, url)
VALUES ('Education 1 Website Image', 'Education1WebsiteImage.jpeg')
RETURNING id INTO test_education_1_website_image_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_education_1_website_id, test_education_1_website_image_id);

--Creating test portfolio education 1 honors and awards (achievements)
INSERT INTO achievements (description)
VALUES ('Test Honor/Award 1')
RETURNING id INTO test_honor_1_id;

INSERT INTO education_achievements (education_id, achievement_id)
VALUES (test_education_1_id, test_honor_1_id);

INSERT INTO achievements (description)
VALUES ('Test Honor/Award 2')
RETURNING id INTO test_honor_2_id;

INSERT INTO education_achievements (education_id, achievement_id)
VALUES (test_education_1_id, test_honor_2_id);

--Creating test portfolio education 1 honor/award achievement 1 image

INSERT INTO images (name, url)
VALUES ('Honor/Award 1 Image', 'HonorAward1Image.jpeg')
RETURNING id INTO test_honor_1_image_id;

INSERT INTO achievement_images (achievement_id, image_id)
VALUES (test_honor_1_id, test_honor_1_image_id);

--Creating test portfolio credentials obtained
INSERT INTO credentials (name, issuing_organization, description, issue_date, 
    expiration_date, credential_id_number)
VALUES ('Test Credential 1', 'Test Issuing Organization 1', 'Test Description 1',
    '2021-01-01', '2021-01-02', 1111111)
RETURNING id INTO test_credential_1_id;

INSERT INTO portfolio_credentials (portfolio_id, credential_id)
VALUES (test_portfolio_id, test_credential_1_id);

INSERT INTO credentials (name, issuing_organization, description)
VALUES ('Test Credential 2', 'Test Issuing Organization 2', 'Test Description 2')
RETURNING id INTO test_credential_2_id;

INSERT INTO portfolio_credentials (portfolio_id, credential_id)
VALUES (test_portfolio_id, test_credential_2_id);

--Creating test portfolio credential 1 organization logo
INSERT INTO images (name, url, type)
VALUES ('Credential 1 Organization Logo', 'Credential1OrganizationLogo.jpeg', 'logo')
RETURNING id INTO test_credential_1_organization_logo_id;

INSERT INTO credential_images (credential_id, image_id)
VALUES (test_credential_1_id, test_credential_1_organization_logo_id);

UPDATE credentials SET organization_logo_id = test_credential_1_organization_logo_id
WHERE id = test_credential_1_id;

--Creating test portfolio credential 1 main image
INSERT INTO images (name, url, type)
VALUES ('Credential 1 Main Image', 'Credential1MainImage.jpeg', 'main image')
RETURNING id INTO test_credential_1_main_image_id;

INSERT INTO credential_images (credential_id, image_id)
VALUES (test_credential_1_id, test_credential_1_main_image_id);

UPDATE credentials SET main_image_id = test_credential_1_main_image_id
WHERE id = test_credential_1_id;

--Creating test portfolio credential organization website
INSERT INTO websites (name, url, type)
VALUES ('Credential 1 Organization Website', 'https://www.credential1.com', 
    'main website')
RETURNING id INTO test_credential_1_website_id;

INSERT INTO credential_websites (credential_id, website_id)
VALUES (test_credential_1_id, test_credential_1_website_id);

UPDATE credentials SET organization_website_id = test_credential_1_website_id
WHERE id = test_credential_1_id;

--Creating test portfolio credential organization website image 
INSERT INTO images (name, url)
VALUES ('Credential 1 Website Image', 'Credential1WebsiteImage.jpeg')
RETURNING id INTO test_credential_1_website_image_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_credential_1_website_id, test_credential_1_website_image_id);

--Creating test portfolio credential 'credential website'
INSERT INTO websites (name, url, type)
VALUES ('Credential 1 Credential Website', 'https://www.credential1.com/credential', 
    'secondary website')
RETURNING id INTO test_credential_1_credential_website_id;

INSERT INTO credential_websites (credential_id, website_id)
VALUES (test_credential_1_id, test_credential_1_credential_website_id);

UPDATE credentials SET credential_website_id = test_credential_1_credential_website_id
WHERE id = test_credential_1_id;

--Creating test portfolio credential 'credential website' image
INSERT INTO images (name, url)
VALUES ('Credential 1 Credential Website Image', 'Credential1CredentialWebsiteImage.jpeg')
RETURNING id INTO test_credential_1_credential_website_image_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_credential_1_credential_website_id, 
    test_credential_1_credential_website_image_id);

--Creating test portfolio credential associated skills
INSERT INTO skills (name)
VALUES ('Credential Test Skill 1')
RETURNING id INTO test_credential_skill_1_id;

INSERT INTO credential_skills (credential_id, skill_id)
VALUES (test_credential_1_id, test_credential_skill_1_id);

--Creating test portfolio volunteer works   
INSERT INTO volunteer_works (organization_name, location, organization_description, 
    position_title, start_date, end_date)
VALUES ('Test Organization 1', 'Test Location 1', 'Test Organization Description 1',
    'Test Position Title 1', '2021-01-01', '2021-01-02')
RETURNING id INTO test_volunteer_1_id;

INSERT INTO portfolio_volunteer_works (portfolio_id, volunteer_id)
VALUES (test_portfolio_id, test_volunteer_1_id);

INSERT INTO volunteer_works (organization_name, location, organization_description, 
    position_title, start_date)
VALUES ('Test Organization 2', 'Test Location 2', 'Test Organization Description 2',
    'Test Position Title 2', '2021-01-01')
RETURNING id INTO test_volunteer_2_id;

INSERT INTO portfolio_volunteer_works (portfolio_id, volunteer_id)
VALUES (test_portfolio_id, test_volunteer_2_id);

--Creating test portfolio volunteer work organization logo
INSERT INTO images (name, url, type)
VALUES ('Volunteer Work 1 Organization Logo', 'VolunteerWork1OrganizationLogo.jpeg', 
    'logo')
RETURNING id INTO test_volunteer_1_organization_logo_id;

INSERT INTO volunteer_images (volunteer_id, image_id)
VALUES (test_volunteer_1_id, test_volunteer_1_organization_logo_id);

UPDATE volunteer_works SET organization_logo_id = test_volunteer_1_organization_logo_id
WHERE id = test_volunteer_1_id;

--Creating test portfolio volunteer work main image
INSERT INTO images (name, url, type)
VALUES ('Volunteer Work 1 Main Image', 'VolunteerWork1MainImage.jpeg', 'main image')
RETURNING id INTO test_volunteer_1_main_image_id;

INSERT INTO volunteer_images (volunteer_id, image_id)
VALUES (test_volunteer_1_id, test_volunteer_1_main_image_id);

UPDATE volunteer_works SET main_image_id = test_volunteer_1_main_image_id
WHERE id = test_volunteer_1_id;

--Creating test portfolio volunteer work additional images 1 and 2
INSERT INTO images (name, url, type)
VALUES ('Volunteer Work 1 Additional Image 1', 'VolunteerWork1AdditionalImage1.jpeg', 
    'additional image')
RETURNING id INTO test_volunteer_1_additional_image_1_id;

INSERT INTO volunteer_images (volunteer_id, image_id)
VALUES (test_volunteer_1_id, test_volunteer_1_additional_image_1_id);

INSERT INTO images (name, url, type)
VALUES ('Volunteer Work 1 Additional Image 2', 'VolunteerWork1AdditionalImage2.jpeg', 
    'additional image')
RETURNING id INTO test_volunteer_1_additional_image_2_id;

INSERT INTO volunteer_images (volunteer_id, image_id)
VALUES (test_volunteer_1_id, test_volunteer_1_additional_image_2_id);

--Creating test portfolio volunteer work 1 website
INSERT INTO websites (name, url)
VALUES ('Volunteer Work 1 Organization Website', 'https://www.volunteer-work1.com')
RETURNING id INTO test_volunteer_1_website_id;

INSERT INTO volunteer_websites (volunteer_id, website_id)
VALUES (test_volunteer_1_id, test_volunteer_1_website_id);

--Creating test portfolio volunteer work 1 website image
INSERT INTO images (name, url)
VALUES ('Volunteer Work 1 Website Image', 'VolunteerWork1WebsiteImage.jpeg')
RETURNING id INTO test_volunteer_1_website_image_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_volunteer_1_website_id, test_volunteer_1_website_image_id);

--Creating test portfolio volunteer work 1 responsibilities/achievements
INSERT INTO achievements (description)
VALUES ('Test Volunteer Work 1 Achievement 1')
RETURNING id INTO test_volunteer_1_achievement_1_id;

INSERT INTO volunteer_achievements (volunteer_id, achievement_id)
VALUES (test_volunteer_1_id, test_volunteer_1_achievement_1_id);

INSERT INTO achievements (description)
VALUES ('Test Volunteer Work 1 Achievement 2')
RETURNING id INTO test_volunteer_1_achievement_2_id;

INSERT INTO volunteer_achievements (volunteer_id, achievement_id)
VALUES (test_volunteer_1_id, test_volunteer_1_achievement_2_id);

--Creating test portfolio volunteer work 1 achievement 1 image
INSERT INTO images (name, url)
VALUES ('Volunteer Work 1 Achievement 1 Image', 'VolunteerWork1Achievement1Image.jpeg')
RETURNING id INTO test_volunteer_1_achievement_1_image_id;

INSERT INTO achievement_images (achievement_id, image_id)
VALUES (test_volunteer_1_achievement_1_id, test_volunteer_1_achievement_1_image_id);

--Creating test portfolio volunteer work 1 skills used and obtained
INSERT INTO skills (name)
VALUES ('Volunteer Work Test Skill 1')
RETURNING id INTO test_volunteer_1_skill_1_id;

INSERT INTO volunteer_skills (volunteer_id, skill_id)
VALUES (test_volunteer_1_id, test_volunteer_1_skill_1_id);

INSERT INTO skills (name)
VALUES ('Volunteer Work Test Skill 2')
RETURNING id INTO test_volunteer_1_skill_2_id;

INSERT INTO volunteer_skills (volunteer_id, skill_id)
VALUES (test_volunteer_1_id, test_volunteer_1_skill_2_id);

--Creating test portfolio volunteer work 1 skill 1 icon
INSERT INTO images (name, url)
VALUES ('Volunteer Work Skill 1 Icon', 'VolunteerWorkSkill1Icon.jpeg')
RETURNING id INTO test_volunteer_1_skill_1_icon_id;

INSERT INTO skill_images (skill_id, image_id)
VALUES (test_volunteer_1_skill_1_id, test_volunteer_1_skill_1_icon_id);

--Creating test portfolio volunteer work 1 skill 2 icon
INSERT INTO images (name, url)
VALUES ('Volunteer Work Skill 2 Icon', 'VolunteerWorkSkill2Icon.jpeg')
RETURNING id INTO test_volunteer_1_skill_2_icon_id;

INSERT INTO skill_images (skill_id, image_id)
VALUES (test_volunteer_1_skill_2_id, test_volunteer_1_skill_2_icon_id);

--Creating test portfolio open source contributions
INSERT INTO open_source_contributions (project_name, organization_name, start_date,
    end_date, project_description, contribution_details)
VALUES ('Test Project 1', 'Test Organization 1', '2021-01-01', '2021-01-02', 
    'Test Project Description 1', 'Test Contribution Details 1')
RETURNING id INTO test_open_source_1_id;

INSERT INTO portfolio_open_source_contributions (portfolio_id, contribution_id)
VALUES (test_portfolio_id, test_open_source_1_id);

INSERT INTO open_source_contributions (project_name, organization_name, start_date,
    project_description, contribution_details)
VALUES ('Test Project 2', 'Test Organization 2', '2021-01-01',
    'Test Project Description 2', 'Test Contribution Details 2')
RETURNING id INTO test_open_source_2_id;

INSERT INTO portfolio_open_source_contributions (portfolio_id, contribution_id)
VALUES (test_portfolio_id, test_open_source_2_id);

--Creating test portfolio open source contribution organization logo
INSERT INTO images (name, url, type)
VALUES ('Open Source 1 Organization Logo', 'OpenSource1OrganizationLogo.jpeg', 'logo')
RETURNING id INTO test_open_source_1_organization_logo_id;

INSERT INTO open_source_contribution_images (contribution_id, image_id)
VALUES (test_open_source_1_id, test_open_source_1_organization_logo_id);

UPDATE open_source_contributions 
SET organization_logo_id = test_open_source_1_organization_logo_id
WHERE id = test_open_source_1_id;

--Creating test portfolio open source contribution main image
INSERT INTO images (name, url, type)
VALUES ('Open Source 1 Main Image', 'OpenSource1MainImage.jpeg', 'main image')
RETURNING id INTO test_open_source_1_main_image_id;

INSERT INTO open_source_contribution_images (contribution_id, image_id)
VALUES (test_open_source_1_id, test_open_source_1_main_image_id);

UPDATE open_source_contributions
SET main_image_id = test_open_source_1_main_image_id
WHERE id = test_open_source_1_id;

--Creating test portfolio open source contribution additional images 1 and 2
INSERT INTO images (name, url, type)
VALUES ('Open Source 1 Additional Image 1', 'OpenSource1AdditionalImage1.jpeg', 
    'additional image')
RETURNING id INTO test_open_source_1_additional_image_1_id;

INSERT INTO open_source_contribution_images (contribution_id, image_id)
VALUES (test_open_source_1_id, test_open_source_1_additional_image_1_id);

INSERT INTO images (name, url, type)
VALUES ('Open Source 1 Additional Image 2', 'OpenSource1AdditionalImage2.jpeg', 
    'additional image')
RETURNING id INTO test_open_source_1_additional_image_2_id;

INSERT INTO open_source_contribution_images (contribution_id, image_id)
VALUES (test_open_source_1_id, test_open_source_1_additional_image_2_id);

--Creating test portfolio open source contribution 1 organization website
INSERT INTO websites (name, url, type)
VALUES ('Open Source 1 Organization Website', 'https://www.open-source1.com', 
    'main website')
RETURNING id INTO test_open_source_1_website_id;

INSERT INTO open_source_contribution_websites (contribution_id, website_id)
VALUES (test_open_source_1_id, test_open_source_1_website_id);

UPDATE open_source_contributions
SET organization_website_id = test_open_source_1_website_id
WHERE id = test_open_source_1_id;

--Creating test portfolio open source contribution 1 organization website image
INSERT INTO images (name, url)
VALUES ('Open Source 1 Website Image', 'OpenSource1WebsiteImage.jpeg')
RETURNING id INTO test_open_source_1_website_image_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_open_source_1_website_id, test_open_source_1_website_image_id);

--Creating test portfolio open source contribution 1 github website
INSERT INTO websites (name, url, type)
VALUES ('Open Source 1 GitHub', 'https://www.github.com/open-source1', 'github')
RETURNING id INTO test_open_source_1_github_id;

INSERT INTO open_source_contribution_websites (contribution_id, website_id)
VALUES (test_open_source_1_id, test_open_source_1_github_id);

UPDATE open_source_contributions
SET organization_github_id = test_open_source_1_github_id
WHERE id = test_open_source_1_id;

--Creating test portfolio open source contribution 1 github website image
INSERT INTO images (name, url)
VALUES ('Open Source 1 GitHub Image', 'OpenSource1GitHubImage.jpeg')
RETURNING id INTO test_open_source_1_github_image_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_open_source_1_github_id, test_open_source_1_github_image_id);

--Creating test portfolio open source contribution 1 pull request links 1 and 2
INSERT INTO websites(name, url, type)
VALUES ('Open Source 1 Pull Request 1', 'https://www.github.com/open-source1/pullrequest1', 
    'pull request link')
RETURNING id INTO test_open_source_1_pull_request_1_id;

INSERT INTO open_source_contribution_websites (contribution_id, website_id)
VALUES (test_open_source_1_id, test_open_source_1_pull_request_1_id);

INSERT INTO websites(name, url, type)
VALUES ('Open Source 1 Pull Request 2', 'https://www.github.com/open-source1/pullrequest2', 
    'pull request link')
RETURNING id INTO test_open_source_1_pull_request_2_id;

INSERT INTO open_source_contribution_websites (contribution_id, website_id)
VALUES (test_open_source_1_id, test_open_source_1_pull_request_2_id);

--Creating test portfolio open source contribution 1 pull request link 1 image
INSERT INTO images (name, url)
VALUES ('Open Source 1 Pull Request 1 Image', 'OpenSource1PullRequest1Image.jpeg')
RETURNING id INTO test_open_source_1_pull_request_1_image_id;

INSERT INTO website_images (website_id, image_id)
VALUES (test_open_source_1_pull_request_1_id, test_open_source_1_pull_request_1_image_id);

--Creating test portfolio open source contribution 1 tech skills utilized
INSERT INTO skills (name)
VALUES ('Open Source 1 Tech Skill 1')
RETURNING id INTO test_open_source_1_tech_skill_1_id;

INSERT INTO open_source_contribution_skills (contribution_id, skill_id)
VALUES (test_open_source_1_id, test_open_source_1_tech_skill_1_id);

--Creating test portfolio open source contribution 1 tech skill 1 icon
INSERT INTO images (name, url)
VALUES ('Open Source 1 Tech Skill 1 Icon', 'OpenSource1TechSkill1Icon.jpeg')
RETURNING id INTO test_open_source_1_tech_skill_1_icon_id;

INSERT INTO skill_images (skill_id, image_id)
VALUES (test_open_source_1_tech_skill_1_id, test_open_source_1_tech_skill_1_icon_id);

--Creating test portfolio open source contribution 1 review comments and feedback received (achievements)
INSERT INTO achievements (description)
VALUES ('Open Source 1 Review Comment 1')
RETURNING id INTO test_open_source_1_review_comment_1_id;

INSERT INTO open_source_contribution_achievements (contribution_id, achievement_id)
VALUES (test_open_source_1_id, test_open_source_1_review_comment_1_id);

INSERT INTO achievements (description)
VALUES ('Open Source 1 Review Comment 2')
RETURNING id INTO test_open_source_1_review_comment_2_id;

INSERT INTO open_source_contribution_achievements (contribution_id, achievement_id)
VALUES (test_open_source_1_id, test_open_source_1_review_comment_2_id);

--Creating test portfolio open source contribution 1 review comment/achievement 1 image
INSERT INTO images (name, url)
VALUES ('Open Source 1 Review Comment 1 Image', 'OpenSource1ReviewComment1Image.jpeg')
RETURNING id INTO test_open_source_1_review_comment_1_image_id;

INSERT INTO achievement_images (achievement_id, image_id)
VALUES (test_open_source_1_review_comment_1_id, test_open_source_1_review_comment_1_image_id);

--Creating test blog posts
INSERT INTO blogposts (name, author, description, content, created_at, updated_at)
VALUES ('Test Blog Post 1', 'Test Author 1', 'Test Description 1', 'Test Content 1', 
    '2021-01-01', '2021-01-02')
RETURNING id INTO test_blog_post_1_id;

INSERT INTO blogposts (name, author, description, content, created_at, updated_at)
VALUES ('Test Blog Post 2', 'Test Author 2', 'Test Description 2', 'Test Content 2', 
    '2021-01-01', '2021-01-02')
RETURNING id INTO test_blog_post_2_id;

--Creating test blog post 1 main image
INSERT INTO images (name, url, type)
VALUES ('Blog Post 1 Main Image', 'BlogPost1MainImage.jpeg', 'main image')
RETURNING id INTO test_blog_post_1_main_image_id;

INSERT INTO blogpost_images (blogpost_id, image_id)
VALUES (test_blog_post_1_id, test_blog_post_1_main_image_id);

UPDATE blogposts SET main_image_id = test_blog_post_1_main_image_id
WHERE id = test_blog_post_1_id;

END $$;