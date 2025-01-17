﻿using MyApp.ServiceModel;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add DBContext
builder.Services.AddDbContext<UserDbContext>(
                    options => options.UseSqlServer(builder.Configuration.GetConnectionString("users-evo")),
                                                                                              ServiceLifetime.Singleton);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseServiceStack(new AppHost());
app.Run(async context => context.Response.Redirect("/metadata"));

app.Run();

