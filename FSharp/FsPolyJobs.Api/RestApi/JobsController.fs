namespace FsPolyJobs.Api.RestApi

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Validus
open FsPolyJobs.Api
open FsToolkit.ErrorHandling
open RestApiPreamble

[<ApiController>]
[<Route("job")>]
type JobsController(logger: ILogger<JobsController>) =
    inherit ControllerBase()

    [<HttpPost("create", Name = "JobCreate")>]
    member this.Create([<FromBody>] body: JobCreateRequest) =
        task {
            let! result =
                taskResult {
                    let! input =
                        body.ToCommandInput()
                        |> Result.mapError BadRequestOfErrors

                    return input
                }

            return toActionResult result this
        }
