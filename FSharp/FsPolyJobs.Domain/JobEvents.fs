namespace FsPolyJobs.Domain

type JobEvent =
    | JobWasCreated of JobWasCreated
    | JobWasUpdated of JobWasUpdated

and JobWasCreated = {
    title: string
    description: string option
    jobType: JobType
    stepType: StepType
    state: JobState
    progress: int
    maxProgress: int
    status: JobStatus option
    meta: Map<string, string>
}

and JobWasUpdated = {
    title: string
    description: string option
    jobType: JobType
    stepType: StepType
    state: JobState option
    progress: int option
    maxProgress: int option
    status: JobStatus option
    meta: Map<string, string>
}
