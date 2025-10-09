using CyberCity.Application.Implement;
using CyberCity.Application.Interface;
using CyberCity.Doman.DBcontext;
using CyberCity.Infrastructure;
using CyberCity_AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using CyberCity.AutoMapper;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using CyberCity.Controller.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromMinutes(1);
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
});
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "CyberCity API",
		Version = "v1",
		Description = "CyberCity backend API"
	});

	var jwtSecurityScheme = new OpenApiSecurityScheme
	{
		BearerFormat = "JWT",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = JwtBearerDefaults.AuthenticationScheme,
		Description = "JWT Authorization header using the Bearer scheme.",
		Reference = new OpenApiReference
		{
			Type = ReferenceType.SecurityScheme,
			Id = JwtBearerDefaults.AuthenticationScheme
		}
	};
	c.AddSecurityDefinition("Bearer", jwtSecurityScheme);
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{ jwtSecurityScheme, new string[] { } }
	});
});

builder.Services.AddDbContext<CyberCityLearningFlatFormDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


//repos
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<CourseRepo>();
builder.Services.AddScoped<TopicRepo>();
builder.Services.AddScoped<SubtopicRepo>();
builder.Services.AddScoped<LessonRepo>();
builder.Services.AddScoped<CourseEnrollmentRepo>();
builder.Services.AddScoped<ModuleRepo>();
builder.Services.AddScoped<SubtopicProgressRepo>();
builder.Services.AddScoped<OrderRepo>();
builder.Services.AddScoped<NotificationRepo>();
builder.Services.AddScoped<CertificateRepo>();
builder.Services.AddScoped<OrganizationRepo>();
builder.Services.AddScoped<OrgMemberRepo>();
builder.Services.AddScoped<ConversationRepo>();
builder.Services.AddScoped<MessageRepo>();
builder.Services.AddScoped<ConversationMemberRepo>();
builder.Services.AddScoped<PricingPlanRepo>();
builder.Services.AddScoped<TeacherStudentRepo>();
//services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICourseEnrollmentService, CourseEnrollmentService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<ISubtopicService, SubtopicService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<ISubtopicProgressService, SubtopicProgressService>();
builder.Services.AddScoped<IDashboardSerivce, DashboardService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IPricingPlanService, PricingPlanService>();
builder.Services.AddScoped<ITeacherStudentService, TeacherStudentService>();
builder.Services.AddScoped<IEmailService, EmailService>();
//mapper
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddAutoMapper(typeof(CourseProfile));
builder.Services.AddAutoMapper(typeof(TopicProfile));
builder.Services.AddAutoMapper(typeof(SubtopicProfile));
builder.Services.AddAutoMapper(typeof(LessonProfile));
builder.Services.AddAutoMapper(typeof(CourseEnrollmentProfile));
builder.Services.AddAutoMapper(typeof(ModuleProfile));
builder.Services.AddAutoMapper(typeof(SubtopicProgressProfile));
builder.Services.AddAutoMapper(typeof(OrganizationProfile));
builder.Services.AddAutoMapper(typeof(ConversationProfile));
builder.Services.AddAutoMapper(typeof(MessageProfile));
builder.Services.AddAutoMapper(typeof(PricingPlanProfile));
builder.Services.AddAutoMapper(typeof(TeacherStudentProfile));
// JWT Auth
var jwtKey = builder.Configuration["Jwt:Key"];
var keyBytes = Encoding.UTF8.GetBytes(jwtKey ?? "");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Get token from Authorization header
                var bearerToken = context.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(bearerToken))
                {
                    return Task.CompletedTask;
                }

                // Get token from cookie
                var token = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                    return Task.CompletedTask;
                }

                // Get token from query string for SignalR
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = 401,
                    message = "Unauthorized: Token is missing or invalid"
                });
                return context.Response.WriteAsync(result);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = 403,
                    message = "Forbidden: You don’t have permission to access this resource."
                });
                return context.Response.WriteAsync(result);
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReactApp",
		policy => policy.WithOrigins("http://localhost:5173", "https://cyber-city-fe.vercel.app")
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();

// Enable static files
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR Hub
app.MapHub<ChatHub>("/chatHub");

app.Run();
