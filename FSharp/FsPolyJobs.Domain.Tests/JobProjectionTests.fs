module FsPolyJobs.Domain.JobProjectionTests

open System
open Xunit
open Swensen.Unquote
open FsPolyJobs.Domain.Projection
open FsCheck.Xunit
open JobPreamble

[<Property>]
let ``projectEvent JobWasCreated on any record should project updates`` (event: JobWasCreated) (record: JobRecord) =
    let expectedState: JobRecordData =
        { title = event.title
          description = event.description
          jobType = event.jobType
          stepType = event.stepType
          payload = event.payload
          progress = event.progress
          maxProgress = event.maxProgress
          status = event.status
          meta = event.meta }

    let result =
        projectEvent record (JobWasCreated event)

    test <@ result = Some expectedState @>

[<Property>]
let ``projectEvent JobWasUpdated on existing record should project updates``
    (event: JobWasUpdated)
    (record: JobRecordData)
    =
    let expectedState: JobRecordData =
        { record with
            stepType = event.stepType
            payload = event.payload
            progress = event.progress
            maxProgress = event.maxProgress
            status = event.status
            meta = event.meta }

    let result =
        projectEvent (Some record) (JobWasUpdated event)

    test <@ result = Some expectedState @>

[<Property>]
let ``projectEvent JobWasUpdated on new record should not project any record``
    (event: JobWasUpdated)
    =
    let result =
        projectEvent None (JobWasUpdated event)

    test <@ result = None @>
