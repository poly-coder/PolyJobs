namespace FsPolyJobs.Api

open System
open Swashbuckle.AspNetCore.Annotations
open System.ComponentModel.DataAnnotations
open System.Collections.Generic
open System.Text.Json.Serialization
open FsPolyJobs.Domain
open Validus

[<JsonConverter(typeof<JsonStringEnumConverter>)>]
type JobStatusEnum =
    | InProgress = 0
    | Success = 1
    | Failure = 2

type JobStatusModel =
    { ``type``: JobStatusEnum
      data: string }


type JobCreateRequest =
    { [<Required; MaxLength(100)>]
      title: string
      description: string
      [<Required>]
      jobType: string
      [<Required>]
      stepType: string
      payload: string
      progress: int Nullable
      maxProgress: int Nullable
      status: JobStatusModel
      meta: Dictionary<string, string> }
    member this.ToCommandInput() : ValidationResult<JobCreateInput> =
        validate {
            let! status =
                match box this.status with
                | null -> InProgressInput |> Ok
                | _ ->
                    match this.status.``type`` with
                    | JobStatusEnum.InProgress -> InProgressInput |> Ok
                    | JobStatusEnum.Success -> SuccessInput this.status.data |> Ok
                    | JobStatusEnum.Failure -> FailureInput this.status.data |> Ok
                    | _ ->
                        Error(
                            ValidationErrors.create
                                "status.type"
                                [ "Type must be one of InProgress, Success or Failure" ]
                        )

            let meta =
                match box this.meta with
                | null -> None
                | _ ->
                    this.meta
                    |> Seq.map (fun x -> x.Key, x.Value)
                    |> Map.ofSeq
                    |> Some

            return
                { title = this.title
                  description = this.description
                  jobType = this.jobType
                  stepType = this.stepType
                  payload = this.payload
                  progress = this.progress |> Option.ofNullable
                  maxProgress = this.maxProgress |> Option.ofNullable
                  status = status
                  meta = meta }
        }
