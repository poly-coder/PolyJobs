module FsPolyJobs.Domain.JobCommandHandlersTests

open System
open Xunit
open Swensen.Unquote
open Validus
open FsPolyJobs.Domain.CommandHandlers
open FsCheck.Xunit

[<Property>]
let ``applyEvent JobWasCreated on any state should apply updates`` (event: JobWasCreated) (state: JobState) =
    let expectedState: JobStateData =
        { progress = event.progress
          maxProgress = event.maxProgress
          status = event.status }

    let result = applyEvent state (JobWasCreated event)

    test <@ result = Some expectedState @>

[<Property>]
let ``applyEvent JobWasUpdated on new state should skip updates`` (event: JobWasUpdated) =
    let result = applyEvent None (JobWasUpdated event)

    test <@ result = None @>

[<Property>]
let ``applyEvent JobWasUpdated on existing state should apply updates`` (event: JobWasUpdated) (state: JobStateData) =
    let expectedState: JobStateData =
        { state with
            progress = event.progress
            maxProgress = event.maxProgress
            status = event.status }

    let result =
        applyEvent (Some state) (JobWasUpdated event)

    test <@ result = Some expectedState @>

[<Property>]
let ``handleCommand JobCreate on existing state should return error`` (command: JobCreate) (state: JobStateData) =
    let result =
        handleCommand (Some state) (JobCreate command)

    test <@ result = jobAlreadyExists @>

[<Property>]
let ``handleCommand JobCreate on new state should return events`` (command: JobCreate) =
    let result = handleCommand None (JobCreate command)

    let expectedEvents =
        [ JobWasCreated
              { title = command.title
                description = command.description
                jobType = command.jobType
                stepType = command.stepType
                payload = command.payload
                progress = command.progress
                maxProgress = command.maxProgress
                status = command.status
                meta = command.meta } ]

    test <@ result = Ok expectedEvents @>

[<Property>]
let ``handleCommand JobUpdate on new state should return error`` (command: JobUpdate) =
    let result = handleCommand None (JobUpdate command)

    test <@ result = jobDoesNotExists @>

[<Property>]
let ``handleCommand JobUpdate on InProgress state should return events`` (command: JobUpdate) (state: JobStateData) =
    let state = { state with status = InProgress }

    let maxProgress =
        match command.maxProgress with
        | Some maxProgress -> maxProgress
        | None -> state.maxProgress

    let progress =
        match command.progress with
        | Some progress -> progress
        | None -> state.progress

    let result =
        handleCommand (Some state) (JobUpdate command)

    let expectedEvents =
        [ JobWasUpdated
              { stepType = command.stepType
                payload = command.payload
                progress = progress
                maxProgress = maxProgress
                status = command.status
                meta = command.meta } ]

    test <@ result = Ok expectedEvents @>

[<Property>]
let ``handleCommand JobUpdate on Success state should return error`` (command: JobUpdate) (state: JobStateData) (success: JobUserData) =
    let state = { state with status = Success success }

    let result =
        handleCommand (Some state) (JobUpdate command)

    test <@ result = jobAlreadyCompleted @>

[<Property>]
let ``handleCommand JobUpdate on Failure state should return error`` (command: JobUpdate) (state: JobStateData) (failure: JobUserData) =
    let state = { state with status = Failure failure }

    let result =
        handleCommand (Some state) (JobUpdate command)

    test <@ result = jobAlreadyCompleted @>
