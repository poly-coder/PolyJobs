namespace FsPolyJobs.Domain

open System

type JobCommand =
    | JobCreate of JobCreate
    | JobUpdate of JobUpdate

and JobCreate =
    { title: string
      description: string option
      jobType: JobType
      stepType: StepType
      state: JobState option
      progress: int
      maxProgress: int
      status: JobStatus option
      meta: Map<string, string> }

and JobUpdate =
    { title: string
      description: string option
      stepType: StepType
      state: JobState option
      progress: int option
      maxProgress: int option
      status: JobStatus option
      meta: Map<string, string> }
