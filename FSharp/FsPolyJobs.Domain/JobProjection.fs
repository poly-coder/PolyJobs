module FsPolyJobs.Domain.Projection

type JobRecord = JobRecordData option

and JobRecordData =
    { title: JobTitle
      description: JobDescription
      jobType: JobType
      stepType: JobType
      payload: JobUserData
      progress: JobProgress
      maxProgress: JobMaxProgress
      status: JobStatus
      meta: JobMetadata }

module internal Utils =
    let projectJobWasCreated (event: JobWasCreated) : JobRecord =
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

    let projectJobWasUpdated (record: JobRecordData) (event: JobWasUpdated) : JobRecord =
        Some
            { record with
                  stepType = event.stepType
                  payload = event.payload
                  progress = event.progress
                  maxProgress = event.maxProgress
                  status = event.status
                  meta = event.meta }

open Utils

let projectEvent (record: JobRecord) (event: JobEvent) =
    match event, record with
    | JobWasCreated event, _ -> projectJobWasCreated event

    | JobWasUpdated event, Some record -> projectJobWasUpdated record event
    | JobWasUpdated _, None -> None
