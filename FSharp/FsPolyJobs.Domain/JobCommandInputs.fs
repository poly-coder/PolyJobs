namespace FsPolyJobs.Domain

open System

type JobCommandInput =
    | JobCreateInput of JobCreateInput
    | JobUpdateInput of JobUpdateInput

and [<CLIMutable>] JobCreateInput =
    { title: string option
      description: string option
      jobType: string option
      stepType: string option
      state: string option
      progress: int option
      maxProgress: int option
      status: JobStatusInput option
      meta: Map<string, string> option }

and [<CLIMutable>] JobUpdateInput =
    { title: string option
      description: string option
      stepType: string option
      state: string option
      progress: int option
      maxProgress: int option
      status: JobStatusInput option
      meta: Map<string, string> option }

and JobStatusInput =
    | SuccessInput of string option
    | FailureInput of string option
