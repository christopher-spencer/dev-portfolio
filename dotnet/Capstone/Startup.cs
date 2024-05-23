using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Capstone.DAO;
using Capstone.Security;
using Capstone.DAO.Interfaces;

namespace Capstone
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    });
            });

            string connectionString = Configuration.GetConnectionString("Project");

            // configure jwt authentication
            var key = Encoding.ASCII.GetBytes(Configuration["JwtSecret"]);
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[JwtRegisteredClaimNames.Sub] = "sub";
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    NameClaimType = "name"
                };
            });

            // Dependency Injection configuration
            
            // Instantiate DAOs for dependencies
            IImageDao imageDao = new ImagePostgresDao(connectionString);
            IGoalDao goalDao = new GoalPostgresDao(connectionString, imageDao);
            ISkillDao skillDao = new SkillPostgresDao(connectionString, imageDao);
            IWebsiteDao websiteDao = new WebsitePostgresDao(connectionString, imageDao);
            IContributorDao contributorDao = new ContributorPostgresDao(connectionString, imageDao, websiteDao);
            IApiServiceDao apiServiceDao = new ApiServicePostgresDao(connectionString, imageDao, websiteDao);
            IDependencyLibraryDao dependencyLibraryDao = new DependencyLibraryPostgresDao(connectionString, imageDao, 
                websiteDao);

            ISideProjectDao sideProjectDao = new SideProjectPostgresDao(
                connectionString, goalDao, imageDao, skillDao, contributorDao, apiServiceDao, dependencyLibraryDao, 
                websiteDao);

            IAchievementDao achievementDao = new AchievementPostgresDao(connectionString, imageDao);
            IHobbyDao hobbyDao = new HobbyPostgresDao(connectionString, imageDao);
            ICredentialDao credentialDao = new CredentialPostgresDao(connectionString, imageDao, websiteDao, skillDao);
            IVolunteerWorkDao volunteerWorkDao = new VolunteerWorkPostgresDao(connectionString, imageDao, websiteDao, 
                achievementDao, skillDao);
            IOpenSourceContributionDao openSourceContributionDao = new OpenSourceContributionPostgresDao(connectionString, 
                imageDao, websiteDao, achievementDao, skillDao);
            IEducationDao educationDao = new EducationPostgresDao(connectionString, imageDao, websiteDao, achievementDao);
            IWorkExperienceDao workExperienceDao = new WorkExperiencePostgresDao(connectionString, imageDao, skillDao, websiteDao, 
                achievementDao);

            IPortfolioDao portfolioDao = new PortfolioPostgresDao(connectionString, sideProjectDao, websiteDao, imageDao, 
                skillDao, workExperienceDao, educationDao, credentialDao, volunteerWorkDao, openSourceContributionDao, hobbyDao);
// NOTE added this IBlogPostDao, possible missing fix
            IBlogPostDao blogPostDao = new BlogPostPostgresDao(connectionString, imageDao);

            // Register services with DI container
            services.AddSingleton<ITokenGenerator>(tk => new JwtGenerator(Configuration["JwtSecret"]));
            services.AddSingleton<IPasswordHasher>(ph => new PasswordHasher());
            services.AddTransient<IUserDao>(m => new UserPostgresDao(connectionString));

            services.AddTransient<IBlogPostDao>(m => new BlogPostPostgresDao(connectionString, imageDao));

            services.AddTransient<IPortfolioDao>(m => new PortfolioPostgresDao(
                connectionString, sideProjectDao, websiteDao, imageDao, skillDao, workExperienceDao, educationDao,
                credentialDao, volunteerWorkDao, openSourceContributionDao, hobbyDao));

            services.AddTransient<IWorkExperienceDao>(m => new WorkExperiencePostgresDao(connectionString, imageDao, skillDao, 
                websiteDao, achievementDao));
            services.AddTransient<IEducationDao>(m => new EducationPostgresDao(connectionString, imageDao, websiteDao, 
                achievementDao));
            services.AddTransient<IOpenSourceContributionDao>(m => new OpenSourceContributionPostgresDao(connectionString, imageDao, 
                websiteDao, achievementDao, skillDao));
            services.AddTransient<IVolunteerWorkDao>(m => new VolunteerWorkPostgresDao(connectionString, imageDao, websiteDao, 
                achievementDao, skillDao));
            services.AddTransient<ICredentialDao>(m => new CredentialPostgresDao(connectionString, imageDao, websiteDao, skillDao));
            services.AddTransient<IHobbyDao>(m => new HobbyPostgresDao(connectionString, imageDao));
            services.AddTransient<IAchievementDao>(m => new AchievementPostgresDao(connectionString, imageDao));

            services.AddTransient<ISideProjectDao>(m => new SideProjectPostgresDao(
                connectionString, goalDao, imageDao, skillDao, contributorDao, apiServiceDao,
                dependencyLibraryDao, websiteDao));

            services.AddTransient<IContributorDao>(m => new ContributorPostgresDao(connectionString, imageDao, websiteDao));
            services.AddTransient<IApiServiceDao>(m => new ApiServicePostgresDao(connectionString, imageDao, websiteDao));
            services.AddTransient<IDependencyLibraryDao>(m => new DependencyLibraryPostgresDao(connectionString, imageDao, 
                websiteDao));

            services.AddTransient<IImageDao>(m => new ImagePostgresDao(connectionString));
            services.AddTransient<IGoalDao>(m => new GoalPostgresDao(connectionString, imageDao));
            services.AddTransient<ISkillDao>(m => new SkillPostgresDao(connectionString, imageDao));
            services.AddTransient<IWebsiteDao>(m => new WebsitePostgresDao(connectionString, imageDao));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
