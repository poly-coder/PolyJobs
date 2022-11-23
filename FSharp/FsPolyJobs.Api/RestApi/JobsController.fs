namespace FsPolyJobs.Api.RestApi

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Swashbuckle.AspNetCore.Annotations
open FsToolkit.ErrorHandling
open Validus
open FsPolyJobs.Domain
open FsPolyJobs.App
open FsPolyJobs.Api
open RestApiPreamble

[<ApiController>]
[<Route("job")>]
type JobsController(logger: ILogger<JobsController>) =
    inherit ControllerBase()

    let mapJobError = function
        | JobValidationError errors -> BadRequestOfErrors errors
        | JobNotFound id -> NotFoundEntity(id, "Job")

    [<HttpPost("create", Name = "JobCreate")>]
    [<SwaggerResponse(200, "Returns the identifier of the new Job", typeof<JobCommandResponse>)>]
    [<SwaggerResponse(400, "Validation error", typeof<ValidationProblemDetails>)>]
    [<SwaggerResponse(500, "Service error", typeof<ProblemDetails>)>]
    member this.Create([<FromBody>] body: JobCreateRequest, [<FromServices>] createService: IJobCreateService) =
        task {
            let! result =
                taskResult {
                    let! input =
                        body.ToCommandInput()
                        |> Result.mapError BadRequestOfErrors
                        
                    let! output =
                        createService.Create(input)
                        |> AsyncResult.mapError mapJobError

                    return JobCommandResponse.FromOutput output
                }

            return toActionResult result this
        }
