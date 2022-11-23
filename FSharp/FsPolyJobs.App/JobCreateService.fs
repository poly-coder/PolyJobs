namespace FsPolyJobs.App

open FsPolyJobs.Domain
open FsToolkit.ErrorHandling

type IJobCreateService =
    abstract member Create : JobCreateInput -> Async<Result<JobCommandOutput, JobCommandError>>

type JobCreateService(idGen: IIdGen) =
    interface IJobCreateService with
        member this.Create input =
            asyncResult {
                let id = idGen.NewId

                return { id = id }
            }
