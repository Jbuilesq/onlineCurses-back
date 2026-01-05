    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using onlineCurses.Infrastructure.Data;
    using onlineCurses.Infrastructure.Repositories;
    using onlineCurses.Domain.Interfaces;
    using onlineCurses.Application.Services;
    using onlineCurses.Infrastructure;
    using Microsoft.OpenApi.Models;

    var builder = WebApplication.CreateBuilder(args);

    // Controllers + Swagger
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using Bearer scheme",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                Array.Empty<string>()
            }
        });
    });

    // DbContext con Pomelo
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            new MySqlServerVersion(new Version(8, 0, 36)),
            opts => opts.EnableRetryOnFailure()));

    // JWT
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };
        });
    // CORS - Permite que el frontend React (puerto 5173) acceda a la API
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactFrontend", policy =>
        {
            policy.WithOrigins("http://localhost:5173")   // Puerto de Vite/React
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials(); // Importante para cookies/JWT si usas credenciales
        });
    });
    

    // DI
    builder.Services.AddScoped<ICourseRepository, CourseRepository>();
    builder.Services.AddScoped<ILessonRepository, LessonRepository>();
    builder.Services.AddScoped<CourseService>();
    builder.Services.AddScoped<LessonService>();
    builder.Services.AddScoped<AuthService>();

    var app = builder.Build();

    // Migraciones + Seed
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        await DataSeeder.SeedAsync(db);
    }
    
    // CORS - 
    app.UseCors("AllowReactFrontend");
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();