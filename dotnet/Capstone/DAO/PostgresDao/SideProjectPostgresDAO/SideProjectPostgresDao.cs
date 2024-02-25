using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class SideProjectPostgresDao : ISideProjectDao
    {
        private readonly string connectionString;
        private readonly IGoalDao _goalDao;
        private readonly IImageDao _imageDao;
        private readonly ISkillDao _skillDao;
        private readonly IContributorDao _contributorDao;
        private readonly IApiServiceDao _apiServiceDao;
        private readonly IWebsiteDao _websiteDao;
        private readonly IDependencyLibraryDao _dependencyLibraryDao;

        public SideProjectPostgresDao(string dbConnectionString, IGoalDao goalDao, IImageDao imageDao,
            ISkillDao skillDao, IContributorDao contributorDao, IApiServiceDao apiServiceDao,
            IDependencyLibraryDao dependencyLibraryDao, IWebsiteDao websiteDao)
        {
            connectionString = dbConnectionString;
            this._goalDao = goalDao;
            this._imageDao = imageDao;
            this._skillDao = skillDao;
            this._contributorDao = contributorDao;
            this._apiServiceDao = apiServiceDao;
            this._dependencyLibraryDao = dependencyLibraryDao;
            this._websiteDao = websiteDao;
        }

        /*  
            **********************************************************************************************
                                            SIDE PROJECT CRUD
            **********************************************************************************************
        */        

        public List<SideProject> GetSideProjects()
        {
            List<SideProject> sideProjects = new List<SideProject>();

            string sql = "SELECT id, name, main_image_id, description, video_walkthrough_url, " +
                    "website_id, github_repo_link_id, project_status, start_date, finish_date " +
                    "FROM sideprojects";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        SideProject sideProject = MapRowToSideProject(reader);
                        sideProjects.Add(sideProject);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }

            return sideProjects;
        }

        public SideProject GetSideProjectById(int sideProjectId)
        {
            SideProject sideProject = null;

            string sql = "SELECT id, name, main_image_id, description, video_walkthrough_url, " +
                    "website_id, github_repo_link_id, project_status, start_date, finish_date " +
                    "FROM sideprojects WHERE id = @sideProjectId";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        sideProject = MapRowToSideProject(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }

            return sideProject;
        }

        public SideProject CreateSideProject(SideProject sideProject)
        {
            SideProject newSideProject = null;

            string sql = "INSERT INTO sideprojects (name, description, video_walkthrough_url, project_status, " +
                        "start_date, finish_date, main_image_id, website_id, github_repo_link_id) " +
                         "VALUES (@name, @description, @video_walkthrough_url, @project_status, @start_date, " +
                         "@finish_date, @main_image_id, @website_id, @github_repo_link_id) " +
                         "RETURNING id";

            int newSideProjectId = 0;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@name", sideProject.Name);
                    cmd.Parameters.AddWithValue("@description", sideProject.Description);
                    cmd.Parameters.AddWithValue("@video_walkthrough_url", sideProject.VideoWalkthroughUrl);
                    cmd.Parameters.AddWithValue("@project_status", sideProject.ProjectStatus);
                    cmd.Parameters.AddWithValue("@start_date", sideProject.StartDate);
                    cmd.Parameters.AddWithValue("@finish_date", sideProject.FinishDate);
                    // cmd.Parameters.AddWithValue("@main_image_id", sideProject.MainImageId);
                    // cmd.Parameters.AddWithValue("@website_id", sideProject.WebsiteId);
                    // cmd.Parameters.AddWithValue("@github_repo_link_id", sideProject.GitHubRepoLinkId);

                    newSideProjectId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                newSideProject = GetSideProjectById(newSideProjectId);
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }

            return newSideProject;
        }
        public SideProject UpdateSideProject(SideProject sideProject, int sideProjectId)
        {
            string sql = "UPDATE sideprojects SET name = @name, description = @description, " +
                         "video_walkthrough_url = @video_walkthrough_url, project_status = @project_status, " +
                         "start_date = @start_date, finish_date = @finish_date, main_image_id = @main_image_id, " +
                         "website_id = @website_id, github_repo_link_id = @github_repo_link_id " +
                         "WHERE id = @sideProjectId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                    cmd.Parameters.AddWithValue("@name", sideProject.Name);
                    cmd.Parameters.AddWithValue("@description", sideProject.Description);
                    cmd.Parameters.AddWithValue("@video_walkthrough_url", sideProject.VideoWalkthroughUrl);
                    cmd.Parameters.AddWithValue("@project_status", sideProject.ProjectStatus);
                    cmd.Parameters.AddWithValue("@start_date", sideProject.StartDate);
                    cmd.Parameters.AddWithValue("@finish_date", sideProject.FinishDate);
                    // cmd.Parameters.AddWithValue("@main_image_id", sideProject.MainImageId);
                    // cmd.Parameters.AddWithValue("@website_id", sideProject.WebsiteId);
                    // cmd.Parameters.AddWithValue("@github_repo_link_id", sideProject.GitHubRepoLinkId);

                    int count = cmd.ExecuteNonQuery();

                    if (count == 1)
                    {
                        return sideProject;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the side project.", ex);
            }
        }

        public int DeleteSideProjectBySideProjectId(int sideProjectId)
        {
            int numberOfRowsAffected = 0;

            string sql = "DELETE FROM sideprojects WHERE id = @sideProjectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);

                    numberOfRowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the side project.", ex);
            }

            return numberOfRowsAffected;
        }
        private SideProject MapRowToSideProject(NpgsqlDataReader reader)
        {
            SideProject sideProject = new SideProject
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Description = Convert.ToString(reader["description"]),
                VideoWalkthroughUrl = Convert.ToString(reader["video_walkthrough_url"]),
                ProjectStatus = Convert.ToString(reader["project_status"]),
                StartDate = Convert.ToDateTime(reader["start_date"]),
                FinishDate = Convert.ToDateTime(reader["finish_date"])
            };


            int projectId = sideProject.Id;

            if (reader["main_image_id"] != DBNull.Value)
            {   
                sideProject.MainImageId = Convert.ToInt32(reader["main_image_id"]);

                int mainImageId = Convert.ToInt32(reader["main_image_id"]);
                sideProject.MainImage = _imageDao.GetImageByProjectIdAndImageId(projectId, mainImageId);
            }
            else
            {
                sideProject.MainImageId = 0;
            }

            if (reader["website_id"] != DBNull.Value)
            {
                sideProject.WebsiteId = Convert.ToInt32(reader["website_id"]);

                int websiteId = Convert.ToInt32(reader["website_id"]);
                sideProject.Website = _websiteDao.GetWebsiteByProjectIdAndWebsiteId(projectId, websiteId);
            }
            else
            {
                sideProject.WebsiteId = 0;
            }

            if (reader["github_repo_link_id"] != DBNull.Value)
            {
                sideProject.GitHubRepoLinkId = Convert.ToInt32(reader["github_repo_link_id"]);

                int githubRepoLinkId = Convert.ToInt32(reader["github_repo_link_id"]);
                sideProject.GitHubRepoLink = _websiteDao.GetWebsiteByProjectIdAndWebsiteId(projectId, githubRepoLinkId);
            }
            else
            {
                sideProject.GitHubRepoLinkId = 0;
            }

            sideProject.GoalsAndObjectives = _goalDao.GetGoalsByProjectId(projectId);
            sideProject.AdditionalImagesUrl = _imageDao.GetImagesByProjectId(projectId);
            sideProject.ToolsUsed = _skillDao.GetSkillsByProjectId(projectId);
            sideProject.Contributors = _contributorDao.GetContributorsByProjectId(projectId);
            sideProject.ExternalAPIsAndServicesUsed = _apiServiceDao.GetAPIsAndServicesByProjectId(projectId);
            sideProject.DependenciesOrLibrariesUsed = _dependencyLibraryDao.GetDependenciesAndLibrariesByProjectId(projectId);

            return sideProject;
        }
    }
}
