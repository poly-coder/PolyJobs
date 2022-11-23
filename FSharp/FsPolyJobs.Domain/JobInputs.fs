namespace FsPolyJobs.Domain

open Validus

type JobCommandInput =
    | JobCreateInput of JobCreateInput
    | JobUpdateInput of JobUpdateInput

and JobCreateInput =
    { title: string
      description: string
      jobType: string
      stepType: string
      payload: string
      progress: int option
      maxProgress: int option
      status: JobStatusInput
      meta: Map<string, string> option }

and JobUpdateInput =
    { stepType: string
      payload: string
      progress: int option
      maxProgress: int option
      status: JobStatusInput
      meta: Map<string, string> option }

and JobStatusInput =
    | InProgressInput
    | SuccessInput of string
    | FailureInput of string

type JobCommandOutput = { id: string }

type JobCommandError =
    | JobValidationError of ValidationErrors
    | JobNotFound of id: string
