using DefNotEbay_API.Data;
using DefNotEbay_API.Recommender;
using DefNotEbay_API.Seeding;
using DefNotEbay_API.Services;
using DefNotEbay_API.Services.Interfaces;
using DefNotEbay_API.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");
;

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlServer(connection);
    }
    else
    {
        options.UseAzureSql(connection);
    }
});

builder.Services.AddAutoMapper(cfg => {}, AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen((c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Definetely-Not Ebay", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: \"Bearer eyJhbGciOiJI...\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
}));

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key Not Found");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        options.MapInboundClaims = false;
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuctionService, AuctionService>();
builder.Services.AddScoped<IBidService, BidService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IClickTrackService, TrackService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IAdminStatsService, AdminStatsService>();
builder.Services.AddScoped<IExportService, ExportService>();

builder.Services.AddScoped<IAuctionExpiryService, AuctionExpiryService>();
// builder.Services.AddHostedService<AuctionExpiryWorker>();
builder.Services.AddScoped<ExplicitMfRecommenderService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // await db.Database.MigrateAsync();

    if (!await db.Users.AnyAsync())
    {
        await SeedData.SeedAsync(db);
    }
}


// using (var scope = app.Services.CreateScope())
// {
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//     var shouldTrain =
//     !db.UserRecommendations.Any() ||
//     db.UserRecommendations.Max(x => x.GeneratedAt)
//         < DateTime.UtcNow.AddDays(-7);

//     if (shouldTrain){
//         var svc = scope.ServiceProvider.GetRequiredService<ExplicitMfRecommenderService>();
//         var since = DateTime.UtcNow.AddDays(-360);
//         await svc.TrainAndPersistAsync(since, topN: 50);
//     }
// }


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy"
}));

app.Run();




