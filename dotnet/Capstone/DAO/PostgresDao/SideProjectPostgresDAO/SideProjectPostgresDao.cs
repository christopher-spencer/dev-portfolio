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
        private readonly IGoalDao goalDao;
        private readonly IImageDao imageDao;
        private readonly ISkillDao skillDao;
        private readonly IContributorDao contributorDao;
        private readonly IApiServiceDao apiServiceDao;
        private readonly IDependencyLibraryDao dependencyLibraryDao;

        public SideProjectPostgresDao(string dbConnectionString, IGoalDao goalDao, IImageDao imageDao, 
            ISkillDao skillDao, IContributorDao contributorDao, IApiServiceDao apiServiceDao, 
            IDependencyLibraryDao dependencyLibraryDao)
            {
                connectionString = dbConnectionString;
                this.goalDao = goalDao;
                this.imageDao = imageDao;
                this.skillDao = skillDao;
                this.contributorDao = contributorDao;
                this.apiServiceDao = apiServiceDao;
                this.dependencyLibraryDao = dependencyLibraryDao;
            }

        public List<SideProject> GetSideProjects()
        {
            List<SideProject> sideProjects = new List<SideProject>();

            string sql = "SELECT sideproject_id, sideproject_name, main_image_url, description, " +
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

            string sql = "SELECT sideproject_id, sideproject_name, main_image_url, description, " +
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

        public SideProject CreateSideProject(SideProject sideProject)
        {
            SideProject newSideProject = null;

            string sql = "INSERT into sideprojects (sideproject_name, main_image_url, description, " +
                         "video_walkthrough_url, project_status, start_date, finish_date) " +
                         "VALUES (@sideproject_name, @main_image_url, @description, " +
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
                    cmd.Parameters.AddWithValue("@main_image_url", sideProject.MainImageUrl);
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
                         "main_image_url = @main_image_url, description = @description, " +
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
                    cmd.Parameters.AddWithValue("@main_image_url", sideProject.MainImageUrl);
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

        private SideProject MapRowToSideProject(NpgsqlDataReader reader)
        {
            SideProject sideProject = new SideProject
            {
                Id = Convert.ToInt32(reader["sideproject_id"]),
                Name = Convert.ToString(reader["sideproject_name"]),
                // TODO switch to get images in same way below through Interface? 
                // FIXME (image_name not current sql query)
                MainImageUrl = new Image 
                {   
                    Name = Convert.ToString(reader["main_image_name"]),
                    Url = Convert.ToString(reader["main_image_url"]) 
                },
                Description = Convert.ToString(reader["description"]),
                VideoWalkthroughUrl = Convert.ToString(reader["video_walkthrough_url"]),
                // TODO switch to get links in same way below through Interface? (or add sql queries)
                Website = new Website 
                { 
                    Name = Convert.ToString(reader["website_name"]),
                    Url = Convert.ToString(reader["website_url"]) 
                },
                GitHubRepoLink = new Website { Url = Convert.ToString(reader["github_repo_link_url"]) },
                ProjectStatus = Convert.ToString(reader["project_status"]),
                StartDate = Convert.ToDateTime(reader["start_date"]),
                FinishDate = Convert.ToDateTime(reader["finish_date"])
            };

            int projectId = sideProject.Id;

            sideProject.GoalsAndObjectives = goalDao.GetGoalsAndObjectivesByProjectId(projectId);
            sideProject.AdditionalImagesUrl = imageDao.GetImagesByProjectId(projectId);
            sideProject.ToolsUsed = skillDao.GetSkillsByProjectId(projectId);
            sideProject.Contributors = contributorDao.GetContributorsByProjectId(projectId);
            sideProject.ExternalAPIsAndServicesUsed = apiServiceDao.GetAPIsAndServicesByProjectId(projectId);
            sideProject.DependenciesOrLibrariesUsed = dependencyLibraryDao.GetDependenciesAndLibrariesByProjectId(projectId);

            return sideProject;
        }
    }
}
