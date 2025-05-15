using Microsoft.EntityFrameworkCore;
using PM.Infrastructure.DataContext;
using PM.Application.Constants;
using PM.Application.InterfaceService;
using PM.Application.ImplementService;
using PM.Application.Payloads.Mappers;
using PM.Domain.InterfaceRepositories;
using PM.Domain.Entities;
using PM.Infrastructure.ImplementRepositories;
using PM.Application.Handle.HandleEmail;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using PM.Application;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(Constant.AppSettingKeys.DEFAULT_CONNECTION),
        b => b.MigrationsAssembly("Print_Management")));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserConverter>();
builder.Services.AddScoped <IBaseRepository<User>, BaseRepository<User>>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDbContext, AppDbContext>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBaseRepository<ConfirmEmail>, BaseRepository<ConfirmEmail>>();
builder.Services.AddScoped<IBaseRepository<RefreshToken>, BaseRepository<RefreshToken>>();
builder.Services.AddScoped<IBaseRepository<Permissions>, BaseRepository<Permissions>>();
builder.Services.AddScoped<IBaseRepository<Role>, BaseRepository<Role>>();
builder.Services.AddScoped<IBaseRepository<Step>, BaseRepository<Step>>();
builder.Services.AddScoped<IBaseRepository<StepTemplate>, BaseRepository<StepTemplate>>();
builder.Services.AddScoped<IBaseRepository<FlowTemplate>, BaseRepository<FlowTemplate>>();
builder.Services.AddScoped<IBaseRepository<StepFlowTemplate>, BaseRepository<StepFlowTemplate>>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IBaseRepository<Team>, BaseRepository<Team>>();
builder.Services.AddScoped<ITeamService, TeamService>();

builder.Services.AddScoped<IBaseRepository<Project>, BaseRepository<Project>>();
builder.Services.AddScoped<IProjectService, ProjectService>();

builder.Services.AddScoped<IBaseRepository<Design>, BaseRepository<Design>>();
builder.Services.AddScoped<IDesignService, DesignService>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IStepTemplateService, StepTemplateService>();
builder.Services.AddScoped<IFlowTemplateService, FlowTemplateService>();
builder.Services.AddScoped<IStepService, StepService>();

// Register the repositories for Product and Order
builder.Services.AddScoped<IBaseRepository<Product>, BaseRepository<Product>>();
builder.Services.AddScoped<IBaseRepository<Order>, BaseRepository<Order>>();


builder.Services.AddScoped<IBaseRepository<Notification>, BaseRepository<Notification>>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IBaseRepository<PrintJobs>, BaseRepository<PrintJobs>>();
builder.Services.AddScoped<IPrintJobService, PrintJobService>();

builder.Services.AddScoped<IBaseRepository<Customer>, BaseRepository<Customer>>();

builder.Services.AddScoped<IBaseRepository<ResourceForPrintJob>, BaseRepository<ResourceForPrintJob>>();
//builder.Services.AddScoped<IResourceForPrintJobService, ResourceForPrintJobService>();

builder.Services.AddScoped<IBaseRepository<ResourcePropertyDetail>, BaseRepository<ResourcePropertyDetail>>();
//builder.Services.AddScoped<IResourcePropertyDetailService, ResourcePropertyDetailService>();

builder.Services.AddScoped<IBaseRepository<ResourceProperty>, BaseRepository<ResourceProperty>>();
//builder.Services.AddScoped<IResourcePropertyService, ResourcePropertyService>();

builder.Services.AddScoped<IBaseRepository<Resources>, BaseRepository<Resources>>();
//builder.Services.AddScoped<IResourcesService, ResourcesService>();

builder.Services.AddScoped<IResourceManagementService, ResourceManagementService>();

builder.Services.AddScoped<IBaseRepository<Delivery>, BaseRepository<Delivery>>();
builder.Services.AddScoped<IBaseRepository<ShippingMethod>, BaseRepository<ShippingMethod>>();

builder.Services.AddScoped<IShippingService, ShippingService>();


var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
    };
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Auth API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter the token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type =ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthorization();

app.MapControllers();

app.Run();
