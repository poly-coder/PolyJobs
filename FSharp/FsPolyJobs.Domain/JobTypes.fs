namespace FsPolyJobs.Domain

type JobType = string
type StepType = string
type JobState = string
type JobSuccess = string
type JobFailure = string

type JobStatus =
    | Success of JobSuccess option
    | Failure of JobFailure option

//type JobData = {
//    title: string
//    description: string option
//    jobType: JobType
//    state: JobState
//    progress: int
//    maxProgress: int
//    status: JobStatus option
//    meta: Map<string, string>
//}

//and JobStepData = {
//    title: string
//    description: string option
//    stepType: StepType
//    state: JobState option
//    progress: JobProgressDelta
//    maxProgress: int option
//    status: JobStatus option
//    meta: Map<string, string>
//}

