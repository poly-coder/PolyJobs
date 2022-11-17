namespace FsPolyJobs.Domain

type JobCommandInput =
    | JobCreateInput of JobCreateInput
    | JobUpdateInput of JobUpdateInput

and [<CLIMutable>] JobCreateInput =
    { title: string
      description: string
      jobType: string
      stepType: string
      payload: string
      progress: int option
      maxProgress: int option
      status: JobStatusInput
      meta: Map<string, string> option }

and [<CLIMutable>] JobUpdateInput =
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
