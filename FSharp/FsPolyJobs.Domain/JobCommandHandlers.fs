module FsPolyJobs.Domain.CommandHandlers

open Validus
open JobPreamble

type JobState = JobStateData option

and JobStateData =
    { progress: JobProgress
      maxProgress: JobMaxProgress
      status: JobStatus
      meta: JobMetadata }

let jobAlreadyExists: ValidationResult<JobEvent list> =
    Error(ValidationErrors.create "command" [ "Job already exists" ])

let jobDoesNotExists: ValidationResult<JobEvent list> =
    Error(ValidationErrors.create "command" [ "Job does not exist" ])

let jobAlreadyCompleted: ValidationResult<JobEvent list> =
    Error(ValidationErrors.create "command" [ "Job is already completed" ])

module internal Utils =
    let applyJobWasCreated (event: JobWasCreated) : JobState =
        Some
            { progress = event.progress
              maxProgress = event.maxProgress
              status = event.status
              meta = event.meta }

    let applyJobWasUpdated state (event: JobWasUpdated) : JobState =
        Some
            { state with
                progress = event.progress
                maxProgress = event.maxProgress
                status = event.status
                meta = event.meta }

    let handleJobCreate (command: JobCreate) : ValidationResult<JobEvent list> =
        let created =
            JobWasCreated
                { title = command.title
                  description = command.description
                  jobType = command.jobType
                  stepType = command.stepType
                  payload = command.payload
                  progress = command.progress
                  maxProgress = command.maxProgress
                  status = command.status
                  meta = command.meta }

        Ok [ created ]

    let handleJobUpdate (state: JobStateData) (command: JobUpdate) : ValidationResult<JobEvent list> =
        match state.status with
        | InProgress ->
            let maxProgress =
                match command.maxProgress with
                | Some maxProgress -> maxProgress
                | None -> state.maxProgress

            let progress =
                match command.progress with
                | Some progress -> progress
                | None -> state.progress

            let meta =
                addMap state.meta.value command.meta.value
                |> fun value -> { value = value }

            let updated =
                JobWasUpdated
                    { stepType = command.stepType
                      payload = command.payload
                      progress = progress
                      maxProgress = maxProgress
                      status = command.status
                      meta = meta }

            Ok [ updated ]

        | Success _ -> jobAlreadyCompleted
        | Failure _ -> jobAlreadyCompleted

open Utils

let applyEvent state event =
    match event, state with
    | JobWasCreated event, _ -> applyJobWasCreated event

    | JobWasUpdated event, Some state -> applyJobWasUpdated state event
    | JobWasUpdated _, None -> None

let handleCommand state command =
    match command, state with
    | JobCreate command, None -> handleJobCreate command
    | JobCreate _, Some _ -> jobAlreadyExists

    | JobUpdate command, Some state -> handleJobUpdate state command
    | JobUpdate _, None -> jobDoesNotExists
