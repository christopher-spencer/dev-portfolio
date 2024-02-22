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

        public List<SideProject> GetSideProjects()
        {
            List<SideProject> sideProjects = new List<SideProject>();

            string sql = "SELECT sideproject_id, sideproject_name, description, " +
                         "video_walkthrough_url, project_status, start_date, finish_date " +
                         "FROM sideprojects;";

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

            string sql = "SELECT sideproject_id, sideproject_name, description, " +
                         "video_walkthrough_url, project_status, start_date, finish_date " +
                         "FROM sideprojects WHERE sideproject_id = @sideproject_id";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@sideproject_id", sideProjectId);

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

        public int GetWebsiteIdBySideProjectId(int sideProjectId)
        {
            int websiteId = -1; // Default value to indicate absence
            //TODO fix this sql
            string sql = "SELECT w.id " +
                "FROM website w " +
                "JOIN side_project_website spw ON w.id = spw.website_id " +
                "WHERE spw.project_id = @sideProjectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);

                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        websiteId = Convert.ToInt32(result);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the website ID by side project ID.", ex);
            }

            return websiteId;
        }

        public SideProject CreateSideProject(SideProject sideProject)
        {
            SideProject newSideProject = null;

            string sql = "INSERT into sideprojects (sideproject_name, description, " +
                         "video_walkthrough_url, project_status, start_date, finish_date) " +
                         "VALUES (@sideproject_name, @description, " +
                         "@video_walkthrough_url, @project_status, @start_date, @finish_date) " +
                         "RETURNING sideproject_id;";

            int newSideProjectId = 0;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@sideproject_name", sideProject.Name);
                    cmd.Parameters.AddWithValue("@description", sideProject.Description);
                    cmd.Parameters.AddWithValue("@video_walkthrough_url", sideProject.VideoWalkthroughUrl);
                    cmd.Parameters.AddWithValue("@project_status", sideProject.ProjectStatus);
                    cmd.Parameters.AddWithValue("@start_date", sideProject.StartDate);
                    cmd.Parameters.AddWithValue("@finish_date", sideProject.FinishDate);

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
            string sql = "UPDATE sideprojects SET sideproject_name = @sideproject_name, " +
                         "description = @description, " +
                         "video_walkthrough_url = @video_walkthrough_url, " +
                         "project_status = @project_status, start_date = @start_date, " +
                         "finish_date = @finish_date " +
                         "WHERE sideproject_id = @sideproject_id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@sideproject_id", sideProjectId);
                    cmd.Parameters.AddWithValue("@sideproject_name", sideProject.Name);
                    cmd.Parameters.AddWithValue("@description", sideProject.Description);
                    cmd.Parameters.AddWithValue("@video_walkthrough_url", sideProject.VideoWalkthroughUrl);
                    cmd.Parameters.AddWithValue("@project_status", sideProject.ProjectStatus);
                    cmd.Parameters.AddWithValue("@start_date", sideProject.StartDate);
                    cmd.Parameters.AddWithValue("@finish_date", sideProject.FinishDate);

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

            string sql = "DELETE FROM sideprojects WHERE sideproject_id = @sideproject_id;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@sideproject_id", sideProjectId);

                    numberOfRowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the side project.", ex);
            }

            return numberOfRowsAffected;
        }
        // TODO will need full CRUD methods for all PostgresDaos by PROJECT ID :)
        private SideProject MapRowToSideProject(NpgsqlDataReader reader)
        {
            SideProject sideProject = new SideProject
            {
                Id = Convert.ToInt32(reader["sideproject_id"]),
                Name = Convert.ToString(reader["sideproject_name"]),
                Description = Convert.ToString(reader["description"]),
                VideoWalkthroughUrl = Convert.ToString(reader["video_walkthrough_url"]),
                // TODO switch to get links in same way below through Interface? (or add sql queries)
                // Website = new Website 
                // { 
                //     Name = Convert.ToString(reader["website_name"]),
                //     Url = Convert.ToString(reader["website_url"]),
                //     Logo = new Image 
                //     {
                //         Name = Convert.ToString(reader["website_logo_name"]),
                //         Url = Convert.ToString(reader["website_logo_url"]) 
                //     }    
                // },
                // GitHubRepoLink = new Website { Url = Convert.ToString(reader["github_repo_link_url"]) },
                ProjectStatus = Convert.ToString(reader["project_status"]),
                StartDate = Convert.ToDateTime(reader["start_date"]),
                FinishDate = Convert.ToDateTime(reader["finish_date"])
            };


            int projectId = sideProject.Id;
            int websiteId = GetWebsiteIdBySideProjectId(projectId);
            // FIXME Figure out how to do Image and Website correctly
            sideProject.MainImageUrl = _imageDao.GetImageByProjectId(projectId);

            sideProject.Website = _websiteDao.GetWebsiteByProjectIdAndWebsiteId(projectId, websiteId);
            sideProject.GitHubRepoLink = _websiteDao.GetWebsiteByProjectIdAndWebsiteId(projectId, websiteId);

            sideProject.GoalsAndObjectives = _goalDao.GetGoalsAndObjectivesByProjectId(projectId);
            sideProject.AdditionalImagesUrl = _imageDao.GetImagesByProjectId(projectId);
            sideProject.ToolsUsed = _skillDao.GetSkillsByProjectId(projectId);
            sideProject.Contributors = _contributorDao.GetContributorsByProjectId(projectId);
            sideProject.ExternalAPIsAndServicesUsed = _apiServiceDao.GetAPIsAndServicesByProjectId(projectId);
            sideProject.DependenciesOrLibrariesUsed = _dependencyLibraryDao.GetDependenciesAndLibrariesByProjectId(projectId);

            return sideProject;
        }
    }
}
