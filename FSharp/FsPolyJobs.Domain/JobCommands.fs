namespace FsPolyJobs.Domain

open Validus

type JobCommand =
    | JobCreate of JobCreate
    | JobUpdate of JobUpdate
    static member Create: Validator<JobCommandInput, JobCommand> =
        fun field input ->
            validate {
                match input with
                | JobCreateInput input ->
                    let! input = input |> JobCreate.Create (field + ".create")
                    return JobCreate input
                | JobUpdateInput input ->
                    let! input = input |> JobUpdate.Create (field + ".update")
                    return JobUpdate input
            }

and JobCreate =
    { title: JobTitle
      description: JobDescription
      jobType: JobType
      stepType: JobType
      payload: JobUserData
      progress: JobProgress
      maxProgress: JobMaxProgress
      status: JobStatus
      meta: JobMetadata }
    static member Create: Validator<JobCreateInput, JobCreate> =
        fun field input ->
            validate {
                let! title = input.title |> JobTitle.Create (field + ".title")
                and! description = input.description |> JobDescription.Create (field + ".description")
                and! jobType = input.jobType |> JobType.Create (field + ".jobType")
                and! stepType = input.stepType |> JobType.Create (field + ".stepType")
                and! payload = input.payload |> JobUserData.Create (field + ".payload")
                and! progress = input.progress |> JobProgress.Create (field + ".progress")
                and! maxProgress = input.maxProgress |> JobMaxProgress.Create (field + ".maxProgress")
                and! status = input.status |> JobStatus.Create (field + ".status")
                and! meta = input.meta |> JobMetadata.Create (field + ".meta")

                return {
                    title = title
                    description = description
                    jobType = jobType
                    stepType = stepType
                    payload = payload
                    progress = progress
                    maxProgress = maxProgress
                    status = status
                    meta = meta
                }
            }

and JobUpdate =
    { stepType: JobType
      payload: JobUserData
      progress: JobProgress option
      maxProgress: JobMaxProgress option
      status: JobStatus
      meta: JobMetadata }
    static member Create: Validator<JobUpdateInput, JobUpdate> =
        fun field input ->
            validate {
                let! stepType = input.stepType |> JobType.Create (field + ".stepType")
                and! payload = input.payload |> JobUserData.Create (field + ".payload")
                and! progress = input.progress |> JobProgress.CreateOptional (field + ".progress")
                and! maxProgress = input.maxProgress |> JobMaxProgress.CreateOptional (field + ".maxProgress")
                and! status = input.status |> JobStatus.Create (field + ".status")
                and! meta = input.meta |> JobMetadata.Create (field + ".meta")

                return {
                    stepType = stepType
                    payload = payload
                    progress = progress
                    maxProgress = maxProgress
                    status = status
                    meta = meta
                }
            }
