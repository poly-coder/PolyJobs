module FsPolyJobs.Domain.CommandHandlers
open Validus

type JobState = JobStateData option

and JobStateData =
    { title: JobTitle
      description: JobDescription
      jobType: JobType
      stepType: JobType
      payload: JobUserData
      progress: JobProgress
      maxProgress: JobMaxProgress
      status: JobStatus
      meta: JobMetadata }

let applyEvents state event =
    match event, state with
    | JobWasCreated event, None ->
        Some
            { title = event.title
              description = event.description
              jobType = event.jobType
              stepType = event.stepType
              payload = event.payload
              progress = event.progress
              maxProgress = event.maxProgress
              status = event.status
              meta = event.meta }

    | JobWasCreated _, Some state -> Some state

    | JobWasUpdated event, Some state ->
        Some
            { state with
                stepType = event.stepType
                payload = event.payload
                progress = event.progress
                maxProgress = event.maxProgress
                status = event.status
                meta = event.meta }

    | JobWasUpdated _, None -> None

let handleCommand state command =
    match command, state with
    | JobCreate command, None ->
        let created = JobWasCreated {
              title = command.title
              description = command.description
              jobType = command.jobType
              stepType = command.stepType
              payload = command.payload
              progress = command.progress
              maxProgress = command.maxProgress
              status = command.status
              meta = command.meta }
        Ok [ created ]

    | JobCreate _, Some _ ->
        Error (ValidationErrors.create "command" [ "Job already exists" ])

    | JobUpdate command, Some state ->
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

            let updated = JobWasUpdated {
                stepType = command.stepType
                payload = command.payload
                progress = progress
                maxProgress = maxProgress
                status = command.status
                meta = command.meta }
            Ok [ updated ]

        | _ ->
            Error (ValidationErrors.create "command" [ "Job is already completed" ])

    | JobUpdate _, None ->
        Error (ValidationErrors.create "command" [ "Job does not exist" ])