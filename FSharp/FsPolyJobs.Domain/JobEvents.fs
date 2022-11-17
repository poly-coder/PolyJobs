namespace FsPolyJobs.Domain

type JobEvent =
    | JobWasCreated of JobWasCreated
    | JobWasUpdated of JobWasUpdated

and JobWasCreated =
    { title: JobTitle
      description: JobDescription
      jobType: JobType
      stepType: JobType
      payload: JobUserData
      progress: JobProgress
      maxProgress: JobMaxProgress
      status: JobStatus
      meta: JobMetadata }

and JobWasUpdated =
    { stepType: JobType
      payload: JobUserData
      progress: JobProgress
      maxProgress: JobMaxProgress
      status: JobStatus
      meta: JobMetadata }
