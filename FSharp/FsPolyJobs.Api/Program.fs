namespace FsPolyJobs.Api

open Microsoft.OpenApi.Models

#nowarn "20"

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddControllers()

        builder.Services.AddSwaggerGen (fun options ->
            let info =
                let info = OpenApiInfo()
                info.Title <- "Jobs API"
                info.Version <- "v1"
                info

            options.SwaggerDoc("v1", info))

        let app = builder.Build()

        //app.UseHttpsRedirection()

        app.UseAuthorization()
        app.UseSwagger()
        app.UseSwaggerUI(fun options -> options.SwaggerEndpoint("/swagger/v1/swagger.json", "Jobs API v1"))
        app.MapControllers()

        app.Run()

        exitCode
