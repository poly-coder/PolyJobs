module FsPolyJobs.Domain.Validations

open FsToolkit.ErrorHandling
open Validus
open System

module internal Utils =
    let validateRequiredBetweenLen min max field =
        function
        | None -> Check.String.notEmpty field ""
        | Some value -> Check.String.betweenLen min max field value

    let validateOptionalLessThanLen max field =
        function
        | None -> Ok None
        | Some value ->
            Check.String.lessThanLen max field value
            |> Result.map Some

    let validateType = validateRequiredBetweenLen 1 32

    let validateUserData = validateOptionalLessThanLen 0x10000

    let validateMaxProgress field =
        function
        | None -> Ok 100
        | Some value -> Check.Int.greaterThan 0 field value

    let validateOptionalMaxProgress field =
        function
        | None -> Ok None
        | Some value -> Check.Int.greaterThan 0 field value |> Result.map Some

    let validateProgress field =
        function
        | None -> Ok 0
        | Some value -> Check.Int.greaterThan -1 field value

    let validateOptionalProgress field =
        function
        | None -> Ok None
        | Some value -> Check.Int.greaterThan -1 field value |> Result.map Some

    let validationMap fn =
        function
        | Ok value -> Ok(fn value)
        | Error e -> Error e

open Utils

// ValueTypes
let validateJobType = validateType "jobType"
let validateStepType = validateType "stepType"

let validateJobState = validateUserData "state"
let validateJobSuccess = validateUserData "success"
let validateJobFailure = validateUserData "failure"

let validateTitle = validateRequiredBetweenLen 1 100 "title"

let validateDescription =
    validateOptionalLessThanLen 0x10000 "description"

let validateMaxProgress = validateMaxProgress "maxProgress"
let validateOptionalMaxProgress = validateOptionalMaxProgress "maxProgress"
let validateProgress = validateProgress "progress"
let validateOptionalProgress = validateOptionalProgress "progress"

let validateJobStatus =
    function
    | None -> Ok None
    | Some (SuccessInput success) ->
        validateJobSuccess success
        |> Result.map (Option.map (Some >> Success))
    | Some (FailureInput failure) ->
        validateJobFailure failure
        |> Result.map (Option.map (Some >> Failure))

let validateMeta =
    function
    | None -> Ok Map.empty
    | Some meta -> Ok meta

// Commands

let validateJobCreate (input: JobCreateInput) : ValidationResult<JobCreate> =
    validate {
        let! title = validateTitle input.title
        and! description = validateDescription input.description
        and! jobType = validateJobType input.jobType
        and! stepType = validateStepType input.stepType
        and! state = validateJobState input.state
        and! progress = validateProgress input.progress
        and! maxProgress = validateMaxProgress input.maxProgress
        and! status = validateJobStatus input.status
        and! meta = validateMeta input.meta
        
        let command: JobCreate = {
            title = title
            description = description
            jobType = jobType
            stepType = stepType
            state = state
            progress = progress
            maxProgress = maxProgress
            status = status
            meta = meta
        }

        return command
    }

let validateJobUpdate (input: JobUpdateInput) : ValidationResult<JobUpdate> =
    validate {
        let! title = validateTitle input.title
        and! description = validateDescription input.description
        and! stepType = validateStepType input.stepType
        and! state = validateJobState input.state
        and! progress = validateOptionalProgress input.progress
        and! maxProgress = validateOptionalMaxProgress input.maxProgress
        and! status = validateJobStatus input.status
        and! meta = validateMeta input.meta
        
        let command: JobUpdate = {
            title = title
            description = description
            stepType = stepType
            state = state
            progress = progress
            maxProgress = maxProgress
            status = status
            meta = meta
        }

        return command
    }
